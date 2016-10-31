namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// 考试
    /// </summary>
    [Table("Exam")]
    public partial class Exam 
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        /// <summary>
        /// 学员id
        /// </summary>
        [Required(ErrorMessage = "学员id不能为空")]
        [Column("StudentID", TypeName = "char"), MaxLength(32)]
        public string StudentID { get; set; }

        /// <summary>
        /// 学员姓名
        /// </summary>
        [NotMapped]
        public string StudentName { get; set; }

        /// <summary>
        /// 考试次数
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///考试项目
        /// </summary>
        public ThemeCode Code { get; set; }


        /// <summary>
        /// 考试结果
        /// </summary>
        public ExamCode Result { get; set; }


        /// <summary>
        /// 创建时间
        /// </summary>
        [Display(Name = "创建时间")]
        [Required]
        public System.DateTime CreatedTime { get; set; }

    }
}
