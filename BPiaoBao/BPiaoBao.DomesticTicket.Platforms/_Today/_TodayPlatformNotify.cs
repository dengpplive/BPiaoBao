using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platforms._Today
{
    public class _TodayPlatformNotify : BasePlatform, IPlatformNotify
    {
        public override string Code
        {
            get { return "Today"; }
        }

        public bool CanProcess(System.Collections.Specialized.NameValueCollection nameValueCollection)
        {
            bool IsCanProcess = false;
            try
            {
                string sign = nameValueCollection["sign"];
                string areaCity = nameValueCollection["areaCity"];
                var parames = platformConfig.Areas.GetByArea(ref areaCity);
                string _privateKey = parames["_privateKey"].Value;
                string decrSign = getSign(nameValueCollection, _privateKey);
                //签名验证
                if (sign == decrSign)
                {
                    IsCanProcess = true;
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

            try
            {
                if (CanProcess(nameValueCollection))
                {
                    //订单号
                    string order_no = nameValueCollection["order_no"];
                    //服务类型
                    string service = nameValueCollection["service"];
                    if (service == "order_payment_notify")//支付通知
                    {
                        #region 支付通知
                        //支付交易号
                        string trade_no = nameValueCollection["trade_no"];
                        //支付时间
                        string pay_time = nameValueCollection["pay_time"];
                        //交易备注
                        string remark = nameValueCollection["remark"];
                        //支付状态 1.支付成功，订单状态更新成功 2.支付成功，订单状态更新异常 3.支付状态验证失败 4.支付状态接收失败 5.支付成功，订单状态更新失败 6.支付失败
                        string status = nameValueCollection["status"];
                        //支付类型 order:订单支付 change:改签支付 risecabin:升舱支付 itinerary:行程单支付
                        string pay_type = nameValueCollection["pay_type"];

                        if (OnPaid != null)
                        {
                            OnPaid(this, new PaidEventArgs()
                            {
                                OutOrderId = order_no,
                                PlatformCode = this.Code,
                                //PaidMeony = _TotalPay,
                                SerialNumber = trade_no,
                                OtherInfo = string.Format("支付时间:{0},支付状态:{1},支付类型:{2},备注:{3}",
                                pay_time, GetPayStatus(status), GetPayType(pay_type), remark
                                )
                            });
                        }
                        #endregion
                    }
                    else if (service == "order_issue_notify")//出票通知
                    {
                        #region 出票通知
                        //备注
                        string remark = nameValueCollection["remark"];
                        //出票状态 T:出票完成 F:暂不能出票
                        string status = nameValueCollection["status"].Trim().ToUpper();
                        //出票乘客  多人用^隔开
                        string passenger = nameValueCollection["passenger"];
                        //出票票号 多个用^隔开
                        string ticket_no = nameValueCollection["ticket_no"];
                        //证件号 多证件号用^分隔
                        //string card_no = nameValueCollection["card_no"];
                        if (status == "T")
                        {
                            string[] passengerArr = passenger.Split('^');
                            string[] ticketNoArr = ticket_no.Split('^');
                            string passengerName = string.Empty;
                            string ticketNo = string.Empty;
                            NameValueCollection nvTicketInfo = new NameValueCollection();
                            if (passengerArr.Length == ticketNoArr.Length)
                            {
                                for (int i = 0; i < passengerArr.Length; i++)
                                {
                                    passengerName = passengerArr[i].ToUpper().Replace("CHD", "").Trim();
                                    if (!string.IsNullOrEmpty(passengerName))
                                    {
                                        ticketNo = ticketNoArr[i].Replace("--", "-").Trim();
                                        if (!string.IsNullOrEmpty(ticketNo))
                                            nvTicketInfo.Add(passengerName, ticketNo);
                                    }
                                }
                            }
                            if (nvTicketInfo.Count > 0 && OnIssue != null)
                            {
                                OnIssue(this, new IssueEventArgs()
                                {
                                    OutOrderId = order_no,
                                    TicketInfo = nvTicketInfo,
                                    PlatformCode = this.Code,
                                    Remark = remark
                                });
                            }
                        }
                        else
                        {
                            //取消出票
                            if (OnCancelIssue != null)
                            {
                                OnCancelIssue(this, new CancelIssueEventArgs()
                                {
                                    OutOrderId = order_no,
                                    Ramark = remark,
                                    PlatformCode = this.Code,
                                    NotifyCollection = nameValueCollection
                                });
                            }

                            //暂不出票
                            //全退款
                            //if (OnRefund != null)
                            //{
                            //    OnRefund(this, new PlatformRefundEventArgs()
                            //    {
                            //        OutOrderId = order_no,
                            //        RefundMoney = 0,
                            //        PlatformCode = this.Code,
                            //        RefundRemark = "暂不能出票",
                            //        RefundSerialNumber = "",
                            //        RefundType = "",
                            //        NotifyCollection = nameValueCollection
                            //    });
                            //}
                        }
                        #endregion
                    }
                    else if (service == "order_refund_notify")//退款通知
                    {
                        #region 退款通知
                        //退费类型 1退票 2废票
                        string refund_type = nameValueCollection["refund_type"];
                        //退费状态 1.退/废成功 2.暂不能退/废 3.退款中
                        string status = nameValueCollection["status"];
                        //备注
                        string remark = nameValueCollection["remark"];
                        //乘客  多人用^隔开
                        string passenger = nameValueCollection["passenger"];
                        //票号 多个用^隔开
                        string ticket_no = nameValueCollection["ticket_no"];
                        //证件号 多证件号用^分隔
                        //string card_no = nameValueCollection["card_no"];
                        //退款金额
                        decimal amount = decimal.Parse(nameValueCollection["amount"]);
                        if (amount > 0)
                        {
                            //if (OnRefund != null)
                            //{
                            //    OnRefund(this, new PlatformRefundEventArgs()
                            //    {
                            //        OutOrderId = order_no,
                            //        RefundMoney = amount,
                            //        PlatformCode = this.Code,
                            //        RefundRemark = string.Format("退费状态:{0},乘客:{1},票号:{2},备注:{3}", GetTFStatus(status), passenger, ticket_no, remark),
                            //        // RefundSerialNumber = "",
                            //        // TotalRefundPoundage = procedures,
                            //        RefundType = refund_type == "1" ? "退票" : "废票",
                            //        NotifyCollection = nameValueCollection
                            //    });
                            //}
                            if (OnRefundTicket != null)
                            {
                                OnRefundTicket(this, new RefundTicketArgs()
                                {
                                    NotifyType = int.Parse(refund_type),
                                    OutOrderId = order_no,
                                    Remark = string.Format("退费状态:{0},乘客:{1},票号:{2},备注:{3}", GetTFStatus(status), passenger, ticket_no, remark),
                                     //ProcessStatus=


                                });
                            }
                        }
                        #endregion
                    }
                    else if (service == "order_change_notify")//改签通知
                    {
                        #region 改签通知

                        //改签状态 1.审核通过,等待退款 2.不能改签 3.改签完成
                        string status = nameValueCollection["status"];
                        //说明
                        string remark = nameValueCollection["remark"];
                        //乘客姓名  多人用^隔开
                        string passenger = nameValueCollection["passenger"];
                        //改签信息 格式:出发地^目的地^航班号^舱位^起飞日期
                        string ticket_no = nameValueCollection["ticket_info"];
                        #endregion
                    }
                    else if (service == "order_change_pnr_notify")//换编码出票通知
                    {
                        #region 换编码出票通知

                        //创建订单的Pnr
                        string old_pnr = nameValueCollection["old_pnr"];
                        //出票的编码
                        string new_pnr = nameValueCollection["new_pnr"];

                        #endregion
                    }

                    //签名验证通过
                    responseResult = "success";
                }
                else
                {
                    //签名验证失败
                    responseResult = "fail";
                }
            }
            catch (Exception ex)
            {
                responseResult = "fail";
                throw new OrderCommException(ex.Message);
            }
            return responseResult;
        }

        #region 签名
        private string getSign(NameValueCollection nameValueCollection, string privateKey)
        {
            List<string> paramsList = new List<string>();
            foreach (string key in nameValueCollection)
            {
                if (key.ToLower() != "sign" && key.ToLower() != "areacity")
                {
                    paramsList.Add(key + "=" + nameValueCollection[key]);
                }
            }
            //生成md5签名
            string result = Md5(string.Join("&", BubbleSort(paramsList.ToArray())) + privateKey, Encoding.Default);
            return result;
        }
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
            return sb.ToString();
        }
        /// <summary>
        /// 把需要签名的参数数组按英文字母升序排列
        /// </summary>
        /// <param name="r">参数数组</param>
        /// <returns>返回数组</returns>
        private string[] BubbleSort(string[] r)
        {
            int i, j; //交换标志 
            string temp;
            bool exchange;
            //最多做R.Length-1趟排序 
            for (i = 0; i < r.Length; i++)
            {
                exchange = false;
                //本趟排序开始前，交换标志应为假
                for (j = r.Length - 2; j >= i; j--)
                {
                    //交换条件
                    if (System.String.CompareOrdinal(r[j + 1], r[j]) < 0)
                    {
                        temp = r[j + 1];
                        r[j + 1] = r[j];
                        r[j] = temp;
                        //发生了交换，故将交换标志置为真 
                        exchange = true;
                    }
                }
                if (!exchange) //本趟排序未发生交换，提前终止算法 
                {
                    break;
                }
            }
            return r;
        }

        #endregion

        private string GetPayStatus(string statusCode)
        {
            string result = string.Empty;
            switch (statusCode.Trim())
            {
                case "1":
                    result = "支付成功，订单状态更新成功";
                    break;
                case "2": result = "支付成功，订单状态更新异常";
                    break;
                case "3":
                    result = "支付状态验证失败";
                    break;
                case "4":
                    result = "支付状态接收失败";
                    break;
                case "5":
                    result = "支付成功，订单状态更新失败";
                    break;
                case "6":
                    result = "支付失败";
                    break;
                default:
                    break;
            }
            return result;
        }

        private string GetTFStatus(string statusCode)
        {
            //1.退/废成功 2.暂不能退/废 3.退款中
            string result = string.Empty;
            switch (statusCode)
            {
                case "1":
                    result = "退/废成功";
                    break;
                case "2":
                    result = "暂不能退/废";
                    break;
                case "3":
                    result = "退款中";
                    break;
                default:
                    break;
            }
            return result;
        }

        private string GetPayType(string payTypeCode)
        {
            string result = string.Empty;
            switch (payTypeCode.Trim())
            {
                case "order":
                    result = "订单支付";
                    break;
                case "change": result = "改签支付";
                    break;
                case "risecabin":
                    result = "升舱支付";
                    break;
                case "itinerary":
                    result = "行程单支付";
                    break;
                default:
                    break;
            }
            return result;
        }


        public event PaidEventHandler OnPaid;
        public event IssueEventHandler OnIssue;
        public event CancelIssueHandler OnCancelIssue;
        public event PlatformRefundHandler OnRefund;
        public event RefundTicketHandler OnRefundTicket;
    }
}
