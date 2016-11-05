using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Refunds
{
    /// <summary>
    /// 退款领域事件
    /// </summary>
    public class RefundHandler : IDomainEventHandler<RefundEvent>
    {
        public void Handle(RefundEvent domainEvent)
        {
            //修改各类订单的退款状态
        }
    }
}
