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
            
            var loaddata = (from s in xdocument.Descendants("Content").Elements()
                            select new
                            {
                                ImportId = s.Element("TextField.Importid").Attribute("Text").Value,
                                Firstname = s.Element("TextField.Firstname").Attribute("Text").Value,
                                Lastname = s.Element("TextField.Lastname").Attribute("Text").Value,
                                Companyname = s.Element("TextField.Companyname").Attribute("Text").Value,
                                Title = s.Element("TextField.Title").Attribute("Text").Value
                              
                            }).ToList();

            foreach (var id in loaddata)
            {
                XmlModel obj = new XmlModel();
                obj.ImportId = id.ImportId;
                obj.Firstname = id.Firstname;
                obj.Lastname = id.Lastname;
                obj.Companyname = id.Companyname;
                obj.Title = id.Title;
               
            }

            return null;
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
