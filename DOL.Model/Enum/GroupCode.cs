using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 字典分组
    /// </summary>
    public enum GroupCode
    {
        None=0,
        /// <summary>
        /// 地区
        /// </summary>
        [Description("地区")]
        Area = 1,

        [Description("支付方式")]
        PayMethod = 2,

        [Description("支付渠道")]
        PayType = 3,

        [Description("培训方式")]
        Train = 4,

        [Description("证书类型")]
        Certificate = 5,

        [Description("收款账户")]
        Account = 6,
    }
}
