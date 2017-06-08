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

     

        private void save()
        {
            if (textBox1.Text != "")
            {
                string path = textBox1.Text;

                ConvertXmlToCsv xpath = new ConvertXmlToCsv();               

                string csverite = xpath.csvwrite(path);

                SaveFileDialog saveFileDialog1 = new SaveFileDialog();
                saveFileDialog1.Filter = "CSV files (*.csv)|*.csv";
                DialogResult save = saveFileDialog1.ShowDialog();
                if (save == DialogResult.OK)
                {
                    string filename = saveFileDialog1.FileName;

                    //var xcsv = new CsvWriter(filename);

                    //XmltoCsv write = new XmltoCsv();

                    //write.ConvertXmlToCsvHelper(filename, value);

                    File.WriteAllText(filename, csverite);
                    MessageBox.Show("Save File Success");
                   

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
