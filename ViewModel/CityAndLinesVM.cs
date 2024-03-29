﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class CityAndLinesVM
    {    
        public long rr { get; set; }
        public string CityName { get; set; }      
        public string LINENAME { get; set; }
        public long? TRNB { get; set; }
        public string TPYS { get; set; }
        public string STATUS { get; set; }
        public string ISCANCELD { get; set; }
        public long? MINCARS { get; set; }
        public long? MAXCARS { get; set; }
    }
}