using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{
    /// <summary>
    /// 出票
    /// </summary>
    [OrderState(EnumOrderStatus.WaitIssue)]
    public class WaitIssueState : BaseOrderState
    {
        public override Type[] GetBahaviorTypes()
        {
            return new Type[] { 
            typeof(TicketsIssueBehavior),
            typeof(PlatformCancelTicketNotifyBehavior),
            typeof(QueryPaidStatusBebavior),
            typeof(PlatformRefundNotifyBehavior),
            typeof(WaitReimburseWithRepelIssueBahavior)
            };
        }
    }
}
