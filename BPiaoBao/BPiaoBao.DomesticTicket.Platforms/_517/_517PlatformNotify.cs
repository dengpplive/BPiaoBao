using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.DomesticTicket.Platform.Plugin.MessageMap;
using JoveZhao.Framework;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platforms._517
{
    //POST请求
    public class _517PlatformNotify : BasePlatform, IPlatformNotify
    {
        public override string Code
        {
            get { return "517"; }
        }
        public bool CanProcess(NameValueCollection nameValueCollection)
        {
            bool IsCanProcess = false;
            try
            {
                if (nameValueCollection.AllKeys.Contains("json"))
                {
                   dynamic dymic= JsonConvert.DeserializeObject(nameValueCollection["json"]);
                   if(dymic.NotifyType!="")
                        return true;

                }

                //通知类型必须和订单号 签名 必须有 
                if (!string.IsNullOrEmpty(nameValueCollection["NotifyType"])
                    && !string.IsNullOrEmpty(nameValueCollection["OrderId"])
                    && !string.IsNullOrEmpty(nameValueCollection["Sign"])
                    && !string.IsNullOrEmpty(nameValueCollection["areaCity"])
                    )
                {
                    //验证签名
                    string Sign = nameValueCollection["Sign"];
                    //获取区域
                    string areaCity = nameValueCollection["areaCity"];
                    var parames = platformConfig.Areas.GetByArea(ref areaCity);
                    string _517Password = parames["_517Password"].Value;
                    //安全码
                    string _privateKey = parames["_517Ag"].Value;
                    //获取签名                                 
                    string descSign = getSign(nameValueCollection, _517Password, _privateKey);
                    //验证签名
                    if (Sign == descSign)
                    {
                        string NotifyType = nameValueCollection["NotifyType"].ToString();
                        if (NotifyType == "支付宝支付通知"
                            || NotifyType == "财付通支付通知"
                            )
                        {
                            if (!string.IsNullOrEmpty(nameValueCollection["TotalPay"])
                                && !string.IsNullOrEmpty(nameValueCollection["TradeID"])
                                && decimal.Parse(nameValueCollection["TotalPay"].ToString()) > 0
                                )
                            {
                                IsCanProcess = true;
                            }
                        }
                        else if (NotifyType == "出票通知")
                        {
                            if (!string.IsNullOrEmpty(nameValueCollection["TicketNos"]))
                            {
                                IsCanProcess = true;
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

        public string Process(NameValueCollection nameValueCollection)
        {
            string responseResult = string.Empty;
            try
            {
                //验证通知信息正常 暂时留着
                if (CanProcess(nameValueCollection))
                {
                    if (nameValueCollection.AllKeys.Contains("json"))
                    {
                        string notifyMessage = string.Empty;
                        _517NotifyMessage notify = JsonConvert.DeserializeObject<_517NotifyMessage>(notifyMessage);
                        if (notify == null)
                            return responseResult;
                     //   OnRefundTicket(this, new _517RefundTicketArgs { NotityMessage = notify });
                        //NotifyType【1,废票通知，2退票通知】
                        //if (notify.NotifyType == 1)
                        //{
                        //    if (notify.Message.OrderState == 1)
                        //    { 
                        //        //已经退款，交易结束
                        //    }
                        //    else if (notify.Message.OrderState == 3)
                        //    { 
                        //        //审核不通过，拒绝处理

                        //    }
                        //}
                        //else if (notify.NotifyType == 2)
                        //{
                        //    if (notify.Message.OrderState == 1)
                        //    {
                        //        //已经退款，交易结束
                        //    }
                        //    else if (notify.Message.OrderState == 2)
                        //    { 
                        //        //审核不通过，拒绝处理
                        //    }
                        //}
                    }
                    else
                    {
                        string _OrderId = nameValueCollection["OrderId"];
                        //区域
                        string areaCity = nameValueCollection["areaCity"];
                        //判断通知类型           
                        if (nameValueCollection["NotifyType"] == "支付宝支付通知"
                            || nameValueCollection["NotifyType"] == "财付通支付通知"
                            )
                        {
                            #region 支付宝支付通知
                            //票面支付总价
                            decimal _TotalPay = decimal.Parse(nameValueCollection["TotalPay"]);
                            string _TradeID = nameValueCollection["TradeID"];
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
                        else if (nameValueCollection["NotifyType"] == "出票通知")
                        {
                            #region 出票通知
                            //出票状态，0 表示出票，1 表示取消出票
                            string DrawABillFlag = nameValueCollection["DrawABillFlag"].Trim();
                            //取消出票理由
                            string DrawABillRemark = nameValueCollection["DrawABillRemark"].Trim();
                            if (DrawABillFlag == "0")
                            {
                                NameValueCollection nvTicketInfo = new NameValueCollection();
                                string[] ticketNos = nameValueCollection["TicketNos"].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                                string[] indexData = null;
                                string passengerName = string.Empty;
                                string ticketNo = string.Empty;
                                foreach (string item in ticketNos)
                                {
                                    indexData = item.Split('|');
                                    if (indexData.Length >= 5)
                                    {
                                        passengerName = indexData[4].ToUpper().Replace("CHD", "").Trim();
                                        if (!string.IsNullOrEmpty(passengerName))
                                        {
                                            ticketNo = (indexData[0] + "-" + indexData[1]).Replace("--", "-").Trim();
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
                                        Remark = DrawABillRemark
                                    });
                                }
                            }
                            else
                            {
                                #region 取消出票
                                //取消出票
                                if (OnCancelIssue != null)
                                {
                                    OnCancelIssue(this, new CancelIssueEventArgs()
                                    {
                                        OutOrderId = _OrderId,
                                        Ramark = DrawABillRemark,
                                        PlatformCode = this.Code,
                                        NotifyCollection = nameValueCollection
                                    });
                                }
                                //退款
                                if (OnRefund != null)
                                {
                                    OnRefund(this, new PlatformRefundEventArgs()
                                    {
                                        OutOrderId = _OrderId,
                                        RefundMoney = 0,
                                        PlatformCode = this.Code,
                                        RefundRemark = "取消出票全退款",
                                        RefundSerialNumber = "",
                                        RefundType = "",
                                        NotifyCollection = nameValueCollection
                                    });
                                }
                                #endregion
                            }
                            #endregion
                        }
                    }
                    responseResult = "SUCCESS";
                }
            }
            catch (Exception ex)
            {
                responseResult = "FAIL";
                throw new OrderCommException(ex.Message);
            }
            return responseResult;
        }
        #region 签名
        private string Md5(string param, Encoding encode)
        {
            MD5 md5 = MD5.Create();
            byte[] b = encode.GetBytes(param);
            byte[] md5b = md5.ComputeHash(b);
            md5.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (var item in md5b)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString().ToUpper();//转大写
        }
        private string getSign(System.Collections.Specialized.NameValueCollection nameValueCollection, string userPwd, String privateKey)
        {
            string result = string.Empty;
            StringBuilder sbParams = new StringBuilder();
            //参数包括key一起签名
            foreach (string key in nameValueCollection.Keys)
            {
                if (key.ToLower() != "sign" && key.ToLower() != "areacity")
                {
                    sbParams.Append(key + "=" + nameValueCollection[key]);
                }
            }
            //当天日期
            sbParams.Append(System.DateTime.Now.ToString("yyyy-MM-dd"));
            //安全码
            sbParams.Append(privateKey);
            //获取签名
            result = Md5(sbParams.ToString(), Encoding.UTF8);
            return result;
        }
        #endregion



        public event PaidEventHandler OnPaid;
        public event IssueEventHandler OnIssue;
        public event CancelIssueHandler OnCancelIssue;
        public event PlatformRefundHandler OnRefund;
        public event RefundTicketHandler OnRefundTicket;

    }

}
