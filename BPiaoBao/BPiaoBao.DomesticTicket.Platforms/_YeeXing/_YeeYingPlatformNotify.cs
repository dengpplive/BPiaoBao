using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace BPiaoBao.DomesticTicket.Platforms._YeeXing
{
    //POST
    public class _YeeYingPlatformNotify : BasePlatform, IPlatformNotify
    {
        public override string Code
        {
            get { return "YeeXing"; }
        }
        public bool CanProcess(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            bool IsCanProcess = false;
            try
            {
                //通知类型必须和订单号 签名 必须有 
                if (!string.IsNullOrEmpty(nameValueCollection["type"])
                    && !string.IsNullOrEmpty(nameValueCollection["orderid"])
                    && !string.IsNullOrEmpty(nameValueCollection["sign"])
                     && !string.IsNullOrEmpty(nameValueCollection["areaCity"])
                    )
                {
                    string areaCity = nameValueCollection["areaCity"];
                    var parames = platformConfig.Areas.GetByArea(ref areaCity);
                    string _privateKey = parames["_privateKey"].Value;
                    //签名验证
                    string decrSign = getSign(nameValueCollection, _privateKey);
                    string sign = nameValueCollection["sign"];
                    if (sign == decrSign)
                    {
                        string type = nameValueCollection["type"].ToString();
                        if (type == "1")//出票
                        {
                            if (!string.IsNullOrEmpty(nameValueCollection["passengerName"])
                                && !string.IsNullOrEmpty(nameValueCollection["airId"])
                                )
                            {
                                IsCanProcess = true;
                            }
                        }
                        else if (type == "2")//支付成功通知
                        {
                            if (!string.IsNullOrEmpty(nameValueCollection["totalPrice"])
                                && !string.IsNullOrEmpty(nameValueCollection["payid"])
                                && decimal.Parse(nameValueCollection["totalPrice"]) > 0
                                )
                            {
                                IsCanProcess = true;
                            }
                        }
                        else if (type == "3")//取消成功通知（乘客或订单）
                        {
                            if (!string.IsNullOrEmpty(nameValueCollection["passengerName"]))
                            {
                                //passengerName 需要使用urldecode解密
                                IsCanProcess = true;
                            }
                        }
                        else if (type == "4")//退废票通知
                        {
                            if (!string.IsNullOrEmpty(nameValueCollection["refundPrice"])
                                && !string.IsNullOrEmpty(nameValueCollection["procedures"])
                                && !string.IsNullOrEmpty(nameValueCollection["airId"])
                                && decimal.Parse(nameValueCollection["refundPrice"]) > 0
                                )
                            {
                                IsCanProcess = true;
                            }
                        }
                        else if (type == "5")//改期通知
                        {
                            if (!string.IsNullOrEmpty(nameValueCollection["changeMemo"])//改期或改证件备注
                                && !string.IsNullOrEmpty(nameValueCollection["changeStatus"])//改期状态 1—成功 2—拒绝 
                                && !string.IsNullOrEmpty(nameValueCollection["refuseMemo"])//拒绝理由
                                )
                            {
                                IsCanProcess = true;
                            }
                        }
                        else if (type == "6")//拒绝出票通知
                        {
                            if (!string.IsNullOrEmpty(nameValueCollection["passengerName"])
                                && !string.IsNullOrEmpty(nameValueCollection["refuseMemo"])
                                )
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

        public string Process(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            string responseResult = string.Empty;
            //验证通知信息正常                              
            if (CanProcess(nameValueCollection))
            {
                string _OrderId = nameValueCollection["orderid"];
                string _type = nameValueCollection["type"];
                //判断通知类型           
                if (_type == "2")//支付
                {
                    #region 支付通知
                    decimal _TotalPay = decimal.Parse(nameValueCollection["totalPrice"]);
                    string _TradeID = nameValueCollection["payid"];
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
                else if (_type == "1")//出票
                {
                    #region 出票通知
                    NameValueCollection nvTicketInfo = new NameValueCollection();
                    string[] passengerNames = nameValueCollection["passengerName"].Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] ticketNumbers = nameValueCollection["airId"].Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                    string passengerName = string.Empty;
                    string ticketNo = string.Empty;
                    for (int i = 0; i < passengerNames.Length; i++)
                    {
                        passengerName = passengerNames[i].ToUpper().Replace("CHD", "").Trim();
                        if (!string.IsNullOrEmpty(passengerName))
                        {
                            ticketNo = ticketNumbers[i].Replace("--", "-").Trim();
                            if (!string.IsNullOrEmpty(ticketNo))
                                nvTicketInfo.Add(passengerName, ticketNo);
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
                else if (_type == "4")//退废票通知
                {
                    #region 退款通知
                    //退票的票号 多个用^分割
                    string airId = nameValueCollection["airId"];
                    //退款金额
                    decimal refundPrice = decimal.Parse(nameValueCollection["refundPrice"]);
                    //退款手续费 
                    decimal procedures = decimal.Parse(nameValueCollection["procedures"]);
                    //退款理由
                    string refuseMemo = nameValueCollection["refuseMemo"];
                    if (refundPrice > 0)
                    {
                        if (OnRefund != null)
                        {
                            OnRefund(this, new PlatformRefundEventArgs()
                            {
                                OutOrderId = _OrderId,
                                RefundMoney = refundPrice,
                                PlatformCode = this.Code,
                                RefundRemark = refuseMemo + airId,
                                RefundSerialNumber = "",
                                TotalRefundPoundage = procedures,
                                RefundType = "退废票通知",
                                NotifyCollection = nameValueCollection
                            });
                        }
                    }
                    #endregion
                }
                else if (_type == "6")//拒绝出票通知
                {
                    #region 取消出票
                    string passengerName = nameValueCollection["passengerName"];
                    string refuseMemo = nameValueCollection["refuseMemo"];
                    //取消出票
                    if (OnCancelIssue != null)
                    {
                        OnCancelIssue(this, new CancelIssueEventArgs()
                        {
                            OutOrderId = _OrderId,
                            PlatformCode = this.Code,
                            Ramark = "拒绝乘客:" + passengerName + "拒绝理由：" + refuseMemo,
                            NotifyCollection = nameValueCollection
                        });
                    }
                    //全退款
                    if (OnRefund != null)
                    {
                        OnRefund(this, new PlatformRefundEventArgs()
                        {
                            OutOrderId = _OrderId,
                            RefundMoney = 0m,
                            PlatformCode = this.Code,
                            RefundRemark = "取消出票",
                            RefundSerialNumber = "",
                            RefundType = "",
                            NotifyCollection = nameValueCollection
                        });
                    }
                    #endregion
                }
            }

            return responseResult;
        }

        #region 签名
        //MD5加密方法
        private string userMd5(string str, Encoding enco)
        {
            MD5 md5 = MD5.Create();
            byte[] s = md5.ComputeHash(enco.GetBytes(str));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < s.Length; i++)
            {
                sBuilder.Append(s[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }
        //获取签名
        private string getSign(System.Collections.Specialized.NameValueCollection nameValueCollection, String privateKey)
        {
            SortedDictionary<String, String> param = new SortedDictionary<string, string>();
            foreach (string key in nameValueCollection.Keys)
            {
                if (key.ToLower() != "sign" && key.ToLower() != "areacity")
                {
                    if (!param.ContainsKey(key))
                    {
                        param.Add(key, nameValueCollection[key]);
                    }
                }
            }
            //获取哈希表中的所有的key
            ArrayList keys = new ArrayList(param.Keys);
            //将key进行排序
            keys.Sort();
            //将排序后的哈希表中key对应的值取出来
            String prestr = "";
            for (int i = 0; i < keys.Count; i++)
            {
                String s = Convert.ToString(keys[i]);

                String value = Convert.ToString(param["" + s + ""]);
                prestr = prestr + value;
            }
            //将排序后的结果加上key
            String resultstr = prestr + privateKey;
            //将上一步的结果，然后进行utf-8编码，变大写，MD5加密
            String sign = userMd5(HttpUtility.UrlEncode(resultstr).ToUpper(), System.Text.Encoding.UTF8);
            return sign;
        }
        #endregion

        public event PaidEventHandler OnPaid;
        public event IssueEventHandler OnIssue;
        public event CancelIssueHandler OnCancelIssue;
        public event PlatformRefundHandler OnRefund;
    }
}
