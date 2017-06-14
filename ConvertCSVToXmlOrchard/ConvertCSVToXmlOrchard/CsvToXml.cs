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

        private static List<CSVModel> listOfIdFromDatabase = new List<CSVModel>();

        private static string databasepath = $"{Environment.CurrentDirectory}\\Database\\XmlDatabase.xml";

        public static XDocument ConvertCsvToXML(List<Dictionary<string, string>> dataContents, string filename, string textsave)
        {
            string dirName = new DirectoryInfo(filename).Name.Replace(".csv", "");

            //Create the element
            var xsyntax = new XDocument(new XDeclaration("1.0", "UTF-8", "yes")); //<?xml version="1.0" encoding="utf-8" standalone="yes"?>

            XComment comm = new XComment(textsave); // Create the Comment --->   <!--Exported from Orchard-->

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
            xsyntax.Add(comm, xHead);

            // save import id and identity 
            listOfValueFromDatabase.Add(xsyntax.ToString());
            SaveToDataBase(xContent);


            return xsyntax;
        }

        private static void SaveToDataBase(XElement content)
        {
            //listOfValueFromDatabase to database
            if (File.Exists(databasepath))
            {
                XElement ValueFromDatabase = XElement.Load(databasepath);
                var loaddata = ValueFromDatabase.Descendants("Content");
                loaddata.Remove();
                if (loaddata.Any())
                {

                }
                else
                {
                    ValueFromDatabase.Add(content);
                    ValueFromDatabase.Save(databasepath);
                }

            }
            else
            {
                XDocument ValueFromDatabase = new XDocument();
                ValueFromDatabase = new XDocument(new XDeclaration("1.0", "UTF-8", "yes"));
                ValueFromDatabase.Add(new XComment("Exported from Orchard"), XElement.Parse(listOfValueFromDatabase[0]));
                ValueFromDatabase.Save(databasepath);
            }
            // save all content in format 'importid={id},identity={identity}'
        }

        private static void LoadFromDataBase()
        {
            //listOfValueFromDatabase
            // listOfValueFromDatabase[i] is "importid=0001,identity=as932344"
            // check from database

            if (File.Exists(databasepath))
            {
                var loaddatabasexml = XElement.Load(databasepath);
                var loaddata = (from s in loaddatabasexml.Descendants("Content").Elements()
                                select new
                                {
                                    Identity = s.Element("IdentityPart").Attribute("Identifier").Value,
                                    ImportId = s.Element("TextField.Importid").Attribute("Text").Value,
                                    Firstname = s.Element("TextField.Firstname").Attribute("Text").Value,
                                    Lastname = s.Element("TextField.Lastname").Attribute("Text").Value,
                                    Companyname = s.Element("TextField.Companyname").Attribute("Text").Value,
                                    Title = s.Element("TextField.Title").Attribute("Text").Value,
                                    Mailcity = s.Element("TextField.Mailcity").Attribute("Text").Value,
                                    Mailstate = s.Element("TextField.Mailstate").Attribute("Text").Value,
                                    Phone = s.Element("TextField.Phone").Attribute("Text").Value,
                                    Email = s.Element("TextField.Email").Attribute("Text").Value,
                                    Webpage = s.Element("TextField.Webpage").Attribute("Text").Value,
                                    Spouse = s.Element("TextField.Spouse").Attribute("Text").Value,
                                    Guest = s.Element("TextField.Guest").Attribute("Text").Value,
                                    Firsttime = s.Element("TextField.Firsttime").Attribute("Text").Value
                                }).ToList();

                foreach (var id in loaddata)
                {
                    CSVModel obj = new CSVModel();
                    obj.ImportId = id.ImportId;
                    obj.Identity = id.Identity;
                    obj.Firstname = id.Firstname;
                    obj.Lastname = id.Lastname;
                    obj.Companyname = id.Companyname;
                    obj.Title = id.Title;
                    obj.Mailcity = id.Mailcity;
                    obj.Mailstate = id.Mailstate;
                    obj.Phone = id.Phone;
                    obj.Email = id.Email;
                    obj.Webpage = id.Webpage;
                    obj.ImportId = id.ImportId;
                    obj.Spouse = id.Spouse;
                    obj.Guest = id.Guest;
                    obj.Firsttime = id.Firsttime;

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
            // if import id = 0001 then get identity from `importid=0001,identity=as932344` that mean identity is as932344
            // if import id not equal 0001 in any rows from database then generate a new one.

            var checkId = listOfIdFromDatabase.FirstOrDefault(x => x.ImportId == columns["Import id"]);

            if (checkId != null)
            {
                id = checkId.Identity;
            }
            else
            {
                id = Guid.NewGuid().ToString().Replace("-", "");
            }

            var xrow = new XElement(FileName, new XAttribute("Id", $"/Identifier={id}"), new XAttribute("Status", "Published"));

            XElement Identifier = new XElement("IdentityPart", new XAttribute("Identifier", id));

            var commonP = new XElement("CommonPart", new XAttribute("Owner", "/User.UserName=admin"), new XAttribute("CreatedUtc", DateTime.UtcNow), new XAttribute("PublishedUtc", DateTime.UtcNow), new XAttribute("ModifiedUtc", DateTime.UtcNow));

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
                if (checkId != null)
                {
                    if (ToOrchardFieldName(key.ToLower()) == "Firstname")
                    {
                        if (!value.Equals(checkId.Firstname))
                        {
                            checkId.Firstname = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Lastname")
                    {
                        if (!value.Equals(checkId.Lastname))
                        {
                            checkId.Lastname = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Companyname")
                    {
                        if (!value.Equals(checkId.Companyname))
                        {
                            checkId.Companyname = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Title")
                    {
                        if (!value.Equals(checkId.Title))
                        {
                            checkId.Title = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Mailcity")
                    {
                        if (!value.Equals(checkId.Mailcity))
                        {
                            checkId.Mailcity = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Mailstate")
                    {
                        if (!value.Equals(checkId.Mailstate))
                        {
                            checkId.Mailstate = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Phone")
                    {
                        if (!value.Equals(checkId.Phone))
                        {
                            checkId.Phone = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Email")
                    {
                        if (!value.Equals(checkId.Email))
                        {
                            checkId.Email = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Webpage")
                    {
                        if (!value.Equals(checkId.Webpage))
                        {
                            checkId.Webpage = value;

                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Spouse")
                    {
                        if (!value.Equals(checkId.Spouse))
                        {
                            checkId.Spouse = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Guest")
                    {
                        if (!value.Equals(checkId.Guest))
                        {
                            checkId.Guest = value;
                        }
                    }
                    else if (ToOrchardFieldName(key.ToLower()) == "Firsttime")
                    {
                        if (!value.Equals(checkId.Firsttime))
                        {
                            checkId.Firsttime = value;
                        }
                    }
                }
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

            string noWhiteSpace = input.Replace(" ", string.Empty).Replace("-", "");
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
