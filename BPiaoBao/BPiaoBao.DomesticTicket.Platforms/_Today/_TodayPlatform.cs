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
using BPiaoBao.DomesticTicket.Domain.Services.TodayObject;
namespace BPiaoBao.DomesticTicket.Platforms._Today
{
    [Platform("Today")]
    public class _TodayPlatform : BasePlatform, IPlatform
    {
        public override string Code
        {
            get { return "Today"; }
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
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == EnumPlatform._Today.ToString().Replace("_", "")).FirstOrDefault();
            List<PlatformPolicy> PolicyList = new List<PlatformPolicy>();
            if (pnrModel == null || pnrModel._LegList.Count == 0 || !platSystem.State)
            {
                return PolicyList;
            }
            string area = pnrModel._LegList[0].FromCode;
            _TodayAccountInfo accountInfo = GetInfo(platSystem, area);
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            string CarryCode = pnrModel._CarryCode;

            DataSet dsPolicy = new DataSet();
            PTMange ptMange = new PTMange();

            Logger.WriteLog(LogType.DEBUG, "今日获取政策开始时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");
            //PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            //System.Data.DataSet dsPolicy = m_PTService.PT_JinRiGetPolicy(_todayAccout, _todayAccout2, _IsLowerPrice, pnrContent);
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //System.Data.DataSet dsPolicy = m_PTService.PT_NewJinRiGetPolicy(_todayAccout, _todayAccout2, _IsLowerPrice, pnrContent, PTPnrData);
            dsPolicy = ptMange._JinRiGetPolicy(accountInfo._todayAccout, accountInfo._todayAccout2, _IsLowerPrice, pnrContent, pnrData);
            Logger.WriteLog(LogType.DEBUG, "今日获取政策结束时间：" + System.DateTime.Now.ToString("yyy-MM-dd HH:mm:ss.fff") + "\r\n");

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
                            policy.Id = dr["PolicyId"] != DBNull.Value ? dr["PolicyId"].ToString() : "";
                            policy.PlatformCode = this.Code;
                            policy.AreaCity = area;
                            if (!string.IsNullOrEmpty(policy.Id))
                            {
                                policy.IsChangePNRCP = false;
                                policy.IsSp = dr["RateType"].ToString() == "1" ? true : false;
                                policy.PolicyType = string.Compare(dr["RateType"].ToString().Trim(), "B2P", true) == 0 ? "1" : "2";
                                policy.PolicyType = policy.PolicyType == "1" ? "BSP" : "B2B";
                                policy.CarryCode = CarryCode;
                                StartTime = dr["WorkTimeBegin"].ToString();
                                EndTime = dr["WorkTimeEnd"].ToString();
                                policy.WorkTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                StartTime = dr["RefundTimeBegin"].ToString();
                                EndTime = dr["RefundTimeEnd"].ToString();
                                policy.ReturnTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                policy.AnnulTicketTime = new StartAndEndTime() { StartTime = StartTime, EndTime = EndTime };
                                decimal.TryParse(dr["Discounts"].ToString(), out PolicyPoint);
                                policy.PolicyPoint = PolicyPoint;
                                policy.ReturnMoney = 0m; ;
                                policy.CPOffice = dr["OfficeNum"].ToString();
                                policy.Remark = dr["Remark"].ToString();
                                policy.IssueSpeed = platSystem != null ? platSystem.IssueTicketSpeed : "";
                                policy.TodayGYCode = dr["RateId"].ToString();
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
            _TodayAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string _IsLowerPrice = IsLowPrice ? "1" : "0";
            pnrContent = System.Web.HttpUtility.UrlEncode(pnrContent);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrder = m_PTService.PT_TodayCreateOrder(accountInfo._todayAccout, accountInfo._todayAccout2, RateId, _IsLowerPrice, policyPoint.ToString(), policyId, pnrContent);
            //BPiaoBao.DomesticTicket.Platforms._PTService.PnrData PTPnrData = Mapper.DynamicMap<BPiaoBao.Common.PnrData, BPiaoBao.DomesticTicket.Platforms._PTService.PnrData>(pnrData);
            //DataSet dsOrder = m_PTService.PT_NewTodayCreateOrder(_todayAccout, _todayAccout2, _JinriGYCode, _IsLowerPrice, policyPoint.ToString(), policyId, pnrContent, PTPnrData);
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
            _TodayAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            if (accountInfo._PayWay == "0")
            {
                order.PaidMethod = EnumPaidMethod.支付宝;
            }
            else if (accountInfo._PayWay == "1")
            {
                order.PaidMethod = EnumPaidMethod.快钱;
            }
            else if (accountInfo._PayWay == "2")
            {
                order.PaidMethod = EnumPaidMethod.财付通;
            }
            string OutOrderId = order.OutOrderId;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayOrder = m_PTService.PT_JinRiOrderPay(accountInfo._todayAccout, accountInfo._todayAccout2, OutOrderId);
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
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _TodayAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string FailMessage = string.Empty;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsCancelOrder = m_PTService.PT_TodayCancelOrder(accountInfo._todayAccout2, outOrderId, pnr, accountInfo._PayWay, CancelRemark);
            if (dsCancelOrder != null && dsCancelOrder.Tables.Contains("Result")
                && dsCancelOrder.Tables["Result"].Rows.Count > 0
                && dsCancelOrder.Tables["Result"].Rows[0]["Result"].ToString() == "T"
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
            _TodayAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            string FailMessage = string.Empty;
            string result = string.Empty;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_TodayGetOrderInfo(accountInfo._todayAccout2, outOrderId);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Contains("Response")
                && dsOrderInfo.Tables["Response"].Rows.Count > 0
                && dsOrderInfo.Tables["Response"].Columns.Contains("PName")
                && dsOrderInfo.Tables["Response"].Columns.Contains("TicketNo")
                )
            {
                string[] passengers = dsOrderInfo.Tables["Response"].Rows[0]["PName"].ToString().Split('|');
                string[] ticketNos = dsOrderInfo.Tables["Response"].Rows[0]["TicketNo"].ToString().Split('|');
                string ticketNo = "";
                if (passengers.Length == ticketNos.Length)
                {
                    for (int i = 0; i < passengers.Length; i++)
                    {
                        if (passengers[i].Trim() != "" && ticketNos[i].Trim() != "")
                        {
                            ticketNo = ticketNos[i].ToString().Replace("--", "-");
                            if (ticketNo.Contains("-") && ticketNo.Split('-').Length == 2 && ticketNo.Split('-')[1].Trim() == "")
                            {
                                continue;
                            }
                            if (!resultDic.ContainsKey(passengers[i].Trim().ToUpper()))
                                resultDic.Add(passengers[i].Trim().ToUpper(), ticketNo);
                        }
                    }
                }
            }
            return resultDic;
        }

