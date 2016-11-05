using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BPiaoBao.DomesticTicket.Domain;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Platforms._PTService;
using JoveZhao.Framework;
using System.Collections.Specialized;
using PnrAnalysis;
using AutoMapper;
using BPiaoBao.DomesticTicket.Platforms.PTInterface;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework.Helper;
namespace BPiaoBao.DomesticTicket.Platforms._PiaoMeng
{
    [Platform("PiaoMeng")]
    public class _PiaoMengPlatform : BasePlatform, IPlatform
    {
        private readonly _PiaoMengAccountInfo piaoMentAccountInfo;
        public _PiaoMengPlatform()
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            this.piaoMentAccountInfo = GetInfo(platSystem, string.Empty);
        }
        public override string Code
        {
            get { return "PiaoMeng"; }
        }

        private string ErrToMessage(string strErrMessage)
        {
            string strResult = string.Empty;
            /*
             *  POLICY_NOT_FOUND
                USER_NOT_EXIST
                SAFENO_IS_ERROR
             */
            if (strErrMessage.Contains("POLICY_NOT_FOUND"))
            {
                strResult = "政策不存在";
            }
            else if (strErrMessage.Contains("USER_NOT_EXIST"))
            {
                strResult = "接口账号不存在";
            }
            else if (strErrMessage.Contains("SAFENO_IS_ERROR"))
            {
                strResult = "接口密码错误";
            }
            else if (strErrMessage.Contains("USER_PAY_TYPE_MISMATCH"))
            {
                strResult = "代扣账号金额不足【" + strErrMessage + "】";
            }
            else if (strErrMessage.Contains("服务器无法处理请求"))
            {
                strResult = "服务器无法处理请求";
            }
            else
            {
                strResult = strErrMessage;
            }
            return strResult;
        }

