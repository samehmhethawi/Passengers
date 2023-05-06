using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class TRCHANGE_CAR_LINESVM
    {
        public long NB { get; set; }

        public long? CARNB { get; set; }

        public String TABNU { get; set; }

        public string CITYNB { get; set; }
     
        public string CARREG { get; set; }
    
        public string CARKIND { get; set; }
 
        public long? LINENB { get; set; }
    
        public string LINENAME { get; set; }

        public int? LINE_TYPE { get; set; }

        public DateTime? UDATE { get; set; }

    }
}