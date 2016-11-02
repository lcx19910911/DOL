
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
    /// 推荐人
    /// </summary>
    [Table("Reference")]
    public class Reference : BaseEntity
    {

        /// <summary>
        /// 推荐人姓名
        /// </summary>
        [Required(ErrorMessage = "推荐人姓名不能为空")]
        [MaxLength(32)]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 性别
        /// </summary>        
        public GenderCode GenderCode { get; set; }


        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(11)]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }


        /// <summary>
        /// 报名点权限
        /// </summary>
        [MaxLength(5120)]
        [Column("EnteredPointIDStr", TypeName = "varchar")]
        public string EnteredPointIDStr { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }
    }
}
