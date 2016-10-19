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
        public string DepartmentID { get; set; }

        /// <summary>
        /// 角色位域值
        /// </summary>
        public string RoleID { get; set; }

        /// <summary>
        /// 菜单权限 
        /// </summary>
        public long? MenuFlag { get; set; }
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
        [MaxLength(100)]
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
        /// 电话
        /// </summary>
        [Display(Name = "电话")]
        [MaxLength(11)]
        [RegularExpression(@"((\d{3,4}-\d{3,7})$)", ErrorMessage = "电话格式不正确")]
        public string Telephone { get; set; }
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
        /// 创建时间
        /// </summary>
        [Display(Name = "账号到期时间")]       
        public Nullable<System.DateTime> ExpireTime { get; set; }

        /// <summary>
        /// 是否是管理员
        /// </summary>
        public YesOrNoCode IsAdmin { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [NotMapped]
        public string State { get; set; }
    }
}
