using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 耗损类别
    /// </summary>
    public enum WasteCode
    {
        None=0,
        /// <summary>
        /// 挂科
        /// </summary>
        [Description("油耗")]
        Oil = 1,

        [Description("维修")]
        Repair = 2,
        

    }
}
