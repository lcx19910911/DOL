namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 科目
    /// </summary>
    public partial class Theme : BaseEntity
    {

        /// <summary>
        /// 科目名称
        /// </summary>
        [Display(Name = "科目名称")]
        [MaxLength(32)]
        [Required(ErrorMessage = "科目不能为空")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        
    }
}
