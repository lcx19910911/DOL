namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ת�˼�¼
    /// </summary>
    public partial class PayOrder : BaseEntity
    {

        /// <summary>
        /// ѧԱID
        /// </summary>
        [Required(ErrorMessage = "ѧԱID����Ϊ��")]
        [Column("StudentID", TypeName = "char"), MaxLength(32)]
        public string StudentID { get; set; }


        /// <summary>
        /// ת�˷���
        /// </summary>
        public decimal PayMoney { get; set; } = 0;


        /// <summary>
        /// ֧������ID
        /// </summary>
        [Required(ErrorMessage = "֧������ID����Ϊ��")]
        [Column("PayTypeID", TypeName = "char"), MaxLength(32)]
        public string PayTypeID { get; set; }

        /// <summary>
        /// ֧����������
        /// </summary>
        [NotMapped]
        public string PayTypeName { get; set; }


        /// <summary>
        /// ƾ֤��
        /// </summary>
        [MaxLength(32)]
        [Required(ErrorMessage = "ƾ֤�Ų���Ϊ��")]
        [Column("VoucherNO", TypeName = "varchar")]
        public string VoucherNO { get; set; }

        /// <summary>
        /// ƾ֤������ͼ
        /// </summary>
        [MaxLength(32)]
        [Column("VoucherThum", TypeName = "varchar")]
        public string VoucherThum { get; set; }

        /// <summary>
        /// �տ��˺�ID
        /// </summary>
        [Required(ErrorMessage = "�տ��˺�ID����Ϊ��")]
        [Column("AccountID", TypeName = "char"), MaxLength(32)]
        public string AccountID { get; set; }

        /// <summary>
        /// �տ��˺�����
        /// </summary>
        [NotMapped]
        public string AccountName { get; set; }

        

    }
}
