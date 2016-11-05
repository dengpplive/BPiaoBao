using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.HttpServers;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;

namespace BPiaoBao.AppServices.Hosting.Notify
{
    /// <summary>
    /// 改签支付，退款异步通知
    /// </summary>
    [HttpCode("SaleNotify")]
    public class SaleNotifyHttpHandler : IHttpHandler
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        IAfterSaleOrderRepository afterSaleOrderRepository = ObjectFactory.GetInstance<IAfterSaleOrderRepository>();
        public void Process(HttpRequest request, HttpResponse response)
        {

            Logger.WriteLog(LogType.INFO, request.Url.ToString());
            string orderid = request.QueryString["orderId"];
            string outPayNo = request.QueryString["payNo"];
            string remark = request.QueryString["remark"];
            string price = request.QueryString["price"];//交易金额
            string currentTime = request.QueryString["currentTime"];
            string isRefund = request.QueryString["isRefund"];//退款标志(0=支付1=退款)
            string signature = request.QueryString["signature"];
            string payway = request.QueryString["payWay"];//支付方式
            int saleorderid = orderid.Substring(2).ToInt();
            if (isRefund == "1")
            {
                int refundId = remark.ToInt();
                /*
                 *orderid:退款单ID
                 *remark:退票单ID
                 *price:金额
                 */
                if (SaleOrderRefund(refundId, price, orderid))
                {
                    response.WriteLine("success");
                }
                return;
            }
            var model = afterSaleOrderRepository.FindAll(p => p.Id == saleorderid).FirstOrDefault();
            if (model == null)
            {
                Logger.WriteLog(LogType.INFO, string.Format("改签支付售后订单ID:{0}不存在", saleorderid));
                return;
            }
            ChangeOrder changeOrder = null;
            if (model is ChangeOrder)
                changeOrder = model as ChangeOrder;
            if (changeOrder == null)
            {
                Logger.WriteLog(LogType.INFO, string.Format("售后订单ID:{0}不是改签类型", saleorderid));
                return;
            }
            if (changeOrder.ProcessStatus != EnumTfgProcessStatus.ProcessingWaitPay)
                return;
            changeOrder.OutPayNo = outPayNo;
            changeOrder.PayTime = DateTime.Now;
            changeOrder.PayStatus = EnumChangePayStatus.Payed;
            changeOrder.ProcessStatus = EnumTfgProcessStatus.WaitIssue;
            changeOrder.PayWay = ExtHelper.GetPayMethod(payway);
            changeOrder.WriteLog(new OrderLog
            {
                IsShowLog = true,
                OperationContent = "订单改签支付完成,等待出票",
                OperationDatetime = DateTime.Now,
                OperationPerson = "系统"
            });
            unitOfWorkRepository.PersistUpdateOf(changeOrder);
            unitOfWork.Commit();
            response.WriteLine("success");
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saleorderid">退票单ID</param>
        /// <param name="money">退款金额</param>
        /// <param name="refundid">退款单ID</param>
        /// <returns></returns>
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


                    if (list.Where(p => p.Status == EnumBoundRefundStatus.Refunding || p.Status == EnumBoundRefundStatus.NoRefund).Count() == 0)
                    {
                        annulOrder.Passenger.ForEach(n => n.Status = EnumTfgPassengerStatus.AnnulTicketed);
                        annulOrder.ProcessStatus = EnumTfgProcessStatus.Processed;
                        annulOrder.LockCurrentAccount = string.Empty;
                        annulOrder.CompletedTime = DateTime.Now;

                    }

                }
                else if (model is BounceOrder)
                {
                    BounceOrder bounceOrder = model as BounceOrder;
                    var list = bounceOrder.BounceLines.ToList();
                    var bline = list.Where(p => p.ID == refundid).FirstOrDefault();
                    bline.RefundTime = DateTime.Now;
                    bline.Status = EnumBoundRefundStatus.Refunded;
                    bline.ChangeOrder.RefundMoney += money.ToDecimal();
                    var sigModel = bline.ChangeOrder.Passenger.Where(y => y.Passenger.PassengerName == bline.PassgenerName).FirstOrDefault();
                    if (sigModel != null)
                        sigModel.IsRefund = true;
                    if (list.Where(p => p.Status == EnumBoundRefundStatus.Refunding).Count() == 0)
                    {
                        bounceOrder.Passenger.ForEach(n => n.Status = EnumTfgPassengerStatus.Refunded);
                        bounceOrder.ProcessStatus = EnumTfgProcessStatus.Processed;
                        bounceOrder.LockCurrentAccount = string.Empty;
                        bounceOrder.CompletedTime = DateTime.Now;
                    }
                }
                model.WriteLog(new OrderLog
                {
                    IsShowLog = true,
                    OperationContent = "售后订单退款完成,交易结束",
                    OperationDatetime = DateTime.Now,
                    OperationPerson = "系统"
                });
                unitOfWorkRepository.PersistUpdateOf(model);
                unitOfWork.Commit();
                if (model.ProcessStatus == EnumTfgProcessStatus.Processed)
                {
                    //领域事件数据
                    MessageQueueManager.SendMessage(model.Id.ToString(), 1);
                }
                return true;
            }
            WriteLog(string.Format("该订单不是退票单或者废票单"), string.Empty);
            return false;
        }
    }
}
