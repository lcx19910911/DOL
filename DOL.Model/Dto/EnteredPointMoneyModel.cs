using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOL.Model
{
    public class EnteredPointMoneyModel
    {
        public DateTime StartTime { get; set; }
        public int EndDay { get; set; }

        public List<Tuple<string, string, Dictionary<int, decimal>, decimal>> List { get; set; }
        public Dictionary<int, decimal> TotalDic { get; set; }
        public decimal TotalCount { get; set; }


    }

}