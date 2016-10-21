
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    [Table("Department")]
    public class Department : BaseEntity
    {
        /// <summary>
        /// 父级ID
        /// </summary>
        [MaxLength(32)]
        [Column("ParentID", TypeName = "char")]
        public string ParentID { get; set; }

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
        /// 备注
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 所属部门
        /// </summary>
        [NotMapped]
        public string ParentName { get; set; }
    }
}
