using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    [Behavior("AfterSaleOrder")]
    public class AfterSaleApplyBehavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            var afterSaleOrder = getParame<AfterSaleOrder>("AfterSaleOrder");
            string operatorName = getParame("operatorName").ToString();
            afterSaleOrder.WriteLog(new OrderLog
            {
                OperationContent = string.Format("申请{0}成功,等待处理", afterSaleOrder.AfterSaleType),
                OperationDatetime = DateTime.Now,
                IsShowLog = true,
                OperationPerson = operatorName
            });
            order.WriteLog(new OrderLog
            {
                OperationContent = string.Format("申请{0}", afterSaleOrder.AfterSaleType),
                OperationDatetime = DateTime.Now,
                IsShowLog = true,
                OperationPerson = operatorName
            });
            order.HasAfterSale = true;
            return true;
        }
    }
}
