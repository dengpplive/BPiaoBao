using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 手动取消出票
    /// </summary>
    [Behavior("CancelIssueTicket")]
    public class CancelIssueTicketBehavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            string remark = getParame("remark").ToString();
            string operatorName = getParame("operatorName").ToString();
            //修改状态 改成拒绝出票等待退款 
            order.ChangeStatus(EnumOrderStatus.WaitReimburseWithRepelIssue);
            order.WriteLog(new OrderLog()
            {
                OperationContent = "取消出票，订单号" + order.OrderId + ",备注:" + remark,
                OperationDatetime = System.DateTime.Now,
                OperationPerson = operatorName
                ,
                IsShowLog = true
            });
            return null;
        }
    }
}
