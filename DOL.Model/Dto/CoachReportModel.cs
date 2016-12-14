using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DOL.Model
{
    public class CoachReportModel
    {         
        public string CoachName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ExamModel ExamModel { get; set; }
        public ThemeSalaryModel ThemeSalaryModel { get; set; }
    }

    public class ExamModel
    {
        //                   时间   学员名  考试人数 通过人数 通过率
        public List<Tuple<ThemeCode,DateTime, string, int, int, int>> List { get; set; }
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
        public int ThemeTwoMonthPassScaling { get; set; }

        /// <summary>
        /// 当月（人数）
        /// </summary>
        public int ThemeTwoMonthPeopleExamCount { get; set; }
        

        /// <summary>
        /// 通过比例（人数）
        /// </summary>
        public int ThemeTwoMonthPeoplePassScaling { get; set; }



        public int ThemeTwoAllExamCount { get; set; }

        public int ThemeTwoAllPassCount { get; set; }

        /// <summary>
        /// 通过比例
        /// </summary>
        public int ThemeTwoAllPassScaling { get; set; }







        public int ThemeThreeMonthExamCount { get; set; }

        public int ThemeThreeMonthPassCount { get; set; }

        /// <summary>
        /// 通过比例
        /// </summary>
        public int ThemeThreeMonthPassScaling { get; set; }


        /// <summary>
        /// 当月（人数）
        /// </summary>
        public int ThemeThreeMonthPeopleExamCount { get; set; }
        
        /// <summary>
        /// 通过比例（人数）
        /// </summary>
        public int ThemeThreeMonthPeoplePassScaling { get; set; }


        public int ThemeThreeAllExamCount { get; set; }

        public int ThemeThreeAllPassCount { get; set; }

        /// <summary>
        /// 通过比例
        /// </summary>
        public int ThemeThreeAllPassScaling { get; set; }
    }

    public class ThemeSalaryModel
    {
        //              科目  次数    薪资级别  技术  金额 小计
        public List<Tuple<ThemeCode,int,string, int, decimal, decimal>> List { get; set; }

        public List<Tuple<ThemeCode, int, string, int, decimal, decimal>> OldList { get; set; }

        public decimal BasicSalary { get; set; }

        public decimal TotalMoeny { get; set; }

    }
}