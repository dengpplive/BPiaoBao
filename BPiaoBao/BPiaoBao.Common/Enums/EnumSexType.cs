using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 性别类型枚举
    /// </summary>
    public enum EnumSexType
    {

        [Description("未知")]
        UnKnown = 0,

        [Description("男")]
        Male = 1,

        [Description("女")]
        Female = 2
    }
}
