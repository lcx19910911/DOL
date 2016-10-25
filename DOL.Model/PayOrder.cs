namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 转账记录
    /// </summary>
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
        /// 支付渠道ID
        /// </summary>
        [Required(ErrorMessage = "支付渠道ID不能为空")]
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
        [Required(ErrorMessage = "凭证号不能为空")]
        [Column("VoucherNO", TypeName = "varchar")]
        public string VoucherNO { get; set; }

        /// <summary>
        /// 凭证号缩略图
        /// </summary>
        [MaxLength(32)]
        [Column("VoucherThum", TypeName = "varchar")]
        public string VoucherThum { get; set; }

        /// <summary>
        /// 收款账号ID
        /// </summary>
        [Required(ErrorMessage = "收款账号ID不能为空")]
        [Column("AccountID", TypeName = "char"), MaxLength(32)]
        public string AccountID { get; set; }

        /// <summary>
        /// 收款账号名称
        /// </summary>
        [NotMapped]
        public string AccountName { get; set; }

        

    }
}
