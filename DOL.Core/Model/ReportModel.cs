using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOL.Core
{
    public class ReportModel
    {
        public List<string> x { get; set; }

        public List<Series> series { get; set; }
        

        public List<Series> expendSeries { get; set; }

    }
    

    public class Series
    {
        public string name { get; set; }
        public List<decimal> data { get; set; }
        public decimal y { get; set; }
    }

    

}