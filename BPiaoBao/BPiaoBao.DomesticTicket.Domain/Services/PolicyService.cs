using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using BPiaoBao.DomesticTicket.Domain.Services.B2BParam;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework.Expand;
using PnrAnalysis;
using PnrAnalysis.Model;
using StructureMap;
using System.Diagnostics;
using BPiaoBao.Common;
using System.Threading.Tasks;
namespace BPiaoBao.DomesticTicket.Domain.Services
{
    public class PolicyService
    {
        ILocalPolicyRepository localPolicyRepository;
        IBusinessmanRepository businessmanRepository;
        FormatPNR format = new FormatPNR();
        DataBill dataBill = new DataBill();
        public PolicyService(ILocalPolicyRepository localPolicyRepository, IBusinessmanRepository businessmanRepository)
        {
            this.localPolicyRepository = localPolicyRepository;
            this.businessmanRepository = businessmanRepository;
        }
        /// <summary>
        /// 获取接口政策
        /// </summary>      
        /// <returns></returns>
        public List<Policy> GetInterfacePolicy(PolicyParam policyParam, string platformCode, UserRelation userRealtion, PnrData pnrData)
        {
            List<Policy> listPolicy = new List<Policy>();
            //记录时间
            StringBuilder sblog = new StringBuilder();
            Stopwatch watch = new Stopwatch();
            try
            {
                //关闭接口政策
                if (!userRealtion.carrier.InterfacePolicySwitch)
                {
                    return listPolicy;
                }
                //含有婴儿排除接口政策
                if (pnrData.PnrMode != null && pnrData.PnrMode.HasINF)
                {
                    return listPolicy;
                }
                sblog.AppendFormat("[GetInterfacePolicy]订单号:{0}\r\n", policyParam.OrderId);
                watch.Start();
                string pnrContext = policyParam.PnrContent;
                List<PlatformPolicy> platformPolicyList = new List<PlatformPolicy>();
                bool IsLowPrice = false;
                if (pnrData.PatMode.UninuePatList.Count > 1)
                {
                    IsLowPrice = policyParam.IsLowPrice;
                }
                if (!string.IsNullOrEmpty(platformCode))
                {
                    platformPolicyList = PlatformFactory.GetPlatformByCode(platformCode).GetPoliciesByPnrContent(pnrContext, IsLowPrice, pnrData);
                }
                else
                    platformPolicyList = PlatformFactory.GetPoliciesByPnrContent(pnrContext, IsLowPrice, pnrData);
                watch.Stop();
                sblog.AppendFormat("编码内容获取政策用时:{0}\r\n", watch.Elapsed.ToString());
                watch.Restart();
                //循环接口政策
                platformPolicyList.ForEach(p =>
                {
                    if (userRealtion.carrier != null)
                    {
                        #region 过滤工作时间
                        StartAndEndTime WorkTime = new StartAndEndTime()
                        {
                            StartTime = p.WorkTime.StartTime,
                            EndTime = p.WorkTime.EndTime
                        };
                        StartAndEndTime ReturnTicketTime = new StartAndEndTime()
                        {
                            StartTime = p.ReturnTicketTime.StartTime,
                            EndTime = p.ReturnTicketTime.EndTime
                        };
                        StartAndEndTime AnnulTicketTime = new StartAndEndTime()
                        {
                            StartTime = p.AnnulTicketTime.StartTime,
                            EndTime = p.AnnulTicketTime.EndTime
                        };
                        if (WorkTime != null && WorkTime.ToString() != "-")
                        {
                            string CurrTime = System.DateTime.Now.ToString("HH:mm");
                            int start = -1;
                            int end = -1;
                            start = FormatPNR.CompareTime(CurrTime, WorkTime.StartTime);
                            end = FormatPNR.CompareTime(WorkTime.EndTime, CurrTime);
                            if (!(start > 0 && end > 0))
                            {
                                return;
                            }
                        }

                        #endregion
                        Policy policy = new Policy()
                        {
                            PolicyId = p.Id,
                            CarryCode = p.CarryCode,
                            PlatformCode = p.PlatformCode,
                            OriginalPolicyPoint = p.PolicyPoint,
                            DownPoint = 0m,
                            PaidPoint = p.PolicyPoint,
                            PolicyPoint = p.PolicyPoint,
                            ReturnMoney = p.ReturnMoney,
                            Remark = p.Remark,
                            IsChangePNRCP = p.IsChangePNRCP,
                            IsSp = p.IsSp,
                            PolicyType = p.PolicyType,
                            WorkTime = WorkTime,
                            ReturnTicketTime = ReturnTicketTime,
                            AnnulTicketTime = AnnulTicketTime,
                            CPOffice = p.CPOffice,
                            AreaCity = p.AreaCity,
                            IssueSpeed = p.IssueSpeed,
                            CarrierCode = userRealtion.carrier.Code,//重要
                            Code = userRealtion.carrier.Code,
                            Name = userRealtion.carrier.Name,
                            CashbagCode = userRealtion.carrier.CashbagCode,
                            Rate = userRealtion.carrier.Rate,
                            PolicySourceType = EnumPolicySourceType.Interface,
                            EnumIssueTicketWay = EnumIssueTicketWay.Manual,
                            IsLow = p.IsLow,
                            ABFee = p.ABFee,
                            SeatPrice = p.SeatPrice,
                            RQFee = p.RQFee,
                            Commission = dataBill.GetCommission(p.PolicyPoint, p.SeatPrice, 0m),
                            PolicyOwnUserRole = EnumPolicyOwnUserRole.Carrier,
                            TodayGYCode = p.TodayGYCode
                        };

                        //换编码过滤
                        if (!policyParam.IsChangePnrTicket)
                        {
                            if (!p.IsChangePNRCP)
                            {
                                //添加结果集
                                listPolicy.Add(policy);
                            }
                        }
                        else
                        {
                            listPolicy.Add(policy);
                        }
                    }
                });//EndFor
                watch.Stop();
                sblog.AppendFormat("循环政策用时:{0}\r\n", watch.Elapsed.ToString());
            }
            catch (Exception ex)
            {
                new CommLog().WriteLog("GetInterfacePolicy", ex.Message + ex.StackTrace);
            }
            finally
            {
                new CommLog().WriteLog("GetPolicy", sblog.ToString());
            }
            return listPolicy;
        }
        /// <summary>
        /// 获取本地政策
        /// </summary>       
        /// <returns></returns>
        public List<Policy> GetLocalPolicy(UserRelation userRealtion, string platformCode, PolicyParam policyParam, PnrData pnrData)
        {
            //记录时间
            StringBuilder sblog = new StringBuilder();
            Stopwatch watch = new Stopwatch();
            List<Policy> listPolicy = new List<Policy>();
            try
            {
                watch.Start();
                if (userRealtion.carrier == null) return listPolicy;
                FilterPolicy(userRealtion, platformCode, EnumPolicySourceType.Local, policyParam, listPolicy, pnrData);
                watch.Stop();
                sblog.AppendFormat("[GetLocalPolicy]订单号:{0}获取政策用时:{1}\r\n", policyParam.OrderId, watch.Elapsed.ToString());
            }
            finally
            {
                new CommLog().WriteLog("GetPolicy", sblog.ToString());
            }
            return listPolicy;
        }
        /// <summary>
        /// 获取该采购的上级运营商的政策
        /// </summary>       
        /// <returns></returns>
        public List<Policy> GetLocalParentPolicy(UserRelation userRealtion, string platformCode, PolicyParam policyParam, PnrData pnrData)
        {
            //记录时间
            StringBuilder sblog = new StringBuilder();
            Stopwatch watch = new Stopwatch();
            List<Policy> listPolicy = new List<Policy>();
            try
            {
                watch.Start();
                if (userRealtion.carrier == null) return listPolicy;
                FilterPolicy(userRealtion, platformCode, EnumPolicySourceType.Local, policyParam, listPolicy, pnrData, true);
                watch.Stop();
                sblog.AppendFormat("[GetLocalParentPolicy]订单号:{0}获取政策用时:{1}\r\n", policyParam.OrderId, watch.Elapsed.ToString());
            }
            finally
            {
                new CommLog().WriteLog("GetPolicy", sblog.ToString());
            }
            return listPolicy;
        }
        /// <summary>
        /// 获取共享政策
        /// </summary>      
        /// <returns></returns>
        public List<Policy> GetSharePolicy(UserRelation userRealtion, string platformCode, PolicyParam policyParam, PnrData pnrData)
        {
            //记录时间
            StringBuilder sblog = new StringBuilder();
            Stopwatch watch = new Stopwatch();
            List<Policy> listPolicy = new List<Policy>();
            try
            {
                //含有婴儿排除共享政策
                if (pnrData.PnrMode != null && pnrData.PnrMode.HasINF)
                {
                    return listPolicy;
                }
                watch.Start();
                if (userRealtion.carrier == null) return listPolicy;
                FilterPolicy(userRealtion, platformCode, EnumPolicySourceType.Share, policyParam, listPolicy, pnrData);
                watch.Stop();
                sblog.AppendFormat("[GetSharePolicy]订单号:{0}获取政策用时:{1}\r\n", policyParam.OrderId, watch.Elapsed.ToString());
            }
            finally
            {
                new CommLog().WriteLog("GetPolicy", sblog.ToString());
            }
            return listPolicy;
        }
        /// <summary>
        /// 获取默认政策
        /// </summary>
        /// <param name="userRealtion"></param>
        /// <returns></returns>
        public List<Policy> GetDefaultPolicy(UserRelation userRealtion, string platformCode, PolicyParam policyParam, PnrData pnrData, string PolicyId = "")
        {
            List<Policy> listPolicy = new List<Policy>();
            try
            {
                decimal LocalPoint = 0m;
                Carrier carrier = userRealtion.carrier;
                DefaultPolicy defaultPolicy = null;
                string strCarrayCode = (pnrData.PnrMode != null && pnrData.PnrMode._LegList.Count > 0) ? pnrData.PnrMode._LegList[0].AirCode : "";
                if (!string.IsNullOrEmpty(strCarrayCode)
                    && carrier != null
                    && carrier.DefaultPolicys != null
                    && carrier.DefaultPolicys.Count > 0)
                {
                    carrier = userRealtion.carrier;
                    #region 本地默认政策是否开启
                    if (carrier.LocalPolicySwitch)
                    {
                        defaultPolicy = userRealtion.carrier.DefaultPolicys.Where(p => p.CarrayCode.ToUpper().Contains(strCarrayCode.ToUpper())).FirstOrDefault();
                        if (defaultPolicy == null)
                        {
                            defaultPolicy = carrier.DefaultPolicys.Where(p => p.CarrayCode.ToUpper().Contains("ALL")).FirstOrDefault();
                        }
                        //设置了默认政策就添加默认政策 否则不添加
                        if (defaultPolicy != null)
                        {
                            #region 运营上级的默认政策
                            LocalPoint = policyParam.OrderType == 0 ? defaultPolicy.DefaultPolicyPoint : defaultPolicy.ChildrenPolicyPoint;
                            if (LocalPoint > 0)
                            {
                                #region 工作时间
                                StartAndEndTime WorkTime = null;
                                StartAndEndTime ReturnTicketTime = null;
                                StartAndEndTime AnnulTicketTime = null;
                                //周末
                                if (System.DateTime.Now.DayOfWeek == DayOfWeek.Saturday
                                    || System.DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    WorkTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.RestWork.WorkOnLineTime,
                                        EndTime = carrier.RestWork.WorkUnLineTime
                                    };
                                    ReturnTicketTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.RestWork.ServiceOnLineTime,
                                        EndTime = carrier.RestWork.ServiceUnLineTime
                                    };
                                    AnnulTicketTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.RestWork.ServiceOnLineTime,
                                        EndTime = carrier.RestWork.ServiceUnLineTime
                                    };
                                }
                                else
                                {
                                    WorkTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.NormalWork.WorkOnLineTime,
                                        EndTime = carrier.NormalWork.WorkUnLineTime
                                    };
                                    ReturnTicketTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.NormalWork.ServiceOnLineTime,
                                        EndTime = carrier.NormalWork.ServiceUnLineTime
                                    };
                                    AnnulTicketTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.NormalWork.ServiceOnLineTime,
                                        EndTime = carrier.NormalWork.ServiceUnLineTime
                                    };
                                }
                                if (WorkTime != null && WorkTime.ToString() != "-")
                                {
                                    string CurrTime = System.DateTime.Now.ToString("HH:mm");
                                    int start = -1;
                                    int end = -1;
                                    start = FormatPNR.CompareTime(CurrTime, WorkTime.StartTime);
                                    end = FormatPNR.CompareTime(WorkTime.EndTime, CurrTime);
                                    if (!(start > 0 && end > 0))
                                    {
                                        return listPolicy;
                                    }
                                }
                                #endregion

