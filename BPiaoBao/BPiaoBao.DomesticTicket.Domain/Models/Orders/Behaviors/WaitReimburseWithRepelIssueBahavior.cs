using BPiaoBao.Common;
using JoveZhao.Framework.Expand;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Services;
using JoveZhao.Framework;
using BPiaoBao.DomesticTicket.Domain.Services.Insurance;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 手动退款
    /// </summary>
    [Behavior("WaitReimburseWithRepelIssue")]
    public class WaitReimburseWithRepelIssueBahavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            decimal refundMoney = decimal.Parse(getParame("refundMoney").ToString());
            string remark = getParame("remark").ToString();
            string operatorName = getParame("operatorName").ToString();
            string platformCode = getParame("platformCode").ToString();

            if (refundMoney > order.OrderPay.PayMoney || refundMoney < 0)
            {
                throw new OrderCommException("订单（" + order.OrderId + "）退款金额（￥" + refundMoney + "）超出范围！");
            }
            if (order.OrderStatus == EnumOrderStatus.RepelIssueRefunding)
            {
                throw new OrderCommException("订单（" + order.OrderId + "）正在退款中。。。");
            }
            string cashbagCode = getParame("cashbagCode").ToString();
            string cashbagKey = getParame("cashbagKey").ToString();
            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            string payType = order.OrderPay.PayMethod.ToEnumDesc();
            string serialNum = order.OrderPay.PaySerialNumber;
            string BuyDesc = (order.CreateTime < DateTime.Parse("2014/07/19 04:10:00")) ? OldGetRefundDetail(order, refundMoney) : GetRefundDetail(order, refundMoney);
            Logger.WriteLog(LogType.INFO, "退款分润 时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " OrderId=" + order.OrderId + " ProfitDetail=" + BuyDesc + "\r\n");
            if (payType == EnumPayMethod.Bank.ToEnumDesc() || payType == EnumPayMethod.Platform.ToEnumDesc())
            {
                client.Reimburse(cashbagCode, cashbagKey, serialNum, refundMoney, order.OrderId, "REFUND", BuyDesc);
                //修改为拒绝出票,退款中
                order.ChangeStatus(EnumOrderStatus.RepelIssueRefunding);
                order.WriteLog(new OrderLog()
                {
                    OperationContent = string.Format("退款状态：退款中,订单号:{0},退款金额:{1},退款方式:{2},备注:{3}", order.OrderId, refundMoney, payType, remark),
                    OperationDatetime = System.DateTime.Now,
                    OperationPerson = string.IsNullOrEmpty(operatorName) ? "系统" : operatorName
                    ,
                    IsShowLog = true
                });
            }
            else
            {
                client.Reimburse(cashbagCode, cashbagKey, serialNum, refundMoney, order.OrderId, "退款", BuyDesc, string.Format("PNR:{0},乘机人:【{1}】", this.order.PnrCode, string.Join("|", this.order.Passengers.Select(x => x.PassengerName).ToArray())));
                //修改为拒绝出票，订单完成
                order.ChangeStatus(EnumOrderStatus.RepelIssueAndCompleted);
                order.WriteLog(new OrderLog()
                {
                    OperationContent = string.Format("拒绝出票【退款状态：退款完成,订单号:{0},退款金额:{1},退款方式:{2},备注:{3}】", order.OrderId, refundMoney, payType, remark),
                    OperationDatetime = System.DateTime.Now,
                    OperationPerson = string.IsNullOrEmpty(operatorName) ? "系统" : operatorName
                    ,
                    IsShowLog = true
                });
            }

            //修改保险状态
            var insuranceFac = StructureMap.ObjectFactory.GetInstance<InsuranceDomainService>();
            insuranceFac.ReturnInsurance(this.order);
            return null;
        }

        public string GetRefundDetail(Order order, decimal refundMoney)
        {
            StringBuilder args = new StringBuilder();
            if (order.OrderPay.PayBillDetails != null && order.OrderPay.PayBillDetails.Count > 0)
            {
                //支付明细
                var payBillList = order.OrderPay.PayBillDetails.Where(p => p.OpType != EnumOperationType.PayMoney && p.OpType != EnumOperationType.Insurance && p.OpType != EnumOperationType.InsuranceServer).Select(p => new { OpType = p.OpType, Money = p.Money, CashBagCode = p.CashbagCode }).ToList();
                payBillList.ForEach(p =>
                {
                    args.AppendFormat("{0}^{1}^{2}|", p.CashBagCode, p.Money, p.OpType.ToEnumDesc());
                });
            }
            return args.ToString().TrimEnd('|');
        }
        public string OldGetRefundDetail(Order order, decimal refundMoney)
        {
            List<string> refundList = new List<string>();
            List<PayBillDetail> PayBillDetails = order.OrderPay.PayBillDetails;
            if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                var payBillList = this.order.OrderPay.PayBillDetails.Select(p => new { OpType = p.OpType, Money = p.Money }).ToList();
                //运营商分润
                var carrierFR = payBillList.Where(p => p.OpType == EnumOperationType.Profit).Select(p => p.Money).FirstOrDefault();
                //运营商服务费
                var carrierFW = payBillList.Where(p => p.OpType == EnumOperationType.Receivables).Select(p => p.Money).FirstOrDefault();
                if ((carrierFR + carrierFW) != 0)
                {
                    string args = string.Format("{0}^{1}^分润退款", this.order.Policy.CashbagCode, (carrierFR + carrierFW));
                    refundList.Add(args);
                }
            }
            else if (order.Policy.PolicySourceType == EnumPolicySourceType.Local)
            {

            }
            else if (order.Policy.PolicySourceType == EnumPolicySourceType.Share)
            {

            }
            return refundList.Count > 0 ? string.Join("|", refundList.ToArray()) : string.Empty;
        }
    }
}
