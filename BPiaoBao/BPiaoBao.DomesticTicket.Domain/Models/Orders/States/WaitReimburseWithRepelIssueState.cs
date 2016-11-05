using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{
    /// <summary>
    /// 退款
    /// </summary>
    [OrderState(EnumOrderStatus.WaitReimburseWithRepelIssue)]
    public class WaitReimburseWithRepelIssueState : BaseOrderState
    {
        public override Type[] GetBahaviorTypes()
        {
            return new Type[] { 
                typeof(WaitReimburseWithRepelIssueBahavior),
                typeof(NewSelectPolicyBehavior),
                typeof(CancelIssueTicketBehavior),
                typeof(TicketsIssueBehavior)
            };
        }
    }
}
