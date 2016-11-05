using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.UIExt.Model.Enumeration
{
    /// <summary>
    /// 账单状态
    /// </summary>
    public enum BillStatus
    {
        [Description("所有")]
        All,
        [Description("已清")]
        Paid,
        [Description("未清")]
        Unpaid
    }
}
