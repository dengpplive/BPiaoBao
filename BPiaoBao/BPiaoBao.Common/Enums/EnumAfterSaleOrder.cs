using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumAfterSaleOrder:int
    {
        [Description("废票")]
        Annul,
        [Description("退票")]
        Bounce,
        [Description("改签")]
        Change,
        [Description("其它")]
        Modify
    }
}
