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
        [Display(Name = "性别")]
        public GenderCode GenderCode { get; set; }

        /// <summary>
        /// 省份编码
        /// </summary>
        [Display(Name = "省份编码")]
        [MaxLength(6)]
        [Column("ProvinceCode", TypeName = "varchar")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// 城市编码
        /// </summary>
        [Display(Name = "城市编码")]
        [MaxLength(6)]
        [Column("CityCode", TypeName = "varchar")]
        public string CityCode { get; set; }
        /// <summary>
        /// 省份名称
        /// </summary>
        [NotMapped]
        [Display(Name = "省份名称")]
        public string ProvinceName { get; set; }

        /// <summary>
        /// 城市称
        /// </summary>
        [Display(Name = "城市称")]
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
        /// 制卡手机号
        /// </summary>
        [Display(Name = "制卡手机号")]
        [MaxLength(11)]
        [Required(ErrorMessage = "制卡手机号不能为空")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "制卡手机号格式不正确")]
        public string Mobile { get; set; }


        /// <summary>
        /// 制卡手机归属地
        /// </summary>
        [Display(Name = "制卡手机归属地")]
        [MaxLength(32)]
        [Column("MobileArea", TypeName = "varchar")]
        public string MobileArea { get; set; }

        /// <summary>
        /// 证书ID
        /// </summary>
        [Display(Name = "证书ID")]
        [Required(ErrorMessage = "证书不能为空")]
        [Column("CertificateID", TypeName = "char"), MaxLength(32)]
        public string CertificateID { get; set; }


        /// <summary>
        /// 证书名称
        /// </summary>
        [NotMapped]
        [Display(Name = "证书名称")]
        public string CertificateName { get; set; }

        #endregion


        #region 学车事宜
        /// <summary>
        /// 报名点ID
        /// </summary>
        [Display(Name = "报名点ID")]
        [Required(ErrorMessage = "报名点ID不能为空")]
        [Column("EnteredPointID", TypeName = "char"), MaxLength(32)]
        public string EnteredPointID { get; set; }


        /// <summary>
        /// 报名点名称
        /// </summary>
        [NotMapped]
        [Display(Name = "报名点名称")]
        public string EnteredPointName { get; set; }

        /// <summary>
        /// 推荐人ID
        /// </summary>
        [Display(Name = "推荐人ID")]
        [Required(ErrorMessage = "报名点ID不能为空")]
        [Column("ReferenceID", TypeName = "char"), MaxLength(32)]
        public string ReferenceID { get; set; }

        /// <summary>
        /// 推荐人名称
        /// </summary>
        [NotMapped]
        [Display(Name = "推荐人名称")]
        public string ReferenceName { get; set; }

        /// <summary>
        /// 意向驾校ID
        /// </summary>
        [Display(Name = "意向驾校ID")]
        [Column("WantDriverShopID", TypeName = "char"), MaxLength(32)]
        public string WantDriverShopID { get; set; }

        /// <summary>
        /// 意向驾校名称
        /// </summary>
        [NotMapped]
        [Display(Name = "意向驾校名称")]
        public string WantDriverShopName { get; set; }

        /// <summary>
        /// 培训班别ID
        /// </summary>
        [Display(Name = "培训班别ID")]
        [Required(ErrorMessage = "培训班别ID不能为空")]
        [Column("TrianID", TypeName = "char"), MaxLength(32)]
        public string TrianID { get; set; }

        /// <summary>
        /// 培训班别名称
        /// </summary>
        [NotMapped]
        [Display(Name = "身份证号码")]
        public string TrianName { get; set; }


        /// <summary>
        /// 费用
        /// </summary>
        [Display(Name = "费用")]
        public decimal Money { get; set; }


        /// <summary>
        /// 已交费用
        /// </summary>
        [Display(Name = "已交费用")]
        public decimal HadPayMoney { get; set; } = 0;

        /// <summary>
        /// 是否缴清费用
        /// </summary>
        [Display(Name = "是否缴清费用")]
        public YesOrNoCode MoneyIsFull { get; set; } = YesOrNoCode.No;


        /// <summary>
        /// 缴费方式ID
        /// </summary>
        [Display(Name = "缴费方式ID")]
        [Required(ErrorMessage = "缴费方式ID不能为空")]
        [Column("PayMethodID", TypeName = "char"), MaxLength(32)]
        public string PayMethodID { get; set; }

        /// <summary>
        /// 缴费方式名称
        /// </summary>
        [NotMapped]
        [Display(Name = "缴费方式名称")]
        public string PayMethodName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 报名地点（省份）
        /// </summary>
        [Display(Name = "报名地点（省份）")]
        [MaxLength(6)]
        [Column("EnteredProvinceCode", TypeName = "varchar")]
        public string EnteredProvinceCode { get; set; }
        /// <summary>
        /// 报名地点（省份）
        /// </summary>
        [NotMapped]
        [Display(Name = "报名地点（省份）")]
        public string EnteredProvinceName { get; set; }


        /// <summary>
        /// 报名地点（市）
        /// </summary>
        [Display(Name = "报名地点（市）")]
        [MaxLength(6)]
        [Column("EnteredCityCode", TypeName = "varchar")]
        public string EnteredCityCode { get; set; }

        /// <summary>
        /// 报名时间
        /// </summary>
        [Display(Name = "报名时间")]
        public DateTime EnteredDate { get; set; }

        /// <summary>
        /// 报名地点（市）
        /// </summary>
        [NotMapped]
        [Display(Name = "报名地点（市）")]
        public string EnteredCityName { get; set; }



        /// <summary>
        /// 制卡驾校ID
        /// </summary>
        [Display(Name = "制卡驾校ID")]
        [Column("MakeDriverShopID", TypeName = "char"), MaxLength(32)]
        public string MakeDriverShopID { get; set; }

        /// <summary>
        /// 制卡驾校名称
        /// </summary>
        [NotMapped]
        [Display(Name = "制卡驾校名称")]
        public string MakeDriverShopName { get; set; }

        /// <summary>
        /// 制卡日期
        /// </summary>
        [Display(Name = "制卡日期")]
        public Nullable<DateTime> MakeCardDate { get; set; }

        /// <summary>
        /// 制卡城市编码
        /// </summary>
        [Display(Name = "制卡城市编码")]
        [MaxLength(6)]
        [Column("MakeCardCityCode", TypeName = "varchar")]
        public string MakeCardCityCode { get; set; }

        /// <summary>
        /// 制卡地点名称
        /// </summary>
        [NotMapped]
        [Display(Name = "制卡地点名称")]
        public string MakeCardCityName { get; set; }

        /// <summary>
        /// 制卡备注
        /// </summary>
        [Display(Name = "制卡备注")]
        [MaxLength(128)]
        [Column("MakeCardRemark", TypeName = "varchar")]
        public string MakeCardRemark { get; set; }


        /// <summary>
        /// 联系手机号
        /// </summary>
        [Display(Name = "联系手机号")]
        [MaxLength(11)]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "手机格式不正确")]
        public string ConactMobile { get; set; }


        /// <summary>
        /// 高校id
        /// </summary>
        [Display(Name = "高校id")]
        [Column("SchoolID", TypeName = "char"), MaxLength(32)]
        public string SchoolID { get; set; }

        [NotMapped]
        public string SchoolName { get; set; }
        /// <summary>
        /// 学院id
        /// </summary>
        [Display(Name = "学院id")]
        [Column("CollegeID", TypeName = "char"), MaxLength(32)]
        public string CollegeID { get; set; }
        [NotMapped]
        public string CollegeName { get; set; }

        /// <summary>
        /// 专业id
        /// </summary>
        [Display(Name = "专业id")]
        [Column("MajorID", TypeName = "char"), MaxLength(32)]
        public string MajorID { get; set; }
        [NotMapped]
        public string MajorName { get; set; }
        /// <summary>
        /// 年级
        /// </summary>
        [Display(Name = "年级")]
        [Column("SchoolAge", TypeName = "varchar"), MaxLength(32)]
        public string SchoolAge { get; set; }

        /// <summary>
        /// 生源
        /// </summary>
        public FromCode From { get; set; }

        #endregion

        #region 科目情况



        /// <summary>
        /// 科目一时间
        /// </summary>
        [Display(Name = "科目一时间")]
        public Nullable<DateTime> ThemeOneDate { get; set; }

        /// <summary>
        /// 科目一是否通过
        /// </summary>
        [Display(Name = "科目一是否通过")]
        public YesOrNoCode ThemeOnePass { get; set; }


        /// <summary>
        /// 科目二时间
        /// </summary>
        [Display(Name = "科目二时间")]
        public Nullable<DateTime> ThemeTwoDate { get; set; }


        /// <summary>
        /// 科目二是否通过
        /// </summary>
        [Display(Name = "科目二是否通过")]
        public YesOrNoCode ThemeTwoPass { get; set; }

        /// <summary>
        /// 科目二学时状态
        /// </summary>
        [Display(Name = "科目二学时状态")]
        public ThemeTimeCode ThemeTwoTimeCode { get; set; }

        /// <summary>
        /// 科目二教练
        /// </summary>
        [Display(Name = "科目二教练")]
        [Column("ThemeTwoCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeTwoCoachID { get; set; }

        public YesOrNoCode ThemeTwoConfirm { get; set; }

        /// <summary>
        ///  科目二教练名称
        /// </summary>
        [NotMapped]
        [Display(Name = "科目二教练名称")]
        public string ThemeTwoCoachName { get; set; }


        /// <summary>
        /// 科目三时间
        /// </summary>
        [Display(Name = "科目三时间")]
        public Nullable<DateTime> ThemeThreeDate { get; set; }

        /// <summary>
        /// 科目三是否通过
        /// </summary>
        [Display(Name = "科目三是否通过")]
        public YesOrNoCode ThemeThreePass { get; set; }

        public YesOrNoCode ThemeThreeConfirm { get; set; }
        /// <summary>
        /// 科目三学时状态
        /// </summary>
        [Display(Name = "科目三学时状态")]
        public ThemeTimeCode ThemeThreeTimeCode { get; set; }

        /// <summary>
        /// 科目三教练
        /// </summary>
        [Display(Name = "科目三教练")]
        [Column("ThemeThreeCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeThreeCoachID { get; set; }

        /// <summary>
        ///  科目三教练名称
        /// </summary>
        [Display(Name = "科目三教练名称")]
        [NotMapped]
        public string ThemeThreeCoachName { get; set; }


        /// <summary>
        /// 科目四时间
        /// </summary>
        [Display(Name = "科目四时间")]
        public Nullable<DateTime> ThemeFourDate { get; set; }

        /// <summary>
        /// 科目四是否通过
        /// </summary>
        [Display(Name = "科目四是否通过")]
        public YesOrNoCode ThemeFourPass { get; set; }


        #endregion

        /// <summary>
        /// 学员状态
        /// </summary>
        [Display(Name = "学员状态")]
        public StudentCode State { get; set; }

        /// <summary>
        /// 退学时间
        /// </summary>
        [Display(Name = "退学时间")]

        public Nullable<DateTime> DropOutDate { get; set; }

        /// <summary>
        /// 退款记录id
        /// </summary>
        [Display(Name = "退款记录id")]
        [Column("DropOutPayOrderId", TypeName = "char"), MaxLength(32)]
        public string DropOutPayOrderId { get; set; }


        /// <summary>
        /// 当前科目
        /// </summary>
        [Display(Name = "当前科目")]
        public ThemeCode NowTheme { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        [NotMapped]
        public int ExamCount { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        [NotMapped]
        public int OtherExamCount { get; set; }

        /// <summary>
        /// 缴费记录
        /// </summary>
        [Display(Name = "缴费记录")]
        [NotMapped]
        public List<PayOrder> PayOrderList { get; set; }

        [NotMapped]
        public decimal DoConfirmMoney { get; set; }


        [Required]
        [Display(Name = "创建者id")]
        [Column("CreaterID", TypeName = "char"), MaxLength(32)]
        public string CreaterID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [NotMapped]
        public string CreaterName { get; set; }


    }
}
