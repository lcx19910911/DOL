
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
    /// 操作权限
    /// </summary>
    [Table("Operate")]
    public class Operate : BaseEntity
    {

        /// <summary>
        /// 权限位域值
        /// </summary>
        public long? LimitFlag { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>  
        [Required(ErrorMessage = "菜单ID不能为空")]
        [Column("MenuID", TypeName = "char"), MaxLength(32)]
        public string MenuID { get; set; }

        /// <summary>
        /// 操作名称
        /// </summary>
        [MaxLength(32)]
        [Required(ErrorMessage = "操作名称不能为空")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }


        /// <summary>
        /// 控制器地址
        /// </summary>
        [MaxLength(64)]
        [Required(ErrorMessage = "控制器地址不能为空")]
        [Column("ActionUrl", TypeName = "varchar")]
        public string ActionUrl { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Sort { get; set; }

        [NotMapped]
        public string MenuName { get; set; }
    }
}
