
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
    /// 支出
    /// </summary>
    [Table("Expend")]
    public class Expend : BaseEntity
    {


        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money { get; set; }


        /// <summary>
        /// 耗损项目
        /// </summary>
        [Required(ErrorMessage = "耗损项目不能为空")]
        [MaxLength(32)]
        [Column("ThingID", TypeName = "char")]
        public string ThingID { get; set; }

        [NotMapped]
        public string ThingName { get; set; }


        /// <summary>
        /// 批次
        /// </summary>
        [Required(ErrorMessage = "批次不能为空")]
        [MaxLength(32)]
        [Column("NO", TypeName = "varchar")]
        public string NO { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 记录时间
        /// </summary>
        public DateTime AddDate { get; set; }
    }
}
