using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 支付状态
    /// </summary>
    public enum EnumPayStatus
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
        OK = 1
    }
    /// <summary>
    /// 改签支付状态 
    /// </summary>
    public enum EnumChangePayStatus
    { 
        /// <summary>
        /// 未支付
        /// </summary>
        [Description("未支付")]
        NoPay=0,
        /// <summary>
        /// 已支付，支付完成
        /// </summary>
        [Description("已支付")]
        Payed=1,
        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        Refunding=2,
        /// <summary>
        /// 退款完成
        /// </summary>
        [Description("已退款")]
        Refunded=3
    }
}
