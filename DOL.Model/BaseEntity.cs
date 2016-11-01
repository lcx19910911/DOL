using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    //基础实体
    public class BaseEntity
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        /// <summary>
        /// 更新时间
        /// </summary>
        [Display(Name = "更新时间")]
        [Timestamp]
        public byte[] TimeStamp { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Required]
        public System.DateTime CreatedTime { get; set; }


        /// <summary>
        /// 修改人
        /// </summary>
        [Required(ErrorMessage = "修改人ID不能为空")]
        [Column("UpdatedID", TypeName = "char"), MaxLength(32)]
        public string UpdaterID { get; set; }
        /// <summary>
        /// 修改人
        /// </summary>
        [NotMapped]
        public string UpdaterName { get; set; }


        /// <summary>
        /// 修改时间
        /// </summary>
        [Display(Name = "修改时间")]
        [Required]
        public System.DateTime UpdatedTime { get; set; }

        /// <summary>
        /// 状态枚举
        /// </summary>
        public long Flag { get; set; }
    }
}
