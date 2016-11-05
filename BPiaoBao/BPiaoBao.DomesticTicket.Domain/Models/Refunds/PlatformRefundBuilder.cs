using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Refunds
{
    public class PlatformRefundBuilder : IAggregationBuilder
    {
        public PlatformRefundOrder CreateRefundOrder(EnumPlatformRefundType refundType, string refundOrderId, string remark)
        {
            var refundOrder = new PlatformRefundOrder()
            {
                RefundLogs = new List<OrderLog>(),
                RefundType = refundType,
                RefundStatus = EnumPlatformRefundStatus.UnRefund,
                RefundOrderId = refundOrderId,
                Remark = remark
            };

            return refundOrder;

        }
    }
}
