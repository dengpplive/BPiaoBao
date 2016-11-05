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
namespace BPiaoBao.DomesticTicket.Platforms._8000YI
{
    [Platform("8000YI")]
    public class _8000YIPlatform : BasePlatform, IPlatform
    {
        public override string Code
        {
            get { return "8000YI"; }
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
            else if (strErrMessage.Contains("服务器无法处理请求"))
            {
                strResult = "服务器无法处理请求";
            }
            else if (strErrMessage.Contains("USER_PAY_TYPE_MISMATCH"))
            {
                strResult = "代扣账号金额不足【" + strErrMessage + "】";
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
            string areaCity = pnrModel._LegList[0].FromCode;
            _8000YIAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            //是否换编码出票 1:是                
            string _IsChangePNRCP = accountInfo._IsChangePNRCP;
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            DataSet dsPolicy = new DataSet();
            PTMange ptMange = new PTMange();
            Logger.WriteLog(LogType.DEBUG, "8000YI开始时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
            //PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //System.Data.DataSet dsPolicy = m_PTService.PT_8000YIGetPolicy(_8000yiAccout, _8000yiPassword, _IsLowerPrice, pnrContent);
            //System.Data.DataSet dsPolicy = m_PTService.PT_New8000YIGetPolicy(_8000yiAccout, _8000yiPassword, _IsLowerPrice, pnrContent, PTPnrData);
            dsPolicy = ptMange._8000YIGetPolicy(accountInfo._8000yiAccout, accountInfo._8000yiPassword, _IsLowerPrice, pnrContent, pnrData);
            Logger.WriteLog(LogType.DEBUG, "8000YI结束时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
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
                            policy.Id = dr["A1"] != DBNull.Value ? dr["A1"].ToString() : "";
                            policy.PlatformCode = this.Code;
                            policy.AreaCity = areaCity;
                            if (!string.IsNullOrEmpty(policy.Id))
                            {
                                policy.IsChangePNRCP = dr["A17"].ToString().Contains("换编码出票") ? true : false;
                                policy.IsSp = dr["A22"].ToString() == "1" ? true : false;
                                policy.PolicyType = string.Compare(dr["A16"].ToString().Trim(), "BSP", true) == 0 ? "1" : "2";
                                policy.PolicyType = policy.PolicyType == "1" ? "BSP" : "B2B";
                                policy.CarryCode = dr["A4"].ToString();

                                if (dr["A12"].ToString().Split('|').Length == 2)
                                {
                                    StartTime = dr["A12"].ToString().Split('|')[0];
                                    EndTime = dr["A12"].ToString().Split('|')[1];
                                }
                                policy.WorkTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                if (dr["A19"].ToString().Split('|').Length == 2)
                                {
                                    StartTime = dr["A19"].ToString().Split('|')[0];
                                    EndTime = dr["A19"].ToString().Split('|')[1];
                                }
                                policy.ReturnTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                if (dr["A20"].ToString().Split('|').Length == 2)
                                {
                                    StartTime = dr["A20"].ToString().Split('|')[0];
                                    EndTime = dr["A20"].ToString().Split('|')[1];
                                }
                                policy.AnnulTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                policy.ReturnMoney = 0m;
                                policy.CPOffice = dr["A26"].ToString();
                                policy.Remark = dr["A17"].ToString();
                                decimal.TryParse(dr["A8"].ToString(), out PolicyPoint);
                                policy.PolicyPoint = PolicyPoint;
                                policy.IssueSpeed = platSystem != null ? platSystem.IssueTicketSpeed : "";
                                if (_IsChangePNRCP != "1" && policy.IsChangePNRCP)
                                {
                                    continue;
                                }
                                //dr["A30"].ToString() == "1" ? true : false;
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
            _8000YIAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            //内容编码
            pnrContent = System.Web.HttpUtility.UrlEncode(pnrContent);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrder = m_PTService.PT_8000YICreateOrder(accountInfo._8000yiAccout, accountInfo._8000yiPassword, policyId, _IsLowerPrice, pnrContent);
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //DataSet dsOrder = m_PTService.PT_New8000YICreateOrder(_8000yiAccout, _8000yiPassword, policyId, _IsLowerPrice, pnrContent, PTPnrData);
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
            _8000YIAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            if (accountInfo._PayWay == "0")
            {
                order.PaidMethod = EnumPaidMethod.支付宝;
            }
            string OutOrderId = order.OutOrderId;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayOrder = m_PTService.PT_8000YIOrderPay(accountInfo._8000yiAccout, accountInfo._8000yiPassword, OutOrderId, accountInfo._AlipayAccount8000yi);
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
            _8000YIAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            string result = m_PTService.PT_8000YICancelOrderNew(accountInfo._8000yiAccout, accountInfo._8000yiPassword, outOrderId);
            if (result.Trim().ToLower().StartsWith("true"))
            {
                //成功
            }
            else
            {
                //失败
                FailMessage = result;
            }
            if (!string.IsNullOrEmpty(FailMessage))
            {
                throw new OrderCommException(ErrToMessage(FailMessage));
            }
        }

        public Dictionary<string, string> AutoCompositeTicket(string areaCity, string orderId, string outOrderId, string pnr)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _8000YIAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_8000YIReturnTicekNo(accountInfo._8000yiAccout, accountInfo._8000yiPassword, outOrderId);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0)
            {
                foreach (DataTable table in dsOrderInfo.Tables)
                {
                    if (table.Rows.Count > 0
                        && table.Columns.Contains("PsgerName")
                         && table.Columns.Contains("PsgerTicketNo")
                        )
                    {
                        string ticketNo = "";
                        foreach (DataRow dr in table.Rows)
                        {
                            ticketNo = dr["PsgerTicketNo"].ToString().Trim().Replace("--", "-");
                            if (ticketNo != "")
                            {
                                if (ticketNo.Contains("-") && ticketNo.Split('-').Length == 2 && ticketNo.Split('-')[1].Trim() == "")
                                {
                                    continue;
                                }
                                if (!resultDic.ContainsKey(dr["PsgerName"].ToString().ToUpper()))
                                    resultDic.Add(dr["PsgerName"].ToString().ToUpper(), ticketNo);
                            }
                        }
                    }
                }
            }
            return resultDic;
        }
        public string GetOrderStatus(string areaCity, string orderId, string outOrderId, string pnr)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _8000YIAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string FailMessage = string.Empty;
            string result = string.Empty;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_8000YIGetOrderStatusInfo(accountInfo._8000yiAccout, accountInfo._8000yiPassword, outOrderId);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Count > 0
              && dsOrderInfo.Tables[0].Rows.Count > 0)
            {
                //成功
                result = dsOrderInfo.Tables[0].Rows[0]["a9"].ToString();
            }
            else
            {
                //失败
                FailMessage = "获取订单状态失败";
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
            _8000YIAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayStatus = m_PTService.PT_8000YIGetPayStatus(accountInfo._8000yiAccout, accountInfo._8000yiPassword, outOrderId);
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
        private _8000YIAccountInfo GetInfo(PlatSystem platSystem, string areaCity)
        {
            _8000YIAccountInfo _8000YIAccount = new _8000YIAccountInfo();
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
            _8000YIAccount._8000yiAccout = areaParameterList.Where(p => string.Equals(p.Name, "_8000yiAccout", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _8000YIAccount._8000yiPassword = areaParameterList.Where(p => string.Equals(p.Name, "_8000yiPassword", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _8000YIAccount._AlipayAccount8000yi = areaParameterList.Where(p => string.Equals(p.Name, "_AlipayAccount8000yi", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _8000YIAccount._IsChangePNRCP = areaParameterList.Where(p => string.Equals(p.Name, "_IsChangePNRCP", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _8000YIAccount._PayWay = areaParameterList.Where(p => string.Equals(p.Name, "_PayWay", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            return _8000YIAccount;
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
    public class _8000YIAccountInfo
    {
        public string _8000yiAccout { get; set; }
        public string _8000yiPassword { get; set; }
        public string _AlipayAccount8000yi { get; set; }
        public string _IsChangePNRCP { get; set; }
        public string _PayWay { get; set; }
    }
}
