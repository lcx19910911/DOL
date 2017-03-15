namespace DOL.Model
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    /// <summary>
    /// ѧԱ
    /// </summary>
    [Table("Student")]
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
        public string IDCard { get; set; }

        /// <summary>
        /// �Ա�
        /// </summary>      
        [Display(Name = "�Ա�")]
        public GenderCode GenderCode { get; set; }

        /// <summary>
        /// ʡ�ݱ���
        /// </summary>
        [Display(Name = "ʡ�ݱ���")]
        [MaxLength(6)]
        [Column("ProvinceCode", TypeName = "varchar")]
        public string ProvinceCode { get; set; }

        /// <summary>
        /// ���б���
        /// </summary>
        [Display(Name = "���б���")]
        [MaxLength(6)]
        [Column("CityCode", TypeName = "varchar")]
        public string CityCode { get; set; }
        /// <summary>
        /// ʡ������
        /// </summary>
        [NotMapped]
        [Display(Name = "ʡ������")]
        public string ProvinceName { get; set; }

        /// <summary>
        /// ���г�
        /// </summary>
        [Display(Name = "���г�")]
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
        /// �ƿ��ֻ���
        /// </summary>
        [Display(Name = "�ƿ��ֻ���")]
        [MaxLength(11)]
        [Required(ErrorMessage = "�ƿ��ֻ��Ų���Ϊ��")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "�ƿ��ֻ��Ÿ�ʽ����ȷ")]
        public string Mobile { get; set; }


        /// <summary>
        /// �ƿ��ֻ�������
        /// </summary>
        [Display(Name = "�ƿ��ֻ�������")]
        [MaxLength(32)]
        [Column("MobileArea", TypeName = "varchar")]
        public string MobileArea { get; set; }

        /// <summary>
        /// ֤��ID
        /// </summary>
        [Display(Name = "֤��ID")]
        [Required(ErrorMessage = "֤�鲻��Ϊ��")]
        [Column("CertificateID", TypeName = "char"), MaxLength(32)]
        public string CertificateID { get; set; }


        /// <summary>
        /// ֤������
        /// </summary>
        [NotMapped]
        [Display(Name = "֤������")]
        public string CertificateName { get; set; }

        #endregion


        #region ѧ������
        /// <summary>
        /// ������ID
        /// </summary>
        [Display(Name = "������ID")]
        [Required(ErrorMessage = "������ID����Ϊ��")]
        [Column("EnteredPointID", TypeName = "char"), MaxLength(32)]
        public string EnteredPointID { get; set; }


        /// <summary>
        /// ����������
        /// </summary>
        [NotMapped]
        [Display(Name = "����������")]
        public string EnteredPointName { get; set; }

        /// <summary>
        /// �Ƽ���ID
        /// </summary>
        [Display(Name = "�Ƽ���ID")]
        [Required(ErrorMessage = "������ID����Ϊ��")]
        [Column("ReferenceID", TypeName = "char"), MaxLength(32)]
        public string ReferenceID { get; set; }

        /// <summary>
        /// �Ƽ�������
        /// </summary>
        [NotMapped]
        [Display(Name = "�Ƽ�������")]
        public string ReferenceName { get; set; }

        /// <summary>
        /// �����УID
        /// </summary>
        [Display(Name = "�����УID")]
        [Column("WantDriverShopID", TypeName = "char"), MaxLength(32)]
        public string WantDriverShopID { get; set; }

        /// <summary>
        /// �����У����
        /// </summary>
        [NotMapped]
        [Display(Name = "�����У����")]
        public string WantDriverShopName { get; set; }

        /// <summary>
        /// ��ѵ���ID
        /// </summary>
        [Display(Name = "��ѵ���ID")]
        [Required(ErrorMessage = "��ѵ���ID����Ϊ��")]
        [Column("TrianID", TypeName = "char"), MaxLength(32)]
        public string TrianID { get; set; }

        /// <summary>
        /// ��ѵ�������
        /// </summary>
        [NotMapped]
        [Display(Name = "���֤����")]
        public string TrianName { get; set; }


        /// <summary>
        /// ����
        /// </summary>
        [Display(Name = "����")]
        public decimal Money { get; set; }


        /// <summary>
        /// �ѽ�����
        /// </summary>
        [Display(Name = "�ѽ�����")]
        public decimal HadPayMoney { get; set; } = 0;

        /// <summary>
        /// �Ƿ�������
        /// </summary>
        [Display(Name = "�Ƿ�������")]
        public YesOrNoCode MoneyIsFull { get; set; } = YesOrNoCode.No;


        /// <summary>
        /// �ɷѷ�ʽID
        /// </summary>
        [Display(Name = "�ɷѷ�ʽID")]
        [Required(ErrorMessage = "�ɷѷ�ʽID����Ϊ��")]
        [Column("PayMethodID", TypeName = "char"), MaxLength(32)]
        public string PayMethodID { get; set; }

        /// <summary>
        /// �ɷѷ�ʽ����
        /// </summary>
        [NotMapped]
        [Display(Name = "�ɷѷ�ʽ����")]
        public string PayMethodName { get; set; }

        /// <summary>
        /// ��ע
        /// </summary>
        [Display(Name = "��ע")]
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// �����ص㣨ʡ�ݣ�
        /// </summary>
        [Display(Name = "�����ص㣨ʡ�ݣ�")]
        [MaxLength(6)]
        [Column("EnteredProvinceCode", TypeName = "varchar")]
        public string EnteredProvinceCode { get; set; }
        /// <summary>
        /// �����ص㣨ʡ�ݣ�
        /// </summary>
        [NotMapped]
        [Display(Name = "�����ص㣨ʡ�ݣ�")]
        public string EnteredProvinceName { get; set; }


        /// <summary>
        /// �����ص㣨�У�
        /// </summary>
        [Display(Name = "�����ص㣨�У�")]
        [MaxLength(6)]
        [Column("EnteredCityCode", TypeName = "varchar")]
        public string EnteredCityCode { get; set; }

        /// <summary>
        /// ����ʱ��
        /// </summary>
        [Display(Name = "����ʱ��")]
        public DateTime EnteredDate { get; set; }

        /// <summary>
        /// �����ص㣨�У�
        /// </summary>
        [NotMapped]
        [Display(Name = "�����ص㣨�У�")]
        public string EnteredCityName { get; set; }



        /// <summary>
        /// �ƿ���УID
        /// </summary>
        [Display(Name = "�ƿ���УID")]
        [Column("MakeDriverShopID", TypeName = "char"), MaxLength(32)]
        public string MakeDriverShopID { get; set; }

        /// <summary>
        /// �ƿ���У����
        /// </summary>
        [NotMapped]
        [Display(Name = "�ƿ���У����")]
        public string MakeDriverShopName { get; set; }

        /// <summary>
        /// �ƿ�����
        /// </summary>
        [Display(Name = "�ƿ�����")]
        public Nullable<DateTime> MakeCardDate { get; set; }

        /// <summary>
        /// �ƿ����б���
        /// </summary>
        [Display(Name = "�ƿ����б���")]
        [MaxLength(6)]
        [Column("MakeCardCityCode", TypeName = "varchar")]
        public string MakeCardCityCode { get; set; }

        /// <summary>
        /// �ƿ��ص�����
        /// </summary>
        [NotMapped]
        [Display(Name = "�ƿ��ص�����")]
        public string MakeCardCityName { get; set; }

        /// <summary>
        /// �ƿ���ע
        /// </summary>
        [Display(Name = "�ƿ���ע")]
        [MaxLength(128)]
        [Column("MakeCardRemark", TypeName = "varchar")]
        public string MakeCardRemark { get; set; }


        /// <summary>
        /// ��ϵ�ֻ���
        /// </summary>
        [Display(Name = "��ϵ�ֻ���")]
        [MaxLength(11)]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "�ֻ���ʽ����ȷ")]
        public string ConactMobile { get; set; }


        /// <summary>
        /// ��Уid
        /// </summary>
        [Display(Name = "��Уid")]
        [Column("SchoolID", TypeName = "char"), MaxLength(32)]
        public string SchoolID { get; set; }

        [NotMapped]
        public string SchoolName { get; set; }
        /// <summary>
        /// ѧԺid
        /// </summary>
        [Display(Name = "ѧԺid")]
        [Column("CollegeID", TypeName = "char"), MaxLength(32)]
        public string CollegeID { get; set; }
        [NotMapped]
        public string CollegeName { get; set; }

        /// <summary>
        /// רҵid
        /// </summary>
        [Display(Name = "רҵid")]
        [Column("MajorID", TypeName = "char"), MaxLength(32)]
        public string MajorID { get; set; }
        [NotMapped]
        public string MajorName { get; set; }
        /// <summary>
        /// �꼶
        /// </summary>
        [Display(Name = "�꼶")]
        [Column("SchoolAge", TypeName = "varchar"), MaxLength(32)]
        public string SchoolAge { get; set; }

        /// <summary>
        /// ��Դ
        /// </summary>
        public FromCode From { get; set; }

        #endregion

        #region ��Ŀ���



        /// <summary>
        /// ��Ŀһʱ��
        /// </summary>
        [Display(Name = "��Ŀһʱ��")]
        public Nullable<DateTime> ThemeOneDate { get; set; }

        /// <summary>
        /// ��Ŀһ�Ƿ�ͨ��
        /// </summary>
        [Display(Name = "��Ŀһ�Ƿ�ͨ��")]
        public YesOrNoCode ThemeOnePass { get; set; }


        /// <summary>
        /// ��Ŀ��ʱ��
        /// </summary>
        [Display(Name = "��Ŀ��ʱ��")]
        public Nullable<DateTime> ThemeTwoDate { get; set; }


        /// <summary>
        /// ��Ŀ���Ƿ�ͨ��
        /// </summary>
        [Display(Name = "��Ŀ���Ƿ�ͨ��")]
        public YesOrNoCode ThemeTwoPass { get; set; }

        /// <summary>
        /// ��Ŀ��ѧʱ״̬
        /// </summary>
        [Display(Name = "��Ŀ��ѧʱ״̬")]
        public ThemeTimeCode ThemeTwoTimeCode { get; set; }

        /// <summary>
        /// ��Ŀ������
        /// </summary>
        [Display(Name = "��Ŀ������")]
        [Column("ThemeTwoCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeTwoCoachID { get; set; }

        public YesOrNoCode ThemeTwoConfirm { get; set; }

        /// <summary>
        ///  ��Ŀ����������
        /// </summary>
        [NotMapped]
        [Display(Name = "��Ŀ����������")]
        public string ThemeTwoCoachName { get; set; }


        /// <summary>
        /// ��Ŀ��ʱ��
        /// </summary>
        [Display(Name = "��Ŀ��ʱ��")]
        public Nullable<DateTime> ThemeThreeDate { get; set; }

        /// <summary>
        /// ��Ŀ���Ƿ�ͨ��
        /// </summary>
        [Display(Name = "��Ŀ���Ƿ�ͨ��")]
        public YesOrNoCode ThemeThreePass { get; set; }

        public YesOrNoCode ThemeThreeConfirm { get; set; }
        /// <summary>
        /// ��Ŀ��ѧʱ״̬
        /// </summary>
        [Display(Name = "��Ŀ��ѧʱ״̬")]
        public ThemeTimeCode ThemeThreeTimeCode { get; set; }

        /// <summary>
        /// ��Ŀ������
        /// </summary>
        [Display(Name = "��Ŀ������")]
        [Column("ThemeThreeCoachID", TypeName = "char"), MaxLength(32)]
        public string ThemeThreeCoachID { get; set; }

        /// <summary>
        ///  ��Ŀ����������
        /// </summary>
        [Display(Name = "��Ŀ����������")]
        [NotMapped]
        public string ThemeThreeCoachName { get; set; }


        /// <summary>
        /// ��Ŀ��ʱ��
        /// </summary>
        [Display(Name = "��Ŀ��ʱ��")]
        public Nullable<DateTime> ThemeFourDate { get; set; }

        /// <summary>
        /// ��Ŀ���Ƿ�ͨ��
        /// </summary>
        [Display(Name = "��Ŀ���Ƿ�ͨ��")]
        public YesOrNoCode ThemeFourPass { get; set; }


        #endregion

        /// <summary>
        /// ѧԱ״̬
        /// </summary>
        [Display(Name = "ѧԱ״̬")]
        public StudentCode State { get; set; }

        /// <summary>
        /// ��ѧʱ��
        /// </summary>
        [Display(Name = "��ѧʱ��")]

        public Nullable<DateTime> DropOutDate { get; set; }

        /// <summary>
        /// �˿��¼id
        /// </summary>
        [Display(Name = "�˿��¼id")]
        [Column("DropOutPayOrderId", TypeName = "char"), MaxLength(32)]
        public string DropOutPayOrderId { get; set; }


        /// <summary>
        /// ��ǰ��Ŀ
        /// </summary>
        [Display(Name = "��ǰ��Ŀ")]
        public ThemeCode NowTheme { get; set; }

        /// <summary>
        /// ���Դ���
        /// </summary>
        [NotMapped]
        public int ExamCount { get; set; }

        /// <summary>
        /// ���Դ���
        /// </summary>
        [NotMapped]
        public int OtherExamCount { get; set; }

        /// <summary>
        /// �ɷѼ�¼
        /// </summary>
        [Display(Name = "�ɷѼ�¼")]
        [NotMapped]
        public List<PayOrder> PayOrderList { get; set; }

        [NotMapped]
        public decimal DoConfirmMoney { get; set; }


        [Required]
        [Display(Name = "������id")]
        [Column("CreaterID", TypeName = "char"), MaxLength(32)]
        public string CreaterID { get; set; }

        /// <summary>
        /// ������
        /// </summary>
        [NotMapped]
        public string CreaterName { get; set; }


    }
}
