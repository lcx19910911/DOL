using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Core
{
    public class StudentExportModel
    {

        #region 基本信息

        /// <summary>
        /// 学员姓名
        /// </summary>
        public string Name { get; set; }


        /// <summary>
        /// 身份证号码
        /// </summary>
        public string IDCard { get; set; }

        /// <summary>
        /// 性别
        /// </summary>        
        public string GenderCode { get; set; }
        
        /// <summary>
        /// 省份名称
        /// </summary>
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市称
        /// </summary>
        public string CityName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }



        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        


        /// <summary>
        /// 证书名称
        /// </summary>
        public string CertificateName { get; set; }

        #endregion


        #region 学车事宜

        /// <summary>
        /// 报名点名称
        /// </summary>
        public string EnteredPointName { get; set; }
        

        /// <summary>
        /// 推荐人名称
        /// </summary>
        public string ReferenceName { get; set; }
        

        /// <summary>
        /// 意向驾校名称
        /// </summary>
        public string WantDriverShopName { get; set; }
        

        /// <summary>
        /// 培训班别名称
        /// </summary>
        public string TrianName { get; set; }


        /// <summary>
        /// 费用
        /// </summary>
        public decimal Money { get; set; }


        /// <summary>
        /// 已交费用
        /// </summary>
        public decimal HadPayMoney { get; set; } = 0;

        /// <summary>
        /// 是否缴清费用
        /// </summary>
        public string MoneyIsFull { get; set; } 
        

        /// <summary>
        /// 缴费方式名称
        /// </summary>
        public string PayMethodName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        

        /// <summary>
        /// 报名时间
        /// </summary>
        public DateTime EnteredDate { get; set; }

        /// <summary>
        /// 报名地点（省份）
        /// </summary>
        public string EnteredProvinceName { get; set; }

        /// <summary>
        /// 报名地点（市）
        /// </summary>
        public string EnteredCityName { get; set; }
        
        /// <summary>
        /// 制卡驾校名称
        /// </summary>
        public string MakeDriverShopName { get; set; }

        /// <summary>
        /// 制卡日期
        /// </summary>
        public Nullable<DateTime> MakeCardDate { get; set; }
        
        /// <summary>
        /// 制卡地点名称
        /// </summary>
        public string MakeCardCityName { get; set; }

        /// <summary>
        /// 制卡备注
        /// </summary>
        public string MakeCardRemark { get; set; }        


        /// <summary>
        ///  分配院校
        /// </summary>
        public string DriverShopName { get; set; }


        #endregion

        #region 科目情况



        /// <summary>
        /// 科目一时间
        /// </summary>
        public Nullable<DateTime> ThemeOneDate { get; set; }

        /// <summary>
        /// 科目一是否通过
        /// </summary>
        public string ThemeOnePass { get; set; }


        /// <summary>
        /// 科目二时间
        /// </summary>
        public Nullable<DateTime> ThemeTwoDate { get; set; }


        /// <summary>
        /// 科目二是否通过
        /// </summary>
        public string ThemeTwoPass { get; set; }

        /// <summary>
        /// 科目二学时状态
        /// </summary>
        public string ThemeTwoTimeCode { get; set; }
        
        /// <summary>
        ///  科目二教练名称
        /// </summary>
        public string ThemeTwoCoachName { get; set; }


        /// <summary>
        /// 科目三时间
        /// </summary>
        public Nullable<DateTime> ThemeThreeDate { get; set; }

        /// <summary>
        /// 科目三是否通过
        /// </summary>
        public string ThemeThreePass { get; set; }

        /// <summary>
        /// 科目三学时状态
        /// </summary>
        public string ThemeThreeTimeCode { get; set; }
        
        /// <summary>
        ///  科目三教练名称
        /// </summary>
        public string ThemeThreeCoachName { get; set; }


        /// <summary>
        /// 科目四时间
        /// </summary>
        public Nullable<DateTime> ThemeFourDate { get; set; }

        /// <summary>
        /// 科目四是否通过
        /// </summary>
        public string ThemeFourPass { get; set; }


        #endregion

        /// <summary>
        /// 学员状态
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// 退学时间
        /// </summary>

        public Nullable<DateTime> DropOutDate { get; set; }
        

        /// <summary>
        /// 当前科目
        /// </summary>
        public string NowTheme { get; set; }
        
    }
}
