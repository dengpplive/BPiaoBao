using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{
    [OrderState(EnumOrderStatus.AutoIssueFail)]
    public class AutoIssueFailState : BaseOrderState
    {
        public override Type[] GetBahaviorTypes()
        {
            return new Type[] { 
            typeof(TicketsIssueBehavior)            
            };
        }
    }
}
