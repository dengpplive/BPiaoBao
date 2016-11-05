using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 接口平台退款通知 的行为
    /// </summary>
    [Behavior("PlatformRefundNotify")]
    public class PlatformRefundNotifyBehavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            decimal refundMoney = decimal.Parse(getParame("refundMoney").ToString());
            string remark = getParame("remark").ToString();
            string operatorName = getParame("operatorName").ToString();
            string platformCode = getParame("platformCode").ToString();
            //拒绝出票等待退款
            order.ChangeStatus(EnumOrderStatus.WaitReimburseWithRepelIssue);
            order.WriteLog(new OrderLog()
            {
                OperationContent = string.Format("通知:{0}退款,订单号:{1},退款金额:{2},备注:{3}", platformCode, order.OrderId, refundMoney, remark),
                OperationDatetime = System.DateTime.Now,
                OperationPerson = string.IsNullOrEmpty(operatorName) ? "系统" : operatorName
                ,
                IsShowLog = false
            });

            return null;
        }
    }
}
