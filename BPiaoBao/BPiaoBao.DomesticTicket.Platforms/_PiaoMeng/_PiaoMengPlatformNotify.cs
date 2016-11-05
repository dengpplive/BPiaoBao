using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platforms._PiaoMeng
{
    //GET
    public class _PiaoMengPlatformNotify : BasePlatform, IPlatformNotify
    {
        public override string Code
        {
            get { return "PiaoMeng"; }
        }
        public bool CanProcess(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            bool IsCanProcess = false;
            try
            {
                //通知类型必须和订单号  必须有 
                if (!string.IsNullOrEmpty(nameValueCollection["Status"])
                    && !string.IsNullOrEmpty(nameValueCollection["orderId"])
                     && !string.IsNullOrEmpty(nameValueCollection["areaCity"])
                    )
                {
                    string Status = nameValueCollection["Status"].ToString();
                    if (Status == "10")//支付
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["transNo"]))
                        {
                            IsCanProcess = true;
                        }
                    }
                    else if (Status == "13")//出票
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["name"])
                            && !string.IsNullOrEmpty(nameValueCollection["ticketno"])
                            && nameValueCollection["name"].Split(';').Length == nameValueCollection["ticketno"].Split(';').Length
                            )
                        {
                            IsCanProcess = true;
                        }
                    }
                    else if (Status == "90")//完成退款 完成退费票
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["payfee"])
                            && decimal.Parse(nameValueCollection["payfee"]) > 0
                            )
                        {
                            IsCanProcess = true;
                        }
                    }
                    else if (Status == "99")//交易取消退款 取消出票
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["Comment"]))
                        {
                            IsCanProcess = true;
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
            string responseResult = string.Empty;

            //验证通知信息正常 暂时留着
            if (CanProcess(nameValueCollection))
            {
                string _OrderId = nameValueCollection["orderId"];
                string _Status = nameValueCollection["Status"];
                //判断通知类型           
                if (_Status == "10")//支付
                {
                    #region 支付
                    decimal _TotalPay = 0m;//票盟没有
                    string _TradeID = nameValueCollection["transNo"];
                    if (OnPaid != null)
                    {
                        OnPaid(this, new PaidEventArgs()
                        {
                            OutOrderId = _OrderId,
                            PaidMeony = _TotalPay,
                            SerialNumber = _TradeID,
                            PlatformCode = this.Code
                        });
                    }
                    #endregion
                }
                else if (_Status == "13")//出票
                {
                    #region 出票通知
                    NameValueCollection nvTicketInfo = new NameValueCollection();
                    string[] passengers = nameValueCollection["name"].Split(new string[] { ";" }, StringSplitOptions.None);
                    string[] ticketNumbers = nameValueCollection["ticketno"].Split(new string[] { ";" }, StringSplitOptions.None);
                    if (passengers.Length == ticketNumbers.Length)
                    {
                        string passengerName = string.Empty;
                        string ticketNo = string.Empty;
                        for (int i = 0; i < passengers.Length; i++)
                        {
                            passengerName = passengers[i].ToUpper().Replace("CHD", "").Trim();
                            if (!string.IsNullOrEmpty(passengerName))
                            {
                                ticketNo = ticketNumbers[i].Replace("--", "-").Trim();
                                if (!string.IsNullOrEmpty(ticketNo))
                                    nvTicketInfo.Add(passengerName, ticketNo);
                            }
                        }
                    }
                    if (nvTicketInfo.Count > 0 && OnIssue != null)
                    {
                        OnIssue(this, new IssueEventArgs()
                        {
                            OutOrderId = _OrderId,
                            TicketInfo = nvTicketInfo,
                            PlatformCode = this.Code,
                            Remark = string.Empty
                        });
                    }
                    #endregion
                }
                else if (_Status == "90")//完成退款 完成退费票
                {
                    #region 退款通知
                    string newOrderId = string.Empty;
                    try
                    {
                        //申请退废票升舱生成的新订单号
                        newOrderId = nameValueCollection["neworderId"];
                    }
                    catch (Exception)
                    {
                    }
                    decimal payfee = decimal.Parse(nameValueCollection["payfee"]);
                    if (payfee > 0)
                    {
                        if (OnRefund != null)
                        {
                            OnRefund(this, new PlatformRefundEventArgs()
                            {
                                OutOrderId = _OrderId,
                                RefundMoney = payfee,
                                PlatformCode = this.Code,
                                RefundRemark = newOrderId + "",
                                RefundSerialNumber = "",
                                TotalRefundPoundage = 0,
                                NotifyCollection = nameValueCollection
                            });
                        }
                    }
                    #endregion
                }
                else if (_Status == "99")//交易取消退款 取消出票
                {
                    #region 取消出票
                    string Comment = nameValueCollection["Comment"];
                    //取消出票
                    if (OnCancelIssue != null)
                    {
                        OnCancelIssue(this, new CancelIssueEventArgs()
                        {
                            OutOrderId = _OrderId,
                            Ramark = Comment,
                            PlatformCode = this.Code,
                            NotifyCollection = nameValueCollection
                        });
                    }
                    //全退款
                    if (OnRefund != null)
                    {
                        OnRefund(this, new PlatformRefundEventArgs()
                        {
                            OutOrderId = _OrderId,
                            RefundMoney = 0,
                            PlatformCode = this.Code,
                            RefundRemark = "取消出票全退款",
                            RefundSerialNumber = "",
                            TotalRefundPoundage = 0,
                            NotifyCollection = nameValueCollection
                        });
                    }
                    #endregion
                }
            }

            return responseResult;
        }

        public event PaidEventHandler OnPaid;
        public event IssueEventHandler OnIssue;
        public event CancelIssueHandler OnCancelIssue;
        public event PlatformRefundHandler OnRefund;

    }
}
