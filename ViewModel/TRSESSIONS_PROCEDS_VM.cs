using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class TRSESSIONS_PROCEDS_VM
    {
        public long NB { get; set; }
       public string PROCEDNAME { get; set; }
        public long SESSIONNB { get; set; }

        public long CARPROCEDNB { get; set; }

        public DateTime RECDAT { get; set; }

        public long PSTATUS { get; set; }

        public long? CARNB { get; set; }

        public long ORDR { get; set; }

        public long CARPROCEDSTEPNB { get; set; }


    }
}