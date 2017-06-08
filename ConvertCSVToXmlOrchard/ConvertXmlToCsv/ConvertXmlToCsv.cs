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
   public class ConvertXmlToCsv
    {
        public ConvertXmlToCsv()
        {

        }

        public string csvwrite(string strpath)
        {
            try
            {
                StreamReader xread = new StreamReader(strpath, true);
                string read = xread.ReadToEnd();

                Func<string, string> csvFormat =
      t => String.Format("\"{0}\"", t.Replace("\"", "\"\""));

                var xml = XDocument.Parse(read);

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

                xread.Close();

                return csv;

            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());

                return null;
            }
        }

        private static bool ContainsText(string name)
        {
            return name.Contains("TextField.");
        }
    
    }
}