                                decimal SeatPrice = 0m, TaxFare = 0m, RQFare = 0m;
                                bool IsLowPrice = false;
                                if (pnrData.PatMode.UninuePatList.Count > 1)
                                {
                                    IsLowPrice = policyParam.IsLowPrice;
                                }
                                PatInfo pat = GetPat(platformCode, policyParam, IsLowPrice, pnrData);
                                decimal.TryParse(pat.Fare, out SeatPrice);
                                decimal.TryParse(pat.TAX, out TaxFare);
                                decimal.TryParse(pat.RQFare, out RQFare);
                                listPolicy.Add(new Policy()
                                {
                                    PolicyId = string.IsNullOrEmpty(PolicyId) ? (carrier.Code + "_" + policyParam.OrderId) : PolicyId,
                                    OrderId = policyParam.OrderId,
                                    CarryCode = strCarrayCode,
                                    CarrierCode = userRealtion.carrier.Code,
                                    PlatformCode = platformCode,
                                    DownPoint = 0m,
                                    OriginalPolicyPoint = LocalPoint,
                                    PaidPoint = LocalPoint,
                                    PolicyPoint = LocalPoint,
                                    ReturnMoney = 0m,
                                    Remark = defaultPolicy.IssueTicketType,
                                    IsChangePNRCP = false,
                                    IsSp = false,
                                    PolicyType = defaultPolicy.IssueTicketType,
                                    WorkTime = WorkTime,
                                    ReturnTicketTime = ReturnTicketTime,
                                    AnnulTicketTime = AnnulTicketTime,
                                    CPOffice = defaultPolicy.Office,
                                    AreaCity = userRealtion.carrier.Code,
                                    IssueSpeed = carrier.IssueSpeed.ToString(),
                                    Code = carrier.Code,
                                    Name = carrier.Name,
                                    CashbagCode = carrier.CashbagCode,
                                    Rate = carrier.Rate,
                                    PolicySourceType = EnumPolicySourceType.Local,
                                    EnumIssueTicketWay = defaultPolicy.IssueTicketWay,
                                    IsLow = false,
                                    ABFee = TaxFare,
                                    SeatPrice = SeatPrice,
                                    RQFee = RQFare,
                                    Commission = dataBill.GetCommission(LocalPoint, SeatPrice, 0m),
                                    PolicyOwnUserRole = EnumPolicyOwnUserRole.Carrier
                                });
                            }
                            #endregion
                        }
                    }
                    #endregion
                }
                //该运营开了采购异地的政策就查询成都的ctuadmin的默认政策 属于共享
                if (carrier != null && carrier.BuyerRemotoPolicySwich && listPolicy.Count == 0)
                {
                    carrier = this.businessmanRepository.FindAll(p => p.Code == PnrHelper.DefAccount).FirstOrDefault() as Carrier;
                    //ctuadmin的对外政策是否开启                    
                    if (carrier != null && carrier.ForeignRemotePolicySwich)
                    {
                        defaultPolicy = carrier.DefaultPolicys.Where(p => p.CarrayCode.ToUpper().Contains(strCarrayCode.ToUpper())).FirstOrDefault();
                        if (defaultPolicy == null)
                        {
                            defaultPolicy = carrier.DefaultPolicys.Where(p => p.CarrayCode.ToUpper().Contains("ALL")).FirstOrDefault();
                        }
                        if (defaultPolicy != null)
                        {
                            #region ctuadmin的默认政策
                            LocalPoint = policyParam.OrderType == 0 ? defaultPolicy.DefaultPolicyPoint : defaultPolicy.ChildrenPolicyPoint;
                            if (LocalPoint > 0)
                            {
                                #region 工作时间
                                StartAndEndTime WorkTime = null;
                                StartAndEndTime ReturnTicketTime = null;
                                StartAndEndTime AnnulTicketTime = null;
                                //周末
                                if (System.DateTime.Now.DayOfWeek == DayOfWeek.Saturday
                                    || System.DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
                                {
                                    WorkTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.RestWork.WorkOnLineTime,
                                        EndTime = carrier.RestWork.WorkUnLineTime
                                    };
                                    ReturnTicketTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.RestWork.ServiceOnLineTime,
                                        EndTime = carrier.RestWork.ServiceUnLineTime
                                    };
                                    AnnulTicketTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.RestWork.ServiceOnLineTime,
                                        EndTime = carrier.RestWork.ServiceUnLineTime
                                    };
                                }
                                else
                                {
                                    WorkTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.NormalWork.WorkOnLineTime,
                                        EndTime = carrier.NormalWork.WorkUnLineTime
                                    };
                                    ReturnTicketTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.NormalWork.ServiceOnLineTime,
                                        EndTime = carrier.NormalWork.ServiceUnLineTime
                                    };
                                    AnnulTicketTime = new StartAndEndTime()
                                    {
                                        StartTime = carrier.NormalWork.ServiceOnLineTime,
                                        EndTime = carrier.NormalWork.ServiceUnLineTime
                                    };
                                }
                                if (WorkTime != null && WorkTime.ToString() != "-")
                                {
                                    string CurrTime = System.DateTime.Now.ToString("HH:mm");
                                    int start = -1;
                                    int end = -1;
                                    start = FormatPNR.CompareTime(CurrTime, WorkTime.StartTime);
                                    end = FormatPNR.CompareTime(WorkTime.EndTime, CurrTime);
                                    if (!(start > 0 && end > 0))
                                    {
                                        return listPolicy;
                                    }
                                }
                                #endregion

                                decimal SeatPrice = 0m, TaxFare = 0m, RQFare = 0m;
                                bool IsLowPrice = false;
                                if (pnrData.PatMode.UninuePatList.Count > 1)
                                {
                                    IsLowPrice = policyParam.IsLowPrice;
                                }
                                PatInfo pat = GetPat(platformCode, policyParam, IsLowPrice, pnrData);
                                decimal.TryParse(pat.Fare, out SeatPrice);
                                decimal.TryParse(pat.TAX, out TaxFare);
                                decimal.TryParse(pat.RQFare, out RQFare);
                                listPolicy.Add(new Policy()
                                {
                                    PolicyId = string.IsNullOrEmpty(PolicyId) ? (carrier.Code + "_" + policyParam.OrderId) : PolicyId,
                                    OrderId = policyParam.OrderId,
                                    CarryCode = strCarrayCode,
                                    CarrierCode = carrier.Code,
                                    PlatformCode = platformCode,
                                    DownPoint = 0m,
                                    OriginalPolicyPoint = LocalPoint,
                                    PaidPoint = LocalPoint,
                                    PolicyPoint = LocalPoint,
                                    ReturnMoney = 0m,
                                    Remark = defaultPolicy.IssueTicketType,
                                    IsChangePNRCP = false,
                                    IsSp = false,
                                    PolicyType = defaultPolicy.IssueTicketType,
                                    WorkTime = WorkTime,
                                    ReturnTicketTime = ReturnTicketTime,
                                    AnnulTicketTime = AnnulTicketTime,
                                    CPOffice = defaultPolicy.Office,
                                    AreaCity = carrier.Code,
                                    IssueSpeed = carrier.IssueSpeed.ToString(),
                                    Code = carrier.Code,
                                    Name = carrier.Name,
                                    CashbagCode = carrier.CashbagCode,
                                    Rate = carrier.RemoteRate,
                                    PolicySourceType = EnumPolicySourceType.Share,
                                    EnumIssueTicketWay = defaultPolicy.IssueTicketWay,
                                    IsLow = false,
                                    ABFee = TaxFare,
                                    SeatPrice = SeatPrice,
                                    RQFee = RQFare,
                                    Commission = dataBill.GetCommission(LocalPoint, SeatPrice, 0m),
                                    PolicyOwnUserRole = EnumPolicyOwnUserRole.Carrier
                                });
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                new CommLog().WriteLog("GetDefaultPolicy", ex.Message + ex.StackTrace);
            }
            return listPolicy;
        }


        /// <summary>
        /// 获取航空公司B2B网站政策
        /// </summary>
        /// <param name="userRealtion"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public List<Policy> GetAirB2BPolicy(UserRelation userRealtion, Order order)
        {
            //AutoEtdzParam autoEtdzParam = new AutoEtdzParam();
            //AutoTicketService autoTicketService = ObjectFactory.GetInstance<AutoTicketService>();
            //autoTicketService.NewQueryPriceByPnr(autoEtdzParam);
            return null;
        }

        public bool IsCanGetPolicy(LocalPolicy p, UserRelation userRelation, string carrierCode, int type)
        {
            bool r = false;
            if (type == 0)
            {
                r = userRelation.SupplierList.Exists(p1 => (p1.Code == p.Code && p1.CarrierCode == carrierCode) && (p1.IsEnable && p1.SupLocalPolicySwitch));
            }
            else if (type == 1)
            {
                r = userRelation.SupplierList.Exists(p1 => p1.Code == p.Code && p1.IsEnable && p1.SupRemotePolicySwitch);
            }
            else if (type == 2)
            {
                r = userRelation.CarrierList.Exists(p1 => p1.Code == p.Code && p1.IsEnable && p1.ForeignRemotePolicySwich);
            }
            return r;
        }
        /// <summary>
        /// 查航班获取政策
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public List<PolicyCache> GetFlightPolicy(string code, PolicyQueryParam policyQueryParam)
        {
            List<PolicyCache> PolicyCacheList = new List<PolicyCache>();
            try
            {
                DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
                UserRelation userRelation = domesticService.GetUserRealtion(code);
                //上级运营商code
                string carrierCode = userRelation.carrier.Code;
                //运营是否启用
                bool carrierIsEnable = userRelation.carrier.IsEnable;
                var result = this.localPolicyRepository.FindAll();
                //没有挂起的政策
                result = result.Where(p => !p.HangUp);
                //已审核的政策
                result = result.Where(p => p.Review);
                //限制的班期 星期一到星期天 1-7
                string strDayOfWeek = ((int)System.DateTime.Now.DayOfWeek).ToString();
                if (strDayOfWeek == "0") strDayOfWeek = "7";
                result = result.Where(p => string.IsNullOrEmpty(p.WeekLimit) || !p.WeekLimit.Contains(strDayOfWeek));
                //int a = result.ToList().Count();
                //行程类型
                switch (policyQueryParam.TravelType)
                {
                    case TravelType.Oneway:
                        result =
                            result.Where(p => p.TravelType == EnumTravelType.OneWay || p.TravelType == EnumTravelType.All);
                        break;
                    case TravelType.Twoway:
                        result = result.Where(p => p.TravelType == EnumTravelType.Return || p.TravelType == EnumTravelType.All);
                        break;
                    case TravelType.Connway:
                        result = result.Where(p => p.TravelType == EnumTravelType.Connecting || p.TravelType == EnumTravelType.All);
                        break;
                }
                //a = result.ToList().Count();
                foreach (QueryParam item in policyQueryParam.InputParam)
                {
                    DateTime dt = DateTime.Parse(item.FlyDate);
                    //出发城市
                    result = result.Where(p => p.FromCityCodes.ToUpper().Contains(item.FromCode.ToUpper()));
                    //到达城市
                    result = result.Where(p => p.ToCityCodes.ToUpper().Contains(item.ToCode.ToUpper()));
                    //航空公司
                    if (!string.IsNullOrEmpty(item.CarrierCode))
                    {
                        result = result.Where(p => p.CarrayCode.ToUpper() == item.CarrierCode.ToUpper());
                    }
                    //起飞日期
                    result = result.Where(p => dt >= p.PassengeDate.StartTime && dt <= p.PassengeDate.EndTime);
                    //a = result.ToList().Count();
                }
                var result1 = result.ToList();
                result1.ForEach(p =>
                {
                    //过滤不查询的航空公司
                    AirSystem airSystem = SystemConsoSwitch.AirSystems.Where(p1 => p1.AirCode.ToUpper() == p.CarrayCode.ToUpper()).FirstOrDefault();
                    if (airSystem != null)
                    {
                        //不查询的航空公司
                        //B2B政策禁用 过滤
                        //BSP政策禁用 过滤
                        if ((!airSystem.IsQuery)
                            || (p.LocalPolicyType == "B2B" && !airSystem.IsB2B)
                            || (p.LocalPolicyType == "BSP" && !airSystem.IsBSP))
                            return;
                    }
                    #region 政策开关
                    bool IsPass = false;
                    if (p.CarrierCode == carrierCode)
                    {
                        IsPass = p.RoleType == "Carrier" ? (carrierIsEnable && userRelation.carrier.LocalPolicySwitch)
                           : userRelation.SupplierList.Exists(p1 => (p1.Code == p.Code && p1.CarrierCode == carrierCode) && (p1.IsEnable && p1.SupLocalPolicySwitch));
                    }
                    else
                    {
                        IsPass = userRelation.carrier.BuyerRemotoPolicySwich &&
                       (
                          p.RoleType == "Carrier" ? IsCanGetPolicy(p, userRelation, carrierCode, 2)
                           : IsCanGetPolicy(p, userRelation, carrierCode, 1)
                       );
                    }
                    if (!IsPass) return;
                    #endregion


                    PolicyCache pc = new PolicyCache();
                    pc.PolicySourceType = p.CarrierCode == carrierCode ? EnumPolicySourceType.Local : EnumPolicySourceType.Share;
                    pc.Applay = p.Apply;
                    pc._id = Guid.NewGuid().ToString();
                    pc.PolicyId = p.ID.ToString();
                    pc.CarrierCode = p.CarrayCode;
                    if (string.IsNullOrEmpty(p.Seats))
                        p.Seats = "";
                    pc.CabinSeatCode = p.Seats.Split(new string[] { ",", "|", "/" }, StringSplitOptions.RemoveEmptyEntries);
                    pc.FromCityCode = p.FromCityCodes.Replace(",", "/");
                    pc.ToCityCode = p.ToCityCodes.Replace(",", "/");
                    pc.SuitableFlightNo = p.Apply == EnumApply.Apply
                        ? p.ApplyFlights.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        : new string[] { };
                    pc.ExceptedFlightNo = p.Apply == EnumApply.NotApply
                        ? p.ApplyFlights.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        : new string[] { };
                    List<DayOfWeek> dayOfList = new List<DayOfWeek>()
                    {
                        DayOfWeek.Sunday,
                        DayOfWeek.Monday,
                        DayOfWeek.Tuesday,
                        DayOfWeek.Wednesday,
                        DayOfWeek.Thursday,
                        DayOfWeek.Friday,
                        DayOfWeek.Saturday
                    };
                    if (!string.IsNullOrEmpty(p.WeekLimit))
                    {
                        string[] strWeekLimit = p.WeekLimit.Split(new string[] { "," },
                            StringSplitOptions.RemoveEmptyEntries);
                        for (int i = 0; i < 7; i++)
                        {
                            if (strWeekLimit.Contains(i.ToString()))
                            {
                                dayOfList.Remove((DayOfWeek)i);
                            }
                        }
                        pc.SuitableWeek = dayOfList.ToArray();
                    }
                    pc.CheckinTime = new TimePeriod()
                    {
                        FromTime = p.PassengeDate.StartTime,
                        EndTime = p.PassengeDate.EndTime
                    };
                    pc.IssueTime = new TimePeriod()
                    {
                        FromTime = p.IssueDate.StartTime,
                        EndTime = p.IssueDate.EndTime
                    };
                    pc.ServiceTime = new WorkTime()
                    {
                        WeekTime = new TimePeriod()
                        {
                            FromTime = p.IssueDate.StartTime,
                            EndTime = p.IssueDate.EndTime
                        },
                        WeekendTime = new TimePeriod()
                        {
                            FromTime = p.IssueDate.StartTime,
                            EndTime = p.IssueDate.EndTime
                        }
                    };
                    pc.TFGTime = new WorkTime()
                    {
                        WeekTime = new TimePeriod()
                        {
                            FromTime = p.IssueDate.StartTime,
                            EndTime = p.IssueDate.EndTime
                        },
                        WeekendTime = new TimePeriod()
                        {
                            FromTime = p.IssueDate.StartTime,
                            EndTime = p.IssueDate.EndTime
                        }
                    };
                    pc.Remark = p.Remark;
                    pc.TravelType = p.TravelType == EnumTravelType.Return ? TravelType.Twoway : (p.TravelType == EnumTravelType.Connecting ? TravelType.Connway : TravelType.Oneway);
                    pc.PolicyType = p.LocalPolicyType == "B2B" ? PolicyType.B2B : PolicyType.BSP;
                    pc.Point = pc.PolicySourceType == EnumPolicySourceType.Local ? p.LocalPoint : p.Different;
                    pc.OldPoint = pc.PolicySourceType == EnumPolicySourceType.Local ? p.LocalPoint : p.Different;
                    pc.PlatformCode = p.Code;
                    pc.CacheDate = System.DateTime.Now;
                    if (p is SpecialPolicy)
                    {
                        SpecialPolicy specialPolicy = p as SpecialPolicy;
                        if (specialPolicy.SpecialType == SpeciaType.Dynamic)
                            pc.PolicySpecialType = EnumPolicySpecialType.DynamicSpecial;
                        else
                        {
                            switch (specialPolicy.Type)
                            {
                                case FixedOnSaleType.Credit:
                                    pc.PolicySpecialType = EnumPolicySpecialType.DiscountOnDiscount;
                                    break;
                                case FixedOnSaleType.Fixed:
                                    pc.PolicySpecialType = EnumPolicySpecialType.FixedSpecial;
                                    break;
                                case FixedOnSaleType.Plummeted:
                                    pc.PolicySpecialType = EnumPolicySpecialType.DownSpecial;
                                    break;
                                default:
                                    break;
                            }
                            pc.SpecialPriceOrDiscount = specialPolicy.FixedSeatPirce;
                        }
                    }
                    //上下班时间判断
                    StartAndEndTime WorkTime = null;
                    StartAndEndTime ReturnTicketTime = null;
                    StartAndEndTime AnnulTicketTime = null;
                    //设置时间
                    SetTime(p, out WorkTime, out ReturnTicketTime, out AnnulTicketTime);
                    if (WorkTime != null && WorkTime.ToString() != "-")
                    {
                        string CurrTime = System.DateTime.Now.ToString("HH:mm");
                        int start = -1;
                        int end = -1;
                        start = FormatPNR.CompareTime(CurrTime, WorkTime.StartTime);
                        end = FormatPNR.CompareTime(WorkTime.EndTime, CurrTime);
                        if (!(start > 0 && end > 0))
                        {
                            return;
                        }
                    }
                    PolicyCacheList.Add(pc);
                });
            }
            catch (Exception ex)
            {
                new CommLog().WriteLog("GetFlightPolicy", ex.Message);
            }
            return PolicyCacheList;
        }

        private void FilterPolicy(UserRelation userRealtion, string platformCode, EnumPolicySourceType PolicySourceType, PolicyParam policyParam, List<Policy> listPolicy, PnrData pnrData, bool IsGetParent = false)
        {
            try
            {
                //上级运营商code
                string carrierCode = userRealtion.carrier.Code;
                //运营是否启用
                bool carrierIsEnable = userRealtion.carrier.IsEnable;
                var result = this.localPolicyRepository.FindAll();
                //本地政策
                if (PolicySourceType == EnumPolicySourceType.Local)
                {
                    //查询所有本地政策
                    result = result.Where(p => p.CarrierCode == carrierCode);
                    //查询该商户的上级运营   
                    if (IsGetParent)
                    {
                        //政策为运营商政策并且（运营商状态为启用用并且运营的本地政策开启） 则查询运营商政策
                        result = result.Where(p => p.Code == carrierCode && carrierIsEnable && userRealtion.carrier.LocalPolicySwitch);
                    }
                }
                //共享政策
                else if (PolicySourceType == EnumPolicySourceType.Share)
                {
                    //查询所有非本地政策 
                    result = result.Where(p => p.CarrierCode != carrierCode);
                }
                //没有挂起的政策
                result = result.Where(p => !p.HangUp);
                //已审核的政策
                result = result.Where(p => p.Review);
                int a = 0;
                a = result.Count();
                //限制的班期 星期一到星期天 1-7
                string strDayOfWeek = ((int)System.DateTime.Now.DayOfWeek).ToString();
                if (strDayOfWeek == "0") strDayOfWeek = "7";
                result = result.Where(p => !string.IsNullOrEmpty(p.WeekLimit) ? !p.WeekLimit.Contains(strDayOfWeek) : true);
                //a = result.Count();
                //是否换编码政策
                if (!policyParam.IsChangePnrTicket)
                {
                    result = result.Where(p => p.ChangeCode == false);
                }
                //a = result.Count();
                if (pnrData.PatMode.UninuePatList.Count > 1)
                {
                    //高低价格匹配一一对应
                    result = result.Where(p => p.Low == policyParam.IsLowPrice);
                }
                //a = result.Count();
                List<LegInfo> legs = pnrData.PnrMode != null && pnrData.PnrMode._LegList.Count > 0 ? pnrData.PnrMode._LegList : null;
                //int a = result.Count();
                if (legs != null && legs.Count > 0)
                {
                    //航线
                    foreach (LegInfo leg in legs)
                    {
                        //出发城市
                        result = result.Where(p => p.FromCityCodes.ToUpper().Contains(leg.FromCode.ToUpper()));
                        //到达城市
                        result = result.Where(p => p.ToCityCodes.ToUpper().Contains(leg.ToCode.ToUpper()));
                        //航空公司
                        result = result.Where(p => p.CarrayCode.ToUpper() == leg.AirCode.ToUpper());
                        //a = result.Count();
                        //舱位
                        result = result.Where(p => p.Seats.ToUpper().Contains(leg.Seat.ToUpper()));
                        //起飞日期
                        DateTime StartDateTime = DateTime.Parse(leg.FlyDate1 + " " + leg.FlyStartTime.Insert(2, ":") + ":00");
                        //乘机日期
                        result = result.Where(p => StartDateTime >= p.PassengeDate.StartTime && StartDateTime <= p.PassengeDate.EndTime);
                        //出票日期
                        //result = result.Where(p => StartDateTime >= p.IssueDate.StartTime && StartDateTime <= p.IssueDate.EndTime);
                        //a = result.Count();
                    }
                    //a = result.Count();
                    StartAndEndTime WorkTime = null;
                    StartAndEndTime ReturnTicketTime = null;
                    StartAndEndTime AnnulTicketTime = null;
                    decimal SeatPrice = 0m, TaxFare = 0m, RQFare = 0m;
                    //执行查询  
                    List<LocalPolicy> LocalPolicyList = result.ToList();
                    if (LocalPolicyList != null && LocalPolicyList.Count > 0)
                    {
                        DataBill dataBill = new DataBill();
                        LocalPolicyList.ForEach(p =>
                        {
                            #region 政策开关
                            bool IsPass = false;
                            //本地政策
                            if (PolicySourceType == EnumPolicySourceType.Local)
                            {
                                if (!IsGetParent)
                                {
                                    //政策为运营商政策并且（运营商状态为启用用并且运营的本地政策开启） 则查询运营商政策
                                    //政策为供应商的 该供应商的是本地供应商并且供应商状态是开启的 供应商的本地政策也开启 则查询
                                    IsPass = p.RoleType == "Carrier" ?
                                          (carrierIsEnable && userRealtion.carrier.LocalPolicySwitch)
                                          : IsCanGetPolicy(p, userRealtion, carrierCode, 0);
                                }
                            }
                            //共享政策
                            else if (PolicySourceType == EnumPolicySourceType.Share)
                            {
                                //采购异地政策开关 异地的运营和供应账号状态开启并且对外异地政策开关开启 则查到
                                IsPass = userRealtion.carrier.BuyerRemotoPolicySwich &&
                                     (
                                         p.RoleType == "Carrier" ? IsCanGetPolicy(p, userRealtion, carrierCode, 2) : IsCanGetPolicy(p, userRealtion, carrierCode, 1)
                                     );
                            }
                            if (!IsPass) return;
                            #endregion

                            #region //排除适用和不适用的航班号
                            if (p.ApplyFlights != null)
                            {
                                foreach (LegInfo leg in legs)
                                {
                                    bool Apply = (
                                         p.ApplyFlights.ToUpper().Contains(leg.FlightNum.Trim())
                                        || p.ApplyFlights.ToUpper().Contains((leg.AirCode + leg.FlightNum).ToUpper().Trim())
                                        || p.ApplyFlights.ToUpper().Contains(leg.FlightNum.Trim() + ",")
                                        || p.ApplyFlights.ToUpper().Contains("," + leg.FlightNum.Trim())
                                        || p.ApplyFlights.ToUpper().Contains((leg.AirCode + leg.FlightNum).ToUpper().Trim() + ",")
                                        || p.ApplyFlights.ToUpper().Contains("," + (leg.AirCode + leg.FlightNum).ToUpper().Trim())
                                        );
                                    if (p.Apply == EnumApply.Apply)
                                    {
                                        if (!Apply)
                                        {
                                            return;
                                        }
                                    }
                                    else if (p.Apply == EnumApply.NotApply)
                                    {
                                        if (Apply)
                                        {
                                            return;
                                        }
                                    }
                                }
                            }
                            #endregion

                            #region 工作时间
                            //设置时间
                            SetTime(p, out WorkTime, out ReturnTicketTime, out AnnulTicketTime);
                            if (WorkTime != null && WorkTime.ToString() != "-")
                            {
                                string CurrTime = System.DateTime.Now.ToString("HH:mm");
                                int start = -1;
                                int end = -1;
                                start = FormatPNR.CompareTime(CurrTime, WorkTime.StartTime);
                                end = FormatPNR.CompareTime(WorkTime.EndTime, CurrTime);
                                if (!(start > 0 && end > 0))
                                {
                                    return;
                                }
                            }
                            #endregion


                            decimal OriginalPolicyPoint = PolicySourceType == EnumPolicySourceType.Share ? p.Different : p.LocalPoint;
                            Policy policy = new Policy()
                            {
                                PolicyId = p.ID.ToString(),
                                OrderId = policyParam.OrderId,
                                CarryCode = p.CarrayCode,
                                PlatformCode = platformCode,
                                DownPoint = 0m,
                                OriginalPolicyPoint = OriginalPolicyPoint,
                                PaidPoint = OriginalPolicyPoint,
                                PolicyPoint = OriginalPolicyPoint,
                                ReturnMoney = 0m,
                                Remark = p.Remark,
                                IsChangePNRCP = p.ChangeCode,
                                IsSp = false,
                                PolicyType = p.LocalPolicyType,
                                WorkTime = WorkTime,
                                ReturnTicketTime = ReturnTicketTime,
                                AnnulTicketTime = AnnulTicketTime,
                                CPOffice = p.Office,
                                AreaCity = "",
                                CarrierCode = p.CarrierCode,//重要
                                Code = p.Code,//重要
                                PolicySourceType = PolicySourceType,
                                EnumIssueTicketWay = p.IssueTicketWay,
                                IsLow = p.Low,
                                Commission = dataBill.GetCommission(OriginalPolicyPoint, SeatPrice, 0m),
                                PolicyOwnUserRole = p.RoleType == "Supplier" ? EnumPolicyOwnUserRole.Supplier : EnumPolicyOwnUserRole.Carrier
                            };
                            if (p is SpecialPolicy)
                            {
                                SpecialPolicy specialPolicy = p as SpecialPolicy;
                                if (specialPolicy.SpecialType == SpeciaType.Dynamic)
                                    policy.PolicySpecialType = EnumPolicySpecialType.DynamicSpecial;
                                else
                                {
                                    switch (specialPolicy.Type)
                                    {
                                        case FixedOnSaleType.Credit:
                                            policy.PolicySpecialType = EnumPolicySpecialType.DiscountOnDiscount;
                                            break;
                                        case FixedOnSaleType.Fixed:
                                            policy.PolicySpecialType = EnumPolicySpecialType.FixedSpecial;
                                            break;
                                        case FixedOnSaleType.Plummeted:
                                            policy.PolicySpecialType = EnumPolicySpecialType.DownSpecial;
                                            break;
                                        default:
                                            break;
                                    }
                                    policy.SpecialPriceOrDiscount = specialPolicy.FixedSeatPirce;
                                }
                            }
                            //根据特价类型显示价格
                            PatInfo pat = GetPat(platformCode, policyParam, p.Low, pnrData);
                            decimal.TryParse(pat.Fare, out SeatPrice);
                            decimal.TryParse(pat.TAX, out TaxFare);
                            decimal.TryParse(pat.RQFare, out RQFare);
                            //特价的政策 需要计算特价的舱位价
                            if (policy.PolicySpecialType != EnumPolicySpecialType.Normal)
                            {
                                //处理特价
                                if (policy.PolicySpecialType == EnumPolicySpecialType.FixedSpecial)
                                {
                                    //固定特价不为0时 更改舱位价为固定特价
                                    if (policy.SpecialPriceOrDiscount != 0 && !policyParam.IsUseSpecial)
                                        SeatPrice = policy.SpecialPriceOrDiscount;
                                }
                                else if (policy.PolicySpecialType == EnumPolicySpecialType.DownSpecial)
                                {
                                    //直降  直降不为0时 舱位价中减除直降的价格   
                                    if (policy.SpecialPriceOrDiscount != 0 && !policyParam.IsUseSpecial)
                                        SeatPrice -= policy.SpecialPriceOrDiscount;
                                }
                                else if (policy.PolicySpecialType == EnumPolicySpecialType.DiscountOnDiscount)
                                {
                                    //折上折 折扣不为0 在现有的舱位价上根据折扣再次计算
                                    if (policy.SpecialPriceOrDiscount != 0 && !policyParam.IsUseSpecial)
                                    {
                                        decimal zk = policy.SpecialPriceOrDiscount / 100;
                                        SeatPrice *= zk;
                                        //进到十位
                                        SeatPrice = dataBill.CeilAddTen((int)SeatPrice);
                                    }
                                }
                                //舱位价小于0时为0处理
                                if (SeatPrice <= 0)
                                {
                                    return;
                                }
                                policy.SeatPrice = SeatPrice;
                                policy.Commission = dataBill.GetCommission(policy.PolicyPoint, SeatPrice, 0m);
                            }
                            policy.SeatPrice = SeatPrice;
                            policy.ABFee = TaxFare;
                            policy.RQFee = RQFare;
                            //添加结果集
                            listPolicy.Add(policy);
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                string strDir = string.Empty;
                if (PolicySourceType == EnumPolicySourceType.Share)
                {
                    strDir = "GetSharePolicy";
                }
                else
                {
                    strDir = "GetLocalPolicy";
                }
                new CommLog().WriteLog(strDir, ex.Message + ex.StackTrace);
            }
        }

        /// <summary>
        /// 取各个平台的价格
        /// </summary>
        /// <param name="PnrContent">编码内容</param>
        /// <param name="IsLow"></param>
        /// <param name="IsChdSeat"></param>
        /// <returns></returns>
        private PatInfo GetPat(string PlatformCode, PolicyParam policyParam, bool isLow, PnrData pnrData)
        {
            PatInfo pat = new PatInfo();
            if ((PlatformCode != "系统" && !policyParam.IsDestine)
                || (PlatformCode == "系统"
                && !policyParam.IsUseSpecial
                && policyParam.PolicySpecialType != EnumPolicySpecialType.Normal
                && policyParam.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial))
            {
                //特价政策取航班查询传过来的价格
                pat.Fare = policyParam.defFare;
                pat.TAX = policyParam.defTAX;
                pat.RQFare = policyParam.defRQFare;
            }
            else
            {
                //成人或者儿童PAT
                PatModel patMode = pnrData.PatMode;
                PnrModel pnrMode = pnrData.PnrMode;
                //设置默认价格
                pat.Fare = policyParam.defFare;
                pat.TAX = policyParam.defTAX;
                pat.RQFare = policyParam.defRQFare;
                if (patMode != null && patMode.UninuePatList.Count > 0)
                {
                    //存在子舱位 取子舱位
                    if (pnrMode.IsExistChildSeat && patMode.ChildPat != null)
                    {
                        pat = patMode.ChildPat;//子舱位价格                   
                    }
                    else
                    {
                        //默认取高价
                        pat = patMode.UninuePatList[patMode.UninuePatList.Count - 1];
                        //取低价
                        if (isLow && patMode.UninuePatList.Count > 1)
                        {
                            pat = patMode.UninuePatList[0];
                        }
                    }
                }
            }
            return pat;
        }
        private void SetTime(LocalPolicy localPolicy, out StartAndEndTime WorkTime, out StartAndEndTime ReturnTicketTime, out StartAndEndTime AnnulTicketTime)
        {
            WorkTime = null;
            ReturnTicketTime = null;
            AnnulTicketTime = null;
            string[] strArr = null;
            string strTime = string.Empty;
            var dayOfWeek = (int)DateTime.Now.DayOfWeek;
            string curDayOfWeek = dayOfWeek == 0 ? "7" : dayOfWeek.ToString();
            if (localPolicy.RoleType == "Carrier")
            {
                //周末
                //if (System.DateTime.Now.DayOfWeek == DayOfWeek.Saturday
                //    || System.DateTime.Now.DayOfWeek == DayOfWeek.Sunday)                 
                if (localPolicy.CarrierWeek != null && localPolicy.CarrierWeek.Contains(curDayOfWeek))
                {
                    WorkTime = localPolicy.Carrier_WeekWorkTime;
                    ReturnTicketTime = localPolicy.Carrier_WeekReturnTicketTime;
                    AnnulTicketTime = localPolicy.Carrier_WeekAnnulTicketTime;
                }
                else
                {
                    //非周末
                    WorkTime = localPolicy.Carrier_WorkTime;
                    ReturnTicketTime = localPolicy.Carrier_ReturnTicketTime;
                    AnnulTicketTime = localPolicy.Carrier_AnnulTicketTime;
                }
            }
            else if (localPolicy.RoleType == "Supplier")
            {
                //周末
                //if (System.DateTime.Now.DayOfWeek == DayOfWeek.Saturday
                //    || System.DateTime.Now.DayOfWeek == DayOfWeek.Sunday
                StartAndEndTime CarrierWorkTime = localPolicy.Carrier_WeekWorkTime;
                StartAndEndTime CarrierReturnTicketTime = localPolicy.Carrier_WeekReturnTicketTime;
                StartAndEndTime CarrierAnnulTicketTime = localPolicy.Carrier_WeekAnnulTicketTime;
                if (localPolicy.CarrierWeek != null && !localPolicy.CarrierWeek.Contains(curDayOfWeek))
                {
                    CarrierWorkTime = localPolicy.Carrier_WorkTime;
                    CarrierReturnTicketTime = localPolicy.Carrier_ReturnTicketTime;
                    CarrierAnnulTicketTime = localPolicy.Carrier_AnnulTicketTime;
                }
                if (localPolicy.SupplierWeek != null && localPolicy.SupplierWeek.Contains(curDayOfWeek))
                {
                    StartAndEndTime SupplierWorkTime = localPolicy.WeeKWorkTime;
                    StartAndEndTime SupplierReturnTicketTime = localPolicy.WeekReturnTicketTime;
                    StartAndEndTime SupplierAnnulTicketTime = localPolicy.WeekAnnulTicketTime;


                    //取交集
                    strTime = FormatPNR.GetIntersectionTimeSlot(CarrierWorkTime.ToString(), SupplierWorkTime.ToString());
                    strArr = strTime.Split('-');
                    if (strArr.Length == 2)
                    {
                        WorkTime = new StartAndEndTime()
                        {
                            StartTime = strArr[0],
                            EndTime = strArr[1]
                        };
                        strTime = FormatPNR.GetIntersectionTimeSlot(CarrierReturnTicketTime.ToString(), SupplierReturnTicketTime.ToString());
                        strArr = strTime.Split('-');
                        ReturnTicketTime = new StartAndEndTime()
                        {
                            StartTime = strArr[0],
                            EndTime = strArr[1]
                        };
                        strTime = FormatPNR.GetIntersectionTimeSlot(CarrierAnnulTicketTime.ToString(), SupplierAnnulTicketTime.ToString());
                        strArr = strTime.Split('-');
                        AnnulTicketTime = new StartAndEndTime()
                        {
                            StartTime = strArr[0],
                            EndTime = strArr[1]
                        };
                    }
                }
                else
                {
                    StartAndEndTime SupplierWorkTime = localPolicy.WorkTime;
                    StartAndEndTime SupplierReturnTicketTime = localPolicy.ReturnTicketTime;
                    StartAndEndTime SupplierAnnulTicketTime = localPolicy.AnnulTicketTime;

                    //取交集
                    strTime = FormatPNR.GetIntersectionTimeSlot(CarrierWorkTime.ToString(), SupplierWorkTime.ToString());
                    strArr = strTime.Split('-');
                    if (strArr.Length == 2)
                    {
                        WorkTime = new StartAndEndTime()
                        {
                            StartTime = strArr[0],
                            EndTime = strArr[1]
                        };
                        strTime = FormatPNR.GetIntersectionTimeSlot(CarrierReturnTicketTime.ToString(), SupplierReturnTicketTime.ToString());
                        strArr = strTime.Split('-');
                        ReturnTicketTime = new StartAndEndTime()
                        {
                            StartTime = strArr[0],
                            EndTime = strArr[1]
                        };
                        strTime = FormatPNR.GetIntersectionTimeSlot(CarrierAnnulTicketTime.ToString(), SupplierAnnulTicketTime.ToString());
                        strArr = strTime.Split('-');
                        AnnulTicketTime = new StartAndEndTime()
                        {
                            StartTime = strArr[0],
                            EndTime = strArr[1]
                        };
                    }
                }
            }
        }
    }
}
