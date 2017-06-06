using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using CsvHelper;
using ConvertXmlToCsv.Model;

namespace ConvertXmlToCsv
{
    public class XmltoCsv
    {
        public XmltoCsv()
        {

        }

        public void WriterInCSV(string filename , string textxml) 
        {
            using (TextWriter fileWriter = GenerateStreamFromString(filename))

            //using (var write = new CsvWriter(filename))
            {

                var write = new CsvWriter(fileWriter);
                write.Configuration.Encoding = Encoding.UTF8;
                foreach (var value in textxml)
                {
                    write.WriteRecord(value);
                }
                fileWriter.Close();

            }
           
        }


        public StreamWriter GenerateStreamFromString(string s)
        {
            MemoryStream stream = new MemoryStream();
            StreamWriter writer = new StreamWriter(stream);
            writer.Write(s);
            writer.Flush();
            return writer;
        }
    }
}



