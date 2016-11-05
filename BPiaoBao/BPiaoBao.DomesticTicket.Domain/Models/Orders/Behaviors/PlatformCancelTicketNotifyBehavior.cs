using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 接口平台取消出票 通知
    /// </summary>
    [Behavior("PlatformCancelTicketNotify")]
    public class PlatformCancelTicketNotifyBehavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            string remark = getParame("remark").ToString();
            string operatorName = getParame("operatorName").ToString();
            order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
            order.WriteLog(new OrderLog()
            {
                OperationContent = "平台取消出票，订单号" + order.OrderId + ",备注:" + remark,
                OperationDatetime = System.DateTime.Now,
                OperationPerson = string.IsNullOrEmpty(operatorName) ? "系统" : operatorName
                ,
                IsShowLog = false
            });
            return null;
        }
    }
}