        public string GetOrderStatus(string areaCity, string orderId, string outOrderId, string pnr)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _TodayAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            string FailMessage = string.Empty;
            string result = string.Empty;
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsOrderInfo = m_PTService.PT_TodayGetOrderInfo(accountInfo._todayAccout2, outOrderId);
            if (dsOrderInfo != null && dsOrderInfo.Tables.Contains("Response")
                && dsOrderInfo.Tables["Response"].Rows.Count > 0
                && dsOrderInfo.Tables["Response"].Columns.Contains("Status")
                )
            {
                //成功
                result = getOrderStatus(dsOrderInfo.Tables["Response"].Rows[0]["Status"].ToString());
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
            _TodayAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            PTServiceSoapClient m_PTService = new PTServiceSoapClient();
            DataSet dsPayStatus = m_PTService.PT_TodayGetPayStatus(accountInfo._todayAccout2, outOrderId);
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


        /// <summary>
        /// 今日退废票申请
        /// </summary>
        /// <param name="refundArgs"></param>
        /// <returns></returns>
        public RefundTicketResult BounceOrAnnulTicket(RefundArgs refundArgs)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _TodayAccountInfo accountInfo = GetInfo(platSystem, refundArgs.areaCity);
            //设置请求参数
            TodayTuiFeiOrderRequest request = new TodayTuiFeiOrderRequest();
            request._Type = refundArgs.RefundType == 0 ? "B" : "A";
            request._OrderNo = refundArgs.OutOrderId;
            List<int> _RepealList = new List<int>();
            List<string> _PNameList = new List<string>();
            List<string> _TicketNoList = new List<string>();
            refundArgs.Passengers.ForEach(p =>
            {
                //默认
                p.Repeal = refundArgs.RefundType == 0 ? 2 : 1;
                _RepealList.Add(p.Repeal);
                _PNameList.Add(p.PassengerName);
                _TicketNoList.Add(p.TicketNo);
            });
            request._Repeal = string.Join("|", _RepealList.ToArray());
            request._PersonName = string.Join("|", _PNameList.ToArray());
            request._TicketNo = string.Join("|", _TicketNoList.ToArray());
            //是否取消座位
            request._IsCancelSeat = "是";// refundArgs.IsCancelSeat ? "是" : "否";
            string[] strArray = refundArgs.Guid.Split('|');
            request._Cause = strArray.Length > 1 ? strArray[1] : "B";//理由 待定
            request._Remarks = refundArgs.Remark;
            request._Rnum = refundArgs.Passengers.Count();
            request._Ramount = refundArgs.Passengers.Sum(p => p.Amount);
            //调用
            PTMange ptMange = new PTMange();
            string result = ptMange.Today_TuiFeiOrder(refundArgs.OrderId, accountInfo._todayAccout2, request);
            RefundTicketResult refundTicketResult = new RefundTicketResult();
            refundTicketResult.Descript = result;
            if (result.ToUpper().Contains("<RESULT>T</RESULT>"))
            {
                refundTicketResult.Result = true;
            }
            return refundTicketResult;
        }

