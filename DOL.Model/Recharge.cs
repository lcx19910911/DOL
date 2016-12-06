namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 充值记录
    /// </summary>
    [Table("Recharge")]
    public partial class Recharge 
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 油卡id
        /// </summary>
        [Required(ErrorMessage = "油卡不能为空")]
        [MaxLength(32)]
        [Column("OilID", TypeName = "char")]
        public string OilID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [Required(ErrorMessage = "创建人不能为空")]
        [MaxLength(32)]
        [Column("CreatedUserID", TypeName = "char")]
        public string CreatedUserID { get; set; }

        /// <summary>
        /// 创建人
        /// </summary>
        [NotMapped]
        public string CreatedUserName { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Required]
        public System.DateTime CreatedTime { get; set; }

    }
}
