using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{
    /// <summary>
    /// 拒绝出票，等待平台退款 
    /// </summary>
    [OrderState(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue)]
    public class WaitReimburseWithPlatformRepelIssueState : BaseOrderState
    {
        public override Type[] GetBahaviorTypes()
        {
            return new Type[] { 
                typeof(PlatformRefundNotifyBehavior),
                typeof(TicketsIssueBehavior),
                typeof(CancelIssueTicketBehavior)
            };
        }
    }
}
