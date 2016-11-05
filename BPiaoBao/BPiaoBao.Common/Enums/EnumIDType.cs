using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 证件类型枚举
    /// </summary>
    public enum EnumIDType
    { 
        [Description("身份证")]
        NormalId = 0,
        [Description("护照")]
        Passport = 1,
        [Description("军官证")]
        MilitaryId = 2,
        [Description("出生日期")]
        BirthDate = 3,
        [Description("其它有效证件")]
        Other = 4

    }
}
