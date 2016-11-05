using BPiaoBao.DomesticTicket.Domain.Models.Orders.States;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;
using JoveZhao.Framework;
using BPiaoBao.Common.Enums;
using StructureMap;
using BPiaoBao.DomesticTicket.Domain.Services.Insurance;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders.Behaviors
{
    /// <summary>
    /// 出票操作
    /// </summary>
    [Behavior("TicketsIssue")]
    public class TicketsIssueBehavior : BaseOrderBehavior
    {
        public override object Execute()
        {
            //出票字符串
            var ticketDict = getParame("ticketDict") as Dictionary<string, string>;
            //出票操作人
            string operatorName = getParame("operatorName").ToString();
            string platformCode = getParame("platformCode").ToString();
            string opratorSource = getParame("opratorSource").ToString();
            string remark = string.Empty;
            try
            {
                remark = getParame("remark").ToString();
                if (platformCode == "517")
                {
                    remark = remark + " 改期收回代理费";
                }
            }
            catch
            {
            }
            StringBuilder sbSucces = new StringBuilder();
            StringBuilder sbFail = new StringBuilder();
            bool tags = false;
            order.Passengers.ForEach(p =>
            {
                if ((ticketDict.ContainsKey(p.PassengerName) && !string.IsNullOrEmpty(ticketDict[p.PassengerName].ToString()))
                    || (ticketDict.ContainsKey(p.PassengerName.ToUpper()) && !string.IsNullOrEmpty(ticketDict[p.PassengerName.ToUpper()].ToString()))
                    || (ticketDict.ContainsKey(p.PassengerName.ToLower()) && !string.IsNullOrEmpty(ticketDict[p.PassengerName.ToLower()].ToString()))
                    || ticketDict.ContainsKey(p.PassengerName.Replace(" ", "")) && !string.IsNullOrEmpty(ticketDict[p.PassengerName.Replace(" ", "")].ToString())
                    || ticketDict.ContainsKey(p.PassengerName.ToUpper() + "CHD") && !string.IsNullOrEmpty(ticketDict[p.PassengerName.ToUpper() + "CHD"].ToString())
                    )
                {
                    if (ticketDict.ContainsKey(p.PassengerName))
                    {
                        p.TicketNumber = ticketDict[p.PassengerName];
                    }
                    if (string.IsNullOrEmpty(p.TicketNumber)
                        && ticketDict.ContainsKey(p.PassengerName.Replace(" ", "")))
                    {
                        p.TicketNumber = ticketDict[p.PassengerName.Replace(" ", "")];
                    }
                    if (string.IsNullOrEmpty(p.TicketNumber)
                       && ticketDict.ContainsKey(p.PassengerName.ToUpper())
                        )
                    {
                        p.TicketNumber = ticketDict[p.PassengerName.ToUpper()];
                    }
                    if (string.IsNullOrEmpty(p.TicketNumber)
                      && ticketDict.ContainsKey(p.PassengerName.ToLower())
                       )
                    {
                        p.TicketNumber = ticketDict[p.PassengerName.ToLower()];
                    }
                    if (string.IsNullOrEmpty(p.TicketNumber)
                        && ticketDict.ContainsKey(p.PassengerName.ToUpper() + "CHD"))
                    {
                        p.TicketNumber = ticketDict[p.PassengerName.ToUpper() + "CHD"];
                    }
                    p.TicketNumber = p.TicketNumber.Replace("-", "");
                    if (string.IsNullOrEmpty(p.TicketNumber))
                    {
                        tags = true;
                        sbFail.Append("乘客：" + p.PassengerName + " 票号:" + p.TicketNumber + "失败");
                    }
                    else
                    {
                        p.TicketStatus = EnumTicketStatus.IssueTicket;
                        sbSucces.Append("乘客：" + p.PassengerName + " 票号:" + p.TicketNumber + "成功");
                    }
                }
                else
                {
                    tags = true;
                    sbFail.Append("乘客：" + p.PassengerName + " 票号:" + p.TicketNumber + "失败");
                }
            });


            //已经出票 交易结束
            if (tags)
            {
                ticketDict.ForEach(p =>
                {
                    sbFail.Append("乘客：" + p.Key + " 票号:" + p.Value);
                });
                order.WriteLog(new OrderLog()
                {
                    OperationContent = (!string.IsNullOrEmpty(opratorSource) ? "来源:" + opratorSource : "") + opratorSource + ",出票时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "," + platformCode + "复合票号失败" + sbFail.ToString(),
                    OperationDatetime = DateTime.Now,
                    OperationPerson = operatorName,
                    IsShowLog = false,
                    Remark = remark
                });
                order.WriteLog(new OrderLog()
                {
                    OperationContent = "复合票号失败" + sbFail.ToString(),
                    OperationDatetime = DateTime.Now,
                    OperationPerson = operatorName,
                    IsShowLog = true,
                    Remark = remark
                });
                order.ChangeStatus(EnumOrderStatus.AutoIssueFail);
            }
            else
            {
                order.IssueTicketTime = System.DateTime.Now;
                order.ChangeStatus(EnumOrderStatus.IssueAndCompleted);
                //日志
                order.WriteLog(new OrderLog()
                {
                    OperationContent = (!string.IsNullOrEmpty(opratorSource) ? "来源:" + opratorSource + "," : "") + "出票时间:" + order.IssueTicketTime.Value.ToString("yyyy-MM-dd HH:mm:ss") + "," + platformCode + "出票成功",
                    OperationDatetime = DateTime.Now,
                    OperationPerson = operatorName
                    ,
                    IsShowLog = false,
                    Remark = remark
                });
                order.WriteLog(new OrderLog()
                {
                    OperationContent = "出票成功",
                    OperationDatetime = DateTime.Now,
                    OperationPerson = operatorName
                    ,
                    IsShowLog = true,
                    Remark = remark
                });
                Logger.WriteLog(LogType.INFO, "出票成功" + sbSucces.ToString());
            }



            return null;
        }
    }
}
