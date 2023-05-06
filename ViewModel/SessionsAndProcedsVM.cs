using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class SessionsAndProcedsVM
    {
        public string CityName { get; set; }
        public long CommNb { get; set; }
        public long SessNb { get; set; }
        public string SessNo { get; set; }
        public DateTime SessDate { get; set; }
        public string SessStatus { get; set; }

        public string NOTES { get; set; }
        

        public string ProcedName { get; set; }
        public string ProcedRes { get; set; }

    }
}