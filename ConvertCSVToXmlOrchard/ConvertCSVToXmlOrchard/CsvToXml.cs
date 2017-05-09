using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using CsvHelper;


namespace ConvertCSVToXmlOrchard
{

    public class CsvToXml
    {
        private static long index = 1;

        public static XDocument ConvertCsvToXML(string csvString, string filename)
        {
            index = 1;

            string dirName = new DirectoryInfo(filename).Name.Replace(".csv", "");

            //split the rows
            var sep = new[] { "\r\n" };
            string[] rows = csvString.Split(sep, StringSplitOptions.RemoveEmptyEntries);

            //Create the element
            var xsyntax = new XDocument(new XDeclaration("1.0", "UTF-8", "yes")); //<?xml version="1.0" encoding="utf-8" standalone="yes"?>

            XComment comm = new XComment("Exported from Orchard"); // Create the Comment --->   <!--Exported from Orchard-->

            var xHead = new XElement("Orchard");// Create the head  -->  <Orchard>

            var xContentDe = new XElement("ContentDefinition"); //Create the ContentDefinition -->  <ContentDefinition>

            var xContent = new XElement("Content"); //Create the Content -->  <Content>

            for (int i = 0; i < rows.Length; i++)
            {


                if (i > 0)
                {
                    xContent.Add(Content(rows[i], rows[0], dirName));

                }
            }

            xContentDe.Add(Type(dirName)/*, Part(rows[0], dirName)*/);
            xHead.Add(Reciperow(), xContentDe, xContent);
            xsyntax.Add(comm, xHead);
            return xsyntax;
        }

        private static XElement Reciperow()
        {
            var xRecipe = new XElement("Recipe");// Create the recipe
            var xExportUtc = new XElement("ExportUtc", DateTime.UtcNow);
            xRecipe.Add(xExportUtc);
            return xRecipe;
        }


        private static XElement Type(string Filename)
        {
            var xTypes = new XElement("Types");

            var xattendeestypes = new XElement(Filename, new XAttribute("ContentTypeSettings.Creatable", "True"), new XAttribute("ContentTypeSettings.Draftable", "True"),
                new XAttribute("ContentTypeSettings.Listable", "True"), new XAttribute("ContentTypeSettings.Securable", "True"),
                new XAttribute("TypeIndexing.Indexes", Filename), new XAttribute("ContentTypeLayoutSettings.Placeable", "False")
                , new XAttribute("DisplayName", Filename)); //Create the contenttypes

            var xattendees1 = new XElement(Filename + 1); //Create --> <attendees1 />

            var CommonPart = new XElement("CommonPart", new XAttribute("DateEditorSettings.ShowDateEditor", "False"), new XAttribute("OwnerEditorSettings.ShowOwnerEditor", "True"));

            var xIdentityPart = new XElement("IdentityPart"); //Create --> <IdentityPart />

            var xTitlePart = new XElement("TitlePart"); //Create -->  <TitlePart />

            xattendeestypes.Add(xattendees1, CommonPart, xIdentityPart, xTitlePart);

            xTypes.Add(xattendeestypes);

            return xTypes;
        }



        private static XElement Content(string row, string firstRow, string FileName)
        {
            //var sep = new[] { "\t" };
            string[] Text = SplitCSV(row);
            string[] name = SplitCSV(firstRow.Replace(" ", ""));
            string id = Guid.NewGuid().ToString().Replace("-", "");


            var xrow = new XElement(FileName, new XAttribute("Id", $"/Identifier={id}"), new XAttribute("Status", "Published"));

            XElement Identifier = new XElement("IdentityPart", new XAttribute("Identifier", id));

            XElement ImportID = new XElement("ImportID", new XAttribute("Text", index));

            var commonP = new XElement("CommonPart", new XAttribute("Owner", "/User.UserName=admin"), new XAttribute("CreatedUtc", DateTime.UtcNow));

            xrow.Add(Identifier, ImportID, commonP);

            index = index + 1;

            for (int i = 0; i < Text.Length; i++)
            {
                //Create the element var and Attributes with the field name and value
                var xvar = new XElement($"TextField.{ FirstCharToUpper(name[i].ToLower())}",
                                        new XAttribute("Text", Text[i]));
                xrow.Add(xvar);
            }
            return xrow;
        }

        public static string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
            {
                throw new ArgumentException("ARGH!");
            }
            return input.First().ToString().ToUpper() + input.Substring(1);
        }

        protected static string[] SplitCSV(string line)
        {
            System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace | System.Text.RegularExpressions.RegexOptions.Multiline)
        | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            Regex reg = new Regex("(?:^|,)(\\\"(?:[^\\\"]+|\\\"\\\")*\\\"|[^,]*)", options);
            MatchCollection coll = reg.Matches(line);
            string[] items = new string[coll.Count];
            int i = 0;
            foreach (Match m in coll)
            {
                items[i++] = m.Groups[0].Value.Trim('"').Trim(',').Trim('"').Trim();
            }
            return items;
        }
    }
}
