using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.States
{
    /// <summary>
    /// 订单完成
    /// </summary>
    [OrderState(EnumOrderStatus.IssueAndCompleted)]
    public class IssueAndCompletedState : BaseOrderState
    {

        public override Type[] GetBahaviorTypes()
        {
            return new Type[] { 
                typeof(AfterSaleApplyBehavior)
            };
        }
    }
}
