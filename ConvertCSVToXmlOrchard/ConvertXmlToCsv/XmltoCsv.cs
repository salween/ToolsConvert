using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertXmlToCsv
{
    public class XmltoCsv
    {
        public XmltoCsv()
        {

        }

        public void WriterInCSV(string textxml)
        {
            using (TextWriter fileWriter = GenerateStreamFromString(textxml))
            {
                var csv = new CsvWriter(fileWriter);
                fileWriter.Write(textxml);              

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
