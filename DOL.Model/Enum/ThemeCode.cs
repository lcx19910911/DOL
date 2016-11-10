using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 科目
    /// </summary>
    public enum ThemeCode
    {
        [Description("未知")]
        None =0,
        /// <summary>
        /// 科目一
        /// </summary>
        [Description("科目一")]
        One = 1,

        [Description("科目二")]
        Two = 2,

        [Description("科目三")]
        Three = 3,

        [Description("科目四")]
        Four = 4,
    }
}
