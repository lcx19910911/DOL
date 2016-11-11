namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ��Ŀн��
    /// </summary>
    [Table("ThemeSalary")]
    public partial class ThemeSalary:BaseEntity
    {
        /// <summary>
        /// ��Ŀн������
        /// </summary>
        [Required(ErrorMessage = "��Ŀн�����Ʋ���Ϊ��")]
        [MaxLength(32)]
        [Display(Name = "��Ŀн�ʲ˵�����")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }

        /// <summary>
        /// ���Դ���
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// ��Ŀ
        /// </summary>
        public ThemeCode Code { get; set; }


        /// <summary>
        /// ���Խ��
        /// </summary>
        public decimal Money { get; set; }




        /// <summary>
        /// ʹ�ý���ʱ��
        /// </summary>
        [Display(Name = "ʹ�ý���ʱ��")]
        public System.DateTime? EndTime { get; set; }

    }
}
