using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{
    [OrderState(EnumOrderStatus.PayWaitCreatePlatformOrder)]
    public class PayWaitCreatePlatformOrderState : BaseOrderState
    {
        public override Type[] GetBahaviorTypes()
        {
            return new Type[] { 
                typeof(WaitReimburseWithRepelIssueBahavior),
                typeof(CreatePlatformOrderBehavior)
            };
        }
    }
}
