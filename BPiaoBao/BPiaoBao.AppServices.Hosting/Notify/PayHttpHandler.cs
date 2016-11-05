using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using JoveZhao.Framework.Expand;
using JoveZhao.Framework.DDD;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using StructureMap;
using BPiaoBao.Common;
using JoveZhao.Framework.HttpServers;
using JoveZhao.Framework;
using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using System.Collections.Specialized;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;
using BPiaoBao.DomesticTicket.Domain.Services.Insurance;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;

namespace BPiaoBao.AppServices.Hosting.Notify
{
    [HttpCode("ReturnPay")]
    public class PayHttpHandler : IHttpHandler
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        IOrderRepository orderRepository = ObjectFactory.GetInstance<IOrderRepository>();
        IAfterSaleOrderRepository afterSaleOrderRepository = ObjectFactory.GetInstance<IAfterSaleOrderRepository>();
        IBusinessmanRepository businessmanRepository = ObjectFactory.GetInstance<IBusinessmanRepository>();
        public void Process(HttpRequest request, HttpResponse writer)
        {
            System.Threading.Thread.Sleep(2000);
            //日志
            StringBuilder sbLog = new StringBuilder();
            try
            {
                sbLog.Append("请求参数:\r\n");
                NameValueCollection nv = new NameValueCollection();
                nv.Add(request.Form);
                nv.Add(request.QueryString);
                foreach (string key in nv.Keys)
                {
                    sbLog.Append(key + "=" + nv[key] + "\r\n");
                }
                sbLog.Append("处理结果:");

                bool rs = false;
                string PayWayDiscription = "";
                EnumPayMethod? payMethod = null;
                string payWay = request.QueryString["payWay"]; // 充值方式 
                 string bankCode = request.QueryString["bankCode"]; // 银行Code
                string orderId = request.QueryString["orderId"];//订单编号
                string payNo = request.QueryString["payNo"];//交易号
                string price = request.QueryString["price"];//交易金额
                string currentTime = request.QueryString["currentTime"];
                string isRefund = request.QueryString["isRefund"];//退款标志(0=支付1=退款)
                string signature = request.QueryString["signature"];
                string remark = request.QueryString["remark"];
                if (isRefund == "1" && remark.Contains("SaleOrderRefund_"))
                {
                    /*退票单ID:SaleOrderRefund_xx
                     * 退款单ID:orderID
                     * price:退款金额
                     */
                    int saleorderid = remark.Split('_')[1].ToInt();
                    if (SaleOrderRefund(saleorderid, price, orderId))
                        writer.WriteLine("success");
                    return;
                }
                List<string> list = new List<string>();
                list.Add(string.Format("payWay={0}", payWay));
                list.Add(string.Format("bankCode={0}", bankCode));
                list.Add(string.Format("orderId={0}", orderId));
                list.Add(string.Format("payNo={0}", payNo));
                list.Add(string.Format("price={0}", price));
                list.Add(string.Format("currentTime={0}", currentTime));
                list.Add(string.Format("isRefund={0}", isRefund));
                list.Add(string.Format("remark={0}", remark.UrlEncode()));
                var newlist = list.OrderByDescending(p => p);
                string data = string.Join("&", newlist);
                string sign = data.Md5();
                // AliPay = 20,ChinaPnrPay = 21,TenPay = 22,BillPay = 23
                switch (payWay.ToLower())
                {
                    case "alipay":
                        payMethod = EnumPayMethod.AliPay;
                        PayWayDiscription = "支付宝";
                        break;
                    //case "ChinaPnrPay":
                    //    payWay = 3;
                    //    PayWayDiscription = "汇付";
                    //    break;
                    case "tenpay":
                        payMethod = EnumPayMethod.TenPay;
                        PayWayDiscription = "财付通";
                        break;
                    //case "BillPay":
                    //    payWay = 2;
                    //    PayWayDiscription = "快钱";
                    //    break;
                    case "internetbank":
                        payMethod = EnumPayMethod.Bank; 
                        PayWayDiscription = "银行卡";
                        payWay = bankCode;
                        break;
                }
                if (sign == signature)
                {
                    var order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
                    if (isRefund == "1")//退款
                    {
                        //退款中的状态 修改成 退款结束
                        if (order.OrderStatus == EnumOrderStatus.RepelIssueRefunding || order.OrderStatus == EnumOrderStatus.WaitReimburseWithRepelIssue)
                        {
                            //修改为拒绝出票，订单完成
                            order.ChangeStatus(EnumOrderStatus.RepelIssueAndCompleted);
                            order.WriteLog(new OrderLog()
                            {
                                OperationContent = "日志来源:通知,订单状态:" + order.OrderStatus.ToEnumDesc() + " 退款交易号:" + payNo + ",退款状态：退款完成,退款方式:" + PayWayDiscription,
                                OperationDatetime = System.DateTime.Now,
                                OperationPerson = remark,
                                IsShowLog = true
                            });
                            unitOfWorkRepository.PersistUpdateOf(order);
                            unitOfWork.Commit();
                            sbLog.Append("退款处理\r\n");
                        }

                    }
                    else if (isRefund == "0")//支付
                    {
                        if (order.OrderPay.PayStatus == EnumPayStatus.NoPay &&
                            (order.OrderStatus == EnumOrderStatus.NewOrder || order.OrderStatus == EnumOrderStatus.PaymentInWaiting)
                            )
                        {
                            if (payMethod.HasValue)
                            {
                                order.OrderPay.PayMethod = payMethod.Value;
                            }
                            //订单支付通知                         
                            order.PayToPaid(remark, order.OrderPay.PayMethod, payWay, payNo, "通知");
                            unitOfWorkRepository.PersistUpdateOf(order);
                            unitOfWork.Commit();
                            //生成接口订单和代付
                            DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
                            //支付宝通知回来自动出票
                            if (order.Policy.PolicySourceType != EnumPolicySourceType.Interface)
                                if (order.OrderPay.PayMethod != null && (order.OrderPay.PayMethod == EnumPayMethod.Bank || order.OrderPay.PayMethod == EnumPayMethod.Platform))
                                {
                                    domesticService.AutoIssue(order.OrderId, order.OrderPay.PayMethod.ToEnumDesc() + "支付", () =>
                                    {
                                        try
                                        {
                                            MessageQueueManager.SendMessage(orderId, 0);
                                        }
                                        catch (Exception e)
                                        {
                                            Logger.WriteLog(LogType.ERROR, string.Format("{0}:写入总表失败", order.OrderId), e);
                                        }
                                    });
                                }

                            domesticService.CreatePlatformOrderAndPaid(order.OrderId, "系统", "支付通知");
                        }
                        else if (order.OrderPay.PayStatus == EnumPayStatus.NoPay && order.OrderStatus == EnumOrderStatus.OrderCanceled)
                        {
                            order.OrderPay.PayStatus = EnumPayStatus.OK;
                            order.OrderPay.PaySerialNumber = payNo;
                            order.OrderPay.PayDateTime = DateTime.Now;
                            order.OrderStatus = EnumOrderStatus.RepelIssueAndCompleted;
                            order.WriteLog(new OrderLog
                            {
                                IsShowLog = true,
                                OperationContent = "该订单已过支付时间,退回支付金额",
                                OperationDatetime = DateTime.Now,
                                OperationPerson = "系统"
                            });
                            unitOfWorkRepository.PersistUpdateOf(order);
                            unitOfWork.Commit();
                            MessageQueueManager.SendMessage(order.OrderId, 2);
                            AutoRefund(order);


                        }
                        sbLog.Append("支付处理\r\n");
                    }
                    else
                    {
                        sbLog.Append("未处理\r\n");
                    }
                    writer.WriteLine("success");
                }
                else
                {
                    sbLog.Append("签名验证失败\r\n");
                }
            }
            catch (Exception ex)
            {
                sbLog.Append(ex.Message + "\r\n");
            }
            finally
            {
                //日志
                WriteLog(sbLog.ToString(), request != null ? request.Url.ToString() : "异常");
            }
        }
        private void AutoRefund(Order order)
        {
            IPaymentClientProxy client = new CashbagPaymentClientProxy();
            var businessman = this.businessmanRepository.FindAll(x => x.Code == order.BusinessmanCode).FirstOrDefault();
            string payType = order.OrderPay.PayMethod.ToEnumDesc();
            string BuyDesc = (order.CreateTime < DateTime.Parse("2014/07/19 04:10:00")) ? OldGetRefundDetail(order, order.OrderMoney) : GetRefundDetail(order, order.OrderMoney);
            if (payType == EnumPayMethod.Bank.ToEnumDesc() || payType == EnumPayMethod.Platform.ToEnumDesc())
            {
                client.Reimburse(businessman.CashbagCode, businessman.CashbagKey, order.OrderPay.PaySerialNumber, order.OrderMoney, order.OrderId, "REFUND", BuyDesc);
            }
            else
            {
                client.Reimburse(businessman.CashbagCode, businessman.CashbagKey, order.OrderPay.PaySerialNumber, order.OrderMoney, order.OrderId, "退款", BuyDesc);

            }
        }
        private string GetRefundDetail(Order order, decimal refundMoney)
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
        private string OldGetRefundDetail(Order order, decimal refundMoney)
        {
            List<string> refundList = new List<string>();
            List<PayBillDetail> PayBillDetails = order.OrderPay.PayBillDetails;
            if (order.Policy.PolicySourceType == EnumPolicySourceType.Interface)
            {
                var payBillList = order.OrderPay.PayBillDetails.Select(p => new { OpType = p.OpType, Money = p.Money }).ToList();
                //运营商分润
                var carrierFR = payBillList.Where(p => p.OpType == EnumOperationType.Profit).Select(p => p.Money).FirstOrDefault();
                //运营商服务费
                var carrierFW = payBillList.Where(p => p.OpType == EnumOperationType.Receivables).Select(p => p.Money).FirstOrDefault();
                if ((carrierFR + carrierFW) != 0)
                {
                    string args = string.Format("{0}^{1}^分润退款", order.Policy.CashbagCode, (carrierFR + carrierFW));
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
        //记录日志
        private void WriteLog(string Content, string Url)
        {
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("Start==========================ReturnPay通知===========================================\r\n\r\n");
            sbLog.Append("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n");
            sbLog.Append("URL:" + Url + "\r\n");
            sbLog.AppendFormat("内容:{0}\r\n", Content);
            sbLog.Append("End=====================================================================\r\n\r\n");
            Logger.WriteLog(LogType.INFO, sbLog.ToString());
        }
        private bool SaleOrderRefund(int saleorderid, string money, string refundid)
        {
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid).FirstOrDefault();
            if (model == null)
            {
                WriteLog(string.Format("售后订单ID:{0}不存在", saleorderid), string.Empty);
                return false;

            }


            if (model.ProcessStatus != EnumTfgProcessStatus.Refunding)
            {
                WriteLog(string.Format("当前订单状态不是可完成状态，订单ID:{0},当时订单状态{1}", saleorderid, model.ProcessStatus.ToEnumDesc()), string.Empty);
                return false;
            }

            if (model is AnnulOrder || model is BounceOrder)
            {
                if (model is AnnulOrder)
                {
                    AnnulOrder annulOrder = model as AnnulOrder;
                    var list = annulOrder.BounceLines.ToList();
                    var bline = list.Where(p => p.ID == refundid).FirstOrDefault();
                    bline.RefundTime = DateTime.Now;
                    bline.Status = EnumBoundRefundStatus.Refunded;
                    if (list.All(p => p.Status == EnumBoundRefundStatus.Refunded))
                    {

                        annulOrder.Passenger.ForEach(n => n.Status = EnumTfgPassengerStatus.AnnulTicketed);
                        annulOrder.ProcessStatus = EnumTfgProcessStatus.Processed;
                        annulOrder.CompletedTime = DateTime.Now;
                        annulOrder.LockCurrentAccount = string.Empty;
                    }

                }
                else if (model is BounceOrder)
                {
                    BounceOrder bounceOrder = model as BounceOrder;
                    var list = bounceOrder.BounceLines.ToList();
                    var bline = list.Where(p => p.ID == refundid).FirstOrDefault();
                    bline.RefundTime = DateTime.Now;
                    bline.Status = EnumBoundRefundStatus.Refunded;
                    if (list.All(p => p.Status == EnumBoundRefundStatus.Refunded))
                    {
                        bounceOrder.Passenger.ForEach(n => n.Status = EnumTfgPassengerStatus.Refunded);
                        bounceOrder.ProcessStatus = EnumTfgProcessStatus.Processed;
                        bounceOrder.CompletedTime = DateTime.Now;
                        bounceOrder.LockCurrentAccount = string.Empty;
                    }
                }
                model.WriteLog(new OrderLog
                {
                    IsShowLog = true,
                    OperationContent = "订单退款完成,交易结束",
                    OperationDatetime = DateTime.Now,
                    OperationPerson = "系统"
                });
                unitOfWorkRepository.PersistUpdateOf(model);
                unitOfWork.Commit();
                if (model.ProcessStatus == EnumTfgProcessStatus.Processed)
                {
                    MessageQueueManager.SendMessage(model.Id.ToString(), 1);
                }
                return true;
            }
            WriteLog(string.Format("该订单不是退票单或者废票单"), string.Empty);
            return false;

        }
    }
}
