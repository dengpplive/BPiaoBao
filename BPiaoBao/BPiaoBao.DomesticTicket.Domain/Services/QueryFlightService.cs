using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using StructureMap;
namespace BPiaoBao.DomesticTicket.Domain.Services
{
    /// <summary>
    /// 查询航班服务
    /// </summary>
    public class QueryFlightService
    {
        DataBill dataBill = new DataBill();
        CommLog log = new CommLog();
        GetFlightBasicData test = new GetFlightBasicData();
        /// <summary>
        /// 获取航班数据
        /// </summary>
        /// <param name="avhList"></param>
        /// <param name="PolicyList"></param>
        /// <returns></returns>
        public List<AVHData> MatchPolicy(string code, List<AVHData> avhList, TravelType travelType, List<PolicyCache> PolicyList)
        {
            DataBill dataBill = new DataBill();
            try
            {
                //PolicyList = PolicyList.Where(p => p.PlatformCode == "系统").ToList();
                if (avhList != null && PolicyList != null && avhList.Count > 0 && PolicyList.Count > 0)
                {
                    //周六至周日
                    DayOfWeek[] WeekendTimes = new DayOfWeek[]
                    {
                        DayOfWeek.Saturday,
                        DayOfWeek.Sunday
                    };
                    //调整缓存政策
                    PolicyList = PolicyCacheSetting(PolicyList);
                    PolicyList = DeductionSetting(code, avhList, PolicyList);
                    List<string> codeList = ObjectFactory.GetAllInstances<IPlatform>().Select(p => p.Code).ToList();
                    for (int i = 0; i < avhList.Count; i++)
                    {
                        AVHData avhData = avhList[i];
                        DateTime flyDate = DateTime.Parse(avhData.QueryParam.FlyDate);

                        //循环IBE数据
                        for (int j = 0; j < avhData.IbeData.Count; j++)
                        {
                            //排除没有舱位的
                            if (avhData.IbeData[j].IBESeat == null || avhData.IbeData[j].IBESeat.Count() == 0)
                            {
                                continue;
                            }
                            //循环舱位 每个舱位可能对应多条政策 取最优的一条  
                            List<IbeSeat> addRowList = new List<IbeSeat>();
                            for (int k = 0; k < avhData.IbeData[j].IBESeat.Count; k++)
                            {
                                avhData.IbeData[j].IBESeat[k].IbeSeatPrice = avhData.IbeData[j].IBESeat[k].SeatPrice;
                                if (avhData.DicYSeatPrice != null
                                    && avhData.DicYSeatPrice.ContainsKey(avhData.IbeData[j].CarrierCode)
                                    )
                                {
                                    if (avhData.IbeData[j].IBESeat[k].SeatPrice > 0)
                                    {
                                        avhData.IbeData[j].IBESeat[k].Commission = dataBill.GetCommission(avhData.IbeData[j].IBESeat[k].Policy, avhData.IbeData[j].IBESeat[k].SeatPrice, avhData.IbeData[j].IBESeat[k].ReturnMoney);
                                    }
                                }
                                else
                                {
                                    //没有Y舱直接过滤
                                    continue;
                                }

                                //每个舱位可能对应多条政策 政策过滤
                                var result = PolicyList.Where(p => Filter(p, codeList, avhList, avhData, travelType, flyDate, WeekendTimes, j, k)).ToList();

                                #region 特价舱位匹配政策
                                //if (avhList.Count == 1)//暂时只适用单程
                                if (true)
                                {
                                    //特价舱位 不通的特价类型的多条政策
                                    List<PolicyCache> spPolicyList = result.Where(p => p.PolicySpecialType != EnumPolicySpecialType.Normal).OrderByDescending(p => p.Point).ToList();//.FirstOrDefault();
                                    if (spPolicyList != null && spPolicyList.Count > 0)
                                    {
                                        //获取4种类型政策最优的特价
                                        List<PolicyCache> spList = new List<PolicyCache>();
                                        PolicyCache spDynamicSpecial = spPolicyList.Where(p => p.PolicySpecialType == EnumPolicySpecialType.DynamicSpecial).OrderByDescending(p => p.Point).FirstOrDefault();
                                        if (spDynamicSpecial != null) spList.Add(spDynamicSpecial);
                                        PolicyCache spFixedSpecial = spPolicyList.Where(p => p.PolicySpecialType == EnumPolicySpecialType.FixedSpecial).OrderByDescending(p => p.Point).FirstOrDefault();
                                        if (spFixedSpecial != null) spList.Add(spFixedSpecial);
                                        PolicyCache spDownSpecial = spPolicyList.Where(p => p.PolicySpecialType == EnumPolicySpecialType.DownSpecial).OrderByDescending(p => p.Point).FirstOrDefault();
                                        if (spDownSpecial != null) spList.Add(spDownSpecial);
                                        PolicyCache spDiscountOnDiscount = spPolicyList.Where(p => p.PolicySpecialType == EnumPolicySpecialType.DiscountOnDiscount).OrderByDescending(p => p.Point).FirstOrDefault();
                                        if (spDiscountOnDiscount != null) spList.Add(spDiscountOnDiscount);
                                        //循环克隆一份舱位
                                        spList.ForEach(spPolicy =>
                                        {
                                            IbeSeat ibeSeat = avhData.IbeData[j].IBESeat[k].Clone() as IbeSeat;
                                            if (ibeSeat != null)
                                            {
                                                ibeSeat.Policy = spPolicy.Point;
                                                ibeSeat.PolicySpecialType = spPolicy.PolicySpecialType;
                                                ibeSeat.SpecialPriceOrDiscount = spPolicy.SpecialPriceOrDiscount;
                                                ibeSeat.PolicyRMK = spPolicy.Remark;
                                                ibeSeat.PolicyId = spPolicy.PolicyId;
                                                ibeSeat.PlatformCode = spPolicy.PlatformCode;
                                                //处理特价
                                                if (spPolicy.PolicySpecialType == EnumPolicySpecialType.FixedSpecial)
                                                {
                                                    //固定特价不为0时 更改舱位价为固定特价
                                                    if (ibeSeat.SpecialPriceOrDiscount != 0)
                                                        ibeSeat.SeatPrice = ibeSeat.SpecialPriceOrDiscount;
                                                }
                                                else if (spPolicy.PolicySpecialType == EnumPolicySpecialType.DownSpecial)
                                                {
                                                    //直降  直降不为0时 舱位价中减除直降的价格   
                                                    if (ibeSeat.SpecialPriceOrDiscount != 0)
                                                        ibeSeat.SeatPrice -= ibeSeat.SpecialPriceOrDiscount;
                                                }
                                                else if (spPolicy.PolicySpecialType == EnumPolicySpecialType.DiscountOnDiscount)
                                                {
                                                    //折上折 折扣不为0 在现有的舱位价上根据折扣再次计算
                                                    if (ibeSeat.SpecialPriceOrDiscount != 0)
                                                    {
                                                        decimal zk = ibeSeat.SpecialPriceOrDiscount / 100;
                                                        ibeSeat.SeatPrice *= zk;
                                                        //进到十位
                                                        ibeSeat.SeatPrice = dataBill.CeilAddTen((int)ibeSeat.SeatPrice);
                                                    }
                                                }
                                                //舱位价小于0时为0处理
                                                if (ibeSeat.SeatPrice <= 0)
                                                {
                                                    ibeSeat.SeatPrice = 0;
                                                }
                                                ibeSeat.Commission = dataBill.GetCommission(spPolicy.Point, ibeSeat.SeatPrice, ibeSeat.ReturnMoney);
                                                //屏蔽非单程的非动态特价
                                                //if (avhList.Count > 1 && ibeSeat.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial)
                                                //    return;
                                                //添加到集合
                                                addRowList.Add(ibeSeat);
                                            }
                                        });
                                    }
                                }
                                #endregion

                                //普通舱位
                                PolicyCache policy = result.Where(p => p.PolicySpecialType == EnumPolicySpecialType.Normal).OrderByDescending(p => p.Point).FirstOrDefault();
                                if (policy != null)
                                {
                                    //排除异地政策为0的显示
                                    if (policy.PolicySourceType == EnumPolicySourceType.Share && policy.OldPoint == 0) continue;
                                    avhData.IbeData[j].IBESeat[k].Policy = policy.Point;
                                    avhData.IbeData[j].IBESeat[k].PolicySpecialType = policy.PolicySpecialType;
                                    avhData.IbeData[j].IBESeat[k].SpecialPriceOrDiscount = policy.SpecialPriceOrDiscount;
                                    avhData.IbeData[j].IBESeat[k].PolicyRMK = policy.Remark;
                                    avhData.IbeData[j].IBESeat[k].PolicyId = policy.PolicyId;
                                    avhData.IbeData[j].IBESeat[k].PlatformCode = policy.PlatformCode;
                                    avhData.IbeData[j].IBESeat[k].Commission = dataBill.GetCommission(policy.Point, avhData.IbeData[j].IBESeat[k].SeatPrice, avhData.IbeData[j].IBESeat[k].ReturnMoney);
                                }
                            }
                            if (addRowList.Count > 0)
                            {
                                avhData.IbeData[j].IBESeat.AddRange(addRowList.ToArray());
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //记录日志
                log.WriteLog("MatchPolicy", "异常信息:" + ex.Message + "\r\n");
            }
            return avhList;
        }

        private bool Filter(PolicyCache policy, List<string> InterfaceCode, List<AVHData> avhList, AVHData avhData, TravelType travelType, DateTime flyDate, DayOfWeek[] WeekendTimes, int i, int k)
        {
            bool Issuc = false;
            try
            {
                //出发城市
                if (!(policy.FromCityCode != null && policy.FromCityCode.Contains(avhData.QueryParam.FromCode)))
                {
                    return Issuc;
                }
                //到达城市
                if (!(policy.ToCityCode != null && policy.ToCityCode.Contains(avhData.QueryParam.ToCode)))
                {
                    return Issuc;
                }
                //乘机有效期
                if (!(flyDate > policy.CheckinTime.FromTime && flyDate < policy.CheckinTime.EndTime))
                {
                    return Issuc;
                }
                //出票有效期
                //if (!(flyDate > policy.IssueTime.FromTime && flyDate < policy.IssueTime.EndTime))
                //{
                //    return Issuc;
                //}
                //适用星期
                if (policy.SuitableWeek.Length > 0)
                {
                    if (!policy.SuitableWeek.Contains(flyDate.DayOfWeek))
                    {
                        return Issuc;
                    }
                }
                //承运人
                if (policy.CarrierCode.ToUpper() != avhData.IbeData[i].CarrierCode)
                {
                    return Issuc;
                }
                //适用舱位
                if (policy.CabinSeatCode.Length == 0
                    || !policy.CabinSeatCode.Contains(avhData.IbeData[i].IBESeat[k].Seat))
                {
                    return Issuc;
                }
                if (policy.Applay == EnumApply.Apply)
                {
                    //适用航班号
                    if (policy.SuitableFlightNo.Length == 0
                        || !policy.SuitableFlightNo.Contains(avhData.IbeData[i].FlightNo)
                        )
                    {
                        return Issuc;
                    }
                }
                else if (policy.Applay == EnumApply.NotApply)
                {
                    //排除航班号
                    if (policy.ExceptedFlightNo.Length > 0
                        && policy.ExceptedFlightNo.Contains(avhData.IbeData[i].FlightNo))
                    {
                        return Issuc;
                    }
                }
                //第二程
                if (avhList.Count > 1)
                {
                    string carryCode = avhData.IbeData[i].CarrierCode;
                    string flightNo = avhData.IbeData[i].FlightNo;
                    string seat = avhData.IbeData[i].IBESeat[k].Seat;
                    var IbeRowList = avhList[1].IbeData.Where(p => p.CarrierCode.ToUpper().Trim() == carryCode.ToUpper().Trim()).ToList();
                    //承运人 
                    if (policy.CarrierCode.ToUpper() != carryCode || IbeRowList == null)
                    {
                        return Issuc;
                    }
                    int seatCount = IbeRowList.Where(p => p.IBESeat.Where(p1 => p1.Seat == seat).Count() > 0).Count();
                    //适用舱位 
                    if (policy.CabinSeatCode.Length == 0
                        || seatCount == 0
                        )
                    {
                        return Issuc;
                    }
                }
                if (InterfaceCode.Contains(policy.PlatformCode))
                {
                    ////适用航班号
                    //if (policy.SuitableFlightNo.Length > 0)
                    //{
                    //    if (!policy.SuitableFlightNo.Contains(avhData.IbeData[i].FlightNo))
                    //    {
                    //        return Issuc;
                    //    }
                    //}
                    ////排除航班号
                    //if (policy.ExceptedFlightNo.Length > 0 && policy.ExceptedFlightNo.Contains(avhData.IbeData[i].FlightNo))
                    //{
                    //    return Issuc;
                    //}
                    //上下班时间
                    //周六周日
                    if (WeekendTimes.Contains(System.DateTime.Now.DayOfWeek))
                    {
                        if (!(DateTime.Compare(DateTime.Parse(policy.ServiceTime.WeekendTime.FromTime.ToString("HH:mm:ss")), DateTime.Parse(System.DateTime.Now.ToString("HH:mm:ss"))) < 0
                            && DateTime.Compare(DateTime.Parse(policy.ServiceTime.WeekendTime.EndTime.ToString("HH:mm:ss")), DateTime.Parse(System.DateTime.Now.ToString("HH:mm:ss"))) > 0
                            ))
                        {
                            return Issuc;
                        }
                    }
                    else
                    {
                        //周一至周五
                        if (!(DateTime.Compare(DateTime.Parse(policy.ServiceTime.WeekTime.FromTime.ToString("HH:mm:ss")), DateTime.Parse(System.DateTime.Now.ToString("HH:mm:ss"))) < 0
                            && DateTime.Compare(DateTime.Parse(policy.ServiceTime.WeekTime.EndTime.ToString("HH:mm:ss")), DateTime.Parse(System.DateTime.Now.ToString("HH:mm:ss"))) > 0
                            ))
                        {
                            return Issuc;
                        }
                    }

                }
                //....
            }
            catch (Exception ex)
            {
                log.WriteLog("MatchPolicy_1", "异常信息:" + ex.Message + ex.StackTrace + "\r\n");
            }
            Issuc = true;
            return Issuc;
        }



        /// <summary>
        /// 获取政策
        /// </summary>
        /// <returns></returns>
        public List<PolicyCache> GetPolicy(PolicyQueryParam queryParam)
        {
            //查询政策。。。
            List<PolicyCache> policyList = new List<PolicyCache>();
            bool queryFlightCachePolicyClose = false;
            try
            {
                string QueryFlightCachePolicyClose = System.Configuration.ConfigurationManager.AppSettings["QueryFlightCachePolicyClose"];
                bool.TryParse(QueryFlightCachePolicyClose, out queryFlightCachePolicyClose);
            }
            catch { }
            if (queryFlightCachePolicyClose) return policyList;
            StringBuilder sbWhere = new StringBuilder();
            sbWhere.AppendFormat(" TravelType='{0}' ", ((int)queryParam.TravelType).ToString());
            try
            {
                //过滤不查询的航空公司
                List<string> airCloseList = new List<string>();
                SystemConsoSwitch.AirSystems.ForEach(p =>
                {
                    if (!p.IsQuery)
                    {
                        airCloseList.Add(p.AirCode.ToUpper());
                    }
                });
                //承运人
                if (!string.IsNullOrEmpty(queryParam.InputParam[0].CarrierCode)
                    && !airCloseList.Contains(queryParam.InputParam[0].CarrierCode))
                {
                    sbWhere.AppendFormat(" and CarrierCode like '{0}'", queryParam.InputParam[0].CarrierCode);
                }
                else
                {
                    //过滤不查询的航空公司
                    if (airCloseList.Count > 0)
                    {
                        List<string> tempList = new List<string>();
                        airCloseList.ForEach(p =>
                        {
                            tempList.Add("'" + p.ToUpper().Trim() + "'");
                        });
                        if (tempList.Count > 0)
                            sbWhere.AppendFormat(" and CarrierCode not in({0})", string.Join(",", tempList.ToArray()));
                    }
                }
                if (queryParam.InputParam.Count > 1)
                {
                    if (queryParam.TravelType == TravelType.Twoway)
                    {
                        sbWhere.AppendFormat(" and  FromCityCode Like '%{0}%' and ToCityCode like '%{1}%' ", queryParam.InputParam[0].FromCode, queryParam.InputParam[0].ToCode);
                        sbWhere.AppendFormat(" and  ToCityCode Like '%{0}%' and FromCityCode like '%{1}%' ", queryParam.InputParam[1].FromCode, queryParam.InputParam[1].ToCode);
                    }
                    else if (queryParam.TravelType == TravelType.Connway)
                    {
                        sbWhere.AppendFormat(" and FromCityCode like '%{0}%' and MidCityCode like '%{1}%' ", queryParam.InputParam[0].FromCode, queryParam.InputParam[0].ToCode);
                        sbWhere.AppendFormat(" and MidCityCode like '%{0}%' and ToCityCode like '%{1}%' ", queryParam.InputParam[1].FromCode, queryParam.InputParam[1].ToCode);
                    }
                    DateTime FlyDate = DateTime.Parse(queryParam.InputParam[0].FlyDate);
                    DateTime FlyBackDate = DateTime.Parse(queryParam.InputParam[1].FlyDate);
                    sbWhere.AppendFormat(" and '{0}'>= CheckinTime_FromTime and '{0}'<= CheckinTime_EndTime", FlyDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbWhere.AppendFormat(" and '{0}'>= IssueTime_FromTime and '{0}'<= IssueTime_EndTime", FlyDate.ToString("yyyy-MM-dd HH:mm:ss"));

                    sbWhere.AppendFormat(" and '{0}'>= CheckinTime_FromTime and '{0}'<= CheckinTime_EndTime", FlyBackDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbWhere.AppendFormat(" and '{0}'>= IssueTime_FromTime and '{0}'<= IssueTime_EndTime", FlyBackDate.ToString("yyyy-MM-dd HH:mm:ss"));
                    sbWhere.Append(" and  (getDate()<=CacheExpiresDate or CacheExpiresDate is null)");



                    policyList = test.GetPolicyCache(sbWhere.ToString());
                }
                else
                {
                    if (queryParam.TravelType == TravelType.Oneway)
                    {
                        //去回城市                                          
                        DateTime FlyDate = DateTime.Parse(queryParam.InputParam[0].FlyDate);

                        sbWhere.AppendFormat(" and  FromCityCode Like '%{0}%' and ToCityCode like '%{1}%' ", queryParam.InputParam[0].FromCode, queryParam.InputParam[0].ToCode);
                        sbWhere.AppendFormat(" and '{0}'>= CheckinTime_FromTime and '{0}'<= CheckinTime_EndTime", FlyDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        sbWhere.AppendFormat(" and '{0}'>= IssueTime_FromTime and '{0}'<= IssueTime_EndTime", FlyDate.ToString("yyyy-MM-dd HH:mm:ss"));
                        sbWhere.Append(" and  (getDate()<=CacheExpiresDate or CacheExpiresDate is null)");
                        //过滤不查询的航空公司


                        policyList = test.GetPolicyCache(sbWhere.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                StringBuilder sblog = new StringBuilder();
                sblog.Append("参数:\r\n" + queryParam.ToString());
                sblog.Append("异常信息:" + ex.Message + "\r\n");
                //记录日志
                log.WriteLog("GetPolicy", sblog.ToString());
            }
            return policyList;
        }

        //匹配扣点
        public List<PolicyCache> DeductionSetting(string code, List<AVHData> avhList, List<PolicyCache> pclist)
        {
            PlatformDeductionParam pfDp = new PlatformDeductionParam();
            foreach (AVHData leg in avhList)
            {
                pfDp.FlyLineList.Add(new FlyLine()
                {
                    CarrayCode = leg.QueryParam.CarrierCode,
                    FromCityCode = leg.QueryParam.FromCode,
                    ToCityCode = leg.QueryParam.ToCode
                });
            }
            DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
            UserRelation userRealtion = domesticService.GetUserRealtion(code);
            DeductionGroup deductionGroup = userRealtion.deductionGroup;
            List<string> codeList = ObjectFactory.GetAllInstances<IPlatform>().Select(p => p.Code).ToList();
            DeductionType deductionType = DeductionType.Interface;
            EnumPolicySourceType PolicySourceType = EnumPolicySourceType.Interface;
            //本地运营的下级供应code
            List<string> LocalSupplierCodeList = userRealtion.SupplierList.Where(p => p.CarrierCode == userRealtion.carrier.Code).Select(p => p.Code).ToList();
            for (int i = 0; i < pclist.Count; i++)
            {
                PolicyCache pc = pclist[i];
                if (codeList.Contains(pc.PlatformCode))
                {
                    deductionType = DeductionType.Interface;
                    PolicySourceType = EnumPolicySourceType.Interface;
                }
                else
                {
                    if (userRealtion.carrier.Code == pc.PlatformCode
                        || LocalSupplierCodeList.Contains(pc.PlatformCode))
                    {
                        deductionType = DeductionType.Local;
                        PolicySourceType = EnumPolicySourceType.Local;
                    }
                    else
                    {
                        deductionType = DeductionType.Share;
                        PolicySourceType = EnumPolicySourceType.Share;
                    }
                }
                pc.Point = domesticService.MatchDeductionRole(PolicyCacheToPolicy(pc, PolicySourceType), pfDp, pc.CarrierCode, deductionGroup, userRealtion, deductionType);
            }
            return pclist;
        }

        /// <summary>
        /// 缓存数据修改
        /// </summary>
        /// <param name="pclist"></param>
        /// <returns></returns>
        public List<PolicyCache> PolicyCacheSetting(List<PolicyCache> pclist)
        {
            try
            {
                string strCachePolicySetting = System.Configuration.ConfigurationManager.AppSettings["CachePolicySetting"];
                if (!string.IsNullOrEmpty(strCachePolicySetting) && pclist != null && pclist.Count > 0)
                {
                    string[] strRows = strCachePolicySetting.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    string[] rowArr = null;
                    decimal startPoint = 0m;
                    decimal endPoint = 0m;
                    decimal adujstPoint = 0m;
                    string policySource = "0";
                    pclist.ForEach(p =>
                    {
                        foreach (string row in strRows)
                        {
                            rowArr = row.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                            if (rowArr != null && rowArr.Length >= 4)
                            {
                                policySource = rowArr[0];
                                //0接口缓存 1系统政策 2所有政策
                                bool IsSet = (
                                    (policySource == "0" && p.PlatformCode != "系统")
                                    || (policySource == "1" && p.PlatformCode == "系统")
                                    || policySource == "2") ? true : false;

                                if (IsSet)
                                {
                                    if (p.Point < 0) p.Point = 0m;
                                    if (rowArr[1].ToLower() == "down") startPoint = -100m;
                                    if (rowArr[2].ToLower() == "up") endPoint = 100m;
                                    decimal.TryParse(rowArr[1], out startPoint);
                                    decimal.TryParse(rowArr[2], out endPoint);
                                    decimal.TryParse(rowArr[3], out adujstPoint);
                                    if (p.Point > startPoint && p.Point <= endPoint)
                                    {
                                        p.Point = p.Point + adujstPoint;
                                    }
                                }
                            }
                        }
                    });
                }
            }
            catch (Exception ex)
            {
                //记录日志
                log.WriteLog("PolicyCacheSetting", "缓存政策调整区间配置未设置！");
            }
            return pclist;
        }
        /// <summary>
        /// 获取基础数据
        /// </summary>
        /// <param name="travelParam"></param>
        /// <returns></returns>
        public List<AVHData> GetBasic(List<QueryParam> travelParam)
        {
            // IbeService ibe = new IbeService();
            List<AVHData> result = new List<AVHData>();
            try
            {
                if (travelParam.Count == 1)//单程
                {
                    QueryParam queryParam = travelParam[0];
                    AVHData avhData = new AVHData();
                    FDData fdData = new FDData();

                    Parallel.Invoke(
                       () => avhData = new IbeService(queryParam).GetAvh(queryParam.FromCode, queryParam.ToCode, DateTime.Parse(queryParam.FlyDate), queryParam.CarrierCode),
                       () => fdData = new IbeService(queryParam).GetFD(queryParam.FromCode, queryParam.ToCode, DateTime.Parse(queryParam.FlyDate), queryParam.CarrierCode)
                    );
                    //补全数据
                    avhData = BuQuanAVH(avhData, fdData);
                    result.Add(avhData);
                }
                else if (travelParam.Count == 2)//往返或者联程
                {
                    // IbeService ibeBack = new IbeService();
                    QueryParam fromParam = travelParam[0];
                    AVHData fromParamAvhData = new AVHData();
                    FDData fromParamFdData = new FDData();
                    QueryParam toParam = travelParam[1];
                    AVHData toParamAvhData = new AVHData();
                    FDData toParamFdData = new FDData();
                    Parallel.Invoke(
                       () => fromParamAvhData = new IbeService(fromParam).GetAvh(fromParam.FromCode, fromParam.ToCode, DateTime.Parse(fromParam.FlyDate), fromParam.CarrierCode),
                       () => fromParamFdData = new IbeService(fromParam).GetFD(fromParam.FromCode, fromParam.ToCode, DateTime.Parse(fromParam.FlyDate), fromParam.CarrierCode)
                        //() => toParamAvhData = ibeBack.GetAvh(toParam.FromCode, toParam.ToCode, DateTime.Parse(toParam.FlyDate), toParam.CarrierCode),
                        //() => toParamFdData = ibeBack.GetFD(toParam.FromCode, toParam.ToCode, DateTime.Parse(toParam.FlyDate), toParam.CarrierCode)
                       );

                    Parallel.Invoke(
                      () => toParamAvhData = new IbeService(toParam).GetAvh(toParam.FromCode, toParam.ToCode, DateTime.Parse(toParam.FlyDate), toParam.CarrierCode),
                      () => toParamFdData = new IbeService(toParam).GetFD(toParam.FromCode, toParam.ToCode, DateTime.Parse(toParam.FlyDate), toParam.CarrierCode)
                    );
                    //补全数据
                    fromParamAvhData = BuQuanAVH(fromParamAvhData, fromParamFdData);
                    toParamAvhData = BuQuanAVH(toParamAvhData, toParamFdData);
                    result.Add(fromParamAvhData);
                    result.Add(toParamAvhData);
                }
            }
            catch (Exception ex)
            {
                //记录日志
                log.WriteLog("GetBasic", "异常信息:" + ex.Message + "\r\n");
            }
            return result;
        }
        /// <summary>
        /// 补全数据
        /// </summary>
        /// <param name="avhData"></param>
        /// <param name="fdData"></param>
        /// <returns></returns>
        public AVHData BuQuanAVH(AVHData avhData, FDData fdData)
        {
            try
            {
                if (avhData.IbeData.Count > 0 && fdData.FdRow.Count > 0)
                {
                    // TestData test = new TestData(); 
                    List<AirplainType> listAirplainType = new List<AirplainType>();
                    try
                    {
                        //mgHelper.All<AirplainType>().ToList();//
                        listAirplainType = test.GetAirplainType();
                    }
                    catch (Exception)
                    {
                        listAirplainType = new List<AirplainType>();
                    }
                    //所有的baseCabin
                    List<BaseCabin> allBaseCabin = test.GetBaseCabin("");

                    decimal TaxFee = 0m;//机建费
                    for (int i = 0; i < avhData.IbeData.Count; i++)
                    {
                        //空舱位
                        if (avhData.IbeData[i].IBESeat == null || avhData.DicYSeatPrice == null)
                        {
                            continue;
                        }
                        //Y舱价格
                        if (!avhData.DicYSeatPrice.ContainsKey(avhData.IbeData[i].CarrierCode))
                        {
                            if (fdData.YFdRow.ContainsKey(avhData.IbeData[i].CarrierCode))
                            {
                                avhData.DicYSeatPrice.Add(avhData.IbeData[i].CarrierCode, fdData.YFdRow[avhData.IbeData[i].CarrierCode].SeatPrice);
                            }
                        }
                        //查找机建费
                        AirplainType airplainType = listAirplainType.Find(p => p.Code == avhData.IbeData[i].AirModel);
                        if (airplainType != null)
                        {
                            TaxFee = airplainType.TaxFee;
                        }
                        else
                        {
                            try
                            {
                                AirplainType _airplainType = new AirplainType()
                                  {
                                      Code = avhData.IbeData[i].AirModel,
                                      TaxFee = 50m
                                  };
                                //补全机型机建
                                //mgHelper.Add<AirplainType>();
                                if (!test.ExistAirplainType(_airplainType.Code))
                                {
                                    test.ExecuteSQL(string.Format("insert into AirplainType(Code,TaxFee) values('{0}',{1})", _airplainType.Code, _airplainType.TaxFee));
                                }
                            }
                            catch (Exception)
                            {
                            }
                        }
                        for (int j = 0; j < avhData.IbeData[i].IBESeat.Count; j++)
                        {
                            FdRow fdRow = fdData.FdRow.Where(p => p.CarrierCode == avhData.IbeData[i].CarrierCode && p.Seat == avhData.IbeData[i].IBESeat[j].Seat).FirstOrDefault();
                            if (fdRow != null)
                            {
                                //燃油                            
                                avhData.IbeData[i].ADultFuleFee = fdRow.ADultFuleFee;
                                avhData.IbeData[i].ChildFuleFee = fdRow.ADultFuleFee;
                                //机建费 从数据库中读取 暂时假数据
                                if (TaxFee != 0)
                                {
                                    avhData.IbeData[i].TaxFee = TaxFee;
                                }
                                //舱位价
                                avhData.IbeData[i].IBESeat[j].SeatPrice = fdRow.SeatPrice;
                                //折扣
                                avhData.IbeData[i].IBESeat[j].Rebate = fdRow.Rebate;
                            }
                            else
                            {
                                try
                                {
                                    //从数据库中获取   
                                    //BaseCabin baseCabin = mgHelper.Query<BaseCabin>(p => p.CarrierCode == avhData.IbeData[i].CarrierCode && p.Code == avhData.IbeData[i].IBESeat[j].Seat).FirstOrDefault();
                                    //List<BaseCabin> baseCabinList = test.GetBaseCabin(string.Format(" CarrierCode='{0}' and Code='{1}' ", avhData.IbeData[i].CarrierCode, avhData.IbeData[i].IBESeat[j].Seat));
                                    List<BaseCabin> baseCabinList = allBaseCabin.Where(m => m.CarrierCode == avhData.IbeData[i].CarrierCode && m.Code == avhData.IbeData[i].IBESeat[j].Seat).ToList();
                                    BaseCabin baseCabin = null;
                                    if (baseCabinList != null && baseCabinList.Count > 0)
                                    {
                                        baseCabin = baseCabinList[0];
                                    }
                                    if (baseCabin != null && avhData.DicYSeatPrice.ContainsKey(avhData.IbeData[i].CarrierCode) && avhData.DicYSeatPrice[avhData.IbeData[i].CarrierCode] != 0m)
                                    {
                                        //折扣
                                        avhData.IbeData[i].IBESeat[j].Rebate = baseCabin.Rebate;
                                        //舱位价
                                        avhData.IbeData[i].IBESeat[j].SeatPrice = dataBill.MinusCeilTen((baseCabin.Rebate / 100) * avhData.DicYSeatPrice[avhData.IbeData[i].CarrierCode]);
                                    }
                                    fdRow = fdData.FdRow.Where(p => p.ADultFuleFee != 0).FirstOrDefault();
                                    //取这条航线的燃油 但不精确
                                    avhData.IbeData[i].ADultFuleFee = fdRow.ADultFuleFee;
                                    avhData.IbeData[i].ChildFuleFee = fdRow.ADultFuleFee;
                                    //机建费 从数据库中读取 暂时假数据
                                    avhData.IbeData[i].TaxFee = TaxFee;
                                }
                                catch (Exception)
                                {
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                //记录日志
                log.WriteLog("BuQuanAVH", "异常信息:" + ex.Message + "\r\n");
            }
            return avhData;
        }

        public bool LogPnr(string UserName, string IP, string Port, string Office, string Pnr)
        {
            bool IsSuccess = false;
            ThreadPool.QueueUserWorkItem(it =>
            {
                StringBuilder sbLog = new StringBuilder();
                sbLog.Append("参数:IP=" + IP + "\r\n Port=" + Port + " \r\n Office=" + Office + "\r\n Pnr=" + Pnr + "\r\n");
                PidService.PidServiceSoapClient Pid = new PidService.PidServiceSoapClient();
                try
                {
                    if (IP == "10.11.5.251")
                    {
                        IP = IP.Replace(IP, "mpb.51cbc.com");
                    }
                    IsSuccess = Pid.Flight_AddPNR(UserName, UserName, IP, Port, Pnr, Office, "查航班预订");
                }
                catch (Exception ex1)
                {
                    try
                    {
                        //在记录一次
                        IsSuccess = Pid.Flight_AddPNR(UserName, UserName, IP, Port, Pnr, Office, "查航班预订");
                    }
                    catch (Exception ex)
                    {
                        sbLog.Append("异常信息:" + ex.Message + "\r\n");
                        //记录日志
                        log.WriteLog("LogPnr", sbLog.ToString());
                    }
                }
            });
            return IsSuccess;
        }

        public CabinData GetBaseCabinUsePolicy(string CarrayCode)
        {
            string sqlWhere = string.Empty;
            CabinData cabinData = new CabinData();
            if (!string.IsNullOrEmpty(CarrayCode))
            {
                sqlWhere = string.Format("CarrierCode='{0}'", CarrayCode);
            }
            List<BaseCabin> BaseCabinList = test.GetBaseCabin(sqlWhere);
            BaseCabinList.ForEach(p =>
            {
                if (!cabinData.CabinList.Exists(p1 => p1.Seat.ToUpper() == p.Code.ToUpper()))
                {
                    cabinData.CabinList.Add(new CabinRow()
                    {
                        CarrayCode = p.CarrierCode,
                        Seat = p.Code,
                        Rebate = p.Rebate
                    });
                }
            });
            List<CabinRow> CabinSeatList = test.GetCabinSeatListPolicy(sqlWhere);
            CabinSeatList.ForEach(p =>
            {
                if (!cabinData.CabinList.Exists(p1 => p1.Seat.ToUpper() == p.Seat.ToUpper() && p1.CarrayCode.ToUpper() == p.CarrayCode.ToUpper()))
                {
                    cabinData.CabinList.Add(p);
                }
            });
            return cabinData;
        }

        private Policy PolicyCacheToPolicy(PolicyCache pc, EnumPolicySourceType PolicySourceType)
        {
            Policy p = new Policy();
            p.PolicyId = pc.PolicyId;
            p.CarryCode = pc.CarrierCode;
            p.PolicySourceType = PolicySourceType;
            p.Code = pc.PlatformCode;
            p.Name = pc.PlatformCode;
            p.PlatformCode = pc.PlatformCode;
            p.OriginalPolicyPoint = pc.Point;
            p.PaidPoint = pc.Point;
            p.DownPoint = 0m;
            p.PolicyPoint = pc.Point;
            p.DeductionDetails = p.DeductionDetails == null ? new List<DeductionDetail>() : p.DeductionDetails;
            return p;
        }


    }
}
