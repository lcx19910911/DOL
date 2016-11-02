using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOL.Model
{
    public class EnteredPointReportModel
    {
        public DateTime StartTime { get; set; }
        public int EndDay { get; set; }

        public List<Tuple<string, string, Dictionary<int,int>,int>> List { get; set; }
        public Dictionary<int, int> TotalDic { get; set; }
        public int TotalCount { get; set; }

       
    }
}