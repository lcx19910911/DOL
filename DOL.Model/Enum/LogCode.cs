using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DOL.Model
{
    /// <summary>
    /// 日志状态
    /// </summary>
    public enum LogCode
    {
        None=0,
        /// <summary>
        /// 修改学员信息
        /// </summary>
        [Description("编辑学员信息")]
        UpdateStudent = 1,

        /// <summary>
        /// 删除学员信息
        /// </summary>
        [Description("删除学员信息")]
        DeleteStudent = 2,

        [Description("确认缴费记录")]
        ConfirmPayOrder = 3,


        [Description("修改缴费记录")]
        UpdatePayOrder = 4,

        [Description("删除缴费记录")]
        DeletePayOrder = 5,


        [Description("删除考试记录")]
        DeleteExam = 6,
    }
}
