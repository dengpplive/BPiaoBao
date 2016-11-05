using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 接口订单代付
    /// </summary>
    [Behavior("PaidOrder")]
    public class PaidOrderBehavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            string areaCity = getParame("areaCity").ToString();
            string platformCode = getParame("PlatformCode").ToString();
            string operatorName = getParame("operatorName").ToString();
            string isNotify = getParame("isNotify").ToString();
            PlatformOrder platformOrder = null;
            try
            {
                if (order.Policy == null || order.Policy.PolicySourceType != EnumPolicySourceType.Interface)
                    throw new OrderCommException("该订单(" + order.OrderId + ")不是接口订单不能进行代付!");
                if (order.OrderPay.PayStatus == EnumPayStatus.NoPay)
                    throw new OrderCommException("该订单(" + order.OrderId + ")未支付不能进行代付操作!");
                if (order.OrderPay.PaidStatus == EnumPaidStatus.OK)
                {
                    throw new OrderCommException("该订单(" + order.OrderId + ")已经代付成功,不能重复支付同一个订单");
                }
                else
                {
                    bool paidIsTest = PlatformSection.GetInstances().Platforms[0].paidIsTest;
                    if (paidIsTest)
                    {
                        platformOrder = new PlatformOrder()
                        {
                            OrderId = order.OrderId,
                            AreaCity = areaCity,
                            OutOrderId = order.OutOrderId,
                            PnrCode = order.PnrCode,
                            TotlePaidPirce = order.OrderPay.PaidMoney,
                            TotaSeatlPrice = order.Passengers.Sum(p => p.SeatPrice)
                        };
                        PlatformFactory.Pay(platformCode, areaCity, platformOrder);
                        order.OrderPay.PaidMethod = platformOrder.PaidMethod.ToString();
                    }
                    else
                    {
                        //查看是否有补点
                        decimal bdMoney = 0m;
                        decimal PaidMoney = 0m;
                        decimal PayMoney = 0m;
                        foreach (PayBillDetail payDetail in order.OrderPay.PayBillDetails)
                        {
                            if (payDetail.AdjustType == AdjustType.Compensation)//补点
                            {
                                bdMoney += Math.Abs(payDetail.Money);
                            }
                        }
                        PaidMoney = order.OrderPay.PaidMoney;
                        PayMoney = (order.OrderPay.PayMoney + bdMoney);
                        //代付金额高于支付金额 就不代付
                        if (isNotify != "手动代付" && PaidMoney > PayMoney)
                        {
                            throw new PayInterfaceOrderException("代付金额(" + PaidMoney + ")高于用户支付金额(" + PayMoney + "),不进行代付！");
                        }
                        else
                        {
                            if (order.OrderPay.PaidMoney <= 0)
                            {
                                throw new PayInterfaceOrderException("支付金额无效！");
                            }
                            else if (order.OrderPay.PayMoney <= 0)
                            {
                                throw new PayInterfaceOrderException("支付金额无效！");
                            }
                            else
                            {
                                platformOrder = new PlatformOrder()
                                   {
                                       OrderId = order.OrderId,
                                       AreaCity = areaCity,
                                       OutOrderId = order.OutOrderId,
                                       PnrCode = order.PnrCode,
                                       TotlePaidPirce = order.OrderPay.PaidMoney,
                                       TotaSeatlPrice = order.Passengers.Sum(p => p.SeatPrice)
                                   };
                                PlatformFactory.Pay(platformCode, areaCity, platformOrder);
                                order.OrderPay.PaidMethod = platformOrder.PaidMethod.ToString();
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                order.WriteLog(new OrderLog()
                {
                    OperationContent = string.Format("日志来源:" + isNotify + ",接口订单号{0},{1}代付失败,失败信息:{2}", order.OutOrderId, platformCode, ex.Message),
                    OperationDatetime = DateTime.Now,
                    OperationPerson = operatorName
                    ,
                    IsShowLog = false
                });
                Logger.WriteLog(LogType.INFO, platformCode + "代付失败 时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + " OrderId=" + order.OrderId + " 接口订单号" + order.OutOrderId + " 异常信息=" + ex.Message + "\r\n");
                throw new OrderCommException(ex.Message);
            }
            order.WriteLog(new OrderLog()
            {
                OperationContent = string.Format("日志来源:{0},{1}代付成功,接口订单号{2}", isNotify, platformCode, order.OutOrderId),
                OperationDatetime = DateTime.Now,
                OperationPerson = operatorName,
                IsShowLog = false
            });
            order.OrderPay.PaidDateTime = System.DateTime.Now;
            //order.OrderPay.PaidStatus = EnumPaidStatus.OK;
            //代付成功修改状态
            order.ChangeStatus(EnumOrderStatus.WaitIssue);
            return null;
        }
    }
}