        public List<PlatformPolicy> GetPoliciesByPnrContent(string pnrContent, bool IsLowPrice, BPiaoBao.Common.PnrData pnrData)
        {
            PnrAnalysis.PnrModel pnrModel = pnrData.PnrMode;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == EnumPlatform._PiaoMeng.ToString().Replace("_", "")).FirstOrDefault();
            List<PlatformPolicy> PolicyList = new List<PlatformPolicy>();
            if (pnrModel == null || pnrModel._LegList.Count == 0 || !platSystem.State)
            {
                return PolicyList;
            }
            string area = pnrModel._LegList[0].FromCode;
            _PiaoMengAccountInfo accountInfo = GetInfo(platSystem, area);
            //是否换编码出票 1:是                
            string _IsChangePNRCP = accountInfo._IsChangePNRCP;
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            //航空公司
            string CarryCode = pnrModel._CarryCode;
            DataSet dsPolicy = new DataSet();
            PTMange ptMange = new PTMange();
            Logger.WriteLog(LogType.DEBUG, "票盟获取政策开始时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            //System.Data.DataSet dsPolicy = m_PTService.PT_PiaoMengGetPolicy(_pmAccout, _pmPassword, _pmAg, _IsLowerPrice, pnrContent);            
            //System.Data.DataSet dsPolicy = m_PTService.PT_NewPiaoMengGetPolicy(_pmAccout, _pmPassword, _pmAg, _IsLowerPrice, pnrContent, PTPnrData);            
            dsPolicy = ptMange._PiaoMengGetPolicy(accountInfo._pmAccout, accountInfo._pmPassword, accountInfo._pmAg, _IsLowerPrice, pnrContent, pnrData);
            Logger.WriteLog(LogType.DEBUG, "票盟获取政策结束时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
            //转化
            if (dsPolicy != null && dsPolicy.Tables.Count > 0)
            {
                if (dsPolicy.Tables.Contains(this.Code) && dsPolicy.Tables.Contains("Policy") && dsPolicy.Tables[this.Code].Rows.Count > 0)
                {
                    DataRow dr_Price = dsPolicy.Tables[this.Code].Rows[0];
                    if (dr_Price["Status"].ToString() == "T")
                    {
                        decimal SeatPrice = 0m, TaxFare = 0m, RQFare = 0m;
                        decimal.TryParse(dr_Price["SeatPrice"].ToString(), out SeatPrice);
                        decimal.TryParse(dr_Price["ABFare"].ToString(), out TaxFare);
                        decimal.TryParse(dr_Price["RQFare"].ToString(), out RQFare);
                        bool IsLow = _IsLowerPrice == "1" ? true : false;

                        string StartTime = "00:00", EndTime = "00:00";
                        decimal PolicyPoint = 0m;
                        DataRowCollection drs = dsPolicy.Tables[0].Rows;
                        foreach (DataRow dr in drs)
                        {
                            PlatformPolicy policy = new PlatformPolicy();
                            StartTime = "00:00";
                            EndTime = "00:00";
                            policy.Id = dr["id"] != DBNull.Value ? dr["id"].ToString() : "";
                            policy.PlatformCode = this.Code;
                            policy.AreaCity = area;
                            if (!string.IsNullOrEmpty(policy.Id))
                            {
                                policy.IsChangePNRCP = dr["changerecord"].ToString() == "1" ? true : false;
                                policy.IsSp = dr["isspecmark"].ToString() == "1" ? true : false;
                                policy.PolicyType = dr["policytype"].ToString().Contains("BSP") ? "BSP" : "B2B";
                                // policy.PolicyType = dr["policytype"].ToString().ToUpper().Contains("B2P") ? "1" : "2";
                                // policy.PolicyType = policy.PolicyType == "1" ? "BSP" : "B2B";
                                policy.CarryCode = CarryCode;
                                if (dr["worktime"].ToString().Split('-').Length == 2)
                                {
                                    StartTime = dr["worktime"].ToString().Split('-')[0];
                                    EndTime = dr["worktime"].ToString().Split('-')[1];
                                    if (!StartTime.Contains(":"))
                                    {
                                        StartTime = StartTime.Insert(2, ":");
                                    }
                                    if (!EndTime.Contains(":"))
                                    {
                                        EndTime = EndTime.Insert(2, ":");
                                    }
                                }
                                policy.WorkTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                EndTime = dr["RefundWorkTimeTo"].ToString().Insert(2, ":");
                                policy.ReturnTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                policy.AnnulTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                policy.ReturnMoney = 0m;
                                policy.CPOffice = dr["officeid"].ToString();
                                policy.Remark = dr["note"].ToString();
                                decimal.TryParse(dr["rate"].ToString(), out PolicyPoint);
                                policy.PolicyPoint = PolicyPoint;
                                policy.IssueSpeed = platSystem != null ? platSystem.IssueTicketSpeed : "";
                                if (_IsChangePNRCP != "1" && policy.IsChangePNRCP)
                                {
                                    continue;
                                }
                                policy.IsLow = IsLow;
                                policy.SeatPrice = SeatPrice;
                                policy.ABFee = TaxFare;
                                policy.RQFee = RQFare;
                                //过滤不符合的政策点数
                                if (PolicyPoint > 0 && PolicyPoint < 100)
                                    PolicyList.Add(policy);

                            }
                        }//foreach
                        //取前几条政策
                        if (platSystem != null)
                            PolicyList = PolicyList.OrderByDescending(pp => pp.PolicyPoint).Take(platSystem.GetPolicyCount).ToList();
                    }
                    else
                    {
                        //失败信息
                        string Message = dr_Price["Message"].ToString();
                    }
                }
            }
            return PolicyList;
        }

        public PlatformOrder CreateOrder(string pnrContent, bool IsLowPrice, string areaCity, string policyId, string RateId, string localOrderId, decimal policyPoint, decimal ReturnMoney, BPiaoBao.Common.PnrData pnrData)
        {
            PlatformOrder platformOrder = null;
            string FailMessage = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _PiaoMengAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            pnrContent = System.Web.HttpUtility.UrlEncode(pnrContent);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrder = m_PTService.PT_PiaoMengCreateOrder(accountInfo._pmAccout, accountInfo._pmPassword, accountInfo._pmAg, _IsLowerPrice, policyId, pnrContent);
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //DataSet dsOrder = m_PTService.PT_NewPiaoMengCreateOrder(_pmAccout, _pmPassword, _pmAg, _IsLowerPrice, policyId, pnrContent, PTPnrData);
            if (dsOrder != null && dsOrder.Tables.Count > 0 && dsOrder.Tables.Contains(Code))
            {
                DataRow dr = dsOrder.Tables[Code].Rows[0];
                if (dsOrder.Tables[Code].Rows[0]["Status"].ToString() == "T")
                {
                    //成功 获取数据                   
                    platformOrder = new PlatformOrder();
                    decimal TotlePaidPirce = 0m, TotaSeatlPrice = 0m;
                    decimal.TryParse(dr["PaidTotlePirce"].ToString(), out TotlePaidPirce);
                    decimal.TryParse(dr["SeatTotalPrice"].ToString(), out TotaSeatlPrice);
                    platformOrder.OrderId = localOrderId;
                    platformOrder.TotlePaidPirce = TotlePaidPirce;
                    platformOrder.TotaSeatlPrice = TotaSeatlPrice;
                    platformOrder.OutOrderId = dr["OutOrderId"].ToString();
                    platformOrder.PnrCode = dr["Pnr"].ToString();
                    platformOrder.AreaCity = areaCity;
                }
                else
                {
                    //失败
                    FailMessage = dr["Message"].ToString() + " 失败";
                    //FailMessage = "生成订单失败！";
                }
            }
            else
            {
                //异常 或超时
                FailMessage = "调用接口超时";
            }
            if (!string.IsNullOrEmpty(FailMessage))
            {
                throw new CreateInterfaceOrderException(ErrToMessage(FailMessage));
            }
            return platformOrder;
        }

        public void Pay(string areaCity, PlatformOrder order)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _PiaoMengAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            if (accountInfo._PayWay == "0")
            {
                order.PaidMethod = EnumPaidMethod.支付宝;
            }
            string OutOrderId = order.OutOrderId;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayOrder = m_PTService.PT_PiaoMengOrderPay(accountInfo._pmAccout, accountInfo._pmPassword, accountInfo._pmAg, OutOrderId);
            string FailMessage = string.Empty;
            if (dsPayOrder != null && dsPayOrder.Tables.Count > 0 && dsPayOrder.Tables.Contains(Code))
            {
                DataRow dr = dsPayOrder.Tables[Code].Rows[0];
                if (dsPayOrder.Tables[Code].Rows[0]["Status"].ToString() == "T")
                {
                    //成功 获取数据     
                    Logger.WriteLog(LogType.INFO, this.Code + "接口订单（" + order.OutOrderId + "）代付成功,订单号:" + order.OrderId);
                }
                else
                {
                    //失败
                    FailMessage = dr["Message"].ToString() + " 失败";
                }
            }
            else
            {
                //异常 或超时
                FailMessage = "调用支付接口超时";
            }
            if (!string.IsNullOrEmpty(FailMessage))
            {
                throw new PayInterfaceOrderException(ErrToMessage(FailMessage));
            }
        }


