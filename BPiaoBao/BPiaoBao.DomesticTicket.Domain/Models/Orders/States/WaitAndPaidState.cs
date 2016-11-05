using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{
    [OrderState(EnumOrderStatus.WaitAndPaid)]
    public class WaitAndPaidState : BaseOrderState
    {
        public override Type[] GetBahaviorTypes()
        {
            return new Type[] { 
                typeof(NewSelectPolicyBehavior),
                typeof(CancelIssueTicketBehavior), 
                typeof(PlatformCancelTicketNotifyBehavior),
                typeof(PaidOrderBehavior), 
                typeof(QueryPaidStatusBebavior), 
                typeof(TicketsIssueBehavior),
                typeof(WaitReimburseWithRepelIssueBahavior)
            };
        }
    }
}
