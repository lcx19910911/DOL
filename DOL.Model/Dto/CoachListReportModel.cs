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
        public Dictionary<string, ThemeTrainItemModel> AllTrainDic { get; set; }

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

    public class ThemeTrainItemModel
    {
        public string CoachName { get; set; }

        /// <summary>
        /// 当月（次数）
        /// </summary>
        public int ThemeTwoMonthExamCount { get; set; }
        /// <summary>
        /// 当月通过（次数）
        /// </summary>
        public int ThemeTwoMonthPassCount { get; set; }

        /// <summary>
        /// 通过比例（次数）
        /// </summary>
        public double ThemeTwoMonthPassScaling { get; set; }

        /// <summary>
        /// 当月（人数）
        /// </summary>
        public int ThemeTwoMonthPeopleExamCount { get; set; }


        /// <summary>
        /// 通过比例（人数）
        /// </summary>
        public double ThemeTwoMonthPeoplePassScaling { get; set; }


       
        public int ThemeTwoAllPeopleExamCount { get; set; }
        public int ThemeTwoAllExamCount { get; set; }

        public int ThemeTwoAllPassCount { get; set; }
        public int ThemeTwoAllPeoplePassCount { get; set; }

        /// <summary>
        /// 通过比例
        /// </summary>
        public double ThemeTwoAllPassScaling { get; set; }

        /// <summary>
        /// 通过比例
        /// </summary>
        public double ThemeTwoAllPeoplePassScaling { get; set; }





        public int ThemeThreeMonthExamCount { get; set; }

        public int ThemeThreeMonthPassCount { get; set; }

        /// <summary>
        /// 通过比例
        /// </summary>
        public double ThemeThreeMonthPassScaling { get; set; }


        /// <summary>
        /// 当月（人数）
        /// </summary>
        public int ThemeThreeMonthPeopleExamCount { get; set; }

        /// <summary>
        /// 通过比例（人数）
        /// </summary>
        public double ThemeThreeMonthPeoplePassScaling { get; set; }


        public int ThemeThreeAllPeopleExamCount { get; set; }
        public int ThemeThreeAllExamCount { get; set; }

        public int ThemeThreeAllPassCount { get; set; }

        public int ThemeThreeAllPeoplePassCount { get; set; }
        

        /// <summary>
        /// 通过比例
        /// </summary>
        public double ThemeThreeAllPassScaling { get; set; }
        /// <summary>
        /// 通过比例
        /// </summary>
        public double ThemeThreeAllPeoplePassScaling { get; set; }

    }
}