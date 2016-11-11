namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 学员
    /// </summary>
    [Table("Student")]
    public partial class Student : BaseEntity
    {


        #region 基本信息
  
        /// <summary>
        /// 学员姓名
        /// </summary>
        [Display(Name = "学员姓名")]
        [MaxLength(32)]
        [Required(ErrorMessage = "学员姓名不能为空")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }


        /// <summary>
        /// 身份证号码
        /// </summary>
        [Display(Name = "身份证号码")]
        [MaxLength(32)]
        [Required(ErrorMessage = "身份证号码不能为空")]
        [Column("IDCard", TypeName = "varchar")]
        public string IDCard { get; set; }

        /// <summary>
        /// 性别
        /// </summary>        
        public GenderCode GenderCode { get; set; }

        /// <summary>
        /// 省份编码
        /// </summary>
        [MaxLength(6)]
        [Column("ProvinceCode", TypeName = "varchar")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [MaxLength(6)]
        [Column("CityCode", TypeName = "varchar")]
        public string CityCode { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        [NotMapped]
        public string ProvinceName { get; set; } 

        /// <summary>
        /// 城市称
        /// </summary>
        [NotMapped]
        public string CityName { get; set; }

        /// <summary>
        /// 地址
        /// </summary>
        [MaxLength(512)]
        [Display(Name = "地址")]
        [Column("Address", TypeName = "varchar")]
        public string Address { get; set; }



        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(11)]
        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }

        /// <summary>
        /// 证书ID
        /// </summary>
        [Required(ErrorMessage = "证书不能为空")]
        [Column("CertificateID", TypeName = "char"), MaxLength(32)]
        public string CertificateID { get; set; }


        /// <summary>
        /// 证书名称
        /// </summary>
        [NotMapped]
        public string CertificateName { get; set; }

        #endregion


        #region 学车事宜
        /// <summary>
        /// 报名点ID
        /// </summary>
        [Required(ErrorMessage = "报名点ID不能为空")]
        [Column("EnteredPointID", TypeName = "char"), MaxLength(32)]
        public string EnteredPointID { get; set; }


        /// <summary>
        /// 报名点名称
        /// </summary>
        [NotMapped]
        public string EnteredPointName { get; set; }

        /// <summary>
        /// 推荐人ID
        /// </summary>
        [Required(ErrorMessage = "报名点ID不能为空")]
        [Column("ReferenceID", TypeName = "char"), MaxLength(32)]
        public string ReferenceID { get; set; }

        /// <summary>
        /// 推荐人名称
        /// </summary>
        [NotMapped]
        public string ReferenceName { get; set; }

        /// <summary>
        /// 意向驾校ID
        /// </summary>
        [Column("WantDriverShopID", TypeName = "char"), MaxLength(32)]
        public string WantDriverShopID { get; set; }

        /// <summary>
        /// 意向驾校名称
        /// </summary>
        [NotMapped]
        public string WantDriverShopName { get; set; }

        /// <summary>
        /// 培训班别ID
        /// </summary>
        [Required(ErrorMessage = "培训班别ID不能为空")]
        [Column("TrianID", TypeName = "char"), MaxLength(32)]
        public string TrianID { get; set; }

        /// <summary>
        /// 培训班别名称
        /// </summary>
        [NotMapped]
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
        public YesOrNoCode MoneyIsFull { get; set; } = YesOrNoCode.No;


        /// <summary>
        /// 缴费方式ID
        /// </summary>
        [Required(ErrorMessage = "培训班别ID不能为空")]
        [Column("PayMethodID", TypeName = "char"), MaxLength(32)]
        public string PayMethodID { get; set; }

        /// <summary>
        /// 缴费方式名称
        /// </summary>
        [NotMapped]
        public string PayMethodName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 报名地点（省份）
        /// </summary>
        [MaxLength(6)]
        [Column("EnteredProvinceCode", TypeName = "varchar")]
        public string EnteredProvinceCode { get; set; }
        /// <summary>
        /// 报名地点（省份）
        /// </summary>
        [NotMapped]
        public string EnteredProvinceName { get; set; }


        /// <summary>
        /// 报名地点（市）
        /// </summary>
        [MaxLength(6)]
        [Column("EnteredCityCode", TypeName = "varchar")]
        public string EnteredCityCode { get; set; }

        /// <summary>
        /// 报名时间
        /// </summary>
        public DateTime EnteredDate { get; set; }

        /// <summary>
        /// 报名地点（市）
        /// </summary>
        [NotMapped]
        public string EnteredCityName { get; set; }



        /// <summary>
        /// 制卡驾校ID
        /// </summary>
        [Column("MakeDriverShopID", TypeName = "char"), MaxLength(32)]
        public string MakeDriverShopID { get; set; }

        /// <summary>
        /// 制卡驾校名称
        /// </summary>
        [NotMapped]
        public string MakeDriverShopName { get; set; }

        /// <summary>
        /// 制卡日期
        /// </summary>
        public Nullable<DateTime> MakeCardDate { get; set; }

        /// <summary>
        /// 制卡城市编码
        /// </summary>
        [MaxLength(6)]
        [Column("MakeCardCityCode", TypeName = "varchar")]
        public string MakeCardCityCode { get; set; }

        /// <summary>
        /// 制卡地点名称
        /// </summary>
        [NotMapped]
        public string MakeCardCityName { get; set; }

        /// <summary>
        /// 制卡备注
        /// </summary>
        [MaxLength(128)]
        [Column("MakeCardRemark", TypeName = "varchar")]
        public string MakeCardRemark { get; set; }


        /// <summary>
        /// 是否增驾
        /// </summary>

        public YesOrNoCode IsAddCertificate { get; set; }

        /// <summary>
        /// 原驾车型
        /// </summary>
        [MaxLength(32)]
        [Column("OldCertificate", TypeName = "varchar")]
        public string OldCertificate { get; set; }
        #endregion

        #region 科目情况



        /// <summary>
        /// 科目一时间
        /// </summary>
        public Nullable<DateTime> ThemeOneDate { get; set; }     

        /// <summary>
        /// 科目一是否通过
        /// </summary>
        public YesOrNoCode ThemeOnePass { get; set; }


        /// <summary>
        /// 科目二时间
        /// </summary>
        public Nullable<DateTime> ThemeTwoDate { get; set; }


        /// <summary>
        /// 科目二是否通过
        /// </summary>
        public YesOrNoCode ThemeTwoPass { get; set; }

        /// <summary>
        /// 科目二学时状态
        /// </summary>
        public ThemeTimeCode ThemeTwoTimeCode { get; set; }

        /// <summary>
        /// 科目二教练
        /// </summary>
        [Column("ThemeTwoCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeTwoCoachID { get; set; }

        /// <summary>
        ///  科目二教练名称
        /// </summary>
        [NotMapped]
        public string ThemeTwoCoachName { get; set; }


        /// <summary>
        /// 科目三时间
        /// </summary>
        public Nullable<DateTime> ThemeThreeDate { get; set; }

        /// <summary>
        /// 科目三是否通过
        /// </summary>
        public YesOrNoCode ThemeThreePass { get; set; }

        /// <summary>
        /// 科目三学时状态
        /// </summary>
        public ThemeTimeCode ThemeThreeTimeCode { get; set; }

        /// <summary>
        /// 科目三教练
        /// </summary>
        [Column("ThemeThreeCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeThreeCoachID { get; set; }

        /// <summary>
        ///  科目三教练名称
        /// </summary>
        [NotMapped]
        public string ThemeThreeCoachName { get; set; }


        /// <summary>
        /// 科目四时间
        /// </summary>
        public Nullable<DateTime> ThemeFourDate { get; set; }

        /// <summary>
        /// 科目四是否通过
        /// </summary>
        public YesOrNoCode ThemeFourPass { get; set; }


        #endregion

        /// <summary>
        /// 学员状态
        /// </summary>
        public StudentCode State { get; set; }

        /// <summary>
        /// 退学时间
        /// </summary>

        public Nullable<DateTime> DropOutDate { get; set; }

        /// <summary>
        /// 退款记录
        /// </summary>
        [Column("DropOutPayOrderId", TypeName = "char"), MaxLength(32)]
        public string DropOutPayOrderId { get; set; }


        /// <summary>
        /// 当前科目
        /// </summary>
        public ThemeCode NowTheme { get; set; }


        /// <summary>
        /// 缴费记录
        /// </summary>
        [NotMapped]
        public List<PayOrder> PayOrderList { get; set; }
    }
}
