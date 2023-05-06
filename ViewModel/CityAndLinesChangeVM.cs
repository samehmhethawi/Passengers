using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class CityAndLinesChangeVM
    {
        public long NB   { get; set; }
        public long PROCEDNB { get; set; }
        public string PROCEDNAME { get; set; }
        public int? CITYNB { get; set; }
        public DateTime RECDAT { get; set; }
        public string RESULT { get; set; }
        public string NEWLINENAME { get; set; }
        //public string TPYS { get; set; }
        public long? LINENB { get; set; }
        public string LINENAME { get; set; }
        public long? DONE { get; set; }
        public string NOTE { get; set; }
     //   public string LINENAME2 { get; set; }

    }
    public class CityAndLinesChangePDFVM
    {
        public long? ROW_NUM { get; set; }
        public long NB { get; set; }
        public long PROCEDNB { get; set; }
        public string PROCEDNAME { get; set; }
        public string CITYNB { get; set; }
        public DateTime RECDAT { get; set; }
        public string RESULT { get; set; }
        public string NEWLINENAME { get; set; }
        //public string TPYS { get; set; }
        public long? LINENB { get; set; }
        public string LINENAME { get; set; }
        public long? DONE { get; set; }
        public string NOTE { get; set; }
        //   public string LINENAME2 { get; set; }

    }
}