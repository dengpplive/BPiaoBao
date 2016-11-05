using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platforms._51book
{
    //GET请求
    public class _51bookPlatformNotify : BasePlatform, IPlatformNotify
    {
        public override string Code
        {
            get { return "51Book"; }
        }
        public bool CanProcess(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            bool IsCanProcess = false;
            try
            {
                if (!string.IsNullOrEmpty(nameValueCollection["type"])
                    && !string.IsNullOrEmpty(nameValueCollection["sequenceNo"])
                    && !string.IsNullOrEmpty(nameValueCollection["areaCity"])
                    )
                {
                    string type = nameValueCollection["type"];
                    string venderRefundTime = string.Empty;
                    bool IssueNotify = true;
                    try
                    {
                        venderRefundTime = nameValueCollection["venderRefundTime"];
                        IssueNotify = false;
                    }
                    catch { }

                    //出票通知
                    if (IssueNotify && type == "1")
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["passengerNames"])
                            && !string.IsNullOrEmpty(nameValueCollection["ticketNos"])
                            && nameValueCollection["passengerNames"].Split(',').Length == nameValueCollection["ticketNos"].Split(',').Length
                            )
                        {
                            IsCanProcess = true;
                        }
                    }
                    else
                    {
                        //退票通知2.0版本
                        if (!string.IsNullOrEmpty(nameValueCollection["venderRefundTime"])
                            && !string.IsNullOrEmpty(nameValueCollection["orderNo"])
                            )
                        {
                            if (type == "1")
                            {
                                //退款成功
                                if (!string.IsNullOrEmpty(nameValueCollection["venderPayPrice"])
                                    && !string.IsNullOrEmpty(nameValueCollection["refundFee"])
                                    && decimal.Parse(nameValueCollection["venderPayPrice"]) > 0
                                    )
                                {

                                    IsCanProcess = true;
                                }
                            }
                            else
                            {
                                //退款失败
                                if (!string.IsNullOrEmpty(nameValueCollection["venderRemark"]))
                                {
                                    IsCanProcess = true;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
            }
            return IsCanProcess;
        }

        public string Process(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            string responseResult = "F";
            if (CanProcess(nameValueCollection))
            {
                string type = nameValueCollection["type"];
                string OrderId = nameValueCollection["sequenceNo"];//接口订单号
                string venderRefundTime = string.Empty;
                bool IssueNotify = true;
                try
                {
                    venderRefundTime = nameValueCollection["venderRefundTime"];
                    IssueNotify = false;
                }
                catch { }

                //出票通知 
                if (IssueNotify && type == "1")
                {
                    #region  出票通知
                    NameValueCollection nvTicketInfo = new NameValueCollection();
                    string[] passengerNames = nameValueCollection["passengerNames"].Split(',');
                    string[] ticketNos = nameValueCollection["ticketNos"].Split(',');
                    if (passengerNames.Length == ticketNos.Length)
                    {
                        string passengerName = string.Empty;
                        string ticketNo = string.Empty;
                        for (int i = 0; i < passengerNames.Length; i++)
                        {
                            passengerName = passengerNames[i].ToUpper().Replace("CHD", "").Trim();
                            if (!string.IsNullOrEmpty(passengerName))
                            {
                                ticketNo = ticketNos[i].Replace("--", "-").Trim();
                                if (!string.IsNullOrEmpty(ticketNo))
                                    nvTicketInfo.Add(passengerName, ticketNo);
                            }
                        }
                    }
                    if (nvTicketInfo.Count > 0 && OnIssue != null)
                    {
                        OnIssue(this, new IssueEventArgs()
                        {
                            OutOrderId = OrderId,
                            TicketInfo = nvTicketInfo,
                            PlatformCode = this.Code,
                            Remark = string.Empty
                        });
                    }
                    #endregion
                }
                else
                {
                    #region 退票通知2.0版本
                    if (!string.IsNullOrEmpty(nameValueCollection["venderRefundTime"])
                        && !string.IsNullOrEmpty(nameValueCollection["orderNo"])
                        )
                    {
                        if (type == "1")
                        {
                            //退款成功
                            decimal venderPayPrice = decimal.Parse(nameValueCollection["venderPayPrice"]);//退款金额
                            //退款手续费 
                            decimal refundFee = decimal.Parse(nameValueCollection["refundFee"]);
                            if (OnRefund != null)
                            {
                                OnRefund(this, new PlatformRefundEventArgs()
                                {
                                    OutOrderId = OrderId,
                                    RefundMoney = venderPayPrice,
                                    PlatformCode = this.Code,
                                    RefundRemark = "退款时间:" + venderRefundTime,
                                    RefundSerialNumber = "",
                                    RefundType = "退票",
                                    TotalRefundPoundage = refundFee,
                                    NotifyCollection = nameValueCollection
                                });
                            }

                            //退款成功返回"S"不在发送通知
                            responseResult = "S";
                        }
                        else
                        {
                            //退款失败
                            string venderRemark = nameValueCollection["venderRemark"];//失败备注
                        }
                    }
                    #endregion
                }
                responseResult = "S";
            }
            return responseResult;
        }

        public event PaidEventHandler OnPaid;
        public event IssueEventHandler OnIssue;
        public event CancelIssueHandler OnCancelIssue;
        public event PlatformRefundHandler OnRefund;

    }
}
