using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platforms._BaiTuo
{
    //GET
    public class _BaiTuoPlatformNotify : BasePlatform, IPlatformNotify
    {
        public override string Code
        {
            get { return "BaiTuo"; }
        }

        public bool CanProcess(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            bool IsCanProcess = false;
            try
            {
                //通知类型必须和订单号必须有 
                if (!string.IsNullOrEmpty(nameValueCollection["messageType"])
                    && !string.IsNullOrEmpty(nameValueCollection["areaCity"])
                    )
                {
                    string messageType = nameValueCollection["messageType"].ToString();
                    if (messageType == "1")//支付通知
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["shouldPaid"])
                            && !string.IsNullOrEmpty(nameValueCollection["orderID"])
                            && !string.IsNullOrEmpty(nameValueCollection["SystermId"])
                            && decimal.Parse(nameValueCollection["shouldPaid"]) > 0
                            )
                        {
                            IsCanProcess = true;
                        }
                    }
                    else if (messageType == "2")//出票通知
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["tickets"])
                            && !string.IsNullOrEmpty(nameValueCollection["forderformid"])
                            )
                        {
                            IsCanProcess = true;
                        }
                    }
                    else if (messageType == "3")//退款成功消息
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["RefundDeatils"])
                            && !string.IsNullOrEmpty(nameValueCollection["orderID"])
                            && !string.IsNullOrEmpty(nameValueCollection["SystermId"])
                            && !string.IsNullOrEmpty(nameValueCollection["ReturnMoney"])
                            && !string.IsNullOrEmpty(nameValueCollection["ReturnStatus"])
                            )
                        {
                            string refundDeatils = nameValueCollection["RefundDeatils"];
                            string returnMoney = nameValueCollection["ReturnMoney"];
                            if (decimal.Parse(returnMoney) > 0 && refundDeatils.Split(',').Length > 0)
                            {
                                IsCanProcess = true;
                            }
                        }
                    }
                    else if (messageType == "7")//改期的消息
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["portorderid"])//用户订单号
                         && !string.IsNullOrEmpty(nameValueCollection["orderID"]) //百拓订单号
                         && !string.IsNullOrEmpty(nameValueCollection["changeDeatils"])//姓名|票号 多个乘机人用“,"隔开就可以了
                          && !string.IsNullOrEmpty(nameValueCollection["changeStatus"])//Y  成功，N失败
                         )
                        {
                            IsCanProcess = true;
                        }
                    }
                    else if (messageType == "16")//拒绝出票消息
                    {
                        if (!string.IsNullOrEmpty(nameValueCollection["forderformid"])
                           && !string.IsNullOrEmpty(nameValueCollection["remark"]))
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
                string messageType = nameValueCollection["messageType"].ToString();
                string _OrderId = string.Empty;//接口订单号
                //判断通知类型           
                if (messageType == "1")//支付通知
                {
                    #region 支付通知
                    _OrderId = nameValueCollection["orderID"];
                    decimal _TotalPay = decimal.Parse(nameValueCollection["shouldPaid"]);
                    string _TradeID = nameValueCollection["SystermId"];
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
                else if (messageType == "2")//出票通知
                {
                    #region 出票通知
                    _OrderId = nameValueCollection["forderformid"];
                    NameValueCollection nvTicketInfo = new NameValueCollection();
                    string[] ticketNos = nameValueCollection["tickets"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    string[] indexData = null;
                    string passengerName = string.Empty;
                    string ticketNo = string.Empty;
                    foreach (string item in ticketNos)
                    {
                        indexData = item.Split('|');
                        if (indexData.Length >= 4)
                        {
                            passengerName = indexData[0].ToUpper().Replace("CHD", "").Trim();
                            if (!string.IsNullOrEmpty(passengerName))
                            {
                                ticketNo = indexData[1].Replace("--", "-").Trim();
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
                else if (messageType == "3")//退款通知
                {
                    #region 退款通知
                    _OrderId = nameValueCollection["orderID"];
                    string ReturnStatus = nameValueCollection["ReturnStatus"] == "Y" ? "成功" : "失败";
                    //退款交易号
                    string _TradeID = nameValueCollection["SystermId"];
                    //退款详情
                    string refundDeatils = nameValueCollection["RefundDeatils"];
                    //退款金额
                    decimal returnMoney = decimal.Parse(nameValueCollection["ReturnMoney"]);
                    //退款总手续费
                    decimal totalRefundPoundage = 0m;
                    string[] refundDeatilsArr = refundDeatils.Split(',');
                    if (refundDeatilsArr.Length > 0)
                    {
                        string[] itemArr = null;
                        foreach (string item in refundDeatilsArr)
                        {
                            itemArr = item.Split('|');
                            if (itemArr.Length >= 4)
                            {
                                totalRefundPoundage += decimal.Parse(itemArr[3]);
                            }
                        }
                    }
                    if (returnMoney > 0)
                    {
                        if (OnRefund != null)
                        {
                            OnRefund(this, new PlatformRefundEventArgs()
                            {
                                OutOrderId = _OrderId,
                                RefundMoney = returnMoney,
                                PlatformCode = this.Code,
                                RefundRemark = refundDeatils,
                                RefundSerialNumber = _TradeID,
                                RefundType = "退款" + ReturnStatus + "消息",
                                TotalRefundPoundage = totalRefundPoundage,
                                NotifyCollection = nameValueCollection
                            });
                        }
                    }
                    #endregion
                }
                else if (messageType == "16")//拒绝出票消息
                {
                    #region 取消出票
                    _OrderId = nameValueCollection["forderformid"];
                    string remark = nameValueCollection["remark"];
                    //取消出票
                    if (OnCancelIssue != null)
                    {
                        OnCancelIssue(this, new CancelIssueEventArgs()
                        {
                            OutOrderId = _OrderId,
                            Ramark = remark,
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
                            RefundRemark = "取消出票,全退款",
                            RefundSerialNumber = "",
                            RefundType = "取消出票消息",
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
