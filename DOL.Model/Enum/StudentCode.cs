using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 学员状态
    /// </summary>
    public enum StudentCode
    {
        None=0,
        /// <summary>
        /// 报名
        /// </summary>
        [Description("报名")]
        Entered = 1,

        [Description("培训中")]
        Training = 2,

        [Description("毕业")]
        Graduated = 3,

        [Description("退学")]
        DropOut = 4,
    }
}
