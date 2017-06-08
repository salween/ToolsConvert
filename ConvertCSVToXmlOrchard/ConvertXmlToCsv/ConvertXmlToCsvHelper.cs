using ConvertXmlToCsv.Model;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace ConvertXmlToCsv
{
   public class ConvertXmlToCsvHelper
    {
        public ConvertXmlToCsvHelper()
        {

        }

        public void csvwritehelper(string filename)
        {
            try
            {
                StreamReader xread = new StreamReader(filename, true);
                string reader = xread.ReadToEnd();

                var xml = XDocument.Parse(reader);

                List<XmlModel> attendeesmodel = new List<XmlModel>();

                using (StreamWriter xwrite = new StreamWriter(filename))
                using (CsvWriter cw = new CsvWriter(xwrite))
                {
                    cw.WriteHeader<XmlModel>();

                    foreach(XmlModel attendees in attendeesmodel)
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
    }
}
