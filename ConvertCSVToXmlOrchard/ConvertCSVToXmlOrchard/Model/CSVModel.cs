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

        public int Id { get; set; }

        public String Firstname { get; set; }

        public String Lastname { get; set; }

        public String Companyname { get; set; }

        public String Title { get; set; }

        public String Mailcity { get; set; }

        public String Mailstate { get; set; }

        public String Phone { get; set; }

        public String Email { get; set; }

        public String Webpage { get; set; }

        public String Importid { get; set; }

        public String Spouse { get; set; }

        public String Guest { get; set; }

        public String Firsttime { get; set; }
    }

}
