using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using PnrAnalysis;
using JoveZhao.Framework;
using PnrAnalysis.Model;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.DomesticTicket.Domain.Models;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using StructureMap;
using BPiaoBao.Common;
using PBPid.WebManage;
using BPiaoBao.Common.Enums;
namespace BPiaoBao.DomesticTicket.Domain.Services
{
    public class FlightService
    {
        QueryFlightService queryFlightService = new QueryFlightService();
        PnrResource format = new PnrResource();
        FormatPNR pnrformat = new FormatPNR();
        CommLog log = new CommLog();
        //配置文件中读取的IP端口 office号
        string strServerIP = string.Empty;
        string strServerPort = string.Empty;
        string strOffice = string.Empty;
        bool IsTripTest = false;
        IBusinessmanRepository businessmanRepository;
        PolicyService policyService = null;
        //登陆商户
        CurrentUserInfo currentUser;
        //Carrier carrier = null;
        dynamic businessman = null;
        public FlightService(IBusinessmanRepository businessmanRepository, CurrentUserInfo currentUser)
        {
            this.businessmanRepository = businessmanRepository;
            this.currentUser = currentUser;
            strServerIP = System.Configuration.ConfigurationManager.AppSettings["ServerIP"];
            strServerPort = System.Configuration.ConfigurationManager.AppSettings["ServerPort"];
            strOffice = System.Configuration.ConfigurationManager.AppSettings["Office"];
            bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["IsTripTest"], out IsTripTest);
            if (currentUser != null)
            {
                Buyer buyer = this.businessmanRepository.FindAll(p => p.Code == currentUser.Code && (p is Buyer)).FirstOrDefault() as Buyer;
                if (buyer != null)
                {
                    businessman = this.businessmanRepository.FindAll(p => p.Code == buyer.CarrierCode).FirstOrDefault();
                }
            }
            policyService = ObjectFactory.GetInstance<PolicyService>();
        }
        public FlightResponse QueryOnewayFlight(string code, string formCityCode, string toCityCode, DateTime takeDate, bool IsShare = false, string carrayCode = null)
        {
            FlightResponse flightResponse = null;
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            StringBuilder DataLog = new StringBuilder();
            sbLog.Append("参数:code=" + code + " formCityCode=" + formCityCode + " \r\n  toCityCode=" + toCityCode + "\r\n takeDate=" + takeDate.ToString("yyyy-MM-dd HH:mm:ss") + " IsShare=" + IsShare + " \r\n carrayCode=" + (carrayCode == null ? "" : carrayCode));
            try
            {
                List<QueryParam> QueryParamList = new List<QueryParam>();
                //查询航班条件
                QueryParamList.Add(new QueryParam()
                {
                    FromCode = formCityCode,
                    ToCode = toCityCode,
                    FlyDate = takeDate.ToString("yyyy-MM-dd"),
                    CarrierCode = carrayCode,
                    IsShare = IsShare,
                    Code = code
                });
                //查询政策条件参数
                PolicyQueryParam policyQueryParam = new PolicyQueryParam();
                policyQueryParam.InputParam = QueryParamList;
                policyQueryParam.TravelType = TravelType.Oneway;
                //获取航班数据
                List<AVHData> avhList = new List<AVHData>();
                //获取政策
                List<PolicyCache> interfacePolicyList = new List<PolicyCache>();
                List<PolicyCache> localPolicyList = new List<PolicyCache>();
                Parallel.Invoke(
                    () => avhList = queryFlightService.GetBasic(QueryParamList),
                    () => interfacePolicyList = queryFlightService.GetPolicy(policyQueryParam),
                    () => localPolicyList = policyService.GetFlightPolicy(code, policyQueryParam)
                );
                List<PolicyCache> policyList = new List<PolicyCache>();
                policyList.AddRange(interfacePolicyList.ToArray());
                policyList.AddRange(localPolicyList.ToArray());
                //匹配政策
                avhList = queryFlightService.MatchPolicy(code, avhList, TravelType.Oneway, policyList);
                //avhList->FlightResponse 转换即可
                if (avhList != null && avhList.Count > 0)
                {
                    AVHData avhData = avhList[0];
                    flightResponse = AVHDataToFlightResponse(avhData);
                    DataLog.Append("单程:\r\n" + sbLog.ToString() + "\r\n返回数据条数:" + avhData.IbeData.Count.ToString());
                }
                else
                {
                    //抛出异常
                    throw new CustomException(111, "您查询的航班已售完！");
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message);
                //记录日志
                WriteLog("QueryOnewayFlight", sbLog.ToString());
                throw new CustomException(111, ex.Message);
            }
            finally
            {
                if (DataLog.ToString() != "")
                {
                    WriteLog("QueryOnewayFlight_1", DataLog.ToString());
                }
            }
            return flightResponse;
        }

        public FlightResponse[] QueryTwowayFlight(string code, string formCityCode, string toCityCode, DateTime takeDate, DateTime backDate, bool IsShare = false, string carrayCode = null)
        {
            FlightResponse[] flightResponse = null;
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            StringBuilder DataLog = new StringBuilder();
            sbLog.Append("参数:code=" + code + " formCityCode=" + formCityCode + " \r\n  toCityCode=" + toCityCode + "\r\n takeDate=" + takeDate.ToString("yyyy-MM-dd HH:mm:ss") + " \r\n backDate=" + backDate.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n carrayCode=" + (carrayCode == null ? "" : carrayCode));
            try
            {
                List<QueryParam> QueryParamList = new List<QueryParam>();
                //查询航班条件 去程
                QueryParamList.Add(new QueryParam()
                {
                    FromCode = formCityCode,
                    ToCode = toCityCode,
                    FlyDate = takeDate.ToString("yyyy-MM-dd"),
                    CarrierCode = carrayCode,
                    IsShare = IsShare,
                    Code = code
                });
                // 回程
                QueryParamList.Add(new QueryParam()
                {
                    FromCode = toCityCode,
                    ToCode = formCityCode,
                    FlyDate = backDate.ToString("yyyy-MM-dd"),
                    CarrierCode = carrayCode,
                    IsShare = IsShare,
                    Code = code
                });
                //查询政策条件参数
                PolicyQueryParam policyQueryParam = new PolicyQueryParam();
                policyQueryParam.InputParam = QueryParamList;
                policyQueryParam.TravelType = TravelType.Twoway;
                flightResponse = QueryFlight(code, flightResponse, QueryParamList, policyQueryParam);

                DataLog.Append("往返:\r\n" + sbLog.ToString() + "\r\n返回数据条数:" + flightResponse[0].List.Count().ToString());
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message);
                //记录日志
                WriteLog("QueryTwowayFlight", sbLog.ToString());
                throw new CustomException(111, ex.Message);
            }
            finally
            {
                if (DataLog.ToString() != "")
                {
                    WriteLog("QueryTwowayFlight_1", DataLog.ToString());
                }
            }
            return flightResponse;
        }

        public FlightResponse[] QueryConnwayFlight(string code, string formCityCode, string midCityCode, string toCityCode, DateTime takeDate, DateTime midDate, bool IsShare = false, string carrayCode = null)
        {
            FlightResponse[] flightResponse = null;
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            StringBuilder DataLog = new StringBuilder();
            sbLog.Append("参数:code=" + code + " formCityCode=" + formCityCode + " \r\n midCityCode=" + midCityCode + "\r\n  toCityCode=" + toCityCode + "\r\n takeDate=" + takeDate.ToString("yyyy-MM-dd HH:mm:ss") + " \r\n midDate=" + midDate.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n carrayCode=" + (carrayCode == null ? "" : carrayCode));
            try
            {
                List<QueryParam> QueryParamList = new List<QueryParam>();
                //查询航班条件 去程
                QueryParamList.Add(new QueryParam()
                {
                    FromCode = formCityCode,
                    ToCode = midCityCode,
                    FlyDate = takeDate.ToString("yyyy-MM-dd"),
                    CarrierCode = carrayCode,
                    IsShare = IsShare,
                    Code = code
                });
                // 回程
                QueryParamList.Add(new QueryParam()
                {
                    FromCode = midCityCode,
                    ToCode = toCityCode,
                    FlyDate = midDate.ToString("yyyy-MM-dd"),
                    CarrierCode = carrayCode,
                    IsShare = IsShare,
                    Code = code
                });
                //查询政策条件参数
                PolicyQueryParam policyQueryParam = new PolicyQueryParam();
                policyQueryParam.InputParam = QueryParamList;
                policyQueryParam.TravelType = TravelType.Connway;
                flightResponse = QueryFlight(code, flightResponse, QueryParamList, policyQueryParam);
                DataLog.Append("联程:\r\n" + sbLog.ToString() + "\r\n返回数据条数:" + flightResponse[0].List.Count().ToString());
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message);
                //记录日志
                WriteLog("QueryConnwayFlight", sbLog.ToString());
                throw new CustomException(111, ex.Message);
            }
            finally
            {
                if (DataLog.ToString() != "")
                {
                    WriteLog("QueryConnwayFlight_1", DataLog.ToString());
                }
            }
            return flightResponse;
        }

