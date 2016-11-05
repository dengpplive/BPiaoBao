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
using System.Xml.Serialization;
using System.IO;
namespace BPiaoBao.DomesticTicket.Platforms._BaiTuo
{
    [Platform("BaiTuo")]
    public class _BaiTuoPlatform : BasePlatform, IPlatform
    {
        private readonly _BaiTuoAccountInfo baiTuoInfo;
        public _BaiTuoPlatform()
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == EnumPlatform._BaiTuo.ToString().Replace("_", "")).FirstOrDefault();
            this.baiTuoInfo = GetInfo(platSystem, string.Empty);
        }
        public override string Code
        {
            get { return "BaiTuo"; }
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
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == EnumPlatform._BaiTuo.ToString().Replace("_", "")).FirstOrDefault();
            List<PlatformPolicy> PolicyList = new List<PlatformPolicy>();
            if (pnrModel == null || pnrModel._LegList.Count == 0 || !platSystem.State)
            {
                return PolicyList;
            }
            PnrAnalysis.Model.LegInfo leg = pnrModel._LegList[0];
            //离起飞时间2小时内屏蔽获取政策接口
            DateTime t = DateTime.Parse(leg.FlyDate1 + " " + leg.FlyStartTime.Insert(2, ":") + ":00");
            t = t.AddHours(-2);
            if (DateTime.Compare(t, System.DateTime.Now) <= 0)
            {
                return PolicyList;
            }
            string area = pnrModel._LegList[0].FromCode;
            _BaiTuoAccountInfo accountInfo = GetInfo(platSystem, area);
            //是否换编码出票 1:是                
            string _IsChangePNRCP = accountInfo._IsChangePNRCP;
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            string Message = string.Empty;
            string CarryCode = pnrModel._CarryCode;
            if (pnrModel._LegList.Count > 0)
            {
                int Index = 0;
                DayOfWeek dayOfWeek = System.DateTime.Now.DayOfWeek;
                switch (dayOfWeek)
                {
                    case DayOfWeek.Monday:
                        Index = 0;
                        break;
                    case DayOfWeek.Tuesday:
                        Index = 1;
                        break;
                    case DayOfWeek.Wednesday:
                        Index = 2;
                        break;
                    case DayOfWeek.Thursday:
                        Index = 3;
                        break;
                    case DayOfWeek.Friday:
                        Index = 4;
                        break;
                    case DayOfWeek.Saturday:
                        Index = 5;
                        break;
                    case DayOfWeek.Sunday:
                        Index = 6;
                        break;
                    default:
                        break;
                }
                DataSet dsPolicy = new DataSet();
                PTMange ptMange = new PTMange();
                Logger.WriteLog(LogType.DEBUG, "百拓获取政策开始时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
                //PTServiceSoapClient m_PTService = new PTServiceSoapClient();
                //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
                //System.Data.DataSet dsPolicy = m_PTService.PT_BaiTuoGetPolicy(_baiTuoAccout, _baiTuoPassword, baiTuoAg, _IsLowerPrice, pnrContent);                
                //System.Data.DataSet dsPolicy = m_PTService.PT_NewBaiTuoGetPolicy(_baiTuoAccout, _baiTuoPassword, baiTuoAg, _IsLowerPrice, pnrContent, PTPnrData);                
                dsPolicy = ptMange._BaiTuoGetPolicy(accountInfo._baiTuoAccout, accountInfo._baiTuoPassword, accountInfo._baiTuoAg, _IsLowerPrice, pnrContent, pnrData);
                Logger.WriteLog(LogType.DEBUG, "百拓获取政策结束时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
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
                            string[] strTimeArr = null;
                            DataRowCollection drs = dsPolicy.Tables[0].Rows;
                            foreach (DataRow dr in drs)
                            {
                                PlatformPolicy policy = new PlatformPolicy();
                                StartTime = "00:00";
                                EndTime = "00:00";
                                policy.Id = dr["Id"] != DBNull.Value ? dr["Id"].ToString() : "";
                                policy.PlatformCode = this.Code;
                                policy.AreaCity = area;
                                if (!string.IsNullOrEmpty(policy.Id))
                                {
                                    policy.IsChangePNRCP = dr["ChangePnr"].ToString() == "1" ? true : false;
                                    policy.IsSp = false;
                                    policy.PolicyType = dr["PolicyType"].ToString() == "2" ? "2" : "1";
                                    policy.PolicyType = policy.PolicyType == "1" ? "BSP" : "B2B";
                                    policy.CarryCode = CarryCode;
                                    if (dr["ProviderWorkTime"].ToString().Split(',').Length == 7 && Index > -1 && Index < 7)
                                    {
                                        strTimeArr = dr["ProviderWorkTime"].ToString().Split(',');
                                        if (strTimeArr[Index].Split('-').Length == 2)
                                        {
                                            StartTime = strTimeArr[Index].Split('-')[0];
                                            EndTime = strTimeArr[Index].Split('-')[1];
                                        }
                                    }
                                    policy.WorkTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                    if (dr["VoidWorkTime"].ToString().Split(',').Length == 7 && Index > -1 && Index < 7)
                                    {
                                        strTimeArr = dr["VoidWorkTime"].ToString().Split(',');
                                        if (strTimeArr[Index].Split('-').Length == 2)
                                        {
                                            StartTime = strTimeArr[Index].Split('-')[0];
                                            EndTime = strTimeArr[Index].Split('-')[1];
                                        }
                                    }
                                    policy.ReturnTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                    policy.AnnulTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                    policy.ReturnMoney = 0m;
                                    policy.CPOffice = dr["Office"].ToString();
                                    policy.Remark = dr["Remark"].ToString();
                                    decimal.TryParse(dr["Rate"].ToString(), out PolicyPoint);
                                    policy.PolicyPoint = PolicyPoint * 100;
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
                            Message = dr_Price["Message"].ToString();
                        }
                    }
                    else
                    {
                        if (dsPolicy.Tables.Contains("Error"))
                        {
                            //失败信息
                            Message = dsPolicy.Tables["Error"].Rows[0]["Error_Text"].ToString();
                        }
                    }
                }
            }
            else
            {
                Message = string.Format("PNR内容解析航段失败:{0}", pnrContent);
            }
            if (!string.IsNullOrEmpty(Message))
            {
                throw new PnrAnalysisFailException(Message);
            }
            return PolicyList;
        }

        public PlatformOrder CreateOrder(string pnrContent, bool IsLowPrice, string areaCity, string policyId, string RateId, string localOrderId, decimal policyPoint, decimal ReturnMoney, BPiaoBao.Common.PnrData pnrData)
        {
            PlatformOrder platformOrder = null;
            string FailMessage = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _BaiTuoAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string _SsrCardType = "1";
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            pnrContent = System.Web.HttpUtility.UrlEncode(pnrContent);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrder = m_PTService.PT_BaiTuoCreateOrder(accountInfo._baiTuoAccout, accountInfo._baiTuoPassword, accountInfo._baiTuoAg, _IsLowerPrice, policyId, localOrderId, _SsrCardType, policyPoint.ToString(), accountInfo._LinkMan, accountInfo._LinkManPhone, pnrContent);
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //DataSet dsOrder = m_PTService.PT_NewBaiTuoCreateOrder(_baiTuoAccout, _baiTuoPassword, _baiTuoAg, _IsLowerPrice, policyId, localOrderId, _SsrCardType, policyPoint.ToString(), _LinkMan, _LinkManPhone, pnrContent, PTPnrData);
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
            _BaiTuoAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            if (accountInfo._PayWay == "1")
            {
                order.PaidMethod = EnumPaidMethod.支付宝;
            }
            else if (accountInfo._PayWay == "2")
            {
                order.PaidMethod = EnumPaidMethod.快钱;
            }
            string OutOrderId = order.OutOrderId;
            string OrderPMFare = order.TotaSeatlPrice.ToString();
            string OutOrderPayMoney = order.TotlePaidPirce.ToString();
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayOrder = m_PTService.PT_BaiTuoOrderPay(accountInfo._baiTuoAccout, accountInfo._baiTuoPassword, accountInfo._baiTuoAg, OutOrderId, OrderPMFare, OutOrderPayMoney, accountInfo._PayWay, accountInfo._NotifyUrl);
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
            get { return false; }
        }

        public void CancelOrder(string areaCity, string outOrderId, string pnr, string CancelRemark, string passengerName)
        {
            throw new OrderCommException("百拓不支持取消订单！");
        }


        public Dictionary<string, string> AutoCompositeTicket(string areaCity, string orderId, string outOrderId, string pnr)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _BaiTuoAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_BaiTuoGetOrderInfo(accountInfo._baiTuoAccout, accountInfo._baiTuoPassword, accountInfo._baiTuoAg, outOrderId);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0
                && dsOrderInfo.Tables.Contains("TICKETINFO")
                && dsOrderInfo.Tables["TICKETINFO"].Columns.Contains("personName")
                && dsOrderInfo.Tables["TICKETINFO"].Rows.Count > 0
                )
            {
                DataRowCollection drs = dsOrderInfo.Tables["TICKETINFO"].Rows;
                string ticketNo = "";
                foreach (DataRow dr in drs)
                {
                    ticketNo = dr["TICKETINFO_Text"].ToString().Trim().Replace("--", "-");
                    if (ticketNo != "")
                    {
                        if (ticketNo.Contains("-") && ticketNo.Split('-').Length == 2 && ticketNo.Split('-')[1].Trim() == "")
                        {
                            continue;
                        }
                        if (!resultDic.ContainsKey(dr["personName"].ToString().ToUpper()))
                            resultDic.Add(dr["personName"].ToString().ToUpper(), ticketNo);
                    }
                }
            }
            return resultDic;
        }


        public string GetOrderStatus(string areaCity, string orderId, string outOrderId, string pnr)
        {
            string result = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _BaiTuoAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_BaiTuoGetOrderInfo(accountInfo._baiTuoAccout, accountInfo._baiTuoPassword, accountInfo._baiTuoAg, outOrderId);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0
                && dsOrderInfo.Tables.Contains("ORDERINFO")
                && dsOrderInfo.Tables["ORDERINFO"].Columns.Contains("Status")
                && dsOrderInfo.Tables["ORDERINFO"].Rows.Count > 0
                )
            {
                result = getOrderStatus(dsOrderInfo.Tables["ORDERINFO"].Rows[0]["Status"].ToString());
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
            _BaiTuoAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayStatus = m_PTService.PT_BaiTuoPayStatus(accountInfo._baiTuoAccout, accountInfo._baiTuoPassword, accountInfo._baiTuoAg, outOrderId);
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

        //转换
        private string getOrderStatus(string code)
        {
            NameValueCollection nc = new NameValueCollection();
            //百拓给出的订单状态对照
            nc.Add("1", "预订成功,等待采购方支付");
            nc.Add("2", "支付成功,等待出票方出票");
            nc.Add("3", "出票成功,等待采购方确认");
            nc.Add("4", "出票成功,交易结束");
            nc.Add("5", "采购方申请退票，等待出票方处理");
            nc.Add("6", "采购方申请废票，等待出票方处理");
            nc.Add("7", "采购方申请退票,待出票方退款");
            nc.Add("8", "采购方申请废票,待出票方退款");
            nc.Add("9", "出票方完成退款,交易结束");
            nc.Add("10", "采购方已取消订单,交易结束");
            nc.Add("11", "出票方已取消订单,交易结束");
            nc.Add("12", "已提交平台处理,请等待平台回复");
            nc.Add("13", "采购方申请改期,待出票方处理");
            nc.Add("14", "改期完成,交易结束");
            nc.Add("15", "废票办理完成,待出票方退款");
            nc.Add("16", "退票办理完成,待出票方退款");
            nc.Add("17", "直接取消订单,待出票方退款");

            foreach (string key in nc.Keys)
            {
                if (key == code)
                {
                    code = nc[key];
                    break;
                }
            }
            return code;
        }


        private _BaiTuoAccountInfo GetInfo(PlatSystem platSystem, string areaCity)
        {
            _BaiTuoAccountInfo _BaiTuoAccount = new _BaiTuoAccountInfo();
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
            _BaiTuoAccount._baiTuoAccout = areaParameterList.Where(p => string.Equals(p.Name, "_baiTuoAccout", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _BaiTuoAccount._baiTuoPassword = areaParameterList.Where(p => string.Equals(p.Name, "_baiTuoPassword", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _BaiTuoAccount._baiTuoAg = areaParameterList.Where(p => string.Equals(p.Name, "_baiTuoAg", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _BaiTuoAccount._PayWay = areaParameterList.Where(p => string.Equals(p.Name, "_PayWay", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _BaiTuoAccount._NotifyUrl = areaParameterList.Where(p => string.Equals(p.Name, "_NotifyUrl", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _BaiTuoAccount._IsChangePNRCP = areaParameterList.Where(p => string.Equals(p.Name, "_IsChangePNRCP", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _BaiTuoAccount._LinkMan = areaParameterList.Where(p => string.Equals(p.Name, "_LinkMan", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _BaiTuoAccount._LinkManPhone = areaParameterList.Where(p => string.Equals(p.Name, "_LinkManPhone", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            return _BaiTuoAccount;
        }


        public RefundTicketResult BounceOrAnnulTicket(RefundArgs refundArgs)
        {
            StringBuilder paramXml = new StringBuilder();
            int refundType = 0;
            if (refundArgs.RefundType == 0)
            {
                refundType = refundArgs.IsVoluntary.Value ? 0 : 2;
            }
            else if (refundArgs.RefundType == 1)
            {
                refundType = 1;
            }
            paramXml.AppendFormat("<Order_Refund_RQ AgentCode=\"{0}\" AgentUserName=\"{1}\" AgentPwd=\"{2}\">", baiTuoInfo._baiTuoAg, baiTuoInfo._baiTuoAccout, baiTuoInfo._baiTuoPassword);
            paramXml.AppendFormat("<RefundOrderInfo ForderformID=\"{0}\" InternationalTicket=\"{1}\" RefundType=\"{2}\" UserFare=\"{3}\" UserFetchInFare=\"{4}\" IsHaveTakeOff=\"{5}\" HaveTakeOffUrl=\"{6}\" RefundSrc=\"{7}\" RefundPortorderId =\"{8}\">", refundArgs.OutOrderId, 0, refundType, 200, 0, string.Empty, refundArgs.AttachmentUrl, 1, string.Empty);
            paramXml.Append("<RefundFlightSegments>");
            //乘机人
            StringBuilder personXml = new StringBuilder();
            refundArgs.Passengers.ForEach(p =>
            {
                personXml.AppendFormat("<PersonInfo PersonName=\"{0}\" PNR=\"{1}\" />", p.PassengerName, refundArgs.PnrCode);
            });
            //退航段
            refundArgs.Sky.ForEach(p =>
            {
                paramXml.AppendFormat("<RefundFlightSegment DepartureAirport=\"{0}\" ArrivalAirport=\"{1}\">", p.FromCityCode, p.ToCityCode);
                paramXml.Append(personXml);

                paramXml.Append("</RefundFlightSegment>");
            });
            paramXml.Append("</RefundFlightSegments>");
            /*非自愿退票原因*/
            if (refundType == 2)
                paramXml.AppendFormat("<Remark>{0}</Remark>", refundArgs.Reason + refundArgs.Remark);
            paramXml.Append("</RefundOrderInfo>");
            paramXml.Append("</Order_Refund_RQ>");
            string resultXml = new _baiTuo.BaitourServiceSoapClient().RefundOrder(paramXml.ToString());
            RefundTicketResult refundResult = new RefundTicketResult();
            refundResult.Result = !resultXml.Contains("Error");
            refundResult.Descript = resultXml;
            return refundResult;
            //Order_Refund_RS resultOrder = null;
            //using (StringReader reader = new StringReader(resultXml))
            //{
            //    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Order_Refund_RS));
            //    resultOrder = (Order_Refund_RS)xmlSerializer.Deserialize(reader);

            //}
        }
    }
    public class _BaiTuoAccountInfo
    {
        /// <summary>
        /// 帐号
        /// </summary>
        public string _baiTuoAccout { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string _baiTuoPassword { get; set; }
        /// <summary>
        /// 密钥
        /// </summary>
        public string _baiTuoAg { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string _PayWay { get; set; }
        /// <summary>
        /// 通知地址
        /// </summary>
        public string _NotifyUrl { get; set; }
        /// <summary>
        /// 换编码出票
        /// </summary>
        public string _IsChangePNRCP { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string _LinkMan { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string _LinkManPhone { get; set; }
    }

    [Serializable]
    public class Order_Refund_RS
    {
        public RefundOrderInfo RefundOrderInfo { get; set; }
        public Error Error { get; set; }
    }
    [Serializable]
    public class RefundOrderInfo
    {
        /// <summary>
        /// 订单号
        /// </summary>
        [XmlAttribute]
        public string ForderformID { get; set; }
        /// <summary>
        /// 国内国际【0：国内 1：国际】
        /// </summary>
        [XmlAttribute]
        public int InternationalTicket { get; set; }
        /// <summary>
        /// 退废票类型
        /// 0 自愿退票
        /// 1 申请废票
        /// 2 非自愿退票
        /// 3 取消订单
        /// </summary>
        [XmlAttribute]
        public int RefundType { get; set; }
        /// <summary>
        /// 接口合作方应退客户金额
        /// </summary>
        [XmlAttribute]
        public decimal UserFare { get; set; }
        [XmlAttribute]
        public int UserFetchInFare { get; set; }
        [XmlAttribute]
        public string RefundPortorderId { get; set; }
        /// <summary>
        /// 平台应退客户金额
        /// </summary>
        [XmlAttribute]
        public decimal RefundSumMoney { get; set; }
        public RefundFlightSegments RefundFlightSegments { get; set; }

    }
    [Serializable]
    public class RefundFlightSegments
    {
        [XmlElement]
        public List<RefundFlightSegment> RefundFlightSegment { get; set; }
    }
    [Serializable]
    public class RefundFlightSegment
    {

        /// <summary>
        /// 出发城市
        /// </summary>
        [XmlAttribute]
        public string DepartureAirport { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        [XmlAttribute]
        public string ArrivalAirport { get; set; }
        /// <summary>
        /// 乘机人
        /// </summary>
        [XmlElement]
        public List<PersonInfo> PersonInfo { get; set; }
    }
    [Serializable]
    public class PersonInfo
    {
        /// <summary>
        /// 乘机人姓名
        /// </summary>
        [XmlAttribute]
        public string PersonName { get; set; }
        /// <summary>
        /// 应退金额
        /// </summary>
        [XmlAttribute]
        public decimal RefundMoney { get; set; }
    }
    [Serializable]
    public class Error
    {
        [XmlAttribute]
        public string Code { get; set; }
        [XmlText]
        public string Description { get; set; }
    }

}
