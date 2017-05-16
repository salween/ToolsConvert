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
using System.Diagnostics;

namespace ConvertCSVToXmlOrchard
{

    public class CsvToXml
    {
        //private static int index = 1;
        private static List<string> listOfValueFromDatabase = new List<string>();

        public static XDocument ConvertCsvToXML(List<Dictionary<string, string>> dataContents, string filename)
        {
            //index = 1;

            string dirName = new DirectoryInfo(filename).Name.Replace(".csv", "");

            //Create the element
            var xsyntax = new XDocument(new XDeclaration("1.0", "UTF-8", "yes")); //<?xml version="1.0" encoding="utf-8" standalone="yes"?>

            XComment comm = new XComment("Exported from Orchard"); // Create the Comment --->   <!--Exported from Orchard-->

            var xHead = new XElement("Orchard");// Create the head  -->  <Orchard>

            var xContentDe = new XElement("ContentDefinition"); //Create the ContentDefinition -->  <ContentDefinition>

            var xContent = new XElement("Content"); //Create the Content -->  <Content>

            // load previous data from database
            LoadFromDataBase();

            for (int i = 0; i < dataContents.Count; i++)
            {
                xContent.Add(Content(dataContents[i], dirName));
            }

            xContentDe.Add(Type(dirName)/*, Part(rows[0], dirName)*/);
            xHead.Add(Reciperow(), xContentDe, xContent);
            // save import id and identity 
            SaveToDataBase(xContent);
            //
            xsyntax.Add(comm, xHead);
            return xsyntax;
        }

        private static void SaveToDataBase(XElement content)
        {
            //listOfValueFromDatabase to database
            // save all content in format 'importid={id},identity={identity}'
        }

