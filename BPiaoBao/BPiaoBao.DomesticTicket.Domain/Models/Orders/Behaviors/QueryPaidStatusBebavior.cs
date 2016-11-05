using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 查询代付状态
    /// </summary>
    [Behavior("QueryPaidStatus")]
    public class QueryPaidStatusBebavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            string operatorName = getParame("operatorName").ToString();
            var plateform = ObjectFactory.GetNamedInstance<IPlatform>(order.Policy.PlatformCode);
            bool IsPaid = plateform.IsPaid(order.Policy.AreaCity, order.OrderId, order.OutOrderId, order.PnrCode);
            string result = string.Empty;
            if (IsPaid)
            {
                if (order.OrderStatus == EnumOrderStatus.WaitAndPaid)
                {
                    order.OrderStatus = EnumOrderStatus.WaitIssue;
                }
                order.OrderPay.PaidStatus = EnumPaidStatus.OK;
                result = "已代付";
            }
            else
            {
                result = "未代付";
            }
            order.WriteLog(new OrderLog()
            {
                OperationContent = "查询代付状态:" + result + "，订单号" + order.OrderId,
                OperationDatetime = System.DateTime.Now,
                OperationPerson = operatorName
                ,
                IsShowLog = false
            });
            return result;
        }
    }
}
