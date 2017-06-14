using ConvertXmlToCsv.Model;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ConvertXmlToCsv
{
    public class ConvertXmlToCsvHelper
    {
        public ConvertXmlToCsvHelper()
        {

        }

        public void csvwritehelper(string filename, string savefilewrite)
        {
            try
            {
                StreamReader xread = new StreamReader(filename, true);
                string reader = xread.ReadToEnd();

                var xml = XDocument.Parse(reader);

                List<XmlModel> attendeesmodel = modelxml(xml);

                using (StreamWriter xwrite = new StreamWriter(savefilewrite))
                using (CsvWriter cw = new CsvWriter(xwrite))
                {
                    cw.WriteHeader<XmlModel>();

                    foreach (XmlModel attendees in attendeesmodel)
                    {
                        cw.WriteRecord<XmlModel>(attendees);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        
        private List<XmlModel> modelxml(XDocument xdocument)
        {
            List<XmlModel> attendeesmodel = new List<XmlModel>();

            var loaddata = (from s in xdocument.Descendants("Content").Elements()
                            select new
                            {
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
                XmlModel obj = new XmlModel();
                obj.ImportId = id.ImportId;
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

                attendeesmodel.Add(obj);
            }


            return attendeesmodel;
        }


        public void savefilewritehelper(string savecsv)
        {

            SaveFileDialog saveFileDialog1 = new SaveFileDialog();
            saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
            DialogResult save = saveFileDialog1.ShowDialog();
            if (save == DialogResult.OK)
            {
                string filename = saveFileDialog1.FileName;

                csvwritehelper(savecsv, filename); 

                MessageBox.Show("Save File Success");

                 
            }
        }

    }
}
