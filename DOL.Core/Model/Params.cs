using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Web.Security;

namespace DOL.Core
{
    public class Params
    {
        /// <summary>
        /// 缓存前缀
        /// </summary>
        public static readonly string Cache_Prefix_Key = "WOL";

        public static readonly string Mobile = ConfigurationManager.AppSettings["Mobile"];

        /// <summary>
        /// 公司名称
        /// </summary>
        public static readonly string CompanyName = ConfigurationManager.AppSettings["CompanyName"];               

        /// <summary>
        /// 密钥
        /// </summary>
        public static readonly string SecretKey = ConfigurationManager.AppSettings["SecretKey"];

        /// <summary>
        /// 教练员默认权限
        /// </summary>
        public static readonly int CoachMenuFlag = ConfigurationManager.AppSettings["CoachMenuFlag"].GetInt();

        /// <summary>
        /// 登陆cookie
        /// </summary>
        public static readonly string UserCookieName = "dol_user";

        /// <summary>
        /// cookie 过期时间
        /// </summary>
        public static readonly int CookieExpires = 12000;
    }
}
