using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using PnrAnalysis;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System.Text.RegularExpressions;
using PnrAnalysis.Model;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using PBPid.WebManage;
using BPiaoBao.DomesticTicket.Domain.Services;
using StructureMap;
using BPiaoBao.Common;
using System.Threading.Tasks;
using System.IO;
using JoveZhao.Framework.DDD;
using PiaoBao.BTicket.EFRepository;
using System.Threading;
namespace BPiaoBao.AppServices.DomesticTicket
{
    public class PidService : IPidService
    {
        IOrderRepository orderRepository;
        IBusinessmanRepository businessmanRepository;
        FormatPNR format = new FormatPNR();
        public PidService(IOrderRepository orderRepository, IBusinessmanRepository businessmanRepository)
        {
            this.orderRepository = orderRepository;
            this.businessmanRepository = businessmanRepository;
        }
        /// <summary>
        /// 使用运营设置的配置发送指令
        /// </summary>
        /// <param name="carrierCode"></param>
        /// <param name="cmd"></param>
        /// <param name="Office"></param>
        /// <param name="ExtendData"></param>
        /// <param name="isRT"></param>
        /// <param name="isUseExtend"></param>
        /// <returns></returns>
        private string SendCmdByCarrierCode(string carrierCode, string cmd, string Office, string ExtendData = "", bool isRT = false, bool isUseExtend = false)
        {
            string result = string.Empty;
            try
            {
                var rsCode = this.businessmanRepository.FindAll(p => p.Code == carrierCode).FirstOrDefault();
                Carrier carrier = rsCode as Carrier;
                Supplier supplier = rsCode as Supplier;
                if (carrier != null || supplier != null)
                {
                    if ((carrier != null && (carrier.Pids == null || carrier.Pids.Count == 0))
                         || (supplier != null && (supplier.SupPids == null || supplier.SupPids.Count == 0))
                        )
                    {
                        result = "商户配置信息未设置,请联系所属商户设置！";
                    }
                    else
                    {
                        if (!string.IsNullOrEmpty(Office))
                        {
                            PID PidInfo = carrier != null ? carrier.Pids.Where(p => p.Office.ToUpper() == Office.ToUpper()).FirstOrDefault() :
                                supplier.SupPids.Where(p => p.Office.ToUpper() == Office.ToUpper()).FirstOrDefault();
                            if (PidInfo != null)
                            {
                                PidParam pidParam = new PidParam();
                                pidParam.ServerIP = PidInfo.IP;
                                pidParam.ServerPort = PidInfo.Port.ToString();
                                pidParam.Office = string.IsNullOrEmpty(Office) ? PidInfo.Office : Office;
                                pidParam.Cmd = cmd;
                                pidParam.IsUseExtend = isUseExtend;
                                pidParam.ExtendData = ExtendData;
                                pidParam.UserName = carrier.Code;
                                pidParam.IsHandResult = false;
                                if (isRT)
                                {
                                    pidParam.IsPn = true;
                                }
                                result = SendPid(pidParam);
                                if (result.Contains("授权"))
                                {
                                    result = string.Format("请授权,授权指令:RMK TJ AUTH {0}", Office);
                                }
                            }
                            else
                            {
                                result = "未找到商户配置信息所设置的Office:" + Office;
                            }
                        }
                        else
                        {
                            //循环发送
                            List<PID> PIDs = carrier != null ? carrier.Pids.ToList() : supplier.SupPids.ToList();
                            foreach (PID PidInfo in PIDs)
                            {
                                PidParam pidParam = new PidParam();
                                pidParam.ServerIP = PidInfo.IP;
                                pidParam.ServerPort = PidInfo.Port.ToString();
                                pidParam.Office = string.IsNullOrEmpty(Office) ? PidInfo.Office : Office;
                                pidParam.Cmd = cmd;
                                pidParam.IsUseExtend = isUseExtend;
                                pidParam.ExtendData = ExtendData;
                                pidParam.UserName = carrier.Code;
                                pidParam.IsHandResult = false;
                                if (isRT)
                                {
                                    pidParam.IsPn = true;
                                }
                                result = SendPid(pidParam);
                                if (!result.Contains("授权"))
                                {
                                    break;
                                }
                            }
                            if (result.Contains("授权") && PIDs.Count > 0)
                            {
                                result = string.Format("请授权,授权指令:RMK TJ AUTH {0}", PIDs[0].Office);
                            }
                        }
                    }
                }
                else
                {
                    result = "商户号信息不存在";
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.INFO, string.Format("[SendCmdByCarrierCode]{0}\r\n{1}",
                    System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ex.Message));
            }
            return result;
        }
        /// <summary>
        /// 直接发送指令
        /// </summary>
        /// <param name="pidParam">配置参数</param>
        /// <returns></returns>
        [ExtOperationInterceptor("直接发送指令")]
        public string SendPid(PidParam pidParam)
        {
            StringBuilder sbLog = new StringBuilder();
            string result = string.Empty;
            sbLog.Append("START=====================================\r\n");
            try
            {
                ParamObject pm = new ParamObject();
                if (string.IsNullOrEmpty(pidParam.ServerIP)
                    || string.IsNullOrEmpty(pidParam.ServerPort)
                    || string.IsNullOrEmpty(pidParam.Office))
                {
                    result = "错误信息:服务器IP,端口和Office不能为空！";
                }
                else
                {
                    int port = 350;
                    if (!int.TryParse(pidParam.ServerPort, out port))
                    {
                        result = "错误信息:服务器端口错误，必须为数字！";
                    }
                }
                if (string.IsNullOrEmpty(result))
                {
                    pidParam.UserName = pidParam.UserName == null ? "" : pidParam.UserName;
                    sbLog.Append("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n");
                    sbLog.Append("Office:" + pidParam.Office + "\r\n");
                    sbLog.Append("IP:" + pidParam.ServerIP + ":" + pidParam.ServerPort + "\r\n");
                    sbLog.Append("Code:" + pidParam.UserName + "\r\n");

                    pm.ServerIP = pidParam.ServerIP;
                    pm.ServerPort = int.Parse(pidParam.ServerPort);
                    pm.Office = pidParam.Office;
                    pm.code = pidParam.Cmd;
                    pm.IsPn = pidParam.IsPn;
                    pm.IsAllResult = pidParam.IsAllResult;
                    pm.IsUseExtend = pidParam.IsUseExtend;
                    pm.ExtendData = pidParam.ExtendData;
                    pm.WebUserName = pidParam.UserName;
                    pm.IsHandResult = pidParam.IsHandResult;
                    sbLog.Append("发送:" + pm.code + "\r\n");
                    pidParam.RecvData = SendNewPID.SendCommand(pm);
                    sbLog.Append("接收:\r\n" + pidParam.RecvData + "\r\n");
                }
            }
            catch (Exception ex)
            {
                pidParam.RecvData = ex.Message;
                sbLog.Append("错误信息:" + ex.Message + "\r\n");
            }
            finally
            {
                sbLog.Append("END=====================================\r\n\r\n");
                PnrAnalysis.LogText.LogWrite(sbLog.ToString(), (string.IsNullOrEmpty(pidParam.LogDir) ? "webPid" : pidParam.LogDir));
            }
            return pidParam.RecvData;
        }
        /// <summary>
        /// 指定商户 发送指令
        /// </summary>
        /// <param name="businessmanCode">分销商户号</param>
        /// <param name="cmd">发送指令</param>
        /// <param name="Office">使用的Office</param>
        /// <param name="ExtendData">扩展</param>
        /// <param name="isRT">是否RT指令</param>
        /// <param name="isUseExtend">是否使用指定的扩展数据</param>
        /// <returns></returns>        
        public string SendCmd(string businessmanCode, string cmd, string Office, string ExtendData = "", bool isRT = false, bool isUseExtend = false)
        {
            string result = string.Empty;
            //判断商户号是否存在            
            var businessman = this.businessmanRepository.FindAll(p => p.Code == businessmanCode).FirstOrDefault();
            Buyer buyer = businessman as Buyer;
            Carrier carrier = businessman as Carrier;
            Supplier supplier = businessman as Supplier;
            string CarrierCode = string.Empty;
            if (buyer != null)
            {
                CarrierCode = buyer.CarrierCode;
            }
            else
            {
                if (carrier != null)
                {
                    CarrierCode = carrier.Code;
                }
                else if (supplier != null)
                {
                    CarrierCode = supplier.Code;
                }
            }
            if (!string.IsNullOrEmpty(CarrierCode))
            {
                result = SendCmdByCarrierCode(CarrierCode, cmd, Office, ExtendData, isRT, isUseExtend);
            }
            else
            {
                result = "商户号" + businessmanCode + "不存在";
            }
            return result.Replace("^", "\r");
        }

