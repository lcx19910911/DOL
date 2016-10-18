
using DOL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Core
{
    /// <summary>
    /// 登录用户
    /// </summary>
    public class LoginUser
    {
        public LoginUser(User user)
        {
            this.ID = user.ID;
            this.Account = user.Account;
            this.Name = user.Name;
            this.IsAdmin = user.IsAdmin;
            this.MenuFlag = user.MenuFlag;
        }


        public LoginUser()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }

        /// <summary>
        /// 公司名称
        /// </summary>
        public string Name { get; set; }
        

        /// <summary>
        /// 权限值
        /// </summary>
        public Nullable<long> MenuFlag { get; set; }
        public YesOrNoCode IsAdmin { get; set; }
    }
}
