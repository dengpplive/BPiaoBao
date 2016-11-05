using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Refunds
{
    /// <summary>
    /// 退款事件定义
    /// </summary>
    public class RefundEvent : IDomainEvent
    {
        public PlatformRefundOrder RefundOrder { get; set; }
    }
}
