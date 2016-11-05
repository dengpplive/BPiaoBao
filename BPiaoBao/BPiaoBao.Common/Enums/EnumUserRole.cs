using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 用户角色
    /// </summary>
    public enum EnumUserRole
    {
        [Description("采购商")]
        Buyer = 0,
        [Description("运营商")]
        Carrier = 1,
        [Description("供应商")]
        Supplier = 2,
        [Description("系统")]
        Platform = 3
    }
}
