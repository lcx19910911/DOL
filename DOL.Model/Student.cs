namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ѧԱ
    /// </summary>
    public partial class Student : BaseEntity
    {


        #region ������Ϣ
  
        /// <summary>
        /// ѧԱ����
        /// </summary>
        [Display(Name = "ѧԱ����")]
        [MaxLength(32)]
        [Required(ErrorMessage = "ѧԱ��������Ϊ��")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }


        /// <summary>
        /// ���֤����
        /// </summary>
        [Display(Name = "���֤����")]
        [MaxLength(32)]
        [Required(ErrorMessage = "���֤���벻��Ϊ��")]
        [Column("IDCard", TypeName = "varchar")]
        [RegularExpression(@"(^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$)", ErrorMessage = "�ֻ���ʽ����ȷ")]
        public string IDCard { get; set; }

        /// <summary>
        /// �Ա�
        /// </summary>        
        public GenderCode GenderCode { get; set; }

        /// <summary>
        /// ʡ�ݱ���
        /// </summary>
        [MaxLength(6)]
        [Column("ProvinceCode", TypeName = "varchar")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// ���б���
        /// </summary>
        [MaxLength(6)]
        [Column("CityCode", TypeName = "varchar")]
        public string CityCode { get; set; }
        /// <summary>
        /// ʡ������
        /// </summary>
        [NotMapped]
        public string ProvinceName { get; set; }

        /// <summary>
        /// ���г�
        /// </summary>
        [NotMapped]
        public string CityName { get; set; }

        /// <summary>
        /// ��ַ
        /// </summary>
        [MaxLength(512)]
        [Display(Name = "��ַ")]
        [Column("Address", TypeName = "varchar")]
        public string Address { get; set; }



        /// <summary>
        /// �ֻ���
        /// </summary>
        [Display(Name = "�ֻ���")]
        [MaxLength(11)]
        [Required(ErrorMessage = "�ֻ��Ų���Ϊ��")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "�ֻ���ʽ����ȷ")]
        public string Mobile { get; set; }

        /// <summary>
        /// ֤��ID
        /// </summary>
        [Column("CertificateID", TypeName = "char"), MaxLength(32)]
        public string CertificateID { get; set; }


        /// <summary>
        /// ֤������
        /// </summary>
        [NotMapped]
        public string CertificateName { get; set; }

        #endregion


        #region ѧ������
        /// <summary>
        /// ������ID
        /// </summary>
        [Required(ErrorMessage = "������ID����Ϊ��")]
        [Column("EnteredPointID", TypeName = "char"), MaxLength(32)]
        public string EnteredPointID { get; set; }


        /// <summary>
        /// ����������
        /// </summary>
        [NotMapped]
        public string EnteredPointName { get; set; }

        /// <summary>
        /// �Ƽ���ID
        /// </summary>
        [Required(ErrorMessage = "������ID����Ϊ��")]
        [Column("ReferenceID", TypeName = "char"), MaxLength(32)]
        public string ReferenceID { get; set; }

        /// <summary>
        /// �Ƽ�������
        /// </summary>
        [NotMapped]
        public string ReferenceName { get; set; }

        /// <summary>
        /// �����УID
        /// </summary>
        [Required(ErrorMessage = "�����УID����Ϊ��")]
        [Column("WantDriverShopID", TypeName = "char"), MaxLength(32)]
        public string WantDriverShopID { get; set; }

        /// <summary>
        /// �����У����
        /// </summary>
        [NotMapped]
        public string WantDriverShopName { get; set; }

        /// <summary>
        /// ��ѵ���ID
        /// </summary>
        [Required(ErrorMessage = "��ѵ���ID����Ϊ��")]
        [Column("TrianID", TypeName = "char"), MaxLength(32)]
        public string TrianID { get; set; }

        /// <summary>
        /// ��ѵ�������
        /// </summary>
        [NotMapped]
        public string TrianName { get; set; }


        /// <summary>
        /// ����
        /// </summary>
        public decimal Money { get; set; }


        /// <summary>
        /// �ѽ�����
        /// </summary>
        public decimal HadPayMoney { get; set; } = 0;

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        public YesOrNoCode MoneyIsFull { get; set; } = YesOrNoCode.No;


        /// <summary>
        /// �ɷѷ�ʽID
        /// </summary>
        [Required(ErrorMessage = "��ѵ���ID����Ϊ��")]
        [Column("PayMethodID", TypeName = "char"), MaxLength(32)]
        public string PayMethodID { get; set; }

        /// <summary>
        /// �ɷѷ�ʽ����
        /// </summary>
        [NotMapped]
        public string PayMethodName { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }


        /// <summary>
        /// �����ص㣨�У�
        /// </summary>
        [MaxLength(6)]
        [Column("EnteredCityCode", TypeName = "varchar")]
        public string EnteredCityCode { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        public DateTime EnteredDate { get; set; }

        /// <summary>
        /// �����ص㣨�У�
        /// </summary>
        [NotMapped]
        public string EnteredCityName { get; set; }



        /// <summary>
        /// �ƿ���УID
        /// </summary>
        [Required(ErrorMessage = "�ƿ���УID����Ϊ��")]
        [Column("MakeDriverShopID", TypeName = "char"), MaxLength(32)]
        public string MakeDriverShopID { get; set; }

        /// <summary>
        /// �ƿ���У����
        /// </summary>
        [NotMapped]
        public string MakeDriverShopName { get; set; }

        /// <summary>
        /// �ƿ�����
        /// </summary>
        public Nullable<DateTime> MakeCardDate { get; set; }

        /// <summary>
        /// �ƿ����б���
        /// </summary>
        [MaxLength(6)]
        [Column("MakeCardCityCode", TypeName = "varchar")]
        public string MakeCardCityCode { get; set; }

        /// <summary>
        /// �ƿ��ص�����
        /// </summary>
        [NotMapped]
        public string MakeCardCityName { get; set; }

        /// <summary>
        /// �ƿ���ע
        /// </summary>
        [MaxLength(128)]
        [Column("MakeCardRemark", TypeName = "varchar")]
        public string MakeCardRemark { get; set; }

        #endregion

        #region ��Ŀ���



        /// <summary>
        /// ��Ŀһʱ��
        /// </summary>
        public Nullable<DateTime> ThemeOneDate { get; set; }

        /// <summary>
        /// ��Ŀһ����
        /// </summary>
        [Column("ThemeOneCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeOneCoachID { get; set; }

        /// <summary>
        ///  ��Ŀһ��������
        /// </summary>
        [NotMapped]
        public string ThemeOneCoachName { get; set; }

        /// <summary>
        /// ��Ŀ��ʱ��
        /// </summary>
        public Nullable<DateTime> ThemeTwoDate { get; set; }

        /// <summary>
        /// ��Ŀ������
        /// </summary>
        [Column("ThemeTwoCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeTwoCoachID { get; set; }

        /// <summary>
        ///  ��Ŀ����������
        /// </summary>
        [NotMapped]
        public string ThemeTwoCoachName { get; set; }

        /// <summary>
        /// ��Ŀ��ʱ��
        /// </summary>
        public Nullable<DateTime> ThemeThreeDate { get; set; }
        /// <summary>
        /// ��Ŀ������
        /// </summary>
        [Column("ThemeThreeCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeThreeCoachID { get; set; }

        /// <summary>
        ///  ��Ŀ����������
        /// </summary>
        [NotMapped]
        public string ThemeThreeCoachName { get; set; }
        /// <summary>
        /// ��Ŀ��ʱ��
        /// </summary>
        public Nullable<DateTime> ThemeFourDate { get; set; }

        /// <summary>
        /// ��Ŀ�Ľ���
        /// </summary>
        [Column("ThemeFourCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeFourCoachID { get; set; }

        /// <summary>
        ///  ��Ŀ�Ľ�������
        /// </summary>
        [NotMapped]
        public string ThemeFourCoachName { get; set; }

        /// <summary>
        /// ��ǰ��Ŀ
        /// </summary>
        public ThemeCode NowTheme { get; set; }

        #endregion


        /// <summary>
        /// ѧԱ״̬
        /// </summary>
        public StudentCode State { get; set; }
    }
}
