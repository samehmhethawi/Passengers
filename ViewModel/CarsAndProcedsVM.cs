using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class CarsAndProcedsVM
    {
        public long? carnb { get; set; }
        public string tabnu { get; set; }
        public string kind { get; set; }
        public string reg { get; set; }
        public string carcity { get; set; }
        public long? CARPROCEDNB { get; set; }
        public string PROCEDNAME { get; set; }

        public DateTime PROCEDDATE { get; set; }
        public string RESULT { get; set; }

        
        public string TYPSNAMEAGR { get; set; }
        public string STEPCITY { get; set; }
        public string PROPSTATUS { get; set; }
    }
}