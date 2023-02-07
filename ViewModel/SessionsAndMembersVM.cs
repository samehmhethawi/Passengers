using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class SessionsAndMembersVM
    {
        public string CityName { get; set; }
        public long CommNb { get; set; }
        public long SessNb { get; set; }
        public string SessNo { get; set; }
        public DateTime SessDate { get; set; }
        public string SessStatus { get; set; }
        //public string BossName { get; set; }
        //public string BossPostion{ get; set; }
        public string MemberName { get; set; }
        public string MemberShip { get; set; }
        public string MemberPostion { get; set; }
        public string ISPRESENT { get; set; }
    }
}