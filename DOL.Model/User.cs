namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    [Table("User")]
    public partial class User: BaseEntity
    {
        /// <summary>
        /// ����ID
        /// </summary>
        [Required(ErrorMessage = "����ID����Ϊ��")]
        [Column("DepartmentID", TypeName = "char"), MaxLength(32)]
        public string DepartmentID { get; set; }
        /// <summary>
        /// ״̬
        /// </summary>
        [NotMapped]
        public string DepartmentName { get; set; }
        /// <summary>
        /// ��ɫ
        /// </summary>
        [Required(ErrorMessage = "��ɫ����Ϊ��")]
        [Column("RoleID", TypeName = "char"), MaxLength(32)]
        public string RoleID { get; set; }


        /// <summary>
        /// ״̬
        /// </summary>
        [NotMapped]
        public string RoleName { get; set; }

        /// <summary>
        /// �Ƿ����Ա
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// ����Ȩ��
        /// </summary>
        public long? OperateFlag { get; set; } 


        /// <summary>
        /// ������Id
        /// </summary>
        [Display(Name = "������Id")]
        [Column("CreaterId", TypeName = "char"), MaxLength(32)]
        public string CreaterId { get; set; }

        /// <summary>
        /// �˺�
        /// </summary>
        [Display(Name = "�˺�")]
        [MaxLength(12)]
        [Required(ErrorMessage ="��½�˺Ų���Ϊ��")]
        [Column("Account", TypeName = "varchar")]
        public string Account { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Display(Name = "����")]
        [MaxLength(32)]
        [Required(ErrorMessage = "���Ʋ���Ϊ��")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }
        /// <summary>
        /// ����
        /// </summary>
        [Display(Name = "����")]
        [MaxLength(128)]
        [Required(ErrorMessage = "���벻��Ϊ��")]
        [Column("Password", TypeName = "varchar")]
        public string Password { get; set; }
        /// <summary>
        /// �ֻ���
        /// </summary>
        [Display(Name = "�ֻ���")]
        [MaxLength(11)]
        [Required(ErrorMessage = "�ֻ��Ų���Ϊ��")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "�ֻ���ʽ����ȷ")]
        public string Mobile { get; set; }
        /// <summary>
        /// ��ע
        /// </summary>
        [Display(Name = "��ע")]
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// ����������
        /// </summary>
        [Display(Name = "����������")]
        [MaxLength(12),MinLength(6)]
        [NotMapped]
        public string NewPassword { get; set; }

        /// <summary>
        /// �ٴ���������
        /// </summary>
        [Display(Name = "�ٴ���������")]
        [MaxLength(12), MinLength(6),Compare("NewPassword",ErrorMessage="�����������벻һ��")]
        [NotMapped]
        public string ConfirmPassword { get; set; }
       

        /// <summary>
        /// ״̬
        /// </summary>
        [NotMapped]
        public string State { get; set; }

        /// <summary>
        /// ����Աid
        /// </summary>
        [Column("CoachID", TypeName = "char"), MaxLength(32)]
        public string CoachID { get; set; }

        /// <summary>
        /// Ȩ�޼���
        /// </summary>
        [NotMapped]
        public List<string> OperateList { get; set; }

        /// <summary>
        /// �����㼯��
        /// </summary>
        [Column("EnteredPointIDStr", TypeName = "text")]
        public string EnteredPointIDStr { get; set; }

        /// <summary>
        /// ҳ��Ȩ��
        /// </summary>
        [Column("MenuIDStr", TypeName = "text")]
        public string MenuIDStr { get; set; }

        /// <summary>
        /// �Ƿ����ŵ����Ա
        /// </summary>
        public YesOrNoCode IsStoreAdmin { get; set; } = YesOrNoCode.No;



        /// <summary>
        /// �Ƿ�չʾ���
        /// </summary>
        [NotMapped]
        public bool IsNotShowMoney { get; set; }

        public DateTime QuitTime { get; set; }
    }
}
