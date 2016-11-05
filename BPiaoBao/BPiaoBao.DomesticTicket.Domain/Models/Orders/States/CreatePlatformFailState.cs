using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{
    [OrderState(EnumOrderStatus.CreatePlatformFail)]
    public class CreatePlatformFailState : BaseOrderState
    {
        public override Type[] GetBahaviorTypes()
        {
            return new Type[] { 
                typeof(CreatePlatformOrderBehavior),
                typeof(CancelOrderBehavior),
                typeof(WaitReimburseWithRepelIssueBahavior),
                typeof(NewSelectPolicyBehavior)
            };
        }
    }
}
