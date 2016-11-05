using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumInsuranceConfigType : int
    {
        [Description("分销商")]
        Buyer = 0,
        [Description("运营商")]
        Carrier = 1
    }
}
