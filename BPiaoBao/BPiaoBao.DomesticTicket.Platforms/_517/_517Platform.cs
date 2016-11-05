using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.DomesticTicket.Platforms._PTService;
using BPiaoBao.Common;
using JoveZhao.Framework;
using JoveZhao.Framework.Expand;
using PnrAnalysis;
using AutoMapper;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using BPiaoBao.DomesticTicket.Platforms.PTInterface;
using BPiaoBao.Common.Enums;
using System.Security.Cryptography;
using System.Xml.Linq;
namespace BPiaoBao.DomesticTicket.Platforms._517
{
    [Platform("517")]
    public class _517Platform : BasePlatform, IPlatform
    {
        private readonly _517AccountInfo _517param = null;
        public _517Platform()
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            this._517param = GetInfo(platSystem, string.Empty);
        }
        public override string Code
        {
            get { return "517"; }
        }
        //是否支持取消订单接口
        public bool CanCancelOrder
        {
            get { return true; }
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
            //REPEAT_ORDERS_CAN_NOT_BE_GENERATED
            else if (strErrMessage.Contains("服务器无法处理请求"))
            {
                strResult = "服务器无法处理请求";
            }
            else if (strErrMessage.Contains("USER_PAY_TYPE_MISMATCH"))
            {
                strResult = "代扣账号金额不足【" + strErrMessage + "】";
            }
            else if (strErrMessage.Contains("REPEAT_ORDERS_CAN_NOT_BE_GENERATED"))
            {
                strResult = "接口订单不能重复生成";
            }
            else if (strErrMessage.Contains("OVER_TIME_FOR_PAYMENT"))
            {
                strResult = "超过支付时间，不能支付!";
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
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == EnumPlatform._517.ToString().Replace("_", "")).FirstOrDefault();
            List<PlatformPolicy> PolicyList = new List<PlatformPolicy>();
            if (pnrModel == null || pnrModel._LegList.Count == 0 || platSystem == null || !platSystem.State)
            {
                return PolicyList;
            }
            string area = pnrModel._LegList[0].FromCode;
            _517AccountInfo accountInfo = GetInfo(platSystem, area);
            //是否换编码出票 1:是                
            string _IsChangePNRCP = accountInfo._IsChangePNRCP;
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            PTMange ptMange = new PTMange();
            DataSet dsPolicy = new DataSet();
            Logger.WriteLog(LogType.DEBUG, "517开始时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
            //PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            //System.Data.DataSet dsPolicy = m_PTService.PT_517GetPolicy(_517Accout, _517Password, _517Ag, _IsLowerPrice, pnrContent);
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);            
            //dsPolicy= m_PTService.PT_New517GetPolicy(_517Accout, _517Password, _517Ag, _IsLowerPrice, pnrContent, PTPnrData);
            dsPolicy = ptMange._517GetPolicy(accountInfo._517Accout, accountInfo._517Password, accountInfo._517Ag, _IsLowerPrice, pnrContent, pnrData);
            Logger.WriteLog(LogType.DEBUG, "517结束时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
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
                        DataRowCollection drs = dsPolicy.Tables[0].Rows;
                        string StartTime = "00:00", EndTime = "00:00";
                        decimal PolicyPoint = 0m;
                        foreach (DataRow dr in drs)
                        {
                            PlatformPolicy policy = new PlatformPolicy();
                            policy.AreaCity = area;
                            StartTime = "00:00";
                            EndTime = "00:00";
                            policy.PlatformCode = this.Code;
                            policy.Id = dr["PolicyID"].ToString() + "~" + dr["PolicyChildID"].ToString();
                            if (policy.Id != "~")
                            {
                                //IsChangePNRCP         //是否换编码出票
                                //IsSp                  //是否是特殊政策
                                //CarryCode             //航空格式二字码
                                //TravelType            //1单程  2单程/往返  3往返 4联程
                                //PolicyType            //政策类型1.BSP 2.B2B
                                //FromCity              //出发城市二字码
                                //ToCity                //到达城市二字码
                                //FlightType            //航班适用类型 0.适用全部 1.适用航班 2.不适用航班
                                //ScheduleConstraints   //班期限制
                                //Space                 //舱位
                                //EffectDate            //政策开始生效期
                                //ExpirationDate         //政策结束生效期
                                //Remark                //政策备注
                                //Policy                //政策点数
                                //GYOnlineTime          //供应工作时间
                                //GYFPTime              //退废票时间
                                //Office                //Office号                            
                                bool IsChangePNRCP = false;
                                policy.CarryCode = dr["CarryCode"].ToString();
                                bool.TryParse(dr["IsChangePNRCP"].ToString(), out IsChangePNRCP);
                                policy.IsChangePNRCP = IsChangePNRCP;
                                policy.IsSp = dr["IsSp"].ToString() == "1" ? true : false;
                                policy.PolicyType = dr["PolicyType"].ToString() == "1" ? "BSP" : "B2B";
                                if (dr["GYOnlineTime"].ToString().Split('-').Length == 2)
                                {
                                    StartTime = dr["GYOnlineTime"].ToString().Split('-')[0];
                                    EndTime = dr["GYOnlineTime"].ToString().Split('-')[1];
                                }
                                policy.WorkTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };

                                var now = DateTime.Now;
                                string GYFPTime = "";
                                if (now.DayOfWeek == DayOfWeek.Sunday || now.DayOfWeek == DayOfWeek.Saturday)
                                {
                                    GYFPTime = dr["GYFPTimeNew"].ToString();
                                }
                                else
                                {
                                    GYFPTime = dr["GYFPTime"].ToString();
                                }
                                if (GYFPTime.Split('-').Length == 2)
                                {
                                    StartTime = GYFPTime.ToString().Split('-')[0];
                                    EndTime = GYFPTime.ToString().Split('-')[1];
                                }
                                policy.ReturnTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                policy.AnnulTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                policy.ReturnMoney = 0m;
                                policy.CPOffice = dr["Office"].ToString();
                                policy.Remark = dr["Remark"].ToString().Contains("改期收回代理费") ? dr["Remark"].ToString() : dr["Remark"].ToString() + ",改期收回代理费";
                                decimal.TryParse(dr["Policy"].ToString(), out PolicyPoint);
                                policy.PolicyPoint = PolicyPoint;
                                if (_IsChangePNRCP != "1" && policy.IsChangePNRCP)
                                {
                                    continue;
                                }
                                string IssueSpeed = dr["ChupPiaoXiaolu"] == DBNull.Value ? "" : dr["ChupPiaoXiaolu"].ToString();
                                int _IssueSpeed = 0;
                                if (!string.IsNullOrEmpty(IssueSpeed) && int.TryParse(IssueSpeed, out _IssueSpeed))
                                {
                                    policy.IssueSpeed = _IssueSpeed <= 20 ? "极速" : _IssueSpeed + "秒";
                                }
                                else
                                {
                                    policy.IssueSpeed = platSystem != null ? platSystem.IssueTicketSpeed : "";
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
            _517AccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            pnrContent = System.Web.HttpUtility.UrlEncode(pnrContent);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrder = m_PTService.PT_517CreateOrder(accountInfo._517Accout, accountInfo._517Password, accountInfo._517Ag, _IsLowerPrice, pnrContent, policyId, accountInfo._LinkMan, accountInfo._LinkManPhone);
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
                    //FailMessage = "生成订单失败！";//dr["Message"].ToString();
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
            _517AccountInfo accountInfo = GetInfo(platSystem, areaCity);
            if (accountInfo._PayWay == "1")
            {
                order.PaidMethod = EnumPaidMethod.预存款;
            }
            else
            {
                order.PaidMethod = EnumPaidMethod.支付宝;
            }
            string OutOrderId = order.OutOrderId;
            string OutOrderPayMoney = order.TotlePaidPirce.ToString();
            string PNR = order.PnrCode;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayOrder = m_PTService.PT_517OrderPay(accountInfo._517Accout, accountInfo._517Password, accountInfo._517Ag, accountInfo._PayAccout517, accountInfo._PayPassword517, OutOrderId, OutOrderPayMoney, accountInfo._NotifyUrl, PNR, accountInfo._PayWay);
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
        public void CancelOrder(string areaCity, string outOrderId, string pnr, string CancelRemark, string passengerName)
        {
            string FailMessage = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _517AccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsCancelOrder = m_PTService.PT_517CancelOrder(accountInfo._517Accout, accountInfo._517Password, accountInfo._517Ag, outOrderId);
            if (dsCancelOrder != null && dsCancelOrder.Tables.Contains("Result") &&
                dsCancelOrder.Tables["Result"].Rows.Count > 0
                 && dsCancelOrder.Tables["Result"].Rows[0]["Success"].ToString().ToUpper() == "TRUE"
                && dsCancelOrder.Tables["Result"].Rows[0]["OrderID"].ToString().Trim() == outOrderId.Trim()
                )
            {
                //取消成功
            }
            else
            {
                FailMessage = "取消订单失败！";
            }
            if (!string.IsNullOrEmpty(FailMessage))
            {
                throw new OrderCommException(ErrToMessage(FailMessage));
            }
        }

        public Dictionary<string, string> AutoCompositeTicket(string areaCity, string orderId, string outOrderId, string pnr)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _517AccountInfo accountInfo = GetInfo(platSystem, areaCity);
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_517GetOrderInfo(accountInfo._517Accout, accountInfo._517Password, accountInfo._517Ag, outOrderId, pnr);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0
                && dsOrderInfo.Tables.Contains("Passenger")
                    && dsOrderInfo.Tables["Passenger"].Columns.Contains("Name")
                 && dsOrderInfo.Tables["Passenger"].Rows.Count > 0
                 && dsOrderInfo.Tables["OrderInfo"].Rows.Count > 0
                )
            {
                DataRowCollection drs = dsOrderInfo.Tables["Passenger"].Rows;
                string ticketNo = "";
                foreach (DataRow dr in drs)
                {
                    ticketNo = dr["TicketNo"].ToString().Trim().Replace("--", "-");
                    if (ticketNo != "")
                    {
                        if (ticketNo.Contains("-") && ticketNo.Split('-').Length == 2 && ticketNo.Split('-')[1].Trim() == "")
                        {
                            continue;
                        }
                        if (!resultDic.ContainsKey(dr["Name"].ToString().ToUpper()))
                            resultDic.Add(dr["Name"].ToString().ToUpper(), ticketNo);
                    }
                }
            }
            return resultDic;
        }

        public string GetOrderStatus(string areaCity, string orderId, string outOrderId, string pnr)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _517AccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string result = string.Empty;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_517GetOrderInfo(accountInfo._517Accout, accountInfo._517Password, accountInfo._517Ag, outOrderId, pnr);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0
                && dsOrderInfo.Tables.Contains("OrderInfo")
                    && dsOrderInfo.Tables["OrderInfo"].Columns.Contains("OrderStatus")
                 && dsOrderInfo.Tables["OrderInfo"].Rows.Count > 0
                )
            {
                result = dsOrderInfo.Tables["OrderInfo"].Rows[0]["OrderStatus"].ToString();
            }
            else
            {
                throw new OrderCommException("获取订单状态失败！");
            }
            return result;
        }


