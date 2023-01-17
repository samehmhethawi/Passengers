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
        public int PROCEDNB { get; set; }

        public long SESSIONNB { get; set; }

        public long CARPROCEDNB { get; set; }

        public DateTime RECDAT { get; set; }

        public long PSTATUS { get; set; }

        public long? CARNB { get; set; }

        public long ORDR { get; set; }

        public long CARPROCEDSTEPNB { get; set; }


    }

    public class PROCEDS_Print_ALL
    { 
    
       
        public List<PROCEDS_2001_VM> proces2001 { get; set; }

        public PROCEDS_Print_ALL()
        {
            proces2001 = new List<PROCEDS_2001_VM>();
        }

    }
    public class PROCEDS_2000_VM : PROCEDS_Print_ALL
    { 
    }
    public class PROCEDS_2001_VM 
    {
        public string PROCEDNAME { get; set; }
        public int? PROCEDNB { get; set; }
        public long? CARPROCEDNB { get; set; }
        public long?  NB { get; set; } 
        public string NAME { get; set; }
        public string TYP { get; set; }
        public string LINEPATH { get; set; }
        public  List<string> LISTCITYS { get; set; }
    }


}