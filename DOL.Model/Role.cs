
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    [Table("Role")]
    public class Role : BaseEntity
    {

        /// <summary>
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [MaxLength(32)]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 页面权限
        /// </summary>
        [Column("MenuIDStr", TypeName = "text")]
        public string MenuIDStr { get; set; }

        /// <summary>
        /// 操作权限
        /// </summary>
        public long? OperateFlag { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 是否展示金额
        /// </summary>
        public bool IsNotShowMoney { get; set; }

        [NotMapped]
        public string IsNotShowMoneyStr { get; set; }
    }
}
