using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumInsurancePayStatus
    {
        /// <summary>
        /// 未支付
        /// </summary>
        [Description("未支付")]
        NoPay = 0,
        /// <summary>
        /// 已支付
        /// </summary>
        [Description("已支付")]
        OK = 1,
        /// <summary>
        /// 赠送
        /// </summary>
        [Description("赠送")]
        Offer=2
    }
}
