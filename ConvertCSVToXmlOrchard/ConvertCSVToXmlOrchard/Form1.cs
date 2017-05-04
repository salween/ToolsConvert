using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using System.Xml.Serialization;

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
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Xml files (*.xml)|*.xml";
                    DialogResult dr = saveFileDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        string filename = saveFileDialog1.FileName;
                        doc.Save(filename);
                        MessageBox.Show("Save File Success");
                        //Console.WriteLine(doc.Declaration);
                        //foreach (XElement c in doc.Elements())
                        //{
                        //    Console.WriteLine(c);
                        //}
                    }

                }
                catch (Exception e)
                {
                    MessageBox.Show(e.ToString());
                }

            }

        }

        private void Modified()
        {
            if (textBox1.Text != "")
            {
                try
                {
                    string filenames = textBox1.Text;
                    string csv = File.ReadAllText(textBox1.Text);
                    XDocument doc = CsvToXml.ConvertCsvToXML(csv, filenames);
                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Xml files (*.xml)|*.xml";
                    DialogResult dr = saveFileDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        string filename = saveFileDialog1.FileName;

                        XDocument xmlDoc = XDocument.Load(filename);
                        string dirName = new DirectoryInfo(filenames).Name.Replace(".csv", "");
                        var items = from item in xmlDoc.Elements("Orchard").Elements("Content").Elements(dirName)
                                    where item != null && (item.Attribute("Id").Value != @"/Identifier=e9402713bc744511a11cc8de980cf0fb")
                                    select item;
                        //foreach (var item in items)
                        //

                        //assign new value to the sub-element author
                        //item.Element("TextField.CompanyName").Attribute("Text").Value) = "Faii";

                        //}
                        xmlDoc.Save(filename);

                        //doc.Save(filename);
                        MessageBox.Show("Modified File Success");
                        //Console.WriteLine(doc.Declaration);
                        //foreach (XElement c in doc.Elements())
                        //{
                        //    Console.WriteLine(c);
                        //}
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
            List<CsvToXml> doc = new List<CsvToXml>();
            XmlSerializer searial = new XmlSerializer(typeof(List<CsvToXml>));

            using (FileStream fs = new FileStream(Environment.CurrentDirectory + "\\jacob.xml", FileMode.Create, FileAccess.Write))
            {
                searial.Serialize(fs, doc);
                MessageBox.Show("Created");
            }

            savefile();
        }

        private void btn_mdf_Click(object sender, EventArgs e)
        {
            Modified();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }

}
