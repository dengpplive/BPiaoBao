using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common;
using PnrAnalysis;
using StructureMap;
using System.Threading;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 生成接口订单操作
    /// </summary>
    [Behavior("CreatePlatformOrder")]
    public class CreatePlatformOrderBehavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            //获取登录账号
            string OperationPerson = getParame("operatorName").ToString();
            string platformCode = getParame("PlatformCode").ToString();
            string PlatformName = getParame("PlatformName").ToString();
            //PlatformPolicy platformPolicy = null;
            PlatformOrder platformOrder = null;
            StringBuilder sblog = new StringBuilder();
            try
            {
                sblog.AppendFormat("订单号:{0}\r\n", order.OrderId);
                if (order.Policy == null)
                {
                    order.WriteLog(new OrderLog()
                    {
                        OperationContent = string.Format("{0}生成接口订单失败,订单号{1},失败信息:未选择政策", PlatformName, order.OrderId),
                        OperationDatetime = System.DateTime.Now,
                        OperationPerson = OperationPerson.ToString(),
                        Remark = "",
                        IsShowLog = false
                    });
                    order.ChangeStatus(EnumOrderStatus.CreatePlatformFail);
                    return null;
                }
                sblog.AppendFormat("platformCode={0}\r\n", platformCode);
                //PnrData pnrData = PnrHelper.GetPnrData(order.PnrContent);
                //platformPolicy = PlatformFactory.GetPlatformByCode(platformCode).GetPoliciesByPnrContent(order.PnrContent, order.IsLowPrice, pnrData).Find((p) => p.Id == order.Policy.PolicyId);
                //sblog.AppendFormat("platformPolicy==null\r\n");
                //if (platformPolicy == null)
                //{
                //    order.WriteLog(new OrderLog()
                //    {
                //        OperationContent = string.Format("{0}生成接口订单失败,订单号{1},失败信息:政策【{2}】发生变动，未获取到对应的政策", PlatformName, order.OrderId, order.Policy.PolicyId),
                //        OperationDatetime = System.DateTime.Now,
                //        OperationPerson = OperationPerson.ToString(),
                //        Remark = "",
                //        IsShowLog = false
                //    });
                //    order.ChangeStatus(EnumOrderStatus.CreatePlatformFail);
                //    sblog.AppendFormat("{0}生成接口订单失败,订单号{1},失败信息:政策【{2}】发生变动，未获取到对应的政策", PlatformName, order.OrderId, order.Policy.PolicyId);
                //    new CommLog().WriteLog("platformPolicyChange", sblog.ToString());
                //    return null;
                //}
                sblog.AppendFormat("开始CreateOrder\r\n");
                platformOrder = PlatformFactory.CreateOrder(platformCode, order.IsLowPrice, order.Policy.AreaCity, order.PnrContent, order.Policy.PolicyId, order.Policy.TodayGYCode, order.OrderId, order.Policy.PolicyPoint, order.Policy.ReturnMoney, null);
                sblog.AppendFormat("结束CreateOrder\r\n");
            }
            catch (Exception e)
            {
                sblog.AppendFormat("异常信息1:{0}\r\n", e.Message + e.StackTrace + e.TargetSite);
                try
                {
                    CancelIOrder(order.Policy.PlatformCode, OperationPerson);
                    Thread.Sleep(1000);
                    platformOrder = PlatformFactory.CreateOrder(platformCode, order.IsLowPrice, order.Policy.AreaCity, order.PnrContent, order.Policy.PolicyId, order.Policy.TodayGYCode, order.OrderId, order.Policy.PolicyPoint, order.Policy.ReturnMoney, null);
                }
                catch (Exception ex)
                {
                    sblog.AppendFormat("异常信息2:{0}\r\n", ex.Message + ex.StackTrace + ex.TargetSite);
                    order.WriteLog(new OrderLog()
                    {
                        OperationContent = string.Format("{0}生成接口订单失败,订单号{1},失败信息:{2}", PlatformName, order.OrderId, ex.Message),
                        OperationDatetime = System.DateTime.Now,
                        OperationPerson = OperationPerson.ToString(),
                        Remark = "",
                        IsShowLog = false
                    });
                    order.ChangeStatus(EnumOrderStatus.CreatePlatformFail);
                    new CommLog().WriteLog("CreatePlatformOrder_Exception", sblog.ToString());
                    throw new OrderCommException(ex.Message);
                }
            }
            order.OutOrderId = platformOrder.OutOrderId;
            order.OrderPay = order.OrderPay == null ? new OrderPay() : order.OrderPay;

            order.OrderPay.OrderId = order.OrderId;
            order.OrderPay.PaidMoney = platformOrder.TotlePaidPirce;
            order.OrderPay.PaidStatus = EnumPaidStatus.NoPaid;
            order.WriteLog(new OrderLog()
            {
                OperationContent = string.Format("{0}生成接口订单成功,接口订单号{1}", PlatformName, order.OutOrderId),
                OperationDatetime = System.DateTime.Now,
                OperationPerson = OperationPerson.ToString(),
                Remark = "",
                IsShowLog = false
            });
            order.ChangeStatus(EnumOrderStatus.WaitAndPaid);
            return null;
        }

        private void CancelIOrder(string platformCode, string operatorName)
        {
            string Message = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(order.OutOrderId)
                    && !string.IsNullOrEmpty(platformCode)
                    )
                {
                    var plateform = ObjectFactory.GetNamedInstance<IPlatform>(platformCode);//platformCode
                    StringBuilder sbPassenger = new StringBuilder();
                    foreach (var item in order.Passengers)
                    {
                        sbPassenger.Append(item.PassengerName + "^");
                    }
                    bool CanCancel = false;
                    string strData = plateform.GetOrderStatus(order.Policy.AreaCity, order.OrderId, order.OutOrderId, order.PnrCode);
                    switch (platformCode)
                    {
                        case "517":
                            if (strData.Contains("新订单等待支付"))
                            {
                                CanCancel = true;
                            }
                            break;
                        case "BaiTuo":
                            if (strData.Contains("预订成功,等待采购方支付"))
                            {
                                CanCancel = true;
                            }
                            break;
                        case "Today":
                            //if (strData.Contains("等待支付"))
                            //{
                            CanCancel = false;
                            //}
                            break;
                        case "PiaoMeng":
                            if (strData.Contains("尚未支付"))
                            {
                                CanCancel = true;
                            }
                            break;
                        case "51Book":
                            if (strData.Contains("新建订单"))
                            {
                                CanCancel = true;
                            }
                            break;
                        case "8000YI":
                            if (strData.Contains("新订单，等待支付"))
                            {
                                CanCancel = true;
                            }
                            break;
                        case "YeeXing":
                            if (strData.Contains("等待支付"))
                            {
                                CanCancel = true;
                            }
                            break;
                        default:
                            break;
                    }
                    if (CanCancel)
                    {
                        plateform.CancelOrder(order.Policy.AreaCity, order.OutOrderId, order.PnrCode, "",
                            sbPassenger.ToString().Trim(new char[] { '^' }));
                        Message = "取消接口订单号" + order.OutOrderId + "：取消成功";
                    }
                    else
                    {
                        Message = " 接口订单状态:" + strData + " ,不取消";
                    }
                }
            }
            catch (Exception ex)
            {
                Message = "取消接口订单号:" + order.OutOrderId + " 取消失败:" + ex.Message;
                Logger.WriteLog(LogType.ERROR, "[CancelOrder]订单号:" + order.OrderId + " 接口错误信息:" + ex.Message + "\r\n");
            }
            if (!string.IsNullOrEmpty(Message))
            {
                order.WriteLog(new OrderLog()
                {
                    OperationContent = Message,
                    OperationDatetime = DateTime.Now,
                    OperationPerson = operatorName,
                    IsShowLog = false
                });
                new CommLog().WriteLog("CancelOrder", Message);
            }
        }
    }
}
