using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using CsvHelper;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace ConvertXmlToCsv
{
    public partial class wf_xmlconvert : Form
    {
        public wf_xmlconvert()
        {
            InitializeComponent();
        }

        private void OpenFilexml()
        {
            OpenFileDialog fdlg = new OpenFileDialog();
            fdlg.Title = "Open File XML";
            //fdlg.InitialDirectory = @"c:\";
            fdlg.DefaultExt = "xml";
            fdlg.Filter = "XML files (*.xml)|*.xml";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fdlg.FileName;
            }
        }

        private static bool ContainsText(string name)
        {
            return name.Contains("TextField.");
        }

        private void save()
        {
            if (textBox1.Text != "")
            {
                try
                {
                    StreamReader wr = new StreamReader(textBox1.Text, true);
                    string fs = wr.ReadToEnd();

                    Func<string, string> csvFormat =
          t => String.Format("\"{0}\"", t.Replace("\"", "\"\""));

                    var xml = XDocument.Parse(fs);

                    Func<XDocument, IEnumerable<string>> getFields =
                        xd =>
                            xd
                                .Descendants("Content")
                                 .Elements()
                                .SelectMany(d => d.Elements())
                                .Select(e => e.Name.ToString())
                                .Distinct();

                    Func<XDocument, IEnumerable<string>> getFields2 =
                        xd =>
                            xd
                                .Descendants("Content")
                                .Elements()
                                .SelectMany(d => d.Elements())
                                .Where(e => e.Name.ToString().Contains("TextField."))
                                .Select(e => e.Name.ToString())
                                .Distinct()
                                .ToList();


                    var headers = String.Join(",", getFields(xml).Select(f => csvFormat(f)));

                    var header = "";
                    List<string> name = new List<string>();
                    foreach (var head in headers.Split(','))
                    {
                        if (ContainsText(head))
                        {
                            name.Add(head);

                        }
                    }

                    header = String.Join(",", name);


                    var query =
                        from dealer in xml.Descendants("Content").Elements()
                        select string.Join(",",
                            getFields2(xml)
                                .Select(f => dealer.Elements(f).Attributes("Text").Any()
                                    ? dealer.Element(f).Attribute("Text").Value
                                    : "")
                                .Select(x => csvFormat(x)));

                    var csv =
                       String.Join(Environment.NewLine,
                           new[] { header.Replace("TextField.", "") }.Concat(query));
                  

                    SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                    saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
                    DialogResult dr = saveFileDialog1.ShowDialog();
                    if (dr == DialogResult.OK)
                    {
                        string filename = saveFileDialog1.FileName;
                      
                        XmltoCsv ss = new XmltoCsv();
                        ss.WriterInCSV(csv);

                        File.WriteAllText(filename, csv);
                        MessageBox.Show("Save File Success");
                        wr.Close();

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
            OpenFilexml();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            save();
        }
    }
}