        public DestineResponse Destine(string code, DestineRequest destine)
        {
            string result = string.Empty;
            string ErrorResult = string.Empty;
            DestineResponse response = new DestineResponse();
            //记录日志
            StringBuilder sbLog = new StringBuilder();
            StringBuilder DataLog = new StringBuilder();
            sbLog.Append("预订参数:code=" + code + " " + destine.ToString());
            try
            {
                string CarrayCode = string.Empty;
                if (destine.SkyWay.Count() > 0)
                {
                    CarrayCode = destine.SkyWay[0].CarrayCode;
                }
                if (businessman == null)
                {
                    ErrorResult = "未找到运营商信息，请联系客服！";
                    throw new CustomException(111, ErrorResult);
                }
                if (businessman.Pids == null || businessman.Pids.Count == 0)
                {
                    ErrorResult = "配置信息未设置，请联系运营商设置！";
                    throw new CustomException(111, ErrorResult);
                }
                else
                {
                    PID pid = (businessman is Carrier) ? (businessman as Carrier).Pids.FirstOrDefault() : (businessman as Supplier).SupPids.FirstOrDefault();
                    if (pid != null)
                    {
                        strServerIP = pid.IP;
                        strServerPort = pid.Port.ToString();
                        strOffice = pid.Office;
                    }
                    if (string.IsNullOrEmpty(strServerIP)
                        || string.IsNullOrEmpty(strServerPort)
                         || string.IsNullOrEmpty(strOffice)
                        )
                    {
                        ErrorResult = "默认配置信息未设置，请联系运营商设置！";
                        throw new CustomException(111, ErrorResult);
                    }
                }
                if (businessman != null && !string.IsNullOrEmpty(CarrayCode))
                {
                    CarrierSetting carrierSetting = (businessman is Carrier) ? (businessman as Carrier).CarrierSettings.Where(p => p.CarrayCode.ToUpper() == CarrayCode.ToUpper()).FirstOrDefault()
                    : (businessman as Supplier).CarrierSettings.Where(p => p.CarrayCode.ToUpper() == CarrayCode.ToUpper()).FirstOrDefault();
                    if (carrierSetting != null)
                    {
                        if (!string.IsNullOrEmpty(carrierSetting.YDOffice))
                        {
                            strOffice = carrierSetting.YDOffice;
                            PID PidInfo = (businessman is Carrier) ? (businessman as Carrier).Pids.Where(p => p.Office.ToUpper() == strOffice.ToUpper()).FirstOrDefault() :
                                 (businessman as Supplier).SupPids.Where(p => p.Office.ToUpper() == strOffice.ToUpper()).FirstOrDefault();
                            if (PidInfo != null)
                            {
                                strServerIP = PidInfo.IP;
                                strServerPort = PidInfo.Port.ToString();
                            }
                        }
                    }
                }
                //strServerIP = "210.14.138.29";
                //strServerPort = "2232";
                //strOffice = "CTU186";
                //从配置文件中读取数据
                sbLog.Append(" ServerIP=" + strServerIP + "\r\n ServerPort=" + strServerPort + "\r\n Office=" + strOffice + "\r\n");
                PnrParamObj PnrParam = new PnrParamObj();
                ParamObject poct = new ParamObject();
                //必填项 是否开启新版PID发送指令 
                PnrParam.UsePIDChannel = 2; //2;
                double _FlyAdvanceTime = 1;
                double.TryParse(System.Configuration.ConfigurationManager.AppSettings["FlyAdvanceTime"].ToString(), out _FlyAdvanceTime);
                PnrParam.FlyAdvanceTime = _FlyAdvanceTime;//SettingSection.GetInstances().Cashbag.FlyAdvanceTime; //飞机起飞前几个小时的预定
                //连接PID的IP地址
                PnrParam.ServerIP = strServerIP;
                poct.ServerIP = strServerIP;
                //连接PID的IP端口
                int ServerPort = 392;
                int.TryParse(strServerPort, out ServerPort);
                PnrParam.ServerPort = ServerPort;
                poct.ServerPort = ServerPort;
                //配置号
                PnrParam.Office = strOffice;
                poct.Office = strOffice;
                //用户
                string strUserName = string.Format("{0}#{1}", code, (businessman != null ? ((businessman is Carrier) ? (businessman as Carrier).Code : (businessman as Supplier).Code) : "null"));
                //可选项                
                PnrParam.UserName = strUserName;//用户标识
                //只是儿童时需要备注的成人编码 
                PnrParam.AdultPnr = destine.ChdRemarkAdultPnr;
                //PnrParam.CTTel = "";//公司电话
                PnrParam.CTCTPhone = string.IsNullOrEmpty(destine.Tel) ? string.Empty : destine.Tel.Trim();//CTCT联系人手机
                //为空即失效
                PnrParam.EUChdValidDate = System.DateTime.Now.AddDays(-1).ToString("yyyy-MM-dd HH:mm:ss");

                //初始化数据
                List<IPassenger> pasList = new List<IPassenger>();
                List<ISkyLeg> skyList = new List<ISkyLeg>();
                PnrParam.PasList = pasList;
                PnrParam.SkyList = skyList;
                //有婴儿
                bool IsExistINF = false;
                //是否为 固定特价
                PnrParam.IsGdNotPAT = (destine.PolicySpecialType != EnumPolicySpecialType.Normal && destine.PolicySpecialType != EnumPolicySpecialType.DynamicSpecial) ? 1 : 0;
                //乘机人
                foreach (PassengerRequest pas in destine.Passengers)
                {
                    IPassenger p1 = new IPassenger();
                    pasList.Add(p1);
                    p1.PassengerName = pas.PassengerName.Trim();
                    p1.PassengerType = (int)pas.PassengerType;
                    p1.PasSsrCardID = string.IsNullOrEmpty(pas.CardNo) ? string.Empty : pas.CardNo.Trim();
                    p1.ChdBirthday = pas.ChdBirthday.ToString("yyyy-MM-dd");
                    p1.CpyandNo = pas.MemberCard;
                    p1.LinkPhone = string.IsNullOrEmpty(pas.LinkPhone) ? string.Empty : pas.LinkPhone.Trim();
                    if (!IsExistINF && pas.PassengerType == 3)
                    {
                        IsExistINF = true;
                    }
                }
                List<string> seatGroupList = new List<string>();
                //航段
                foreach (DestineSkyWayRequest skyway in destine.SkyWay)
                {
                    ISkyLeg leg1 = new ISkyLeg();
                    skyList.Add(leg1);
                    leg1.CarryCode = skyway.CarrayCode;
                    leg1.FlightCode = skyway.FlightNumber;
                    leg1.FlyStartTime = skyway.StartDate.ToString("HHmm");
                    leg1.FlyStartDate = skyway.StartDate.ToString("yyyy-MM-dd");
                    leg1.fromCode = skyway.FromCityCode;
                    leg1.toCode = skyway.ToCityCode;
                    leg1.Space = skyway.Seat;
                    PnrParam.CarryCode = skyway.CarrayCode;
                    seatGroupList.Add(skyway.Seat);
                }
                //预订编码
                SendNewPID sendPid = new SendNewPID();
                sbLog.AppendFormat("时间1:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                RePnrObj pObj = sendPid.ISendIns(PnrParam);
                sbLog.AppendFormat("时间2:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (PnrParam.FlyTimeIsOverCurrTime)
                    ErrorResult = "航班起飞前" + PnrParam.FlyAdvanceTime + "小时内不能预定机票！";
                if (string.IsNullOrEmpty(ErrorResult))
                {
                    #region 成人
                    //以下为验证              
                    //成人编码预定信息
                    if (pObj.AdultYudingList.Count > 0)
                    {
                        string AdultPnr = string.Empty;
                        string AdultYDResult = string.Empty;
                        string AdultRTResult = string.Empty;
                        string AdultPATResult = string.Empty;
                        string INFPATResult = string.Empty;

                        AdultPnr = pObj.AdultPnr;
                        AdultYDResult = pObj.AdultYudingList.Values[0];
                        AdultRTResult = pObj.AdultPnrRTContent;
                        if (PnrParam.IsGdNotPAT != 1)
                            AdultPATResult = pObj.PatList[0];

                        response.INFPnrIsSame = pObj.YdInfIsSame;
                        sbLog.Append("结果:\r\n 成人Pnr=" + AdultPnr + " PNRContent=" + AdultRTResult + " \r\n PATContent=" + AdultPATResult + "\r\n");
                        #region 检查验证
                        //预定编码失败或者取到的非编码
                        if (!pnrformat.IsPnr(AdultPnr))
                        {
                            //提示pnr失败信息                                                                   
                            ErrorResult = ShowPnrFailInfo(1, AdultYDResult);
                            AdultPnr = "";
                        }
                        else
                        {
                            //记录PNR
                            queryFlightService.LogPnr(strUserName, strServerIP, strServerPort, strOffice, AdultPnr);

                            #region 检查并重发指令
                            string msg = string.Empty;
                            if (AdultRTResult.Trim().Replace("\r", "").Replace("\n", "") == "S")
                            {
                                poct.code = "RT" + AdultPnr;
                                poct.IsPn = true;
                                AdultRTResult = SendNewPID.SendCommand(poct);
                                pObj.PnrList[0] = pnrformat.GetPNRInfo(AdultPnr, AdultRTResult, false, out msg);
                            }
                            if (PnrParam.IsGdNotPAT != 1 && AdultPATResult.Trim().Replace("\r", "").Replace("\n", "") == "S")
                            {
                                poct.code = "RT" + AdultPnr + "|PAT:A";
                                AdultPATResult = SendNewPID.SendCommand(poct);
                                pObj.PatModelList[0] = pnrformat.GetPATInfo(AdultPATResult, out msg);
                            }

                            if (pObj.PnrList[0] == null || pObj.PnrList[0]._PassengerList.Count == 0)
                            {
                                ErrorResult = "成人编码" + AdultPnr + " 没有提取到编码信息！错误信息:" + AdultRTResult;
                            }
                            else
                            {
                                if (PnrParam.IsGdNotPAT != 1 && (pObj.PatModelList[0] == null || pObj.PatModelList[0].PatList.Count == 0))
                                {
                                    ErrorResult = "成人编码" + AdultPnr + " 没有P到价格信息！错误信息:" + AdultPATResult;
                                }
                            }
                            if (pObj.PnrList.Length > 0 && pObj.PnrList[0].HasINF)
                            {
                                INFPATResult = pObj.PatList[2];
                                if (INFPATResult.Trim().Replace("\r", "").Replace("\n", "") == "S" || string.IsNullOrEmpty(INFPATResult.Trim()))
                                {
                                    poct.code = "RT" + AdultPnr + "|PAT:A*IN";
                                    INFPATResult = SendNewPID.SendCommand(poct);
                                    pObj.PatModelList[2] = pnrformat.GetPATInfo(INFPATResult, out msg);
                                }
                            }
                            #endregion



                            if (string.IsNullOrEmpty(ErrorResult))
                            {
                                //成人编码预定成功
                                if (AdultRTResult.Contains("不支持的汉字"))
                                {
                                    ErrorResult = "成人编码" + AdultPnr + "姓名中存在航信不支持的汉字，请仔细检查！";
                                }
                                else
                                {
                                    //检查有备注婴儿失败的情况
                                    string strTempMsg = string.Empty;
                                    //婴儿是否丢失
                                    //bool INFLose = IsExistINF && !pObj.PnrList[0].HasINF;
                                    //if (INFLose)
                                    //{
                                    //    ErrorResult = "成人编码" + AdultPnr + "备注婴儿失败,请手动补全婴儿！";
                                    //}
                                    //else
                                    //{
                                    if (CheckINFRemark(pObj, out strTempMsg))
                                    {
                                        ErrorResult = "成人编码" + AdultPnr + "备注婴儿失败,失败原因:" + strTempMsg;
                                    }
                                    else
                                    {
                                        //检查编码状态
                                        string PnrStatus = string.Empty;
                                        if (!CheckPnrStatus(pObj, 1, out PnrStatus))
                                        {
                                            ErrorResult = "成人编码" + AdultPnr + " 解析状态为" + PnrStatus + "，不能生成订单,请用PNR内容导入！";
                                        }
                                        else
                                        {
                                            //婴儿没有P到价格时
                                            if (IsExistINF && (pObj.PatModelList[2] == null || pObj.PatModelList[2].PatList.Count == 0))
                                            {
                                                if (PnrParam.IsGdNotPAT != 1)
                                                {
                                                    decimal InfSpecialSeatFare = destine.SkyWay.Sum(p => p.SpecialYPrice / 10);
                                                    //特价的婴儿 没有PAT出价格时
                                                    PatInfo pat = new PatInfo();
                                                    pat.SerialNum = "1";
                                                    pat.Fare = InfSpecialSeatFare.ToString();
                                                    pat.TAX = "0";
                                                    pat.RQFare = "0";
                                                    pat.Price = InfSpecialSeatFare.ToString();
                                                    pat.SeatGroup = string.Join("/", seatGroupList.ToArray());
                                                    AdultPATResult = pat.ToString();
                                                    string err = string.Empty;
                                                    pObj.PatModelList[2] = pnrformat.GetPATInfo(AdultPATResult, out err);
                                                }
                                                else
                                                {
                                                    ErrorResult = "成人编码" + AdultPnr + "未能P到婴儿价格，原因如下:" + pObj.PatList[2];
                                                }
                                            }
                                            else
                                            {
                                                //OK
                                            }
                                        }
                                    }
                                    //}
                                }
                            }
                            response.AdultPnr = AdultPnr;
                            if (!pnrformat.IsExistBigCode(AdultRTResult) && pnrformat.IsExistBigCode(pObj.AdultRTRContent))
                            {
                                AdultRTResult = pObj.AdultRTRContent;
                            }
                            //去重复和备注
                            //string pnrRemark = string.Empty;
                            //AdultRTResult = pnrformat.DelRepeatRTCon(AdultRTResult, ref pnrRemark);
                            //if (!string.IsNullOrEmpty(pnrRemark))
                            //{
                            //    AdultPATResult = AdultPATResult.Replace(pnrRemark, "");
                            //}
                            if (PnrParam.IsGdNotPAT == 1)
                            {
                                decimal SpecialSeatFare = destine.SkyWay.Sum(p => p.SpecialPrice);
                                decimal SpecialTaxFare = destine.SkyWay.Sum(p => p.SpecialTax);
                                decimal SpecialRQFare = destine.SkyWay.Sum(p => p.SpecialFuelFee);
                                if (destine.SkyWay.Count() > 1)
                                {
                                    destine.IbeSeatPrice = SpecialSeatFare;
                                    destine.IbeTaxFee = SpecialTaxFare;
                                    destine.IbeRQFee = SpecialRQFare;
                                }
                                if (destine.PolicySpecialType == EnumPolicySpecialType.FixedSpecial)
                                {
                                    SpecialSeatFare = destine.SpecialPrice;
                                    //SpecialTaxFare = destine.SpecialTax;
                                    //SpecialRQFare = destine.SpecialFuelFee;

                                    destine.IbeSeatPrice = SpecialSeatFare;
                                    //destine.IbeTaxFee = SpecialTaxFare;
                                    //destine.IbeRQFee = SpecialRQFare;
                                }
                                //为特价时
                                PatInfo pat = new PatInfo();
                                pat.SerialNum = "1";
                                pat.Fare = SpecialSeatFare.ToString();
                                pat.TAX = SpecialTaxFare.ToString();
                                pat.RQFare = SpecialRQFare.ToString();
                                pat.Price = (SpecialSeatFare + SpecialTaxFare + SpecialRQFare).ToString();
                                pat.SeatGroup = string.Join("/", seatGroupList.ToArray());
                                AdultPATResult = pat.ToString();
                                string err = string.Empty;
                                pObj.PatModelList[0] = pnrformat.GetPATInfo(AdultPATResult, out err);
                            }
                            response.AdultPnrContent = AdultRTResult + "\r\n" + AdultPATResult + "\r\n" + INFPATResult;
                            response.AdultPnrData.PnrMode = pObj.PnrList[0];
                            response.AdultPnrData.PatMode = pObj.PatModelList[0];
                            response.AdultPnrData.InfPatMode = pObj.PatModelList[2];
                            response.AdultPnrData.AdultPnr = AdultPnr;
                        }
                        #endregion
                    }
                    #endregion

                    sbLog.AppendFormat("时间3:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                    #region 儿童
                    if (string.IsNullOrEmpty(ErrorResult))
                    {
                        string ChdPnr = string.Empty;
                        string ChdYDResult = string.Empty;
                        string ChdRTResult = string.Empty;
                        string ChdPATResult = string.Empty;
                        if (pObj.ChildYudingList.Count > 0)
                        {
                            ChdPnr = pObj.childPnr;
                            ChdYDResult = pObj.ChildYudingList.Values[0];
                            ChdRTResult = pObj.childPnrRTContent;
                            ChdPATResult = pObj.PatList[1];
                            sbLog.Append("结果:\r\n 儿童Pnr=" + ChdPnr + " PNRContent=" + ChdRTResult + " \r\n PATContent=" + ChdPATResult + "\r\n");
                            if (!pnrformat.IsPnr(ChdPnr))
                            {
                                //提示pnr失败信息                                                                   
                                ErrorResult = ShowPnrFailInfo(2, ChdYDResult);
                                ChdPnr = "";
                            }
                            else
                            {
                                //记录编码
                                queryFlightService.LogPnr(strUserName, strServerIP, strServerPort, strOffice, ChdPnr);
                                #region 检查并重发指令
                                string msg = string.Empty;
                                if (ChdRTResult.Trim().Replace("\r", "").Replace("\n", "") == "S")
                                {
                                    poct.code = "RT" + ChdPnr;
                                    poct.IsPn = true;
                                    ChdRTResult = SendNewPID.SendCommand(poct);
                                    pObj.PnrList[1] = pnrformat.GetPNRInfo(ChdPnr, ChdRTResult, false, out msg);
                                }
                                if (PnrParam.IsGdNotPAT == 1 || string.IsNullOrEmpty(ChdPATResult) || ChdPATResult.Trim().Replace("\r", "").Replace("\n", "") == "S")
                                {
                                    poct.code = "RT" + ChdPnr + "|PAT:A*CH";
                                    ChdPATResult = SendNewPID.SendCommand(poct);
                                    pObj.PatModelList[1] = pnrformat.GetPATInfo(ChdPATResult, out msg);
                                }

                                if (pObj.PnrList[1] == null || pObj.PnrList[1]._PassengerList.Count == 0)
                                {
                                    ErrorResult = "儿童编码" + ChdPnr + " 没有提取到编码信息！错误信息:" + ChdRTResult;
                                }
                                else
                                {
                                    if (pObj.PatModelList[1] == null || pObj.PatModelList[1].PatList.Count == 0)
                                    {
                                        if (PnrParam.IsGdNotPAT == 1)
                                        {
                                            //为特价时
                                            PatInfo pat = new PatInfo();
                                            pat.SerialNum = "1";
                                            pat.Fare = (destine.SpecialYPrice / 2).ToString();
                                            pat.TAX = destine.SpecialTax.ToString();
                                            pat.RQFare = destine.SpecialFuelFee.ToString();
                                            pat.Price = ((destine.SpecialYPrice / 2) + destine.SpecialTax + destine.SpecialFuelFee).ToString();
                                            pat.SeatGroup = string.Join("/", seatGroupList.ToArray());
                                            ChdPATResult = pat.ToString();
                                            string err = string.Empty;
                                            pObj.PatModelList[1] = pnrformat.GetPATInfo(ChdPATResult, out err);
                                        }
                                        else
                                        {
                                            ErrorResult = "儿童编码" + ChdPnr + " 没有提取到PAT价格信息！错误信息:" + ChdPATResult;
                                        }
                                    }
                                }
                                #endregion

                                if (string.IsNullOrEmpty(ErrorResult))
                                {
                                    //儿童编码预定成功
                                    if (ChdRTResult.Contains("不支持的汉字"))
                                    {
                                        ErrorResult = "儿童编码" + ChdPnr + "姓名中存在航信不支持的汉字，请仔细检查！";
                                    }
                                    else
                                    {
                                        //检查编码状态
                                        string PnrStatus = string.Empty;
                                        if (!CheckPnrStatus(pObj, 2, out PnrStatus))
                                        {
                                            ErrorResult = "儿童编码" + ChdPnr + " 解析状态为" + PnrStatus + "，不能生成订单,请用PNR内容导入！";
                                        }
                                    }
                                }
                            }

                            response.ChdPnr = ChdPnr;

                            if (!pnrformat.IsExistBigCode(ChdRTResult) && pnrformat.IsExistBigCode(pObj.CHDRTRContent))
                            {
                                ChdRTResult = pObj.CHDRTRContent;
                            }
                            //去重复和备注
                            //string pnrRemark = string.Empty;
                            //ChdRTResult = pnrformat.DelRepeatRTCon(ChdRTResult, ref pnrRemark);
                            //if (!string.IsNullOrEmpty(pnrRemark))
                            //{
                            //    ChdPATResult = ChdPATResult.Replace(pnrRemark, "");
                            //}
                            response.ChdPnrContent = ChdRTResult + "\r\n" + ChdPATResult;
                            response.ChdPnrData.PnrMode = pObj.PnrList[1];
                            response.ChdPnrData.PatMode = pObj.PatModelList[1];
                            response.ChdPnrData.ChdPnr = ChdPnr;
                        }
                    }//儿童
                    #endregion
                }
                //抛出失败信息
                if (!string.IsNullOrEmpty(ErrorResult))
                {
                    throw new CustomException(111, ErrorResult);
                }
                DataLog.Append("预订编码:\r\n" + sbLog.ToString() + "\r\n");
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message);
                //记录日志
                WriteLog("Destine", sbLog.ToString());
                //抛出异常                
                throw new CustomException(111, ex.Message);
            }
            finally
            {
                if (DataLog.ToString() != "")
                {
                    WriteLog("Destine_1", DataLog.ToString());
                }
                sbLog.AppendFormat("时间4:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            return response;
        }

        /// <summary>
        /// 提示编码生成失败原因
        /// </summary>
        /// <param name="type"></param>
        /// <param name="yudingRecvData"></param>
        /// <returns></returns>
        public string ShowPnrFailInfo(int type, string yudingRecvData)
        {
            string msg = "";
            if (yudingRecvData.ToUpper().Contains("PLS NM1XXXX/XXXXXX"))
            {
                //PLS NM1XXXX/XXXXXX
                msg = "乘客输入姓名格式错误！原因如下:（" + yudingRecvData.ToUpper() + "）姓名中应加斜线(/),或斜线数量不正确!";
            }
            else if (yudingRecvData.ToUpper().Contains("DUPLICATE TEL NUMBER"))
            {
                msg = "输入乘客手机号不能重复!原因如下:" + yudingRecvData;
            }
            else if (yudingRecvData.ToUpper().Contains("UNABLE TO SELL.PLEASE")
                || yudingRecvData.ToUpper().Contains("SEATS"))
            {
                msg = "座位数不足或座位已销售完,请重新预订!";
            }
            else if (yudingRecvData.Contains("不支持的汉字"))
            {
                msg = "乘客(" + (type == 1 ? "成人" : "儿童") + ")姓名中存在航信不支持的汉字，请仔细检查！";
            }
            else if (yudingRecvData.Contains("INVALID FOID"))
            {
                msg = (type == 1 ? "成人" : "儿童") + "证件号中含有航信不识别的证件号码，请仔细检查！【" + yudingRecvData + "】";
            }
            else if (yudingRecvData.IndexOf("地址无效") != -1
                || yudingRecvData.ToUpper().Contains("CHECK CONNECTION")
                || yudingRecvData.IndexOf("无法从传输连接中读取数据") != -1
                || yudingRecvData.IndexOf("无法连接") != -1
                || yudingRecvData.IndexOf("强迫关闭") != -1
                || yudingRecvData.IndexOf("由于") != -1)
            {
                msg = "与航信连接失败，请重新预订！错误信息:" + yudingRecvData;
            }
            else if (yudingRecvData.IndexOf("超时！") != -1
                || yudingRecvData.IndexOf("服务器忙") != -1)
            {
                msg = "系统繁忙,请稍后再试！";
            }
            else if (yudingRecvData.ToUpper().Contains("WSACancelBlockingCall")
                || yudingRecvData.ToUpper().Contains("TRANSACTION IN PROGRESS")
                || yudingRecvData.ToUpper().Contains(" FORMAT ")
                || yudingRecvData.ToUpper().Contains("NO PNR")
                || yudingRecvData.ToUpper().Contains("CHECK TKT DATE")
                || yudingRecvData.ToUpper().Contains("为空")
                || yudingRecvData.ToUpper().Contains("对象的实例"))
            {
                msg = (type == 1 ? "成人" : "儿童") + "编码生成失败！原因如下:" + yudingRecvData;
            }
            else if (yudingRecvData.Trim().ToUpper() == "S")
            {
                msg = "生成编码失败，信息:" + yudingRecvData;
            }
            else if (yudingRecvData.Trim().ToUpper().Contains("FORMAT"))
            {
                msg = "请检查输入乘客是否规范，失败原因:" + yudingRecvData;
            }
            else
            {
                string strYuding = yudingRecvData;
                if (strYuding.Contains("通讯断开"))
                {
                    strYuding = "发送指令失败，通讯断开请重试！";
                }
                msg = (type == 1 ? "成人" : "儿童") + "编码生成失败,原因如下:" + strYuding + (
                    strYuding.ToUpper().Contains("FFP TOP TIER CODE INPUT ERROR")
                    || strYuding.ToUpper().Contains("INVLID PROFILE NUMBER")
                    || strYuding.ToUpper().Contains("INVALID CHARACTER") ? " 输入航空公司卡号错误!" : "");
            }
            return msg;
        }



        /// <summary>
        /// 检查婴儿名字是否有生僻字  备注指令
        /// </summary>
        /// <returns></returns>
        public bool CheckINFRemark(RePnrObj PnrInfo, out string recvData)
        {
            bool IsSpz = false;
            recvData = "";
            List<RemarkInfo> rmkList = PnrInfo.RemarkList;
            if (rmkList.Count > 0)
            {
                foreach (RemarkInfo item in rmkList)
                {
                    if (item.PassengerType == "3")
                    {
                        if (item.RemarkRecvData.Contains("存在不支持的汉字"))
                        {
                            IsSpz = true;
                            recvData = item.RemarkRecvData;
                            break;
                        }
                    }
                }
            }
            return IsSpz;
        }
        /// <summary>
        /// 检查预订出来的编码  状态是否正常
        /// </summary>
        /// <param name="PnrInfo">编码所有信息实体</param>
        /// <param name="PasType">编码类型 1成人 2儿童</param>
        /// <param name="ckMsg">编码状态信息</param>
        /// <returns></returns>
        public bool CheckPnrStatus(RePnrObj PnrInfo, int PasType, out string ckMsg)
        {
            bool IsOk = false;
            ckMsg = "";
            //日志信息
            StringBuilder snLog = new StringBuilder();
            try
            {
                snLog.Append("start======================================================================\r\n");
                snLog.Append("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n");
                snLog.Append("编码类型:" + (PasType == 1 ? "成人" : "儿童") + "  编码:");
                if (PasType == 1)//成人
                {
                    if (PnrInfo.PnrList.Length > 0 && PnrInfo.PnrList[0] != null)
                    {
                        if (!string.IsNullOrEmpty(PnrInfo.PnrList[0].PnrStatus))
                        {
                            if (PnrInfo.PnrList[0].PnrStatus.Contains("HK")
                            || PnrInfo.PnrList[0].PnrStatus.Contains("KK")
                             || PnrInfo.PnrList[0].PnrStatus.Contains("DK")
                            || PnrInfo.PnrList[0].PnrStatus.Contains("K")
                            )
                            {
                                //通过
                                IsOk = true;
                            }
                            else
                            {
                                ckMsg = PnrInfo.PnrList[0].PnrStatus;
                            }
                        }
                        else
                        {
                            ckMsg = "解析失败";
                        }
                        snLog.Append(PnrInfo.PnrList[0].Pnr + " 编码状态：" + PnrInfo.PnrList[0].PnrStatus + "\r\n");
                    }
                }
                else if (PasType == 2)//儿童
                {
                    if (PnrInfo.PnrList.Length > 1 && PnrInfo.PnrList[1] != null)
                    {
                        if (!string.IsNullOrEmpty(PnrInfo.PnrList[1].PnrStatus))
                        {
                            if (PnrInfo.PnrList[1].PnrStatus.Contains("HK")
                            || PnrInfo.PnrList[1].PnrStatus.Contains("KK")
                            || PnrInfo.PnrList[1].PnrStatus.Contains("DK")
                            || PnrInfo.PnrList[1].PnrStatus.Contains("K"))
                            {
                                //通过
                                IsOk = true;
                            }
                            else
                            {
                                ckMsg = PnrInfo.PnrList[1].PnrStatus;
                            }
                        }
                        else
                        {
                            ckMsg = "解析失败";
                        }
                        snLog.Append(PnrInfo.PnrList[1].Pnr + " 编码状态：" + PnrInfo.PnrList[1].PnrStatus + "\r\n");
                    }
                }
            }
            catch (Exception ex)
            {
                snLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
            }
            finally
            {
                snLog.Append("IsOk=" + IsOk + "\r\n");
                snLog.Append("end======================================================================\r\n");
                PnrAnalysis.LogText.LogWrite(snLog.ToString(), "CheckPnrStatus");
            }
            return IsOk;
        }

        public FlightResponse AVHDataToFlightResponse(AVHData avhData)
        {
            FlightResponse flightResponse = new FlightResponse();
            bool flag = false;
            try
            {
                flightResponse.List = new List<FlightLineResponse>();
                //舱位数
                int seatCount = 0;
                for (int i = 0; i < avhData.IbeData.Count; i++)
                {
                    FlightLineResponse flightLineResponse = new FlightLineResponse();
                    flightLineResponse.SkyWay = new FlightSkyWayResponse();
                    flightLineResponse.SkyWay.CarrayCode = avhData.IbeData[i].CarrierCode;
                    CarryInfo carryInfo = format.GetAirInfo(flightLineResponse.SkyWay.CarrayCode);
                    flightLineResponse.SkyWay.CarrayShortName = carryInfo != null ? carryInfo.Carry.AirShortName : "";
                    flightLineResponse.SkyWay.FlightNumber = avhData.IbeData[i].FlightNo;
                    flightLineResponse.SkyWay.FromCityCode = avhData.QueryParam.FromCode;
                    flightLineResponse.SkyWay.ToCityCode = avhData.QueryParam.ToCode;
                    CityInfo cityInfo = format.GetCityInfo(avhData.QueryParam.FromCode);
                    flightLineResponse.SkyWay.FromCity = cityInfo != null ? cityInfo.city.Name : "";
                    flightLineResponse.SkyWay.FromAirPortrName = cityInfo != null ? cityInfo.city.AirPortName : "";
                    cityInfo = format.GetCityInfo(avhData.QueryParam.ToCode);
                    flightLineResponse.SkyWay.ToCity = cityInfo != null ? cityInfo.city.Name : "";
                    flightLineResponse.SkyWay.ToAirPortrName = cityInfo != null ? cityInfo.city.AirPortName : "";
                    flightLineResponse.SkyWay.StartDateTime = DateTime.Parse(avhData.IbeData[i].FlyDate + " " + avhData.IbeData[i].StartTime + ":00");
                    flightLineResponse.SkyWay.ToDateTime = GetEndDate(avhData.IbeData[i].FlyDate, avhData.IbeData[i].StartTime, avhData.IbeData[i].EndTime);
                    flightLineResponse.SkyWay.FromTerminal = avhData.IbeData[i].FromCityT1;
                    flightLineResponse.SkyWay.ToTerminal = avhData.IbeData[i].ToCityT1;
                    flightLineResponse.SkyWay.TaxFee = avhData.IbeData[i].TaxFee;
                    flightLineResponse.SkyWay.RQFee = avhData.IbeData[i].ADultFuleFee;
                    flightLineResponse.SkyWay.Model = avhData.IbeData[i].AirModel;
                    flightLineResponse.SkyWay.IsStop = avhData.IbeData[i].IsStop;
                    flightLineResponse.SkyWay.IsShareFlight = avhData.IbeData[i].IsShareFlight;
                    //过滤航空公司的查询                  
                    AirSystem airSystem = SystemConsoSwitch.AirSystems.Where(p => p.AirCode.ToUpper() == avhData.IbeData[i].CarrierCode.ToUpper()).FirstOrDefault();
                    if (airSystem != null && !airSystem.IsQuery) continue;
                    //机建为0的过滤掉
                    if (avhData.IbeData[i].TaxFee <= 0)
                    {
                        //没有机建费的机型
                        //WriteLog("Model", avhData.QueryParam.FromCode + "|" + avhData.QueryParam.ToCode + "|" + avhData.IbeData[i].CarrierCode + "|" + avhData.IbeData[i].AirModel + "\r\n");
                        //continue;
                        avhData.IbeData[i].TaxFee = 50m;//给个默认机建费
                    }
                    //过滤没有舱位的航空公司
                    if (avhData.IbeData[i].IBESeat != null && avhData.IbeData[i].IBESeat.Count > 0)
                    {
                        flag = true;
                        //Y舱价格
                        decimal YSeatPrice = 0m;
                        IbeSeat ibeYSeat = avhData.IbeData[i].IBESeat.Where(p => p.Seat.Trim().ToUpper() == "Y").FirstOrDefault();
                        if (ibeYSeat == null)
                        {
                            if (avhData.DicYSeatPrice.ContainsKey(avhData.IbeData[i].CarrierCode)) YSeatPrice = avhData.DicYSeatPrice[avhData.IbeData[i].CarrierCode];
                        }
                        else
                            YSeatPrice = ibeYSeat.SeatPrice;
                        List<SeatResponse> SeatArray = new List<SeatResponse>();
                        for (int j = 0; j < avhData.IbeData[i].IBESeat.Count; j++)
                        {
                            int.TryParse(avhData.IbeData[i].IBESeat[j].SeatCount.Replace(">", ""), out seatCount);
                            //数据过滤 舱位数>0 折扣>0  舱位价>0  政策为0
                            if (seatCount > 0
                                //&& avhData.IbeData[i].IBESeat[j].Rebate > 0
                                // && avhData.IbeData[i].IBESeat[j].SeatPrice > 0
                                //&& avhData.IbeData[i].IBESeat[j].Policy > 0
                                )
                            {
                                if (avhData.IbeData[i].IBESeat[j].SeatPrice == 0
                                    && (avhData.IbeData[i].IBESeat[j].PolicySpecialType == EnumPolicySpecialType.Normal
                                    || avhData.IbeData[i].IBESeat[j].PolicySpecialType == EnumPolicySpecialType.DiscountOnDiscount
                                    || avhData.IbeData[i].IBESeat[j].PolicySpecialType == EnumPolicySpecialType.DownSpecial
                                    ))
                                {
                                    //没有舱位价的舱位
                                    WriteLog("SeatPrice", avhData.QueryParam.FromCode + "|" + avhData.QueryParam.ToCode + "|" + avhData.IbeData[i].CarrierCode + "|" + avhData.IbeData[i].IBESeat[j].Seat + "\r\n");
                                    continue;
                                    //没有运价的当做动态特价处理
                                    //avhData.IbeData[i].IBESeat[j].PolicySpecialType = EnumPolicySpecialType.DynamicSpecial;
                                }
                                //排除MU Q舱
                                if (string.Compare(avhData.IbeData[i].CarrierCode, "MU", true) == 0
                                    && string.Compare(avhData.IbeData[i].IBESeat[j].Seat, "Q", true) == 0)
                                {
                                    continue;
                                }
                                //排除CZ K舱
                                if (string.Compare(avhData.IbeData[i].CarrierCode, "CZ", true) == 0
                                    && string.Compare(avhData.IbeData[i].IBESeat[j].Seat, "K", true) == 0)
                                {
                                    continue;
                                }
                                SeatArray.Add(new SeatResponse()
                                {
                                    PolicySpecialType = avhData.IbeData[i].IBESeat[j].PolicySpecialType,
                                    Discount = avhData.IbeData[i].IBESeat[j].Rebate,
                                    PolicyPoint = avhData.IbeData[i].IBESeat[j].Policy,
                                    SeatCode = avhData.IbeData[i].IBESeat[j].Seat,
                                    SeatPrice = avhData.IbeData[i].IBESeat[j].SeatPrice,
                                    IbeSeatPrice = avhData.IbeData[i].IBESeat[j].IbeSeatPrice,
                                    SeatCount = avhData.IbeData[i].IBESeat[j].SeatCount,
                                    TicketPrice = avhData.IbeData[i].IBESeat[j].SeatPrice + avhData.IbeData[i].TaxFee + avhData.IbeData[i].ADultFuleFee,
                                    YPrice = YSeatPrice
                                });
                            }
                        }
                        flightLineResponse.SeatList = SeatArray.ToArray();
                        if (SeatArray.Count > 0)
                        {
                            flightResponse.List.Add(flightLineResponse);
                        }
                    }
                    else
                    {
                        //WriteLog("NoIBESeat", string.Format("{0}{1}起飞日期：{2},城市对{3}-{4}", avhData.IbeData[i].CarrierCode, avhData.IbeData[i].FlightNo, avhData.IbeData[i].FlyDate, avhData.IbeData[i].FromCode, avhData.IbeData[i].ToCode));
                    }
                }
                if (!flag)
                {
                    WriteLog("NoIBESeat_Old", string.Format("城市对{0}-{1},起飞日期：{2}\r\nAVH返回原始数据:{3}\r\n", avhData.QueryParam.FromCode,
                     avhData.QueryParam.ToCode, avhData.QueryParam.FlyDate, avhData.AVHString
                     ));
                    throw new CustomException(10001, "您查询的航班已售完");
                }
            }
            catch (Exception ex)
            {
                //记录日志              
                WriteLog("AVHDataToFlightResponse", "异常信息:" + ex.Message);
                throw new CustomException(10001, ex.Message);
            }
            return flightResponse;
        }


        /// <summary>
        /// 创建行程单
        /// </summary>
        /// <param name="travelAppRequrst"></param>
        public TravelAppResponse CreateTrip(TravelAppRequrst travelAppRequrst)
        {
            bool IsCreate = false;
            string ThrowMsg = string.Empty;
            TravelAppResponse response = new TravelAppResponse();
            StringBuilder sbLog = new StringBuilder();
            try
            {
                if (businessman == null
                    || ((businessman is Carrier) ? (businessman as Carrier).Pids : (businessman as Supplier).SupPids) == null
                    || ((businessman is Carrier) ? (businessman as Carrier).Pids.Count : (businessman as Supplier).SupPids.Count) == 0)
                {
                    ThrowMsg = "配置信息未设置，请联系运营商设置！";
                }
                else
                {
                    PID pid = (businessman is Carrier) ? (businessman as Carrier).Pids.FirstOrDefault() : (businessman as Supplier).SupPids.FirstOrDefault();
                    if (pid != null)
                    {
                        strServerIP = pid.IP;
                        strServerPort = pid.Port.ToString();
                        strOffice = pid.Office;
                    }

                    if (string.IsNullOrEmpty(strServerIP)
                        || string.IsNullOrEmpty(strServerPort)
                         || string.IsNullOrEmpty(strOffice)
                        )
                    {
                        ThrowMsg = "服务端配置信息未设置！";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(travelAppRequrst.CreateOffice))
                        {
                            if (!string.IsNullOrEmpty(strOffice))
                            {
                                PID PidInfo = (businessman is Carrier) ? (businessman as Carrier).Pids.Where(p => p.Office.ToUpper() == strOffice.ToUpper()).FirstOrDefault() : (businessman as Supplier).SupPids.Where(p => p.Office.ToUpper() == strOffice.ToUpper()).FirstOrDefault();
                                if (PidInfo != null)
                                {
                                    strServerIP = PidInfo.IP;
                                    strServerPort = PidInfo.Port.ToString();
                                }
                            }
                        }
                        else
                        {
                            strOffice = travelAppRequrst.CreateOffice;
                        }

                        #region 验证数据
                        //验证票号有效性
                        string pattern = @"\d{3,4}(\-?|\s+)\d{10}";
                        if (!Regex.Match(travelAppRequrst.TicketNumber, pattern, RegexOptions.IgnoreCase).Success)
                        {
                            ThrowMsg = string.Format("客票票号{0}格式错误！", travelAppRequrst.TicketNumber);
                        }
                        if (string.IsNullOrEmpty(ThrowMsg))
                        {
                            //验证行程单格式有效性
                            pattern = @"(?:\d{10})";
                            if (!Regex.IsMatch(travelAppRequrst.TripNumber, pattern, RegexOptions.IgnoreCase))
                            {
                                ThrowMsg = string.Format("行程单号{0}格式不正确！", travelAppRequrst.TripNumber);
                            }
                        }
                        if (string.IsNullOrEmpty(ThrowMsg))
                        {
                            //验证Office号
                            pattern = @"[a-zA-Z]{3}\d{3}";
                            if (!Regex.IsMatch(strOffice, pattern, RegexOptions.IgnoreCase))
                            {
                                ThrowMsg = string.Format("行程单终端号({0})设置错误！", strOffice.Trim());
                            }
                        }
                        travelAppRequrst.TicketNumber = travelAppRequrst.TicketNumber.Replace(" ", "").Replace("-", "").Trim();
                        response.TicketNumber = travelAppRequrst.TicketNumber;
                        response.TripNumber = travelAppRequrst.TripNumber;
                        response.CreateOffice = strOffice;
                        #endregion;
                        if (IsTripTest)
                        {
                            //测试
                            response.PnrAnalysisTripNumber = travelAppRequrst.TripNumber;
                            IsCreate = true;
                            response.IsSuc = IsCreate;
                            return response;
                        }

                        if (string.IsNullOrEmpty(ThrowMsg))
                        {
                            #region 发指令
                            ParamObject pmObject = new ParamObject();
                            pmObject.ServerIP = strServerIP;
                            //连接PID的IP端口
                            int ServerPort = 392;
                            int.TryParse(strServerPort, out ServerPort);
                            pmObject.ServerPort = ServerPort;
                            pmObject.Office = strOffice;
                            pmObject.WebUserName = (businessman is Carrier) ? (businessman as Carrier).Code : (businessman as Supplier).Code;


                            sbLog.Append("IP端口:" + pmObject.ServerIP + ":" + pmObject.ServerPort + " Office：" + pmObject.Office + " WebUserName:" + pmObject.WebUserName + "\r\n");
                            pmObject.code = string.Format("DETR:TN/{0}", travelAppRequrst.TicketNumber);
                            sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                            string recvData = SendNewPID.SendCommand(pmObject);
                            sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                            //重发一次
                            //if (IsReSend(recvData))
                            //{
                            //    sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                            //    recvData = SendNewPID.SendCommand(pmObject);
                            //    sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                            //}
                            if (!IsReSend(recvData)
                                && !recvData.ToUpper().Contains("NOT EXIST")
                                && !recvData.ToUpper().Contains("TICKET NUMBER"))// && (recvData.ToUpper().Contains("NO RECORD") || recvData.ToUpper().Contains("DETR:TN") || recvData.ToUpper().Contains("DETR TN")))
                            {
                                //检查该票号是否创建行程单
                                if (recvData.ToUpper().Contains("RECEIPT PRINTED"))
                                {
                                    #region 已创建
                                    //检查票号中关联的行程单号与需要关联的行程单号是否一致
                                    //一致的话 就直接返回创建成功
                                    //不一致。。。                                                    
                                    pmObject.code = string.Format("DETR:TN/{0},F", travelAppRequrst.TicketNumber);
                                    sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                    recvData = SendNewPID.SendCommand(pmObject);
                                    sbLog.AppendFormat("返回结果:{0}\r\n", recvData);

                                    //重发一次
                                    //if (IsReSend(recvData))
                                    //{
                                    //    sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                    //    recvData = SendNewPID.SendCommand(pmObject);
                                    //    sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                                    //}
                                    //解析票号中的行程单号
                                    List<DetrInfo> detrList = pnrformat.GetDetrF(recvData);
                                    if (detrList.Count > 0)
                                    {
                                        DetrInfo detrInfo = detrList[0];
                                        if (detrInfo.CreateSerialNumber.Trim() != travelAppRequrst.TripNumber.Trim())
                                        {
                                            ThrowMsg = string.Format("该票号{0}已创建行程单，关联行程单号【{1}】与输入行程单号{2}不一致！", travelAppRequrst.TicketNumber, detrInfo.CreateSerialNumber.Trim(), travelAppRequrst.TripNumber.Trim());
                                        }
                                        else
                                        {
                                            ThrowMsg = string.Format("创建行程单成功,票号【{0}】与行程单号【{1}】已关联！", travelAppRequrst.TicketNumber, travelAppRequrst.TripNumber.Trim());
                                        }
                                        response.PnrAnalysisTripNumber = detrInfo.CreateSerialNumber.Trim();
                                    }
                                    else
                                    {
                                        ThrowMsg = string.Format("票号【{0}】已创建行程单号,未能解析出行程单号", travelAppRequrst.TicketNumber);
                                    }
                                    IsCreate = true;
                                    #endregion
                                }
                                else
                                {
                                    #region 再次验证票号是否创建行程单
                                    pmObject.code = string.Format("DETR:TN/{0},F", travelAppRequrst.TicketNumber);
                                    sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                    recvData = SendNewPID.SendCommand(pmObject);
                                    sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                                    //重发一次
                                    //if (IsReSend(recvData))
                                    //{
                                    //    sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                    //    recvData = SendNewPID.SendCommand(pmObject);
                                    //    sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                                    //}
                                    //解析票号中的行程单号
                                    List<DetrInfo> detrList = pnrformat.GetDetrF(recvData);
                                    if (detrList.Count > 0)
                                    {
                                        DetrInfo detrInfo = detrList[0];
                                        if (detrInfo.CreateSerialNumber.Trim() != "")
                                        {
                                            if (detrInfo.CreateSerialNumber.Trim() != travelAppRequrst.TripNumber.Trim())
                                            {
                                                ThrowMsg = string.Format("该票号{0}已创建行程单，关联行程单号【{1}】与输入行程单号{2}不一致！", travelAppRequrst.TicketNumber, detrInfo.CreateSerialNumber.Trim(), travelAppRequrst.TripNumber.Trim());
                                            }
                                            else
                                            {
                                                ThrowMsg = string.Format("创建行程单成功,票号【{0}】与行程单号【{1}】已关联！", travelAppRequrst.TicketNumber, travelAppRequrst.TripNumber.Trim());
                                            }
                                            response.PnrAnalysisTripNumber = detrInfo.CreateSerialNumber.Trim();
                                            IsCreate = true;
                                        }
                                    }
                                    #endregion

                                    #region  创建
                                    if (!IsCreate)
                                    {
                                        #region 方法一 //需要创建
                                        //正式
                                        pmObject.code = string.Format("PRINV:{0},{{ITTN={1}}}", travelAppRequrst.TicketNumber, travelAppRequrst.TripNumber);
                                        sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                        recvData = SendNewPID.SendCommand(pmObject);
                                        sbLog.AppendFormat("返回结果:{0}\r\n", recvData);

                                        //重发一次
                                        if (IsReSend(recvData))
                                        {
                                            sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                            recvData = SendNewPID.SendCommand(pmObject);
                                            sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                                        }
                                        if (recvData.Contains("该行程单号已被"))
                                        {
                                            string GetPattern = @"ERROR:(?<TravelNumber>\d{10})该行程单号已被(?<TicketNum>\d{3,4}(\-?|\s+)\d{10})使用!";
                                            Match m_mch = Regex.Match(recvData, GetPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                            if (m_mch.Success)
                                            {
                                                string innner_TravelNumber = m_mch.Groups["TravelNumber"].Value.Trim();
                                                string innner_TicketNum = m_mch.Groups["TicketNum"].Value.Trim();
                                                if (innner_TravelNumber == travelAppRequrst.TripNumber.Trim() && innner_TicketNum == travelAppRequrst.TicketNumber.Trim())
                                                {
                                                    IsCreate = true;
                                                }
                                                else
                                                {
                                                    ThrowMsg = recvData;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            string sucPatern = @"(?<=\<Flag\>\s*)(?<Flag>\w)(?=\s*\<\/Flag\>)";
                                            Match mch = Regex.Match(recvData, sucPatern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                            string TripFlag = "";
                                            if (mch.Success)
                                            {
                                                TripFlag = mch.Groups["Flag"].Value.Trim().ToUpper();
                                            }
                                            if (recvData.Trim() == "SUCCESS" || recvData.Contains("成功") || TripFlag == "S")
                                            {
                                                IsCreate = true;
                                            }
                                            else
                                            {
                                                ThrowMsg = recvData;
                                            }
                                        }

                                        if (IsCreate)
                                        {
                                            pmObject.code = string.Format("DETR:TN/{0},F", travelAppRequrst.TicketNumber);
                                            sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                            recvData = SendNewPID.SendCommand(pmObject);
                                            sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                                            //解析票号中的行程单号
                                            detrList = pnrformat.GetDetrF(recvData);
                                            if (detrList.Count > 0)
                                            {
                                                DetrInfo detrInfo = detrList[0];
                                                if (detrInfo.CreateSerialNumber.Trim() != "")
                                                {
                                                    if (detrInfo.CreateSerialNumber.Trim() != travelAppRequrst.TripNumber.Trim())
                                                    {
                                                        ThrowMsg = string.Format("该票号{0}已创建行程单，关联行程单号【{1}】与输入行程单号{2}不一致！", travelAppRequrst.TicketNumber, detrInfo.CreateSerialNumber.Trim(), travelAppRequrst.TripNumber.Trim());
                                                    }
                                                    else
                                                    {
                                                        ThrowMsg = string.Format("创建行程单成功,票号【{0}】与行程单号【{1}】已关联！", travelAppRequrst.TicketNumber, travelAppRequrst.TripNumber.Trim());
                                                    }
                                                    response.PnrAnalysisTripNumber = detrInfo.CreateSerialNumber.Trim();
                                                    IsCreate = true;
                                                }
                                            }
                                            else
                                            {
                                                ThrowMsg = string.Format("票号【{0}】已创建行程单号,未能解析出关联行程单号", travelAppRequrst.TicketNumber);
                                            }
                                            sbLog.AppendFormat("解析出来的行程单号:{0}\r\n", response.PnrAnalysisTripNumber);
                                        }
                                        #endregion
                                    }
                                    #endregion
                                }
                            }
                            else
                            {
                                //返回具体错误信息
                                ThrowMsg = recvData;
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowMsg = ex.Message;
            }
            finally
            {
                response.ShowMsg = ThrowMsg;
                response.IsSuc = IsCreate;
                sbLog.Append("ThrowMsg=" + ThrowMsg);
                new CommLog().WriteLog("CreateTrip", sbLog.ToString());
            }
            return response;
        }
        /// <summary>
        /// 作废行程单
        /// </summary>
        /// <param name="travelAppResponse"></param>
        public TravelAppResponse VoidTrip(TravelAppRequrst travelAppRequrst)
        {
            TravelAppResponse response = new TravelAppResponse();
            bool IsVoid = false;
            string ThrowMsg = string.Empty;
            StringBuilder sbLog = new StringBuilder();
            try
            {
                if (businessman == null
                     || ((businessman is Carrier) ? (businessman as Carrier).Pids : (businessman as Supplier).SupPids) == null
                     || ((businessman is Carrier) ? (businessman as Carrier).Pids.Count : (businessman as Supplier).SupPids.Count) == 0)
                {
                    ThrowMsg = "配置信息未设置，请联系运营商设置！";
                }
                else
                {
                    PID pid = (businessman is Carrier) ? (businessman as Carrier).Pids.FirstOrDefault() : (businessman as Supplier).SupPids.FirstOrDefault();
                    if (pid != null)
                    {
                        strServerIP = pid.IP;
                        strServerPort = pid.Port.ToString();
                        strOffice = pid.Office;
                    }
                    if (string.IsNullOrEmpty(strServerIP)
                        || string.IsNullOrEmpty(strServerPort)
                         || string.IsNullOrEmpty(strOffice)
                        )
                    {
                        ThrowMsg = "服务端配置信息未设置！";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(travelAppRequrst.CreateOffice))
                        {
                            if (!string.IsNullOrEmpty(strOffice))
                            {
                                //PID PidInfo = carrier.Pids.Where(p => p.Office.ToUpper() == strOffice.ToUpper()).FirstOrDefault();
                                PID PidInfo = (businessman is Carrier) ? (businessman as Carrier).Pids.Where(p => p.Office.ToUpper() == strOffice.ToUpper()).FirstOrDefault() : (businessman as Supplier).SupPids.Where(p => p.Office.ToUpper() == strOffice.ToUpper()).FirstOrDefault();
                                if (PidInfo != null)
                                {
                                    strServerIP = PidInfo.IP;
                                    strServerPort = PidInfo.Port.ToString();
                                }
                            }
                        }
                        else
                        {
                            strOffice = travelAppRequrst.CreateOffice;
                        }

                        #region 验证数据
                        //验证票号有效性
                        string pattern = @"\d{3,4}(\-?|\s+)\d{10}";
                        if (!Regex.Match(travelAppRequrst.TicketNumber, pattern, RegexOptions.IgnoreCase).Success)
                        {
                            ThrowMsg = string.Format("客票票号{0}格式错误！", travelAppRequrst.TicketNumber);
                        }
                        if (string.IsNullOrEmpty(ThrowMsg))
                        {
                            //验证行程单格式有效性
                            pattern = @"(?:\d{10})";
                            if (!Regex.IsMatch(travelAppRequrst.TripNumber, pattern, RegexOptions.IgnoreCase))
                            {
                                ThrowMsg = string.Format("行程单号{0}格式不正确！", travelAppRequrst.TripNumber);
                            }
                        }
                        if (string.IsNullOrEmpty(ThrowMsg))
                        {
                            //验证Office号
                            pattern = @"[a-zA-Z]{3}\d{3}";
                            if (!Regex.IsMatch(strOffice, pattern, RegexOptions.IgnoreCase))
                            {
                                ThrowMsg = string.Format("行程单终端号({0})设置错误！", strOffice.Trim());
                            }
                        }

                        travelAppRequrst.TicketNumber = travelAppRequrst.TicketNumber.Replace(" ", "").Replace("-", "").Trim();
                        response.CreateOffice = strOffice;
                        response.TicketNumber = travelAppRequrst.TicketNumber;
                        response.TripNumber = travelAppRequrst.TripNumber;
                        #endregion
                        if (IsTripTest)
                        {
                            //测试
                            response.PnrAnalysisTripNumber = travelAppRequrst.TripNumber;
                            IsVoid = true;
                            response.IsSuc = IsVoid;
                            return response;
                        }
                        if (string.IsNullOrEmpty(ThrowMsg))
                        {
                            #region 发指令
                            ParamObject pmObject = new ParamObject();
                            pmObject.ServerIP = strServerIP;
                            //连接PID的IP端口
                            int ServerPort = 392;
                            int.TryParse(strServerPort, out ServerPort);
                            pmObject.ServerPort = ServerPort;
                            pmObject.Office = strOffice;
                            pmObject.WebUserName = (businessman is Carrier) ? (businessman as Carrier).Code : (businessman as Supplier).Code;
                            sbLog.Append("IP端口:" + pmObject.ServerIP + ":" + pmObject.ServerPort + " Office：" + pmObject.Office + " WebUserName:" + pmObject.WebUserName + "\r\n");

                            #region 验证是否已作废
                            pmObject.code = string.Format("DETR:TN/{0},F", travelAppRequrst.TicketNumber);
                            sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                            string recvData = SendNewPID.SendCommand(pmObject);
                            sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                            //重发一次
                            //if (IsReSend(recvData)
                            //    || (!recvData.ToUpper().Contains("DETR:TN") && !recvData.ToUpper().Contains("DETR TN")))
                            //{
                            //    sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                            //    recvData = SendNewPID.SendCommand(pmObject);
                            //    sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                            //}
                            List<DetrInfo> detrList = pnrformat.GetDetrF(recvData);
                            if (detrList.Count > 0)
                            {
                                DetrInfo detrInfo = detrList[0];
                                if (detrInfo.VoidSerialNumber.Trim() != "")
                                {
                                    IsVoid = true;
                                    if (detrInfo.VoidSerialNumber.Trim() != travelAppRequrst.TripNumber)
                                    {
                                        ThrowMsg = string.Format("该票号{0}已作废行程单,票号中关联的作废行程单号【{1}】与输入行程单号【{2}】不一致！", travelAppRequrst.TicketNumber, detrInfo.VoidSerialNumber.Trim(), travelAppRequrst.TripNumber.Trim()
                                        );
                                    }
                                    response.PnrAnalysisTripNumber = detrInfo.VoidSerialNumber.Trim();
                                }
                                //该票号关联的行程单号与传入需要作废的行程单号不一致不能作废
                                if (detrInfo.CreateSerialNumber.Trim() != ""
                                    && detrInfo.CreateSerialNumber.Trim() != travelAppRequrst.TripNumber)
                                {
                                    ThrowMsg = string.Format("该票号{0}关联行程单号" + detrInfo.CreateSerialNumber.Trim() + "与传入作废的行程单号" + travelAppRequrst.TripNumber + "不一致,无法作废!",
                                        travelAppRequrst.TicketNumber);
                                }
                            }
                            #endregion

                            if (!IsVoid && ThrowMsg == "")
                            {
                                //可以作废
                                pmObject.code = string.Format("VTINV:{0},{{ITTN={1}}}", travelAppRequrst.TicketNumber, travelAppRequrst.TripNumber);
                                sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                recvData = SendNewPID.SendCommand(pmObject);
                                sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                                //重发一次
                                if (IsReSend(recvData))
                                {
                                    sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                    recvData = SendNewPID.SendCommand(pmObject);
                                    sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                                }
                                //验证是否作废成功
                                if (recvData.Trim() == "SUCCESS")
                                {
                                    IsVoid = true;
                                }
                                else
                                {
                                    ThrowMsg = recvData;
                                }
                                //if (IsVoid)
                                //{
                                #region 继续检查是否作废
                                pmObject.code = string.Format("DETR:TN/{0},F", travelAppRequrst.TicketNumber);
                                sbLog.AppendFormat("【{0}】发送数据:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM:dd HH:mm:ss.fff"), pmObject.code);
                                recvData = SendNewPID.SendCommand(pmObject);
                                sbLog.AppendFormat("返回结果:{0}\r\n", recvData);
                                detrList = pnrformat.GetDetrF(recvData);
                                if (detrList.Count > 0)
                                {
                                    DetrInfo detrInfo = detrList[0];
                                    if (detrInfo.VoidSerialNumber.Trim() != "")
                                    {
                                        if (detrInfo.VoidSerialNumber.Trim() == travelAppRequrst.TripNumber)
                                        {
                                            IsVoid = true;
                                        }
                                        else
                                        {
                                            // ThrowMsg = string.Format("该票号{0}作废行程单失败", travelAppRequrst.TicketNumber, detrInfo.VoidSerialNumber.Trim());
                                        }
                                        response.PnrAnalysisTripNumber = detrInfo.VoidSerialNumber.Trim();
                                    }
                                }
                                else
                                {
                                    ThrowMsg = string.Format("该票号{0}已作废行程单,未能解析出作废行程单号！", travelAppRequrst.TicketNumber);
                                }
                                sbLog.AppendFormat("解析出来的行程单号:{0}\r\n", response.PnrAnalysisTripNumber);
                                #endregion
                                //}
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ThrowMsg = ex.Message;
            }
            finally
            {
                response.ShowMsg = ThrowMsg;
                response.IsSuc = IsVoid;
                sbLog.Append("ThrowMsg=" + ThrowMsg);
                new CommLog().WriteLog("VoidTrip", sbLog.ToString());
            }
            return response;
        }


        private FlightResponse[] QueryFlight(string code, FlightResponse[] flightResponse, List<QueryParam> QueryParamList, PolicyQueryParam policyQueryParam)
        {
            try
            {
                //获取航班数据
                List<AVHData> avhList = new List<AVHData>();

                //获取政策
                List<PolicyCache> interfacePolicyList = new List<PolicyCache>();
                List<PolicyCache> localPolicyList = new List<PolicyCache>();
                Parallel.Invoke(
                    () => avhList = queryFlightService.GetBasic(QueryParamList),
                    () => interfacePolicyList = queryFlightService.GetPolicy(policyQueryParam),
                    () => localPolicyList = policyService.GetFlightPolicy(code, policyQueryParam)
                );
                List<PolicyCache> policyList = new List<PolicyCache>();
                policyList.AddRange(interfacePolicyList.ToArray());
                policyList.AddRange(localPolicyList.ToArray());
                //匹配政策
                avhList = queryFlightService.MatchPolicy(code, avhList, policyQueryParam.TravelType, policyList);
                //avhList->FlightResponse 转换即可
                if (avhList != null && avhList.Count > 0)
                {
                    List<FlightResponse> resList = new List<FlightResponse>();
                    for (int i = 0; i < avhList.Count; i++)
                    {
                        AVHData avhData = avhList[i];
                        FlightResponse fResponse = AVHDataToFlightResponse(avhData);
                        if (fResponse != null)
                        {
                            resList.Add(fResponse);
                        }
                    }
                    flightResponse = resList.ToArray();
                }
                else
                {
                    //抛出异常
                    throw new CustomException(111, "没有查找到航班数据！");
                }
            }
            catch (Exception ex)
            {
                //记录日志
                WriteLog("QueryFlight", "异常信息:" + ex.Message);
                throw new CustomException(111, ex.Message);
            }
            return flightResponse;
        }



        /// <summary>
        /// 自动加一天
        /// </summary>
        /// <param name="flyDate"></param>
        /// <param name="StartTime"></param>
        /// <param name="EndTime"></param>
        /// <returns></returns>
        private DateTime GetEndDate(string flyDate, string StartTime, string EndTime)
        {
            DateTime dt1 = DateTime.Parse(flyDate + " " + StartTime + ":00");
            DateTime dt2 = DateTime.Parse(flyDate + " " + EndTime + ":00");
            if (DateTime.Compare(dt1, dt2) > 0)
            {
                dt2 = dt2.AddDays(1);
            }
            return dt2;
        }

        /// <summary>
        /// 配置查航班
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fromCode"></param>
        /// <param name="toCode"></param>
        /// <param name="airCode"></param>
        /// <param name="flyDate"></param>
        /// <param name="flyTime"></param>
        /// <returns></returns>
        public string GetAV(string code, string fromCode, string toCode, string airCode, string flyDate, string flyTime)
        {
            string result = string.Empty;
            List<PidSetting> pidSettingList = GetSetting(code);
            if (pidSettingList.Count > 0)
            {
                PidSetting setting = pidSettingList[0];
                WebManage.ServerIp = setting.IP;
                WebManage.ServerPort = setting.Port;
                string err = string.Empty;
                airCode = string.IsNullOrEmpty(airCode) ? "" : airCode;
                bool isok = WebManage.PidSendCommand_av(fromCode, toCode, airCode, flyDate, flyTime, setting.Office, "AVH", ref result, ref err, setting.IP, setting.Port);
            }
            return result;
        }
        /// <summary>
        /// 商户号code
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        private List<PidSetting> GetSetting(string code)
        {
            List<PidSetting> settingList = new List<PidSetting>();
            var user = this.businessmanRepository.FindAll(p => p.Code == code).FirstOrDefault();
            if (user != null)
            {
                if (user is Buyer)
                {
                    Buyer buyer = (user as Buyer);
                    Carrier carrier = this.businessmanRepository.FindAll(p => p.Code == buyer.CarrierCode).FirstOrDefault() as Carrier;
                    if (carrier != null)
                    {
                        if (carrier.Pids != null && carrier.Pids.Count > 0)
                        {
                            carrier.Pids.ToList().ForEach(p =>
                            {
                                settingList.Add(new PidSetting()
                                {
                                    buyerCode = code,
                                    CarrierCode = carrier.Code,
                                    IP = p.IP,
                                    Port = p.Port,
                                    Office = p.Office
                                });
                            });
                        }
                    }
                }
                else if (user is Carrier)
                {
                    Carrier carrier = user as Carrier;
                    if (carrier.Pids != null && carrier.Pids.Count > 0)
                    {
                        carrier.Pids.ToList().ForEach(p =>
                        {
                            settingList.Add(new PidSetting()
                            {
                                CarrierCode = carrier.Code,
                                IP = p.IP,
                                Port = p.Port,
                                Office = p.Office
                            });
                        });
                    }
                }
                else if (user is Supplier)
                {
                    Supplier supplier = user as Supplier;
                    if (supplier.SupPids != null && supplier.SupPids.Count > 0)
                    {
                        supplier.SupPids.ToList().ForEach(p =>
                        {
                            settingList.Add(new PidSetting()
                            {
                                SupplierCode = supplier.Code,
                                IP = p.IP,
                                Port = p.Port,
                                Office = p.Office
                            });
                        });
                    }
                }
            }
            return settingList;
        }

        private bool IsReSend(string data)
        {
            bool resend = false;
            if (data == null)
            {
                data = "";
            }
            data = data.ToLower();
            if (data == "" || data.Contains("地址无效") || data.Contains("超时") || data.Contains("服务器忙") || data.Contains("wsacancelblockingcall") || data.Contains("无法连接") || data.Contains("由于") || data.Contains("强迫关闭") || data.Contains("无法从传输连接中读取数据") || data.Contains("NO PNR")
                )
            {
                resend = true;
            }
            return resend;
        }
        //记录日志
        private void WriteLog(string method, string content)
        {
            log.WriteLog(method, content);
        }

        public RareResponse IsRarePidString(string passengerName)
        {
            RareResponse rareResponse = new RareResponse();
            string strRareData = PinYingMange.ReadRare();
            if (!string.IsNullOrEmpty(strRareData) && !string.IsNullOrEmpty(passengerName))
            {
                List<char> rareList = new List<char>();
                foreach (var item in passengerName)
                {
                    if (strRareData.Contains(item))
                    {
                        rareList.Add(item);
                    }
                }
                if (rareList.Count > 0)
                {
                    rareResponse.IsRare = true;
                    rareResponse.RareFontString = string.Join("", rareList.ToArray());
                }
            }
            return rareResponse;
        }


    }
}
