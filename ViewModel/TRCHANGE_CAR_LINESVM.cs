using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class TRCHANGE_CAR_LINESVM
    {
        public long NB { get; set; }
        public long CARNB { get; set; }
        public string TABNU { get; set; }
        public long? CARCITY { get; set; }
        public long? CARREG { get; set; }
        public long? CARKIND { get; set; }


        public long? CAR_OLD_LIN { get; set; }
        public string CAR_OLD_LIN_NAME { get; set; }


        public long? CAR_NEW_LIN { get; set; }
        public string CAR_NEW_LIN_NAME { get; set; }



        public long? CAR_OLD_BASELIN { get; set; }
        public string CAR_OLD_BASELIN_NAME { get; set; }


        public long? CAR_NEW_BASELIN { get; set; }
        public string CAR_NEW_BASELIN_NAME { get; set; }

        public DateTime? UDATE { get; set; }

    }
}