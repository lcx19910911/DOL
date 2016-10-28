namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 转账记录
    /// </summary>
    [Table("PayOrder")]
    public partial class PayOrder : BaseEntity
    {

        /// <summary>
        /// 学员ID
        /// </summary>
        [Required(ErrorMessage = "学员ID不能为空")]
        [Column("StudentID", TypeName = "char"), MaxLength(32)]
        public string StudentID { get; set; }


        /// <summary>
        /// 转账费用
        /// </summary>
        public decimal PayMoney { get; set; } = 0;


        /// <summary>
        /// 退款费用
        /// </summary>
        public decimal WantDropMoney { get; set; } = 0;

        /// <summary>
        /// 支付渠道ID
        /// </summary>
        [Column("PayTypeID", TypeName = "char"), MaxLength(32)]
        public string PayTypeID { get; set; }

        /// <summary>
        /// 支付渠道名称
        /// </summary>
        [NotMapped]
        public string PayTypeName { get; set; }


        /// <summary>
        /// 凭证号
        /// </summary>
        [MaxLength(32)]
        [Column("VoucherNO", TypeName = "varchar")]
        public string VoucherNO { get; set; }

        /// <summary>
        /// 凭证号缩略图
        /// </summary>
        [MaxLength(256)]
        [Column("VoucherThum", TypeName = "varchar")]
        public string VoucherThum { get; set; }

        /// <summary>
        /// 收款账号ID
        /// </summary>
        [Column("AccountID", TypeName = "char"), MaxLength(32)]
        public string AccountID { get; set; }

        /// <summary>
        /// 收款账号名称
        /// </summary>
        [NotMapped]
        public string AccountName { get; set; }

        /// <summary>
        /// 退款费用
        /// </summary>
        public Nullable<DateTime> WantDropDate { get; set; }

        /// <summary>
        /// 支付时间
        /// </summary>

        public DateTime PayTime { get; set; }


        /// <summary>
        /// 退款收款账号
        /// </summary>
        [Column("AccountNO", TypeName = "varchar"), MaxLength(32)]
        public string AccountNO { get; set; }


        /// <summary>
        /// 是否确认收款
        /// </summary>
        public YesOrNoCode IsConfirm { get; set; }

        /// <summary>
        /// 是否退款
        /// </summary>
        public YesOrNoCode IsDrop { get; set; }
        /// <summary>
        /// 确认人ID
        /// </summary>
        [Column("ConfirmUserID", TypeName = "char"), MaxLength(32)]
        public string ConfirmUserID { get; set; }
        /// <summary>
        /// 确认时间
        /// </summary>
        public Nullable<DateTime> ConfirmDate { get; set; }

        /// <summary>
        /// 确认人名称
        /// </summary>
        [NotMapped]
        public string ConfirmUserName { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Column("CreaterID", TypeName = "char"), MaxLength(32)]
        public string CreaterID { get; set; }





        /// <summary>
        /// 推荐人名称
        /// </summary>
        [NotMapped]
        public string ReferenceName { get; set; }
        /// <summary>
        /// 制卡驾校名称
        /// </summary>
        [NotMapped]
        public string MakeDriverShopName { get; set; }
        /// <summary>
        /// 证书名称
        /// </summary>
        [NotMapped]
        public string CertificateName { get; set; }
        /// <summary>
        /// 创建人名称
        /// </summary>
        [NotMapped]
        public string CreaterName { get; set; }

        /// <summary>
        /// 报名时间
        /// </summary>
        [NotMapped]
        public DateTime EnteredDate { get; set; }

        /// <summary>
        /// 报名点
        /// </summary>
        [NotMapped]
        public string EnteredPointName { get; set; }

        /// <summary>
        /// 制卡日期
        /// </summary>
        [NotMapped]
        public Nullable<DateTime> MakeCardDate { get; set; }

        /// <summary>
        /// 费用
        /// </summary>
        [NotMapped]
        public decimal Money { get; set; }


        /// <summary>
        /// 已交费用
        /// </summary>
        [NotMapped]
        public decimal HadPayMoney { get; set; } = 0;

        /// <summary>
        /// 学员姓名
        /// </summary>
        [NotMapped]
        public string Name { get; set; }


        /// <summary>
        /// 身份证号码
        /// </summary>
        [NotMapped]
        public string IDCard { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        [NotMapped]
        public string Mobile { get; set; }

    }
}