        /// <summary>
        /// 获取退废票详情
        /// </summary>
        /// <param name="areaCity"></param>
        /// <param name="outOrderId">今日订单号</param>
        /// <returns></returns>
        public DataSet Today_GetTuiFeiOrderInfo(string areaCity, string outOrderId)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _TodayAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            //调用
            PTMange ptMange = new PTMange();
            DataSet result = ptMange.Today_GetTuiFeiOrderInfo(accountInfo._todayAccout2, outOrderId);
            return result;
        }

        /// <summary>
        /// 今日申请机票升舱
        /// </summary>
        /// <returns></returns>
        public void Today_RiseCabin(string areaCity, string orderId, TodayRiseCabinRequest request)
        {
            PlatSystem platSystem = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode == this.Code).FirstOrDefault();
            _TodayAccountInfo accountInfo = GetInfo(platSystem, areaCity);
            //调用
            PTMange ptMange = new PTMange();
            string result = ptMange.Today_RiseCabin(orderId, accountInfo._todayAccout2, request);
            if (!result.ToUpper().Contains("<RESULT>T</RESULT>"))
            {
                throw new PayInterfaceOrderException(result);
            }
        }


        //转换
        private string getOrderStatus(string code)
        {
            NameValueCollection nc = new NameValueCollection();
            nc.Add("-1", "等待确认");
            nc.Add("0", "等待支付");
            nc.Add("1", "支付成功");
            nc.Add("2", "出票成功");
            nc.Add("3", "申请废票");
            nc.Add("4", "申请退票");
            nc.Add("5", "退款中");
            nc.Add("6", "取消订单");
            nc.Add("7", "暂不能出票");
            nc.Add("8", "暂不能废");
            nc.Add("9", "暂不能退");
            nc.Add("10", "航班延误");
            nc.Add("14", "退款成功");
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

        private _TodayAccountInfo GetInfo(PlatSystem platSystem, string areaCity)
        {
            _TodayAccountInfo _TodayAccount = new _TodayAccountInfo();
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
            _TodayAccount._todayAccout = areaParameterList.Where(p => string.Equals(p.Name, "_todayAccout", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _TodayAccount._todayAccout2 = areaParameterList.Where(p => string.Equals(p.Name, "_todayAccout2", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _TodayAccount._PayWay = areaParameterList.Where(p => string.Equals(p.Name, "_PayWay", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            //_TodayAccount._JinriGYCode = areaParameterList.Where(p => string.Equals(p.Name, "_JinriGYCode", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            _TodayAccount._privateKey = areaParameterList.Where(p => string.Equals(p.Name, "_privateKey", StringComparison.OrdinalIgnoreCase)).FirstOrDefault().Value;
            return _TodayAccount;
        }





    }
    public class _TodayAccountInfo
    {
        public string _todayAccout { get; set; }
        public string _todayAccout2 { get; set; }
        public string _PayWay { get; set; }
        // public string _JinriGYCode { get; set; }
        public string _privateKey { get; set; }
    }
}
