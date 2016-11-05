using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 订单支付操作
    /// </summary>     
    [Behavior("PayOrder")]
    public class PayOrderBehavior : BaseOrderBehavior
    {
        private string GetPnrAndPassenger()
        {
            return string.Format("PNR:{0},乘机人:【{1}】", this.order.PnrCode, string.Join("|", this.order.Passengers.Select(x => x.PassengerName).ToArray()));
        }
        public override object Execute()
        {
            //string cashbagCode = getParame("Code").ToString();
            string cashbagCode = getParame("cashbagCode").ToString();
            string cashbagKey = getParame("cashbagKey").ToString();
            string collaboratorKey = getParame("collaboratorKey").ToString();
            string operatorName = getParame("operatorName").ToString();

            string platformCode = getParame("platformCode").ToString();
            decimal payMoney = order.OrderPay.PayMoney;//本次需要支付的总金额
            //if (order.OrderStatus == EnumOrderStatus.PaymentInWaiting)
            //    throw new OrderCommException("该订单(" + order.OrderId + ")正在支付中,请稍后。。。");
            if (order.OrderPay.PayStatus == EnumPayStatus.OK)
                throw new OrderCommException("该订单(" + order.OrderId + ")已经支付成功,不能重复支付同一个订单");
            //其他参数
            var ticketNotify = SettingSection.GetInstances().Payment.TicketNotify;
            EnumPayMethod payType = (EnumPayMethod)getParame("payType");
            IPaymentClientProxy client = new CashbagPaymentClientProxy();

            List<string> ProfitDetailList = new List<string>();
            foreach (PayBillDetail item in order.OrderPay.PayBillDetails)
            {
                if (item.OpType != EnumOperationType.PayMoney && item.OpType != EnumOperationType.Insurance && item.Money != 0)
                {
                    //接口
                    if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
                    {
                        if (item.OpType != EnumOperationType.Receivables)
                        {
                            ProfitDetailList.Add(item.CashbagCode + "^" + item.Money + "^" + item.OpType.ToEnumDesc());
                        }
                    }
                    else
                    {
                        //非接口
                        ProfitDetailList.Add(item.CashbagCode + "^" + item.Money + "^" + item.OpType.ToEnumDesc());
                    }
                }
            }
            //分润明细
            string ProfitDetail = string.Join("|", ProfitDetailList.ToArray());
            Logger.WriteLog(LogType.INFO, "支付分润 时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " OrderId=" + order.OrderId + " ProfitDetail=" + ProfitDetail + "\r\n"); 
            order.OrderPay.PayMethod = payType;
            object r = null;
            if (payType == EnumPayMethod.Bank)
            {
                string bank = getParame("bank").ToString();
                order.OrderPay.PayMethodCode = bank;
                order.ChangeStatus(EnumOrderStatus.PaymentInWaiting);
                r = client.PaymentByBank(cashbagCode, cashbagKey, order.OrderId, "机票订单", payMoney, bank, ticketNotify, operatorName, ProfitDetail, GetPnrAndPassenger());
            }
            else if (payType == EnumPayMethod.Platform)
            {
                string platform = getParame("platform").ToString();
                order.OrderPay.PayMethodCode = platform;
                switch (platform.ToLower())
                {
                    case "tenpay":
                        payType = EnumPayMethod.TenPay;
                        break;
                    case "alipay":
                        payType = EnumPayMethod.AliPay;
                        break;

                }
                order.ChangeStatus(EnumOrderStatus.PaymentInWaiting);
                r = client.PaymentByPlatform(cashbagCode, cashbagKey, order.OrderId, "机票订单", payMoney, platform, ticketNotify, operatorName, ProfitDetail, GetPnrAndPassenger());
                order.OrderPay.PayMethod = payType;
            }
            else
            {
                string serialNumber = "";
                try
                {
                    string payPassword = getParame("payPassword").ToString();
                    if (payType == EnumPayMethod.Credit)
                    {
                        order.OrderPay.PayMethodCode = "信用账户";
                        //验证是否在风控范围内，允许使用信用账户购买                        
                        //serialNumber = client.PaymentByCreditAccount(cashbagCode, cashbagKey, order.OrderId, "机票订单", payMoney, payPassword, ProfitDetail, GetPnrAndPassenger());
                        var result = client.PaymentByCreditAccount(cashbagCode, cashbagKey, order.OrderId, "机票订单",
                            payMoney, payPassword, ProfitDetail, GetPnrAndPassenger());
                        if (!result.Item1)
                        {
                            serialNumber = result.Item2;
                        }
                        else
                        {
                            //该订单已经被在线支付时写入日志
                            Logger.WriteLog(LogType.INFO,
                                "订单号" + order.OrderId + "已经支付，交易号为" + result.Item2 + "支付方式为" + result.Item3);
                            //如果查询结果已经支付，则恢复订单支付方式
                            switch (result.Item3.ToLower())
                            {
                                case "tenpay":
                                    order.OrderPay.PayMethod = EnumPayMethod.TenPay;
                                    break;
                                case "alipay":
                                    order.OrderPay.PayMethod = EnumPayMethod.AliPay;
                                    break;
                                case "internetbank":
                                    order.OrderPay.PayMethod = EnumPayMethod.Bank;
                                    break;
                            }
                        }
                    }
                    else if (payType == EnumPayMethod.Account)
                    {
                        order.OrderPay.PayMethodCode = "现金账户";
                        //serialNumber = client.PaymentByCashAccount(cashbagCode, cashbagKey, order.OrderId, "机票订单", payMoney, payPassword, ProfitDetail, GetPnrAndPassenger());
                        var result = client.PaymentByCashAccount(cashbagCode, cashbagKey, order.OrderId, "机票订单",
                            payMoney, payPassword, ProfitDetail, GetPnrAndPassenger());
                        if (!result.Item1)
                        {
                            serialNumber = result.Item2;
                        }
                        else
                        {
                            //该订单已经被在线支付时写入日志
                            Logger.WriteLog(LogType.INFO,
                                "订单号" + order.OrderId + "已经支付，交易号为" + result.Item2 + "支付方式为" + result.Item3);
                            //如果查询结果已经支付，则恢复订单支付方式
                            switch (result.Item3.ToLower())
                            {
                                case "tenpay":
                                    order.OrderPay.PayMethod = EnumPayMethod.TenPay;
                                    break;
                                case "alipay":
                                    order.OrderPay.PayMethod = EnumPayMethod.AliPay;
                                    break;
                                case "internetbank":
                                    order.OrderPay.PayMethod = EnumPayMethod.Bank;
                                    break;
                            }
                        }
                    }
                    else if (payType == EnumPayMethod.AliPay)
                    {
                        string quikalipay = payType.ToEnumDesc();
                        order.OrderPay.PayMethodCode = quikalipay;
                        order.ChangeStatus(EnumOrderStatus.PaymentInWaiting);
                        r = client.PaymentByQuikAliPay(cashbagCode, cashbagKey, order.OrderId, "机票订单", payMoney, quikalipay, ticketNotify, operatorName, payPassword, ProfitDetail, GetPnrAndPassenger());
                        if (r.ToString() != "True") serialNumber = r.ToString();
                    }
                }
                catch (Exception ex)
                {
                    //支付失败的日志
                    //抛出异常
                    order.WriteLog(new OrderLog()
                    {
                        OperationPerson = operatorName,
                        OperationDatetime = DateTime.Now,
                        OperationContent = "使用" + payType.ToEnumDesc() + "支付失败",
                        IsShowLog = true
                    });
                    Logger.WriteLog(LogType.INFO,
                        "支付失败 时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " OrderId=" +
                        order.OrderId + " 异常信息=" + ex.Message + "\r\n");
                    throw new CustomException(00001, ex.Message);
                }
                if (!string.IsNullOrEmpty(serialNumber))
                {
                    //支付成功后，修改日志。改变状态，进行代付
                    order.PayToPaid(operatorName, payType, order.OrderPay.PayMethodCode, serialNumber, "支付");
                }
            }
            return r;

        }

    }
}
