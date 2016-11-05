using System.Globalization;
using AutoMapper;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Services;
using StructureMap;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using System.IO;
using System.Threading.Tasks;
using System.Diagnostics;
using BPiaoBao.AppServices.StationContracts.DomesticTicket.DomesticMap;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public partial class OrderService : IStationOrderService
    {



        /// <summary>
        /// 手动生成接口订单
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="orderId"></param>      
        [ExtOperationInterceptor("手动生成接口订单")]
        public void CreateInterfaceOrder(string platformCode, string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            var behavior = order.State.GetBehaviorByCode("CreatePlatformOrder");
            behavior.SetParame("operatorName", currentUser != null ? currentUser.OperatorName : "");
            behavior.SetParame("PlatformCode", platformCode);
            if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                behavior.SetParame("PlatformName", service.GetPlatList()[platformCode].Name);
            }
            else
            {
                behavior.SetParame("PlatformName", "系统");
            }
            try
            {
                behavior.Execute();
            }
            catch (Exception ex)
            {
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
                throw new OrderCommException(ex.Message);
            }
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }


        /// <summary>
        /// 手动代付接口订单
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="orderId"></param>
        [ExtOperationInterceptor("手动代付接口订单")]
        public void PaidOrderByPlatform(string platformCode, string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            if (order.Policy.PolicySourceType != EnumPolicySourceType.Interface)
                throw new OrderCommException("该订单号" + orderId + "不是接口订单,不能进行代付！");
            var behavior = order.State.GetBehaviorByCode("PaidOrder");
            behavior.SetParame("areaCity", order.Policy.AreaCity);
            behavior.SetParame("operatorName", currentUser != null ? currentUser.OperatorName : "");
            behavior.SetParame("PlatformCode", platformCode);
            behavior.SetParame("isNotify", "手动代付");
            behavior.SetParame("PlatformName", service.GetPlatList()[platformCode].Name);
            try
            {
                behavior.Execute();
            }
            catch (Exception ex)
            {
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
                throw new OrderCommException(ex.Message);
            }
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }


        /// <summary>
        /// 出票
        /// </summary>
        /// <param name="platformCode">平台接口Code 或者自定义名称</param>
        /// <param name="orderId"></param>
        /// <param name="ticketInfo"></param>
        [ExtOperationInterceptor("出票")]
        public void IssueTicket(string outOrderId, IList<PassengerTicketDto> ticketInfo, string remark)
        {
            var order = orderRepository.FindAll(p => p.OutOrderId == outOrderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到接口订单号为：" + outOrderId + "的订单");
            Dictionary<string, string> ticketDict = new Dictionary<string, string>();
            ticketInfo.ForEach(p =>
            {
                ticketDict.Add(p.Name.ToUpper().Trim(), p.TicketNumber);
            });
            if (ticketDict.Count == 0)
            {
                throw new OrderCommException("订单号" + outOrderId + "未获取到票号信息！");
            }
            var behavior = order.State.GetBehaviorByCode("TicketsIssue");
            behavior.SetParame("ticketDict", ticketDict);
            behavior.SetParame("operatorName", currentUser != null ? currentUser.OperatorName : "系统");
            behavior.SetParame("platformCode", order.Policy.PlatformCode);
            behavior.SetParame("opratorSource", "");
            behavior.SetParame("remark", remark);
            behavior.Execute();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            MessageQueueManager.SendMessage(order.OrderId, 0);
        }
        [ExtOperationInterceptor("手动出票")]
        public void HandIssueTicket(string orderId, IList<PassengerTicketDto> ticketInfo, string remark, string newPnr = "")
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找订单号为：" + orderId + "的订单");
            Dictionary<string, string> ticketDict = new Dictionary<string, string>();
            ticketInfo.ForEach(p =>
            {
                ticketDict.Add(p.Name.ToUpper().Trim(), p.TicketNumber);
            });
            if (!string.IsNullOrEmpty(newPnr) && order.IsChangePnrTicket)
            {
                order.WriteLog(new OrderLog
                {
                    IsShowLog = true,
                    OperationContent = string.Format("旧编码:{0},新编码:{1}", order.PnrCode, newPnr),
                    OperationDatetime = DateTime.Now,
                    OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
                });
                order.NewPnrCode = newPnr;
            }
            if (ticketDict.Count == 0)
            {
                throw new OrderCommException("订单号" + orderId + "未获取到票号信息！");
            }
            var behavior = order.State.GetBehaviorByCode("TicketsIssue");
            behavior.SetParame("ticketDict", ticketDict);
            behavior.SetParame("operatorName", currentUser != null ? currentUser.OperatorName : "系统");
            behavior.SetParame("platformCode", order.Policy.PlatformCode);
            behavior.SetParame("opratorSource", "");
            behavior.SetParame("remark", remark);
            behavior.Execute();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            MessageQueueManager.SendMessage(order.OrderId, 0);
        }


        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询订单状态")]
        public string QueryOrderStatus(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            string result = string.Empty;
            result = order.OrderStatus.ToEnumDesc();
            return result;
        }


        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="orderStatus"></param>
        [ExtOperationInterceptor("修改订单状态")]
        public void UpdateOrderStatus(string orderId, int? orderStatus)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");

            EnumOrderStatus enumOrderStatus = order.OrderStatus;
            if (orderStatus.HasValue)
            {
                string oldOrderStatus = enumOrderStatus.ToEnumDesc();
                switch (orderStatus.Value)
                {
                    case 0:
                        enumOrderStatus = EnumOrderStatus.WaitChoosePolicy; break;
                    case 1:
                        enumOrderStatus = EnumOrderStatus.CreatePlatformFail; break;
                    case 2:
                        enumOrderStatus = EnumOrderStatus.NewOrder; break;
                    case 3:
                        enumOrderStatus = EnumOrderStatus.WaitAndPaid; break;
                    case 4:
                        enumOrderStatus = EnumOrderStatus.WaitIssue; break;
                    case 5:
                        enumOrderStatus = EnumOrderStatus.OrderCanceled; break;
                    case 6:
                        enumOrderStatus = EnumOrderStatus.IssueAndCompleted; break;
                    case 7:
                        enumOrderStatus = EnumOrderStatus.WaitReimburseWithRepelIssue; break;
                    case 8:
                        enumOrderStatus = EnumOrderStatus.WaitReimburseWithPlatformRepelIssue; break;
                    case 9:
                        enumOrderStatus = EnumOrderStatus.RepelIssueAndCompleted; break;
                    case 10:
                        enumOrderStatus = EnumOrderStatus.Invalid; break;
                    case 11:
                        enumOrderStatus = EnumOrderStatus.AutoIssueFail; break;
                    default:
                        break;
                }
                string newOrderStatus = enumOrderStatus.ToEnumDesc();
                if (oldOrderStatus != newOrderStatus)
                {
                    order.ChangeStatus(enumOrderStatus);
                    order.WriteLog(new OrderLog()
                    {
                        OperationContent = "订单号" + order.OrderId + "原订单状态：" + oldOrderStatus + " 修改后订单状态:" + newOrderStatus,
                        OperationDatetime = System.DateTime.Now,
                        OperationPerson = currentUser.OperatorName
                        ,
                        IsShowLog = false
                    });
                    //修改
                    unitOfWorkRepository.PersistUpdateOf(order);
                    unitOfWork.Commit();
                }
                else
                {
                    throw new OrderCommException("订单号：" + orderId + "修改前后订单状态一致,无须修改！");
                }
            }
        }


        /// <summary>
        /// 修改支付/代付状态
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="updateType">0.修改支付状态 1.修改代付状态</param>
        /// <param name="Status">状态值 0未支/代付 1已支/代付</param>
        [ExtOperationInterceptor("修改支付/代付状态")]
        public void UpdatePayOrPaidStatus(string orderId, int updateType, int? Status)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            if (Status.HasValue)
            {
                StringBuilder sbLog = new StringBuilder();
                if (updateType == 0)//修改支付状态
                {
                    EnumPayStatus enumPayStatus = order.OrderPay.PayStatus;
                    if ((int)enumPayStatus != Status.Value)
                    {
                        order.OrderPay.PayStatus = Status.Value == 0 ? EnumPayStatus.NoPay : EnumPayStatus.OK;
                        sbLog.AppendFormat("订单号{0}原支付状态修改为{1}", order.OrderId, order.OrderPay.PayStatus.ToEnumDesc());
                    }
                    else
                    {
                        throw new OrderCommException("订单号" + orderId + "支付状态修改前后一致,无须修改");
                    }
                }
                else if (updateType == 1)//修改代付状态
                {
                    EnumPaidStatus enumPaidStatus = order.OrderPay.PaidStatus;
                    if ((int)enumPaidStatus != Status.Value)
                    {
                        order.OrderPay.PaidStatus = Status.Value == 0 ? EnumPaidStatus.NoPaid : EnumPaidStatus.OK;
                        sbLog.AppendFormat("订单号{0}原代付状态 修改为{1}", order.OrderId, order.OrderPay.PaidStatus.ToEnumDesc());
                    }
                    else
                    {
                        throw new OrderCommException("订单号" + orderId + "代付状态修改前后一致,无须修改");
                    }
                }
                else
                {
                    throw new OrderCommException("订单号" + orderId + "修改类型错误");
                }
                order.WriteLog(new OrderLog()
                {
                    OperationContent = sbLog.ToString(),
                    OperationDatetime = System.DateTime.Now,
                    OperationPerson = currentUser.OperatorName
                    ,
                    IsShowLog = false
                });
                //修改
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
            }
        }

        /// <summary>
        /// 查询支付状态
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="flag">标识是否为支付窗口支付时查询订单状态</param>
        /// <returns></returns>
        [ExtOperationInterceptor("查询支付状态")]
        public string QueryPayStatus(string orderId, string flag = null)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            if (order.OrderStatus != EnumOrderStatus.NewOrder && order.OrderStatus != EnumOrderStatus.PaymentInWaiting)
                return string.Format("{0}已支付", orderId);
            var businessman = this.businessmanRepository.FindAll(x => x.Code == order.BusinessmanCode).FirstOrDefault();

            string result = string.Empty;
            //查询支付状态
            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            //string serialNumber = client.PayStateQuery(businessman.CashbagCode, businessman.CashbagKey, orderId).Item1;
            var queryresult = client.PayStateQuery(businessman.CashbagCode, businessman.CashbagKey, orderId, flag);
            var serialNumber = queryresult.Item1;
            var payType = queryresult.Item2;
            var bankCode = queryresult.Item3;
            EnumPayMethod? payMethod=null;
            switch (payType.ToLower())
            {
                case "tenpay":
                    payMethod = EnumPayMethod.TenPay;
                    break;
                case "alipay":
                    payMethod = EnumPayMethod.AliPay;
                    break;
                case "internetbank":
                    payMethod = EnumPayMethod.Bank;
                    payType = bankCode;
                    break;
            }

            var sbLog = new StringBuilder();
            sbLog.AppendFormat("查询支付状态：{0},支付方式:{1}", order.OrderPay.PayStatus.ToEnumDesc(), payMethod.HasValue ? payMethod.Value.ToEnumDesc():"");
            order.WriteLog(new OrderLog()
            {
                OperationContent = sbLog.ToString(),
                OperationDatetime =DateTime.Now,
                OperationPerson = currentUser.OperatorName,
                IsShowLog = flag==null
            });
            if (!string.IsNullOrEmpty(serialNumber))
            {
                if (serialNumber != "F")
                {
                    //如果查询结果已经支付，则恢复订单支付方式
                    if (payMethod.HasValue)
                    {
                        order.OrderPay.PayMethod = payMethod.Value;
                    }

                    order.PayToPaid(currentUser.OperatorName, order.OrderPay.PayMethod, payType, serialNumber, "查询支付状态");
                    result = "已支付";
                }
                else
                {
                    order.OrderPay.PayStatus = EnumPayStatus.NoPay;
                    result = "未支付";
                }
            }
            else
            {
                order.OrderPay.PayStatus = EnumPayStatus.NoPay;
                result = "未支付";
            }
            
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            Logger.WriteLog(LogType.INFO, "订单号:" + orderId + " " + sbLog.ToString());
            return result;
        }


        /// <summary>
        /// 售后订单查询支付状态
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <returns></returns>
        [ExtOperationInterceptor("售后订单查询支付状态")]
        public string QueryAfterSaleOrderPayStatus(int orderId)
        {
            var result = string.Empty;
            var order = afterSaleOrderRepository.FindAll(p => p.Id == orderId).OfType<ChangeOrder>().FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到售后订单编号为：" + orderId + "的订单");
            if (order.PayStatus == EnumChangePayStatus.Payed)
                result = "已支付";
            var businessman = this.businessmanRepository.FindAll(x => x.Code == order.Order.BusinessmanCode).FirstOrDefault();
            //查询支付状态
            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            if (businessman != null)
            {
                var queryresult = client.PayStateQuery(businessman.CashbagCode, businessman.CashbagKey, "A_" + orderId.ToString(CultureInfo.InvariantCulture), "pay");
                var serialNumber = queryresult.Item1;
                var payType = queryresult.Item2;
                EnumPayMethod? payMethod = null;
                switch (payType.ToLower())
                {
                    case "tenpay":
                        payMethod = EnumPayMethod.TenPay;
                        break;
                    case "alipay":
                        payMethod = EnumPayMethod.AliPay;
                        break;
                    case "internetbank":
                        payMethod = EnumPayMethod.Bank; 
                        break;
                }
                var sbLog = new StringBuilder();
                sbLog.AppendFormat("查询支付状态：{0},支付方式:{1}", order.PayStatus.ToEnumDesc(), payMethod.HasValue ? payMethod.Value.ToEnumDesc() : "");
                if (!string.IsNullOrEmpty(serialNumber))
                {
                    if (serialNumber != "F")
                    {
                        //赋值相关支付信息
                        order.ProcessStatus = EnumTfgProcessStatus.WaitIssue;
                        order.OutPayNo = serialNumber;
                        if (payMethod.HasValue) order.PayWay = payMethod.Value;
                        order.PayTime = DateTime.Now;
                        result = "已支付";
                    }
                    else
                    {
                        order.PayStatus = EnumChangePayStatus.NoPay;
                        result = "未支付";
                    }
                }
                else
                {
                    order.PayStatus = EnumChangePayStatus.NoPay;
                    result = "未支付";
                }
                order.WriteLog(new OrderLog()
                {
                    OperationContent = sbLog.ToString(),
                    OperationDatetime = System.DateTime.Now,
                    OperationPerson = currentUser.OperatorName,
                    IsShowLog = false
                });
                //修改
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
                Logger.WriteLog(LogType.INFO, "售后订单号:" + orderId + " " + sbLog.ToString());
            }
            return result;
        }

        /// <summary>
        /// 代付成功通知 修改订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="platform"></param>
        [ExtOperationInterceptor("代付成功通知,修改订单")]
        public void UpdateOrderByPayNotify(string outOrderId, string platform, string SerialNumber, decimal PaidMeony)
        {
            string result = string.Empty;
            var order = orderRepository.FindAll(p => p.OutOrderId == outOrderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到" + platform + "订单号为：" + outOrderId + "的订单");
            if (order.OrderPay.PaidStatus != EnumPaidStatus.OK && !string.IsNullOrEmpty(SerialNumber.Trim()) && PaidMeony > 0)
            {
                if (platform != "PiaoMeng" && platform != "517")
                {
                    order.OrderPay.PaidMoney = PaidMeony;
                }
                order.OrderPay.PaidStatus = EnumPaidStatus.OK;
                order.OrderPay.PaidSerialNumber = SerialNumber;
                order.OrderPay.PaidDateTime = !order.OrderPay.PaidDateTime.HasValue ? System.DateTime.Now : order.OrderPay.PaidDateTime;
                if (order.OrderStatus == EnumOrderStatus.WaitAndPaid) order.ChangeStatus(EnumOrderStatus.WaitIssue);
                order.WriteLog(new OrderLog()
                {
                    OperationContent = "通知: 订单号(" + order.OrderId + "),接口订单(" + outOrderId + ")," + platform + "代付成功",
                    OperationDatetime = DateTime.Now,
                    OperationPerson = "系统"
                    ,
                    IsShowLog = false
                });
                //修改
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
            }
        }


        /// <summary>
        /// 平台(接口)取消出票通知
        /// </summary>
        /// <param name="outOrderId"></param>
        /// <param name="nameValueCollection"></param>
        [ExtOperationInterceptor("平台(接口)取消出票通知")]
        public void PlatformCancelIssueNotify(string outOrderId, string remark)
        {
            var order = orderRepository.FindAll(p => p.OutOrderId == outOrderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到接口订单号为：" + outOrderId + "的订单");

            var behavior = order.State.GetBehaviorByCode("PlatformCancelTicketNotify");
            string OperatorName = currentUser != null ? currentUser.OperatorName : "系统";
            behavior.SetParame("operatorName", OperatorName);
            behavior.SetParame("remark", remark);
            behavior.Execute();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }

        /// <summary>
        /// 取消出票 系统
        /// </summary>
        /// <param name="outOrderId"></param>
        /// <param name="nameValueCollection"></param>
        [ExtOperationInterceptor("取消出票(系统)")]
        public void CancelIssueNotify(string orderId, string remark)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");

            var behavior = order.State.GetBehaviorByCode("CancelIssueTicket");
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("remark", remark);
            behavior.Execute();
            if (!string.IsNullOrEmpty(order.OldOrderId) && order.OrderType == 1)
            {
                var oldOrder = orderRepository.FindAll(p => p.OrderId.Equals(order.OldOrderId)).FirstOrDefault();
                if (oldOrder != null)
                {
                    oldOrder.AssocChdCount -= order.Passengers.Count();
                    unitOfWorkRepository.PersistUpdateOf(oldOrder);
                }
            }
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }


        /// <summary>
        /// 平台(接口)退款通知
        /// </summary>
        /// <param name="outOrderId">接口订单号</param>
        /// <param name="nameValueCollection"></param>
        [ExtOperationInterceptor("平台(接口)退款通知")]
        public void PlatformRefundNotify(string outOrderId, decimal refundMoney, string remark)
        {
            var order = orderRepository.FindAll(p => p.OutOrderId == outOrderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到接口订单号为：" + outOrderId + "的订单");
            if (order.OrderStatus == EnumOrderStatus.WaitReimburseWithRepelIssue ||
                order.OrderStatus == EnumOrderStatus.WaitReimburseWithPlatformRepelIssue || order.OrderStatus == EnumOrderStatus.WaitIssue
                )
            {
                var behavior = order.State.GetBehaviorByCode("PlatformRefundNotify");
                behavior.SetParame("refundMoney", refundMoney);
                behavior.SetParame("remark", remark);
                behavior.SetParame("operatorName", currentUser != null ? currentUser.OperatorName : "系统");
                behavior.SetParame("platformCode", order.Policy.PlatformCode);
                behavior.Execute();
                //修改
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
            }
        }


        /// <summary>
        /// 手动钱袋子退款
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="refundMoney">退款金额</param>
        /// <param name="remark">备注</param>
        [ExtOperationInterceptor("手动退款(系统)")]
        public void CashbagRefund(string orderId, decimal refundMoney, string remark)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            var businessman = this.businessmanRepository.FindAll(x => x.Code == order.BusinessmanCode).FirstOrDefault();

            var behavior = order.State.GetBehaviorByCode("WaitReimburseWithRepelIssue");
            behavior.SetParame("refundMoney", refundMoney);
            behavior.SetParame("remark", remark);
            behavior.SetParame("Code", businessman.Code);
            behavior.SetParame("cashbagCode", businessman.CashbagCode);
            behavior.SetParame("cashbagKey", businessman.CashbagKey);
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("platformCode", order.Policy.PlatformCode);
            if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                behavior.SetParame("platformName", service.GetPlatList()[order.Policy.PlatformCode].Name);
            }
            else
            {
                behavior.SetParame("PlatformName", "系统");
            }
            behavior.Execute();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            MessageQueueManager.SendMessage(orderId, 2);
        }

        //[ExtOperationInterceptor("获取订单详情【GetOrderDetail】")]
        public OrderDetailDto GetOrderDetail(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            return order.ToOrderDetail();
        }
        //[ExtOperationInterceptor("获取订单详情（运营）【GetCarrierOrderDetail】")]
        public OrderDetailDto GetCarrierOrderDetail(string orderId)
        { //|| t.OpType == EnumOperationType.PayMoney
            var currentcode = currentUser.Code;
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (AuthManager.GetCurrentUser().Type == "Supplier")
                order.OrderPay.PayBillDetails = order.OrderPay.PayBillDetails.Where(t => t.Code == currentcode).ToList();
            else
            {

                if (order.Policy != null && order.Policy.PolicySourceType == EnumPolicySourceType.Share)
                {
                    //异地票不显示支付金额
                    order.OrderPay.PayBillDetails = order.OrderPay.PayBillDetails.Where(t => t.Code == currentcode || t.Code == order.Policy.Code).ToList();
                }
                else if (order.Policy != null && order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
                {
                    //接口票显示相关
                    order.OrderPay.PayBillDetails = order.OrderPay.PayBillDetails.Where(t => t.Code == currentcode || t.Code == order.Policy.Code || t.OpType == EnumOperationType.PayMoney).ToList();
                }
            }
            return order.ToOrderDetail();
        }
        /// <summary>
        /// 查询代付状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        //[ExtOperationInterceptor("查询代付状态")]
        public string QueryPaidStatus(string orderId)
        {
            string result = "";
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            if (string.IsNullOrEmpty(order.OutOrderId))
                throw new OrderCommException("该订单号" + orderId + "未找到相应的接口订单");
            var behavior = order.State.GetBehaviorByCode("QueryPaidStatus");
            behavior.SetParame("operatorName", currentUser != null ? currentUser.OperatorName : "");
            result = behavior.Execute().ToString();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            return result;
        }
        /// <summary>
        /// 自动复合接口票号
        /// </summary>
        /// <param name="orderId"></param>
        [ExtOperationInterceptor("自动复合接口票号")]
        public void AutoCompositeTicket(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            Dictionary<string, string> ticketDict = new Dictionary<string, string>();
            if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                if (string.IsNullOrEmpty(order.OutOrderId))
                    throw new OrderCommException("该订单号" + orderId + "未找到相应的接口订单");
                ticketDict = service.AutoCompositeTicket(order.Policy.PlatformCode, order.Policy.AreaCity, orderId, order.OutOrderId, order.PnrCode);
            }
            else
            {
                string Pnr = order.PnrCode;
                string strCmd = string.Format("RT{0}", Pnr);
                string strRTContent = this.pidService.SendCmd(currentUser.Code, strCmd, "", "", true).Replace("^", "\r");
                if (strRTContent.Contains("授权"))
                    throw new OrderCommException(strRTContent);
                string errMsg = string.Empty;
                PnrAnalysis.PnrModel pnrModel = format.GetPNRInfo(Pnr, strRTContent, false, out errMsg);
                if (pnrModel != null)
                {
                    foreach (PnrAnalysis.Model.PassengerInfo item in pnrModel._PassengerList)
                    {
                        ticketDict.Add(item.PassengerName, item.TicketNum);
                    }
                }
            }
            if (ticketDict == null || ticketDict.Count == 0)
                throw new OrderCommException("该订单号" + orderId + "未自动取到票号信息！");
            var behavior = order.State.GetBehaviorByCode("TicketsIssue");
            behavior.SetParame("ticketDict", ticketDict);
            behavior.SetParame("operatorName", currentUser != null ? currentUser.OperatorName : "");
            behavior.SetParame("platformCode", order.Policy.PlatformCode);
            behavior.SetParame("opratorSource", "自动复合");
            behavior.Execute();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            MessageQueueManager.SendMessage(order.OrderId, 0);
        }
        /// <summary>
        /// 选择政策
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="policyId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("控台选择政策【BackChoosePolicy(string platformCode, string policyId, string orderId)】")]
        public OrderDto BackChoosePolicy(string platformCode, string policyId, string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            if (order.OrderStatus == EnumOrderStatus.PaymentInWaiting)
                throw new OrderCommException("该订单【" + orderId + "】正在支付中,请稍后。。。");

            var behavior = order.State.GetBehaviorByCode("NewSelectPolicy");
            behavior.SetParame("platformCode", platformCode);
            behavior.SetParame("policyId", policyId);
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("source", "back");
            behavior.Execute();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            return order.ToOrderDto();
        }

        //[ExtOperationInterceptor("根据原订单号获取新的政策列表并且生成默认订单")]
        public PolicyPack GetPolicyList(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            PolicyParam policyParam = new PolicyParam();
            policyParam.IsLowPrice = order.IsLowPrice;
            policyParam.code = order.BusinessmanCode;
            policyParam.PnrContent = order.PnrContent;
            policyParam.OrderId = order.OrderId;
            policyParam.OrderType = order.OrderType;
            policyParam.OrderSource = order.OrderSource;
            policyParam.IsChangePnrTicket = order.IsChangePnrTicket;
            policyParam.IsDestine = order.OrderSource == EnumOrderSource.WhiteScreenDestine ? true : false;
            Passenger pasData = order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).FirstOrDefault();
            if (pasData != null)
            {
                policyParam.defFare = pasData.SeatPrice.ToString();
                policyParam.defTAX = pasData.ABFee.ToString();
                policyParam.defRQFare = pasData.RQFee.ToString();
            }
            //获取政策
            IList<Policy> policyLisy = NewGetPolicy(policyParam);
            PolicyPack policyPack = GetPolicy(policyLisy, order);

            policyPack.PolicyList = policyPack.PolicyList.Where(p => p.PlatformCode != "系统").ToList();
            return policyPack;
        }

        /// <summary>
        /// 同步订单状态
        /// </summary>
        /// <param name="orderId"></param>
        [ExtOperationInterceptor("同步订单状态")]
        public string SynOrder(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            if (string.IsNullOrEmpty(order.OutOrderId))
                throw new OrderCommException("该订单号" + orderId + "未找到相应的接口订单");

            if (order.Policy.PolicySourceType != EnumPolicySourceType.Interface)
                throw new OrderCommException("该订单号" + orderId + "不是接口订单，不能同步数据！");
            bool IsUpdate = false;
            string PlatformCode = order.Policy.PlatformCode;
            string strOrderStatus = service.GetOrderStatus(PlatformCode, order.Policy.AreaCity, order.OrderId, order.OutOrderId, order.PnrCode);
            if (string.IsNullOrEmpty(strOrderStatus))
                throw new OrderCommException("该订单号" + orderId + "未获取到接口订单状态！");

            if (PlatformCode == "8000YI")
            {
                if (strOrderStatus.Contains("已经付款，等待出票"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitIssue);
                }
                else if (strOrderStatus.Contains("已经出票，交易结束"))
                {
                    if (order.OrderStatus != EnumOrderStatus.IssueAndCompleted)
                    {
                        IsUpdate = true;
                        try
                        {
                            AutoCompositeTicket(order.OrderId);
                            order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                            order.IssueTicketTime = System.DateTime.Now;
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, string.Format("自动符合票和出错[同步订单]平台:{0}", PlatformCode), e);
                            throw new CustomException(500, e.Message);
                        }
                    }
                }
                else if (strOrderStatus.Contains("已经退票，交易结束")
                 || strOrderStatus.Contains("已经废票，交易结束")
                )
                {
                    if (order.OrderStatus == EnumOrderStatus.RepelIssueRefunding
                        || order.OrderStatus == EnumOrderStatus.RepelIssueAndCompleted
                        )
                    {
                        IsUpdate = true;
                        order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                    }
                }
                else if (strOrderStatus.Contains("暂停出票，待处理")
                    || strOrderStatus.Contains("取消出票，已退款"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                }

            }
            else if (PlatformCode == "517")
            {
                if (strOrderStatus.Contains("已经付款，等待出票"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitIssue);
                }
                else if (strOrderStatus.Contains("已经出票，交易结束"))
                {
                    if (order.OrderStatus != EnumOrderStatus.IssueAndCompleted)
                    {
                        IsUpdate = true;
                        try
                        {
                            AutoCompositeTicket(order.OrderId);
                            order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                            order.IssueTicketTime = System.DateTime.Now;
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, string.Format("自动符合票和出错[同步订单]平台:{0} 订单号:{1}", PlatformCode, order.OrderId), e);
                            throw new CustomException(500, e.Message);
                        }
                    }
                }
                else if (strOrderStatus.Contains("暂不能出票，等待处理")
                    )
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                }
                else if (strOrderStatus.Contains("已经退款，交易结束"))
                {
                    if (order.OrderStatus == EnumOrderStatus.RepelIssueRefunding
                           || order.OrderStatus == EnumOrderStatus.RepelIssueAndCompleted
                           )
                    {
                        IsUpdate = true;
                        order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                        //}
                        //else if (order.OrderStatus == EnumOrderStatus.WaitIssue)
                        //{
                        //    IsUpdate = true;
                        //    order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                    }
                }
            }
            else if (PlatformCode == "51book")
            {
                if (strOrderStatus.Contains("已支付，等待出票"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitIssue);
                }
                else if (strOrderStatus.Contains("无法出票"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                }
                else if (strOrderStatus.Contains("出票成功"))
                {
                    if (order.OrderStatus != EnumOrderStatus.IssueAndCompleted)
                    {
                        IsUpdate = true;
                        try
                        {
                            AutoCompositeTicket(order.OrderId);
                            order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                            order.IssueTicketTime = System.DateTime.Now;

                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, string.Format("自动符合票和出错[同步订单]平台:{0}", PlatformCode), e);
                            throw new CustomException(500, e.Message);
                        }
                    }
                }
                else if (strOrderStatus.Contains("采购取消并退款"))
                {
                    if (order.OrderStatus == EnumOrderStatus.RepelIssueRefunding
                          || order.OrderStatus == EnumOrderStatus.RepelIssueAndCompleted
                          )
                    {

                        IsUpdate = true;
                        order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                    }
                }
            }
            else if (PlatformCode == "PiaoMeng")
            {
                if (strOrderStatus.Contains("等待出票"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitIssue);
                }
                else if (strOrderStatus.Contains("无法出票"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                }
                else if (strOrderStatus.Contains("出票完成"))
                {
                    if (order.OrderStatus != EnumOrderStatus.IssueAndCompleted)
                    {
                        IsUpdate = true;
                        try
                        {
                            AutoCompositeTicket(order.OrderId);
                            order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                            order.IssueTicketTime = System.DateTime.Now;
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, string.Format("自动符合票和出错[同步订单]平台:{0}", PlatformCode), e);
                            throw new CustomException(500, e.Message);
                        }
                    }
                }
                else if (strOrderStatus.Contains("完成退款"))
                {
                    if (order.OrderStatus == EnumOrderStatus.RepelIssueRefunding
                         || order.OrderStatus == EnumOrderStatus.RepelIssueAndCompleted
                         )
                    {
                        IsUpdate = true;
                        order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                    }
                }
                else if (strOrderStatus.Contains("交易取消已退款"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                }
            }
            else if (PlatformCode == "Today")
            {
                if (strOrderStatus.Contains("支付成功"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitIssue);
                }
                else if (strOrderStatus.Contains("暂不能出票")
                    || strOrderStatus.Contains("航班取消")
                    || strOrderStatus.Contains("退款中")
                    || strOrderStatus.Contains("取消订单")
                    || strOrderStatus.Contains("取消申请退款")
                    )
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                }
                else if (strOrderStatus.Contains("出票完成")
                    || strOrderStatus.Contains("出票成功"))
                {
                    if (order.OrderStatus != EnumOrderStatus.IssueAndCompleted)
                    {
                        IsUpdate = true;
                        try
                        {
                            AutoCompositeTicket(order.OrderId);
                            order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                            order.IssueTicketTime = System.DateTime.Now;

                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, string.Format("自动符合票和出错[同步订单]平台:{0}", PlatformCode), e);
                            throw new CustomException(500, e.Message);
                        }
                    }
                }
                else if (strOrderStatus.Contains("退款成功"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithRepelIssue);
                }
            }
            else if (PlatformCode == "YeeXing")
            {
                if (strOrderStatus.Contains("支付成功"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitIssue);
                }
                else if (strOrderStatus.Contains("拒绝出票"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                }
                else if (strOrderStatus.Contains("已出票,票到付款"))
                {
                    if (order.OrderStatus != EnumOrderStatus.IssueAndCompleted)
                    {
                        IsUpdate = true;
                        try
                        {
                            AutoCompositeTicket(order.OrderId);
                            order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                            order.IssueTicketTime = System.DateTime.Now;
                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, string.Format("自动符合票和出错[同步订单]平台:{0}", PlatformCode), e);
                            throw new CustomException(500, e.Message);
                        }
                    }
                }
            }
            else if (PlatformCode == "BaiTuo")
            {
                if (strOrderStatus.Contains("采购方已取消订单,交易结束")
                    || strOrderStatus.Contains("出票方已取消订单,交易结束")
                    || strOrderStatus.Contains("直接取消订单,待出票方退款")
                    )
                {
                    if (order.OrderStatus == EnumOrderStatus.RepelIssueRefunding
                       || order.OrderStatus == EnumOrderStatus.RepelIssueAndCompleted
                        )
                    {
                        IsUpdate = true;
                        order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                    }
                }
                else if (strOrderStatus.Contains("出票方完成退款,交易结束"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitReimburseWithPlatformRepelIssue);
                }
                else if (strOrderStatus.Contains("支付成功,等待出票方出票"))
                {
                    IsUpdate = true;
                    order.ChangeStatus(EnumOrderStatus.WaitIssue);
                }
                else if (strOrderStatus.Contains("出票成功,交易结束"))
                {
                    if (order.OrderStatus != EnumOrderStatus.IssueAndCompleted)
                    {
                        IsUpdate = true;
                        try
                        {
                            AutoCompositeTicket(order.OrderId);
                            order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                            order.IssueTicketTime = System.DateTime.Now;

                        }
                        catch (Exception e)
                        {
                            Logger.WriteLog(LogType.ERROR, string.Format("自动符合票和出错[同步订单]平台:{0}", PlatformCode), e);
                            throw new CustomException(500, e.Message);
                        }
                    }
                }
            }
            if (IsUpdate)
            {
                order.WriteLog(new OrderLog()
                {
                    OperationContent = "手动同步" + PlatformCode + "订单,订单号:" + order.OrderId + ",接口订单号:" + order.OutOrderId + ",获取到订单状态:" + strOrderStatus + "",
                    OperationDatetime = System.DateTime.Now,
                    OperationPerson = currentUser.OperatorName,
                    IsShowLog = false
                });
                //修改
                unitOfWorkRepository.PersistUpdateOf(order);
                unitOfWork.Commit();
            }
            return strOrderStatus;
        }

        #region 协调
        //[ExtOperationInterceptor("获取订单协调信息")]
        public CoordinationDto GetCoordinationDto(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            return order.ToOrderCoordination();
        }
        [ExtOperationInterceptor("添加订单协调信息")]
        public void AddCoordinationDto(string orderId, string type, string CoordinationContent, bool IsCompleted)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            if (CoordinationContent.Length > 500)
                throw new OrderCommException("协调内容超出范围！");
            if (order.CoordinationLogs != null && order.CoordinationLogs.Count > 100)
                throw new OrderCommException("协调数目最多100条！");

            order.WriteCoordinationContent(new CoordinationLog()
            {
                AddDatetime = System.DateTime.Now,
                Type = type,
                Content = CoordinationContent,
                OperationPerson = currentUser.OperatorName
            });
            order.CoordinationStatus = IsCompleted;
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }
        #endregion


        #region 订单加锁 解锁 判断是否可以操作订单
        [ExtOperationInterceptor("锁定订单")]
        public void LockOrder(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");

            order.LockAccount = currentUser.OperatorAccount;
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }

        [ExtOperationInterceptor("解锁订单")]
        public void UnLockOrder(string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");

            order.LockAccount = string.Empty;
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }

        //[ExtOperationInterceptor("是否可以操作订单")]
        public bool CanOperationOrder(string orderId)
        {
            bool hasOpration = false;
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderId + "的订单");
            if (string.IsNullOrEmpty(order.LockAccount))
                throw new OrderCommException("该订单号（" + orderId + "）未加锁，操作前需要先加锁！");
            if (order.LockAccount != currentUser.OperatorAccount)
                throw new OrderCommException("订单号（" + orderId + "）已被" + currentUser.OperatorAccount + "锁定,请先解锁后再操作！");
            hasOpration = true;
            return hasOpration;
        }
        #endregion
        [ExtOperationInterceptor("售后订单处理")]
        public void Process(int saleorderid, Dictionary<int, decimal> dic, string remark)
        {
            if (dic == null || dic.Count == 0)
                throw new CustomException(500, "乘机人不能为空");
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该订单售后信息不存在");
            IsLock(model.LockCurrentAccount);
            string logMsg = string.Empty;
            dic.ForEach(p =>
            {
                logMsg += string.Format("passengerID:{0},money:{1}||", p.Key, p.Value);
            });
            Logger.WriteLog(LogType.INFO, logMsg);
            var list = model.Passenger.Where(p => dic.ContainsKey(p.PassengerId)).ToList();
            //退改签手续费总和
            decimal money = dic.Sum(p => p.Value);

            list.ForEach(p =>
            {
                //单人退改签手续费
                decimal sigMoney = dic.Where(x => x.Key == p.PassengerId).FirstOrDefault().Value;
                if (sigMoney < 0)
                    throw new CustomException(500, "手续费不能小于0");
                //退改交易金额=[支付金额-手续费]
                decimal rmoney = (p.Passenger.PayMoney - dic.Where(x => x.Key == p.PassengerId).FirstOrDefault().Value);
                if (model is BounceOrder || model is AnnulOrder)
                {
                    if (rmoney < 0 || rmoney > p.Passenger.PayMoney)
                        throw new CustomException(500, string.Format("{0}交易金额错误", model.AfterSaleType));
                    p.RetirementMoney = rmoney;
                    p.RetirementPoundage = sigMoney;
                }
                else if (model is ChangeOrder)
                {
                    p.RetirementMoney = sigMoney;
                    p.RetirementPoundage = sigMoney;
                }
                p.Status = EnumTfgPassengerStatus.Processing;
            });


            if (model is ChangeOrder)
            {
                if (model.ProcessStatus == EnumTfgProcessStatus.WaitIssue)
                    throw new CustomException(500, "订单已支付不能重复处理");
                if (money == 0)
                {
                    model.ProcessStatus = EnumTfgProcessStatus.WaitIssue;
                }
                else
                    model.ProcessStatus = EnumTfgProcessStatus.ProcessingWaitPay;
                model.Money = money;
            }
            else if (model is BounceOrder || model is AnnulOrder)
            {
                if (model.ProcessStatus == EnumTfgProcessStatus.Refunding)
                    throw new CustomException(500, "订单处于退款中，不能处理");
                /*
                 
                if (model.ProcessStatus == EnumTfgProcessStatus.ProcessingWaitRefund)
                    model.Order.RefundedTradeMoney += model.Money;
                else
                 */
                if (model.ProcessStatus != EnumTfgProcessStatus.ProcessingWaitRefund)
                    model.ProcessStatus = EnumTfgProcessStatus.ProcessingWaitRefund;
                //退款金额
                model.Money = list.Sum(p => p.Passenger.PayMoney) - money;
                /*
                 重新处理，清空数据
                 */
                if (model is BounceOrder)
                {
                    (model as BounceOrder).BounceLines.Clear();
                }
                else if (model is AnnulOrder)
                {
                    (model as AnnulOrder).BounceLines.Clear();
                }

                if (model.Order.CreateTime < DateTime.Parse("2014/07/19 04:10:00"))
                    OldGetBouncedetail(model);
                else
                    GetBouncedetail(model);
            }
            else if (model is ModifyOrder)
            {
                model.ProcessStatus = EnumTfgProcessStatus.Processed;
                model.Passenger.ForEach(p => p.Status = EnumTfgPassengerStatus.Modified);
            }
            model.Remark = remark;
            model.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = "订单已处理",
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }
        private void OldGetBouncedetail(AfterSaleOrder model)
        {
            #region
            //生成退款明细
            List<BounceLine> bllist = new List<BounceLine>();
            //退款，按乘机人
            foreach (var item in model.Passenger.ToList())
            {

                decimal refundMoney = item.RetirementMoney;//该乘机人须退金额
                /*
                 * 订单中退款
                 */
                if (refundMoney > 0)
                {
                    BounceLine bounceLine = new BounceLine()
                    {
                        ID = RefundNum(),
                        PayMethod = model.Order.OrderPay.PayMethod.Value,
                        PassgenerName = item.Passenger.PassengerName,
                        OrderID = model.OrderID,
                        PaySerialNumber = model.Order.OrderPay.PaySerialNumber,
                        RefundMoney = refundMoney,
                        Status = EnumBoundRefundStatus.NoRefund
                    };
                    if (model.Order.OrderPay.PayBillDetails != null && model.Order.OrderPay.PayBillDetails.Count > 0)
                    {
                        //支付明细
                        var payBillList = model.Order.OrderPay.PayBillDetails.Select(p => new { OpType = p.OpType, Money = p.Money }).ToList();
                        int passCount = model.Order.Passengers.Count;
                        //运营商分润
                        var carrierFR = payBillList.Where(p => p.OpType == EnumOperationType.Profit).Select(p => p.Money).FirstOrDefault();
                        //运营商服务费
                        var carrierFW = payBillList.Where(p => p.OpType == EnumOperationType.Receivables).Select(p => p.Money).FirstOrDefault();

                        if ((carrierFR + carrierFW) != 0)
                        {
                            var carrierCashBagCode = this.businessmanRepository.FindAll(p => p.Code == model.Order.CarrierCode).OfType<Carrier>().Select(p => p.CashbagCode).FirstOrDefault();
                            string args = string.Format("{0}^{1}^分润退款", carrierCashBagCode, (carrierFR + carrierFW) / passCount);
                            bounceLine.BusArgs = args;
                        }
                    }
                    bllist.Add(bounceLine);
                }

            }
            #endregion
            if (model is BounceOrder)
                bllist.ForEach(p => (model as BounceOrder).BounceLines.Add(p));
            else if (model is AnnulOrder)
                bllist.ForEach(p => (model as AnnulOrder).BounceLines.Add(p));
        }
        private void GetBouncedetail(AfterSaleOrder model)
        {
            #region
            //生成退款明细
            List<BounceLine> bllist = new List<BounceLine>();
            bool _notLocal = model.Order.Policy.PolicySourceType == EnumPolicySourceType.Local ? false : true;
            //退款，按乘机人
            var _pList = model.Passenger.ToList();
            int aduiltCount = model.Order.Passengers.Where(p => p.PassengerType != EnumPassengerType.Baby).Count();//成人个人
            int babyCount = model.Order.Passengers.Where(p => p.PassengerType == EnumPassengerType.Baby).Count();//婴儿个数
            foreach (var item in _pList)
            {
                decimal refundMoney = item.RetirementMoney;//该乘机人须退金额

                int passCount = item.Passenger.PassengerType == EnumPassengerType.Baby ? babyCount : aduiltCount;
                bool _baby = item.Passenger.PassengerType == EnumPassengerType.Baby ? true : false;
                /*
                 * 订单中退款
                 */
                if (refundMoney > 0)
                {
                    BounceLine bounceLine = new BounceLine()
                    {
                        ID = RefundNum(),
                        PayMethod = model.Order.OrderPay.PayMethod.Value,
                        PassgenerName = item.Passenger.PassengerName,
                        OrderID = model.OrderID,
                        PaySerialNumber = model.Order.OrderPay.PaySerialNumber,
                        RefundMoney = refundMoney,
                        Status = EnumBoundRefundStatus.NoRefund
                    };

                    if (model.Order.OrderPay.PayBillDetails != null && model.Order.OrderPay.PayBillDetails.Count > 0)
                    {
                        //支付明细
                        var payBillList = model.Order.OrderPay.PayBillDetails.Where(p => p.OpType != EnumOperationType.PayMoney && p.OpType != EnumOperationType.Insurance && p.OpType != EnumOperationType.InsuranceServer).Select(p => new { OpType = p.OpType, Money = p.Money, CashBagCode = p.CashbagCode, BabyMoney = p.InfMoney }).ToList();

                        StringBuilder args = new StringBuilder();//退款分润格式

                        //系统合作者退款
                        var _partner = payBillList.Where(x => x.OpType == EnumOperationType.ParterServer).FirstOrDefault();
                        //系统合作者分润退款
                        var _partnerProfit = payBillList.Where(x => x.OpType == EnumOperationType.ParterProfitServer).FirstOrDefault();
                        //运营商退款
                        var _carrierRecv = payBillList.Where(x => x.OpType == EnumOperationType.CarrierRecvServer).FirstOrDefault();
                        //运营商补入
                        var _carrierPay = payBillList.Where(x => x.OpType == EnumOperationType.CarrierPayServer).FirstOrDefault();
                        //运营商分润退款
                        var _carrierPayProfit = payBillList.Where(x => x.OpType == EnumOperationType.CarrierPayProfitServer).FirstOrDefault();
                        //出票服务费退款
                        var _issuePay = payBillList.Where(p => p.OpType == EnumOperationType.IssuePayServer).FirstOrDefault();
                        //分润退款
                        var _profit = payBillList.Where(p => p.OpType == EnumOperationType.Profit).FirstOrDefault();
                        //收款方退款
                        var _rece = payBillList.Where(p => p.OpType == EnumOperationType.Receivables).FirstOrDefault();
                        //多余金额ParterRetain
                        var _part = payBillList.Where(p => p.OpType == EnumOperationType.ParterRetainServer).FirstOrDefault();



                        decimal _refundmoney = 0;
                        //分润全退  
                        if (_profit != null && _profit.Money != 0)
                        {
                            decimal _profitmoney = databill.NewRound(((_baby ? _profit.BabyMoney : _profit.Money - _profit.BabyMoney) / passCount), 2);
                            args.AppendFormat("{0}^{1}^分润退款|", _profit.CashBagCode, _profitmoney);
                            _refundmoney += _profitmoney;
                        }
                        if (!_baby && _part != null && _part.Money != 0)
                        {
                            decimal blmoney = databill.NewRound(_part.Money / passCount, 2);
                            args.AppendFormat("{0}^{1}^保留|", _part.CashBagCode, blmoney);
                            _refundmoney += blmoney;

                        }
                        decimal refundPer = (refundMoney - _refundmoney) / (item.Passenger.PayMoney - _refundmoney);//退款支付金额百分比
                        if (_partner != null && _partner.Money != 0)
                            args.AppendFormat("{0}^{1}^系统退款|", _partner.CashBagCode, databill.NewRound(((_baby ? _partner.BabyMoney : _partner.Money - _partner.BabyMoney) / passCount) * refundPer, 2));//合作者服务费

                        if (_issuePay != null && _issuePay.Money != 0)
                            args.AppendFormat("{0}^{1}^出票服务费退款|", _issuePay.CashBagCode, databill.NewRound(((_baby ? _issuePay.BabyMoney : _issuePay.Money - _issuePay.BabyMoney) / passCount) * refundPer, 2));//出票服务费退款

                        if (_partnerProfit != null && _partnerProfit.Money != 0)
                            args.AppendFormat("{0}^{1}^系统分润手续费退款|", _partnerProfit.CashBagCode, _notLocal ? databill.CeilAngle(((_baby ? _partnerProfit.BabyMoney : _partnerProfit.Money - _partnerProfit.BabyMoney) / passCount) * refundPer) : databill.NewRound(((_baby ? _partnerProfit.BabyMoney : _partnerProfit.Money - _partnerProfit.BabyMoney) / passCount) * refundPer, 2));//分润服务费退款

                        if (_carrierRecv != null && _carrierRecv.Money != 0)
                            args.AppendFormat("{0}^{1}^运营商退款|", _carrierRecv.CashBagCode, databill.NewRound(((_baby ? _carrierRecv.BabyMoney : _carrierRecv.Money - _carrierRecv.BabyMoney) / passCount) * refundPer, 2));//运营商退款

                        if (_carrierPay != null && _carrierPay.Money != 0)
                            args.AppendFormat("{0}^{1}^运营商退款收入|", _carrierPay.CashBagCode, databill.NewRound(((_baby ? _carrierPay.BabyMoney : _carrierPay.Money - _carrierPay.BabyMoney) / passCount) * refundPer, 2));

                        if (_carrierPayProfit != null && _carrierPayProfit.Money != 0)
                            args.AppendFormat("{0}^{1}^运营商分润退款|", _carrierPayProfit.CashBagCode, _notLocal ? databill.CeilAngle(((_baby ? _carrierPayProfit.BabyMoney : _carrierPayProfit.Money - _carrierPayProfit.BabyMoney) / passCount) * refundPer) : databill.NewRound(((_baby ? _carrierPayProfit.BabyMoney : _carrierPayProfit.Money - _carrierPayProfit.BabyMoney) / passCount) * refundPer, 2));

                        args.AppendFormat("{0}^{1}^收款方退款|", _rece.CashBagCode, item.RetirementMoney - _refundmoney);

                        bounceLine.BusArgs = args.ToString().Trim('|');
                    }
                    bllist.Add(bounceLine);
                }

            }
            #endregion
            if (model is BounceOrder)
                bllist.ForEach(p => (model as BounceOrder).BounceLines.Add(p));
            else if (model is AnnulOrder)
                bllist.ForEach(p => (model as AnnulOrder).BounceLines.Add(p));
        }
        private string RefundNum()
        {
            Random rd = new Random(Guid.NewGuid().GetHashCode());
            return DateTime.Now.ToString("yyyyMMddHHmmssfff") + rd.Next(1000, 9999);

        }
        [ExtOperationInterceptor("锁定售后订单")]
        public void LockAccount(int saleorderid)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该订单售后信息不存在");
            model.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = "订单已锁定,等待处理",
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            model.LockCurrentAccount = AuthManager.GetCurrentUser().OperatorAccount;
            if (model.ProcessStatus == EnumTfgProcessStatus.UnProcess)
            {
                model.ProcessName = AuthManager.GetCurrentUser().OperatorAccount;
                model.ProcessDate = DateTime.Now;
                model.ProcessStatus = EnumTfgProcessStatus.Processing;
            }
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("解锁售后订单")]
        public void UnlockAccount(int saleorderid)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该订单售后信息不存在");
            if (model.LockCurrentAccount != AuthManager.GetCurrentUser().OperatorAccount)
                throw new CustomException(404, "不是自己锁定的订单不能解锁");
            model.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = "订单已解锁",
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            model.LockCurrentAccount = string.Empty;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }

        //[ExtOperationInterceptor("获取售后订单列表")]
        public DataPack<ResponseAfterSaleOrder> GetAfterSaleOrderByPager(string LockAccount, DateTime? StartCreateTime, DateTime? EndDateTime, string PaySerialNumber, int currentPageIndex, int pageSize, string pnr, string code, string policyFrom, string orderid, EnumAfterSaleOrder? saleOrderType, EnumTfgProcessStatus? status, bool? InterfaceOrder, bool? ShareOrder, bool? localOrder, string passengerName = "", bool? isInsuranceRefund = null)
        {
            //p => p.Order.Policy.PolicySourceType == EnumPolicySourceType.Interface
            var query = afterSaleOrderRepository.FindAll();
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(pnr.Trim()))
                query = query.Where(p => p.Order.PnrCode == pnr.Trim());
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(code.Trim()))
                query = query.Where(p => p.Order.BusinessmanCode == code.Trim());
            if (!string.IsNullOrEmpty(policyFrom) && !string.IsNullOrEmpty(policyFrom.Trim()))
                query = query.Where(p => p.Order.Policy.PlatformCode == policyFrom.Trim());
            if (!string.IsNullOrEmpty(orderid) && !string.IsNullOrEmpty(orderid.Trim()))
                query = query.Where(p => p.OrderID == orderid.Trim());
            if (!string.IsNullOrEmpty(PaySerialNumber) && !string.IsNullOrEmpty(PaySerialNumber.Trim()))
                query = query.Where(p => p.Order.OrderPay.PaySerialNumber == PaySerialNumber.Trim());
            if (!string.IsNullOrEmpty(passengerName) && !string.IsNullOrEmpty(passengerName.Trim()))
                query = query.Where(p => p.Passenger.Where(t => t.Passenger.PassengerName.Contains(passengerName.Trim())).Count() > 0);
            if (!string.IsNullOrEmpty(LockAccount) && !string.IsNullOrEmpty(LockAccount.Trim()))
                query = query.Where(p => p.LockCurrentAccount.Contains(LockAccount.Trim()));

            IList<IQueryable<AfterSaleOrder>> list = new List<IQueryable<AfterSaleOrder>>();

            if (ShareOrder.HasValue && ShareOrder.Value) list.Add(query.Where(q => q.Order.Policy.PolicySourceType == EnumPolicySourceType.Share));
            if (InterfaceOrder.HasValue && InterfaceOrder.Value) list.Add(query.Where(q => q.Order.Policy.PolicySourceType == EnumPolicySourceType.Interface));
            if (localOrder.HasValue && localOrder.Value) list.Add(query.Where(q => q.Order.Policy.PolicySourceType == EnumPolicySourceType.Local));
            IQueryable<AfterSaleOrder> result = null;

            foreach (var record in list)
            {
                if (result == null)
                {
                    result = record;
                }
                else
                {
                    result = result.Union(record);
                }
            }
            if (result != null)
            {
                query = query.Intersect(result);
            }

            if (StartCreateTime.HasValue)
                query = query.Where(p => p.CreateTime > StartCreateTime);
            if (EndDateTime.HasValue)
                query = query.Where(p => p.CreateTime <= EndDateTime);
            //if (ShareOrder && !InterfaceOrder && !localOrder)
            //    query = query.Where(p => p.Order.Policy.PolicySourceType == EnumPolicySourceType.Share);
            //if (InterfaceOrder && !ShareOrder && !localOrder)
            //    query = query.Where(p => p.Order.Policy.PolicySourceType == EnumPolicySourceType.Interface);
            //if (!InterfaceOrder && !ShareOrder && localOrder)
            //    query = query.Where(p => p.Order.Policy.PolicySourceType == EnumPolicySourceType.Local);   
            ////if()

            //if(ShareOrder && InterfaceOrder && localOrder)
            //    query = query.Where(p => p.Order.Policy.PolicySourceType == EnumPolicySourceType.Interface || p.Order.Policy.PolicySourceType == EnumPolicySourceType.Share || p.Order.Policy.PolicySourceType == EnumPolicySourceType.Local);
            if (saleOrderType.HasValue)
                switch (saleOrderType.Value)
                {
                    case EnumAfterSaleOrder.Annul:
                        query = query.Where(p => p is AnnulOrder);
                        break;
                    case EnumAfterSaleOrder.Bounce:
                        query = query.Where(p => p is BounceOrder);
                        break;
                    case EnumAfterSaleOrder.Change:
                        query = query.Where(p => p is ChangeOrder);
                        break;
                    case EnumAfterSaleOrder.Modify:
                        query = query.Where(p => p is ModifyOrder);
                        break;
                    default:
                        break;
                }
            if (status.HasValue)
                query = query.Where(p => p.ProcessStatus == status.Value);
            query = query.OrderByDescending(p => p.CreateTime);
            //如果是
            if (isInsuranceRefund != null)
            {
                if (isInsuranceRefund.Value)
                {
                    //如果是退票，则必须是自愿的，或者是废票，并且，必须购买了急速退
                    query = query.Where(p =>
                        (
                        ((p is BounceOrder) && (p as BounceOrder).IsVoluntary)
                        || (p is AnnulOrder)
                        )
                        && p.Passenger.All(m => m.Passenger.IsInsuranceRefund));
                }
                else
                {
                    query = query.Where(p => !(
                        (
                        ((p is BounceOrder) && (p as BounceOrder).IsVoluntary)
                        || (p is AnnulOrder)
                        )
                        && p.Passenger.All(m => m.Passenger.IsInsuranceRefund)));
                }
            }
            List<ResponseAfterSaleOrder> listafterorder = new List<ResponseAfterSaleOrder>();

            if (pageSize == 10)
            {
                query.Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList().AsParallel().ForEach(p => listafterorder.Add(AutoMapper.Mapper.Map<AfterSaleOrder, ResponseAfterSaleOrder>(p)));
            }
            else
            {

                //导出用
                var afterSaleOrderList = query.Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList();
                int count = 100;
                decimal pageIndex = Math.Floor((decimal)afterSaleOrderList.Count / count);
                for (decimal i = 0; i < pageIndex; i++)
                {
                    var tempList = afterSaleOrderList.Skip(Convert.ToInt32(i * count)).Take(count).ToList();
                    listafterorder.AddRange(AutoMapper.Mapper.Map<List<AfterSaleOrder>, List<ResponseAfterSaleOrder>>(tempList));
                }
            }
            var dataPack = new DataPack<ResponseAfterSaleOrder>
            {
                TotalCount = query.Count(),
                List = listafterorder
            };
            return dataPack;
        }

        //[ExtOperationInterceptor("获当前运营取售后订单列表")]
        public DataPack<ResponseAfterSaleOrder> GetCarrierAfterSaleOrder(string PaySerialNumber, int currentPageIndex, int pageSize, string pnr, string code, string policyFrom, string orderid, EnumAfterSaleOrder? saleOrderType, EnumTfgProcessStatus? status, string passengerName = "")
        {
            var currentUser = AuthManager.GetCurrentUser();
            IQueryable<AfterSaleOrder> query = null;
            if (currentUser.Type == "Carrier")
            {
                query = afterSaleOrderRepository.FindAllNoTracking(p => p.Order.CarrierCode == currentUser.Code || p.Order.Policy.Code == currentUser.Code || p.Order.Policy.CarrierCode == currentUser.Code);
            }
            else
                query = afterSaleOrderRepository.FindAllNoTracking(p => p.Order.Policy.Code == currentUser.Code);
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(pnr.Trim()))
                query = query.Where(p => p.Order.PnrCode == pnr.Trim());
            if (!string.IsNullOrEmpty(code) && !string.IsNullOrEmpty(code.Trim()))
                query = query.Where(p => p.Order.BusinessmanCode == code.Trim());
            if (!string.IsNullOrEmpty(policyFrom) && !string.IsNullOrEmpty(policyFrom.Trim()))
                query = query.Where(p => p.Order.Policy.PlatformCode == policyFrom.Trim());
            if (!string.IsNullOrEmpty(orderid) && !string.IsNullOrEmpty(orderid.Trim()))
                query = query.Where(p => p.OrderID == orderid.Trim());
            if (!string.IsNullOrEmpty(PaySerialNumber) && !string.IsNullOrEmpty(PaySerialNumber.Trim()))
                query = query.Where(p => p.Order.OrderPay.PaySerialNumber == PaySerialNumber.Trim());
            if (!string.IsNullOrEmpty(passengerName) && !string.IsNullOrEmpty(passengerName.Trim()))
                query = query.Where(p => p.Passenger.Where(t => t.Passenger.PassengerName.Contains(passengerName.Trim())).Count() > 0);
            if (saleOrderType.HasValue)
                switch (saleOrderType.Value)
                {
                    case EnumAfterSaleOrder.Annul:
                        query = query.Where(p => p is AnnulOrder);
                        break;
                    case EnumAfterSaleOrder.Bounce:
                        query = query.Where(p => p is BounceOrder);
                        break;
                    case EnumAfterSaleOrder.Change:
                        query = query.Where(p => p is ChangeOrder);
                        break;
                    case EnumAfterSaleOrder.Modify:
                        query = query.Where(p => p is ModifyOrder);
                        break;
                    default:
                        break;
                }
            if (status.HasValue)
                query = query.Where(p => p.ProcessStatus == status.Value);
            query = query.OrderByDescending(p => p.CreateTime);
            var afterSaleOrderList = query.Skip((currentPageIndex - 1) * pageSize).Take(pageSize).ToList();

            var list = AutoMapper.Mapper.Map<List<AfterSaleOrder>, List<ResponseAfterSaleOrder>>(afterSaleOrderList);
            list.ForEach(x => x.CurrentCode = currentUser.Code);

            var dataPack = new DataPack<ResponseAfterSaleOrder>
            {
                TotalCount = query.Count(),
                List = list
            };
            return dataPack;
        }

        [ExtOperationInterceptor("拒绝处理")]
        public void UnProcess(int afterorderid, string unReason)
        {

            var model = afterSaleOrderRepository.FindAll(p => p.Id == afterorderid).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该订单售后信息不存在");
            IsLock(model.LockCurrentAccount);
            if ((model is AnnulOrder || model is BounceOrder) && model.ProcessStatus == EnumTfgProcessStatus.ProcessingWaitRefund)
            {
                /*
                 拒绝处理，清空数据,退，废明细
                 */
                if (model is BounceOrder)
                {
                    (model as BounceOrder).BounceLines.Clear();
                }
                else if (model is AnnulOrder)
                {
                    (model as AnnulOrder).BounceLines.Clear();
                }
            }
            else if (model is ChangeOrder)
            {
                /*
                 * 改签如果支付了，退款
                 */
                if (model.ProcessStatus == EnumTfgProcessStatus.WaitIssue && model.Money != 0)
                {
                    ChangeOrder changeOrder = model as ChangeOrder;
                    string args = this.PayArgs(changeOrder).ToString();
                    IPaymentClientProxy client = new CashbagPaymentClientProxy();
                    var businessman = this.businessmanRepository.FindAll(x => x.Code == changeOrder.Order.BusinessmanCode).FirstOrDefault();
                    client.Reimburse(businessman.CashbagCode, businessman.CashbagKey, changeOrder.OutPayNo, changeOrder.Money, changeOrder.Id.ToString(), changeOrder.Id.ToString(), args);
                }
            }
            model.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = string.Format("订单拒绝处理,拒绝理由:{0}", unReason),
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            model.Passenger.ForEach(p => p.Status = EnumTfgPassengerStatus.RepelProcess);

            model.Remark = unReason;
            model.ProcessStatus = EnumTfgProcessStatus.RepelProcess;
            model.CompletedTime = DateTime.Now;
            model.LockCurrentAccount = string.Empty;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }
        /// <summary>
        /// 银行卡支付平台退款
        /// </summary>         
        private EnumBoundRefundStatus ChangeOrderRefund(IPaymentClientProxy client, Businessman businessman, ChangeOrder changeOrder, decimal refundMoney, string refundid, string mainrefundid)
        {
            if (changeOrder.PayStatus == EnumChangePayStatus.Payed)
            {
                if (changeOrder.PayWay == EnumPayMethod.Bank || changeOrder.PayWay == EnumPayMethod.Platform)
                {
                    client.Reimburse(businessman.CashbagCode, businessman.CashbagKey, changeOrder.OutPayNo, refundMoney, refundid, mainrefundid);
                    return EnumBoundRefundStatus.Refunding;
                }
                else
                {
                    client.Reimburse(businessman.CashbagCode, businessman.CashbagKey, changeOrder.OutPayNo, refundMoney, refundid, mainrefundid);
                    changeOrder.RefundMoney += refundMoney;
                    changeOrder.Passenger.ForEach(p => p.Passenger.TicketStatus = EnumTicketStatus.RefoundTicketed);
                    return EnumBoundRefundStatus.Refunded;
                }
            }
            throw new CustomException(500, "改签未支付完成,不能退款");
        }

        [ExtOperationInterceptor("售后订单退款")]
        public void SaleOrderRefund(int saleorderid)
        {

            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该订单售后信息不存在");
            IsLock(model.LockCurrentAccount);
            var businessman = this.businessmanRepository.FindAll(x => x.Code == model.Order.BusinessmanCode).FirstOrDefault();
            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            if (model is ChangeOrder)
                throw new CustomException(500, "该订单无退款信息");
            if (model.ProcessStatus == EnumTfgProcessStatus.Refunding || model.ProcessStatus == EnumTfgProcessStatus.Processed)
                throw new CustomException(500, "该订单正在处理中...");
            model.ProcessStatus = EnumTfgProcessStatus.Refunding;
            List<BounceLine> refundList = new List<BounceLine>();
            if (model is BounceOrder)
                (model as BounceOrder).BounceLines.ForEach(p => refundList.Add(p));
            else if (model is AnnulOrder)
                (model as AnnulOrder).BounceLines.ForEach(p => refundList.Add(p));
            if (refundList == null && refundList.Count < 1)
                throw new CustomException(500, "无退款数据!");

            StringBuilder exceptionMsg = new StringBuilder();
            refundList.ForEach(p =>
            {
                string remark = p.ChangeOrderID.HasValue ? model.Id.ToString() : string.Format("SaleOrderRefund_{0}", model.Id.ToString());
                if (p.PayMethod == EnumPayMethod.Account || p.PayMethod == EnumPayMethod.Credit)
                {
                    try
                    {
                        client.Reimburse(businessman.CashbagCode, businessman.CashbagKey, p.PaySerialNumber, p.RefundMoney, p.ID, remark, p.BusArgs, string.Format("【{2}】 PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray()), (model is BounceOrder) ? "退票" : "废票"));
                        p.Status = EnumBoundRefundStatus.Refunded;
                        p.RefundTime = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        exceptionMsg.AppendFormat("发生异常退款单号:{0},消息:{1}", p.ID, e.Message);
                        Logger.WriteLog(LogType.ERROR, string.Format("发生异常退款单号:{0},消息:{1}", p.ID, e.Message));
                    }

                }
                else
                {
                    try
                    {
                        client.Reimburse(businessman.CashbagCode, businessman.CashbagKey, p.PaySerialNumber, p.RefundMoney, p.ID, remark, p.BusArgs, string.Format("【{2}】 PNR:{0},乘机人:【{1}】", model.Order.PnrCode, string.Join("|", model.Passenger.Select(x => x.Passenger.PassengerName).ToArray()), (model is BounceOrder) ? "退票" : "废票"));
                        p.Status = EnumBoundRefundStatus.Refunding;
                        p.RefundTime = DateTime.Now;
                    }
                    catch (Exception e)
                    {
                        exceptionMsg.AppendFormat("发生异常退款单号:{0},消息:{1}", p.ID, e.Message);
                        Logger.WriteLog(LogType.ERROR, string.Format("发生异常退款单号:{0},消息:{1}", p.ID, e.Message));
                    }

                }
            });
            if (refundList.All(p => p.Status == EnumBoundRefundStatus.Refunded))
            {
                if (model is AnnulOrder)
                    model.Passenger.ForEach(p => p.Status = EnumTfgPassengerStatus.AnnulTicketed);
                else if (model is BounceOrder)
                    model.Passenger.ForEach(p => p.Status = EnumTfgPassengerStatus.Refunded);
                model.ProcessStatus = EnumTfgProcessStatus.Processed;
                model.LockCurrentAccount = string.Empty;
                model.CompletedTime = DateTime.Now;
            }

            model.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = string.Format("售后订单{0}", model.ProcessStatus == EnumTfgProcessStatus.Processed ? "完成,交易完成" : "退款中"),
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
            if (model.ProcessStatus == EnumTfgProcessStatus.Processed)
                MessageQueueManager.SendMessage(model.Id.ToString(), 1);
            if (exceptionMsg.Length > 0)
                throw new CustomException(500, exceptionMsg.ToString());
        }
        private void IsLock(string lockCurrentName)
        {
            if (!string.Equals(lockCurrentName, AuthManager.GetCurrentUser().OperatorAccount))
                throw new CustomException(503, "禁止操作，该售后订单已被锁定");
        }

        //[ExtOperationInterceptor("售后订单完成")]
        public void Completed(int saleorderid, Dictionary<int, string> dic)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid).OfType<ChangeOrder>().FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该订单售后信息不存在");
            IsLock(model.LockCurrentAccount);
            if (model.ProcessStatus != EnumTfgProcessStatus.WaitIssue)
                throw new CustomException(500, "该售后订单未支付，不能继续操作");
            model.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = "订单处理完成,交易结束",
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            if (dic != null)
            {
                model.Passenger.ForEach(p =>
                {
                    p.Status = EnumTfgPassengerStatus.ChangeTicketed;
                    if (dic.ContainsKey(p.Id))
                        p.AfterSaleTravelTicketNum = dic.Where(m => m.Key == p.Id).FirstOrDefault().Value;
                });
            }
            model.ProcessStatus = EnumTfgProcessStatus.Processed;
            model.CompletedTime = DateTime.Now;
            model.LockCurrentAccount = string.Empty;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
            MessageQueueManager.SendMessage(model.Id.ToString(), 1);
        }

        //[ExtOperationInterceptor("详细售后订单")]
        public ResponseAfterSaleOrder AfterSaleOrderDetail(int saleOrderid)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleOrderid).FirstOrDefault();
            if (model == null)
                throw new CustomException(404, "该订单售后信息不存在");
            return model.ToResponseAfterSaleOrder();
        }



        #region 新机票报表++++
        private IQueryable<Ticket_Conso> QueryWhere(TicketQueryEntity ticketQueryEntity)
        {
            string TicketState = string.Empty;
            switch (ticketQueryEntity.ticketStatus)
            {

                case 0:
                    TicketState = "出票";
                    break;
                case 2:
                    TicketState = "退票";
                    break;
                case 3:
                    TicketState = "改签";
                    break;
                case 5:
                    TicketState = "废票";
                    break;
                default:
                    TicketState = "";
                    break;
            }
            DateTime startTime = DateTime.Now.AddDays(-7);
            var query = ticketRepository.FindAllNoTracking().OfType<Ticket_Conso>();

            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.orderId))
                query = query.Where(p => p.OrderID == ticketQueryEntity.orderId.Trim());
            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.pnr))
                query = query.Where(p => p.PNR == ticketQueryEntity.pnr.Trim());
            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.ticketNumber))
                query = query.Where(p => p.TicketNum == ticketQueryEntity.ticketNumber.Trim());
            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.platformCode))
                query = query.Where(p => p.PolicyFrom == ticketQueryEntity.platformCode.Trim());
            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.carrayCode))
                query = query.Where(p => p.CarryCode == ticketQueryEntity.carrayCode.Trim());
            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.fromCityCode))
                query = query.Where(p => p.Voyage.Contains(ticketQueryEntity.fromCityCode.Trim() + "-"));
            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.toCityCode))
                query = query.Where(p => p.Voyage.Contains("-" + ticketQueryEntity.toCityCode.Trim()));
            if (ticketQueryEntity.ticketStatus.HasValue)
                query = query.Where(p => p.TicketState == TicketState);
            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.businessmanCode))
                query = query.Where(p => p.Code == ticketQueryEntity.businessmanCode.Trim());
            if (ticketQueryEntity.startIssueRefundTime.HasValue)
            {
                query = query.Where(p => p.CreateDate >= ticketQueryEntity.startIssueRefundTime.Value);
            }
            else
            {
                query = query.Where(p => p.CreateDate >= startTime);
            }
            if (ticketQueryEntity.endIssueRefundTime.HasValue)
                query = query.Where(p => p.CreateDate <= ticketQueryEntity.endIssueRefundTime.Value);
            if (!string.IsNullOrWhiteSpace(ticketQueryEntity.carrierCode))
                query = query.Where(p => p.CarrierCode == ticketQueryEntity.carrierCode || p.IssueTicketCode == ticketQueryEntity.carrierCode);

            return query;
        }

        //[ExtOperationInterceptor("机票汇总")]
        public List<TicketInformationSummaryDto> GetTicketSumSummary(TicketQueryEntity ticketQueryEntity)
        {
            var list = QueryWhere(ticketQueryEntity).ToList().AsParallel();
            List<TicketInformationSummaryDto> listSummaryDto = new List<TicketInformationSummaryDto>();
            TicketInformationSummaryDto ticketSummaryDto = null;
            foreach (var item in list.ToLookup(t => t.TicketState))
            {
                ticketSummaryDto = new TicketInformationSummaryDto();
                ticketSummaryDto.TicketType = item.Key;
                ticketSummaryDto.TicketCount = item.Count();
                ticketSummaryDto.TicketPrice = item.Sum(tt => tt.PMFee);
                ticketSummaryDto.TaxFee = item.Sum(xx => xx.ABFee + xx.RQFee);
                ticketSummaryDto.Commission = item.Sum(tt => tt.PMFee - tt.OrderMoney);
                ticketSummaryDto.ShouldMoney = item.Sum(tt => tt.OrderMoney);
                ticketSummaryDto.PaidMoney = item.Sum(xx => xx.PaidMoney);
                ticketSummaryDto.PaidMoneyByAlipay = item.Sum(xx => xx.PaidMoney);
                ticketSummaryDto.PaidMoney = item.Sum(xx => xx.PaidMoney);
                ticketSummaryDto.RealMoney = item.Sum(xx => xx.Money);
                listSummaryDto.Add(ticketSummaryDto);

            }
            return listSummaryDto.OrderByDescending(t => t.TicketCount).ToList();

        }

        //[ExtOperationInterceptor("机票销售统计")]
        public TicketSalesStatisticsDto GetTicketSumSales(TicketQueryEntity ticketQueryEntity)
        {
            #region
            var list = QueryWhere(ticketQueryEntity).ToList().AsParallel();
            TicketSalesStatisticsDto ticketSalesStatisticsDto = new TicketSalesStatisticsDto();
            if (ticketSalesStatisticsDto.PolicyCodeDic == null)
                ticketSalesStatisticsDto.PolicyCodeDic = new List<TicketSalesStatistics>();
            if (list.Count() > 0)
            {
                foreach (var p in list.ToLookup(g => g.PolicyFrom).AsParallel())
                {
                    TicketSalesStatistics ticketSale = new TicketSalesStatistics();
                    ticketSale.PolicyCode = p.Key;
                    //出
                    ticketSale.IssueTicketCount = p.Where(t => t.TicketState == "出票").Count();
                    ticketSale.IssueTicketPrice = p.Where(t => t.TicketState == "出票").Sum(pp => pp.PMFee);
                    ticketSale.IssueTicketTaxFee = p.Where(t => t.TicketState == "出票").Sum(pp => pp.ABFee + pp.RQFee);
                    ticketSale.IssueTicketCommission = p.Where(t => t.TicketState == "出票").Sum(pp => pp.PMFee - pp.OrderMoney);
                    ticketSale.IssueCreditShouldMoney = p.Where(t => t.TicketState == "出票").Sum(pp => pp.Paymethod == "信用账户" ? pp.OrderMoney : 0);
                    ticketSale.IssueAccountShouldMoney = p.Where(t => t.TicketState == "出票").Sum(pp => pp.Paymethod == "现金账户" ? pp.OrderMoney : 0);
                    ticketSale.IssuePlatFormShouldMoney = p.Where(t => t.TicketState == "出票").Sum(pp => (pp.Paymethod != "信用账户" && pp.Paymethod != "现金账户") ? pp.OrderMoney : 0);
                    ticketSale.IssueTicketShouldMoney = p.Where(t => t.TicketState == "出票").Sum(pp => pp.OrderMoney);

                    ////退
                    ticketSale.RefundTicketCount = p.Where(t => t.TicketState == "退票").Count();
                    ticketSale.RefundTicketPrice = p.Where(t => t.TicketState == "退票").Sum(pp => pp.PMFee);
                    ticketSale.RefundTicketTaxFee = p.Where(t => t.TicketState == "退票").Sum(pp => pp.ABFee + pp.RQFee);
                    ticketSale.RefundTicketCommission = p.Where(t => t.TicketState == "退票").Sum(pp => pp.PMFee - pp.OrderMoney);
                    ticketSale.RefundTicketShouldMoney = p.Where(t => t.TicketState == "退票").Sum(pp => pp.OrderMoney);
                    ticketSale.RefundTicketRealMoney = p.Where(t => t.TicketState == "退票").Sum(pp => pp.OrderMoney);
                    //////废
                    ticketSale.InvalidTicketCount = p.Where(t => t.TicketState == "废票").Count();
                    ticketSale.InvalidTicketPrice = p.Where(t => t.TicketState == "废票").Sum(pp => pp.PMFee);
                    ticketSale.InvalidTicketTaxFee = p.Where(t => t.TicketState == "废票").Sum(pp => pp.ABFee + pp.RQFee);
                    ticketSale.InvalidTicketCommission = p.Where(t => t.TicketState == "废票").Sum(pp => pp.PMFee - pp.OrderMoney);
                    ticketSale.InvalidTicketShouldMoney = p.Where(t => t.TicketState == "废票").Sum(pp => pp.OrderMoney);
                    ticketSale.InvalidTicketRealMoney = p.Where(t => t.TicketState == "废票").Sum(pp => pp.OrderMoney);
                    if (ticketSale.CarrayCodeDic == null)
                        ticketSale.CarrayCodeDic = new List<TicketSalesCarrayCode>();
                    var secondList = p.Where(x => x.PolicyFrom == p.Key).ToLookup(xx => xx.CarryCode).AsParallel();
                    foreach (var pp in secondList)
                    {
                        TicketSalesCarrayCode ticketSalesCarrayCode = new TicketSalesCarrayCode();
                        ticketSalesCarrayCode.CarrayCode = pp.Key;
                        ticketSalesCarrayCode.IssueTicketCount = pp.Where(t => t.TicketState == "出票").Count();

                        ticketSalesCarrayCode.IssueTicketPrice = pp.Where(t => t.TicketState == "出票").Sum(tt => tt.PMFee);
                        ticketSalesCarrayCode.IssueTicketTaxFee = pp.Where(t => t.TicketState == "出票").Sum(tt => tt.ABFee + tt.RQFee);
                        ticketSalesCarrayCode.IssueTicketCommission = pp.Where(t => t.TicketState == "出票").Sum(tt => tt.PMFee - tt.OrderMoney);
                        ticketSalesCarrayCode.IssueCreditShouldMoney = pp.Where(t => t.TicketState == "出票").Sum(tt => tt.Paymethod == "信用账户" ? tt.OrderMoney : 0);
                        ticketSalesCarrayCode.IssueAccountShouldMoney = pp.Where(t => t.TicketState == "出票").Sum(tt => tt.Paymethod == "现金账户" ? tt.OrderMoney : 0);
                        ticketSalesCarrayCode.IssuePlatFormShouldMoney = pp.Where(t => t.TicketState == "出票").Sum(tt => (tt.Paymethod != "信用账户" && tt.Paymethod != "现金账户") ? tt.OrderMoney : 0);
                        ticketSalesCarrayCode.IssueTicketShouldMoney = pp.Where(t => t.TicketState == "出票").Sum(tt => tt.OrderMoney);

                        //退
                        ticketSalesCarrayCode.RefundTicketCount = pp.Where(t => t.TicketState == "退票").Count();
                        ticketSalesCarrayCode.RefundTicketPrice = pp.Where(t => t.TicketState == "退票").Sum(tt => tt.PMFee);
                        ticketSalesCarrayCode.RefundTicketTaxFee = pp.Where(t => t.TicketState == "退票").Sum(tt => tt.ABFee + tt.RQFee);
                        ticketSalesCarrayCode.RefundTicketCommission = pp.Where(t => t.TicketState == "退票").Sum(tt => tt.PMFee - tt.OrderMoney);
                        ticketSalesCarrayCode.RefundTicketShouldMoney = pp.Where(t => t.TicketState == "退票").Sum(tt => tt.OrderMoney);
                        ticketSalesCarrayCode.RefundTicketRealMoney = pp.Where(t => t.TicketState == "退票").Sum(tt => tt.OrderMoney);
                        //废
                        ticketSalesCarrayCode.InvalidTicketCount = pp.Where(t => t.TicketState == "废票").Count();
                        ticketSalesCarrayCode.InvalidTicketPrice = pp.Where(t => t.TicketState == "废票").Sum(tt => tt.PMFee);
                        ticketSalesCarrayCode.InvalidTicketTaxFee = pp.Where(t => t.TicketState == "废票").Sum(tt => tt.ABFee + tt.RQFee);
                        ticketSalesCarrayCode.InvalidTicketCommission = pp.Where(t => t.TicketState == "废票").Sum(tt => tt.PMFee - tt.OrderMoney);
                        ticketSalesCarrayCode.InvalidTicketShouldMoney = pp.Where(t => t.TicketState == "废票").Sum(tt => tt.OrderMoney);
                        ticketSalesCarrayCode.InvalidTicketRealMoney = pp.Where(t => t.TicketState == "废票").Sum(tt => tt.OrderMoney);
                        ticketSale.CarrayCodeDic.Add(ticketSalesCarrayCode);
                    }
                    ticketSalesStatisticsDto.PolicyCodeDic.Add(ticketSale);
                }
            }
            ticketSalesStatisticsDto.PolicyCode = "合计";
            //出
            var IssueTicket = list.Where(t => t.TicketState == "出票").AsParallel();
            if (IssueTicket.Count() > 0)
            {
                ticketSalesStatisticsDto.IssueTicketCount = IssueTicket.Count();
                ticketSalesStatisticsDto.IssueTicketPrice = IssueTicket.Sum(tt => tt.PMFee);
                ticketSalesStatisticsDto.IssueTicketTaxFee = IssueTicket.Sum(tt => tt.ABFee + tt.RQFee);
                ticketSalesStatisticsDto.IssueTicketCommission = IssueTicket.Sum(tt => tt.PMFee - tt.OrderMoney);
                ticketSalesStatisticsDto.IssueCreditShouldMoney = IssueTicket.Sum(pp => pp.Paymethod == "信用账户" ? pp.OrderMoney : 0);
                ticketSalesStatisticsDto.IssueAccountShouldMoney = IssueTicket.Sum(pp => pp.Paymethod == "现金账户" ? pp.OrderMoney : 0);
                ticketSalesStatisticsDto.IssuePlatFormShouldMoney = IssueTicket.Sum(pp => (pp.Paymethod != "信用账户" && pp.Paymethod != "现金账户") ? pp.OrderMoney : 0);
                ticketSalesStatisticsDto.IssueTicketShouldMoney = IssueTicket.Sum(tt => tt.OrderMoney);

            }

            //退
            var RefundTicket = list.Where(t => t.TicketState == "退票").AsParallel();
            if (RefundTicket.Count() > 0)
            {
                ticketSalesStatisticsDto.RefundTicketCount = RefundTicket.Count();
                ticketSalesStatisticsDto.RefundTicketPrice = RefundTicket.Sum(tt => tt.PMFee);
                ticketSalesStatisticsDto.RefundTicketTaxFee = RefundTicket.Sum(tt => tt.ABFee + tt.RQFee);
                ticketSalesStatisticsDto.RefundTicketCommission = RefundTicket.Sum(tt => tt.PMFee - tt.OrderMoney);
                ticketSalesStatisticsDto.RefundTicketShouldMoney = RefundTicket.Sum(p => p.OrderMoney);
                ticketSalesStatisticsDto.RefundTicketRealMoney = RefundTicket.Sum(tt => tt.OrderMoney);
            }
            //废
            var InvalidTicket = list.Where(t => t.TicketState == "废票").AsParallel();
            if (InvalidTicket.Count() > 0)
            {
                ticketSalesStatisticsDto.InvalidTicketCount = InvalidTicket.Count();
                ticketSalesStatisticsDto.InvalidTicketPrice = InvalidTicket.Sum(tt => tt.PMFee);
                ticketSalesStatisticsDto.InvalidTicketTaxFee = InvalidTicket.Sum(tt => tt.ABFee + tt.RQFee);
                ticketSalesStatisticsDto.InvalidTicketCommission = InvalidTicket.Sum(tt => tt.PMFee - tt.OrderMoney);
                ticketSalesStatisticsDto.InvalidTicketShouldMoney = InvalidTicket.Sum(p => p.OrderMoney);
                ticketSalesStatisticsDto.InvalidTicketRealMoney = InvalidTicket.Sum(tt => tt.OrderMoney);
            }
            return ticketSalesStatisticsDto;
            #endregion
        }
        //[ExtOperationInterceptor("机票总表明细")]
        public DataPack<TicketSumDetailDto> GetTicketSumDetail(TicketQueryEntity ticketQueryEntity)
        {
            Stopwatch watch = new Stopwatch();

            watch.Start();
            ParallelQuery<Ticket_Conso> query = QueryWhere(ticketQueryEntity).ToList().AsParallel();
            List<TicketSumDetailDto> ticketList = new List<TicketSumDetailDto>();
            var list = query.OrderByDescending(p => p.CreateDate).Skip((ticketQueryEntity.startIndex - 1) * ticketQueryEntity.count).Take(ticketQueryEntity.count).ToList();
            for (int i = 0; i < list.Count; i++)
            {
                ticketList.Add(new TicketSumDetailDto()
                {
                    ABFee = list[i].ABFee,
                    BigCode = list[i].BigCode,
                    CarrierCode = list[i].CarrierCode,
                    CarryCode = list[i].CarryCode,
                    Code = list[i].Code,
                    CreateDate = list[i].CreateDate,
                    CurrentOrderID = list[i].CurrentOrderID,
                    FlightNum = list[i].FlightNum,
                    ID = list[i].ID,
                    InCome = list[i].InCome,
                    IssueTicketCode = list[i].IssueTicketCode,
                    Money = list[i].Money,
                    OrderID = list[i].OrderID,
                    OrderMoney = list[i].OrderMoney,
                    PaidMoney = list[i].PaidMoney,
                    PaidMethod = list[i].PaidMethod,
                    PaidPoint = list[i].PaidPoint,
                    PassengerName = list[i].PassengerName,
                    PayFee = list[i].PayFee,
                    Paymethod = list[i].Paymethod,
                    PMFee = list[i].PMFee,
                    PNR = list[i].PNR,
                    PolicyFrom = list[i].PolicyFrom,
                    RetirementPoundage = list[i].RetirementPoundage,
                    RQFee = list[i].RQFee,
                    Seat = list[i].Seat,
                    SeatPrice = list[i].SeatPrice,
                    StartTime = list[i].StartTime,
                    TicketNum = list[i].TicketNum,
                    TicketState = list[i].TicketState,
                    TransactionFee = list[i].TransactionFee,
                    Voyage = list[i].Voyage
                });
            }
            //query.OrderByDescending(p => p.CreateDate).Skip((ticketQueryEntity.startIndex - 1) * ticketQueryEntity.count).Take(ticketQueryEntity.count).ToList().ForEach(p => ticketList.Add(AutoMapper.Mapper.Map<Ticket_Conso, TicketSumDetailDto>(p)));
            var datapack = new DataPack<TicketSumDetailDto>()
            {
                TotalCount = query.Count(),
                List = ticketList
            };
            watch.Stop();
            var tims3 = watch.ElapsedMilliseconds;

            return datapack;
        }
        /// <summary>
        /// 机票销售汇总
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        //[ExtOperationInterceptor("机票销售汇总")]
        public List<ResponseTicketSaleSum> FindTicketSaleSum(DateTime? startTime, DateTime? endTime)
        {
            var query = ticketRepository.FindAllNoTracking().OfType<Ticket_Conso>();
            if (startTime.HasValue)
                query = query.Where(p => p.CreateDate >= startTime.Value);
            if (endTime.HasValue)
                query = query.Where(p => p.CreateDate <= endTime.Value);

            List<ResponseTicketSaleSum> listsalesum = new List<ResponseTicketSaleSum>();

            var list = query.ToList().AsParallel();

            if (list.Count() > 0)
            {
                foreach (var item in list.ToLookup(t => t.CreateDate.ToString("yyyy-MM-dd")))
                {
                    ResponseTicketSaleSum rtsale = new ResponseTicketSaleSum();
                    rtsale.DateTime = item.Key;
                    foreach (var s in item.Where(w => w.CreateDate.ToString("yyyy-MM-dd") == item.Key).ToLookup(t => t.TicketState))
                    {
                        var liststate = s.Where(w => w.TicketState == s.Key).ToList().AsParallel();
                        switch (s.Key)
                        {
                            case "出票":
                                foreach (var p in liststate.ToLookup(t => t.Paymethod))
                                {
                                    var listIssue = p.Where(w => w.Paymethod == p.Key).AsParallel();
                                    switch (p.Key)
                                    {
                                        case "现金账户":
                                            rtsale.IssueAccountMoney = listIssue.Sum(t => t.OrderMoney);
                                            rtsale.IssueAccountPayFee = listIssue.Sum(t => t.PayFee);
                                            rtsale.IssueAccountTransactionFee = listIssue.Sum(t => t.TransactionFee);
                                            break;
                                        case "信用账户":
                                            rtsale.IssueCreditMoney = listIssue.Sum(t => t.OrderMoney);
                                            rtsale.IssueCreditPayFee = listIssue.Sum(t => t.PayFee);
                                            rtsale.IssueCreditTransactionFee = listIssue.Sum(t => t.TransactionFee);
                                            break;
                                        case "支付宝":
                                            rtsale.IssueAlipayMoney = listIssue.Sum(t => t.OrderMoney);
                                            rtsale.IssueAlipayPayFee = listIssue.Sum(t => t.PayFee);
                                            rtsale.IssueAlipayTransactionFee = listIssue.Sum(t => t.TransactionFee);
                                            break;
                                        case "财付通":
                                            rtsale.IssueTenpayMoney = listIssue.Sum(t => t.OrderMoney);
                                            rtsale.IssueTenpayPayFee = listIssue.Sum(t => t.PayFee);
                                            rtsale.IssueTenpayTransactionFee = listIssue.Sum(t => t.TransactionFee);
                                            break;
                                    }
                                }
                                rtsale.IssuePaidMoney = liststate.Sum(ss => ss.PaidMoney);
                                rtsale.IssueInCome = liststate.Sum(ss => ss.InCome);
                                break;
                            case "退票":
                                foreach (var p in liststate.ToLookup(t => t.Paymethod))
                                {
                                    var listBounce = p.Where(w => w.Paymethod == p.Key).AsParallel();
                                    switch (p.Key)
                                    {
                                        case "现金账户":
                                            rtsale.BounceAccountMoney = listBounce.Sum(t => t.OrderMoney);
                                            rtsale.BounceAccountPayFee = listBounce.Sum(t => t.PayFee);
                                            rtsale.BounceAccountTransactionFee = listBounce.Sum(t => t.TransactionFee);
                                            break;
                                        case "信用账户":
                                            rtsale.BounceCreditMoney = listBounce.Sum(t => t.OrderMoney);
                                            rtsale.BounceCreditPayFee = listBounce.Sum(t => t.PayFee);
                                            rtsale.BounceCreditTransactionFee = listBounce.Sum(t => t.TransactionFee);
                                            break;
                                        case "支付宝":
                                            rtsale.BounceAlipayMoney = listBounce.Sum(t => t.OrderMoney);
                                            rtsale.BounceAlipayPayFee = listBounce.Sum(t => t.PayFee);
                                            rtsale.BounceAlipayTransactionFee = listBounce.Sum(t => t.TransactionFee);
                                            break;
                                        case "财付通":
                                            rtsale.BounceTenpayMoney = listBounce.Sum(t => t.OrderMoney);
                                            rtsale.BounceTenpayPayFee = listBounce.Sum(t => t.PayFee);
                                            rtsale.BounceTenpayTransactionFee = listBounce.Sum(t => t.TransactionFee);
                                            break;
                                    }
                                }
                                rtsale.BounceInCome = liststate.Sum(ss => ss.InCome);
                                break;
                            case "废票":
                                foreach (var p in liststate.ToLookup(t => t.Paymethod))
                                {
                                    var listAnnul = p.Where(w => w.Paymethod == p.Key).AsParallel();
                                    switch (p.Key)
                                    {
                                        case "现金账户":
                                            rtsale.AnnulAccountMoney = listAnnul.Sum(t => t.OrderMoney);
                                            rtsale.AnnulAccountPayFee = listAnnul.Sum(t => t.PayFee);
                                            rtsale.AnnulAccountTransactionFee = listAnnul.Sum(t => t.TransactionFee);
                                            break;
                                        case "信用账户":
                                            rtsale.AnnulCreditMoney = listAnnul.Sum(t => t.OrderMoney);
                                            rtsale.AnnulCreditPayFee = listAnnul.Sum(t => t.PayFee);
                                            rtsale.AnnulCreditTransactionFee = listAnnul.Sum(t => t.TransactionFee);
                                            break;
                                        case "支付宝":
                                            rtsale.AnnulAlipayMoney = listAnnul.Sum(t => t.OrderMoney);
                                            rtsale.AnnulAlipayPayFee = listAnnul.Sum(t => t.PayFee);
                                            rtsale.AnnulAlipayTransactionFee = listAnnul.Sum(t => t.TransactionFee);
                                            break;
                                        case "财付通":
                                            rtsale.AnnulTenpayMoney = listAnnul.Sum(t => t.OrderMoney);
                                            rtsale.AnnulTenpayPayFee = listAnnul.Sum(t => t.PayFee);
                                            rtsale.AnnulTenpayTransactionFee = listAnnul.Sum(t => t.TransactionFee);
                                            break;
                                    }
                                }
                                rtsale.AnnulInCome = liststate.Sum(ss => ss.InCome);
                                break;
                            case "改签":
                                foreach (var p in liststate.ToLookup(t => t.Paymethod))
                                {
                                    var listChange = p.Where(w => w.Paymethod == p.Key).AsParallel();
                                    switch (p.Key)
                                    {
                                        case "现金账户":
                                            rtsale.ChangeAccountMoney = listChange.Sum(t => t.OrderMoney);
                                            rtsale.ChangeAccountPayFee = listChange.Sum(t => t.PayFee);
                                            rtsale.ChangeAccountTransactionFee = listChange.Sum(t => t.TransactionFee);
                                            break;
                                        case "信用账户":
                                            rtsale.ChangeCreditMoney = listChange.Sum(t => t.OrderMoney);
                                            rtsale.ChangeCreditPayFee = listChange.Sum(t => t.PayFee);
                                            rtsale.ChangeCreditTransactionFee = listChange.Sum(t => t.TransactionFee);
                                            break;
                                        case "支付宝":
                                            rtsale.ChangeAlipayMoney = listChange.Sum(t => t.OrderMoney);
                                            rtsale.ChangeAlipayPayFee = listChange.Sum(t => t.PayFee);
                                            rtsale.ChangeAlipayTransactionFee = listChange.Sum(t => t.TransactionFee);
                                            break;
                                        case "财付通":
                                            rtsale.ChangeTenpayMoney = listChange.Sum(t => t.OrderMoney);
                                            rtsale.ChangeTenpayPayFee = listChange.Sum(t => t.PayFee);
                                            rtsale.ChangeTenpayTransactionFee = listChange.Sum(t => t.TransactionFee);
                                            break;
                                    }
                                }
                                rtsale.ChangeInCome = liststate.Sum(ss => ss.InCome);
                                break;
                        }
                    }
                    rtsale.TotalInCome = item.Sum(ss => ss.InCome);
                    listsalesum.Add(rtsale);
                }
            }
            return listsalesum;
        }

        /// <summary>
        /// 用户销售统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="BusinessmanCode"></param>
        /// <returns></returns>
        //[ExtOperationInterceptor("用户销售统计")]
        public TicketBusniessStaticsDto GetTicketBusniessStatics(DateTime? startTime, DateTime? endTime, string BusinessmanCode)
        {
            var query = orderRepository.FindAll(t => t.OrderStatus == EnumOrderStatus.IssueAndCompleted);
            if (!string.IsNullOrEmpty(BusinessmanCode) && !string.IsNullOrEmpty(BusinessmanCode.Trim()))
            {
                query = query.Where(t => t.BusinessmanCode == BusinessmanCode.Trim());
            }
            if (startTime != null)
            {
                query = query.Where(t => t.CreateTime >= startTime);
            }
            if (endTime != null)
            {
                query = query.Where(t => t.CreateTime <= endTime);
            }
            var list = query.Select(t => new
            {
                t.CreateTime,
                t.OrderPay.PayMethod,
                t.OrderMoney,
                PaidMoney = t.OrderPay != null ? t.OrderPay.PaidMoney : 0,
                t.BusinessmanName,
                passengerCount = t.Passengers.Count()
            }).ToList();
            TicketBusniessStaticsDto TicketBusniessStaticsDto = new TicketBusniessStaticsDto();
            TicketBusniessStaticsDto.TicketBusniessListInfo = list.GroupBy(g => g.BusinessmanName).Select(p => (new TicketBusniessListInfo()
            {
                BusnissName = p.Key,
                IssueTicketCount = p.Sum(pp => pp.passengerCount),
                CreditMoney = p.Sum(pp => pp.PayMethod == EnumPayMethod.Credit ? pp.OrderMoney : 0),
                AccountMoney = p.Sum(pp => pp.PayMethod == EnumPayMethod.Account ? pp.OrderMoney : 0),
                PlatFormMoney = p.Sum(pp => (pp.PayMethod != EnumPayMethod.Account && pp.PayMethod != EnumPayMethod.Credit) ? pp.OrderMoney : 0),
                PayMoney = p.Sum(pp => pp.OrderMoney),
                PaidMoney = p.Sum(pp => pp.PaidMoney),
                TicketDayListInfo = p.OrderBy(o => o.CreateTime).GroupBy(gg => DateTime.Parse(gg.CreateTime.ToString()).ToString("yyyy-MM-dd")).Select(pp => (new TicketDayListInfo()
                {
                    Daytime = pp.Key,
                    IssueTicketCount = pp.Sum(ss => ss.passengerCount),
                    CreditMoney = pp.Sum(ss => ss.PayMethod == EnumPayMethod.Credit ? ss.OrderMoney : 0),
                    AccountMoney = pp.Sum(ss => ss.PayMethod == EnumPayMethod.Account ? ss.OrderMoney : 0),
                    PlatFormMoney = pp.Sum(ss => (ss.PayMethod != EnumPayMethod.Account && ss.PayMethod != EnumPayMethod.Credit) ? ss.OrderMoney : 0),
                    PayMoney = pp.Sum(ss => ss.OrderMoney),
                    PaidMoney = pp.Sum(ss => ss.PaidMoney)
                })).ToList()

            })).ToList();
            TicketBusniessStaticsDto.BusnissName = "合计";
            TicketBusniessStaticsDto.IssueTicketCount = list.Sum(pp => pp.passengerCount);
            TicketBusniessStaticsDto.CreditMoney = list.Sum(pp => pp.PayMethod == EnumPayMethod.Credit ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.AccountMoney = list.Sum(pp => pp.PayMethod == EnumPayMethod.Account ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.PlatFormMoney = list.Sum(pp => (pp.PayMethod != EnumPayMethod.Account && pp.PayMethod != EnumPayMethod.Credit) ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.PayMoney = list.Sum(pp => pp.OrderMoney);
            TicketBusniessStaticsDto.PaidMoney = list.Sum(pp => pp.PaidMoney);
            TicketBusniessStaticsDto.TicketDayListInfo = list.OrderBy(o => o.CreateTime).GroupBy(g => DateTime.Parse(g.CreateTime.ToString()).ToString("yyyy-MM-dd")).Select(pp => (new TicketDayListInfo()
            {
                Daytime = pp.Key,
                IssueTicketCount = pp.Sum(ss => ss.passengerCount),
                CreditMoney = pp.Sum(ss => ss.PayMethod == EnumPayMethod.Credit ? ss.OrderMoney : 0),
                AccountMoney = pp.Sum(ss => ss.PayMethod == EnumPayMethod.Account ? ss.OrderMoney : 0),
                PlatFormMoney = pp.Sum(ss => (ss.PayMethod != EnumPayMethod.Account && ss.PayMethod != EnumPayMethod.Credit) ? ss.OrderMoney : 0),
                PayMoney = pp.Sum(ss => ss.OrderMoney),
                PaidMoney = pp.Sum(ss => ss.PaidMoney)
            })).ToList();
            return TicketBusniessStaticsDto;
        }
        /// <summary>
        /// 平台接口销售统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="PlatformCode"></param>
        /// <returns></returns>
        //[ExtOperationInterceptor("平台接口销售统计")]
        public TicketBusniessStaticsDto GetTicketInterfaceStatics(DateTime? startTime, DateTime? endTime, string PlatformCode)
        {
            var query = ticketRepository.FindAllNoTracking().OfType<Ticket_Conso>().Where(t => t.TicketState == "出票");
            if (!string.IsNullOrEmpty(PlatformCode) && !string.IsNullOrEmpty(PlatformCode.Trim()))
            {
                query = query.Where(t => t.PolicyFrom == PlatformCode.Trim());
            }
            if (startTime != null)
            {
                query = query.Where(t => t.CreateDate >= startTime);
            }
            if (endTime != null)
            {
                query = query.Where(t => t.CreateDate <= endTime);
            }
            var list = query.Select(t => new
            {
                t.CreateDate,
                t.Paymethod,
                t.OrderMoney,
                t.PaidMoney,
                t.PolicyFrom
            }).ToList();
            TicketBusniessStaticsDto TicketBusniessStaticsDto = new TicketBusniessStaticsDto();
            TicketBusniessStaticsDto.TicketBusniessListInfo = list.GroupBy(g => g.PolicyFrom).Select(p => (new TicketBusniessListInfo()
            {
                BusnissName = p.Key == "系统" ? "卖票宝" : p.Key,
                IssueTicketCount = p.Count(),
                CreditMoney = p.Sum(pp => pp.Paymethod == "信用账户" ? pp.OrderMoney : 0),
                AccountMoney = p.Sum(pp => pp.Paymethod == "现金账户" ? pp.OrderMoney : 0),
                PlatFormMoney = p.Sum(pp => (pp.Paymethod != "信用账户" && pp.Paymethod != "现金账户") ? pp.OrderMoney : 0),
                PayMoney = p.Sum(pp => pp.OrderMoney),
                PaidMoney = p.Sum(pp => pp.PaidMoney),
                TicketDayListInfo = p.OrderBy(o => o.CreateDate).GroupBy(gg => DateTime.Parse(gg.CreateDate.ToString()).ToString("yyyy-MM-dd")).Select(pp => (new TicketDayListInfo()
                {
                    Daytime = pp.Key,
                    IssueTicketCount = pp.Count(),
                    CreditMoney = pp.Sum(ss => ss.Paymethod == "信用账户" ? ss.OrderMoney : 0),
                    AccountMoney = pp.Sum(ss => ss.Paymethod == "现金账户" ? ss.OrderMoney : 0),
                    PlatFormMoney = pp.Sum(ss => (ss.Paymethod != "信用账户" && ss.Paymethod != "现金账户") ? ss.OrderMoney : 0),
                    PayMoney = pp.Sum(ss => ss.OrderMoney),
                    PaidMoney = pp.Sum(ss => ss.PaidMoney)
                })).ToList()

            })).ToList();
            TicketBusniessStaticsDto.BusnissName = "合计";
            TicketBusniessStaticsDto.IssueTicketCount = list.Count();
            TicketBusniessStaticsDto.CreditMoney = list.Sum(pp => pp.Paymethod == "信用账户" ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.AccountMoney = list.Sum(pp => pp.Paymethod == "现金账户" ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.PlatFormMoney = list.Sum(pp => (pp.Paymethod != "信用账户" && pp.Paymethod != "现金账户") ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.PayMoney = list.Sum(pp => pp.OrderMoney);
            TicketBusniessStaticsDto.PaidMoney = list.Sum(pp => pp.PaidMoney);
            TicketBusniessStaticsDto.TicketDayListInfo = list.OrderBy(o => o.CreateDate).GroupBy(g => DateTime.Parse(g.CreateDate.ToString()).ToString("yyyy-MM-dd")).Select(pp => (new TicketDayListInfo()
            {
                Daytime = pp.Key,
                IssueTicketCount = pp.Count(),
                CreditMoney = pp.Sum(ss => ss.Paymethod == "信用账户" ? ss.OrderMoney : 0),
                AccountMoney = pp.Sum(ss => ss.Paymethod == "现金账户" ? ss.OrderMoney : 0),
                PlatFormMoney = pp.Sum(ss => (ss.Paymethod != "信用账户" && ss.Paymethod != "现金账户") ? ss.OrderMoney : 0),
                PayMoney = pp.Sum(ss => ss.OrderMoney),
                PaidMoney = pp.Sum(ss => ss.PaidMoney)
            })).ToList();
            return TicketBusniessStaticsDto;
        }
        /// <summary>
        /// 运营销售统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="SupplierCode"></param>
        /// <returns></returns>
        //[ExtOperationInterceptor("运营销售统计")]
        public TicketBusniessStaticsDto GetTicketCarrierStatics(DateTime? startTime, DateTime? endTime, string CarrierCode)
        {
            var query = ticketRepository.FindAllNoTracking().OfType<Ticket_Conso>().Where(t => t.TicketState == "出票");
            if (!string.IsNullOrEmpty(CarrierCode) && !string.IsNullOrEmpty(CarrierCode.Trim()))
            {
                query = query.Where(t => t.CarrierCode == CarrierCode.Trim());
            }
            if (startTime != null)
            {
                query = query.Where(t => t.CreateDate >= startTime);
            }
            if (endTime != null)
            {
                query = query.Where(t => t.CreateDate <= endTime);
            }
            var list = query.Select(t => new
            {

                t.CarrierCode,
                t.CreateDate,
                t.Paymethod,
                t.OrderMoney,
                t.PaidMoney,
                t.PolicyFrom
            }).ToList();
            TicketBusniessStaticsDto TicketBusniessStaticsDto = new TicketBusniessStaticsDto();
            TicketBusniessStaticsDto.TicketBusniessListInfo = list.GroupBy(g => g.CarrierCode).Select(p => (new TicketBusniessListInfo()
            {
                BusnissName = p.Key,
                IssueTicketCount = p.Count(),
                CreditMoney = p.Sum(pp => pp.Paymethod == "信用账户" ? pp.OrderMoney : 0),
                AccountMoney = p.Sum(pp => pp.Paymethod == "现金账户" ? pp.OrderMoney : 0),
                PlatFormMoney = p.Sum(pp => (pp.Paymethod != "信用账户" && pp.Paymethod != "现金账户") ? pp.OrderMoney : 0),
                PayMoney = p.Sum(pp => pp.OrderMoney),
                PaidMoney = p.Sum(pp => pp.PaidMoney),
                TicketDayListInfo = p.OrderBy(o => o.CreateDate).GroupBy(gg => DateTime.Parse(gg.CreateDate.ToString()).ToString("yyyy-MM-dd")).Select(pp => (new TicketDayListInfo()
                {
                    Daytime = pp.Key,
                    IssueTicketCount = pp.Count(),
                    CreditMoney = pp.Sum(ss => ss.Paymethod == "信用账户" ? ss.OrderMoney : 0),
                    AccountMoney = pp.Sum(ss => ss.Paymethod == "现金账户" ? ss.OrderMoney : 0),
                    PlatFormMoney = pp.Sum(ss => (ss.Paymethod != "信用账户" && ss.Paymethod != "现金账户") ? ss.OrderMoney : 0),
                    PayMoney = pp.Sum(ss => ss.OrderMoney),
                    PaidMoney = pp.Sum(ss => ss.PaidMoney)
                })).ToList()

            })).ToList();
            TicketBusniessStaticsDto.BusnissName = "合计";
            TicketBusniessStaticsDto.IssueTicketCount = list.Count();
            TicketBusniessStaticsDto.CreditMoney = list.Sum(pp => pp.Paymethod == "信用账户" ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.AccountMoney = list.Sum(pp => pp.Paymethod == "现金账户" ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.PlatFormMoney = list.Sum(pp => (pp.Paymethod != "信用账户" && pp.Paymethod != "现金账户") ? pp.OrderMoney : 0);
            TicketBusniessStaticsDto.PayMoney = list.Sum(pp => pp.OrderMoney);
            TicketBusniessStaticsDto.PaidMoney = list.Sum(pp => pp.PaidMoney);
            TicketBusniessStaticsDto.TicketDayListInfo = list.OrderBy(o => o.CreateDate).GroupBy(g => DateTime.Parse(g.CreateDate.ToString()).ToString("yyyy-MM-dd")).Select(pp => (new TicketDayListInfo()
            {
                Daytime = pp.Key,
                IssueTicketCount = pp.Count(),
                CreditMoney = pp.Sum(ss => ss.Paymethod == "信用账户" ? ss.OrderMoney : 0),
                AccountMoney = pp.Sum(ss => ss.Paymethod == "现金账户" ? ss.OrderMoney : 0),
                PlatFormMoney = pp.Sum(ss => (ss.Paymethod != "信用账户" && ss.Paymethod != "现金账户") ? ss.OrderMoney : 0),
                PayMoney = pp.Sum(ss => ss.OrderMoney),
                PaidMoney = pp.Sum(ss => ss.PaidMoney)
            })).ToList();
            return TicketBusniessStaticsDto;
        }

        #endregion
        //[ExtOperationInterceptor("获取未起飞机票信息")]
        public List<NotTakeOffTicketDto> GetNotTakeOffTicket(List<string> Codelist)
        {
            List<NotTakeOffTicketDto> list = new List<NotTakeOffTicketDto>();
            if (Codelist != null && Codelist.Count > 0)
            {
                string code = string.Empty;
                for (int i = 0; i < Codelist.Count; i++)
                {
                    code = Codelist[i].ToString();
                    var bm = businessmanRepository.FindAll(p => p.CashbagCode == code).FirstOrDefault();
                    if (bm != null)
                    {
                        var query = orderRepository.FindAll(p =>
                            p.BusinessmanCode == bm.Code &&
                            p.OrderStatus == EnumOrderStatus.IssueAndCompleted &&
                            p.SkyWays.Where(s => s.StartDateTime > DateTime.Now).Count() > 0
                            );
                        string Code = Codelist[i].ToString();
                        int TicketCount = query.Count() > 0 ? query.Sum(p => p.Passengers.Count()) : 0;
                        decimal TicketMoney = query.Count() > 0 ? query.Select(p => p.OrderMoney).Sum() : 0;
                        list.Add(new NotTakeOffTicketDto()
                        {
                            Code = Code,
                            TicketCount = TicketCount,
                            TicketMoney = TicketMoney
                        });
                    }
                }
            }
            return list;
        }


        //[ExtOperationInterceptor("获取运营商订单信息【GetOrderBySearch】")]
        public DataPack<ResponseOrder> GetOrderBySearch(string orderId, string pnr, string passengerName, string ticketNumber, string fromCity, string toCity, DateTime? startDateTime, DateTime? toDateTime, string businessmanCode, DateTime? startCreateTime, DateTime? endDateTime, string carrayCode, string platformCode, int[] orderStatus, int startIndex, int count, string OutTradeNo)
        {
            var query = orderRepository.FindAll();
            if (!string.IsNullOrEmpty(orderId) && !string.IsNullOrEmpty(orderId.Trim()))
                query = query.Where(p => p.OrderId == orderId.Trim());
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(pnr.Trim()))
                query = query.Where(p => p.PnrCode == pnr.Trim());
            if (!string.IsNullOrEmpty(passengerName) && !string.IsNullOrEmpty(passengerName.Trim()))
                query = query.Where(p => p.Passengers.Count(t => t.PassengerName.Contains(passengerName.Trim())) > 0);
            if (!string.IsNullOrEmpty(ticketNumber) && !string.IsNullOrEmpty(ticketNumber.Trim()))
                query = query.Where(p => p.Passengers.Count(t => t.TicketNumber.Contains(ticketNumber.Trim())) > 0);
            //订单状态条件判断
            query = orderStatus != null ? query.Where(p => orderStatus.Contains((int)p.OrderStatus)) : query.Where(p => p.OrderStatus != EnumOrderStatus.Invalid);
            //创建日期条件添加
            if (startCreateTime != null)
                query = query.Where(p => p.CreateTime >= startCreateTime);
            if (endDateTime != null)
                query = query.Where(p => p.CreateTime <= endDateTime);
            if (!string.IsNullOrEmpty(fromCity) && !string.IsNullOrEmpty(fromCity.Trim()))
                query = query.Where(p => p.SkyWays.Count(t => t.FromCityCode == fromCity.Trim()) > 0);
            if (!string.IsNullOrEmpty(toCity) && !string.IsNullOrEmpty(toCity.Trim()))
                query = query.Where(p => p.SkyWays.Count(t => t.ToCityCode == toCity.Trim()) > 0);
            if (startDateTime.HasValue)
                query = query.Where(p => p.SkyWays.Any(t => t.StartDateTime >= startDateTime.Value));
            if (toDateTime.HasValue)
                query = query.Where(p => p.SkyWays.Any(t => t.ToDateTime <= toDateTime.Value));
            if (!string.IsNullOrEmpty(carrayCode) && !string.IsNullOrEmpty(carrayCode.Trim()))
                query = query.Where(p => p.SkyWays.Any(t => t.CarrayCode == carrayCode.Trim()));

            if (!string.IsNullOrEmpty(platformCode) && !string.IsNullOrEmpty(platformCode.Trim()))
                query = query.Where(p => p.Policy.PlatformCode == platformCode.Trim());

            if (!string.IsNullOrEmpty(businessmanCode) && !string.IsNullOrEmpty(businessmanCode.Trim()))
                query = query.Where(p => p.BusinessmanCode == businessmanCode.Trim());
            if (!string.IsNullOrEmpty(OutTradeNo) && !string.IsNullOrEmpty(OutTradeNo.Trim()))
                query = query.Where(p => p.OrderPay.PaySerialNumber == OutTradeNo.Trim());
            var list = query.OrderByDescending(p => p.CreateTime).Skip(startIndex).Take(count).Select(p => new ResponseOrder
            {
                CreateTime = p.CreateTime,
                HasAfterSale = p.HasAfterSale,
                OrderId = p.OrderId,
                OrderMoney = p.OrderPay.PayMoney,//p.OrderMoney,  
                InsuranceMoney = p.Passengers.Where(v => v.BuyInsuranceCount > 0).Sum(v => v.BuyInsuranceCount * v.BuyInsurancePrice) > 0 ? p.Passengers.Where(v => v.BuyInsuranceCount > 0).Sum(v => v.BuyInsuranceCount * v.BuyInsurancePrice) : 0,
                OrderStatus = (int)p.OrderStatus,
                ClientOrderStatus = p.OrderStatus,
                PnrCode = p.PnrCode,
                PnrSource = p.PnrSource,
                OutTradeNo = p.OrderPay.PaySerialNumber,
                OrderSource = p.OrderSource,
                NewPnrCode = p.NewPnrCode,
                PayMethodCode = p.OrderPay.PayMethodCode,
                Passengers = p.Passengers.Select(s => new ResponsePassenger
                {
                    PassengerName = s.PassengerName,
                    PassengerType = s.PassengerType,
                    RQFee = s.RQFee,
                    SeatPrice = s.SeatPrice,
                    TaxFee = s.ABFee
                }).ToList(),
                Policy = new ResponsePolicy
                {
                    Commission = p.Policy.Commission,
                    Point = p.Policy.PolicyPoint,
                    PolicySpecialType = p.Policy.PolicySpecialType
                },
                SkyWays = p.SkyWays.Select(s => new ResponseSkyWay
                {
                    CarrayCode = s.CarrayCode,
                    FlightNumber = s.FlightNumber,
                    FromCityCode = s.FromCityCode,
                    Seat = s.Seat,
                    StartDateTime = s.StartDateTime,
                    ToCityCode = s.ToCityCode
                }).ToList()
            }).ToList();
            list.ForEach(p =>
            {
                if (p.OrderStatus.HasValue)
                    p.OrderStatus = p.OrderStatus.Value.ToClientStatus();
            });
            var dataPack = new DataPack<ResponseOrder>()
            {
                TotalCount = query.Count(),
                List = list
            };
            return dataPack;
        }
        //[ExtOperationInterceptor("查询订单【GetOrderByOrderId】")]
        public ResponseOrderDetail GetOrderByOrderId(string orderId)
        {
            var responseOrderDetail = orderRepository.FindAll(p => p.OrderId == orderId).Select(p => new ResponseOrderDetail
            {
                OrderId = p.OrderId,
                OrderMoney = p.OrderMoney,
                OrderStatus = (int)p.OrderStatus,
                PnrCode = p.PnrCode,
                Policy = new ResponsePolicyDetail
                {
                    Point = p.Policy.PolicyPoint,
                    Remark = p.Policy.Remark,
                    ReturnTicketTime = p.Policy.ReturnTicketTime.StartTime + "-" + p.Policy.ReturnTicketTime.EndTime,
                    AnnumTicketTime = p.Policy.AnnulTicketTime.StartTime + "-" + p.Policy.AnnulTicketTime.EndTime
                },
                PayInfo = new ResponsePayInfoDetail
                {
                    PaidSerialNumber = p.OrderPay.PaidSerialNumber,
                    PayMethod = p.OrderPay.PayMethod,
                    PayStatus = p.OrderPay.PayStatus
                },
                Passengers = p.Passengers.Select(x => new ResponsePassengerDetail
                {
                    CardNo = x.CardNo,
                    PassengerName = x.PassengerName,
                    PassengerType
                    = x.PassengerType,
                    RQFee = x.RQFee,
                    SeatPrice = x.SeatPrice,
                    TaxFee = x.ABFee,
                    TicketNumber = x.TicketNumber
                }).ToList(),
                SkyWays = p.SkyWays.Select(x => new ResponseSkyWayDetail
                {
                    CarrayCode = x.CarrayCode,
                    FlightNumber = x.FlightNumber,
                    FromCityCode = x.FromCityCode,
                    ToCityCode = x.ToCityCode,
                    Seat = x.Seat,
                    StartDateTime = x.StartDateTime,
                    ToDateTime = x.ToDateTime
                }).ToList()
            }).FirstOrDefault();
            return responseOrderDetail;
        }

        [ExtOperationInterceptor("添加售后订单协调")]
        public void AddCoordination(int aftersaleid, string type, string CoordinationContent, bool isCompleted)
        {

            var order = afterSaleOrderRepository.FindAll(p => p.Id == aftersaleid).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到售后订单号为：" + aftersaleid + "的订单");
            if (CoordinationContent.Length > 500)
                throw new OrderCommException("协调内容超出范围！");
            if (order.CoordinationLogs != null && order.CoordinationLogs.Count > 100)
                throw new OrderCommException("协调数目最多100条！");

            order.CoordinationLogs.Add(new CoordinationLog()
            {
                AddDatetime = System.DateTime.Now,
                Type = type,
                Content = CoordinationContent,
                OperationPerson = currentUser.OperatorName
            });
            order.IsCoorCompleted = isCompleted;
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }

        //[ExtOperationInterceptor("获取售后订单协调")]
        public CoordinationDto GetCoordinationAfterSale(int aftersaleid)
        {
            var order = afterSaleOrderRepository.FindAll(p => p.Id == aftersaleid).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到售后订单号为：" + aftersaleid + "的订单");
            CoordinationDto d = new CoordinationDto();
            if (d.CoordinationLogs == null)
                d.CoordinationLogs = new List<CoordinationLogDto>();
            order.CoordinationLogs.ForEach(p =>
           {
               d.CoordinationLogs.Add(new CoordinationLogDto
              {
                  Type = p.Type,
                  OperationPerson = p.OperationPerson,
                  Content = p.Content,
                  AddDatetime = p.AddDatetime
              });
           });
            return d;
        }

        //[ExtOperationInterceptor("获取退款明细")]
        public List<ResponseBounLine> RefundDetails(int saleorderid)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid).FirstOrDefault();
            if (model == null)
                return null;
            if (model is AnnulOrder)
                return AutoMapper.Mapper.Map<List<BounceLine>, List<ResponseBounLine>>((model as AnnulOrder).BounceLines.ToList());
            else if (model is BounceOrder)
                return AutoMapper.Mapper.Map<List<BounceLine>, List<ResponseBounLine>>((model as BounceOrder).BounceLines.ToList());
            return null;
        }


        [ExtOperationInterceptor("退款查询")]
        public string RefundQuery(int aftersaleorder_id, string refundNo)
        {
            IPaymentClientProxy clientProxy = ObjectFactory.GetInstance<CashbagPaymentClientProxy>();
            dynamic dyn = clientProxy.RefundCheck(this.currentUser.CashbagCode, refundNo);
            if (dyn == null)
                throw new CustomException(400, "查询失败!");
            if (dyn.status == false)
                return dyn.message;
            if (dyn.result == null || dyn.result == false)
                return dyn.message;
            //查询成功
            var aftermodel = this.afterSaleOrderRepository.FindAll(p => p.Id == aftersaleorder_id).FirstOrDefault();
            if (aftermodel == null)
                throw new CustomException(400, "售后订单不存在");
            if (aftermodel.AfterSaleType == "改签")
                throw new CustomException(400, "订单不是废票或者退票");
            if (aftermodel.ProcessStatus == EnumTfgProcessStatus.Processed)
                return "已退款完成";

            if (aftermodel is AnnulOrder || aftermodel is BounceOrder)
            {

                if (aftermodel is AnnulOrder)
                {
                    AnnulOrder annulOrder = aftermodel as AnnulOrder;
                    var list = annulOrder.BounceLines.ToList();
                    var bline = list.Where(p => p.ID == refundNo).FirstOrDefault();
                    bline.RefundTime = DateTime.Now;
                    bline.Status = EnumBoundRefundStatus.Refunded;

                    if (list.All(p => p.Status == EnumBoundRefundStatus.Refunded))
                    {
                        annulOrder.Passenger.ForEach(n => n.Status = EnumTfgPassengerStatus.AnnulTicketed);
                        annulOrder.ProcessStatus = EnumTfgProcessStatus.Processed;
                        annulOrder.LockCurrentAccount = string.Empty;
                        annulOrder.CompletedTime = DateTime.Now;
                    }

                }
                else if (aftermodel is BounceOrder)
                {
                    BounceOrder bounceOrder = aftermodel as BounceOrder;
                    var list = bounceOrder.BounceLines.ToList();
                    var bline = list.Where(p => p.ID == refundNo).FirstOrDefault();
                    bline.RefundTime = DateTime.Now;
                    bline.Status = EnumBoundRefundStatus.Refunded;

                    if (list.All(p => p.Status == EnumBoundRefundStatus.Refunded))
                    {
                        bounceOrder.Passenger.ForEach(n => n.Status = EnumTfgPassengerStatus.Refunded);
                        bounceOrder.ProcessStatus = EnumTfgProcessStatus.Processed;
                        bounceOrder.LockCurrentAccount = string.Empty;
                        bounceOrder.CompletedTime = DateTime.Now;
                    }
                }
                aftermodel.WriteLog(new OrderLog
               {
                   IsShowLog = true,
                   OperationContent = "订单退款完成,交易结束",
                   OperationDatetime = DateTime.Now,
                   OperationPerson = this.currentUser.OperatorName
               });
                unitOfWorkRepository.PersistUpdateOf(aftermodel);
                unitOfWork.Commit();
                if (aftermodel.ProcessStatus == EnumTfgProcessStatus.Processed)
                {
                    MessageQueueManager.SendMessage(aftermodel.Id.ToString(), 1);
                }

            }
            return "已退款完成";
        }

        [ExtOperationInterceptor("售后明细退款")]
        public void SingleRefund(int saleorderid, string refundid)
        {
            var saleOrder = this.afterSaleOrderRepository.FindAll(p => p.Id == saleorderid && (p is AnnulOrder || p is BounceOrder)).FirstOrDefault();
            if (saleOrder == null)
                throw new CustomException(400, "售后订单不存在");
            BounceLine bounceline = null;
            if (saleOrder is AnnulOrder)
                bounceline = (saleOrder as AnnulOrder).BounceLines.Where(p => p.ID == refundid).FirstOrDefault();
            else if (saleOrder is BounceOrder)
                bounceline = (saleOrder as BounceOrder).BounceLines.Where(p => p.ID == refundid).FirstOrDefault();
            if (bounceline == null)
                throw new CustomException(400, string.Format("{0}退款明细不存在", refundid));

            if (bounceline.Status == EnumBoundRefundStatus.Refunded)
                throw new CustomException(400, string.Format("{0}已退款完成", refundid));

            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            string remark = bounceline.ChangeOrderID.HasValue ? saleorderid.ToString() : string.Format("SaleOrderRefund_{0}", saleorderid.ToString());
            var businessman = this.businessmanRepository.FindAll(x => x.Code == saleOrder.Order.BusinessmanCode).FirstOrDefault();
            client.Reimburse(businessman.CashbagCode, businessman.CashbagKey, bounceline.PaySerialNumber, bounceline.RefundMoney, bounceline.ID, remark, bounceline.BusArgs, (saleOrder is BounceOrder) ? "退票" : "废票");
            if (bounceline.PayMethod == EnumPayMethod.Account || bounceline.PayMethod == EnumPayMethod.Credit)
            {
                bounceline.Status = EnumBoundRefundStatus.Refunded;
                bounceline.RefundTime = DateTime.Now;
            }
            saleOrder.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = string.Format("{0}明细退款", refundid),
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            unitOfWorkRepository.PersistUpdateOf(saleOrder);
            unitOfWork.Commit();

            bool allRefunded = false;
            if (saleOrder is AnnulOrder)
                allRefunded = (saleOrder as AnnulOrder).BounceLines.All(p => p.Status == EnumBoundRefundStatus.Refunded);
            else if (saleOrder is BounceOrder)
                allRefunded = (saleOrder as BounceOrder).BounceLines.All(p => p.Status == EnumBoundRefundStatus.Refunded);
            if (allRefunded)
            {

                if (saleOrder is AnnulOrder)
                    saleOrder.Passenger.ForEach(p => p.Status = EnumTfgPassengerStatus.AnnulTicketed);
                else if (saleOrder is BounceOrder)
                    saleOrder.Passenger.ForEach(p => p.Status = EnumTfgPassengerStatus.Refunded);
                saleOrder.ProcessStatus = EnumTfgProcessStatus.Processed;
                saleOrder.LockCurrentAccount = string.Empty;
                saleOrder.CompletedTime = DateTime.Now;
                unitOfWorkRepository.PersistUpdateOf(saleOrder);
                unitOfWork.Commit();

                MessageQueueManager.SendMessage(saleOrder.Id.ToString(), 1);
            }

        }

        [ExtOperationInterceptor("取消/拒绝出票自动退款")]
        public void AutoRefund(string orderid, string remark, bool cancelPnr = true)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderid).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单号为：" + orderid + "的订单");
            var businessman = this.businessmanRepository.FindAll(x => x.Code == order.BusinessmanCode).FirstOrDefault();

            var behavior = order.State.GetBehaviorByCode("WaitReimburseWithRepelIssue");
            behavior.SetParame("refundMoney", order.OrderMoney);
            behavior.SetParame("remark", remark);
            behavior.SetParame("Code", businessman.Code);
            behavior.SetParame("cashbagCode", businessman.CashbagCode);
            behavior.SetParame("cashbagKey", businessman.CashbagKey);
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("platformCode", order.Policy.PlatformCode);
            if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                behavior.SetParame("platformName", service.GetPlatList()[order.Policy.PlatformCode].Name);
            }
            else
            {
                behavior.SetParame("PlatformName", "系统");
            }
            behavior.Execute();
            if (cancelPnr && order.OrderType != 2 && order.PnrSource == EnumPnrSource.CreatePnr)
            {
                bool isSuccess = false;
                if (pidService.CanCancel(order.BusinessmanCode, order.YdOffice, order.PnrCode))
                {
                    isSuccess = pidService.CancelPnr(order.BusinessmanCode, order.YdOffice, order.PnrCode);
                }
                order.WriteLog(new OrderLog
                {
                    IsShowLog = false,
                    OperationContent = string.Format("{0}取消编码：{1},操作状态：{2}", currentUser.OperatorAccount, order.PnrCode, isSuccess),
                    OperationDatetime = DateTime.Now,
                    OperationPerson = currentUser.OperatorName
                });
            }
            if (!string.IsNullOrEmpty(order.OldOrderId) && order.OrderType == 1)
            {
                var oldOrder = orderRepository.FindAll(p => p.OrderId.Equals(order.OldOrderId)).FirstOrDefault();
                if (oldOrder != null)
                {
                    oldOrder.AssocChdCount -= order.Passengers.Count();
                    unitOfWorkRepository.PersistUpdateOf(oldOrder);
                }
            }
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            MessageQueueManager.SendMessage(orderid, 2);
        }

        [ExtOperationInterceptor("线下代付")]
        public void UpdateOrderPay(OrderDataObject orderdata)
        {
            if (orderdata == null)
                throw new CustomException(500, "Order is null");
            var order = this.orderRepository.FindAll(p => p.OrderId == orderdata.OrderId).FirstOrDefault();
            if (order == null)
                throw new CustomException(500, "未查询到订单");
            string str = string.Format("代付信息修改_平台【{0},{1}】_代付点数【{2},{3}】_接口订单号【{4},{5}】_代付金额【{6},{7}】", order.Policy.PlatformCode, orderdata.PlatForm, order.Policy.OriginalPolicyPoint, orderdata.PlatPolicy, order.OutOrderId, orderdata.InterfaceOrderId, order.OrderPay.PaidMoney, orderdata.Money);
            order.Policy.PlatformCode = orderdata.PlatForm;
            order.Policy.PaidPoint = orderdata.PlatPolicy;
            order.OutOrderId = orderdata.InterfaceOrderId;
            order.OrderPay.PaidMoney = orderdata.Money;
            order.OrderPay.PaidDateTime = DateTime.Now;
            order.OrderPay.PaidMethod = "线下代付";
            order.OrderPay.PaidStatus = EnumPaidStatus.OK;
            order.OrderStatus = EnumOrderStatus.WaitIssue;
            order.Policy.ReturnTicketTime.StartTime = orderdata.ReturnOnTime;
            order.Policy.ReturnTicketTime.EndTime = orderdata.ReturnUnTime;
            order.Policy.AnnulTicketTime.StartTime = orderdata.AnnulOnTime;
            order.Policy.AnnulTicketTime.EndTime = orderdata.AnnulUnTime;

            order.WriteLog(new OrderLog
            {
                IsShowLog = false,
                OperationContent = str,
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }
        [ExtOperationInterceptor("修改乘机人信息")]
        public void UpdatePassenger(string orderid, List<PassengerDataObject> list)
        {
            if (list == null || list.Count == 0)
                throw new CustomException(500, "提交数据是空的");
            string code = AuthManager.GetCurrentUser().Code;
            var query = this.orderRepository.FindAll(p => p.OrderId == orderid);
            if (!string.IsNullOrEmpty(code))
                query = query.Where(p => p.Policy.Code == code);
            var order = query.FirstOrDefault();
            if (order == null)
                throw new CustomException(500, "没有权限修改此订单");
            StringBuilder str = new StringBuilder("修改信息");
            order.Passengers.ForEach(p =>
            {
                foreach (var item in list)
                {
                    if (p.Id == item.PassengerId)
                    {
                        str.AppendFormat("乘机人:【{6}】身份证:【{0},{1}】,手机:【{2},{3}】,票号:【{4},{5}】", p.CardNo, item.CardNo, p.Mobile, item.Phone, p.TicketNumber, item.TicketNumber, p.PassengerName);
                        p.CardNo = item.CardNo;
                        p.Mobile = item.Phone;
                        p.TicketNumber = item.TicketNumber;
                    }
                }
            });
            order.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = str.ToString(),
                OperationDatetime = DateTime.Now,
                OperationPerson = AuthManager.GetCurrentUser().OperatorAccount
            });
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }


        //[ExtOperationInterceptor("获取已有代付信息")]
        public OrderDataObject GetOrderPayInfo(string orderid)
        {
            var order = this.orderRepository.FindAllNoTracking(p => p.OrderId == orderid).Select(p => new OrderDataObject
            {
                OrderId = p.OrderId,
                InterfaceOrderId = p.OutOrderId,
                Money = p.OrderPay.PaidMoney,
                PlatForm = p.Policy.PlatformCode,
                PlatPolicy = p.Policy.OriginalPolicyPoint,
                AnnulOnTime = p.Policy.AnnulTicketTime.StartTime,
                AnnulUnTime = p.Policy.AnnulTicketTime.EndTime,
                ReturnOnTime = p.Policy.ReturnTicketTime.StartTime,
                ReturnUnTime = p.Policy.ReturnTicketTime.EndTime
            }).SingleOrDefault();
            if (order == null)
                throw new CustomException(500, string.Format("未找到订单：{0}", orderid));
            return order;
        }


        //[ExtOperationInterceptor("获取乘机人信息")]
        public List<PassengerDataObject> GetPassengerInfo(string orderid)
        {
            var order = this.orderRepository.FindAllNoTracking(p => p.OrderId == orderid).SingleOrDefault();
            if (order == null)
                throw new CustomException(500, string.Format("未找到订单：{0}", orderid));
            return order.Passengers.Select(p => new PassengerDataObject
              {
                  PassengerId = p.Id,
                  CardNo = p.CardNo,
                  Phone = p.Mobile,
                  PassengerName = p.PassengerName,
                  TicketNumber = p.TicketNumber
              }).ToList();
        }


        //[ExtOperationInterceptor("获取改签单乘机人信息")]
        public List<AfterPassengerDataObject> GetAfterPassengerInfo(int saleorderid)
        {
            var changeOrder = this.afterSaleOrderRepository.FindAllNoTracking(p => p.Id == saleorderid).OfType<ChangeOrder>().FirstOrDefault();
            if (changeOrder == null)
                throw new CustomException(500, "未获取到乘机人信息");
            return changeOrder.Passenger.Select(p => new AfterPassengerDataObject
             {

                 AfterPassengerId = p.Id,
                 AfterPassengerName = p.Passenger.PassengerName,
                 AfterSaleTravelTicketNum = p.Passenger.TicketNumber
             }).ToList();
        }

        [ExtOperationInterceptor("后台重选政策【BackChoosePolicy(PolicyDto policy, string orderId)】")]
        public OrderDto BackChoosePolicy(PolicyDto policy, string orderId)
        {
            var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
            if (order == null)
                throw new OrderCommException("没有找到订单编号为：" + orderId + "的订单");
            if (order.OrderStatus == EnumOrderStatus.PaymentInWaiting)
                throw new OrderCommException("该订单【" + orderId + "】正在支付中,请稍后。。。");
            var behavior = order.State.GetBehaviorByCode("NewSelectPolicy");
            behavior.SetParame("platformCode", "");
            behavior.SetParame("policyId", "");
            behavior.SetParame("policy", policy);
            behavior.SetParame("operatorName", currentUser.OperatorName);
            behavior.SetParame("source", "back");
            behavior.Execute();
            //修改
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
            return order.ToOrderDto();
        }

        /// <summary>
        /// 手动调用自动出票
        /// </summary>
        /// <param name="orderId"></param>
        [ExtOperationInterceptor("手动调用自动出票【HandCallAutoIssue(string orderId)")]
        public void HandCallAutoIssue(string orderId)
        {
            service.AutoIssue(orderId, "手动调用自动出票", () =>
            {
                try
                {
                    MessageQueueManager.SendMessage(orderId, 0);
                }
                catch (Exception e)
                {
                    Logger.WriteLog(LogType.ERROR, string.Format("{0}:写入总表失败", orderId), e);
                }
            });
        }

        /// <summary>
        /// 机票起飞日期监控分析
        /// </summary>
        /// <param name="code">商户号</param>
        /// <param name="CreateTime">出票时间-范围:开始</param>
        /// <param name="CreateTimeEnd">出票时间-范围:结束</param>
        /// <returns></returns>        
        //[ExtOperationInterceptor("机票起飞日期监控分析")]
        public List<ResponeTempSum> MonitorTicketSum(string code, DateTime? CreateTime, DateTime? CreateTimeEnd)
        {
            var query = this.ticketRepository.FindAllNoTracking(x => x.TicketState == "出票" && x.CreateDate >= CreateTime && x.CreateDate <= CreateTimeEnd).OfType<Ticket_Buyer>();

            if (!string.IsNullOrEmpty(code))
            {
                query = query.Where(x => x.Code.Equals(code));
            }
            var list = query.Select(p => new { StartTime = p.StartTime, IssueDate = p.CreateDate }).ToList();
            List<TempSum> tempList = new List<TempSum>();
            list.ForEach(p =>
            {
                var tempArray = p.StartTime.Trim('/').Split('/');
                if (tempArray.Length == 1)
                    tempList.Add(new TempSum { IssueDate = p.IssueDate.Date, FlyDate = DateTime.Parse(tempArray[0]).Date });
                else if (tempArray.Length == 2)
                    //foreach (var item in tempArray)
                    //{
                    //    tempList.Add(new TempSum { IssueDate = p.IssueDate.Date, FlyDate = DateTime.Parse(item).Date });
                    tempList.Add(new TempSum { IssueDate = p.IssueDate.Date, FlyDate = DateTime.Parse(tempArray[0]).Date });
                //}
            });
            var dateGroup = tempList.GroupBy(x => x.IssueDate.Date).Select(x => new TempTSum { IssueDate = x.Key }).OrderByDescending(x => x.IssueDate).ToList();
            foreach (var item in dateGroup)
            {
                item.List.AddRange(tempList.Where(p => p.IssueDate == item.IssueDate).Select(x => x.FlyDate).ToList());
            }
            List<ResponeTempSum> sumList = new List<ResponeTempSum>();
            dateGroup.ForEach(p =>
            {
                ResponeTempSum t = new ResponeTempSum
                {
                    IssueDate = p.IssueDate,
                    TotalCount = p.List.Count,
                    T0 = p.List.Where(x => x == p.IssueDate).Count(),
                    T1 = p.List.Where(x => x == p.IssueDate.AddDays(1)).Count(),
                    T2 = p.List.Where(x => x == p.IssueDate.AddDays(2)).Count(),
                    T3 = p.List.Where(x => x == p.IssueDate.AddDays(3)).Count(),
                    T4 = p.List.Where(x => x == p.IssueDate.AddDays(4)).Count(),
                    T5 = p.List.Where(x => x == p.IssueDate.AddDays(5)).Count(),
                    T6 = p.List.Where(x => x == p.IssueDate.AddDays(6)).Count(),
                    T7 = p.List.Where(x => x == p.IssueDate.AddDays(7)).Count(),
                    T8 = p.List.Where(x => x > p.IssueDate.AddDays(7)).Count(),
                };
                sumList.Add(t);
            });

            return sumList;
        }
        [ExtOperationInterceptor("线下婴儿申请审核")]
        public void ExamineBabyOrder(string orderid, decimal seatPrice)
        {
            var order = this.orderRepository.FindAll(p => p.OrderId.Equals(orderid) && p.OrderStatus == EnumOrderStatus.ApplyBabyFail).FirstOrDefault();
            if (order == null)
                throw new CustomException(500, "改订单已处理，不能继续操作");
            order.Passengers.ForEach(p =>
            {
                p.PayMoney = seatPrice;
                p.CPMoney = seatPrice;
                p.SeatPrice = seatPrice;
            });
            //订单金额
            decimal orderMoney = order.Passengers.Sum(p => p.PayMoney);
            //婴儿人数
            int infCount = order.Passengers.Count;
            order.OrderPay.PayMoney = order.Passengers.Sum(x => x.PayMoney);

            CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;
            Businessman businessman = this.businessmanRepository.FindAll(p => p.Code.Trim() == order.Policy.Code).FirstOrDefault();
            //运营商婴儿服务费
            var buyer = this.businessmanRepository.FindAllNoTracking(p => p.Code.Equals(order.BusinessmanCode)).Select(p => new
            {
                BusinessmanCode = p.Code,
                BusinessmanName = p.Name,
                CashbagCode = p.CashbagCode
            }).FirstOrDefault();
            decimal carrInfServerFee = Math.Abs(databill.NewRound(seatPrice * ((businessman is Supplier) ? (businessman as Supplier).SupRate : (businessman as Carrier).Rate), 2) * infCount);
            //采购付款
            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
            {
                Code = buyer.BusinessmanCode,
                Name = buyer.BusinessmanName,
                CashbagCode = buyer.CashbagCode,
                Money = orderMoney,
                Point = 0,
                OpType = EnumOperationType.PayMoney,
                Remark = "付款"
            });
            //运营商收款
            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
            {
                Code = businessman.Code,
                Name = businessman.Name,
                CashbagCode = businessman.CashbagCode,
                Money = orderMoney,
                OpType = EnumOperationType.Receivables,
                Remark = "收款"
            });
            //运营商付款 支付票价的服务费付给合作方
            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
            {
                Code = businessman.Code,
                Name = businessman.Name,
                CashbagCode = businessman.CashbagCode,
                Money = -carrInfServerFee,
                InfMoney = -carrInfServerFee,
                OpType = EnumOperationType.IssuePayServer,
                Remark = "服务费"
            });
            //合作方收款 收取运营票款服务费                          
            order.OrderPay.PayBillDetails.Add(new PayBillDetail()
            {
                Code = setting.CashbagCode,
                Name = "系统",
                CashbagCode = setting.CashbagCode,
                Money = carrInfServerFee,
                InfMoney = carrInfServerFee,
                OpType = EnumOperationType.ParterServer,
                Remark = "服务费"
            });
            order.TicketPrice = orderMoney;
            order.INFTicketPrice = orderMoney;
            order.OrderMoney = orderMoney;
            order.CPMoney = orderMoney;
            order.LockAccount = string.Empty;
            order.OrderStatus = EnumOrderStatus.NewOrder;
            string currentAccount = AuthManager.GetCurrentUser().OperatorAccount;
            order.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = "审核订单完成",
                OperationDatetime = DateTime.Now,
                OperationPerson = currentAccount
            });
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }

        [ExtOperationInterceptor("线下婴儿拒绝审核拒绝备注")]
        public void UnExamine(string orderid, string remark)
        {
            var order = this.orderRepository.FindAll(p => p.OrderId.Equals(orderid) && p.OrderStatus == EnumOrderStatus.ApplyBabyFail).FirstOrDefault();
            if (order == null)
                throw new CustomException(500, "改订单已处理，不能继续操作");
            order.OrderStatus = EnumOrderStatus.RepelApplyBaby;
            order.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = string.Format("拒绝备注：{0}", remark),
                OperationDatetime = DateTime.Now,
                OperationPerson = currentUser.OperatorAccount
            });
            unitOfWorkRepository.PersistUpdateOf(order);
            unitOfWork.Commit();
        }

        /// <summary>
        /// 出票统计
        /// </summary>
        /// <param name="code">商户号</param>
        /// <param name="CreateTime">出票时间-范围:开始</param>
        /// <param name="CreateTimeEnd">出票时间-范围:结束</param>
        /// <returns></returns>
        //[ExtOperationInterceptor("出票统计")]
        public List<ResponseAllTicketSum> TicketSum(string code, DateTime? CreateTime, DateTime? CreateTimeEnd)
        {
            DateTime dateTime = new DateTime(2014, 08, 31, 23, 59, 59);
            IQueryable<Ticket_Buyer> ticketBuyQuery = null;
            IQueryable<TicketSum> ticketQuery = null;
            if (CreateTimeEnd > dateTime)
            {
                ticketBuyQuery = this.ticketRepository.FindAllNoTracking(x => x.TicketState.Equals("出票") && x.CreateDate >= CreateTime.Value && x.CreateDate <= CreateTimeEnd.Value).OfType<Ticket_Buyer>();
                if (!string.IsNullOrEmpty(code))
                    ticketBuyQuery = ticketBuyQuery.Where(x => x.Code.Equals(code));
            }
            else
            {
                ticketQuery = this.ticketSumRepository.FindAllNoTracking(x => x.TicketState.Equals("出票") && x.CreateDate >= CreateTime.Value && x.CreateDate <= CreateTimeEnd.Value);
                if (!string.IsNullOrEmpty(code))
                    ticketQuery = ticketQuery.Where(x => x.Code.Equals(code));
            }
            var list = ticketBuyQuery == null ? ticketQuery.Select(p => new { CreateDate = p.CreateDate, HourDate = p.CreateDate.Hour }).ToList() : ticketBuyQuery.Select(p => new { CreateDate = p.CreateDate, HourDate = p.CreateDate.Hour }).ToList();
            List<TempAllTicketSum> dateGroup = list.GroupBy(x => x.CreateDate.Date).Select(x => new TempAllTicketSum { CreateDate = x.Key }).OrderByDescending(x => x.CreateDate).ToList();
            foreach (var item in dateGroup)
            {
                item.Hour.AddRange(list.Where(p => p.CreateDate.Date == item.CreateDate).Select(x => x.HourDate).Distinct().ToList());
            }

            List<ResponseAllTicketSum> responstList = new List<ResponseAllTicketSum>();
            dateGroup.ForEach(p =>
            {
                ResponseAllTicketSum T = new ResponseAllTicketSum
                {
                    CreateDate = p.CreateDate,
                    total = list.Where(x => x.CreateDate.Date == p.CreateDate).Count(),
                    Zero = list.Where(x => x.HourDate == 0 && x.CreateDate.Date == p.CreateDate).Count(),
                    one = list.Where(x => x.HourDate == 1 && x.CreateDate.Date == p.CreateDate).Count(),
                    two = list.Where(x => x.HourDate == 2 && x.CreateDate.Date == p.CreateDate).Count(),
                    three = list.Where(x => x.HourDate == 3 && x.CreateDate.Date == p.CreateDate).Count(),
                    four = list.Where(x => x.HourDate == 4 && x.CreateDate.Date == p.CreateDate).Count(),
                    five = list.Where(x => x.HourDate == 5 && x.CreateDate.Date == p.CreateDate).Count(),
                    six = list.Where(x => x.HourDate == 6 && x.CreateDate.Date == p.CreateDate).Count(),
                    seven = list.Where(x => x.HourDate == 7 && x.CreateDate.Date == p.CreateDate).Count(),
                    eight = list.Where(x => x.HourDate == 8 && x.CreateDate.Date == p.CreateDate).Count(),
                    nine = list.Where(x => x.HourDate == 9 && x.CreateDate.Date == p.CreateDate).Count(),
                    ten = list.Where(x => x.HourDate == 10 && x.CreateDate.Date == p.CreateDate).Count(),
                    eleven = list.Where(x => x.HourDate == 11 && x.CreateDate.Date == p.CreateDate).Count(),
                    twelve = list.Where(x => x.HourDate == 12 && x.CreateDate.Date == p.CreateDate).Count(),
                    thirteen = list.Where(x => x.HourDate == 13 && x.CreateDate.Date == p.CreateDate).Count(),
                    fourteen = list.Where(x => x.HourDate == 14 && x.CreateDate.Date == p.CreateDate).Count(),
                    fifteen = list.Where(x => x.HourDate == 15 && x.CreateDate.Date == p.CreateDate).Count(),
                    sixteen = list.Where(x => x.HourDate == 16 && x.CreateDate.Date == p.CreateDate).Count(),
                    seventeen = list.Where(x => x.HourDate == 17 && x.CreateDate.Date == p.CreateDate).Count(),
                    eighteen = list.Where(x => x.HourDate == 18 && x.CreateDate.Date == p.CreateDate).Count(),
                    ninteen = list.Where(x => x.HourDate == 19 && x.CreateDate.Date == p.CreateDate).Count(),
                    twenty = list.Where(x => x.HourDate == 20 && x.CreateDate.Date == p.CreateDate).Count(),
                    twenty_one = list.Where(x => x.HourDate == 21 && x.CreateDate.Date == p.CreateDate).Count(),
                    twenty_two = list.Where(x => x.HourDate == 22 && x.CreateDate.Date == p.CreateDate).Count(),
                    twenty_three = list.Where(x => x.HourDate == 23 && x.CreateDate.Date == p.CreateDate).Count()

                };
                responstList.Add(T);
            });
            return responstList;

        }
        public void InitSystemSwitchInfo()
        {
            InitSystemSwitch.Init();
        }
        //[ExtOperationInterceptor("QT记录信息")]
        public QTInfo QTRecord(string pnrCode)
        {
            QTInfo info = new QTInfo();
            var result = this.orderRepository.FindAllNoTracking(p => string.Compare(p.PnrCode, pnrCode, true) == 0);
            var order = result.Where(p => p.OrderStatus == EnumOrderStatus.IssueAndCompleted).FirstOrDefault();
            if (order != null)
            {
                info.CanPNR = true;
                info.OrderID = order.OrderId;
                info.Code = order.BusinessmanCode;
                info.BusinessmanName = order.BusinessmanName;
                List<string> Name = new List<string>();
                order.Passengers.ForEach(x =>
                         Name.Add(x.PassengerName)
                    );
                info.PassengerName = string.Join("|", Name.ToArray());
            }
            else
            {
                info.CanPNR = false;
            }

            return info;

        }

        /// <summary>
        /// 创建航变信息
        /// </summary>
        /// <param name="model"></param>
        public List<AirChange> CreateAirChangeInfo(QTResponse model)
        {
            string _content = "您好！接到航空公司通知，您预订的此编码航班有变动，请及时通知旅客。若有疑问，请咨询航空公司客服（航司电话请在右上角“客规”里查看），谢谢！";
            IBusinessmanRepository _repository = StructureMap.ObjectFactory.GetInstance<IBusinessmanRepository>();
            List<AirChange> list = new List<AirChange>();
            model.QnList.ForEach(y =>
            {
                AirChange airChange = new AirChange();
                var info = QTRecord(y.Pnr);
                airChange.BusinessmanName = info.BusinessmanName;
                airChange.CanPNR = info.CanPNR;
                airChange.PassengerName = info.PassengerName;
                airChange.Code = info.Code;
                airChange.PNR = y.Pnr;
                airChange.OrderId = info.OrderID;
                airChange.QNContent = y.QnResult;
                airChange.CTCT = y.CTCT;
                airChange.QTDate = model.QTDate;
                airChange.QTCount = model.QnCount;
                airChange.OfficeNum = model.Office;
                airChange.CarrierCode = model.Code;
                airChange.CarrayName = _repository.FindAllNoTracking(x => x.Code.Equals(model.Code)).Select(x => x.Name).FirstOrDefault();
                airChange.QTResult = model.QTResult;
                list.Add(airChange);
            });
            return list;
        }
        /// <summary>
        /// 查询航变信息列表
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="PNR"></param>
        /// <param name="Passenger"></param>
        /// <param name="statue"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        //[ExtOperationInterceptor("查询航变信息列表")]
        public PagedList<ResponeAirChange> GetAirChangeList(DateTime? startDate, DateTime? endDate, string startTime, string endTime, string PNR, string Passenger, bool? status, int page, int rows, int i = -1, string CarrayNmae = null)
        {
            string _code = AuthManager.GetCurrentUser().Code;
            var query = ariChangeRepository.FindAllNoTracking();
            if (!string.IsNullOrEmpty(_code))
                query = query.Where(x => x.CarrierCode == _code);
            if (!string.IsNullOrEmpty(PNR) && PNR != "undefined")
                query = query.Where(x => string.Compare(x.PNR, PNR, true) == 0);
            if (!string.IsNullOrEmpty(Passenger) && Passenger != "undefined")
                query = query.Where(x => x.PassengerName.Contains(Passenger));
            if (startDate.HasValue)
                query = query.Where(x => x.QTDate >= startDate);
            if (status.HasValue)
            {
                query = query.Where(x => x.ProcessStatus == status);
            }
            if (endDate.HasValue)
            {
                endDate = endDate.Value.AddDays(1).AddSeconds(-1);
                query = query.Where(x => x.QTDate <= endDate);
            }
            if (!string.IsNullOrEmpty(startTime) && startTime != "undefined")
            {
                int StartTime = int.Parse(startTime);
                query = query.Where(x => x.QTDate.Hour >= StartTime);
            }
            if (!string.IsNullOrEmpty(endTime) && startTime != "undefined")
            {
                int EndTime = int.Parse(endTime);
                query = query.Where(x => x.QTDate.Hour <= EndTime);
            }
            if (i != -1)
            {
                if (i == 0)//自动处理：即自动弹出提醒,手动处理即不是自动弹出的所有
                    query = query.Where(x => x.NotifyWay == EnumAriChangNotifications.AutoPopMessage);
                else
                    query = query.Where(x => x.NotifyWay != EnumAriChangNotifications.AutoPopMessage);
            }
            if (!string.IsNullOrEmpty(CarrayNmae) && CarrayNmae != "undefined")
            {
                query = query.Where(x => x.CarrayName == CarrayNmae);
            }
            var list = query.OrderByDescending(p => p.QTDate).Skip((page - 1) * rows).Take(rows).ToList();

            return new PagedList<ResponeAirChange>()
            {
                Total = query.Count(),
                Rows = AutoMapper.Mapper.Map<List<AirChange>, List<ResponeAirChange>>(list)
            };
        }

        /// <summary>
        /// 发送自动弹出信息
        /// </summary>
        /// <param name="code">商户号</param>
        /// <param name="command">指令</param>
        /// <param name="content">信息内容</param>
        /// <param name="isRepeatSend">是否重复发送</param>
        /// <param name="param"></param>
        public void MsgByBuyerCodes(string content, string[] code = null, bool isRepeatSend = false, object[] param = null)
        {
            MessagePushManager.SendMsgByBuyerCodes(code, EnumPushCommands.MyMessageTip, content, isRepeatSend, param);
        }

        [ExtOperationInterceptor("新建航变协调")]
        public void CreateAirChangeCoordion(EnumAriChangNotifications type, bool status, string content, int Id)
        {
            var info = ariChangeRepository.FindAll(x => x.Id.Equals(Id)).FirstOrDefault();
            var user = AuthManager.GetCurrentUser();
            var model = new AirChangeCoordion
            {
                CreateDate = DateTime.Now,
                Description = content,
                NotifyWay = type,
                ProcessStatus = status,
                OpertorName = user == null ? "系统" : user.OperatorName
            };

            if (info.AriChangeCoordion == null)
                info.AriChangeCoordion = new List<AirChangeCoordion>();
            info.AriChangeCoordion.Add(model);
            info.NotifyWay = type;
            info.ProcessStatus = status;
            unitOfWorkRepository.PersistUpdateOf(info);
            unitOfWork.Commit();
            if (type == EnumAriChangNotifications.AutoPopMessage)
            {
                var message = new MyMessage
                {
                    Code = info.Code,
                    CreateTime = model.CreateDate,
                    Content = content,
                    State = false,
                    QnContent = info.QNContent
                };
                message.Title = message.CreateTime.ToString("yyyy-MM-dd HH:mm") + "_航变通知";
                _unitOfWorkRepository.PersistCreationOf(message);
                _unitOfWork.Commit();
                MsgByBuyerCodes(message.Title, new[] { message.Code }, false, new object[] { message.ID, info.QNContent });
            }

        }
        /// <summary>
        /// QT信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public ResponseAirQtInfo GetQtInfo(int Id)
        {
            var info = ariChangeRepository.FindAllNoTracking(x => x.Id.Equals(Id)).FirstOrDefault();
            return AutoMapper.Mapper.Map<AirChange, ResponseAirQtInfo>(info);
        }

        /// <summary>
        /// 航变查询-获取商户信息
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        public ResponseBusinessMan GetBuseInfo(string Code)
        {
            var info = businessmanRepository.FindAllNoTracking(x => x.Code.Equals(Code)).FirstOrDefault();
            return AutoMapper.Mapper.Map<Businessman, ResponseBusinessMan>(info);
        }

        public List<AirChangeCoordionDto> GetAirChangeCoordion(int Id)
        {
            var pnrInfo = ariChangeRepository.FindAllNoTracking(x => x.Id == Id).FirstOrDefault();

            if (pnrInfo == null)
                return new List<AirChangeCoordionDto>();
            return AutoMapper.Mapper.Map<List<AirChangeCoordion>, List<AirChangeCoordionDto>>(pnrInfo.AriChangeCoordion.ToList());
        }

        /// <summary>
        /// 返回PNR信息
        /// </summary>
        /// <param name="pnr"></param>
        /// <returns></returns>
        public ResponeAirPnrInfo GetPnrInfo(string pnr, int Id)
        {
            var pnrInfo = ariChangeRepository.FindAllNoTracking(x => x.PNR.Equals(pnr) & x.Id == Id)
                .Select(x => new ResponeAirPnrInfo
                {
                    Id = x.Id,
                    QNContent = x.QNContent,
                    RTContent = null,
                    CanPNR = x.CanPNR,
                    AirChangeCoordion = x.AriChangeCoordion.Where(y => y.ProcessStatus == true).Select(y => new AirChangeCoordionDto
                    {
                        CreateDate = y.CreateDate,
                        NotifyWay = y.NotifyWay,
                        OpertorName = y.OpertorName,
                        Description = y.Description
                    }).ToList(),

                }).FirstOrDefault();
            //var pnrInfo = AutoMapper.Mapper.Map<AirChange, ResponeAirPnrInfo>(info);
            var info = ariChangeRepository.FindAllNoTracking(x => x.PNR.Equals(pnr) & x.Id == Id).FirstOrDefault();
            if (info.CanPNR)//-- 系统PNR 根据商户号查
            {
                var rtInfo = pidService.GetPnrAndTickeNumInfo(info.Code, pnr, info.OfficeNum, false, false);
                pnrInfo.RTContent = rtInfo;
            }
            else//-- 非系统PNR 根据运行商号查
            {
                var rtInfo = pidService.GetPnrAndTickeNumInfo(info.CarrierCode, pnr, info.OfficeNum, false, false);
                pnrInfo.RTContent = rtInfo;
            }
            return pnrInfo;
        }

        public ResponseOperateDetail GetOperateDetail(int Id)
        {
            var info = ariChangeRepository.FindAllNoTracking(x => x.Id == Id)
                 .Select(x => new ResponseOperateDetail
                 {
                     CanPNR = x.CanPNR,
                     AirChangeCoordion = x.AriChangeCoordion.Select(y => new AirChangeCoordionDto
                     {
                         CreateDate = y.CreateDate,
                         NotifyWay = y.NotifyWay,
                         OpertorName = y.OpertorName,
                         Description = y.Description
                     }).ToList(),

                 }).FirstOrDefault();
            return info;
        }

        /// <summary>
        /// 最近月份出票量
        /// </summary>
        /// <param name="code"></param>
        /// <param name="months"></param>
        /// <param name="ticketStatus"></param>
        /// <returns></returns>
        public int FindTicketCountByMonth(string code, int months, EnumTicketStatus? ticketStatus = EnumTicketStatus.IssueTicket)
        {
            var query = ticketRepository.FindAllNoTracking().OfType<Ticket_Buyer>().Where(p => p.Code == code);
            var state = ticketStatus.ToEnumDesc();
            query = query.Where(p => p.TicketState == state);
            if (months <= 0) return query.Count();
            var startdate = DateTime.Now.AddMonths(-months);
            query = query.Where(p => p.CreateDate >= startdate);
            query = query.Where(p => p.CreateDate <= DateTime.Now);
            return query.Count();
        }

        /// <summary>
        /// 查询订单处理理由
        /// </summary>
        /// <param name="Refuse"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        public PagedList<ResponseRefund> RefundReasonList(EnumRefuse? Refuse, int page, int rows)
        {
            var query = RefundReasonRepository.FindAllNoTracking();
            if (Refuse.HasValue)
                query = query.Where(x => x.RefuseType == Refuse);
            var list = query.OrderByDescending(p => p.ID).Skip((page - 1) * rows).Take(rows).ToList();

            return new PagedList<ResponseRefund>()
            {
                Total = query.Count(),
                Rows = AutoMapper.Mapper.Map<List<RefundReason>, List<ResponseRefund>>(list)
            };
        }
        /// <summary>
        ///  修改订单处理信息
        /// </summary>
        public void ModifyRefundReason(RequestRefund info)
        {
            var model = RefundReasonRepository.FindAll(p => p.ID == info.ID).FirstOrDefault();
            if (model == null)
                throw new CustomException(500, "查找的信息不存在");
            model.RefuseType = info.RefuseType;
            model.Reason = info.Reason;
            model.RefundType = info.RefundType;
            model.Guid = info.Guid;
            model.CheckItem = info.CheckItem;
            unitOfWorkRepository.PersistUpdateOf(model);
            unitOfWork.Commit();
        }
        /// <summary>
        ///  新建订单处理信息
        /// </summary>
        /// <param name="info"></param>
        public void CreateRefundReason(RequestRefund info)
        {
            var model = new RefundReason()
            {
                RefuseType = info.RefuseType,
                Reason = info.Reason,
                RefundType = info.RefundType,
                Guid = info.Guid,
                CheckItem = info.CheckItem
            };
            unitOfWorkRepository.PersistCreationOf(model);
            unitOfWork.Commit();
        }
        /// <summary>
        /// 查询订单处理信息
        /// </summary>
        /// <param name="Id"></param>
        public void DeleteRefundReason(int Id)
        {
            var model = RefundReasonRepository.FindAll(p => p.ID == Id).FirstOrDefault();
            unitOfWorkRepository.PersistDeletionOf(model);
            unitOfWork.Commit();
        }
    }


}
