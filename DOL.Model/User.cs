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
        /// 部门ID
        /// </summary>
        [Required(ErrorMessage = "部门ID不能为空")]
        [Column("DepartmentID", TypeName = "char"), MaxLength(32)]
        public string DepartmentID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [NotMapped]
        public string DepartmentName { get; set; }
        /// <summary>
        /// 角色
        /// </summary>
        [Required(ErrorMessage = "角色不能为空")]
        [Column("RoleID", TypeName = "char"), MaxLength(32)]
        public string RoleID { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [NotMapped]
        public string RoleName { get; set; }

        /// <summary>
        /// 是否管理员
        /// </summary>
        public bool IsAdmin { get; set; }

        /// <summary>
        /// 操作权限
        /// </summary>
        public long? OperateFlag { get; set; } 


        /// <summary>
        /// 创建者Id
        /// </summary>
        [Display(Name = "创建者Id")]
        [Column("CreaterId", TypeName = "char"), MaxLength(32)]
        public string CreaterId { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        [Display(Name = "账号")]
        [MaxLength(12)]
        [Required(ErrorMessage ="登陆账号不能为空")]
        [Column("Account", TypeName = "varchar")]
        public string Account { get; set; }
        /// <summary>
        /// 名称
        /// </summary>
        [Display(Name = "名称")]
        [MaxLength(32)]
        [Required(ErrorMessage = "名称不能为空")]
        [Column("Name", TypeName = "varchar")]
        public string Name { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        [Display(Name = "密码")]
        [MaxLength(128)]
        [Required(ErrorMessage = "密码不能为空")]
        [Column("Password", TypeName = "varchar")]
        public string Password { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        [Display(Name = "手机号")]
        [MaxLength(11)]
        [Required(ErrorMessage = "手机号不能为空")]
        [RegularExpression(@"((\d{11})$)", ErrorMessage = "手机格式不正确")]
        public string Mobile { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        [Display(Name = "备注")]
        [MaxLength(128)]
        [Column("Remark", TypeName = "varchar")]
        public string Remark { get; set; }

        /// <summary>
        /// 请输入密码
        /// </summary>
        [Display(Name = "请输入密码")]
        [MaxLength(12),MinLength(6)]
        [NotMapped]
        public string NewPassword { get; set; }

        /// <summary>
        /// 再次输入密码
        /// </summary>
        [Display(Name = "再次输入密码")]
        [MaxLength(12), MinLength(6),Compare("NewPassword",ErrorMessage="两次密码输入不一致")]
        [NotMapped]
        public string ConfirmPassword { get; set; }
       

        /// <summary>
        /// 状态
        /// </summary>
        [NotMapped]
        public string State { get; set; }

        /// <summary>
        /// 教练员id
        /// </summary>
        [Column("CoachID", TypeName = "char"), MaxLength(32)]
        public string CoachID { get; set; }

        /// <summary>
        /// 权限集合
        /// </summary>
        [NotMapped]
        public List<string> OperateList { get; set; }

        /// <summary>
        /// 报名点集合
        /// </summary>
        [Column("EnteredPointIDStr", TypeName = "text")]
        public string EnteredPointIDStr { get; set; }

        /// <summary>
        /// 页面权限
        /// </summary>
        [Column("MenuIDStr", TypeName = "text")]
        public string MenuIDStr { get; set; }

        /// <summary>
        /// 是否是门店管理员
        /// </summary>
        public YesOrNoCode IsStoreAdmin { get; set; } = YesOrNoCode.No;



        /// <summary>
        /// 是否展示金额
        /// </summary>
        [NotMapped]
        public bool IsNotShowMoney { get; set; }

        public DateTime QuitTime { get; set; }
    }
}
