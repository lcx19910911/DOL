using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 目录权限
    /// </summary>
    public enum MenuFlag
    {
        /// <summary>
        /// 在线订单
        /// </summary>
        [Description("Order")]
        Order =1,

        /// <summary>
        /// 主题管理
        /// </summary>
        [Description("Theme")]
        Theme = 2,

        /// <summary>
        /// 密室管理
        /// </summary>
        [Description("Store")]
        Store = 4,

        /// <summary>
        /// 统计报表
        /// </summary>
        [Description("Report")]
        Report = 8,

        /// <summary>
        /// 支付方式
        /// </summary>
        [Description("Pay")]
        Pay = 16,

        /// <summary>
        /// 用户管理
        /// </summary>
        [Description("User")]
        User = 32,

        /// <summary>
        /// 公司管理
        /// </summary>
        [Description("Company")]
        Company = 64,

        /// <summary>
        /// 饮料管理
        /// </summary>
        [Description("Drink")]
        Drink = 128,
    }
}
