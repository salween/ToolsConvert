using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConvertCSVToXmlOrchard
{
    class CSVModel
    {
        //Should have properties which correspond to the Column Names in the file

        //public int Id { get; set; }

        public string Firstvname { get; set; }

        public string Lastname { get; set; }

        public string Companyname { get; set; }

        public string Title { get; set; }

        public string Mailcity { get; set; }

        public string Mailstate { get; set; }

        public string Phone { get; set; }

        public string Email { get; set; }

        public string Webpage { get; set; }

        public string Importid { get; set; }

        public string Spouse { get; set; }

        public string Guest { get; set; }

        public string Firsttime { get; set; }
    }

}
