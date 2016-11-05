using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Xml;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using PnrAnalysis;
using PnrAnalysis.Model;
using JoveZhao.Framework.Helper;
using JoveZhao.Framework;
using System.Reflection;
using BPiaoBao.DomesticTicket.Platforms._Today;
using BPiaoBao.DomesticTicket.Platforms.webTodayOrder;
using BPiaoBao.DomesticTicket.Domain.Services.TodayObject;

namespace BPiaoBao.DomesticTicket.Platforms.PTInterface
{
    public class PTMange
    {
        /// <summary>
        /// true 关 false开
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="airCode"></param>
        /// <returns></returns>
        private bool B2BIsClose(string platform, string airCode)
        {
            bool isClose = false;
            //先看全局开关开了没有 默认开
            var result = SystemConsoSwitch.AirSystems.Where(p => p.AirCode.ToUpper() == airCode.ToUpper()).FirstOrDefault();
            if (result != null && !string.IsNullOrEmpty(airCode))
            {
                //如果全局开关是开的 再看具体的平台的航空公司B2B是否关闭
                if (result.IsB2B)
                {
                    var result1 = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode.ToUpper() == platform.ToUpper()).FirstOrDefault();
                    if (result1.B2B.ToUpper().Contains(airCode) || result1.B2B.ToUpper().Contains("ALL"))
                    {
                        isClose = true;
                    }
                }
                else
                {
                    isClose = true;
                }
            }
            else
            {
                //默认（开）数据处理
                var result1 = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode.ToUpper() == platform.ToUpper()).FirstOrDefault();
                if (result1 == null || result1.B2B.ToUpper().Contains(airCode) || result1.B2B.ToUpper().Contains("ALL"))
                {
                    isClose = true;
                }
            }
            return isClose;
        }
        /// <summary>
        /// true 关 false开
        /// </summary>
        /// <param name="platform"></param>
        /// <param name="airCode"></param>
        /// <returns></returns>
        private bool BSPIsClose(string platform, string airCode)
        {
            bool isClose = false;
            //先看全局开关开了没有 默认开
            var result = SystemConsoSwitch.AirSystems.Where(p => p.AirCode.ToUpper() == airCode.ToUpper()).FirstOrDefault();
            if (result != null && !string.IsNullOrEmpty(airCode))
            {
                //如果全局开关是开的 再看具体的平台的航空公司B2B是否关闭
                if (result.IsBSP)
                {
                    var result1 = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode.ToUpper() == platform.ToUpper()).FirstOrDefault();
                    if (result1.BSP.ToUpper().Contains(airCode.ToUpper()) || result1.BSP.ToUpper().Contains("ALL"))
                    {
                        isClose = true;
                    }
                }
                else
                {
                    isClose = true;
                }
            }
            else
            {
                //默认（开）数据处理
                var result1 = SystemConsoSwitch.PlatSystems.Where(p => p.PlatfromCode.ToUpper() == platform.ToUpper()).FirstOrDefault();
                if (result1 == null || result1.BSP.ToUpper().Contains(airCode.ToUpper()) || result1.BSP.ToUpper().Contains("ALL"))
                {
                    isClose = true;
                }
            }
            return isClose;
        }
        class PMParam
        {
            public string BaseUrl = string.Empty;
            public int TimeOut = 60;
        }
        private PMParam GetPMParam
        {
            get
            {
                string baseUrl = string.Empty;
                int timeout = 60;
                try
                {
                    baseUrl = System.Configuration.ConfigurationManager.AppSettings["baseurl"];
                    int.TryParse(System.Configuration.ConfigurationManager.AppSettings["pmtimeout"], out timeout);
                }
                catch
                {
                    Logger.WriteLog(LogType.ERROR, "票盟请求地址未设置");
                }
                return new PMParam()
                {
                    BaseUrl = baseUrl,
                    TimeOut = timeout
                };
            }
        }




        #region 政策
        public DataSet _517GetPolicy(string _517Accout, string _517Password, string _517Ag, string IsLowerPrice, string PNRContent, PnrData pnrData)
        {
            DataSet dsPolicy = new DataSet();
            dsPolicy.DataSetName = "_517";
            DataSwitch m_switch = new DataSwitch();
            DataTable Message = m_switch.CreatePolicy("517");
            DataRow dr = Message.Rows[0];
            FormatPNR format = new FormatPNR();
            format.IsFilterCon = false;
            DataSwitch m_Switch = new DataSwitch();
            //文本日志
            StringBuilder sbLog = new StringBuilder();
            bool IsSuc = false;
            string strMessage = "";
            try
            {
                sbLog.Append(" 参数:_517Accout=" + _517Accout + "\r\n _517Password=" + _517Password + "\r\n _517Ag=" + _517Ag + "\r\n IsLowerPrice=" + IsLowerPrice + "\r\n PNRContent=" + PNRContent + "\r\n ");
                PNRContent = PNRContent.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "").Replace("\r", "");
                sbLog.AppendFormat("开始解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                PnrAnalysis.PnrModel pnrMode = null;
                PnrAnalysis.PatModel patMode = null;
                PnrAnalysis.PatModel infPatMode = null;
                PnrAnalysis.Model.SplitPnrCon SPC = null;
                string Pnr = string.Empty;
                string RTCon = string.Empty;
                string PatCon = string.Empty;
                string Msg = "";
                if (pnrData != null)
                {
                    pnrMode = pnrData.PnrMode;
                    patMode = pnrData.PatMode;
                    infPatMode = pnrData.InfPatMode;
                    Pnr = pnrMode.Pnr;
                    RTCon = pnrMode._OldPnrContent.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "").Replace("\r", "");
                    PatCon = patMode.PatCon.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "").Replace("\r", "");
                }
                else
                {
                    SPC = format.GetSplitPnrCon(PNRContent);
                    RTCon = SPC.RTCon;
                    PatCon = SPC.AdultPATCon != string.Empty ? SPC.AdultPATCon : SPC.ChdPATCon;
                    Pnr = format.GetPNR(RTCon, out Msg);
                    pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
                    patMode = format.GetPATInfo(PatCon, out Msg);
                    RTCon = RTCon.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "").Replace("\r", "");
                    PatCon = PatCon.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "").Replace("\r", "").Replace(">", "");
                }
                dr["Pnr"] = Pnr;
                if (pnrMode == null || pnrMode._LegList.Count == 0)
                {
                    strMessage = "PNR内容航段数据解析失败！";
                }
                else
                {
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        dr["PnrType"] = pnrMode._PnrType.ToString();
                        if (patMode != null && patMode.PatList.Count > 0)
                        {
                            PatInfo pat = patMode.PatList[patMode.PatList.Count - 1];
                            if (IsLowerPrice == "1")//取低价
                            {
                                pat = patMode.PatList[0];
                            }
                            dr["SeatPrice"] = pat.Fare;
                            dr["ABFare"] = pat.TAX;
                            dr["RQFare"] = pat.RQFare;
                            dr["TotalPrice"] = pat.Price;
                            if (pnrMode.HasINF)
                            {
                                if (pnrData == null)
                                    infPatMode = format.GetPATInfo(SPC.INFPATCon, out Msg);

                                if (infPatMode != null && infPatMode.PatList.Count > 0)
                                {
                                    pat = infPatMode.PatList[infPatMode.PatList.Count - 1];
                                    if (IsLowerPrice == "1")//取低价
                                    {
                                        pat = infPatMode.PatList[0];
                                    }
                                    dr["INFSeatPrice"] = pat.Fare;
                                    dr["INFABFare"] = pat.TAX;
                                    dr["INFRQFare"] = pat.RQFare;
                                    dr["INFTotalPrice"] = pat.Price;
                                }
                                else
                                {
                                    strMessage = "婴儿PAT数据解析失败！";
                                }
                            }
                        }
                        else
                        {
                            strMessage = "PAT数据解析失败！";
                        }
                        sbLog.AppendFormat("解析结束时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            bool m_B2BIsClose = B2BIsClose(EnumPlatform._517.ToString().Replace("_", ""), pnrMode._CarryCode);
                            bool m_BSPIsClose = BSPIsClose(EnumPlatform._517.ToString().Replace("_", ""), pnrMode._CarryCode);
                            //B2B 和BSP都关了的话
                            if (m_B2BIsClose && m_BSPIsClose)
                            {
                                strMessage = "B2B和Bsp政策全部关闭";
                            }
                            else
                            {
                                if (string.IsNullOrEmpty(strMessage))
                                {
                                    sbLog.Append(" 解析参数:RTCon=" + RTCon + "\r\n PatCon=" + PatCon + "\r\n Pnr=" + Pnr + "\r\n Msg=" + Msg + "\r\n  ");
                                    sbLog.AppendFormat("开始获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                    //web517._517WebServiceSoapClient m_517 = new web517._517WebServiceSoapClient();
                                    //dsPolicy = m_517.GetBenefitDataPnrContent(_517Accout, _517Password, _517Ag, UrlEncode(RTCon), UrlEncode(PatCon), Pnr);
                                    dsPolicy = GetBenefitDataPnrContent(_517Accout, _517Password, _517Ag, RTCon, PatCon, Pnr);
                                    sbLog.AppendFormat("结束获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                                    IsSuc = dsPolicy.Tables.Count > 0 && dsPolicy.Tables.Contains("Datas") && dsPolicy.Tables[0].Rows.Count > 0;
                                    //过滤
                                    if (IsSuc)
                                    {
                                        sbLog.AppendFormat("开始过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                                        string StartDate = "", EndDate = "";
                                        StartDate = pnrMode._LegList[0].FlyDate1;
                                        EndDate = pnrMode._LegList[0].FlyDateE;
                                        if (pnrMode._LegList.Count > 1)
                                        {
                                            EndDate = pnrMode._LegList[1].FlyDateE;
                                        }
                                        string TravelType = "";
                                        if (pnrMode.TravelType == 1)
                                        {
                                            TravelType = "单程";
                                        }
                                        else if (pnrMode.TravelType == 2)
                                        {
                                            TravelType = "往返";
                                        }
                                        //单程/往返                 
                                        //TravelType                   
                                        string Where = "EffectDate<='" + StartDate + " 00:00:00' and ExpirationDate>='" + EndDate + " 23:59:59'";
                                        if (!string.IsNullOrEmpty(TravelType))
                                        {
                                            Where += " and (TravelType='" + TravelType + "' or TravelType='单程/往返')";
                                        }
                                        //控制高低价
                                        Where += string.Format(" and PMFee ={0} ", dr["SeatPrice"].ToString());
                                        //过滤B2B/BSP政策政策  
                                        if (m_B2BIsClose)
                                        {
                                            //关闭B2B
                                            Where += " and PolicyType<>2";
                                        }
                                        else if (m_BSPIsClose)
                                        {
                                            //关闭BSP
                                            Where += " and PolicyType=2";
                                        }
                                        DataTable tempTab = dsPolicy.Tables[0].Clone();
                                        tempTab.Columns["EffectDate"].DataType = typeof(DateTime);
                                        tempTab.Columns["ExpirationDate"].DataType = typeof(DateTime);
                                        DataRowCollection drs = dsPolicy.Tables[0].Rows;
                                        foreach (DataRow item in drs)
                                        {
                                            tempTab.ImportRow(item);
                                        }
                                        DataView dv = tempTab.DefaultView;
                                        dv.RowFilter = Where;
                                        dv.Sort = " Policy desc ";
                                        DataTable policy = dv.ToTable();
                                        dsPolicy.Tables.Clear();
                                        if (policy.Rows.Count > 0)
                                        {
                                            policy.TableName = "Policy";
                                            dr["Status"] = "T";
                                            dsPolicy = new DataSet();
                                            dsPolicy.DataSetName = "_517";
                                            dsPolicy.Tables.Add(policy);
                                            strMessage = "获取政策成功";
                                        }
                                        else
                                        {
                                            strMessage = "未获取到政策!";
                                        }
                                    }
                                    else
                                    {
                                        if (dsPolicy.Tables.Contains("error"))
                                        {
                                            strMessage = m_Switch.DataTableToString(dsPolicy.Tables["error"], "|");
                                        }
                                        else
                                        {
                                            strMessage = "未获取到政策";
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
                strMessage = ex.Message;
            }
            finally
            {
                if (dsPolicy == null)
                {
                    dsPolicy = new DataSet();
                    dsPolicy.DataSetName = "_517";
                }
                dr["Message"] = strMessage;
                dsPolicy.Tables.Add(Message);
                sbLog.Append("返回结果:XML=" + m_Switch.DataSetToXml(dsPolicy) + "\r\n");
                sbLog.Append("返回结果:" + m_Switch.DataSetToString(dsPolicy) + "\r\n");
                sbLog.AppendFormat("结束过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //日志                
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_517GetPolicy");
            }
            //缓存政策
            System.Threading.ThreadPool.QueueUserWorkItem(new CachePolicyManage(EnumPlatform._517, dsPolicy, pnrData).StartCachePolicy);
            return dsPolicy;
        }
        public DataSet _51BookGetPolicy(string _51bookAccout, string _51bookPassword, string _51bookAg, string IsLowerPrice, string PNRContent, PnrData pnrData)
        {
            DataSet dsPolicy = new DataSet();
            dsPolicy.DataSetName = "_51book";
            DataSwitch m_switch = new DataSwitch();
            DataTable Message = m_switch.CreatePolicy("51book");
            DataRow dr = Message.Rows[0];
            //文本日志
            StringBuilder sbLog = new StringBuilder();
            bool IsSuc = false;
            FormatPNR format = new FormatPNR();
            format.IsFilterCon = false;
            string strMessage = "";
            try
            {
                sbLog.Append(" 参数:_51bookAccout=" + _51bookAccout + "\r\n _51bookPassword=" + _51bookPassword + "\r\n _51bookAg=" + _51bookAg + "\r\n IsLowerPrice=" + IsLowerPrice + "\r\n PNRContent=" + PNRContent + "\r\n ");
                PNRContent = PNRContent.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                sbLog.AppendFormat("开始解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                PnrAnalysis.PnrModel pnrMode = null;
                PnrAnalysis.PatModel patMode = null;
                PnrAnalysis.PatModel infPatMode = null;
                PnrAnalysis.Model.SplitPnrCon SPC = null;
                string Pnr = string.Empty;
                string RTCon = string.Empty;
                string PatCon = string.Empty;
                string Msg = "";
                if (pnrData != null)
                {
                    pnrMode = pnrData.PnrMode;
                    patMode = pnrData.PatMode;
                    infPatMode = pnrData.InfPatMode;
                    Pnr = pnrMode.Pnr;
                    RTCon = pnrMode._OldPnrContent.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                    PatCon = patMode.PatCon.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                }
                else
                {
                    SPC = format.GetSplitPnrCon(PNRContent);
                    RTCon = SPC.RTCon;
                    PatCon = SPC.AdultPATCon != string.Empty ? SPC.AdultPATCon : SPC.ChdPATCon;
                    Pnr = format.GetPNR(RTCon, out Msg);
                    pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
                    patMode = format.GetPATInfo(PatCon, out Msg);
                }
                dr["Pnr"] = Pnr;
                sbLog.Append(" 解析参数:RTCon=" + RTCon + "\r\n PatCon=" + PatCon + "\r\n Pnr=" + Pnr + "\r\n Msg=" + Msg + "\r\n  ");
                if (pnrMode == null || pnrMode._LegList.Count == 0)
                {
                    strMessage = "PNR内容航段数据解析失败！";
                }
                else
                {
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        if (pnrMode != null && pnrMode.TravelType <= 2)
                        {
                            dr["PnrType"] = pnrMode._PnrType.ToString();
                            if (patMode != null && patMode.PatList.Count > 0)
                            {
                                PatInfo pat = patMode.PatList[patMode.PatList.Count - 1];
                                if (IsLowerPrice == "1")//取低价
                                {
                                    pat = patMode.PatList[0];
                                }
                                dr["SeatPrice"] = pat.Fare;
                                dr["ABFare"] = pat.TAX;
                                dr["RQFare"] = pat.RQFare;
                                dr["TotalPrice"] = pat.Price;
                                if (pnrMode.HasINF)
                                {
                                    if (pnrData == null)
                                        infPatMode = format.GetPATInfo(SPC.INFPATCon, out Msg);
                                    if (infPatMode != null && infPatMode.PatList.Count > 0)
                                    {
                                        pat = infPatMode.PatList[infPatMode.PatList.Count - 1];
                                        if (IsLowerPrice == "1")//取低价
                                        {
                                            pat = infPatMode.PatList[0];
                                        }
                                        dr["INFSeatPrice"] = pat.Fare;
                                        dr["INFABFare"] = pat.TAX;
                                        dr["INFRQFare"] = pat.RQFare;
                                        dr["INFTotalPrice"] = pat.Price;
                                    }
                                    else
                                    {
                                        strMessage = "婴儿PAT数据解析失败！";
                                    }
                                }
                            }
                            else
                            {
                                strMessage = "PAT数据解析失败！";
                            }
                        }
                        else
                        {
                            strMessage = "51book只支持单程和往返政策";
                        }
                    }
                }
                sbLog.AppendFormat("开始结束时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (string.IsNullOrEmpty(strMessage))
                {
                    bool m_B2BIsClose = B2BIsClose(EnumPlatform._51Book.ToString().Replace("_", ""), pnrMode._CarryCode);
                    bool m_BSPIsClose = BSPIsClose(EnumPlatform._51Book.ToString().Replace("_", ""), pnrMode._CarryCode);
                    //B2B 和BSP都关了的话
                    if (m_B2BIsClose && m_BSPIsClose)
                    {
                        strMessage = "B2B和BSP政策全部关闭";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            sbLog.AppendFormat("开始获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            web51book._51bookServiceSoapClient m_51book = new web51book._51bookServiceSoapClient();
                            dsPolicy = m_51book.bookGetPolicyDataByPNR(_51bookAccout, Pnr, _51bookAg, UrlEncode(RTCon), UrlEncode(PatCon));
                            sbLog.AppendFormat("结束获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            #region 过滤
                            sbLog.AppendFormat("开始过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            //<ID>1</ID><IsErorr>F</IsErorr><ErorrMessage>NO_SEG</ErorrMessage> 航段不存在
                            IsSuc = (dsPolicy != null && dsPolicy.Tables.Count > 0 && !dsPolicy.Tables[0].Columns.Contains("IsErorr") && dsPolicy.Tables[0].Rows.Count > 0);
                            if (IsSuc)
                            {
                                string StartDate = pnrMode._LegList[0].FlyDate1, EndDate = pnrMode._LegList[0].FlyDateE;
                                //过滤
                                string travel = "OW";
                                if (pnrMode._LegList.Count > 1)
                                {
                                    travel = "RT";
                                }
                                string where = "startDate<='" + StartDate + "' and expiredDate>='" + EndDate + "' ";
                                where += " and printTicketStartDate<='" + DateTime.Now.ToString("yyyy-MM-dd") + "' and printTicketExpiredDate>='" + DateTime.Now.ToString("yyyy-MM-dd") + "'";
                                where += " and routeType = '" + travel + "' and needSwitchPNR='false' and onWorking='true'";
                                //过滤B2B/BSP政策政策  
                                if (m_B2BIsClose)
                                {
                                    //关闭B2B
                                    where += " and PolicyType<>'B2P'";
                                }
                                else if (m_BSPIsClose)
                                {
                                    //关闭BSP                                    
                                    where += " and PolicyType='B2P'";
                                }
                                DataTable tempTab = dsPolicy.Tables[0].Clone();
                                tempTab.Columns["startDate"].DataType = typeof(DateTime);
                                tempTab.Columns["expiredDate"].DataType = typeof(DateTime);
                                tempTab.Columns["printTicketStartDate"].DataType = typeof(DateTime);
                                tempTab.Columns["printTicketExpiredDate"].DataType = typeof(DateTime);
                                DataRowCollection drs = dsPolicy.Tables[0].Rows;
                                foreach (DataRow item in drs)
                                {
                                    tempTab.ImportRow(item);
                                }
                                DataView dv = tempTab.DefaultView;
                                dv.RowFilter = where;
                                dv.Sort = " Commission desc ";
                                dsPolicy.Tables.Clear();
                                DataTable policy = dv.ToTable();
                                if (policy.Rows.Count > 0)
                                {
                                    policy.TableName = "Policy";
                                    dr["Status"] = "T";
                                    dsPolicy = new DataSet();
                                    dsPolicy.DataSetName = "_51book";
                                    dsPolicy.Tables.Add(policy);
                                    strMessage = "获取政策成功";
                                }
                                else
                                {
                                    strMessage = "未获取到政策!";
                                }
                            }
                            else
                            {
                                strMessage = "未获取到政策";
                            }
                            #endregion
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
                strMessage = ex.Message;
            }
            finally
            {
                if (dsPolicy == null)
                {
                    dsPolicy = new DataSet();
                    dsPolicy.DataSetName = "_51book";
                }
                dr["Message"] = strMessage;
                dsPolicy.Tables.Add(Message);
                sbLog.Append("返回结果:" + m_switch.DataSetToString(dsPolicy) + "\r\n");
                sbLog.AppendFormat("结束过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //日志                
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_51BookGetPolicy");
            }
            //缓存政策
            System.Threading.ThreadPool.QueueUserWorkItem(new CachePolicyManage(EnumPlatform._51Book, dsPolicy, pnrData).StartCachePolicy);
            return dsPolicy;
        }
        public DataSet _8000YIGetPolicy(string _8000yiAccout, string _8000yiPassword, string _IsLowerPrice, string PNRContent, PnrData pnrData)
        {
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("开始解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            DataSet dsPolicy = new DataSet();
            dsPolicy.DataSetName = "_8000YI";
            DataSwitch m_switch = new DataSwitch();
            DataTable Message = m_switch.CreatePolicy("8000YI");
            DataRow dr = Message.Rows[0];
            FormatPNR format = new FormatPNR();
            format.IsFilterCon = false;
            //文本日志           
            string strMessage = "";
            try
            {
                sbLog.Append(" 参数:_8000yiAccout=" + _8000yiAccout + "\r\n _8000yiPassword=" + _8000yiPassword + "\r\n _IsLowerPrice=" + _IsLowerPrice + "\r\n PNRContent=" + PNRContent + "\r\n ");
                PnrAnalysis.PnrModel pnrMode = null;
                PnrAnalysis.PatModel patMode = null;
                PnrAnalysis.PatModel infPatMode = null;
                PnrAnalysis.Model.SplitPnrCon SPC = null;
                string Pnr = string.Empty;
                string RTCon = string.Empty;
                string PatCon = string.Empty;
                string Msg = "";
                if (pnrData != null)
                {
                    pnrMode = pnrData.PnrMode;
                    patMode = pnrData.PatMode;
                    infPatMode = pnrData.InfPatMode;
                    Pnr = pnrMode.Pnr;
                    RTCon = pnrMode._OldPnrContent;
                    PatCon = patMode.PatCon;
                }
                else
                {
                    SPC = format.GetSplitPnrCon(PNRContent);
                    RTCon = SPC.RTCon;
                    PatCon = SPC.AdultPATCon != string.Empty ? SPC.AdultPATCon : SPC.ChdPATCon;
                    Pnr = format.GetPNR(RTCon, out Msg);
                    pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
                    patMode = format.GetPATInfo(PatCon, out Msg);
                }
                dr["Pnr"] = Pnr;
                sbLog.Append(" 解析参数:RTCon=" + RTCon + "\r\n PatCon=" + PatCon + "\r\n Pnr=" + Pnr + "\r\n Msg=" + Msg + "\r\n  ");
                if (pnrMode == null || pnrMode._LegList.Count == 0)
                {
                    strMessage = "PNR内容航段数据解析失败！";
                }
                else
                {
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        if (pnrMode.TravelType <= 2)
                        {
                            dr["PnrType"] = pnrMode._PnrType.ToString();
                            if (patMode != null && patMode.PatList.Count > 0)
                            {
                                PatInfo pat = patMode.PatList[patMode.PatList.Count - 1];
                                if (_IsLowerPrice == "1")//取低价
                                {
                                    pat = patMode.PatList[0];
                                }
                                dr["SeatPrice"] = pat.Fare;
                                dr["ABFare"] = pat.TAX;
                                dr["RQFare"] = pat.RQFare;
                                dr["TotalPrice"] = pat.Price;
                                if (pnrMode.HasINF)
                                {
                                    if (pnrData == null)
                                        infPatMode = format.GetPATInfo(SPC.INFPATCon, out Msg);
                                    if (infPatMode != null && infPatMode.PatList.Count > 0)
                                    {
                                        pat = infPatMode.PatList[infPatMode.PatList.Count - 1];
                                        if (_IsLowerPrice == "1")//取低价
                                        {
                                            pat = infPatMode.PatList[0];
                                        }
                                        dr["INFSeatPrice"] = pat.Fare;
                                        dr["INFABFare"] = pat.TAX;
                                        dr["INFRQFare"] = pat.RQFare;
                                        dr["INFTotalPrice"] = pat.Price;
                                    }
                                    else
                                    {
                                        strMessage = "婴儿PAT数据解析失败！";
                                    }
                                }
                            }
                            else
                            {
                                strMessage = "PAT数据解析失败！";
                            }
                        }
                        else
                        {
                            strMessage = "只支持单程,往返政策";
                        }
                    }
                }
                sbLog.AppendFormat("结束解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (string.IsNullOrEmpty(strMessage))
                {
                    bool m_B2BIsClose = B2BIsClose(EnumPlatform._8000YI.ToString().Replace("_", ""), pnrMode._CarryCode);
                    bool m_BSPIsClose = BSPIsClose(EnumPlatform._8000YI.ToString().Replace("_", ""), pnrMode._CarryCode);
                    //B2B 和BSP都关了的话
                    if (m_B2BIsClose && m_BSPIsClose)
                    {
                        strMessage = "B2B和BSP政策全部关闭";
                    }
                    else
                    {
                        sbLog.AppendFormat("开始获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        web8000yi.W8000YServiceSoapClient m_8000yi = new web8000yi.W8000YServiceSoapClient();
                        dsPolicy = m_8000yi.SPbyPNRNote(_8000yiAccout, _8000yiPassword, Pnr, 0, UrlEncode(RTCon));
                        sbLog.AppendFormat("结束获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        sbLog.AppendFormat("开始过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        #region 过滤
                        if (dsPolicy != null && dsPolicy.Tables.Count > 0 && !dsPolicy.Tables[0].Columns.Contains("ErrInfo"))
                        {
                            DataTable policy = dsPolicy.Tables[0].Copy();
                            //if (!string.IsNullOrEmpty(TravelType))
                            //{
                            //行程类型
                            DataView dv = dsPolicy.Tables[0].DefaultView;

                            //A7:  1表示单程  2表示往返 3表示单程往返
                            //A30  1表示低开 0不低开
                            //单程 舱位价 高低价格 
                            string where = string.Format(" A30={0} ", _IsLowerPrice);//" (A7='" + TravelType + "' and A7<>'')";// or A7='3' 
                            //过滤B2B/BSP政策政策  
                            if (m_B2BIsClose)
                            {
                                //关闭B2B
                                where += " and A16<>'B2B'";
                            }
                            else if (m_BSPIsClose)
                            {
                                //关闭BSP                                    
                                where += " and A16='B2B'";
                            }
                            dv.RowFilter = where;
                            //and A24={0}
                            // A8 政策返点
                            dv.Sort = " A8 desc ";
                            dsPolicy.Tables.Clear();
                            policy = dv.ToTable();
                            //}

                            if (policy.Rows.Count > 0)
                            {
                                policy.TableName = "Policy";
                                dr["Status"] = "T";
                                dsPolicy = new DataSet();
                                dsPolicy.DataSetName = "_8000YI";
                                dsPolicy.Tables.Add(policy);
                                strMessage = "获取政策成功";
                            }
                            else
                            {
                                strMessage = "未获取到政策!";
                            }
                        }
                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
                strMessage = ex.Message;
            }
            finally
            {
                if (dsPolicy == null)
                {
                    dsPolicy = new DataSet();
                    dsPolicy.DataSetName = "_8000YI";
                }
                dr["Message"] = strMessage;
                dsPolicy.Tables.Add(Message);
                sbLog.Append("返回结果:" + m_switch.DataSetToString(dsPolicy) + "\r\n");
                sbLog.AppendFormat("结束过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //日志                
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_8000YIGetPolicy");
            }
            //缓存政策
            System.Threading.ThreadPool.QueueUserWorkItem(new CachePolicyManage(EnumPlatform._8000YI, dsPolicy, pnrData).StartCachePolicy);
            return dsPolicy;
        }
        public DataSet _BaiTuoGetPolicy(string _baiTuoAccout, string _baiTuoPassword, string baiTuoAg, string _IsLowerPrice, string PNRContent, PnrData pnrData)
        {
            //文本日志
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("开始解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            DataSet dsPolicy = new DataSet();
            dsPolicy.DataSetName = "_BaiTuo";
            DataSwitch m_switch = new DataSwitch();
            DataTable Message = m_switch.CreatePolicy("BaiTuo");
            DataRow dr = Message.Rows[0];
            FormatPNR format = new FormatPNR();
            format.IsFilterCon = false;

            bool IsSuc = false;
            string strMessage = "";
            try
            {
                sbLog.Append(" 参数:_baiTuoAccout=" + _baiTuoAccout + "\r\n _baiTuoPassword=" + _baiTuoPassword + "\r\n baiTuoAg=" + baiTuoAg + "\r\n IsLowerPrice=" + _IsLowerPrice + "\r\n PNRContent=" + PNRContent + "\r\n ");

                PnrAnalysis.PnrModel pnrMode = null;
                PnrAnalysis.PatModel patMode = null;
                PnrAnalysis.PatModel infPatMode = null;
                PnrAnalysis.Model.SplitPnrCon SPC = null;
                string Pnr = string.Empty;
                string RTCon = string.Empty;
                string PatCon = string.Empty;
                string Msg = "";
                if (pnrData != null)
                {
                    pnrMode = pnrData.PnrMode;
                    patMode = pnrData.PatMode;
                    infPatMode = pnrData.InfPatMode;
                    Pnr = pnrMode.Pnr;
                    RTCon = pnrMode._OldPnrContent;
                    PatCon = patMode.PatCon;
                }
                else
                {
                    SPC = format.GetSplitPnrCon(PNRContent);
                    RTCon = SPC.RTCon;
                    PatCon = SPC.AdultPATCon != string.Empty ? SPC.AdultPATCon : SPC.ChdPATCon;
                    Pnr = format.GetPNR(RTCon, out Msg);
                    pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
                    patMode = format.GetPATInfo(PatCon, out Msg);
                }
                dr["Pnr"] = Pnr;

                if (pnrMode == null || pnrMode._LegList.Count == 0)
                {
                    strMessage = "PNR内容航段数据解析失败！";
                }
                else
                {
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        if (pnrMode != null && pnrMode.TravelType <= 2)
                        {
                            dr["PnrType"] = pnrMode._PnrType.ToString();
                            if (patMode != null && patMode.PatList.Count > 0)
                            {
                                PatInfo pat = patMode.PatList[patMode.PatList.Count - 1];
                                if (_IsLowerPrice == "1")//取低价
                                {
                                    pat = patMode.PatList[0];
                                }
                                dr["SeatPrice"] = pat.Fare;
                                dr["ABFare"] = pat.TAX;
                                dr["RQFare"] = pat.RQFare;
                                dr["TotalPrice"] = pat.Price;
                                if (pnrMode.HasINF)
                                {
                                    if (pnrData == null)
                                        infPatMode = format.GetPATInfo(SPC.INFPATCon, out Msg);
                                    if (infPatMode != null && infPatMode.PatList.Count > 0)
                                    {
                                        pat = infPatMode.PatList[infPatMode.PatList.Count - 1];
                                        if (_IsLowerPrice == "1")//取低价
                                        {
                                            pat = infPatMode.PatList[0];
                                        }
                                        dr["INFSeatPrice"] = pat.Fare;
                                        dr["INFABFare"] = pat.TAX;
                                        dr["INFRQFare"] = pat.RQFare;
                                        dr["INFTotalPrice"] = pat.Price;
                                    }
                                    else
                                    {
                                        strMessage = "婴儿PAT数据解析失败！";
                                    }
                                }
                            }
                            else
                            {
                                strMessage = "PAT数据解析失败！";
                            }
                        }
                        else
                        {
                            strMessage = "百拓只支持单程,往返政策!";
                        }
                    }
                }
                if (string.IsNullOrEmpty(strMessage))
                {
                    bool m_B2BIsClose = B2BIsClose(EnumPlatform._BaiTuo.ToString().Replace("_", ""), pnrMode._CarryCode);
                    bool m_BSPIsClose = BSPIsClose(EnumPlatform._BaiTuo.ToString().Replace("_", ""), pnrMode._CarryCode);
                    //B2B 和BSP都关了的话
                    if (m_B2BIsClose && m_BSPIsClose)
                    {
                        strMessage = "B2B和BSP政策全部关闭";
                    }
                    else
                    {

                        #region 拼接数据
                        string DepartureDateTime = "";
                        string FlightNumber = "";
                        string ResBookDesigCode = "";
                        string DepartureAirport = "";
                        string ArrivalAirport = "";
                        string TripType = "1";
                        string FlightNumberBack = "";
                        string ResBookDesigCodeBack = "";
                        string DepartureDateTimeBack = "";

                        for (int i = 0; i < pnrMode._LegList.Count; i++)
                        {
                            LegInfo leg = pnrMode._LegList[i];
                            if (i == 0)
                            {
                                DepartureDateTime = leg.FlyDate1 + "T" + leg.FlyStartTime.Insert(2, ":") + ":00";
                                FlightNumber = leg.AirCodeFlightNum.Replace("*", "");
                                ResBookDesigCode = leg.Seat;
                                DepartureAirport = leg.FromCode;
                                ArrivalAirport = leg.ToCode;
                            }
                            else if (i == 1)
                            {
                                TripType = "2";
                                FlightNumberBack = leg.AirCodeFlightNum.Replace("*", "");
                                ResBookDesigCodeBack = leg.Seat;
                                DepartureDateTimeBack = leg.FlyDate1 + "T" + leg.FlyStartTime.Insert(2, ":") + ":00";
                            }
                        }
                        #endregion

                        sbLog.AppendFormat("结束解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        sbLog.AppendFormat("开始获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        XmlElement xmlElement = m_switch.BaiTuoSSPolicySend(_baiTuoAccout, _baiTuoPassword, baiTuoAg, DepartureDateTime, FlightNumber, ResBookDesigCode, DepartureAirport, ArrivalAirport, TripType, FlightNumberBack, ResBookDesigCodeBack, DepartureDateTimeBack);
                        sbLog.Append(" 解析参数:RTCon=" + RTCon + "\r\n PatCon=" + PatCon + "\r\n Pnr=" + Pnr + "\r\n Msg=" + Msg + "\r\n XML=" + xmlElement.OuterXml + "\r\n ");
                        //webbaituo.BaiTuoWebSoapClient m_baituo = new webbaituo.BaiTuoWebSoapClient();
                        //string xmlNode = m_baituo.GetDomesticMatchNormalZRateStr(xmlElement.InnerXml);
                        string xmlNode = GetDomesticMatchNormalZRateStr(xmlElement.InnerXml, Pnr);
                        dsPolicy = m_switch.XmlToDataSet(xmlNode);
                        sbLog.AppendFormat("结束获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        sbLog.AppendFormat("开始过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        #region 过滤
                        IsSuc = (dsPolicy != null && dsPolicy.Tables.Count > 0 && dsPolicy.Tables[0].TableName.ToLower() != "error" && dsPolicy.Tables[0].Rows.Count > 0);
                        if (IsSuc)
                        {
                            string StartDate = pnrMode._LegList[0].FlyDate1, EndDate = pnrMode._LegList[0].FlyDateE;
                            string Where = "Effdate<='" + StartDate + "' and Expdate>='" + EndDate + "'";
                            //高低价格
                            Where += string.Format(" and Price={0} ", dr["SeatPrice"].ToString());
                            //过滤B2B/BSP政策政策
                            //1  BSP  2  B2B  3 BSP及B2B
                            if (m_B2BIsClose)
                            {
                                //关闭B2B
                                Where += " and PolicyType<>2";
                            }
                            else if (m_BSPIsClose)
                            {
                                //关闭BSP                                    
                                Where += " and PolicyType<>1";
                            }
                            DataTable tempTab = dsPolicy.Tables[0].Clone();
                            tempTab.Columns["Effdate"].DataType = typeof(DateTime);
                            tempTab.Columns["Expdate"].DataType = typeof(DateTime);//TripType
                            DataRowCollection drs = dsPolicy.Tables[0].Rows;
                            foreach (DataRow item in drs)
                            {
                                tempTab.ImportRow(item);
                            }
                            DataView dv = tempTab.DefaultView;
                            dv.RowFilter = Where;
                            dv.Sort = " Rate desc ";
                            dsPolicy.Tables.Clear();
                            DataTable policy = dv.ToTable();
                            if (policy.Rows.Count > 0)
                            {
                                policy.TableName = "Policy";
                                dr["Status"] = "T";
                                dsPolicy = new DataSet();
                                dsPolicy.DataSetName = "_BaiTuo";
                                dsPolicy.Tables.Add(policy);
                                strMessage = "获取政策成功";
                            }
                            else
                            {
                                strMessage = "未获取到政策!";
                            }
                        }
                        else
                        {
                            strMessage = "未获取到政策";
                        }
                        #endregion

                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
                strMessage = ex.Message;
            }
            finally
            {
                if (dsPolicy == null)
                {
                    dsPolicy = new DataSet();
                    dsPolicy.DataSetName = "_BaiTuo";
                }
                dr["Message"] = strMessage;
                dsPolicy.Tables.Add(Message);
                sbLog.Append("返回结果:" + m_switch.DataSetToString(dsPolicy) + "\r\n");
                sbLog.AppendFormat("结束过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //日志                
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_BaiTuoGetPolicy");
            }
            //缓存政策
            System.Threading.ThreadPool.QueueUserWorkItem(new CachePolicyManage(EnumPlatform._BaiTuo, dsPolicy, pnrData).StartCachePolicy);
            return dsPolicy;
        }
        public DataSet _JinRiGetPolicy(string _todayAccout, string _todayAccout2, string _IsLowerPrice, string PNRContent, PnrData pnrData)
        {
            DataSet dsPolicy = new DataSet();
            dsPolicy.DataSetName = "_JinRi";
            //文本日志
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("开始解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            bool IsSuc = false;
            DataSwitch m_switch = new DataSwitch();
            DataTable Message = m_switch.CreatePolicy("Today");
            DataRow dr = Message.Rows[0];
            FormatPNR format = new FormatPNR();
            PnrResource pnrResource = new PnrResource();
            format.IsFilterCon = false;
            string strMessage = "";
            try
            {
                sbLog.Append(" 参数:_todayAccout=" + _todayAccout + "\r\n _todayAccout2=" + _todayAccout2 + "\r\n IsLowerPrice=" + _IsLowerPrice + "\r\n PNRContent=" + PNRContent + "\r\n ");
                PnrAnalysis.PnrModel pnrMode = null;
                PnrAnalysis.PatModel patMode = null;
                PnrAnalysis.PatModel infPatMode = null;
                PnrAnalysis.Model.SplitPnrCon SPC = null;
                string Pnr = string.Empty;
                string RTCon = string.Empty;
                string PatCon = string.Empty;
                string Msg = "";
                if (pnrData != null)
                {
                    pnrMode = pnrData.PnrMode;
                    patMode = pnrData.PatMode;
                    infPatMode = pnrData.InfPatMode;
                    Pnr = pnrMode.Pnr;
                    RTCon = pnrMode._OldPnrContent.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                    PatCon = patMode.PatCon.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                }
                else
                {
                    SPC = format.GetSplitPnrCon(PNRContent);
                    RTCon = SPC.RTCon;
                    PatCon = SPC.AdultPATCon != string.Empty ? SPC.AdultPATCon : SPC.ChdPATCon;
                    Pnr = format.GetPNR(RTCon, out Msg);
                    pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
                    patMode = format.GetPATInfo(PatCon, out Msg);
                }
                dr["Pnr"] = Pnr;
                sbLog.Append(" 解析参数:RTCon=" + RTCon + "\r\n PatCon=" + PatCon + "\r\n Pnr=" + Pnr + "\r\n Msg=" + Msg + "\r\n  ");
                if (pnrMode == null || pnrMode._LegList.Count == 0)
                {
                    strMessage = "PNR内容航段数据解析失败！";
                }
                else
                {
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        if (pnrMode != null && pnrMode.TravelType == 1)
                        {
                            dr["PnrType"] = pnrMode._PnrType.ToString();
                            if (patMode != null && patMode.PatList.Count > 0)
                            {
                                PatInfo pat = patMode.PatList[patMode.PatList.Count - 1];
                                if (_IsLowerPrice == "1")//取低价
                                {
                                    pat = patMode.PatList[0];
                                }
                                dr["SeatPrice"] = pat.Fare;
                                dr["ABFare"] = pat.TAX;
                                dr["RQFare"] = pat.RQFare;
                                dr["TotalPrice"] = pat.Price;
                                if (pnrMode.HasINF)
                                {
                                    if (pnrData == null)
                                        infPatMode = format.GetPATInfo(SPC.INFPATCon, out Msg);
                                    if (infPatMode != null && infPatMode.PatList.Count > 0)
                                    {
                                        pat = infPatMode.PatList[infPatMode.PatList.Count - 1];
                                        if (_IsLowerPrice == "1")//取低价
                                        {
                                            pat = infPatMode.PatList[0];
                                        }
                                        dr["INFSeatPrice"] = pat.Fare;
                                        dr["INFABFare"] = pat.TAX;
                                        dr["INFRQFare"] = pat.RQFare;
                                        dr["INFTotalPrice"] = pat.Price;
                                    }
                                    else
                                    {
                                        strMessage = "婴儿PAT数据解析失败！";
                                    }
                                }
                            }
                            else
                            {
                                strMessage = "PAT数据解析失败！";
                            }
                        }
                        else
                        {
                            strMessage = "今日只支持单程政策";
                        }
                    }
                }
                if (string.IsNullOrEmpty(strMessage))
                {
                    bool m_B2BIsClose = B2BIsClose(EnumPlatform._Today.ToString().Replace("_", ""), pnrMode._CarryCode);
                    bool m_BSPIsClose = BSPIsClose(EnumPlatform._Today.ToString().Replace("_", ""), pnrMode._CarryCode);
                    //B2B 和BSP都关了的话
                    if (m_B2BIsClose && m_BSPIsClose)
                    {
                        strMessage = "B2B和BSP政策全部关闭";
                    }
                    else
                    {
                        if (pnrMode != null && patMode != null && patMode.PatList.Count > 0 && pnrMode._LegList.Count > 0)
                        {
                            LegInfo leg = pnrMode._LegList[0];
                            var fromCity = pnrResource.CityDictionary.CityList.Where(p => p.key == leg.FromCode).FirstOrDefault();
                            var toCity = pnrResource.CityDictionary.CityList.Where(p => p.key == leg.ToCode).FirstOrDefault();
                            string fromCityName = fromCity != null ? fromCity.city.Name : "";
                            string toCityName = toCity != null ? toCity.city.Name : "";
                            List<string> pasNameList = new List<string>();
                            List<string> cardNoList = new List<string>();
                            foreach (PassengerInfo item in pnrMode._PassengerList)
                            {
                                pasNameList.Add(item.PassengerName);
                                if (item.PassengerType == "3")
                                {
                                    cardNoList.Add(item.YinerBirthdayDate);
                                }
                                else
                                {
                                    cardNoList.Add(item.SsrCardID);
                                }
                            }
                            string PasName = string.Join("@", pasNameList.ToArray());
                            string cardNo = string.Join("@", cardNoList.ToArray());
                            PatInfo pat = patMode.PatList[patMode.PatList.Count - 1];
                            if (_IsLowerPrice == "1")//取低价
                            {
                                pat = patMode.PatList[0];
                            }
                            //单人机建燃油费
                            decimal ABFeeFueel = decimal.Parse(pat.TAX) + decimal.Parse(pat.RQFare);
                            decimal Discount = 100m;
                            //A:成人 C:儿童  默认为 A
                            string passengerType = pnrMode._PasType == "1" ? "A" : "C";
                            //行程类型 O-单程，F-往返，T-联程
                            string travelType = pnrMode.TravelType == 1 ? "O" : (pnrMode.TravelType == 2 ? "F" : "T");
                            string pnrInfo = travelType + "|P|" + Pnr + "^F^" + pnrMode._BigPnr + "|" + leg.FlyDate1 + "|" + leg.FromCode + "|" + fromCityName + "|" + leg.ToCode + "|" + toCityName + "|" + leg.AirCodeFlightNum.Replace("*", "") + "^" + (leg.IsShareFlight ? "Y" : "N") + "||" + leg.FlyStartTime.Insert(2, ":") + "|" + leg.FlyEndTime.Insert(2, ":") + "|" + leg.Seat + "|" + Discount + "||" + pat.Fare + "|" + ABFeeFueel + "|" + pnrMode._PassengerList.Count + "|" + PasName + "|" + cardNo;
                            sbLog.Append(" 调用接口参数:PnrInfo=" + pnrInfo + "\r\n ");
                            webTodayPolicy.JinRiRateServerSoapClient m_today = new webTodayPolicy.JinRiRateServerSoapClient();
                            StringBuilder sbXml = new StringBuilder();
                            sbXml.Append("<?xml version=\"1.0\" encoding=\"gb2312\" ?>");
                            sbXml.Append("<JIT-Policy-Request>");
                            sbXml.AppendFormat("<Request username=\"{0}\" pnr=\"{1}\" ", _todayAccout, Pnr);
                            sbXml.AppendFormat(" PnrInfo=\"{0}\" ", pnrInfo);
                            sbXml.AppendFormat(" PassengerType=\"{0}\" ", passengerType);
                            sbXml.AppendFormat(" Amount=\"16\" />");
                            sbXml.Append("</JIT-Policy-Request>");
                            sbLog.AppendFormat("请求数据:xml={0}\r\n", sbXml.ToString());
                            //调用接口
                            string strResult = m_today.GetRateListByPNR(sbXml.ToString());
                            sbLog.AppendFormat("返回结果:\r\n{0}\r\n", strResult);
                            sbLog.AppendFormat("结束解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                            sbLog.AppendFormat("开始获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            dsPolicy = m_switch.XmlToDataSet(strResult);
                            sbLog.AppendFormat("结束获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            sbLog.AppendFormat("开始过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                            IsSuc = (dsPolicy != null && dsPolicy.Tables.Contains("Response") && dsPolicy.Tables["Response"].Rows.Count > 0);
                            if (IsSuc)
                            {
                                string StartDate = leg.FlyDate1, EndDate = leg.FlyDateE;
                                //今日时间有问题
                                string Where = "Sdate<='" + StartDate + " 00:00:00' and Edate>='" + EndDate + " 00:00:00' ";
                                string TravelType = "";
                                if (pnrMode.TravelType == 1)
                                {
                                    TravelType = "0";
                                }
                                else if (pnrMode.TravelType == 2)
                                {
                                    TravelType = "1";
                                }
                                else if (pnrMode.TravelType == 3)
                                {
                                    TravelType = "2";
                                }
                                if (!string.IsNullOrEmpty(TravelType))
                                {
                                    Where += " and  VoyageType='" + TravelType + "'";
                                }
                                DataTable tempTab = dsPolicy.Tables["Response"].Clone();
                                tempTab.Columns["Sdate"].DataType = typeof(DateTime);
                                tempTab.Columns["Edate"].DataType = typeof(DateTime);
                                tempTab.Columns["Sdate"].DefaultValue = DateTime.Parse("1901-01-01");
                                tempTab.Columns["Edate"].DefaultValue = DateTime.Parse("1901-01-01");

                                DataRowCollection drs = dsPolicy.Tables["Response"].Rows;
                                foreach (DataRow item in drs)
                                {
                                    if (item["Sdate"] == DBNull.Value || item["Sdate"].ToString().Trim() == "")
                                    {
                                        item["Sdate"] = "1901-01-01";
                                    }
                                    if (item["Edate"] == DBNull.Value || item["Edate"].ToString().Trim() == "")
                                    {
                                        item["Edate"] = "1901-01-01";
                                    }
                                    tempTab.ImportRow(item);
                                }
                                DataView dv = tempTab.DefaultView;
                                //过滤B2B/BSP政策政策  
                                if (m_B2BIsClose)
                                {
                                    //关闭B2B
                                    Where += " and RateType<>'B2P'";
                                }
                                else if (m_BSPIsClose)
                                {
                                    //关闭BSP                                    
                                    Where += " and RateType='B2P'";
                                }
                                dv.RowFilter = Where;
                                dv.Sort = " Discounts desc";
                                DataTable policy = dv.ToTable();
                                if (policy.Rows.Count > 0)
                                {
                                    policy.TableName = "Policy";
                                    dr["Status"] = "T";
                                    dsPolicy = new DataSet();
                                    dsPolicy.DataSetName = "_JinRi";
                                    dsPolicy.Tables.Add(policy);
                                    strMessage = "获取政策成功";
                                }
                                else
                                {
                                    strMessage = "未获取到政策!";
                                }
                            }
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
                strMessage = ex.Message;
            }
            finally
            {
                if (dsPolicy == null)
                {
                    dsPolicy = new DataSet();
                    dsPolicy.DataSetName = "_JinRi";
                }
                dr["Message"] = strMessage;
                dsPolicy.Tables.Add(Message);
                sbLog.Append("返回结果:" + m_switch.DataSetToString(dsPolicy) + "\r\n");
                sbLog.AppendFormat("结束过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //日志                
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_JinRiGetPolicy");
            }
            //缓存政策
            System.Threading.ThreadPool.QueueUserWorkItem(new CachePolicyManage(EnumPlatform._Today, dsPolicy, pnrData).StartCachePolicy);
            return dsPolicy;
        }
        public DataSet _PiaoMengGetPolicy(string _pmAccout, string _pmPassword, string pmAg, string _IsLowerPrice, string PNRContent, PnrData pnrData)
        {
            DataSet dsPolicy = new DataSet();
            dsPolicy.DataSetName = "_PiaoMeng";
            //文本日志
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("开始解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            bool IsSuc = false;
            DataSwitch m_switch = new DataSwitch();
            DataTable Message = m_switch.CreatePolicy("PiaoMeng");
            DataRow dr = Message.Rows[0];
            FormatPNR format = new FormatPNR();
            format.IsFilterCon = false;
            string strMessage = "";
            try
            {
                sbLog.Append(" 参数:_pmAccout=" + _pmAccout + "\r\n _pmPassword=" + _pmPassword + "\r\n pmAg=" + pmAg + "\r\n IsLowerPrice=" + _IsLowerPrice + "\r\n PNRContent=" + PNRContent + "\r\n ");
                PnrAnalysis.PnrModel pnrMode = null;
                PnrAnalysis.PatModel patMode = null;
                PnrAnalysis.PatModel infPatMode = null;
                PnrAnalysis.Model.SplitPnrCon SPC = null;
                string Pnr = string.Empty;
                string RTCon = string.Empty;
                string PatCon = string.Empty;
                string Msg = "";
                if (pnrData != null)
                {
                    pnrMode = pnrData.PnrMode;
                    patMode = pnrData.PatMode;
                    infPatMode = pnrData.InfPatMode;
                    Pnr = pnrMode.Pnr;
                    RTCon = pnrMode._OldPnrContent.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                    PatCon = patMode.PatCon.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                }
                else
                {
                    SPC = format.GetSplitPnrCon(PNRContent);
                    RTCon = SPC.RTCon;
                    PatCon = SPC.AdultPATCon != string.Empty ? SPC.AdultPATCon : SPC.ChdPATCon;
                    Pnr = format.GetPNR(RTCon, out Msg);
                    pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
                    patMode = format.GetPATInfo(PatCon, out Msg);
                }

                dr["Pnr"] = Pnr;
                sbLog.Append(" 解析参数:RTCon=" + RTCon + "\r\n PatCon=" + PatCon + "\r\n Pnr=" + Pnr + "\r\n Msg=" + Msg + "\r\n  ");
                if (pnrMode == null || pnrMode._LegList.Count == 0)
                {
                    strMessage = "PNR内容航段数据解析失败！";
                }
                else
                {
                    if (pnrMode._LegList.Count > 0 && pnrMode._LegList[0].IsShareFlight)
                    {
                        strMessage = "票盟不支持共享航班生成订单";
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(strMessage))
                        {
                            dr["PnrType"] = pnrMode._PnrType.ToString();
                            if (patMode != null && patMode.PatList.Count > 0)
                            {
                                PatInfo pat = patMode.PatList[patMode.PatList.Count - 1];
                                if (_IsLowerPrice == "1")//取低价
                                {
                                    pat = patMode.PatList[0];
                                }
                                dr["SeatPrice"] = pat.Fare;
                                dr["ABFare"] = pat.TAX;
                                dr["RQFare"] = pat.RQFare;
                                dr["TotalPrice"] = pat.Price;
                                if (pnrMode.HasINF)
                                {
                                    if (pnrData == null)
                                        infPatMode = format.GetPATInfo(SPC.INFPATCon, out Msg);
                                    if (infPatMode != null && infPatMode.PatList.Count > 0)
                                    {
                                        pat = infPatMode.PatList[infPatMode.PatList.Count - 1];
                                        if (_IsLowerPrice == "1")//取低价
                                        {
                                            pat = infPatMode.PatList[0];
                                        }
                                        dr["INFSeatPrice"] = pat.Fare;
                                        dr["INFABFare"] = pat.TAX;
                                        dr["INFRQFare"] = pat.RQFare;
                                        dr["INFTotalPrice"] = pat.Price;
                                    }
                                    else
                                    {
                                        strMessage = "婴儿PAT数据解析失败！";
                                    }
                                }
                            }
                            else
                            {
                                strMessage = "PAT数据解析失败！";
                            }
                        }
                    }
                }
                if (string.IsNullOrEmpty(strMessage))
                {
                    bool m_B2BIsClose = B2BIsClose(EnumPlatform._PiaoMeng.ToString().Replace("_", ""), pnrMode._CarryCode);
                    bool m_BSPIsClose = BSPIsClose(EnumPlatform._PiaoMeng.ToString().Replace("_", ""), pnrMode._CarryCode);
                    //B2B 和BSP都关了的话
                    if (m_B2BIsClose && m_BSPIsClose)
                    {
                        strMessage = "B2B和BSP政策全部关闭";
                    }
                    else
                    {

                        #region 构造数据
                        string TravelType = "0";
                        if (pnrMode.TravelType == 2)
                        {
                            TravelType = "1";
                        }
                        else if (pnrMode.TravelType == 3)
                        {
                            TravelType = "2";
                        }
                        string StartDate = "";
                        string StartFlyNo = "";
                        string StartCity = "";
                        string SecondCity = "";
                        string StartSpace = "";

                        string SecondDate = "";
                        string SecondFlyNo = "";
                        string SecondSpace = "";
                        string ThirdCity = "";
                        for (int i = 0; i < pnrMode._LegList.Count; i++)
                        {
                            LegInfo leg = pnrMode._LegList[i];
                            if (i == 0)
                            {
                                StartDate = leg.FlyDate1;
                                StartFlyNo = leg.AirCodeFlightNum.Replace("*", "");
                                StartCity = leg.FromCode;
                                SecondCity = leg.ToCode;
                                StartSpace = (!string.IsNullOrEmpty(leg.ChildSeat) && leg.ChildSeat.Trim().Length == 2) ? leg.ChildSeat : leg.Seat;
                            }
                            else if (i == 1)
                            {
                                SecondDate = leg.FlyDate1;
                                SecondFlyNo = leg.AirCodeFlightNum.Replace("*", "");
                                SecondSpace = (!string.IsNullOrEmpty(leg.ChildSeat) && leg.ChildSeat.Trim().Length == 2) ? leg.ChildSeat : leg.Seat;
                                if (pnrMode.TravelType == 3)
                                {
                                    ThirdCity = leg.ToCode;
                                }
                            }
                        }
                        sbLog.AppendFormat("结束解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        sbLog.AppendFormat("开始获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        //webpm.PMServiceSoapClient m_PiaoMeng = new webpm.PMServiceSoapClient();

                        if (TravelType == "0")
                        {
                            sbLog.Append(" 调用参数:StartDate=" + StartDate + "\r\n StartFlyNo=" + StartFlyNo + "\r\n StartCity=" + StartCity + "\r\n SecondCity=" + SecondCity + "\r\n StartSpace=" + StartSpace + "\r\n _pmAccout=" + _pmAccout + "\r\npmAg=" + pmAg + "\r\n ");
                            dsPolicy = NewGetPolicyDataByDC(pnrMode._PnrType.ToString(), StartDate, StartFlyNo, StartCity, SecondCity, "", StartSpace, _pmAccout, pmAg, Pnr);
                        }
                        else
                        {
                            string citytemp = "";
                            //往返
                            if (TravelType == "1")
                            {
                                citytemp = StartCity;
                            }
                            //联程
                            else
                            {
                                citytemp = ThirdCity;
                            }
                            sbLog.Append(" 调用参数:TravelType=" + TravelType + "\r\n StartDate=" + StartDate + "\r\n SecondDate=" + SecondDate + "\r\n StartFlyNo=" + StartFlyNo + "\r\nStartCity=" + StartCity + "\r\n SecondCity=" + SecondCity + "\r\ncitytemp=" + citytemp + "\r\nStartSpace=" + StartSpace + "\r\n_pmAccout=" + _pmAccout + "\r\npmAg=" + pmAg + "\r\n ");
                            dsPolicy = NewGetPolicyDataByWFLC(pnrMode._PnrType.ToString(), TravelType, StartDate, SecondDate, StartFlyNo, SecondFlyNo, StartCity, SecondCity, SecondCity, citytemp, "", StartSpace, SecondSpace, _pmAccout, pmAg, Pnr);
                        }
                        sbLog.AppendFormat("结束获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        sbLog.AppendFormat("开始过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

                        #region 过滤
                        IsSuc = (dsPolicy != null && dsPolicy.Tables.Count > 1);
                        if (IsSuc)
                        {
                            string Where = "";
                            if (SecondDate == "")//单程
                            {
                                Where = " fromtime<='" + StartDate + " 00:00:00'  ";
                            }
                            else
                            {
                                Where = " fromtime<='" + StartDate + " 00:00:00' and totime>='" + SecondDate + " 23:59:59' ";
                            }
                            //过滤B2B/BSP政策政策  
                            if (m_B2BIsClose)
                            {
                                //关闭B2B
                                Where += " and policytype not like '%B2B%'";
                            }
                            else if (m_BSPIsClose)
                            {
                                //关闭BSP                                    
                                Where += " and policytype  like '%B2B%'";
                            }

                            DataTable tempTab = dsPolicy.Tables[1].Clone();
                            tempTab.Columns["fromtime"].DataType = typeof(DateTime);
                            tempTab.Columns["totime"].DataType = typeof(DateTime);
                            DataRowCollection drs = dsPolicy.Tables[1].Rows;
                            foreach (DataRow item in drs)
                            {
                                tempTab.ImportRow(item);
                            }
                            DataView dv = tempTab.DefaultView;
                            dv.RowFilter = Where;
                            dv.Sort = " rate desc";
                            DataTable policy = dv.ToTable();
                            if (policy.Rows.Count > 0)
                            {
                                policy.TableName = "Policy";
                                dr["Status"] = "T";
                                dsPolicy = new DataSet();
                                dsPolicy.DataSetName = "_PiaoMeng";
                                dsPolicy.Tables.Add(policy);
                                strMessage = "获取政策成功";
                            }
                            else
                            {
                                strMessage = "未获取到政策!";
                            }
                        }
                        else
                        {
                            strMessage = "未获取到政策";
                        }
                        #endregion

                        #endregion
                    }
                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
                strMessage = ex.Message;
            }
            finally
            {
                if (dsPolicy == null)
                {
                    dsPolicy = new DataSet();
                    dsPolicy.DataSetName = "_PiaoMeng";
                }
                dr["Message"] = strMessage;
                dsPolicy.Tables.Add(Message);
                sbLog.Append("返回结果:" + m_switch.DataSetToString(dsPolicy) + "\r\n");
                sbLog.AppendFormat("结束过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //日志                
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_PiaoMengGetPolicy");
            }
            //缓存政策
            System.Threading.ThreadPool.QueueUserWorkItem(new CachePolicyManage(EnumPlatform._PiaoMeng, dsPolicy, pnrData).StartCachePolicy);
            return dsPolicy;
        }
        public DataSet _YeeXingGetPolicy(string yeeXingAccout, string yeeXingAccout2, string _IsLowerPrice, string PNRContent, PnrData pnrData)
        {
            DataSet dsPolicy = new DataSet();
            dsPolicy.DataSetName = "_YeeXing";
            //文本日志
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("开始解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            bool IsSuc = false;
            DataSwitch m_switch = new DataSwitch();
            DataTable Message = m_switch.CreatePolicy("YeeXing");
            DataRow dr = Message.Rows[0];
            FormatPNR format = new FormatPNR();
            format.IsFilterCon = false;
            string strMessage = "";
            try
            {
                sbLog.Append(" 参数:yeeXingAccout=" + yeeXingAccout + "\r\n yeeXingAccout2=" + yeeXingAccout2 + " \r\n IsLowerPrice=" + _IsLowerPrice + "\r\n PNRContent=" + PNRContent + "\r\n ");
                PnrAnalysis.PnrModel pnrMode = null;
                PnrAnalysis.PatModel patMode = null;
                PnrAnalysis.PatModel infPatMode = null;
                PnrAnalysis.Model.SplitPnrCon SPC = null;
                string Pnr = string.Empty;
                string RTCon = string.Empty;
                string PatCon = string.Empty;
                string Msg = "";
                if (pnrData != null)
                {
                    pnrMode = pnrData.PnrMode;
                    patMode = pnrData.PatMode;
                    infPatMode = pnrData.InfPatMode;
                    Pnr = pnrMode.Pnr;
                    RTCon = pnrMode._OldPnrContent.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                    PatCon = patMode.PatCon.Replace("\r\n", "").ToUpper().Replace("\\R\\N", "");
                }
                else
                {
                    SPC = format.GetSplitPnrCon(PNRContent);
                    RTCon = SPC.RTCon;
                    PatCon = SPC.AdultPATCon != string.Empty ? SPC.AdultPATCon : SPC.ChdPATCon;
                    Pnr = format.GetPNR(RTCon, out Msg);
                    pnrMode = format.GetPNRInfo(Pnr, RTCon, false, out Msg);
                    patMode = format.GetPATInfo(PatCon, out Msg);
                }

                dr["Pnr"] = Pnr;
                sbLog.Append(" 解析参数:RTCon=" + RTCon + "\r\n PatCon=" + PatCon + "\r\n Pnr=" + Pnr + "\r\n Msg=" + Msg + "\r\n  ");
                if (pnrMode == null || pnrMode._LegList.Count == 0)
                {
                    strMessage = "PNR内容航段数据解析失败！";
                }
                else
                {
                    if (string.IsNullOrEmpty(strMessage))
                    {
                        if (pnrMode.TravelType <= 2)
                        {
                            dr["PnrType"] = pnrMode._PnrType.ToString();
                            if (patMode != null && patMode.PatList.Count > 0)
                            {
                                PatInfo pat = patMode.PatList[patMode.PatList.Count - 1];
                                if (_IsLowerPrice == "1")//取低价格
                                {
                                    pat = patMode.PatList[0];
                                }
                                dr["SeatPrice"] = pat.Fare;
                                dr["ABFare"] = pat.TAX;
                                dr["RQFare"] = pat.RQFare;
                                dr["TotalPrice"] = pat.Price;
                                if (pnrMode.HasINF)
                                {
                                    if (pnrData == null)
                                        infPatMode = format.GetPATInfo(SPC.INFPATCon, out Msg);
                                    if (infPatMode != null && infPatMode.PatList.Count > 0)
                                    {
                                        pat = infPatMode.PatList[infPatMode.PatList.Count - 1];
                                        if (_IsLowerPrice == "1")//取低价格
                                        {
                                            pat = infPatMode.PatList[0];
                                        }
                                        dr["INFSeatPrice"] = pat.Fare;
                                        dr["INFABFare"] = pat.TAX;
                                        dr["INFRQFare"] = pat.RQFare;
                                        dr["INFTotalPrice"] = pat.Price;
                                    }
                                    else
                                    {
                                        strMessage = "婴儿PAT数据解析失败！";
                                    }
                                }
                            }
                            else
                            {
                                strMessage = "PAT数据解析失败！";
                            }
                        }
                        else
                        {
                            strMessage = "只支持单程,往返政策";
                        }
                    }
                }
                if (string.IsNullOrEmpty(strMessage))
                {
                    sbLog.AppendFormat("结束解析时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                    bool m_B2BIsClose = B2BIsClose(EnumPlatform._YeeXing.ToString().Replace("_", ""), pnrMode._CarryCode);
                    bool m_BSPIsClose = BSPIsClose(EnumPlatform._YeeXing.ToString().Replace("_", ""), pnrMode._CarryCode);
                    //B2B 和BSP都关了的话
                    if (m_B2BIsClose && m_BSPIsClose)
                    {
                        strMessage = "B2B和BSP政策全部关闭";
                    }
                    else
                    {

                        sbLog.AppendFormat("开始获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        webyeexing.YeeXingSerivceSoapClient m_YeeXing = new webyeexing.YeeXingSerivceSoapClient();
                        dsPolicy = m_YeeXing.ParsePnrMatchAirpContract(yeeXingAccout, yeeXingAccout2, Pnr, UrlEncode(RTCon), UrlEncode(PatCon));
                        sbLog.AppendFormat("结束获取政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        sbLog.AppendFormat("开始过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                        #region 过滤
                        IsSuc = dsPolicy != null && dsPolicy.Tables.Count > 0 && dsPolicy.Tables.Contains("result") && dsPolicy.Tables["result"].Rows.Count > 0 && dsPolicy.Tables["result"].Rows[0]["is_success"].ToString() == "T";
                        if (IsSuc)
                        {
                            DataTable NewDt = dsPolicy.Tables["priceinfo"].Clone();
                            NewDt.Columns.Add("startTime", typeof(DateTime));
                            NewDt.Columns.Add("endTime", typeof(DateTime));
                            NewDt.Columns.Add("airComp", typeof(String));
                            NewDt.Columns.Add("airSeg", typeof(int));
                            NewDt.Columns.Add("orgCity", typeof(String));
                            NewDt.Columns.Add("dstCity", typeof(String));
                            NewDt.Columns.Add("flight", typeof(String));
                            NewDt.Columns.Add("cabin", typeof(String));

                            foreach (DataRow drItem in dsPolicy.Tables["priceinfo"].Rows)
                            {
                                DataRow NewDr = NewDt.NewRow();
                                for (int i = 0; i < dsPolicy.Tables["priceinfo"].Columns.Count; i++)
                                {
                                    NewDr[i] = drItem[i].ToString();
                                }
                                NewDr["startTime"] = dsPolicy.Tables["lineinfo"].Rows[0]["startTime"];
                                NewDr["endTime"] = dsPolicy.Tables["lineinfo"].Rows[0]["endTime"];
                                NewDr["airComp"] = dsPolicy.Tables["lineinfo"].Rows[0]["airComp"];
                                NewDr["orgCity"] = dsPolicy.Tables["lineinfo"].Rows[0]["orgCity"];
                                NewDr["dstCity"] = dsPolicy.Tables["lineinfo"].Rows[0]["dstCity"];
                                NewDr["flight"] = dsPolicy.Tables["lineinfo"].Rows[0]["flight"];
                                NewDr["cabin"] = dsPolicy.Tables["lineinfo"].Rows[0]["cabin"];
                                NewDr["airSeg"] = dsPolicy.Tables["lineinfos"].Rows[0]["airSeg"];
                                NewDt.Rows.Add(NewDr);
                            }
                            string TravelType = "";
                            if (pnrMode.TravelType == 1)
                            {
                                TravelType = "1";
                            }
                            else if (pnrMode.TravelType == 2)
                            {
                                TravelType = "2";
                            }
                            DataTable policy = NewDt.Copy();
                            if (!string.IsNullOrEmpty(TravelType))
                            {
                                string Where = " airSeg='" + TravelType + "' ";
                                //过滤B2B/BSP政策政策    过滤B2B政策 1--B2B，2--BSP
                                if (m_B2BIsClose)
                                {
                                    //关闭B2B                                   
                                    Where += " and tickType <> '1' ";
                                }
                                else if (m_BSPIsClose)
                                {
                                    //关闭BSP                                    
                                    Where += " and tickType <> '2' ";
                                }
                                DataView dv = NewDt.DefaultView;
                                dv.RowFilter = Where;
                                dv.Sort = " disc desc";
                                policy = dv.ToTable();
                            }
                            if (policy.Rows.Count > 0)
                            {
                                policy.TableName = "Policy";
                                dr["Status"] = "T";
                                dsPolicy = new DataSet();
                                dsPolicy.DataSetName = "_YeeXing";
                                dsPolicy.Tables.Add(policy);
                                strMessage = "获取政策成功";
                            }
                            else
                            {
                                strMessage = "未获取到政策!";
                            }
                        }
                        #endregion
                    }

                }
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + ex.StackTrace + "\r\n");
                strMessage = ex.Message;
            }
            finally
            {
                if (dsPolicy == null)
                {
                    dsPolicy = new DataSet();
                    dsPolicy.DataSetName = "_YeeXing";
                }
                dr["Message"] = strMessage;
                dsPolicy.Tables.Add(Message);
                sbLog.Append("返回结果:" + m_switch.DataSetToString(dsPolicy) + "\r\n");
                sbLog.AppendFormat("结束过滤政策时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                //日志                
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_YeeXingGetPolicy");
            }
            //缓存政策
            System.Threading.ThreadPool.QueueUserWorkItem(new CachePolicyManage(EnumPlatform._YeeXing, dsPolicy, pnrData).StartCachePolicy);
            return dsPolicy;
        }
        #endregion


        private string UrlDecode(string strData)
        {
            return HttpUtility.UrlDecode(strData);
        }
        private string UrlEncode(string strData)
        {
            return HttpUtility.UrlEncode(strData);
        }
        public DataSet GetBenefitDataPnrContent(string UserName, string UserPwd, string AgentSine, string PNRContent, string PatContent, string PNRCode)
        {
            string PID = "7008600281211297088";
            AgentSine = "6243c1cfffeb4d499729fe903e2571ad";


            PNRContent = PNRContent.Replace("\r\n", "");
            PatContent = PatContent.Replace("\r\n", "");

            PNRContent = PNRContent.Replace("", "").Replace("", "").Replace("", "").Replace("", "").Replace("NO PNR", "");
            PatContent = PatContent.Replace("", "").Replace("", "").Replace("", "").Replace("", "").Replace(">", "").Replace("NO PNR", "");

            DataSet dsNew = new DataSet();
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("start=============================================\r\n");
            sbLog.Append("请求时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");

            try
            {
                #region New
                string inputParam = ("UserName=" + UserName + "\r\n UserPwd=" + UserPwd + "\r\n AgentSine=" + AgentSine + " \r\nPNRContent=" + PNRContent + " \r\nPatContent=" + PatContent + "\r\n PNRCode=" + PNRCode + "\r\n");
                sbLog.Append("入口参数:\r\n" + inputParam);


                if (!PatContent.Contains("〉"))
                {
                    PatContent = PatContent.Replace("SFC:", "〉SFC:");
                }
                //PatContent = PatContent.Replace(">", "");
                if (PNRContent.Contains("&") || PNRContent.Contains(">") || PNRContent.Contains("<"))
                {
                    PNRContent = PNRContent.Replace("&", "").Replace(">", "").Replace("<", "");
                }
                string sss = "<pnrcontent>" + PNRContent + "</pnrcontent><patcontent>" + PatContent + "</patcontent>" + UserName + GetMD5String(UserPwd).ToUpper() + DateTime.Now.ToString("yyyy-MM-dd") + AgentSine + PID;
                string Sign = GetMD5String(sss).ToUpper();

                string XML = "<?xml version=\"1.0\" encoding=\"utf-8\" ?><request><service>get_benefit_pnrcontent</service><pid>" + PID + "</pid><username>" + UserName + "</username><sign>" + Sign + "</sign><params><pnrcontent>" + PNRContent + "</pnrcontent><patcontent>" + PatContent + "</patcontent></params></request>";

                sbLog.AppendFormat("XML请求:\r\n{0}\r\n", XML);

                string Content = new _517Policy.BenefitInterfaceSoapClient().InterfaceFacade(XML);
                sbLog.AppendFormat("返回结果:\r\n{0}\r\n", Content);
                #endregion


                #region 创建返回的DataSet
                DataTable tblDatas = new DataTable("Datas");
                if (Content.IndexOf("error") != -1)
                {
                    DataSwitch dataSwitch = new DataSwitch();
                    dsNew = dataSwitch.XmlToDataSet(Content);
                }
                else
                {
                    Content = Content.Replace("<item>", "");
                    Content = Content.Replace("<benefit>", "");
                    Content = Content.Replace("</benefit>", "");
                    Content = Content.Replace("</item>", "$");
                    string[] ContentNew = Content.Split('$');

                    DataColumn dc = null;
                    dc = tblDatas.Columns.Add("ID", Type.GetType("System.Int32"));
                    dc.AutoIncrement = true;//自动增加
                    dc.AutoIncrementSeed = 1;//起始为1
                    dc.AutoIncrementStep = 1;//步长为1
                    dc.AllowDBNull = false;
                    dc = tblDatas.Columns.Add("PolicyID", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("CarryCode", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("FromCity", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("ToCity", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("FlightType", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("Flight", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("Space", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("TravelType", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("PolicyType", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("Policy", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("ScheduleConstraints", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("CPCondition", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("EffectDate", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("ExpirationDate", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("GYOnlineTime", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("GYOutlineTime", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("IsSp", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("IsChangePNRCP", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("PolicyChildID", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("ZFFee", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("PMFee", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("ABFee", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("FuelFee", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("GYFPTime", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("GYFPTimeNew", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("ChupPiaoXiaolu", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("Remark", Type.GetType("System.String"));
                    dc = tblDatas.Columns.Add("Office", Type.GetType("System.String"));
                    for (int i = 0; i < ContentNew.Length - 1; i++)
                    {
                        string[] Items = ContentNew[i].ToString().Split('^');
                        //屏蔽往返
                        //if (Items[7].ToString() == "往返")
                        //{
                        //    continue;
                        //}
                        DataRow newRow;
                        newRow = tblDatas.NewRow();
                        newRow["PolicyID"] = Items[0].ToString();
                        newRow["CarryCode"] = Items[1].ToString();
                        newRow["FromCity"] = Items[2].ToString();
                        newRow["ToCity"] = Items[3].ToString();
                        newRow["FlightType"] = Items[4].ToString();
                        newRow["Flight"] = Items[5].ToString();
                        newRow["Space"] = Items[6].ToString();
                        newRow["TravelType"] = Items[7].ToString();
                        newRow["PolicyType"] = Items[8].ToString();
                        newRow["Policy"] = Items[9].ToString();
                        newRow["ScheduleConstraints"] = Items[10].ToString();
                        newRow["CPCondition"] = Items[11].ToString();
                        newRow["EffectDate"] = Items[12].ToString();
                        newRow["ExpirationDate"] = Items[13].ToString();
                        newRow["GYOnlineTime"] = Items[14].ToString();
                        newRow["GYOutlineTime"] = Items[15].ToString();
                        newRow["IsSp"] = Items[16].ToString();
                        newRow["IsChangePNRCP"] = Items[17].ToString();
                        newRow["PolicyChildID"] = Items[18].ToString();
                        newRow["ZFFee"] = Items[19].ToString();
                        newRow["PMFee"] = Items[20].ToString();
                        newRow["ABFee"] = Items[21].ToString();
                        newRow["FuelFee"] = Items[22].ToString();
                        newRow["GYFPTime"] = Items[23].ToString();
                        newRow["GYFPTimeNew"] = Items[24].ToString();
                        newRow["ChupPiaoXiaolu"] = Items[25].ToString();
                        newRow["Remark"] = Items[26].ToString();
                        newRow["Office"] = Items[27].ToString();
                        tblDatas.Rows.Add(newRow);

                    }
                    dsNew.Tables.Add(tblDatas);
                }

                #endregion
            }
            finally
            {
                sbLog.Append("\r\nend=============================================\r\n\r\n");
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_517GetPolicy\GetBenefitDataPnrContent");
            }
            return dsNew;
        }
        public static string GetMD5String(string str)
        {
            MD5 md5 = MD5.Create();
            byte[] b = Encoding.UTF8.GetBytes(str);
            byte[] md5b = md5.ComputeHash(b);
            md5.Clear();
            StringBuilder sb = new StringBuilder();
            foreach (var item in md5b)
            {
                sb.Append(item.ToString("x2"));
            }
            return sb.ToString();
        }

        public string GetDomesticMatchNormalZRateStr(string xmlMessage, string Pnr)
        {
            string xml = "";
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("start=============================================\r\n");
            sbLog.Append("请求时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
            sbLog.AppendFormat("编码:{0}\r\n", Pnr);
            sbLog.AppendFormat("请求XML:XML=" + xmlMessage + "\r\n");
            try
            {
                xml = new _baiTuoPolicy.AirWsCoManageSoapClient().GetDomesticMatchNormalZRateStr(xmlMessage);
                sbLog.Append("返回结果:result=" + xml);
            }
            catch (Exception ex)
            {
                sbLog.Append("异常信息:" + ex.Message + ex.StackTrace);
            }
            finally
            {
                sbLog.Append("\r\nend=============================================\r\n\r\n");
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_BaiTuoGetPolicy\GetDomesticMatchNormalZRateStr");
            }
            return xml;
        }

        /// <summary>       
        /// 票盟单程政策实时获取
        /// </summary>
        /// <param name="Date">航班起飞时间</param>
        /// <param name="Flightno">航班号</param>
        /// <param name="FromCity">出发城市</param>
        /// <param name="ToCity">到达城市</param>
        /// <param name="dataFormat"></param>
        /// <param name="Seat">舱位</param>
        /// <param name="UID">票盟用户名</param>
        /// <param name="PWD">密钥</param>
        /// <returns>航班类型</returns>      
        public DataSet NewGetPolicyDataByDC(string PnrType, string Date, string Flightno, string FromCityCode, string ToCityCode, string dataFormat, string Seat, string UID, string PWD, string Pnr)
        {
            DataSet ds = new DataSet();
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("start=============================================\r\n");
            sbLog.AppendFormat("编码:{0}\r\n", Pnr);
            sbLog.Append("请求时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
            try
            {
                List<string> ParamList = new List<string>();
                ParamList.Add("Cmd=SEARCHPOLICY");
                ParamList.Add(string.Format("Date={0}", Date));
                ParamList.Add(string.Format("Flightno={0}", Flightno));
                ParamList.Add(string.Format("FromCity={0}", FromCityCode));
                ParamList.Add("IsSpecmark=");
                ParamList.Add(string.Format("Pwd={0}", PWD));
                ParamList.Add(string.Format("Seat={0}", Seat));
                ParamList.Add(string.Format("ToCity={0}", ToCityCode));
                ParamList.Add(string.Format("Uid={0}", UID));
                ParamList.Add("dataFormat=0");
                if (PnrType == "2")
                {
                    ParamList.Add("IsChild=3");
                }
                string url = string.Format("{0}{1}", this.GetPMParam.BaseUrl, string.Join("&", ParamList.ToArray()));
                sbLog.Append("入口参数:Date=" + Date + "\r\nFlightno=" + Flightno + "\r\nFromCityCode=" + FromCityCode + "\r\nToCityCode=" + ToCityCode + "\r\ndataFormat=" + dataFormat + "\r\nSeat=" + Seat + "\r\nUID=" + UID + "\r\nPWD=" + PWD + "\r\n");
                sbLog.Append("票盟 获取政策请求URL：" + url + "\r\n");
                WebHttp http = new WebHttp();
                bool Status = false;
                string result = http.SendRequest(url, "GET", Encoding.UTF8, this.GetPMParam.TimeOut, ref Status);
                sbLog.Append("票盟获取政策，返回结果字符串：" + result + "\r\n");
                DataSwitch dataSwitch = new DataSwitch();
                ds = dataSwitch.XmlToDataSet(result);
            }
            catch (Exception ex)
            {
                sbLog.Append("票盟获取政策，异常信息：" + ex.Message + ex.StackTrace + "\r\n");
            }
            finally
            {
                sbLog.Append("\r\nend=============================================\r\n\r\n");
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_PiaoMengGetPolicy\GetPolicyDataByDC");
            }
            return ds;
        }

        /// <summary>
        /// 票盟 往返，联程政策实时获取
        /// </summary>
        /// <param name="Type">类型：1，往返 2，联程</param>
        /// <param name="Date">航班起飞时间</param>
        /// <param name="Flightno">航班号</param>
        /// <param name="FromCity">出发城市</param>
        /// <param name="ToCity">到达城市</param>
        /// <param name="dataFormat"></param>
        /// <param name="Seat">舱位</param>
        /// <param name="UID">票盟用户名</param>
        /// <param name="PWD">密钥</param>
        /// <returns>航班类型</returns>        
        public DataSet NewGetPolicyDataByWFLC(string PnrType, string Type, string Date, string Date2, string Flightno, string Flightno2, string FromCityCode, string FromCityCode2, string ToCityCode, string ToCityCode2, string dataFormat, string Seat, string Seat2, string UID, string PWD, string Pnr)
        {
            DataSet ds = new DataSet();
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("start=============================================\r\n");
            sbLog.AppendFormat("编码:{0}\r\n", Pnr);
            sbLog.Append("请求时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\n");
            try
            {
                List<string> ParamList = new List<string>();
                ParamList.Add("cmd=SEARCHPOLICY");
                ParamList.Add("UID=" + UID);
                ParamList.Add("PWD=" + PWD);
                ParamList.Add("FromCity=" + FromCityCode);
                ParamList.Add("TOCITY=" + ToCityCode);
                ParamList.Add("DATE=" + Date);
                ParamList.Add("FLIGHTNO=" + Flightno);
                ParamList.Add("SEAT=" + Seat);
                ParamList.Add("Agiofee=");
                ParamList.Add("oilbuildfee=");
                ParamList.Add("IsSpecmark=0");
                ParamList.Add("RouteType=" + (Type == "1" ? "1" : "2"));
                ParamList.Add("FromCity2=" + FromCityCode2);
                ParamList.Add("ToCity2=" + ToCityCode2);
                ParamList.Add("Date2=" + Date2);
                ParamList.Add("Flightno2=" + Flightno2);
                ParamList.Add("Seat2=" + Seat2);
                if (PnrType == "2")
                {
                    ParamList.Add("IsChild=3");
                }
                string url = string.Format("{0}{1}", this.GetPMParam.BaseUrl, string.Join("&", ParamList.ToArray()));
                sbLog.Append("入口参数:Type=" + Type + "\r\nDate=" + Date + "\r\nDate2=" + Date2 + "\r\nFlightno=" + Flightno + "\r\nFlightno2=" + Flightno2 + "\r\nFromCityCode=" + FromCityCode + "\r\nFromCityCode2=" + FromCityCode2 + "\r\ndataFormat=" + dataFormat + "\r\nSeat=" + Seat + "\r\nSeat2=" + Seat2 + "\r\nUID=" + UID + "\r\nPWD=" + PWD + "\r\n");
                sbLog.Append("票盟 获取政策请求URL：" + url + "\r\n");

                WebHttp http = new WebHttp();
                bool Status = false;
                string result = http.SendRequest(url, "GET", Encoding.UTF8, this.GetPMParam.TimeOut, ref Status);
                sbLog.Append("票盟获取政策，返回结果字符串：" + result + "\r\n");

                DataSwitch dataSwitch = new DataSwitch();
                ds = dataSwitch.XmlToDataSet(result);
            }
            catch (Exception ex)
            {
                sbLog.Append("票盟获取政策，异常信息：" + ex.Message + ex.StackTrace + "\r\n");
            }
            finally
            {
                sbLog.Append("\r\nend=============================================\r\n\r\n");
                PTLog.LogWrite(sbLog.ToString(), @"PTGetPolicy\_PiaoMengGetPolicy\GetPolicyDataByWFLC");
            }
            return ds;
        }


        #region 订单
        /// <summary>
        /// 今日退费票申请请求xml
        /// </summary>
        /// <param name="orderId">系统订单号</param>
        /// <param name="request">请求信息</param>
        /// <returns></returns>
        public string Today_TuiFeiOrder(string orderId, string _todayAccout2, TodayTuiFeiOrderRequest request)
        {
            StringBuilder sbXml = new StringBuilder();
            StringBuilder sbLog = new StringBuilder();
            string result = string.Empty;
            try
            {
                sbLog.Append("start=============================================\r\n");
                sbLog.AppendFormat("请求时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sbLog.AppendFormat("订单号:{0}\r\n", orderId);
                sbXml.Append("<?xml version=\"1.0\" encoding=\"gb2312\"?>");
                sbXml.Append("<JIT-TuiFeiOrder-Request><Request ");
                //---请求参数-----------
                sbXml.AppendFormat(" Name=\"{0}\" ", _todayAccout2);
                Type t = request.GetType();
                PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                object obj = null;
                string strValue = string.Empty;
                foreach (PropertyInfo p in properties)
                {
                    obj = p.GetValue(request, null);
                    strValue = (obj == null ? "" : obj.ToString());
                    //空值就不用传
                    if (!string.IsNullOrEmpty(strValue))
                        sbXml.AppendFormat(" {0}=\"{1}\" ", p.Name.TrimStart('_'), strValue);
                }
                //--------------
                sbXml.Append(" /></JIT-TuiFeiOrder-Request>");
                JinRiOrderServerSoapClient todayOrder = new JinRiOrderServerSoapClient();
                sbLog.AppendFormat("请求xml:\r\n{0}", sbXml.ToString());
                result = todayOrder.TuiFeiOrder(sbXml.ToString());
                sbLog.AppendFormat("返回结果:\r\n{0}", result);
            }
            catch (Exception ex)
            {
                sbLog.Append("今日退费票申请，异常信息：" + ex.Message + ex.StackTrace + "\r\n");
            }
            finally
            {
                sbLog.Append("\r\nend=============================================\r\n\r\n");
                PTLog.LogWrite(sbLog.ToString(), @"Today\Today_TuiFeiOrder");
            }
            return result;
        }
        /// <summary>
        /// 获取退费票信息
        /// </summary>
        /// <param name="userName">今日子账号</param>
        /// <param name="orderNo">今日退废订单号</param>
        /// <returns></returns>
        public DataSet Today_GetTuiFeiOrderInfo(string userName, string outOrderId)
        {
            StringBuilder sbXml = new StringBuilder();
            StringBuilder sbLog = new StringBuilder();
            string result = string.Empty;
            DataSet ds = new DataSet();
            try
            {
                sbLog.Append("start=============================================\r\n");
                sbLog.AppendFormat("请求时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sbLog.AppendFormat("今日订单号:{0}\r\n", outOrderId);
                sbXml.Append("<?xml version=\"1.0\" encoding=\"gb2312\"?>");
                sbXml.Append("<JIT-Order-Request><Request ");
                sbXml.AppendFormat(" username=\"{0}\" ", userName);
                sbXml.AppendFormat(" orderno=\"{0}\" ", outOrderId);
                sbXml.Append(" /></JIT-Order-Request>");
                JinRiOrderServerSoapClient todayOrder = new JinRiOrderServerSoapClient();
                sbLog.AppendFormat("请求xml:\r\n{0}", sbXml.ToString());
                result = todayOrder.GetReturnOrderInfo(sbXml.ToString());
                sbLog.AppendFormat("返回结果:\r\n{0}", result);
                DataSwitch dataSwitch = new DataSwitch();
                ds = dataSwitch.XmlToDataSet(result);
            }
            catch (Exception ex)
            {
                sbLog.Append("今日获取退费票信息，异常信息：" + ex.Message + ex.StackTrace + "\r\n");
            }
            finally
            {
                sbLog.Append("\r\nend=============================================\r\n\r\n");
                PTLog.LogWrite(sbLog.ToString(), @"Today\GetReturnOrderInfo");
            }
            return ds;
        }
        /// <summary>
        /// 申请机票升舱
        /// </summary>
        /// <param name="orderId">系统订单号</param>
        /// <param name="_todayAccout2">今日子账号</param>
        /// <param name="request">升舱信息</param>
        /// <returns></returns>
        public string Today_RiseCabin(string orderId, string _todayAccout2, TodayRiseCabinRequest request)
        {
            StringBuilder sbXml = new StringBuilder();
            StringBuilder sbLog = new StringBuilder();
            string result = string.Empty;
            try
            {
                sbLog.Append("start=============================================\r\n");
                sbLog.AppendFormat("请求时间:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                sbLog.AppendFormat("订单号:{0}\r\n", orderId);
                sbXml.Append("<?xml version=\"1.0\" encoding=\"gb2312\"?>");
                sbXml.Append("<JIT-RiseCabinOrder-Request><Request ");
                //---请求参数-----------
                sbXml.AppendFormat(" UserName=\"{0}\" ", _todayAccout2);
                Type t = request.GetType();
                PropertyInfo[] properties = t.GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                object obj = null;
                string strValue = string.Empty;
                foreach (PropertyInfo p in properties)
                {
                    obj = p.GetValue(request, null);
                    strValue = (obj == null ? "" : obj.ToString());
                    //空值就不用传
                    if (!string.IsNullOrEmpty(strValue))
                        sbXml.AppendFormat(" {0}=\"{1}\" ", p.Name.TrimStart('_'), strValue);
                }
                //--------------
                sbXml.Append(" /></JIT-RiseCabinOrder-Request>");
                JinRiOrderServerSoapClient todayOrder = new JinRiOrderServerSoapClient();
                sbLog.AppendFormat("请求xml:\r\n{0}", sbXml.ToString());
                result = todayOrder.RiseCabin(sbXml.ToString());
                sbLog.AppendFormat("返回结果:\r\n{0}", result);
            }
            catch (Exception ex)
            {
                sbLog.Append("今日申请机票升舱申请，异常信息：" + ex.Message + ex.StackTrace + "\r\n");
            }
            finally
            {
                sbLog.Append("\r\nend=============================================\r\n\r\n");
                PTLog.LogWrite(sbLog.ToString(), @"Today\Today_RiseCabin");
            }
            return result;
        }
        #endregion
    }

}
