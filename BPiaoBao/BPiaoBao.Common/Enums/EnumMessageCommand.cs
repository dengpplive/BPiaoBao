using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// web消息推送指令
    /// </summary>
    public enum EnumMessageCommand:int
    {
        /// <summary>
        /// 支付等待出票
        /// </summary>
        [Description("订单出票")]
        PayWaitIssueTicket=0,
        /// <summary>
        /// 申请售后
        /// </summary>
        [Description("售后订单处理")]
        ApplyAfterSaleOrder=1,
        /// <summary>
        /// 公告通知
        /// </summary>
        [Description("票宝公告")]
        Aannouncement=2
        
    }
}