        public bool CanCancelOrder
        {
            get { throw new NotImplementedException(); }
        }

        public void CancelOrder(string areaCity, string outOrderId, string pnr, string CancelRemark, string passengerName)
        {
            throw new NotImplementedException();
        }
        public Dictionary<string, string> AutoCompositeTicket(string areaCity, string orderId, string outOrderId, string pnr)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _PiaoMengAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_PMOrderQuery(outOrderId, accountInfo._pmAccout, accountInfo._pmAg);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0
                && dsOrderInfo.Tables.Contains("passinfo")
                    && dsOrderInfo.Tables["passinfo"].Columns.Contains("ticketno")
                    && dsOrderInfo.Tables["passinfo"].Columns.Contains("passname")
                 && dsOrderInfo.Tables["passinfo"].Rows.Count > 0
                )
            {
                DataRowCollection drs = dsOrderInfo.Tables["passinfo"].Rows;
                string ticketNo = "";
                foreach (DataRow dr in drs)
                {
                    ticketNo = dr["ticketno"].ToString().Trim().Replace("--", "-");
                    if (ticketNo != "")
                    {
                        if (ticketNo.Contains("-") && ticketNo.Split('-').Length == 2 && ticketNo.Split('-')[1].Trim() == "")
                        {
                            continue;
                        }
                        if (!resultDic.ContainsKey(dr["passname"].ToString().ToUpper()))
                            resultDic.Add(dr["passname"].ToString().ToUpper(), ticketNo);
                    }
                }
            }

            return resultDic;
        }


        public string GetOrderStatus(string areaCity, string orderId, string outOrderId, string pnr)
        {
            string result = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _PiaoMengAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_PMOrderQuery(outOrderId, accountInfo._pmAccout, accountInfo._pmAg);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0
                && dsOrderInfo.Tables.Contains("resp")
                    && dsOrderInfo.Tables["resp"].Columns.Contains("status")
                 && dsOrderInfo.Tables["resp"].Rows.Count > 0
                )
            {
                result = getOrderStatus(dsOrderInfo.Tables["resp"].Rows[0]["status"].ToString());
            }
            else
            {
                throw new OrderCommException("获取订单状态失败！");
            }
            return result;
        }
        //转换
        private string getOrderStatus(string code)
        {
            NameValueCollection nc = new NameValueCollection();
            nc.Add("|1|", "尚未支付");
            nc.Add("|10|", "等待出票");
            nc.Add("|11|15|16|17|", "订单出票处理中");
            nc.Add("|12|", "无法出票");
            nc.Add("|13|19|", "出票完成");
            nc.Add("|14|", "更换PNR出票完成");
            nc.Add("|20|", "申请退票");
            nc.Add("|21|", "退票处理中");
            nc.Add("|22|", "无法退票");
            nc.Add("|30|", "申请废票");
            nc.Add("|31|", "废票处理中");
            nc.Add("|32|", "无法废票");
            nc.Add("|40|", "申请升舱改期");
            nc.Add("|41|", "升舱改期处理中");
            nc.Add("|42|", "无法改期升舱");
            nc.Add("|43|", "完成改期升舱");
            nc.Add("|90|", "完成退款");
            nc.Add("|99|", "交易取消已退款");
            nc.Add("|18|", "出票失败,暂待客服处理");
            foreach (string key in nc.Keys)
            {
                if (key.Contains("|" + code + "|"))
                {
                    code = nc[key];
                    break;
                }
            }
            return code;
        }


        public bool IsPaid(string areaCity, string orderId, string outOrderId, string pnr)
        {
            bool isPaid = false;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _PiaoMengAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayStatus = m_PTService.PT_PiaoMengGetPayStatus(accountInfo._pmAccout, accountInfo._pmAg, outOrderId);
            if (dsPayStatus != null && dsPayStatus.Tables.Count > 0
              && dsPayStatus.Tables.Contains("Result")
                  && dsPayStatus.Tables["Result"].Columns.Contains("Status")
              && dsPayStatus.Tables["Result"].Rows.Count > 0
              )
            {
                if (dsPayStatus.Tables["Result"].Rows[0]["Status"].ToString() == "T")
                {
                    isPaid = true;
                }
            }
            else
            {
                throw new OrderCommException("未获取到" + this.Code + "订单代付状态");
            }
            return isPaid;
        }
        private _PiaoMengAccountInfo GetInfo(PlatSystem platSystem, string areaCity)
        {
            _PiaoMengAccountInfo _PiaoMengAccount = new _PiaoMengAccountInfo();
            if (platSystem == null)
                throw new CreateInterfaceOrderException("平台开关没有设置！");
            string defaultCity = platSystem.SystemBigArea.DefaultCity;
            //获取区域参数
            SystemArea systemArea = platSystem.SystemBigArea.SystemAreas.Where(p => p.City == areaCity).FirstOrDefault();
            if (systemArea == null)
            {
                systemArea = platSystem.SystemBigArea.SystemAreas.Where(p => p.City == defaultCity).FirstOrDefault();
            }
            if (systemArea == null) throw new CreateInterfaceOrderException(string.Format("没有找到区域为：{0}或默认区域{1}的接口配置项", areaCity, defaultCity));
            //参数集合
            List<AreaParameter> areaParameterList = systemArea.Parameters;
            _PiaoMengAccount._pmAccout = areaParameterList.Where(p => string.Equals(p.Name, "_pmAccout", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _PiaoMengAccount._pmPassword = areaParameterList.Where(p => string.Equals(p.Name, "_pmPassword", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _PiaoMengAccount._pmAg = areaParameterList.Where(p => string.Equals(p.Name, "_pmAg", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _PiaoMengAccount._PayWay = areaParameterList.Where(p => string.Equals(p.Name, "_PayWay", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _PiaoMengAccount._IsChangePNRCP = areaParameterList.Where(p => string.Equals(p.Name, "_IsChangePNRCP", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            return _PiaoMengAccount;
        }

        /// <summary>
        /// 退废票
        /// </summary>
        /// <param name="refundArgs"></param>
        /// <returns></returns>
        public RefundTicketResult BounceOrAnnulTicket(RefundArgs refundArgs)
        {
            string submitStr = string.Empty;
            if (refundArgs.RefundType == 0)
                submitStr = RefundTicket(refundArgs);
            else
                submitStr = AnnulTicket(refundArgs);
            WebHttp http = new WebHttp();
            var result = http.SendRequest(submitStr, MethodType.GET, Encoding.UTF8, 60);
            var returnResult= new RefundTicketResult{ Result = false, Descript = string.Empty };
            if (result == null)
                return returnResult;
            returnResult.Result = result.Data.Contains("提交成功");
            returnResult.Descript = result.Data;
            return returnResult;
        }
        /// <summary>
        /// 退票
        /// </summary>
        private string RefundTicket(RefundArgs refundArgs)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("cmd", "SubmitOrderStatus");
            request.Add("Orderid", refundArgs.OutOrderId);
            request.Add("ticketno", string.Join(";", refundArgs.Passengers.Select(p => p.TicketNo).ToArray()));
            request.Add("status", refundArgs.RefundMoneyType ? "20" : "21");
            request.Add("comment", refundArgs.Reason + refundArgs.Remark);
            request.Add("Refoundfee", "0.00");
            request.Add("Dataformat", "0");
            request.Add("Uid", this.piaoMentAccountInfo._pmAccout);
            request.Add("Pwd", this.piaoMentAccountInfo._pmAg);
            request.Add("xepnr", "1");
            request.Add("ArqueFile", refundArgs.AttachmentUrl);
            return CreateLinkString(request);
        }
        /// <summary>
        /// 废票
        /// </summary>
        private string AnnulTicket(RefundArgs refundArgs)
        {
            Dictionary<string, string> request = new Dictionary<string, string>();
            request.Add("cmd", "SubmitOrderStatus");
            request.Add("orderid", refundArgs.OutOrderId);
            request.Add("ticketno", string.Join(";", string.Join(";", refundArgs.Passengers.Select(p => p.TicketNo).ToArray())));
            request.Add("status", "30");
            request.Add("comment", refundArgs.Reason + refundArgs.Remark);
            request.Add("Uid", piaoMentAccountInfo._pmAccout);
            request.Add("Pwd", piaoMentAccountInfo._pmAg);
            request.Add("xepnr", "1");
            request.Add("ArqueFile", refundArgs.AttachmentUrl);
            return CreateLinkString(request);
        }
        private string CreateLinkString(Dictionary<string, string> dicArray)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendFormat("{0}?", System.Configuration.ConfigurationManager.AppSettings["baseurl"]);
            foreach (KeyValuePair<string, string> item in dicArray)
            {
                sb.AppendFormat("{0}={1}&", item.Key, item.Value);
            }
            //去掉最后一个&字符
            sb.Remove(sb.Length - 1, 1);
            return sb.ToString();
        }

    }
    public class _PiaoMengAccountInfo
    {
        public string _pmAccout { get; set; }
        public string _pmPassword { get; set; }
        public string _pmAg { get; set; }
        public string _PayWay { get; set; }
        public string _IsChangePNRCP { get; set; }

    }
}
