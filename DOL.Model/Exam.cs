namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ����
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
        /// ѧԱid
        /// </summary>
        [Required(ErrorMessage = "ѧԱid����Ϊ��")]
        [Column("StudentID", TypeName = "char"), MaxLength(32)]
        public string StudentID { get; set; }

        /// <summary>
        /// ѧԱ����
        /// </summary>
        [NotMapped]
        public string StudentName { get; set; }

        /// <summary>
        /// ���Դ���
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        ///������Ŀ
        /// </summary>
        public ThemeCode Code { get; set; }


        /// <summary>
        /// ���Խ��
        /// </summary>
        public ExamCode Result { get; set; }


        /// <summary>
        /// ����ʱ��
        /// </summary>
        [Display(Name = "����ʱ��")]
        [Required]
        public System.DateTime CreatedTime { get; set; }

    }
}
