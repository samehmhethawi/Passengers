using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Passengers.ViewModel
{
    public class CountTotal
    {

        public int TOTALCOUNT { get; set; }
        public int AGREE { get; set; }

        public int NOTAGREE { get; set; }

        public int DELAYED { get; set; }
        public int UNANSWERED { get; set; }

    }
}
