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
        /// <summary>
        /// 报名
        /// </summary>
        [Description("未制卡")]
        DontMakeCard = 0,

        [Description("科目一")]
        ThemeOne = 1,

        [Description("科目二")]
        ThemeTwo = 2,


        [Description("科目三")]
        ThemeThree = 3,


        [Description("科目四")]
        ThemeFour = 4,

        [Description("毕业")]
        Graduate = 5,


        [Description("申请退学")]
        WantDropOut = 6,

        [Description("已退学")]
        HadDropOut = 7,


        [Description("科目三通过科目二没过")]
        ThemeThreePass = 11,

        [Description("科目二为该教练，科目一、二通过科目三未过的")]
        ThemeThreeNoPass = 12,
    }
}
