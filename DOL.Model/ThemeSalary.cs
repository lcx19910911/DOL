namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 科目薪资
    /// </summary>
    [Table("ThemeSalary")]
    public partial class ThemeSalary:BaseEntity
    {
        /// <summary>
        /// 科目薪资名称
        /// </summary>
        [Required(ErrorMessage = "科目薪资名称不能为空")]
        [MaxLength(32)]
        [Display(Name = "科目薪资菜单名称")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 科目
        /// </summary>
        public ThemeCode Code { get; set; }


        /// <summary>
        /// 考试结果
        /// </summary>
        public decimal Money { get; set; }




        /// <summary>
        /// 使用截至时间
        /// </summary>
        [Display(Name = "使用截至时间")]
        public System.DateTime? EndTime { get; set; }

    }
}
