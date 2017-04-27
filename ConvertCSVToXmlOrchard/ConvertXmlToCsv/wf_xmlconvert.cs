using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Xml.Linq;
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
            fdlg.InitialDirectory = @"c:\";
            fdlg.DefaultExt = "xml";
            fdlg.Filter = "XML files (*.xml)|*.xml";
            fdlg.FilterIndex = 1;
            fdlg.RestoreDirectory = true;
            if (fdlg.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = fdlg.FileName;
            }
        }

        private void save()
        {
            if (textBox1.Text != "")
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
                            .Descendants("attendees")
                            .SelectMany(d => d.Elements())
                            .Select(e => e.Name.ToString())
                            .Distinct();

                var headers =
                    String.Join(",",
                        getFields(xml)
                            .Select(f => csvFormat(f)));

                var query =
                    from dealer in xml.Descendants("Content").Descendants("attendees")
                    select string.Join(",",
                        getFields(xml)
                            .Select(f => dealer.Elements(f).Attributes("Text").Any()
                                ? dealer.Element(f).Value
                                : "")
                            .Select(x => csvFormat(x)));

                var csv =
                    String.Join(Environment.NewLine,
                        new[] { headers }.Concat(query));

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
                DialogResult dr = saveFileDialog1.ShowDialog();
                if (dr == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;
                    File.WriteAllText(filename, csv);
                    MessageBox.Show("Save File Success");
                    wr.Close();

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
