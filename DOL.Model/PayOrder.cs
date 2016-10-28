namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ת�˼�¼
    /// </summary>
    [Table("PayOrder")]
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
        /// �˿����
        /// </summary>
        public decimal WantDropMoney { get; set; } = 0;

        /// <summary>
        /// ֧������ID
        /// </summary>
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
        [Column("VoucherNO", TypeName = "varchar")]
        public string VoucherNO { get; set; }

        /// <summary>
        /// ƾ֤������ͼ
        /// </summary>
        [MaxLength(256)]
        [Column("VoucherThum", TypeName = "varchar")]
        public string VoucherThum { get; set; }

        /// <summary>
        /// �տ��˺�ID
        /// </summary>
        [Column("AccountID", TypeName = "char"), MaxLength(32)]
        public string AccountID { get; set; }

        /// <summary>
        /// �տ��˺�����
        /// </summary>
        [NotMapped]
        public string AccountName { get; set; }

        /// <summary>
        /// �˿����
        /// </summary>
        public Nullable<DateTime> WantDropDate { get; set; }

        /// <summary>
        /// ֧��ʱ��
        /// </summary>

        public DateTime PayTime { get; set; }


        /// <summary>
        /// �˿��տ��˺�
        /// </summary>
        [Column("AccountNO", TypeName = "varchar"), MaxLength(32)]
        public string AccountNO { get; set; }


        /// <summary>
        /// �Ƿ�ȷ���տ�
        /// </summary>
        public YesOrNoCode IsConfirm { get; set; }

        /// <summary>
        /// �Ƿ��˿�
        /// </summary>
        public YesOrNoCode IsDrop { get; set; }
        /// <summary>
        /// ȷ����ID
        /// </summary>
        [Column("ConfirmUserID", TypeName = "char"), MaxLength(32)]
        public string ConfirmUserID { get; set; }
        /// <summary>
        /// ȷ��ʱ��
        /// </summary>
        public Nullable<DateTime> ConfirmDate { get; set; }

        /// <summary>
        /// ȷ��������
        /// </summary>
        [NotMapped]
        public string ConfirmUserName { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [Column("CreaterID", TypeName = "char"), MaxLength(32)]
        public string CreaterID { get; set; }





        /// <summary>
        /// �Ƽ�������
        /// </summary>
        [NotMapped]
        public string ReferenceName { get; set; }
        /// <summary>
        /// �ƿ���У����
        /// </summary>
        [NotMapped]
        public string MakeDriverShopName { get; set; }
        /// <summary>
        /// ֤������
        /// </summary>
        [NotMapped]
        public string CertificateName { get; set; }
        /// <summary>
        /// ����������
        /// </summary>
        [NotMapped]
        public string CreaterName { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [NotMapped]
        public DateTime EnteredDate { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [NotMapped]
        public string EnteredPointName { get; set; }

        /// <summary>
        /// �ƿ�����
        /// </summary>
        [NotMapped]
        public Nullable<DateTime> MakeCardDate { get; set; }

        /// <summary>
        /// ����
        /// </summary>
        [NotMapped]
        public decimal Money { get; set; }


        /// <summary>
        /// �ѽ�����
        /// </summary>
        [NotMapped]
        public decimal HadPayMoney { get; set; } = 0;

        /// <summary>
        /// ѧԱ����
        /// </summary>
        [NotMapped]
        public string Name { get; set; }


        /// <summary>
        /// ���֤����
        /// </summary>
        [NotMapped]
        public string IDCard { get; set; }


        /// <summary>
        /// �ֻ���
        /// </summary>
        [NotMapped]
        public string Mobile { get; set; }

    }
}
