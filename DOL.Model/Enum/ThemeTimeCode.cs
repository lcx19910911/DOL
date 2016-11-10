using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 学时分组
    /// </summary>
    public enum ThemeTimeCode
    {
        [Description("未知")]
        None =0,
        /// <summary>
        /// 状态
        /// </summary>
        [Description("状态")]
        Complete = 1,

        [Description("未完成")]
        Lock = 2,      
    }
}
