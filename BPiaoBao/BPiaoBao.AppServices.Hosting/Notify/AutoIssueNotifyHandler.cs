using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.DomesticTicket.Domain.Services.B2BParam;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.HttpServers;
using StructureMap;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;

namespace BPiaoBao.AppServices.Hosting.Notify
{
    [HttpCode("AutoIssueNotify")]
    public class AutoIssueNotifyHandler : IHttpHandler
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        IOrderRepository orderRepository = ObjectFactory.GetInstance<IOrderRepository>();
        AutoTicketService autoTicketService = ObjectFactory.GetInstance<AutoTicketService>();
        public void Process(HttpRequest request, HttpResponse response)
        {
            StringBuilder sbLog = new StringBuilder();
            string result = "false";
            try
            {
                sbLog.Append("Start================自动出票通知=====================================================\r\n");
                sbLog.Append("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n");
                sbLog.Append("内容:\r\n");
                NameValueCollection nv = new NameValueCollection();
                nv.Add(request.Form);
                nv.Add(request.QueryString);
                string ticketXML = string.Empty;
                string payXML = string.Empty;
                foreach (string key in nv.Keys)
                {
                    sbLog.Append(key + "=" + nv[key] + "\r\n");
                    if (key == "ticketnoinfo")
                    {
                        ticketXML = nv[key];
                    }
                    else if (key == "paymentinfo")
                    {
                        payXML = nv[key];
                    }
                }
                if (!string.IsNullOrEmpty(ticketXML))
                {
                    #region  //B2B自动出票通知
                    B2BResponse b2BResponse = autoTicketService.SyncTicketCall(ticketXML);
                    List<AutoTicketInfo> b2bList = b2BResponse.TicketNofityInfo.AutoTicketList;
                    if (b2bList.Count > 0)
                    {
                        string orderId = b2BResponse.TicketNofityInfo.FlatformOrderId;
                        Order order = orderRepository.FindAll(p => p.OrderId == orderId).FirstOrDefault();
                        if (order != null && order.OrderStatus != EnumOrderStatus.IssueAndCompleted)
                        {
                            Dictionary<string, string> ticketDict = new Dictionary<string, string>();
                            foreach (AutoTicketInfo item in b2bList)
                            {
                                if (!ticketDict.ContainsKey(item.PassengerName))
                                {
                                    ticketDict.Add(item.PassengerName, item.TicketNumber);
                                }
                            }
                            var behavior = order.State.GetBehaviorByCode("TicketsIssue");
                            behavior.SetParame("ticketDict", ticketDict);
                            behavior.SetParame("operatorName", "系统");
                            behavior.SetParame("platformCode", "系统");
                            behavior.SetParame("opratorSource", "通知");
                            behavior.SetParame("remark", order.Policy.PolicyType + "自动出票");
                            behavior.Execute();
                            unitOfWorkRepository.PersistUpdateOf(order);
                            unitOfWork.Commit();
                            result = "true";
                            sbLog.Append("处理成功\r\n");
                            try
                            {
                                MessageQueueManager.SendMessage(orderId, 0);
                            }
                            catch (Exception e)
                            {
                                Logger.WriteLog(LogType.ERROR, string.Format("{0}:写入总表失败", order.OrderId), e);
                            }
                        }
                        else
                        {
                            sbLog.AppendFormat("未找到订单号:{0}\r\n", orderId);
                        }
                    }
                    else
                    {
                        sbLog.AppendFormat("{0}\r\n", b2BResponse.Remark);
                    }
                    #endregion
                }
                else if (!string.IsNullOrEmpty(payXML))
                {
                    //B2B自动支付通知
                    result = "true";
                }
            }
            catch (Exception e)
            {
                sbLog.Append("异常信息:" + e.Message + e.StackTrace + "\r\n");
            }
            finally
            {
                sbLog.Append("End=====================================================================\r\n\r\n");
                new CommLog().WriteLog("B2BNotify", sbLog.ToString());
                //回复数据 true false
                response.WriteLine(result);
            }
        }
    }
}
