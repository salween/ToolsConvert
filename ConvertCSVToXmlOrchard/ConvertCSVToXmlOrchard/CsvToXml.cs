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
        private static List<string> listOfValueFromDatabase = new List<string>();

        private static List<databasemodel> listOfIdFromDatabase = new List<databasemodel>();

        private static string databasepath = $"{Environment.CurrentDirectory}\\Database\\XmlDatabase.xml";

        public static XDocument ConvertCsvToXML(List<Dictionary<string, string>> dataContents, string filename)
        {            
            string dirName = new DirectoryInfo(filename).Name.Replace(".csv", "");

            //Create the element
            var xsyntax = new XDocument(new XDeclaration("1.0", "UTF-8", "yes")); //<?xml version="1.0" encoding="utf-8" standalone="yes"?>

            XComment comm = new XComment("Exported from Orchard"); // Create the Comment --->   <!--Exported from Orchard-->

            var xHead = new XElement("Orchard");// Create the head  -->  <Orchard>

            var xContentDe = new XElement("ContentDefinition"); //Create the ContentDefinition -->  <ContentDefinition>

            var xContent = new XElement("Content"); //Create the Content -->  <Content>

            // load previous data from database
            LoadFromDataBase(dirName);

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

        private static void LoadFromDataBase(string Filename)
        {
            //listOfValueFromDatabase
            // listOfValueFromDatabase[i] is "importid=0001,identity=as932344"
            // check from database

            if (File.Exists(databasepath))
            {
                var loaddatabasexml = XElement.Load(databasepath);
                var loaddata = (from s in loaddatabasexml.Descendants("Content").Elements(Filename)
                               select new
                               {
                                   Iddentity = s.Element("IdentityPart").Attribute("Identifier").Value,
                                   ImportId = s.Element("TextField.Importid").Attribute("Text").Value
                               }).ToList();

                foreach (var id in loaddata)
                {
                    databasemodel obj = new databasemodel();
                    obj.ImportId = id.ImportId;
                    obj.Iddentity = id.Iddentity;
                    listOfIdFromDatabase.Add(obj);
                }

            }
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
            //string id = Guid.NewGuid().ToString().Replace("-", "");
            string id = string.Empty;

            // check from database
            var checkId = listOfIdFromDatabase.FirstOrDefault(x => x.ImportId == columns["Import id"]);

            if (checkId != null)
            {
                id = checkId.ImportId;
            }
            else
            {
                id = Guid.NewGuid().ToString().Replace("-", "");
            }
          
            // if import id = 0001 then get identity from `importid=0001,identity=as932344` that mean identity is as932344
            // if import id not equal 0001 in any rows from database then generate a new one.

            var xrow = new XElement(FileName, new XAttribute("Id", $"/Identifier={id}"), new XAttribute("Status", "Published"));

            XElement Identifier = new XElement("IdentityPart", new XAttribute("Identifier", id));           

            var commonP = new XElement("CommonPart", new XAttribute("Owner", "/User.UserName=admin"), new XAttribute("CreatedUtc", DateTime.UtcNow));

            xrow.Add(Identifier, commonP);

            

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

        public class databasemodel
        {
            public string ImportId { get; set; }
            public string Iddentity { get; set; }
        }
    }
}
