using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumOPEN
    {
        [Description("未扫描")]
        NoScan = 0,
        [Description("扫描中")]
        Scanning = 1,
        [Description("已扫描")]
        Scaned = 2
    }
}
