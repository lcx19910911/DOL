using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 考试状态
    /// </summary>
    public enum ExamCode
    {
        None=0,
        /// <summary>
        /// 挂科
        /// </summary>
        [Description("挂科")]
        Fail = 1,

        [Description("通过")]
        Pass = 2,

        [Description("缺考")]
        Miss = 3,

    }
}
