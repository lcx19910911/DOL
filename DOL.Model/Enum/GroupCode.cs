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
        Region = 1,

        [Description("行业")]
        InvestField = 2,

        [Description("投资理念")]
        InvestmentConcept = 3,

        [Description("提供帮助")]
        ProvideHelp = 4,

        [Description("分享文字")]
        ShareText =5,
    }
}