        public bool IsPaid(string areaCity, string orderId, string outOrderId, string pnr)
        {
            bool isPaid = false;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _517AccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayStatus = m_PTService.PT_517GetPayStatus(accountInfo._517Accout, accountInfo._517Password, accountInfo._517Ag, outOrderId, pnr);
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

        private _517AccountInfo GetInfo(PlatSystem platSystem, string areaCity)
        {
            _517AccountInfo _517Account = new _517AccountInfo();
            if (platSystem == null)
                throw new CreateInterfaceOrderException("平台开关没有设置！");
            string defaultCity = platSystem.SystemBigArea.DefaultCity;
            //获取区域参数
            SystemArea systemArea = platSystem.SystemBigArea.SystemAreas.Where(p => string.Equals(p.City, areaCity, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
            if (systemArea == null)
            {
                systemArea = platSystem.SystemBigArea.SystemAreas.Where(p => p.City == defaultCity).FirstOrDefault();
            }
            if (systemArea == null) throw new CreateInterfaceOrderException(string.Format("没有找到区域为：{0}或默认区域{1}的接口配置项", areaCity, defaultCity));
            //参数集合
            List<AreaParameter> areaParameterList = systemArea.Parameters;
            _517Account._517Accout = areaParameterList.Where(p => string.Equals(p.Name, "_517Accout", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _517Account._517Password = areaParameterList.Where(p => string.Equals(p.Name, "_517Password", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _517Account._517Ag = areaParameterList.Where(p => string.Equals(p.Name, "_517Ag", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _517Account._PayAccout517 = areaParameterList.Where(p => string.Equals(p.Name, "_PayAccout517", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _517Account._PayPassword517 = areaParameterList.Where(p => string.Equals(p.Name, "_PayPassword517", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _517Account._PayWay = areaParameterList.Where(p => string.Equals(p.Name, "_PayWay", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _517Account._NotifyUrl = areaParameterList.Where(p => string.Equals(p.Name, "_NotifyUrl", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _517Account._LinkMan = areaParameterList.Where(p => string.Equals(p.Name, "_LinkMan", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _517Account._LinkManPhone = areaParameterList.Where(p => string.Equals(p.Name, "_LinkManPhone", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            //是否换编码出票 1:是                
            _517Account._IsChangePNRCP = areaParameterList.Where(p => string.Equals(p.Name, "_IsChangePNRCP", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            return _517Account;
        }
        public RefundTicketResult BounceOrAnnulTicket(RefundArgs refundArgs)
        {
            int refundType = refundArgs.RefundType == 0 ? 1 : 0;
            StringBuilder paramXml = new StringBuilder();
            paramXml.AppendFormat("<refundtype>{0}</refundtype>", refundType);
            paramXml.Append(refundType == 1 ? RefundTicket(refundArgs) : AnnulTicket(refundArgs));
            string resultStr = new _517Policy.BenefitInterfaceSoapClient().InterfaceFacade(GenerParamXml("refund_invalidate_ticket", paramXml.ToString()));
            RefundTicketResult result = new RefundTicketResult();
            result.Result = resultStr.Contains("error") ? true : bool.Parse(XDocument.Parse(resultStr, LoadOptions.None).Element("VoidResult").Attribute("VoidSuccess").Value);
            result.Descript = resultStr;
            return result;
        }
        /// <summary>
        /// 退票接口实现
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public string RefundTicket(RefundArgs refundArgs)
        {
            StringBuilder paramXml = new StringBuilder();
            paramXml.Append("<refundorder>");
            paramXml.AppendFormat("<orderid>{0}</orderid>", refundArgs.OutOrderId);
            paramXml.AppendFormat("<poundagedealtype>{0}</poundagedealtype>", 1);
            paramXml.Append("<passengers>");
            paramXml.Append("<passengertype>0</passengertype>");
            refundArgs.Passengers.ForEach(p =>
            {
                paramXml.Append("<passenger>");
                paramXml.AppendFormat("<name>{0}</name>", p.PassengerName);
                paramXml.AppendFormat("<ticketno>{0}</ticketno>", p.TicketNo);
                paramXml.AppendFormat("<credentialid>{0}</credentialid>", p.CardNo);
                paramXml.AppendFormat("<poundage>{0}</poundage>", -1);
                paramXml.AppendFormat("<type>{0}</type>", (int)p.PassengerType);
                paramXml.Append("</passenger>");
            });


            paramXml.Append("</passengers>");

            paramXml.Append("<applyreason>");
            paramXml.AppendFormat("<reasonid>{0}</reasonid>", refundArgs.Guid.Split('|')[0]);
            paramXml.AppendFormat("<reasondetails>{0}</reasondetails>", refundArgs.Reason);
            paramXml.AppendFormat("<reasonremark>{0}</reasonmark>", refundArgs.Remark);

            paramXml.Append(GetExtendNode(refundArgs));

            paramXml.Append("</applyreason>");

            paramXml.Append("</refundorder>");

            return null;
        }
        private string GetExtendNode(RefundArgs refundArgs)
        {
            StringBuilder extendXml = new StringBuilder();
            extendXml.Append("<extends>");
            switch (refundArgs.Guid.Split('|')[0].ToString())
            {
                case "fcd8bd2f-44d4-4c66-968a-18be4afcdd50":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>83b6849f-a2e8-493e-bdb8-7bc3c9d8198a</id>");
                        extendXml.Append("<type>0</type>");
                        extendXml.AppendFormat("<remark>升舱后票号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", p.TicketNo);
                        extendXml.Append("</extend>");
                    });

                    break;
                case "8f964dd0-efbf-4659-8cbf-8709d91dc2fc":
                    refundArgs.Passengers.ForEach(p =>
                   {
                       extendXml.Append("<extend>");
                       extendXml.Append("<id>3b20c577-8d7d-489c-bc55-04c4675a9e2e</id>");
                       extendXml.Append("<type>1</type>");
                       extendXml.AppendFormat("<remark>快递单号</remark>");
                       extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                       extendXml.AppendFormat("<checkvalue></checkvalue>");
                       extendXml.Append("</extend>");
                   });
                    break;
                case "fdeb423a-1669-4e90-8c86-0c39808c51a7":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>b7d98434-7b05-4c66-8c5f-25ae00355f25</id>");
                        extendXml.Append("<type>0</type>");
                        extendXml.AppendFormat("<remark>换开后票号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", p.TicketNo);
                        extendXml.Append("</extend>");

                        extendXml.Append("<extend>");
                        extendXml.Append("<id>50118d8c-f0ea-4bd8-8321-68134b6614a2</id>");
                        extendXml.Append("<type>1</type>");
                        extendXml.AppendFormat("<remark>川航金卡卡号</remark>");
                        extendXml.Append("<passengertype></passengertype>");
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>");
                        extendXml.Append("</extend>");

                    });
                    break;
                case "a0d51b8d-802a-4239-9d7a-419ac483d7a9":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>33131723-c470-4d65-98d0-2c8d01484064</id>");
                        extendXml.Append("<type>0</type>");
                        extendXml.AppendFormat("<remark>换开后票号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", p.TicketNo);
                        extendXml.Append("</extend>");
                    });
                    break;
                case "ac58e70d-f87e-4c6c-a906-c8fec5e34ce3":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>2fff504f-f30f-4944-aea5-20afd5b4a4a9</id>");
                        extendXml.Append("<type>0</type>");
                        extendXml.AppendFormat("<remark>前端票号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", p.TicketNo);
                        extendXml.Append("</extend>");
                    });
                    break;
                case "7f81addc-9c4e-4602-8147-426934ae178a":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>03363334-a628-4df0-a6c2-8f1167265997</id>");
                        extendXml.Append("<type>1</type>");
                        extendXml.AppendFormat("<remark>理由</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", refundArgs.Remark);
                        extendXml.Append("</extend>");
                    });
                    break;
                case "4e434c16-3986-4b5e-88ce-75a0c3d82493":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>6e8f7d6e-c246-4cdc-903d-b993f109a6eb</id>");
                        extendXml.Append("<type>0</type>");
                        extendXml.AppendFormat("<remark>升舱后票号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", p.TicketNo);
                        extendXml.Append("</extend>");
                    });
                    break;
                case "54838210-a6d9-44ad-a544-7255e8ef3cdf":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>4e0b4e35-d4b8-4991-a427-d3479230475c</id>");
                        extendXml.Append("<type>0</type>");
                        extendXml.AppendFormat("<remark>重构票号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", p.TicketNo);
                        extendXml.Append("</extend>");
                    });
                    break;
                case "f3b7ee91-f683-4349-a0ab-6b7e2ac6d223":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>886d7116-963a-4aff-89c6-127a8c8a44ff</id>");
                        extendXml.Append("<type>0</type>");
                        extendXml.AppendFormat("<remark>名字正确的票号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", p.TicketNo);
                        extendXml.Append("</extend>");
                    });
                    break;
                case "15cfb5d6-3c8e-49b9-b276-37de3ab940d9":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>15cfb5d6-3c8e-49b9-b276-37de3ab940d9</id>");
                        extendXml.Append("<type>1</type>");
                        extendXml.AppendFormat("<remark>快递单号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue></checkvalue>");
                        extendXml.Append("</extend>");
                    });
                    break;
                case "3b98b570-24f5-4037-ae54-732a788110c2":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>4afc1427-c952-4412-99ed-1cf6bf347667</id>");
                        extendXml.Append("<type>1</type>");
                        extendXml.AppendFormat("<remark>情况说明</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", refundArgs.Remark);
                        extendXml.Append("</extend>");
                    });
                    break;
                case "4527f672-9b08-4404-8b88-91242d6b0dea":
                    refundArgs.Passengers.ForEach(p =>
                   {
                       extendXml.Append("<extend>");
                       extendXml.Append("<id>434d9a24-a946-47d0-a64d-c41df5c4114f</id>");
                       extendXml.Append("<type>1</type>");
                       extendXml.AppendFormat("<remark>情况说明</remark>");
                       extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                       extendXml.AppendFormat("<checkvalue>{0}</checkvalue>", refundArgs.Remark);
                       extendXml.Append("</extend>");
                   });
                    break;
                case "43e45e71-110a-460d-9168-c72f1dfff77c":
                    refundArgs.Passengers.ForEach(p =>
                    {
                        extendXml.Append("<extend>");
                        extendXml.Append("<id>ebfcb946-2778-47d0-867d-88b5e234bd9e</id>");
                        extendXml.Append("<type>1</type>");
                        extendXml.AppendFormat("<remark>海航金(银)卡号</remark>");
                        extendXml.AppendFormat("<passengertype>{0}</passengertype>", (int)p.PassengerType);
                        extendXml.AppendFormat("<checkvalue></checkvalue>");
                        extendXml.Append("</extend>");
                    });
                    break;
                default:
                    break;
            }
            extendXml.Append("</extends>");
            return extendXml.ToString();
        }
        /// <summary>
        /// 废票接口
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public string AnnulTicket(RefundArgs refundArgs)
        {
            StringBuilder paramXml = new StringBuilder();
            paramXml.Append("<voidorder>");
            paramXml.AppendFormat("<orderid>{0}</orderid>", refundArgs.OutOrderId);
            paramXml.AppendFormat("<poundagedealtype>{0}</poundagedealtype>", 1);

            paramXml.Append("<passengers>");
            paramXml.AppendFormat("<passengertype>0</passengertype>");

            refundArgs.Passengers.ForEach(p =>
            {
                paramXml.Append("<passenger>");
                paramXml.AppendFormat("<name>{0}</name>", p.PassengerName);
                paramXml.AppendFormat("<ticketno>{0}</ticketno>", p.TicketNo);
                paramXml.AppendFormat("<credentialid>{0}</credentialid>", p.CardNo);
                paramXml.AppendFormat("<poundage>{0}</poundage>", -1);
                paramXml.AppendFormat("<type>{0}</type>", (int)p.PassengerType);
                paramXml.Append("</passenger>");
            });

            paramXml.Append("</passengers>");

            paramXml.Append("<applyreason>");
            paramXml.AppendFormat("<reasonid>{0}</reasonid>", refundArgs.Guid.Split('|')[0]);
            paramXml.AppendFormat("<reasondetails>{0}</reasondetails>", refundArgs.Reason);
            paramXml.AppendFormat("<reasonremark>{0}</reasonremark>", refundArgs.Remark);
            paramXml.Append("</applyreason>");

            paramXml.Append("</voidorder>");
            return paramXml.ToString();
        }
        /// <summary>
        /// 退票订单查询
        /// </summary>
        /// <param name="args"></param>
        public void QueryRefund(string orderid,List<RefundPassenger> list)
        {
            StringBuilder paramXml = new StringBuilder();
            paramXml.AppendFormat("<orderid>{0}</orderid>",orderid);
            list.ForEach(p => paramXml.AppendFormat("<ticketno>{0}</ticketno>", p.TicketNo));
            
          string xml=  new _517Policy.BenefitInterfaceSoapClient().InterfaceFacade(GenerParamXml("get_refund_ticket", paramXml.ToString()));
          Console.WriteLine(xml);
        }
        public void QueryAnnul(string orderid, List<RefundPassenger> list)
        {
            StringBuilder paramXml = new StringBuilder();
            paramXml.AppendFormat("<orderid>{0}</orderid>",orderid);
            list.ForEach(p => paramXml.AppendFormat("<ticketno>{0}</ticketno>", p.TicketNo));
           string xml= new _517Policy.BenefitInterfaceSoapClient().InterfaceFacade(GenerParamXml("get_void_ticket", paramXml.ToString()));
           Console.WriteLine(xml);
        }
        /// <summary>
        /// 生成提交参数文档
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        private string GenerParamXml(string service, string param)
        {
            string sign = string.Format("{0}{1}{2}{3}{4}{5}", param, _517param._517Accout, _517param._517Password.Md5().ToUpper(), DateTime.Now.ToString("yyyy-MM-dd"), _517param._517Ag, _517param.PID).Md5().ToUpper();
            StringBuilder sb = new StringBuilder();
            sb.Append("<?xml version=\"1.0\" encoding=\"utf-8\" ?>");
            sb.Append("<request>");
            sb.AppendFormat("<service>{0}</service>", service);
            sb.AppendFormat("<pid>{0}</pid>", _517param.PID);
            sb.AppendFormat("<username>{0}</username>", _517param._517Accout);
            sb.AppendFormat("<sign>{0}</sign>", sign);
            sb.AppendFormat("<params>{0}</params>", param);
            sb.Append("</request>");
            return sb.ToString();
        }
    }

    public class _517AccountInfo
    {
        public string _517Accout { get; set; }
        public string _517Password { get; set; }
        public string _517Ag { get; set; }
        public string _PayAccout517 { get; set; }
        public string _PayPassword517 { get; set; }
        public string _PayWay { get; set; }
        public string _NotifyUrl { get; set; }
        public string _LinkMan { get; set; }
        public string _LinkManPhone { get; set; }
        public string _IsChangePNRCP { get; set; }
        public string PID = "7008600281211297088";
    }
}
