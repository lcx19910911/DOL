
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 教练员
    /// </summary>
    [Table("Coach")]
    public class Coach : BaseEntity
    {

        /// <summary>
        /// 教练员姓名
        /// </summary>
        [Required(ErrorMessage = "教练员姓名不能为空")]
        [MaxLength(32)]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 身份证号码
        /// </summary>
        [Display(Name = "身份证号码")]
        [MaxLength(32)]
        [Required(ErrorMessage = "身份证号码不能为空")]
        [Column("IDCard", TypeName = "varchar")]
        [RegularExpression(@"(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$)", ErrorMessage = "手机格式不正确")]
        public string IDCard { get; set; }

        /// <summary>
        /// 身份证地址
        /// </summary>
        [MaxLength(512)]
        [Display(Name = "地址")]
        [Column("IDCardsAddress", TypeName = "varchar")]
        public string IDCardsAddress { get; set; }

        /// <summary>
        /// 户籍地址
        /// </summary>
        [MaxLength(512)]
        [Display(Name = "地址")]
        [Column("HouseholdAddress", TypeName = "varchar")]
        public string HouseholdAddress { get; set; }


        /// <summary>
        /// 性别
        /// </summary>        
        public GenderCode GenderCode { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(11)]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public Nullable<DateTime> EntryDate { get; set; }


        /// <summary>
        /// 入职信息
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 教练证信息
        /// </summary>
        [MaxLength(512)]
        [Column("CertificateInfo", TypeName = "varchar")]
        public string CertificateInfo { get; set; }

        /// <summary>
        /// 驾龄
        /// </summary>
        public int DrivingYears { get; set; }

        /// <summary>
        /// 档案编号
        /// </summary>
        [MaxLength(128)]
        [Column("ArchivesNO", TypeName = "varchar")]
        public string ArchivesNO { get; set; }
        

        /// <summary>
        /// 教龄
        /// </summary>
        public int TrainYears { get; set; }

        /// <summary>
        /// 初次申领时间
        /// </summary>
        public Nullable<DateTime> FirstClaimDate { get; set; }

        /// <summary>
        /// 入职时间
        /// </summary>
        public Nullable<DateTime> ReportDate { get; set; }

        /// <summary>
        /// 科目二通过数
        /// </summary>

        public int ThemeTwoCount { get; set; }
        
        /// <summary>
        /// 科目三通过数
        /// </summary>

        public int ThemeThreeCount { get; set; }
    }
}