        /// <summary>
        /// 获取编码和票号信息
        /// </summary>    
        [ExtOperationInterceptor("获取编码和票号信息")]
        public string GetPnrAndTickeNumInfo(string businessmanCode, string pnrAndTicketNum, string ydOffice, bool isINF = false, bool isPAT = true)
        {
            string result = string.Empty;
            try
            {
                string cmd = string.Empty;
                //验证票号有效性
                string pattern = @"\d{3,4}(\-?|\s+)\d{10}";
                if (Regex.Match(pnrAndTicketNum, pattern, RegexOptions.IgnoreCase).Success)
                {
                    cmd = string.Format("DETR:TN/{0}", pnrAndTicketNum);
                    result = SendCmd(businessmanCode, cmd, ydOffice);
                }
                else
                {
                    if (format.IsPnr(pnrAndTicketNum))
                    {
                        cmd = string.Format("IG|RT{0}", pnrAndTicketNum);
                        result = SendCmd(businessmanCode, cmd, ydOffice, "", true);
                        if (!result.Contains("不存在") && !result.Contains("运营") && isPAT)
                        {
                            cmd = string.Format("RT{0}|PAT:A", pnrAndTicketNum);
                            result += "\r\n" + SendCmd(businessmanCode, cmd, ydOffice, "", false);
                            if (isINF)
                            {
                                cmd = string.Format("RT{0}|PAT:A*IN", pnrAndTicketNum);
                                result += "\r\n" + SendCmd(businessmanCode, cmd, ydOffice, "", false);
                            }
                        }
                    }
                    else
                    {
                        result = "编码格式错误！";
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.INFO, string.Format("[GetPnrAndTickeNumInfo]{0}\r\n{1}",
                   System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ex.Message));
            }
            return result;
        }
        /// <summary>
        /// 获取原始配置流量计数
        /// </summary>
        /// <param name="businessmanCode">分销商户号</param>
        /// <param name="FindConfigType">查找配置的方式  0.使用指定的Office均衡发送指令 1.使用指令的配置名发送指令 2.指定特定的用户发送</param>
        /// <param name="startDate">开始日期</param>
        /// <param name="endDate">结束日期</param>
        /// <param name="Office">Office</param>
        /// <param name="ConfigName">配置名</param>
        /// <returns></returns>
        [ExtOperationInterceptor("获取原始配置流量计数")]
        public UserFlow GetFlow(string carrierCode, int findConfigType, DateTime? startDate, DateTime? endDate, string office, string configName = "")
        {
            UserFlow flow = new UserFlow();
            //FindConfigType 查找配置的方式  0.使用指定的Office均衡发送指令 1.使用指令的配置名发送指令 2.指定特定的用户发送
            //IsSetting 控制用于获取和设置PID信息 默认否 用于发送指令
            //SettingCmd  用于获取或者设置的命令  格式:(命令)(键:值,键:值) 如获取流量:(GetFlow)(Find:ctu186)
            //Json  //传递json的键值对格式{a:b}
            List<string> optionList = new List<string>();
            if (startDate != null && startDate.HasValue)
            {
                optionList.Add("startDate:" + startDate.Value.ToString("yyyy-MM-dd"));
            }
            if (endDate != null && endDate.HasValue)
            {
                optionList.Add("endDate:" + endDate.Value.ToString("yyyy-MM-dd"));
            }
            if (!string.IsNullOrEmpty(configName))
            {
                optionList.Add("ConfigName:" + configName);
            }
            string options = string.Join(",", optionList.ToArray());
            string ExtentData = "<FindConfigType>" + findConfigType + "</FindConfigType><IsSetting>true</IsSetting><SettingCmd>(GetFlow)(" + options + ")</SettingCmd>";
            string strFlowData = SendCmdByCarrierCode(carrierCode, "", office, ExtentData, false, true);
            if (!string.IsNullOrEmpty(strFlowData))
            {
                string[] arrFlow = strFlowData.Split(new string[] { "^" }, StringSplitOptions.RemoveEmptyEntries);
                string[] strItemArr = null;
                int TotalFlow = 0;
                foreach (string item in arrFlow)
                {
                    //CTU324|32480|488|WEBGUEST|210.14.138.30|2014-05-28|WEB
                    strItemArr = item.Split(new string[] { "|" }, StringSplitOptions.None);
                    if (strItemArr != null && strItemArr.Length >= 7)
                    {
                        FlowData flowdata = new FlowData();
                        flowdata.Office = strItemArr[0];
                        flowdata.ConfigName = strItemArr[1];
                        int total = 0;
                        int.TryParse(strItemArr[2], out total);
                        TotalFlow += total;
                        flowdata.SendCount = total;
                        flowdata.UserName = strItemArr[3];
                        flowdata.ClientIP = strItemArr[4];
                        flowdata.UseDate = strItemArr[5];
                        flowdata.Source = strItemArr[6];
                        flow.FlowList.Add(flowdata);
                    }
                }
                flow.TotalFlow = TotalFlow;
            }
            return flow;
        }

        /// <summary>
        /// 自动授权
        /// </summary>
        /// <param name="businessmanCode">采购商户号</param>
        /// <param name="authOffice">授权的Office</param>
        /// <param name="office">预定的Office</param>
        /// <param name="pnr">编码</param>
        /// <returns></returns>
        [ExtOperationInterceptor("自动授权")]
        public bool AuthToOffice(string businessmanCode, string authOffice, string office, string pnr)
        {
            string cmd = string.Format("RT{0}|RMK TJ AUTH {1}|@", pnr, authOffice);
            string strRecvData = SendCmd(businessmanCode, cmd, office);
            string OutMsg = string.Empty;
            return format.INFMarkIsOK(strRecvData, out OutMsg);
        }
        /// <summary>
        /// 取消编码
        /// </summary>
        /// <param name="businessmanCode">采购商户号</param>
        /// <param name="office">预定的Office</param>
        /// <param name="pnr">取消的编码</param>
        /// <returns></returns>
        [ExtOperationInterceptor("取消编码")]
        public bool CancelPnr(string businessmanCode, string office, string pnr)
        {
            string cmd = string.Format("RT{0}|XEPNR@{0}", pnr, pnr);
            string strRecvData = SendCmd(businessmanCode, cmd, office);
            return strRecvData.Contains("CANCELLED");
        }
        /// <summary>
        /// 分离编码
        /// </summary>
        /// <param name="splitPnrParam">分离编码参数</param>
        /// <returns></returns>
        [ExtOperationInterceptor("分离编码")]
        public ResposeSplitPnrInfo SplitPnr(RequestSplitPnrInfo splitPnrParam)
        {
            string responseMsg = string.Empty;
            ResposeSplitPnrInfo response = new ResposeSplitPnrInfo();
            response.IsSUccess = false;
            response.OldPnr = splitPnrParam.Pnr;
            if (string.IsNullOrEmpty(splitPnrParam.Office) || string.IsNullOrEmpty(splitPnrParam.Pnr))
            {
                responseMsg = "分离的编码或者Office不能为空！";
            }
            else
            {
                if (splitPnrParam.SplitPasList == null || splitPnrParam.SplitPasList.Count == 0)
                {
                    responseMsg = "分离的乘客数据不能为空！";
                }
                else
                {
                    foreach (SplitPassenger item in splitPnrParam.SplitPasList)
                    {
                        if (string.IsNullOrEmpty(item.PassengerName))
                        {
                            responseMsg = "分离的乘客姓名不能为空！";
                            break;
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(responseMsg))
            {
                //提取编码信息转向实体
                string cmd = string.Format("RT{0}", splitPnrParam.Pnr);
                string strRecvData = SendCmd(splitPnrParam.BusinessmanCode, cmd, splitPnrParam.Office);
                if (strRecvData.Contains("授权"))
                {
                    responseMsg = splitPnrParam.Pnr + "提取信息失败:" + strRecvData;
                }
                else
                {
                    string ErrMsg = string.Empty;
                    PnrModel model = format.GetPNRInfo(splitPnrParam.Pnr, strRecvData, false, out ErrMsg);
                    if (model != null)
                    {
                        if (splitPnrParam.SplitPasList.Count <= 0)
                        {
                            responseMsg = "分离的乘客姓名不能为空！";
                        }
                        else
                        {
                            if (model._PassengerList.Count <= 1)
                            {
                                responseMsg = string.Format("{0}中的乘客少于一人,分离失败！", splitPnrParam.Pnr);
                            }
                            else
                            {
                                List<string> spList = new List<string>();
                                List<string> noPasNameList = new List<string>();
                                string passengerName = string.Empty;
                                //补票号需要用的票号
                                string TicketNumber = splitPnrParam.SplitPasList.Where(p => !string.IsNullOrEmpty(p.TicketNumber)).Select(p => p.TicketNumber).FirstOrDefault();
                                //查找需要分离的乘客在编码中的编码项序号
                                for (int i = 0; i < splitPnrParam.SplitPasList.Count; i++)
                                {
                                    SplitPassenger item = splitPnrParam.SplitPasList[i];
                                    #region 名字处理---------------
                                    passengerName = item.PassengerName.ToUpper().Trim();
                                    //处理儿童姓名问题
                                    if (PinYingMange.IsChina(passengerName))
                                    {
                                        if (passengerName.EndsWith("CHD"))
                                        {
                                            passengerName = passengerName.Substring(0, passengerName.LastIndexOf("CHD"));
                                        }
                                    }
                                    else
                                    {
                                        if (passengerName.EndsWith(" CHD"))
                                        {
                                            passengerName = passengerName.Substring(0, passengerName.LastIndexOf(" CHD"));
                                        }
                                    }
                                    #endregion ---------------

                                    //比较乘客姓名 获取分离乘客序号
                                    PassengerInfo _PassengerInfo = model._PassengerList.Where(p => p.PassengerName.ToUpper().Trim() == passengerName).FirstOrDefault();
                                    if (_PassengerInfo != null)
                                    {
                                        spList.Add(_PassengerInfo.SerialNum);
                                    }
                                    else
                                    {
                                        //编码中不存在的乘客
                                        noPasNameList.Add(item.PassengerName);
                                    }
                                }
                                if (noPasNameList.Count > 0)
                                {
                                    responseMsg = string.Format("分离的乘客{0}在编码{1}中不存在!", string.Join(",", noPasNameList.ToArray()), splitPnrParam.Pnr);
                                }
                                else
                                {
                                    //进行分离
                                    if (spList.Count > 0)
                                    {
                                        #region xe 在补票号 在分离
                                        //不含有TL项时用\KI强制封口
                                        string FK = @"\\KI";
                                        //含有TL项时用@封口
                                        if (!string.IsNullOrEmpty(model._Other.StrTL))
                                        {
                                            //如果是出票了的 分离编码需要删掉TL项 添加分离乘客的票号 再分离
                                            if (model.PnrStatus.Contains("RR") && model._Other.TKTL != null && !string.IsNullOrEmpty(TicketNumber))
                                            {
                                                #region //xe出票时限
                                                cmd = string.Format("rt{0}|xe{1}|TKT/{2}|@", splitPnrParam.Pnr, model._Other.TKTL.SerialNum, TicketNumber);
                                                strRecvData = SendCmd(splitPnrParam.BusinessmanCode, cmd, splitPnrParam.Office);
                                                #endregion
                                            }
                                            FK = "@";
                                        }
                                        #endregion
                                        //分离编码指令
                                        cmd = string.Format("rt{0}|sp{1}|{2}", splitPnrParam.Pnr, string.Join("/", spList.ToArray()), FK);
                                        strRecvData = SendCmd(splitPnrParam.BusinessmanCode, cmd, splitPnrParam.Office);
                                        //获取分离后的返回数据 解析出新编码
                                        string newPnr = format.GetSplitPnr(splitPnrParam.Pnr, strRecvData, out ErrMsg);
                                        //解析出来的新编码和原编码不一样时
                                        if (newPnr != splitPnrParam.Pnr)
                                        {
                                            //验证编码格式
                                            if (format.IsPnr(newPnr))
                                            {
                                                //KI封口失败在用 @尝试一次
                                                if (strRecvData.Contains("MRT:") && FK == @"\\KI" && newPnr == "")
                                                {
                                                    FK = "@";
                                                    cmd = string.Format("rt{0}|sp{1}|{2}", splitPnrParam.Pnr, string.Join("/", spList.ToArray()), FK);
                                                    strRecvData = SendCmd(splitPnrParam.BusinessmanCode, cmd, splitPnrParam.Office);
                                                    newPnr = format.GetSplitPnr(splitPnrParam.Pnr, strRecvData, out ErrMsg);
                                                }
                                                //验证编码格式
                                                if (format.IsPnr(newPnr))
                                                {
                                                    //分离成功
                                                    response.IsSUccess = true;
                                                    response.NewPnr = newPnr;
                                                }
                                                else
                                                {
                                                    //分离失败
                                                    responseMsg = strRecvData;
                                                }
                                            }
                                            else
                                            {
                                                //分离解析失败
                                                responseMsg = strRecvData;
                                            }
                                        }
                                        else
                                        {
                                            //分离解析失败
                                            responseMsg = string.Format("新编码【{0}】原编码{1}一样，不分离:{2}", newPnr, splitPnrParam.Pnr, strRecvData);
                                        }
                                    }
                                    else
                                    {
                                        responseMsg = "未找到分离的乘客在编码中的编号！";
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        responseMsg = splitPnrParam.Pnr + "解析乘客失败！";
                    }
                }
            }
            //抛出异常信息
            if (!string.IsNullOrEmpty(responseMsg))
            {
                throw new OrderCommException(responseMsg);
            }
            return response;
        }
        /// <summary>
        /// 票号的挂起解挂 
        /// </summary>
        /// <param name="ticketSuppendRequest"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("票号的挂起解挂")]
        public TicketSuppendResponse TicketNumberLock(TicketSuppendRequest ticketSuppendRequest)
        {
            TicketSuppendResponse response = new TicketSuppendResponse();
            string cmd = string.Format("TSS:TN/{0}/{1}", ticketSuppendRequest.TicketNumber, (ticketSuppendRequest.TicketNumberOpType == TicketNumberOpType.Suppend ? "S" : "B"));
            string strRecvData = SendCmd(ticketSuppendRequest.BusinessmanCode, cmd, ticketSuppendRequest.Office);
            response.Remark = strRecvData;
            if (strRecvData.Trim().ToUpper().Contains("ACCEPTED"))
            {
                response.Result = true;
            }
            else
            {
                response.Result = false;
            }
            return response;
        }
        /// <summary>
        /// 修改乘客证件号
        /// </summary>
        /// <param name="ssrUpdateRequest"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("修改乘客证件号")]
        public SsrUpdateResponse UpdateSsr(SsrUpdateRequest ssrUpdateRequest)
        {
            SsrUpdateResponse response = new SsrUpdateResponse();
            try
            {
                string cmd = string.Format("RT{0}", ssrUpdateRequest.Pnr);
                string recvData = SendCmd(ssrUpdateRequest.BusinessmanCode, cmd, ssrUpdateRequest.Office);
                if (recvData.Contains("授权"))
                {
                    response.Recvdata = recvData;
                    response.Remark = string.Format("{0}需要授权：RMK TJ AUTH {1}", ssrUpdateRequest.Pnr, ssrUpdateRequest.Office);
                }
                else
                {
                    if (ssrUpdateRequest.DicSsrData.Count == 0)
                    {
                        response.Recvdata = recvData;
                        response.Remark = "修改的乘客信息不能为空！";
                    }
                    else
                    {
                        string errMsg = string.Empty;
                        PnrModel pnrModel = format.GetPNRInfo(ssrUpdateRequest.Pnr, recvData, false, out errMsg);
                        if (pnrModel != null && errMsg == "")
                        {
                            //编码没有取消
                            if (!pnrModel.PnrStatus.Contains("XX"))
                            {
                                //需要修改的乘客是否不存在编码中
                                bool IsNotExist = false;
                                List<string> pasList = new List<string>();
                                foreach (KeyValuePair<string, string> item in ssrUpdateRequest.DicSsrData)
                                {
                                    PassengerInfo pas = pnrModel._PassengerList.Where(p => p.PassengerName.Trim().ToUpper() == item.Key.Trim().ToUpper()
                                        || p.YinToINFTName.Trim().ToUpper() == item.Key.Trim().ToUpper()
                                        ).FirstOrDefault();
                                    if (pas == null)
                                    {
                                        IsNotExist = true;
                                        pasList.Add(item.Key);
                                        break;
                                    }
                                }
                                if (!IsNotExist)
                                {
                                    response.Recvdata = recvData;
                                    response.Remark = string.Format("修改的乘客姓名[{0}]编码{1}中不存在！", string.Join(",", pasList.ToArray()), ssrUpdateRequest.Pnr);
                                }
                                else
                                {
                                    string pnr = ssrUpdateRequest.Pnr;
                                    string carryCode = ssrUpdateRequest.CarryCode;
                                    string passengerName = string.Empty;
                                    string newCid = string.Empty;
                                    //循环修改
                                    foreach (KeyValuePair<string, string> item in ssrUpdateRequest.DicSsrData)
                                    {
                                        PassengerInfo pas = pnrModel._PassengerList.Where(p => p.PassengerName.Trim().ToUpper() == item.Key.Trim().ToUpper()
                                            || p.YinToINFTName.Trim().ToUpper() == item.Key.Trim().ToUpper()
                                            ).FirstOrDefault();
                                        if (pas != null)
                                        {
                                            //修改的乘客姓名
                                            passengerName = item.Key.Trim();
                                            //新证件号
                                            newCid = item.Value.Trim();
                                            if (pas.PassengerType != "3")
                                            {
                                                //成人或者儿童 存在证件号删除再添加
                                                if (recvData.ToUpper().Contains("SSR FOID"))//修改
                                                {
                                                    //删除                            
                                                    cmd = string.Format("RT{0}|XE{1}|@", pnr, pas.SsrCardIDSerialNum);
                                                    recvData = SendCmd(ssrUpdateRequest.BusinessmanCode, cmd, ssrUpdateRequest.Office);
                                                    //补入
                                                    cmd = "RT" + pnr + "|SSR FOID " + carryCode + " HK/NI" + newCid + "/P" + pas.SerialNum + "|@";
                                                    recvData = SendCmd(ssrUpdateRequest.BusinessmanCode, cmd, ssrUpdateRequest.Office);
                                                }
                                                else//添加
                                                {
                                                    cmd = "RT" + pnr + "|SSR FOID " + carryCode + " HK/NI" + newCid + "/P" + pas.SerialNum + "|@";
                                                    recvData = SendCmd(ssrUpdateRequest.BusinessmanCode, cmd, ssrUpdateRequest.Office);
                                                }
                                                //验证是否成功
                                                if (format.INFMarkIsOK(recvData, out errMsg))
                                                {
                                                    response.Status = true;
                                                }
                                            }
                                            else
                                            {
                                                //婴儿 证件号为日期或者非日期
                                                string Pattern = @"^(19|20)\d{2}\-\d{2}\-\d{2}$";
                                                if (Regex.IsMatch(newCid, Pattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                                                {
                                                    //修改婴儿证件号
                                                    if (recvData.ToUpper().Contains("SSR INFT"))
                                                    {
                                                        //婴儿
                                                        //rtHS6PQ2|xe8|SSR INFT 3U NN1 Zhang/Ming 08sep12/p1/s2|@
                                                        cmd = string.Format("RT{0}|XE{1}|SSR INFT {2} NN1 {3} {4}/p{5}/s{6}|@", pnr, pas.YinToINFTNum, carryCode, pas.YinToINFTName, FormatPNR.DateToStr(newCid, DataFormat.dayMonthYear), pas.YinToAdltNum, pas.YinToLegNum);
                                                        recvData = SendCmd(ssrUpdateRequest.BusinessmanCode, cmd, ssrUpdateRequest.Office);
                                                    }
                                                    else //添加婴儿证件号
                                                    {
                                                        PnrAnalysis.Model.LegInfo Leg = pnrModel._LegList[0];
                                                        string pinyin = "";
                                                        if (PinYingMange.IsChina(passengerName.Trim()))
                                                        {
                                                            pinyin = PinYingMange.GetSpellByChinese(passengerName.Trim().Substring(0, 1)) + "/" + PinYingMange.GetSpellByChinese(PinYingMange.RepacePinyinChar(passengerName.Trim().Substring(1)));
                                                        }
                                                        else
                                                        {
                                                            pinyin = passengerName.Trim();
                                                        }
                                                        //rtHS6PQ2|XN:IN/张明INF(sep12)/p1  
                                                        //SSR INFT 3U NN1 CTUCAN 8737 E 31OCT Zhang/Ming 02sep12/p1/s2
                                                        //@&CTU303
                                                        //RTJDQPVZ|XN:IN/王宇INF(jan08)/p1^SSR INFT 3U NN1 CTUPEK 3U8881 Y 31jan Wang/Yu 02jan08/P1/S2^@
                                                        StringBuilder sbSendIns = new StringBuilder();
                                                        sbSendIns.AppendFormat("RT{0}|XN:IN/{1}INF({2})/p{3}\r", pnr, passengerName.Trim(), FormatPNR.DateToStr(newCid, DataFormat.monthYear), pas.YinToAdltNum);
                                                        sbSendIns.AppendFormat("SSR INFT {0} NN1 {1} {2} {3} {4} {5} {6}/P{7}/S{8}\r", carryCode, Leg.FromCode + Leg.ToCode, Leg.AirCode.Replace("*", "") + Leg.FlightNum, Leg.Seat, FormatPNR.DateToStr(Leg.FlyDate1, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(newCid, DataFormat.dayMonthYear), pas.YinToAdltNum, Leg.SerialNum);
                                                        sbSendIns.Append("@");

                                                        cmd = sbSendIns.ToString();
                                                        recvData = SendCmd(ssrUpdateRequest.BusinessmanCode, cmd, ssrUpdateRequest.Office);
                                                    }
                                                    //验证是否成功
                                                    if (format.INFMarkIsOK(recvData, out errMsg))
                                                    {
                                                        response.Status = true;
                                                    }
                                                }
                                                else
                                                {
                                                    response.Recvdata = recvData;
                                                    response.Remark = string.Format("{0}编码中,婴儿证件号必须为年-月-日格式!", ssrUpdateRequest.Pnr);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                response.Recvdata = recvData;
                                response.Remark = string.Format("{0}编码状态为:{1},不能修改证件号！", ssrUpdateRequest.Pnr, pnrModel.PnrStatus);
                            }
                        }
                        else
                        {
                            response.Recvdata = recvData;
                            response.Remark = string.Format("提取编码({0})信息失败！", ssrUpdateRequest.Pnr);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.INFO, string.Format("[UpdateSsr]{0}\r\n{1}",
                  System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), ex.Message));
            }
            return response;
        }

        /// <summary>
        /// 儿童编码备注成人编码信息
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="chdPnr"></param>
        /// <param name="adultPnr"></param>
        /// <param name="CarryCode"></param>
        /// <param name="Office"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("儿童编码备注成人编码信息")]
        public void ChdRemarkAdultPnr(string businessmanCode, string chdPnr, string adultPnr, string CarryCode, string Office)
        {
            string cmd = string.Format("RT{0}|SSR OTHS {1} ADULT PNR IS {2}|@",
                chdPnr.ToUpper(), CarryCode, adultPnr.ToUpper());
            string recvData = SendCmd(businessmanCode, cmd, Office);
        }

        public List<string> GetOffice(string carrierCode)
        {
            List<string> pids = new List<string>();
            Carrier carrier = this.businessmanRepository.FindAll(p => p.Code == carrierCode).FirstOrDefault() as Carrier;
            if (!(carrier == null || carrier.Pids == null || carrier.Pids.Count == 0))
            {
                pids.AddRange(carrier.Pids.Select(p => p.Office.ToUpper()).OfType<string>().ToList());
            }
            return pids;
        }

        /// <summary>
        /// 判断编码是否可以取消 true可以取消    false不能取消（出票的不能取消）
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="office"></param>
        /// <param name="pnr"></param>
        /// <returns></returns>        
        [ExtOperationInterceptor("判断编码是否可以取消")]
        public bool CanCancel(string businessmanCode, string office, string pnr)
        {
            bool _CanCancel = false;
            string strPnrContent = GetPnrAndTickeNumInfo(businessmanCode, pnr, office);
            string strErr = string.Empty;
            string PnrStatus = format.GetPnrStatus(strPnrContent, out strErr);
            if (!PnrStatus.Contains("RR")
                && !PnrStatus.Contains("XX")
                && PnrStatus != "")
            {
                if (
                    PnrStatus.ToUpper().Contains("HK")
                    || PnrStatus.ToUpper().Contains("HN")
                    || PnrStatus.ToUpper().Contains("HL")
                    || PnrStatus.ToUpper().Contains("NO")
                    )
                {
                    _CanCancel = true;
                }
            }
            return _CanCancel;
        }
        /// <summary>
        /// AV查航班
        /// </summary>
        /// <param name="code"></param>
        /// <param name="fromCode"></param>
        /// <param name="toCode"></param>
        /// <param name="airCode"></param>
        /// <param name="flyDate"></param>
        /// <param name="flyTime"></param>
        /// <returns></returns>        
        [ExtOperationInterceptor("使用代理人配置查航班")]
        public string GetAV(string code, string fromCode, string toCode, string airCode, string flyDate, string flyTime)
        {
            FlightService flightService = ObjectFactory.GetInstance<FlightService>();
            string result = flightService.GetAV(code, fromCode, toCode, airCode, flyDate, flyTime);
            return result;
        }

        /// <summary>
        /// 添加婴儿
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="adultPnr"></param>
        /// <param name="infName"></param>
        /// <param name="birthDay"></param>
        /// <param name="carryCode"></param>
        /// <param name="dblCityCode"></param>
        /// <param name="flightCode"></param>
        /// <param name="seat"></param>
        /// <param name="flyStartDate"></param>
        /// <param name="adultNum"></param>
        /// <param name="skyNum"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("编码备注添加婴儿")]
        public bool AddINF(string businessmanCode, string adultPnr, string infName, string birthDay, string carryCode, string dblCityCode, string flightCode, string seat, string flyStartDate, string adultNum, string skyNum)
        {
            bool IsAddSuc = false;
            string pinyin = string.Empty;
            string errMsg = string.Empty;
            if (PinYingMange.IsChina(infName))
            {
                pinyin = PinYingMange.GetSpellByChinese(infName.Substring(0, 1)) + "/" + PinYingMange.GetSpellByChinese(PinYingMange.RepacePinyinChar(infName.Substring(1)));
            }
            else
            {
                pinyin = infName;
            }
            StringBuilder sbInfRmk = new StringBuilder();
            sbInfRmk.AppendFormat("RT{0}|XN:IN/{1}INF({2})/p{3}\r", adultPnr, infName, FormatPNR.DateToStr(birthDay, DataFormat.monthYear), adultNum);
            sbInfRmk.AppendFormat("SSR INFT {0} NN1 {1} {2} {3} {4} {5} {6}/P{7}/S{8}\r", carryCode, dblCityCode, carryCode.Replace("*", "") + flightCode, seat.Substring(0, 1), FormatPNR.DateToStr(flyStartDate, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(birthDay, DataFormat.dayMonthYear), adultNum, skyNum);
            sbInfRmk.Append("@");
            //发送指令
            string recvData = SendCmd(businessmanCode, sbInfRmk.ToString(), "", "").Replace("^", "\r");
            //失败重发
            if (!format.INFMarkIsOK(recvData, out errMsg))
            {
                sbInfRmk = new StringBuilder();
                sbInfRmk.AppendFormat("RT{0}|XN:IN/{1}INF({2})/p{3}\r", adultPnr, infName, FormatPNR.DateToStr(birthDay, DataFormat.monthYear), adultNum);
                sbInfRmk.AppendFormat("SSR INFT {0} NN1 {1} {2} {3} {4} {5} {6}/P{7}/S{8}\r", carryCode, dblCityCode, carryCode.Replace("*", "") + flightCode, seat.Substring(0, 1), FormatPNR.DateToStr(flyStartDate, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(birthDay, DataFormat.dayMonthYear), adultNum, skyNum);
                sbInfRmk.Append(@"\ki");
                recvData = SendCmd(businessmanCode, sbInfRmk.ToString(), "", "").Replace("^", "\r");
            }
            //检查
            IsAddSuc = format.INFMarkIsOK(recvData, out errMsg);
            return IsAddSuc;
        }

        /// <summary>
        /// 获取对应的PAT价格数据
        /// </summary>
        /// <param name="businessmanCode"></param>
        /// <param name="Pnr"></param>
        /// <param name="passengerType"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("获取对应编码的PAT价格数据")]
        public PatModel sendPat(string businessmanCode, string Pnr, EnumPassengerType passengerType)
        {
            PatModel patModel = null;
            string strPatCmd = string.Empty;
            string strPATContent = string.Empty;
            string strMsg = string.Empty;
            if (passengerType == EnumPassengerType.Adult)
            {
                strPatCmd = string.Format("RT{0}|PAT:A", Pnr);
            }
            else if (passengerType == EnumPassengerType.Child)
            {
                strPatCmd = string.Format("RT{0}|PAT:A*CH", Pnr);
            }
            else if (passengerType == EnumPassengerType.Baby)
            {
                strPatCmd = string.Format("RT{0}|PAT:A*IN", Pnr);
            }
            strPATContent = SendCmd(businessmanCode, strPatCmd, "", "").Replace("^", "\r");
            if (!string.IsNullOrEmpty(strPATContent))
            {
                patModel = format.GetPATInfo(strPATContent, out strMsg);
            }
            if (patModel == null || patModel.PatList.Count == 0)
            {
                throw new OrderCommException(string.Format("为获取到PAT信息，返回数据:", strPATContent));
            }
            return patModel;
        }
        /// <summary>
        /// 根据航段获取运价信息
        /// </summary>
        /// <param name="businessmanCode"></param>        
        /// <param name="airCode">MU</param>
        /// <param name="flightNo">7341</param>
        /// <param name="seat">Y</param>
        /// <param name="flyDate">2014-09-30</param>
        /// <param name="fromCode">CTU</param>
        /// <param name="toCode">PEK</param>
        /// <param name="startTime">07:00</param>
        /// <param name="endTime">09:00</param>
        /// <returns></returns>
        [ExtOperationInterceptor("根据航段获取运价信息")]
        public List<PatInfo> sendSSPat(string businessmanCode, string airCode, string flightNo, string seat, string flyDate, string fromCode, string toCode, string startTime, string endTime)
        {
            PatModel patModel = null;
            string strMsg = string.Empty;
            string strPatCmd = string.Format("IG|SS {0}{1}/{2}/{3}/{4}{5}NN1/{6} {7}|PAT:A", airCode, flightNo, seat, FormatPNR.DateToStr(flyDate, DataFormat.dayMonth), fromCode, toCode, startTime.Replace(":", ""), endTime.Replace(":", ""));
            string strPATContent = SendCmd(businessmanCode, strPatCmd, "", "").Replace("^", "\r");
            if (!string.IsNullOrEmpty(strPATContent))
            {
                patModel = format.GetPATInfo(strPATContent, out strMsg);
            }
            if (patModel == null || patModel.PatList.Count == 0)
            {
                //throw new OrderCommException(string.Format("未获取到价格，错误信息：{0}", strPATContent));
            }
            return patModel.UninuePatList;
        }
        public List<PIDInfo> GetPids()
        {
            string sql = "select IP,Port, Office, Carrier_Code CarrierCode, Supplier_Code SupplierCode from dbo.PID";
            BusinessmanRepository businessmanRepository = ObjectFactory.GetInstance<BusinessmanRepository>();
            var result = businessmanRepository.SqlQuery(sql, typeof(PIDInfo)).Cast<PIDInfo>();
            List<PIDInfo> pids = new List<PIDInfo>();
            Dictionary<string, PIDInfo> dic = new Dictionary<string, PIDInfo>();
            string key = string.Empty;
            result.ToList().ForEach(pid =>
            {
                string code = !string.IsNullOrEmpty(pid.CarrierCode) ? pid.CarrierCode : pid.SupplierCode;
                key = string.Format("{0}{1}{2}{3}", pid.IP, pid.Port, pid.Office.ToUpper(), (string.IsNullOrEmpty(code) ? "" : code));
                if (!dic.ContainsKey(key))
                {
                    dic.Add(key, pid);
                    pids.Add(pid);
                }
            });
            dic.Clear();
            return pids;
        }

        /// <summary>
        /// 发送QT指令获取航变信息
        /// </summary>
        /// <returns></returns>
        [ExtOperationInterceptor("发送QT指令获取航变信息")]
        public QTResponse SendQT(PIDInfo pid)
        {
            QTResponse response = new QTResponse();
            StringBuilder sbLog0 = new StringBuilder();
            try
            {
                bool qtTest = false, qtClose = true;
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["qtTest"], out qtTest);
                bool.TryParse(System.Configuration.ConfigurationManager.AppSettings["qtClose"], out qtClose);
                if (qtClose)
                    return response;//暂时屏蔽
                if (qtTest)
                {
                    #region 测试

                    response.Code = !string.IsNullOrEmpty(pid.CarrierCode) ? pid.CarrierCode : pid.SupplierCode;
                    response.QTDate = System.DateTime.Now;
                    List<string> TestList = GetTestDataList(response.Code);
                    if (TestList.Count > 0)
                    {
                        response.QTResult = TestList[0];
                        if (TestList.Count > 1)
                        {
                            List<string> qnList = TestList.GetRange(1, TestList.Count - 1);
                            QT_Queue QT = format.FormatQT(response.QTResult);
                            response.Office = QT.Office;
                            //查询SC项 航变项
                            var sc = QT.QTList.Where(p => p.QT_Type == PnrAnalysis.Model.QT_Queue.QT_Type.SC).FirstOrDefault();
                            if (sc != null && sc.CapacityNumber > 0)
                            {
                                response.QnCount = sc.CapacityNumber;
                                response.Number = sc.Capacity;
                            }
                            foreach (var recv in qnList)
                            {
                                string errMsg = string.Empty;
                                string pnr = format.GetPNR(recv, out errMsg);
                                string ctct = format.GetCTCT(recv);
                                if (!string.IsNullOrEmpty(pnr))
                                {
                                    response.QnList.Add(new QnItem()
                                    {
                                        Pnr = pnr,
                                        CTCT = ctct,
                                        QnResult = recv
                                    });
                                }
                            }
                        }
                    }

                    #endregion
                }
                else
                {
                    #region 正式
                    StringBuilder sbLog = new StringBuilder();
                    try
                    {
                        sbLog.Append("START=====================================\r\n");
                        //商户Code
                        response.Code = !string.IsNullOrEmpty(pid.CarrierCode) ? pid.CarrierCode : pid.SupplierCode;
                        sbLog.AppendFormat("时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        sbLog.AppendFormat("Office:{0}\r\n", pid.Office);
                        sbLog.AppendFormat("IP:{0}:{1}\r\n", pid.IP, pid.Port);
                        sbLog.AppendFormat("Code:{0}\r\n", response.Code);

                        FormatPNR format = new FormatPNR();
                        PidParam pm = new PidParam();
                        pm.ServerIP = pid.IP;
                        pm.ServerPort = pid.Port.ToString();
                        pm.Office = pid.Office;
                        pm.IsPn = false;
                        pm.IsAllResult = true;
                        pm.IsUseExtend = false;
                        pm.UserName = response.Code;
                        pm.IsHandResult = true;
                        //pm.LogDir = pm.UserName;
                        //发送QT指令
                        pm.Cmd = "QT";
                        response.QTDate = System.DateTime.Now;
                        sbLog.AppendFormat("时间:{0}\r\n发送:{1}\r\n", response.QTDate.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.Cmd);
                        string strRecv = SendPid(pm);
                        sbLog.AppendFormat("时间:{0}接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), strRecv);
                        response.QTResult = strRecv;
                        QT_Queue QT = format.FormatQT(strRecv);
                        //查询SC项 航变项
                        var sc = QT != null ? QT.QTList.Where(p => p.QT_Type == PnrAnalysis.Model.QT_Queue.QT_Type.SC).FirstOrDefault() : null;
                        if (sc != null && sc.CapacityNumber > 0)
                        {
                            response.Office = QT.Office;
                            response.QnCount = sc.CapacityNumber;
                            response.Number = sc.Capacity;
                            //发送QN指令
                            pm.Cmd = "QN";
                            for (int i = 0; i < sc.CapacityNumber; i++)
                            {
                                sbLog.AppendFormat("时间:{0}\r\n发送[{1}]:{2}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), (i + 1), pm.Cmd);
                                string recv = SendPid(pm);
                                sbLog.AppendFormat("接收[0]:\r\n{1}\r\n", (i + 1), recv);
                                string errMsg = string.Empty;
                                string pnr = format.GetPNR(recv, out errMsg);
                                string ctct = format.GetCTCT(recv);
                                if (!string.IsNullOrEmpty(pnr))
                                {
                                    response.QnList.Add(new QnItem()
                                    {
                                        Pnr = pnr,
                                        CTCT = ctct,
                                        QnResult = recv
                                    });
                                }
                                //延时
                                Thread.Sleep(50);
                            }
                            //获取CTCT
                            pm.IsPn = true;
                            for (int i = 0; i < response.QnList.Count; i++)
                            {
                                var item = response.QnList[i];
                                if (string.IsNullOrEmpty(item.CTCT))
                                {
                                    pm.Cmd = string.Format("RT{0}", item.Pnr);
                                    sbLog.AppendFormat("时间:{0}\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.Cmd);
                                    string recv = SendPid(pm);
                                    sbLog.AppendFormat("接收:\r\n{0}\r\n", recv);
                                    response.QnList[i].CTCT = format.GetCTCT(recv);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        sbLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
                    }
                    finally
                    {
                        sbLog.Append("END=====================================\r\n\r\n");
                        PnrAnalysis.LogText.LogWrite(sbLog.ToString(), string.Format(@"QT\{0}", response.Code));
                    }
                    #endregion
                }
            }
            catch (Exception ex)
            {
                sbLog0.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
            }
            finally
            {
                if (!string.IsNullOrEmpty(sbLog0.ToString()))
                    PnrAnalysis.LogText.LogWrite(sbLog0.ToString(), "QT");
            }
            return response;
        }
        /// <summary>
        /// Open票扫描
        /// </summary>
        /// <param name="ip">配置的IP</param>
        /// <param name="port">配置的端口</param>
        /// <param name="office">Office</param>
        /// <param name="tkList">要扫面的票号列表</param>
        /// <param name="action">回调(每个票号的指令状况)</param>
        /// <returns></returns>
        [ExtOperationInterceptor("Open票扫描")]
        public OpenTicketResponse ScanOpenTicket(string ip, string port, string office, List<string> tkList, Action<TicketItem> action = null)
        {
            OpenTicketResponse response = new OpenTicketResponse();
            if (tkList != null && tkList.Count > 0)
            {
                FormatPNR pnrformat = new FormatPNR();
                PidParam pidParam = new PidParam();
                pidParam.ServerIP = ip;
                pidParam.ServerPort = port;
                pidParam.Office = office;
                pidParam.UserName = "Open票扫描";
                pidParam.LogDir = "ScanOpenTicket";
                string pattern = @"\d{3,4}(\-?|\s+)\d{10}";
                tkList.ForEach(p =>
                {
                    //验证票号的正确性
                    if (Regex.Match(p, pattern, RegexOptions.IgnoreCase).Success)
                    {
                        //去重复票号
                        TicketItem ti = response.OpenTKList.Where(p1 => p1.TKNumber.Replace("-", "").Trim() == p.Replace("-", "").Trim()).FirstOrDefault();
                        if (ti == null)
                        {
                            ti = new TicketItem();
                            pidParam.Cmd = string.Format("DETR:TN/{0}", p.Replace("-", "").Trim());
                            ti.TKNumber = p;
                            ti.DetrData = SendPid(pidParam);
                            //中断循环                        
                            //if (ti.DetrData.Contains("错误信息:")) return;
                            ti.TKStatus = pnrformat.GetTicketStatus(ti.DetrData);
                            response.OpenTKList.Add(ti);
                            if (action != null)
                            {
                                try { action(ti); }
                                catch { }
                            }
                        }
                    }
                });
            }
            return response;
        }

        private List<string> GetTestDataList(string code)
        {
            List<string> resultList = new List<string>();
            string filePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
            if (!filePath.EndsWith("\\")) { filePath = filePath + "\\"; }
            filePath += string.Format("qt_{0}.txt", code);
            if (File.Exists(filePath))
            {
                using (StreamReader srRead = new StreamReader(filePath, Encoding.Default))
                {
                    resultList.AddRange(srRead.ReadToEnd().Split(new string[] { "|######################################|" }, StringSplitOptions.None));
                    srRead.Close();
                }
            }
            return resultList;
        }
    }
}
