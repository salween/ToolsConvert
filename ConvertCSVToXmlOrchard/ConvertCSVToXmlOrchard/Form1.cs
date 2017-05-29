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
            using (OpenFileDialog openfile = new OpenFileDialog())
            {
                openfile.Title = "Open File CSV";
                //openfile.InitialDirectory = @"c:\";
                openfile.DefaultExt = "csv";
                openfile.Filter = "CSV files (*.csv)|*.csv";
                openfile.FilterIndex = 1;
                openfile.RestoreDirectory = true;
                if (openfile.ShowDialog() == DialogResult.OK)
                {
                    textBox1.Text = openfile.FileName;
                }
            }
        }


        private void savefile()
        {

            if (textBox1.Text != "")
            {
                try
                {
                    var result = ReadInCSV(textBox1.Text);

                    string filenames = textBox1.Text;

                    XDocument doc = CsvToXml.ConvertCsvToXML(result, filenames);

                    // save file       

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "Xml files (*.xml)|*.xml";
                    DialogResult dr = saveFileDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        string filename = saveFileDialog1.FileName;
                        doc.Save(filename);

                        //string checkdata = $"{Environment.CurrentDirectory}\\Database\\XmlDatabase.xml";
                        //doc.Save(checkdata);

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


        public static List<Dictionary<string, string>> ReadInCSV(string absolutePath)
        {
            List<string> contents = new List<string>();
            List<Dictionary<string, string>> mappingContents = new List<Dictionary<string, string>>();
            using (TextReader fileReader = File.OpenText(absolutePath))
            {
                var csv = new CsvReader(fileReader);
                csv.Configuration.HasHeaderRecord = false;
                bool isHeader = true;

                List<string> headers = new List<string>();
                while (csv.Read())
                {
                    // read header
                    // assuming first row is header
                    // `isheader` is boolean that allow access this condition scope only one time
                    if (isHeader)
                    {
                        string headerText = string.Empty;
                        for (int i = 0; csv.TryGetField(i, out headerText); i++)
                            headers.Add(headerText);
                        isHeader = false;
                    }
                    else
                    {
                        //read all contents
                        string value;
                        for (int i = 0; csv.TryGetField(i, out value); i++)
                        {
                            contents.Add(value);
                        }
                    }
                }

                // mapping header and content
                for (int i = 0; i < contents.Count; i += headers.Count)
                {
                    Dictionary<string, string> resultDataDic = new Dictionary<string, string>();
                    for (int j = 0; j < headers.Count; j++)
                    {
                        string column = headers[j];
                        string value = contents[i + j];
                        resultDataDic.Add(column, value);
                    }
                    mappingContents.Add(resultDataDic);
                }
            }
            return mappingContents;
        }
    }

}