        private static void LoadFromDataBase()
        {
            //listOfValueFromDatabase
            // listOfValueFromDatabase[i] is "importid=0001,identity=as932344"
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



        private static XElement Content(Dictionary<string, string> columns, string FileName)
        {
            string id = Guid.NewGuid().ToString().Replace("-", "");

            // check from database
            string checkdata = $"{Environment.CurrentDirectory}\\Database\\XmlDatabase.xml";

            if (File.Exists(checkdata))
            {
                //var modified = XElement.Load(checkdata);
                //var newxml = Descendants("Orchard").Descendants("Content").Elements("attendees");
                //var oldxml = modified.Element("Content").Element("attendees");

                //foreach (var item in newxml)
                //{
                //    var itemx = oldxml.(x => x.Element("TextField.Firstname").Attribute("Text").Value == item.Element("TextField.Firstname")
                //    .Attribute("Text").Value && x.Element("TextField.Lastname").Attribute("Text").Value == item.Element("TextField.Lastname")
                //    .Attribute("Text").Value && x.Element("TextField.Companyname").Attribute("Text").Value == item.Element("TextField.Companyname")
                //    .Attribute("Text").Value && x.Element("TextField.Email").Attribute("Text").Value == item.Element("TextField.Email")
                //    .Attribute("Text").Value && x.Element("TextField.Phone").Attribute("Text").Value == item.Element("TextField.Phone")
                //    .Attribute("Text").Value);

                //    if (itemx != null)
                //    {
                //        item.Element("TextField.Firstname").Attribute("Text").Value = itemx.Element("TextField.Firstname").Attribute("Text").Value;
                //        item.Element("TextField.Lastname").Attribute("Text").Value = itemx.Element("TextField.Lastname").Attribute("Text").Value;
                //        item.Element("TextField.Companyname").Attribute("Text").Value = itemx.Element("TextField.Companyname").Attribute("Text").Value;
                //        item.Element("TextField.Title").Attribute("Text").Value = itemx.Element("TextField.Title").Attribute("Text").Value;
                //        item.Element("TextField.Mailcity").Attribute("Text").Value = itemx.Element("TextField.Mailcity").Attribute("Text").Value;
                //        item.Element("TextField.Mailstate").Attribute("Text").Value = itemx.Element("TextField.Mailstate").Attribute("Text").Value;
                //        item.Element("TextField.Phone").Attribute("Text").Value = itemx.Element("TextField.Phone").Attribute("Text").Value;
                //        item.Element("TextField.Email").Attribute("Text").Value = itemx.Element("TextField.Email").Attribute("Text").Value;
                //        item.Element("TextField.Webpage").Attribute("Text").Value = itemx.Element("TextField.Webpage").Attribute("Text").Value;
                //        item.Element("TextField.Importid").Attribute("Text").Value = itemx.Element("TextField.Importid").Attribute("Text").Value;
                //        item.Element("TextField.Spouse").Attribute("Text").Value = itemx.Element("TextField.Spouse").Attribute("Text").Value;
                //        item.Element("TextField.Guest").Attribute("Text").Value = itemx.Element("TextField.Guest").Attribute("Text").Value;
                //        item.Element("TextField.First-time").Attribute("Text").Value = itemx.Element("TextField.First-time").Attribute("Text").Value;

                //    }
                //    else
                //    {
                //        item.Element("TextField.Firstname").Attribute("Text").Value = item.Element("TextField.Firstname").Attribute("Text").Value;
                //        item.Element("TextField.Lastname").Attribute("Text").Value = item.Element("TextField.Lastname").Attribute("Text").Value;
                //        item.Element("TextField.Companyname").Attribute("Text").Value = item.Element("TextField.Companyname").Attribute("Text").Value;
                //        item.Element("TextField.Title").Attribute("Text").Value = item.Element("TextField.Title").Attribute("Text").Value;
                //        item.Element("TextField.Mailcity").Attribute("Text").Value = item.Element("TextField.Mailcity").Attribute("Text").Value;
                //        item.Element("TextField.Mailstate").Attribute("Text").Value = item.Element("TextField.Mailstate").Attribute("Text").Value;
                //        item.Element("TextField.Phone").Attribute("Text").Value = item.Element("TextField.Phone").Attribute("Text").Value;
                //        item.Element("TextField.Email").Attribute("Text").Value = item.Element("TextField.Email").Attribute("Text").Value;
                //        item.Element("TextField.Webpage").Attribute("Text").Value = item.Element("TextField.Webpage").Attribute("Text").Value;
                //        item.Element("TextField.Importid").Attribute("Text").Value = item.Element("TextField.Importid").Attribute("Text").Value;
                //        item.Element("TextField.Spouse").Attribute("Text").Value = item.Element("TextField.Spouse").Attribute("Text").Value;
                //        item.Element("TextField.Guest").Attribute("Text").Value = item.Element("TextField.Guest").Attribute("Text").Value;
                //        item.Element("TextField.First-time").Attribute("Text").Value = item.Element("TextField.First-time").Attribute("Text").Value;

                //    }
                //}
            }

            // if import id = 0001 then get identity from `importid=0001,identity=as932344` that mean identity is as932344
            // if import id not equal 0001 in any rows from database then generate a new one.

            var xrow = new XElement(FileName, new XAttribute("Id", $"/Identifier={id}"), new XAttribute("Status", "Published"));

            XElement Identifier = new XElement("IdentityPart", new XAttribute("Identifier", id));

            //XElement ImportID = new XElement("ImportID", new XAttribute("Text", index));

            var commonP = new XElement("CommonPart", new XAttribute("Owner", "/User.UserName=admin"), new XAttribute("CreatedUtc", DateTime.UtcNow));

            xrow.Add(Identifier, commonP);

            //index = index + 1;

            string[] keys = columns.Keys.ToArray();
            // looping for each column in data dic
            for (int i = 0; i < columns.Keys.Count; i++)
            {
                string key = keys[i]; // get current key at curent index (i)
                string value = columns[key]; // get current value that pointing to current key
                // if key = importid then check to database
                // if import id = 0001 then get identity from `importid=0001,identity=as932344` that mean identity is as932344
                // if import id not equal 0001 in any rows from database then generate a new one.

                //Create the element var and Attributes with the field name and value
                var xvar = new XElement($"TextField.{ ToOrchardFieldName(key.ToLower())}",
                                        new XAttribute("Text", value));
                xrow.Add(xvar);
            }
            return xrow;
        }

        private bool ProcessExists(int id)
        {
            return Process.GetProcesses().Any(x => x.Id == id);
        }

        private static object Descendants(string v)
        {
            throw new NotImplementedException();
        }

        public static string ToOrchardFieldName(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                throw new ArgumentNullException("Your input is null or empty.");
            }

            string noWhiteSpace = input.Replace(" ", string.Empty);
            string firstUpperCharacter = noWhiteSpace[0].ToString().ToUpper() + noWhiteSpace.Substring(1);

            return firstUpperCharacter;
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
