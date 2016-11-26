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
    public enum FromCode
    {
        None=0,
        /// <summary>
        /// 学校
        /// </summary>
        [Description("学校")]
        School = 1,

        [Description("社会")]
        Society = 2,
        

    }
}
