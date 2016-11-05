using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platforms._8000YI
{
    //POST
    public class _8000YIPlatformNotify : BasePlatform, IPlatformNotify
    {
        public override string Code
        {
            get { return "8000YI"; }
        }
        public bool CanProcess(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            bool IsCanProcess = false;
            try
            {
                if (!string.IsNullOrEmpty(nameValueCollection["type"])
                    && !string.IsNullOrEmpty(nameValueCollection["orderguid"])
                    && !string.IsNullOrEmpty(nameValueCollection["key"])
                    && !string.IsNullOrEmpty(nameValueCollection["notifymsg"])
                    && !string.IsNullOrEmpty(nameValueCollection["areaCity"])
                    )
                {
                    string type = nameValueCollection["type"];
                    string key = nameValueCollection["key"];
                    string orderguid = nameValueCollection["orderguid"];
                    string notifymsg = System.Web.HttpUtility.UrlDecode(nameValueCollection["notifymsg"]);
                    string localKey = string.Empty;
                    //出票通知
                    if (type == "2")
                    {
                        localKey = string.Format("$%^{0}{1}|8000YI$8000yi$", orderguid, notifymsg);
                    }
                    else //其他
                    {
                        localKey = string.Format("$%^{0}{1}8000YI$8000yi$", orderguid, notifymsg);
                    }
                    localKey = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(localKey, "MD5");
                    if (key.Equals(localKey))
                    {
                        if (type == "1")
                        {
                            if (decimal.Parse(notifymsg.Split('^')[4].ToString()) > 0)
                            {
                                IsCanProcess = true;
                            }
                        }
                        else
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

            if (CanProcess(nameValueCollection))
            {
                string type = nameValueCollection["type"];
                string orderguid = nameValueCollection["orderguid"];
                string notifymsg = System.Web.HttpUtility.UrlDecode(nameValueCollection["notifymsg"]);
                string[] notifymsgs = notifymsg.Split('^');
                //支付通知
                if (type == "1")
                {
                    #region 支付通知
                    //notifymsg =I635220850525394286^2013120829050843^JQZH2R^2013-12-08 07:33:25^702.00 
                    if (notifymsgs.Length >= 5)
                    {
                        string _SerialNumber = notifymsgs[1];
                        decimal _PaidMeony = decimal.Parse(notifymsgs[4]);
                        if (OnPaid != null)
                        {
                            OnPaid(this, new PaidEventArgs()
                            {
                                OutOrderId = orderguid,
                                PaidMeony = _PaidMeony,
                                SerialNumber = _SerialNumber,
                                PlatformCode = this.Code
                            });
                        }
                    }
                    #endregion
                }
                //出票通知
                else if (type == "2")
                {
                    #region  出票通知
                    NameValueCollection nvTicketInfo = new NameValueCollection();
                    if (notifymsgs.Length >= 4
                        && !string.IsNullOrEmpty(notifymsgs[1])
                        && !string.IsNullOrEmpty(notifymsgs[2])
                        )
                    {
                        string[] passengers = notifymsgs[1].Split('|');
                        string[] ticketNumbers = notifymsgs[2].Split('|');
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
                    }
                    if (nvTicketInfo.Count > 0 && OnIssue != null)
                    {
                        OnIssue(this, new IssueEventArgs()
                        {
                            OutOrderId = orderguid,
                            TicketInfo = nvTicketInfo,
                            PlatformCode = this.Code,
                            Remark = string.Empty
                        });
                    }
                    #endregion
                }
                else
                {
                    if (type == "3" //退票通知
                        || type == "4"  //废票通知
                        || type == "5" //取消出票通知
                         || type == "9" //直接拒单
                        )
                    {


                        #region 退款和取消出票通知
                        string refundType = (type == "3") ? "退票" : (type == "4" ? "废票" : "取消出票");
                        if (notifymsgs.Length >= 4
                            && !string.IsNullOrEmpty(notifymsgs[0])
                            && !string.IsNullOrEmpty(notifymsgs[1])
                            && !string.IsNullOrEmpty(notifymsgs[2])
                          )
                        {
                            decimal refundMoney = decimal.Parse(notifymsgs[2]);
                            string refundSerialNumber = notifymsgs[1];
                            string refundRemark = notifymsgs[3];
                            if (refundMoney > 0)
                            {
                                if (type == "5")
                                {
                                    //取消出票
                                    if (OnCancelIssue != null)
                                    {
                                        OnCancelIssue(this, new CancelIssueEventArgs()
                                        {
                                            OutOrderId = orderguid,
                                            PlatformCode = this.Code,
                                            Ramark = refundType + "：" + refundRemark,
                                            NotifyCollection = nameValueCollection
                                        });
                                    }
                                    //全退款
                                    if (OnRefund != null)
                                    {
                                        OnRefund(this, new PlatformRefundEventArgs()
                                        {
                                            OutOrderId = orderguid,
                                            RefundMoney = refundMoney,
                                            PlatformCode = this.Code,
                                            RefundRemark = refundRemark,
                                            RefundSerialNumber = refundSerialNumber,
                                            RefundType = refundType,
                                            NotifyCollection = nameValueCollection
                                        });
                                    }
                                }
                                else
                                {
                                    //退款
                                    if (OnRefund != null)
                                    {
                                        OnRefund(this, new PlatformRefundEventArgs()
                                        {
                                            OutOrderId = orderguid,
                                            RefundMoney = refundMoney,
                                            PlatformCode = this.Code,
                                            RefundRemark = refundRemark,
                                            RefundSerialNumber = refundSerialNumber,
                                            RefundType = refundType,
                                            NotifyCollection = nameValueCollection
                                        });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (type == "9")
                            {
                                if (!string.IsNullOrEmpty(notifymsgs[0])
                                    && !string.IsNullOrEmpty(notifymsgs[1])
                                    )
                                {
                                    //拒绝订单                                    
                                    if (OnCancelIssue != null)
                                    {
                                        OnCancelIssue(this, new CancelIssueEventArgs()
                                        {
                                            OutOrderId = orderguid,
                                            PlatformCode = this.Code,
                                            Ramark = "拒单:" + notifymsgs[1],
                                            NotifyCollection = nameValueCollection
                                        });
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else
                    {
                        //其他通知
                    }
                }
                //返回数据 不返回会发送5次通知
                responseResult = "true";
            }
            return responseResult;
        }

        public event PaidEventHandler OnPaid;
        public event IssueEventHandler OnIssue;
        public event CancelIssueHandler OnCancelIssue;
        public event PlatformRefundHandler OnRefund;
    }
}
