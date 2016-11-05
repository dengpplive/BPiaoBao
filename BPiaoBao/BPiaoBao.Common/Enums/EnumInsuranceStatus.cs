using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 保单状态
    /// </summary>
    public enum EnumInsuranceStatus
    {
        [Description("手动出单")]
        Manual = 5,

        [Description("未出单")]
        NoInsurance = 6,

        [Description("已出单")]
        GotInsurance = 7,

        [Description("已撤销")]
        Canceled = 8,


    }
}
