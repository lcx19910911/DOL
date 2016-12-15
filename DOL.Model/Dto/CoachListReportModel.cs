using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOL.Model
{
    public class CoachListReportModel
    {         
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public Dictionary<string, ThemeSalaryItemModel> AllDic { get; set; }

        public List<ThemeSalary> SalaryList { get; set; }
        public List<ThemeSalary> OldSalaryList { get; set; }
    }


    public class ThemeSalaryItemModel
    {
        public string CoachName { get; set; }
        //              科目  次数    薪资级别  技术  金额 小计
        public List<Tuple<ThemeCode,int,string, int, decimal, decimal>> List { get; set; }

        public List<Tuple<ThemeCode, int, string, int, decimal, decimal>> OldList { get; set; }

        public decimal BasicSalary { get; set; }

        public decimal ThemeTwoMoney { get; set; }

        public decimal ThemeThreeMoney { get; set; }

        public decimal TotalMoeny { get; set; }

    }
}