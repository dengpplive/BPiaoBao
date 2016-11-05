using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 投保类型
    /// </summary>
    public enum EnumInsureMethod
    {
        [Description("自动投保")]
        Auto = 0,
        [Description("手动投保")]
        Manually = 1
    }
}
