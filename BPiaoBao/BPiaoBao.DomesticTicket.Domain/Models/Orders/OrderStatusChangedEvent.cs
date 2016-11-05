using BPiaoBao.Common.Enums;
using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class OrderStatusChangedEvent : IDomainEvent
    {
        public Order Order { get; set; }
        public EnumOrderStatus NewStatus { get; set; }
        public EnumOrderStatus OldStatus { get; set; }
    }
}
