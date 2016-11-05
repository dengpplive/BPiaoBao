using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using BPiaoBao.DomesticTicket.Domain;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.DomesticTicket.Platforms._PTService;
using BPiaoBao.Common;
using JoveZhao.Framework;
using PnrAnalysis;
using AutoMapper;
using BPiaoBao.DomesticTicket.Platforms.PTInterface;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Platforms._YeeXing
{
    [Platform("YeeXing")]
    public class _YeeXingPlatform : BasePlatform, IPlatform
    {
        public override string Code
        {
            get { return "YeeXing"; }
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
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == EnumPlatform._YeeXing.ToString().Replace("_", "")).FirstOrDefault();
            List<PlatformPolicy> PolicyList = new List<PlatformPolicy>();
            if (pnrModel == null || pnrModel._LegList.Count == 0 || !platSystem.State)
            {
                return PolicyList;
            }
            string area = PnrHelper.GetArea(pnrContent);
            _YeeXingAccountInfo accountInfo = GetInfo(platSystem, area);
            //是否换编码出票 1:是                
            string _IsChangePNRCP = accountInfo._IsChangePNRCP;
            string _IsLowerPrice = IsLowPrice ? "1" : "0";

            if (pnrModel._LegList.Count > 0)
            {
                DayOfWeek dayOfWeek = System.DateTime.Now.DayOfWeek; //DateTime.Parse(pnrModel._LegList[0].FlyDate1).DayOfWeek;                
                DataSet dsPolicy = new DataSet();
                PTMange ptMange = new PTMange();

                Logger.WriteLog(LogType.DEBUG, "易行获取政策开始时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
                //PTServiceSoapClient m_PTService = new PTServiceSoapClient();
                //System.Data.DataSet dsPolicy = m_PTService.PT_YeeXingGetPolicy(_yeeXingAccout, _yeeXingAccout2, _IsLowerPrice, pnrContent);
                //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
                //System.Data.DataSet dsPolicy = m_PTService.PT_NewYeeXingGetPolicy(_yeeXingAccout, _yeeXingAccout2, _IsLowerPrice, pnrContent, PTPnrData);
                dsPolicy = ptMange._YeeXingGetPolicy(accountInfo._yeeXingAccout, accountInfo._yeeXingAccout2, _IsLowerPrice, pnrContent, pnrData);
                Logger.WriteLog(LogType.DEBUG, "易行获取政策结束时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");

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
                            decimal PolicyPoint = 0m, ReturnMoney = 0m;
                            DataRowCollection drs = dsPolicy.Tables[0].Rows;
                            foreach (DataRow dr in drs)
                            {
                                PlatformPolicy policy = new PlatformPolicy();
                                //outTime  出票速度
                                StartTime = "00:00";
                                EndTime = "00:00";
                                policy.Id = dr["plcid"] != DBNull.Value ? dr["plcid"].ToString() : "";
                                policy.PlatformCode = this.Code;
                                policy.AreaCity = area;
                                if (!string.IsNullOrEmpty(policy.Id))
                                {
                                    policy.IsChangePNRCP = dr["changePnr"].ToString().Trim() == "1" ? true : false;
                                    policy.IsSp = dr["isSphigh"].ToString().Trim() == "1" ? true : false;
                                    policy.PolicyType = dr["tickType"].ToString().Trim() == "1" ? "2" : "1";
                                    policy.PolicyType = policy.PolicyType == "1" ? "BSP" : "B2B";
                                    policy.CarryCode = dr["airComp"].ToString().Trim();
                                    if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
                                    {
                                        //周一到周五
                                        if (dr["workTime"].ToString().Split('-').Length == 2)
                                        {
                                            StartTime = dr["workTime"].ToString().Split('-')[0];
                                            EndTime = dr["workTime"].ToString().Split('-')[1];
                                        }
                                        policy.WorkTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                        if (dr["workReturnTime"].ToString().Split('-').Length == 2)
                                        {
                                            StartTime = dr["workReturnTime"].ToString().Split('-')[0];
                                            EndTime = dr["workReturnTime"].ToString().Split('-')[1];
                                        }
                                        policy.ReturnTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                        policy.AnnulTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                    }
                                    else
                                    {
                                        //周末
                                        if (dr["restWorkTime"].ToString().Split('-').Length == 2)
                                        {
                                            StartTime = dr["restWorkTime"].ToString().Split('-')[0];
                                            EndTime = dr["restWorkTime"].ToString().Split('-')[1];
                                        }
                                        policy.WorkTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                        if (dr["restReturnTime"].ToString().Split('-').Length == 2)
                                        {
                                            StartTime = dr["restReturnTime"].ToString().Split('-')[0];
                                            EndTime = dr["restReturnTime"].ToString().Split('-')[1];
                                        }
                                        policy.ReturnTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                        policy.AnnulTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                    }
                                    decimal.TryParse(dr["extReward"].ToString(), out ReturnMoney);
                                    policy.ReturnMoney = ReturnMoney;
                                    policy.CPOffice = "";//易行适用航空公司大配置自动授权
                                    policy.Remark = dr["memo"].ToString();
                                    decimal.TryParse(dr["disc"].ToString(), out PolicyPoint);
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
                else
                {
                    throw new PnrAnalysisFailException(string.Format("PNR内容解析航段失败:{0}", pnrContent));
                }
            }
            return PolicyList;
        }

        public PlatformOrder CreateOrder(string pnrContent, bool IsLowPrice, string areaCity, string policyId, string RateId, string localOrderId, decimal policyPoint, decimal ReturnMoney, BPiaoBao.Common.PnrData pnrData)
        {
            PlatformOrder platformOrder = null;
            string FailMessage = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _YeeXingAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            pnrContent = System.Web.HttpUtility.UrlEncode(pnrContent);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrder = m_PTService.PT_YeeXingCreateOrder(accountInfo._yeeXingAccout, accountInfo._yeeXingAccout2, _IsLowerPrice, policyId, localOrderId, policyPoint.ToString(), ReturnMoney.ToString(), pnrContent);
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //DataSet dsOrder = m_PTService.PT_NewYeeXingCreateOrder(_yeeXingAccout, _yeeXingAccout2, _IsLowerPrice, policyId, localOrderId, policyPoint.ToString(), ReturnMoney.ToString(), pnrContent, PTPnrData);
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
            _YeeXingAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            if (accountInfo._PayWay == "1")
            {
                order.PaidMethod = EnumPaidMethod.支付宝;
            }
            else if (accountInfo._PayWay == "2")
            {
                order.PaidMethod = EnumPaidMethod.快钱;
            }
            string OutOrderId = order.OutOrderId;
            string OutOrderPayMoney = order.TotlePaidPirce.ToString();
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayOrder = m_PTService.PT_YeeXingOrderPay(accountInfo._yeeXingAccout, accountInfo._yeeXingAccout2, OutOrderId, OutOrderPayMoney, accountInfo._PayWay, accountInfo._yeeXingNotifyUrl);
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
            get { return true; }
        }

        public void CancelOrder(string areaCity, string outOrderId, string pnr, string CancelRemark, string passengerName)
        {
            string FailMessage = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _YeeXingAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsCancelOrder = m_PTService.PT_YeeXingCancelOrder(accountInfo._yeeXingAccout2, outOrderId, passengerName, accountInfo._cancel_notify_url);
            if (dsCancelOrder != null && dsCancelOrder.Tables.Contains("result")
               && dsCancelOrder.Tables["result"].Rows.Count > 0
               && dsCancelOrder.Tables["result"].Columns.Contains("is_success")
                && dsCancelOrder.Tables["result"].Rows[0]["is_success"].ToString().ToUpper() == "T"
               )
            {
                //成功             
            }
            else
            {
                FailMessage = "取消订单失败！";
            }
            if (!string.IsNullOrEmpty(FailMessage))
            {
                throw new PayInterfaceOrderException(ErrToMessage(FailMessage));
            }
        }

        public Dictionary<string, string> AutoCompositeTicket(string areaCity, string orderId, string outOrderId, string pnr)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _YeeXingAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_YeeXingOrderQueryInfo(accountInfo._yeeXingAccout, accountInfo._yeeXingAccout2, outOrderId, orderId);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Contains("result")
               && dsOrderInfo.Tables["result"].Rows.Count > 0
               && dsOrderInfo.Tables["result"].Columns.Contains("ordstate")
               )
            {
                //暂时未完成
            }
            return resultDic;
        }


        public string GetOrderStatus(string areaCity, string orderId, string outOrderId, string pnr)
        {
            string result = string.Empty;
            string FailMessage = string.Empty;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _YeeXingAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_YeeXingQueryOrderStatus(accountInfo._yeeXingAccout2, orderId, outOrderId);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Contains("result")
               && dsOrderInfo.Tables["result"].Rows.Count > 0
               && dsOrderInfo.Tables["result"].Columns.Contains("ordstate")
               )
            {
                if (dsOrderInfo.Tables["result"].Rows[0]["is_success"].ToString().ToUpper() == "T")
                {
                    //成功
                    result = dsOrderInfo.Tables["result"].Rows[0]["ordstate"].ToString();
                }
                else
                {
                    result = "获取订单状态失败：" + dsOrderInfo.Tables["result"].Rows[0]["error"].ToString();
                }
            }
            else
            {
                FailMessage = "获取订单状态失败！";
            }
            if (!string.IsNullOrEmpty(FailMessage))
            {
                throw new PayInterfaceOrderException(ErrToMessage(FailMessage));
            }
            return result;
        }


        public bool IsPaid(string areaCity, string orderId, string outOrderId, string pnr)
        {
            bool isPaid = false;
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _YeeXingAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayStatus = m_PTService.PT_YeeXingOrderQueryInfo(accountInfo._yeeXingAccout, accountInfo._yeeXingAccout2, outOrderId, orderId);
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
                throw new OrderCommException("为获取到" + this.Code + "订单代付状态");
            }
            return isPaid;
        }
        private _YeeXingAccountInfo GetInfo(PlatSystem platSystem, string areaCity)
        {
            _YeeXingAccountInfo _YeeXingAccount = new _YeeXingAccountInfo();
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
            _YeeXingAccount._yeeXingAccout = areaParameterList.Where(p => string.Equals(p.Name, "_yeeXingAccout", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _YeeXingAccount._yeeXingAccout2 = areaParameterList.Where(p => string.Equals(p.Name, "_yeeXingAccout2", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _YeeXingAccount._yeeXingNotifyUrl = areaParameterList.Where(p => string.Equals(p.Name, "_yeeXingNotifyUrl", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _YeeXingAccount._IsChangePNRCP = areaParameterList.Where(p => string.Equals(p.Name, "_IsChangePNRCP", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _YeeXingAccount._PayWay = areaParameterList.Where(p => string.Equals(p.Name, "_PayWay", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _YeeXingAccount._privateKey = areaParameterList.Where(p => string.Equals(p.Name, "_privateKey", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _YeeXingAccount._cancel_notify_url = areaParameterList.Where(p => string.Equals(p.Name, "_cancel_notify_url", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            return _YeeXingAccount;
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
    public class _YeeXingAccountInfo
    {
        public string _yeeXingAccout { get; set; }
        public string _yeeXingAccout2 { get; set; }
        public string _yeeXingNotifyUrl { get; set; }
        public string _IsChangePNRCP { get; set; }
        public string _PayWay { get; set; }
        public string _privateKey { get; set; }
        public string _cancel_notify_url { get; set; }
    }
}
