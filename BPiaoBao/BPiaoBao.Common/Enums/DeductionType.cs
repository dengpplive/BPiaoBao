using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum DeductionSource
    {
        [Description("运营扣点")]
        CarrierDeduction = 0,
        [Description("平台扣点")]
        PlatformDeduction = 1
    }
}
