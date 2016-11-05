using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public partial class PlatformRefundOrderService : IStationPlatformRefundOrderService
    {
        [ExtOperationInterceptor("获取平台退款订单列表")]
        public DataPack<ResponsePlatformRefundOrder> GetList(EnumPlatformRefundType? refundType, string orderId, int startIndex, int count)
        {

            var query = platformRefundOrderRepository.FindAll();
            if (refundType.HasValue)
                query = query.Where(p => p.RefundType == refundType);
            if (!string.IsNullOrEmpty(orderId))
                query = query.Where(p => p.RefundOrderId == orderId);



            return new DataPack<ResponsePlatformRefundOrder>()
            {
                TotalCount = query.Count(),
                List = query
                .OrderByDescending(p => p.Id)
                .Skip(startIndex)
                .Take(count)
                .ToList()
                .Select(p => p.ToResponseRefundOrder())
                .ToList()
            };
        }

        [ExtOperationInterceptor("确认退款")]
        public void ConfirmRefund(int Id, decimal amount, DateTime refundTime)
        {
            var order = platformRefundOrderRepository.FindAll(p => p.Id == Id).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("找不到退款单号为" + Id + "的退单");
            order.ConfirmRefund(amount, refundTime, AuthManager.GetCurrentUser().OperatorName);
        }
    }
}
