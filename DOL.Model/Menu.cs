
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    [Table("Menu")]
    public class Menu : BaseEntity
    {
        /// <summary>
        /// 菜单名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [MaxLength(32)]
        [Display(Name= "菜单名称")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [Display(Name = "排序")]
        public int? Sort { get; set; }

        /// <summary>
        /// 权限位域值
        /// </summary>
        public long? LimitFlag { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [MaxLength(64)]
        [Display(Name = "链接")]
        [Column("Link", TypeName = "varchar")]
        public string Link { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [MaxLength(32)]
        [Display(Name = "所属菜单")]
        [Column("ParentID", TypeName = "char")]
        public string ParentID { get; set; }

        /// <summary>
        /// 类名称
        /// </summary>
        [MaxLength(32)]
        [Display(Name = "类名称")]
        [Column("ClassName", TypeName = "varchar")]
        public string ClassName { get; set; }


        /// <summary>
        /// 所属菜单
        /// </summary>
        [NotMapped]
        public string ParentName { get; set; }
    }
}
