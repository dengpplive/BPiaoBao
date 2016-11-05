using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumPolicyOwnUserRole
    {
        [Description("运营商")]
        Carrier = 0,
        [Description("供应商")]
        Supplier = 1
    }
}
