namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ��ֵ��¼
    /// </summary>
    [Table("Recharge")]
    public partial class Recharge 
    {
        [Key]
        [Required]
        [MaxLength(32)]
        [Column("ID", TypeName = "char")]
        public string ID { get; set; }

        /// <summary>
        /// ��ֵ���
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// �Ϳ�id
        /// </summary>
        [Required(ErrorMessage = "�Ϳ�����Ϊ��")]
        [MaxLength(32)]
        [Column("OilID", TypeName = "char")]
        public string OilID { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [Required(ErrorMessage = "�����˲���Ϊ��")]
        [MaxLength(32)]
        [Column("CreatedUserID", TypeName = "char")]
        public string CreatedUserID { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [NotMapped]
        public string CreatedUserName { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [Display(Name = "����ʱ��")]
        [Required]
        public System.DateTime CreatedTime { get; set; }

    }
}
