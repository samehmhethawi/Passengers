using Proced.DataAccess.Models.CF;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class TRSESSIONS_MEMBERS_PRESENTVM
    {
        public long NB { get; set; }
        public long SESSIONNB { get; set; }
        public long MEMBERNB { get; set; }
        public string MEMBERNAME { get; set; }
        public short? ISPRESENT { get; set; }
        public long MEMBERSHIPNB { get; set; }
        public long MEMBERPOSITIONNB { get; set; }
        public long SESSIONSTATUS { get; set; }

     
    }
}