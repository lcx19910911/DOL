
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
        /// 名称
        /// </summary>
        [Required(ErrorMessage = "名称不能为空")]
        [MaxLength(32)]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }

        /// <summary>
        /// 权限位域值
        /// </summary>
        public long? LimitFlag { get; set; }

        /// <summary>
        /// 链接
        /// </summary>
        [Required(ErrorMessage = "链接不能为空")]
        [MaxLength(64)]
        [Column("Link", TypeName = "varchar")]
        public string Link { get; set; }

        /// <summary>
        /// 父级ID
        /// </summary>
        [MaxLength(32)]
        [Column("ParentID", TypeName = "char")]
        public string ParentID { get; set; }

        /// <summary>
        /// 类名称
        /// </summary>
        [MaxLength(32)]
        [Column("ClassName", TypeName = "varchar")]
        public string ClassName { get; set; }
    }
}
