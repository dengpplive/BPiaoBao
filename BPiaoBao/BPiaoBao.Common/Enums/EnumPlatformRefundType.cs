using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 退款类型
    /// </summary>
    public enum EnumPlatformRefundType
    {
        /// <summary>
        /// 平台拒绝出票退款
        /// </summary>
        [Description("平台拒绝出票退款")]
        RepelIssue=0,
        /// <summary>
        /// 平台退票退款
        /// </summary>
        [Description("平台退票退款")]
        BounceTicket = 1,
        /// <summary>
        /// 平台废票退款
        /// </summary>
        [Description("平台废票退款")]
        AnnulTicket=2
    }

    public enum EnumPlatformRefundStatus
    { 
        /// <summary>
        /// 未退款
        /// </summary>
        [Description("未退款")]
        UnRefund= 0,
        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退款")]
        Refunded = 1,
    }
}
