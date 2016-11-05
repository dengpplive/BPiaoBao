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
using PnrAnalysis;
using AutoMapper;
using BPiaoBao.DomesticTicket.Platforms.PTInterface;
using BPiaoBao.Common.Enums;
namespace BPiaoBao.DomesticTicket.Platforms._51Book
{
    [Platform("51Book")]
    public class _51BookPlatform : BasePlatform, IPlatform
    {
        public override string Code
        {
            get { return "51Book"; }
        }
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
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            List<PlatformPolicy> PolicyList = new List<PlatformPolicy>();
            if (pnrModel == null || pnrModel._LegList.Count == 0 || !platSystem.State)
            {
                return PolicyList;
            }
            //获取区域对象
            string area = pnrModel._LegList[0].FromCode;
            _51BookAccountInfo accountInfo = GetInfo(platSystem, area);
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            //是否换编码出票 1:是                
            string _IsChangePNRCP = accountInfo._IsChangePNRCP;
            PTMange ptMange = new PTMange();
            DataSet dsPolicy = new DataSet();
            Logger.WriteLog(LogType.DEBUG, "51Book开始时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
            //PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //System.Data.DataSet dsPolicy = m_PTService.PT_51BookGetPolicy(_51bookAccout, _51bookPassword, _51bookAg, _IsLowerPrice, pnrContent);            
            //System.Data.DataSet dsPolicy = m_PTService.PT_New51BookGetPolicy(_51bookAccout, _51bookPassword, _51bookAg, _IsLowerPrice, pnrContent, PTPnrData);            
            dsPolicy = ptMange._51BookGetPolicy(accountInfo._51bookAccout, accountInfo._51bookPassword, accountInfo._51bookAg, _IsLowerPrice, pnrContent, pnrData);
            Logger.WriteLog(LogType.DEBUG, "51Book结束时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");

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
                            policy.Id = dr["Id"] != DBNull.Value ? dr["Id"].ToString() : "";
                            policy.PlatformCode = this.Code;
                            policy.AreaCity = area;
                            if (!string.IsNullOrEmpty(policy.Id))
                            {
                                //startDate                         政策开始生效日期
                                //expiredDate                       政策结束生效日期
                                //printTicketStartDate              政策出票开始生效日期
                                //printTicketExpiredDate            政策出票结束生效日期
                                //needSwitchPNR                     是否换编码出票 true是 false否
                                //routeType                         行程类型 OW单程 RT往返 否则联程
                                //businessUnitType                  是否是特殊政策  非0即是
                                //airlineCode                       航空格式二字码
                                //policyType                        B2P B2B
                                //flightCourse                      出发生成三字码 为"999-999"表示所有城市 格式:"出发城市三字码-到达城市三字码"
                                //flightNoIncluding                 适用航班号
                                //flightNoExclude                   不适用航班号
                                //flightCycle                       班期限制
                                //seatClass                         舱位
                                //comment                           政策备注
                                //Commission                        政策点数
                                //workTime                          供应工作时间
                                //chooseOutWorkTime                 退废票时间
                                //param2                            Office号
                                bool IsChangePNRCP = false;
                                bool.TryParse(dr["needSwitchPNR"].ToString(), out IsChangePNRCP);
                                policy.IsChangePNRCP = IsChangePNRCP;
                                policy.CarryCode = dr["airlineCode"].ToString();
                                policy.IsSp = dr["businessUnitType"].ToString() == "1" ? true : false;
                                policy.PolicyType = string.Compare(dr["PolicyType"].ToString(), "B2P", true) == 0 ? "2" : "1";


                                policy.PolicyType = policy.PolicyType == "1" ? "BSP" : "B2B";
                                if (dr["workTime"].ToString().Split('-').Length == 2)
                                {
                                    StartTime = dr["workTime"].ToString().Split('-')[0];
                                    EndTime = dr["workTime"].ToString().Split('-')[1];
                                }
                                policy.WorkTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                if (dr["chooseOutWorkTime"].ToString().Split('-').Length == 2)
                                {
                                    StartTime = dr["chooseOutWorkTime"].ToString().Split('-')[0];
                                    EndTime = dr["chooseOutWorkTime"].ToString().Split('-')[1];
                                }
                                policy.ReturnTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                policy.AnnulTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                decimal.TryParse(dr["Commission"].ToString(), out PolicyPoint);
                                policy.PolicyPoint = PolicyPoint;
                                policy.ReturnMoney = 0m;
                                policy.CPOffice = dr["param2"].ToString();
                                policy.Remark = dr["comment"].ToString();
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
            //转化
            return PolicyList;
        }

        public PlatformOrder CreateOrder(string pnrContent, bool IsLowPrice, string areaCity, string policyId, string RateId, string localOrderId, decimal policyPoint, decimal ReturnMoney, BPiaoBao.Common.PnrData pnrData)
        {
            PlatformOrder platformOrder = null;
            string FailMessage = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _51BookAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            pnrContent = System.Web.HttpUtility.UrlEncode(pnrContent);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrder = m_PTService.PT_51BookCreateOrder(accountInfo._51bookAccout, accountInfo._51bookPassword, accountInfo._51bookAg, _IsLowerPrice, policyId, accountInfo._NotifyUrl, accountInfo._b2cCreatorCn, pnrContent);
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //DataSet dsOrder = m_PTService.PT_New51BookCreateOrder(_51bookAccout, _51bookPassword, _51bookAg, _IsLowerPrice, policyId, _NotifyUrl, _b2cCreatorCn, pnrContent, PTPnrData);
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
            _51BookAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            if (accountInfo._PayWay == "0")
            {
                order.PaidMethod = EnumPaidMethod.支付宝;
            }
            string OutOrderId = order.OutOrderId;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayOrder = m_PTService.PT_51BookOrderPay(accountInfo._51bookAccout, accountInfo._51bookPassword, accountInfo._51bookAg, OutOrderId);
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




        public void CancelOrder(string areaCity, string outOrderId, string pnr, string cancelRemark, string passengerName)
        {
            string FailMessage = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _51BookAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsCancelOrder = m_PTService.PT_51bookCancelOrder(accountInfo._51bookAccout, accountInfo._51bookAg, outOrderId);
            if (dsCancelOrder != null && dsCancelOrder.Tables.Contains("QueryOrderStatus")
                && dsCancelOrder.Tables["QueryOrderStatus"].Rows.Count > 0
                )
            {
                if (dsCancelOrder.Tables["QueryOrderStatus"].Rows[0]["CancelStatus"].ToString().ToUpper().Trim() == "T")
                {
                    //成功
                }
                else
                {
                    FailMessage = dsCancelOrder.Tables["QueryOrderStatus"].Rows[0]["Err"].ToString();
                }
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
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _51BookAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsTicketInfo = m_PTService.PT_51BookGetPayAndPassengerInfo(accountInfo._51bookAccout, outOrderId, "1", accountInfo._51bookAg);
            if (dsTicketInfo != null && dsTicketInfo.Tables.Contains("Datas")
                && dsTicketInfo.Tables["Datas"].Rows.Count > 0
                )
            {
                DataRowCollection drs = dsTicketInfo.Tables["Datas"].Rows;
                string ticketNo = "";
                foreach (DataRow dr in drs)
                {
                    ticketNo = dr["ticketNo"].ToString().Trim().Replace("--", "-");
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
            string result = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _51BookAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            string FailMessage = string.Empty;
            DataSet dsOrderInfo = m_PTService.PT_51BookGetOrderInfo(accountInfo._51bookAccout, outOrderId, accountInfo._statusType, accountInfo._51bookAg);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0
                && dsOrderInfo.Tables[0].Rows.Count > 0)
            {
                if (dsOrderInfo.Tables[0].Columns.Contains("orderStatus"))
                {
                    result = dsOrderInfo.Tables[0].Rows[0]["orderStatus"].ToString();
                }
                else if (dsOrderInfo.Tables[0].Columns.Contains("ErorrMessage"))
                {
                    FailMessage = dsOrderInfo.Tables[0].Rows[0]["ErorrMessage"].ToString();
                }
            }
            else
            {
                FailMessage = "获取订单状态失败！";
            }
            if (!string.IsNullOrEmpty(FailMessage))
            {
                throw new OrderCommException(ErrToMessage(FailMessage));
            }
            return result;
        }


        public bool IsPaid(string areaCity, string orderId, string outOrderId, string pnr)
        {
            bool isPaid = false;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _51BookAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayStatus = m_PTService.PT_51BookGetPayStatus(accountInfo._51bookAccout, outOrderId, accountInfo._51bookAg);
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
        private _51BookAccountInfo GetInfo(PlatSystem platSystem, string areaCity)
        {
            _51BookAccountInfo _51BookAccount = new _51BookAccountInfo();
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
            _51BookAccount._51bookAccout = areaParameterList.Where(p => string.Equals(p.Name, "_51bookAccout", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _51BookAccount._51bookPassword = areaParameterList.Where(p => string.Equals(p.Name, "_51bookPassword", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _51BookAccount._51bookAg = areaParameterList.Where(p => string.Equals(p.Name, "_51bookAg", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _51BookAccount._PayWay = areaParameterList.Where(p => string.Equals(p.Name, "_PayWay", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _51BookAccount._IsChangePNRCP = areaParameterList.Where(p => string.Equals(p.Name, "_IsChangePNRCP", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _51BookAccount._NotifyUrl = areaParameterList.Where(p => string.Equals(p.Name, "_NotifyUrl", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _51BookAccount._b2cCreatorCn = areaParameterList.Where(p => string.Equals(p.Name, "_b2cCreatorCn", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _51BookAccount._statusType = areaParameterList.Where(p => string.Equals(p.Name, "_statusType", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            return _51BookAccount;
        }


        public string BounceOrAnnulTicket(RefundArgs refundArgs)
        {
            throw new NotImplementedException();
        }


        RefundTicketResult IPlatform.BounceOrAnnulTicket(RefundArgs refundArgs)
        {
            throw new NotImplementedException();
        }
    }
    public class _51BookAccountInfo
    {
        public string _51bookAccout { get; set; }
        public string _51bookPassword { get; set; }
        public string _51bookAg { get; set; }
        public string _PayWay { get; set; }
        public string _IsChangePNRCP { get; set; }
        public string _NotifyUrl { get; set; }
        public string _b2cCreatorCn { get; set; }
        public string _statusType { get; set; }
    }
}
