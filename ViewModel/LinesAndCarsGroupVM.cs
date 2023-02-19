using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class LinesAndCarsGroupVM
    {
        public string CityName { get; set; }   
        public string TYPS { get; set; }
        public string STATUS { get; set; }
        public string kind { get; set; }
        public string trname { get; set; }
        public string reg { get; set; }
        public long CoutnCar { get; set; }
    }

    public class LinesAndCarsDetalisVM
    {
        public string CityName { get; set; }
        public string TYPS { get; set; }
        public string STATUS { get; set; }
        
        public string trname { get; set; }
        
        public long? carnb { get; set; }
        public string tabnu { get; set; }
        public string reg { get; set; }
        public string kind { get; set; }
        public long? carcity { get; set; }


    }
    public class LinesAndCarsDetalisPDFVM
    {
        public string CityName { get; set; }
        public string TYPS { get; set; }
        public string STATUS { get; set; }

        public string trname { get; set; }

        public long? carnb { get; set; }
        public string tabnu { get; set; }
        public string reg { get; set; }
        public string kind { get; set; }
        public string carcity { get; set; }


    }
}