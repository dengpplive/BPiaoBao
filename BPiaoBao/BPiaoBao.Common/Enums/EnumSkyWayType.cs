using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumSkyWayType :int
    {
        [Description("单程")]
        OneWay = 0,
        [Description("往返")]
        Return = 1,
        [Description("中转联程")]
        Connecting=2
    }
}
