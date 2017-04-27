using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ConvertCSVToXmlOrchard
{
    public class CsvToXmlModified
    {
        /// <summary>
        /// Conversion Method
        /// </summary>
        /// <param name="csvString">cvs string to converted</param>
        /// <param name="separatorField">separator used by the csv file</param>
        ///   /// <param name="filename">file name </param>
        /// <returns>XDocument with the content of csv file in Xml Format</returns>
        public static XDocument ConvertCsvToXML(string csvString, string filename)
        {
            string dirName = new DirectoryInfo(filename).Name.Replace(".csv", "");

            //split the rows
            var sep = new[] { "\r\n" };
            string[] rows = csvString.Split(sep, StringSplitOptions.RemoveEmptyEntries);
            //Create the declaration
            var xsurvey = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
            XComment comm = new XComment("Exported from Orchard"); // Create the Comment
            var xhead = new XElement("Orchard");// Create the head

            var xconD = new XElement("ContentDefinition"); //Create the ContentDefinition

            var xroot = new XElement("Content"); //Create the Content

            for (int i = 0; i < rows.Length; i++)
            {
                //Create each row
                if (i > 0)
                {
                    //string dirName = new DirectoryInfo(filename).Name.Replace(".csv","");
                    xroot.Add(rowCreator(rows[i], rows[0], dirName));

                }
            }

            xconD.Add(Type(dirName)/*, Part(rows[0], dirName)*/);
            xhead.Add(Reciperow(), xconD, xroot);
            xsurvey.Add(comm, xhead);
            return xsurvey;
        }

        private static XElement Reciperow()
        {
            var xrecipe = new XElement("Recipe");// Create the recipe
            var xrecip = new XElement("ExportUtc", DateTime.UtcNow);
            xrecipe.Add(xrecip);
            return xrecipe;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Filename"></param>
        /// <returns></returns>
        private static XElement Type(string Filename)
        {
            var xtype = new XElement("Types");

            var xfilenamect = new XElement(Filename, new XAttribute("ContentTypeSettings.Creatable", "True"), new XAttribute("ContentTypeSettings.Draftable", "True"),
                new XAttribute("ContentTypeSettings.Listable", "True"), new XAttribute("ContentTypeSettings.Securable", "True"),
                new XAttribute("TypeIndexing.Indexes", Filename), new XAttribute("ContentTypeLayoutSettings.Placeable", "False")
                , new XAttribute("DisplayName", Filename)); //Create the contenttypes

            var xfilenamecp = new XElement(Filename + 1); //Create the contentpart
            var comp = new XElement("CommonPart", new XAttribute("DateEditorSettings.ShowDateEditor", "False"), new XAttribute("OwnerEditorSettings.ShowOwnerEditor", "True"));
            var xIdenP = new XElement("IdentityPart"); //Create the IdentityPart
            var xTitleP = new XElement("TitlePart"); //Create the TitlePart

            xfilenamect.Add(xfilenamecp, comp, xIdenP, xTitleP);
            xtype.Add(xfilenamect);

            return xtype;
        }

        /// <summary>
        /// Private. Take a csv line and convert in a row - var node
        /// with the fields values as attributes. 
        /// </summary>
        /// <param name="row">csv row to process</param>
        /// <param name="firstRow">First row with the fields names</param>
        /// <param name="separatorField">separator string use in the csv fields</param>
        /// <param name="FileName">string name file replace .csv </param>
        /// <returns>XElement with the csv information of the inputed row</returns>
        private static XElement rowCreator(string row, string firstRow, string FileName)
        {
            //var sep = new[] { "\t" };
            string[] Text = SplitCSV(row);
            string[] name = SplitCSV(firstRow.Replace(" ", ""));
            string id = Guid.NewGuid().ToString().Replace("-", "");

            var xrow = new XElement(FileName, new XAttribute("Id", $"/Identifier={id}"), new XAttribute("Status", "Published"));
            XElement Identifier = new XElement("IdentityPart", new XAttribute("Identifier", id));
            var common = new XElement("CommonPart", new XAttribute("Owner", "/User.UserName=admin"), new XAttribute("CreatedUtc", DateTime.UtcNow));
            xrow.Add(Identifier, common);


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
