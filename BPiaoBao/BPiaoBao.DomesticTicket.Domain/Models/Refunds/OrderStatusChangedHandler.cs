using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.DDD.Events;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Refunds
{
  public   class OrderStatusChangedHandler : IDomainEventHandler<OrderStatusChangedEvent>
    {
      IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        public void Handle(OrderStatusChangedEvent domainEvent)
        {
            if (domainEvent.OldStatus == Common.Enums.EnumOrderStatus.WaitIssue && domainEvent.NewStatus == Common.Enums.EnumOrderStatus.WaitReimburseWithPlatformRepelIssue)
            {
                //生成退款单
                var bulider = AggregationFactory.CreateBuiler<PlatformRefundBuilder>();
                var order=bulider.CreateRefundOrder(Common.Enums.EnumPlatformRefundType.RepelIssue, domainEvent.Order.OrderId, "平台拒绝出票，退款");
                unitOfWorkRepository.PersistCreationOf(order);
            }
        }
    }
}
