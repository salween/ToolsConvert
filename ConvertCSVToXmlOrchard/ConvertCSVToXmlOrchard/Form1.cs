using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;
using CsvHelper;


namespace ConvertCSVToXmlOrchard
{
    public partial class Form1 : Form
    {
        public Form1()
        {


            InitializeComponent();
        }

        private void OpenFilecsv()
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Open File CSV";
            fdlg.InitialDirectory = @"c:\";
            fdlg.DefaultExt = "csv";
            fdlg.Filter = "CSV files (*.csv)|*.csv";
            fdlg.FilterIndex = 2;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fdlg.FileName;
            }
        }

        private void savefile()
        {
            if (textBox1.Text != "")
            {
                try
                {
                    string filenames = textBox1.Text;

                    string csv = File.ReadAllText(textBox1.Text);

                    XDocument doc = CsvToXml.ConvertCsvToXML(csv, filenames);

                    // save file 
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Xml files (*.xml)|*.xml";
                    DialogResult dr = saveFileDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        string filename = saveFileDialog1.FileName;
                        string checkdata = $"{Environment.CurrentDirectory}\\Database\\XmlDatabase.xml";

                        if (File.Exists(checkdata))
                        {
                            var modified = XElement.Load(checkdata);
                            var newxml = doc.Descendants("Orchard").Descendants("Content").Elements("attendees");
                            var oldxml = modified.Element("Content").Elements("attendees");

                            foreach (var item in newxml)
                            {
                                var itemx = oldxml.FirstOrDefault(x => x.Element("TextField.Firstname").Attribute("Text").Value == item.Element("TextField.Firstname")
                                .Attribute("Text").Value && x.Element("TextField.Lastname").Attribute("Text").Value == item.Element("TextField.Lastname")
                                .Attribute("Text").Value);

                                if (itemx != null)
                                {
                                    item.Element("TextField.Firstname").Attribute("Text").Value = itemx.Element("TextField.Firstname").Attribute("Text").Value;
                                    item.Element("TextField.Lastname").Attribute("Text").Value = itemx.Element("TextField.Lastname").Attribute("Text").Value;
                                   
                                }
                                else
                                {
                                    item.Element("TextField.Firstname").Attribute("Text").Value = item.Element("TextField.Firstname").Attribute("Text").Value;
                                    item.Element("TextField.Lastname").Attribute("Text").Value = item.Element("TextField.Lastname").Attribute("Text").Value;
                                    
                                }
                            }

                            doc.Save(filename);
                            doc.Save(checkdata);
                        }
                        else
                        {
                            doc.Save(filename);
                            doc.Save(checkdata);
                        }
                        MessageBox.Show("Save File Success");

                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

            }

        }
        

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFilecsv();
        }

        private void btn_save_Click(object sender, EventArgs e)
        {
            savefile();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}
