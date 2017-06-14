using System;
using System.Windows.Forms;


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
            OpenFileDialog openfile = new OpenFileDialog();
            openfile.Title = "Open File XML";
            //openfile.InitialDirectory = @"c:\";
            openfile.DefaultExt = "xml";
            openfile.Filter = "XML files (*.xml)|*.xml";
            openfile.FilterIndex = 1;
            openfile.RestoreDirectory = true;
            if (openfile.ShowDialog() == DialogResult.OK)
            {
                textBox1.Text = openfile.FileName;
            }
        }

     

        private void savefile()
        {
            if (textBox1.Text != "")
            {
                string path = textBox1.Text;

                ConvertXmlToCsvHelper xfilepath = new ConvertXmlToCsvHelper();

                //string csvwrite = xfilepath.csvwrite(path);

                xfilepath.savefilewritehelper(path);


            }
                                    
        }     

        private void button1_Click(object sender, EventArgs e)
        {
            OpenFilexml();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            savefile();
        }
    }
}
