using System;
using System.Collections.Generic;
using System.Text;
using PnrAnalysis.Model;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Data;
namespace PnrAnalysis
{
    /// <summary>
    ///TU19JUN 默认年为当年//30JAN12    
    /// </summary>
    public enum DataFormat
    {
        /// <summary>
        /// 如：TU19JUN
        /// </summary>
        weekDayMonth = 0,
        /// <summary>
        /// 01022012
        /// </summary>
        dayMonthYear = 1,
        monthDayYear = 2,
        dayYearMonth = 3,
        yearDayMonth = 4,
        monthYear = 5,
        dayMonth = 6
    }
    /// <summary>
    /// 编码格式解析
    /// </summary>
    /// 
    [Serializable]
    public class FormatPNR
    {
        /// <summary>
        /// 是否为大系统编码内容 默认false
        /// </summary>
        public bool IsBigContent = false;
        /// <summary>
        /// 传入编码是否为大编码进行解析
        /// </summary>
        public bool PnrIsBigCode = false;

        /// <summary>
        /// 是否过滤RT内容
        /// </summary>
        public bool IsFilterCon = true;
        /// <summary>
        /// 原始Pnr内容
        /// </summary>
        public string oldPnrContent = string.Empty;

        private bool _IsExistChildSapce = false;
        /// <summary>
        /// 是否存在子舱位 只读
        /// </summary>
        public bool IsExistChildSapce
        {
            get
            {
                return _IsExistChildSapce;
            }
        }


        /// <summary>
        /// 规范化PNR格式处理 每一项后面有\r\n   isTuan=false普通编码 isTuan=true标识团编码
        /// </summary>
        /// <param name="PNRData"></param>
        /// <returns></returns>
        private string FormatPnr(string PNR, string PNRData, PnrModel PM)
        {
            //计时
            Stopwatch watch = Stopwatch.StartNew();
            try
            {
                if (!string.IsNullOrEmpty(PNR))
                {
                    PM.Pnr = PNR;
                }

                #region 变量声明
                PM._PnrType = "1";
                //分割PNR项正则
                string RegSplit = @"(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+)";
                string RegTuan = @"\s*(?<TuanName>[\d|\w|\.|\S]+)\s*(NM(?<PCount>\d{1,3}))?\s*(?<TPNR>\w{5,6})\s*";// @"(?<PCount>\d{1,3})\s*(?<TuanName>\w+)";
                //航段正则 \s*(?<Num>\d+)\s*\.
                string RegSkyPattern = @"(?<left>\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d\s*\d\s*\d\s*\d)\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*)(?<orther>[\w\S\.\s]*)";
                //OSI 1E TKNO/833-2432160379 1RENMEI
                //提票号正则 
                //TN/784-4397606181/P1 
                //SSR TKNE CZ HK1 HAKHGH 6518 F15FEB 7842118669979/1/P1   新的
                string TIKNPattern = @"(\s*TN/(?<TNIN>IN/)?(?<TicketNum>\d{3,4}[-|\s]?\d{10})/P(?<Num>\d+))|(SSR\s*TKNE\s*.*?(?<TicketNum>\d{3,4}[-|\s]?\d{10})(/\d+)?/P(?<Num>\d+))";
                //乘机人正则 // @"\s*(?<Name>.+(?=[\u4e00-\u9fa5|\s]))\s*(?<=[\u4e00-\u9fa5|\s])(?<Pnr>[A-Z0-9]{5,6})\s*";
                string RegPasPattern = @"\s*(?<Name>[a-zA-Z\u4e00-\u9fa5/]+(?=[\u4e00-\u9fa5|\s])?)\s*(?<=[\u4e00-\u9fa5/|\s])(?<Pnr>([a-zA-Z0-9]{3}\s*[a-zA-Z0-9]{3})|.*?)\s*";
                //乘机人证件号正则 
                //SSR FOID CZ HK1 NI432824196910135187/P1 
                //SSR FOID 3U HK1 NI 51010319640125343/P1
                //SSR FOID MU HK/NI43010319680414351X/P1
                string RegPasSSRPattern = @"\s*(?<=SSR FOID)\s*(?<carry>\w{2})\s*(?<state>[A-Za-z]{2}\d{0,3})\s*(\/)?NI\s*(?<SSRNum>[\w|\(|\)|\-]+)/P(?<Num>\d+)\s*";
                //16.XN/IN/马晓INF(MAY12/P1                
                string RegYinerPattern = @"\s*(?<=XN\/IN\/)(?<YinerName>\w+\s*(INF)?\(?)(?<YinerBirthday>[A-Z]{3}\d{2})?\)?/P(?<Num>\d+)\s*";
                //"SSR INFT 3U HK1 CTUXIY 8555 Y31JUL SHANG/LING 23APR12/P1";
                //"SSR INFT CA HK1 CTUPVG 405 L28JUL LI/XIAOMING 08JUN12/P1"
                //"SSR INFT CA KK1CANPEK 1322 H31AUG ZHANG/HANXU 06FEB12/P1"
                //"SSR INFT 3U HK1 CKGTNA 8507 E22AUG DOU/GUANGSHUN 09NOV10/S1/P1" 婴儿
                //SSR INFT ZH KK1SZXCTU 9973 V19DEC CHENG/MINMIN 10NOV12/P1                 +
                string RegBigYinerPattern = @"(?<=SSR\s*INFT)\s*(.*?)\s*\w{1,2}\d{2}\w{3}\s*(?<YinerName>.*?)\s*(?<YinerBirthday>\d{2}[A-Za-z]{3}\d{2})\s*(\/S(?<S>\d+))?\/P(?<Num>\d+)\s*";
                //提取价格正则 FN项
                //"FN/A/FCNY1160.00/SCNY1160.00/C3.00/XCNY180.00/TCNY50.00CN/TCNY130.00YQ/          ACNY1340.00                                                                 \r\n ";
                //FN/IN/FCNY110.00/SCNY110.00/C0.00/TEXEMPTCN/TEXEMPTYQ/ACNY110.00 
                //FN/M/FCNY1350.00/XCNY150.00/TCNY50.00CN/TCNY100.00YQ/ACNY1500.00   
                //FN/A/IN/FCNY120.00/SCNY120.00/C0.00/TEXEMPTCN/TEXEMPTYQ/ACNY120.00
                string RegPricePattern = @"\s*(?<=FN/(A|IN|M|A/IN)/FCNY)\s*(?<Fare>\d+\.00)\s*(\/SCNY(?<SFare>\d+\.00)\s*\/C\d+\.00\s*)?\/(XCNY\d+\.00\s*\/)?(TCNY)?(?<JJ>\d+\.00|TEXEMPT)CN\s*\/(TCNY)?(?<YQ>\d+\.00|TEXEMPT)YQ\/[ |\-]*ACNY(?<Price>\d+\.00)\s*";
                //提取Office正则
                string RegOfficePattern = @"^\s*(?<Office>[A-Za-z]{3}\s*\d{3})\s*$";
                //大系统内容提取Office正则
                //PEK1E/HXH4DF/CTU324
                string RegBigCodeOfficePattern = @".*?\/" + PNR.Trim().Replace(" ", "") + @"\/(?<Office>[A-Za-z]{3}\s*\d{3})";
                //取小编码
                //PEK1E/HXH4DF/CTU324
                string RegGetSmallCodeOfficePattern = @".*?\/(?<Pnr>\w{5,6})\/(?<Office>[A-Za-z]{3}\s*\d{3})";
                //提取大编码正则
                string RegBigCodePattern = @"\s*(?<=-CA-|RMK\s*CA\/)(?<BigCode>\w{5,6})\s*";
                //授权的Office号
                string RMK_AUTHPattern = @"(?<=RMK\s*TJ\s*AUTH)\s*(?<AuthOffice>\w{3}\d{3})\s*";
                //5.TL/0920/27JUL/KWE164      出票时限 
                //5.TL/0615/26SEP/KMG226
                string TOfficePatern = @"TL/(?<TTime>\d{4})\/(?<TDate>\d{2}\w{3})\/(?<TOffice>\w{6})\s*";
                //航段楼正则
                string skyT = @"\s*(?<E>[A-Za-z]{1})\s*(?<FT1>T\d|\-{1,2})\s*(?<TT1>T\d|\-{1,2})\s*";

                //10.OSI 3U CTCT18080906080     
                string CTCTPattern = @"\s*(?<=.*?CTCT)\s*(?<phone>\d+)?\s*";
                string TOffice = "";
                //OSI CZ CTCM13682553641/P2
                string CTCMPattern = @"OSI\s*[A-Za-z0-9]{2}\s*CTCM(?<phone>.*?)\/P(?<PNum>\d{1,3})";

                //航段Other项
                //string otherPattern = "";
                string PnrOneStr = "", TempStr = "", TempPasName = "";
                int Num = 1;
                int Count = 0;
                bool Isadd = false;
                bool SkyStart = false;//航段是否开始   
                bool PasEnd = false;//乘机人结束开始  
                bool IsExistSsr = false;//是否存在证件号
                //规范后的Pnr项列表
                List<string> PnrList = new List<string>();
                /*
                //乘机人项列表
                List<string> PasList = new List<string>();            
                //票号项列表 
                List<string> TicketList = new List<string>();
                //RMK项列表
                List<string> RMKList = new List<string>();
                */
                //航段项列表
                List<string> SkyList = new List<string>();
                List<string> rePeatList = new List<string>();
                List<string> OSIList = new List<string>();
                List<string> OSICTCMList = new List<string>();

                //B2B票号格式
                //  OSI 1E HUET TN/880-2148030129 1MAYUYING
                //  OSI 1E MUET TN/7812117118176-7812117118183       
                //  OSI 1E MUET TN/7812117150566        只有一个人      
                //  OSI 1E MUET TN/7812116939024-7812116939026 
                List<string> NewTNList = new List<string>();//B2B票号                
                List<string> childList = new List<string>();
                List<string> YinerList = new List<string>();//所有婴儿项

                bool IsYiner = false;//该项是否为婴儿
                string PnrPoint = string.Empty;
                bool IsStartStr = false;
                PassengerInfo LastPasModel = null;
                string LastPasStr = string.Empty;
                #endregion

                // int OfficeSource = 0;//Office来源
                if (!string.IsNullOrEmpty(PNRData))
                {
                    //分割
                    string[] PnrArr = Regex.Split(PNRData, RegSplit, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    for (int i = 0; i < PnrArr.Length; i++)
                    {
                        PnrOneStr = FilterRowData(PnrArr[i]).Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Trim().ToUpper();
                        try
                        {
                            if (PnrOneStr == "" || IsTiDiaoStr(PnrOneStr)) continue;
                            if (!Isadd)
                            {
                                Num = int.Parse(PnrOneStr.Replace("X", ""));
                                Isadd = true;
                                continue;
                            }
                            IsYiner = false;
                            try
                            {
                                //---------------------start--------------------------------------
                                #region 处理数据

                                #region 编码类型
                                if (Count == 0)
                                {
                                    //团编码 否则普通编码
                                    if (Num == 0)
                                    {
                                        PM._Tuan.IsTuan = true;
                                        PM._PnrType = "2";
                                        Match mch = Regex.Match(PnrOneStr, RegTuan, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                        if (mch.Success)
                                        {
                                            int tuanCount = 0;
                                            string tempCount = mch.Groups["PCount"].Value;
                                            int.TryParse(tempCount, out tuanCount);
                                            PM._Tuan.TuanCount = tuanCount;
                                            PM._Tuan.TuanName = mch.Groups["TuanName"].Value.Replace(tempCount, "").Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Trim();
                                            if (!PnrIsBigCode)
                                            {
                                                //取编码
                                                PM._Pnr = mch.Groups["TPNR"].Value.Replace(" ", "");
                                                //PnrOneStr.Substring(PnrOneStr.Length - 6);
                                                //如果是大系统内容
                                                if (this.IsBigContent)
                                                {
                                                    if (PM._BigPnr == "")
                                                    {
                                                        PM._BigPnr = PM._Pnr;
                                                    }
                                                    if (PM._BigPnr.Trim() != "")
                                                    {
                                                        //将大编码替换为小编码
                                                        PnrOneStr = PnrOneStr.Replace(PM._BigPnr, PNR);
                                                    }
                                                    if (PM._Pnr == "")
                                                    {
                                                        PM._Pnr = PNR;
                                                    }
                                                }
                                            }
                                            PnrPoint = Num.ToString().PadLeft(2, ' ') + "." + PnrOneStr;
                                        }
                                    }
                                    Count++;
                                }
                                #endregion

                                //航段项是否开始                                
                                Match SkyMach = Regex.Match(PnrArr[i].Replace("\n", "").Replace("\r", "").Replace("\r\n", ""), RegSkyPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (SkyMach.Success)
                                {
                                    SkyStart = true;
                                }
                                else
                                {
                                    SkyStart = false;
                                }

                                #region 姓名项
                                //添加乘机人项 成人或者儿童
                                if (Num != 0 && !SkyStart && SkyList.Count == 0 && !PasEnd)
                                {
                                    if (Num == 1)
                                    {
                                        PM.PnrConHasFirstNum = true;
                                    }
                                    //乘机人信息列表
                                    //PasList.Add(Num + "." + PnrOneStr);
                                    PassengerInfo pasModel = new PassengerInfo();
                                    LastPasModel = pasModel;
                                    pasModel.PassengerName = PnrOneStr.Trim();
                                    PnrPoint = Num + "." + PnrOneStr;
                                    LastPasStr = PnrOneStr.Trim();
                                    pasModel.SerialNum = Num.ToString().Trim();
                                    pasModel.PassengerType = "1";

                                    bool IsChd = Regex.IsMatch(pasModel.PassengerName.ToUpper().Trim(), "[\u4e00-\u9fa5]+(?=CHD)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                    //儿童项
                                    if (pasModel.PassengerName.ToUpper().EndsWith(" CHD") || IsChd)
                                    {
                                        pasModel.PassengerType = "2";
                                        PM._PasType = "2";
                                        pasModel.PassengerName = pasModel.PassengerName.Substring(0, pasModel.PassengerName.LastIndexOf("CHD"));
                                    }
                                    PM._PassengerList.Add(pasModel);
                                }
                                else
                                {
                                    if (SkyStart && !PasEnd && LastPasStr != "")
                                    {
                                        PasEnd = true;
                                        bool IsPnrValie = false;
                                        if (LastPasStr.IndexOf(" ") != -1)
                                        {
                                            LastPasStr = GetLiuPnr(LastPasStr, 6);
                                            int index = LastPasStr.LastIndexOf(' ');
                                            string Pnr = LastPasStr.Substring(index).Trim().Replace(" ", "");
                                            string PasName = LastPasStr.Substring(0, index).Trim();
                                            if (Pnr.Length == 5 || Pnr.Length == 6)
                                            {
                                                IsPnrValie = true;
                                                LastPasModel.PassengerName = PasName;
                                                if (!PM._Tuan.IsTuan && PM._Pnr == "")
                                                {
                                                    PM._Pnr = Pnr;
                                                }
                                                if (this.IsBigContent)
                                                {
                                                    if (PM._BigPnr == "")
                                                    {
                                                        PM._BigPnr = Pnr;
                                                    }
                                                    if (PM._BigPnr.Trim() != "" && PnrList.Count > 0)
                                                    {
                                                        //将大编码替换为小编码
                                                        PnrList[PnrList.Count - 1] = (Num - 1).ToString().PadLeft(2, ' ') + "." + PasName + " " + PNR;
                                                    }
                                                    if (!PM._Tuan.IsTuan && PM._Pnr == "")
                                                    {
                                                        PM._Pnr = PNR;
                                                    }
                                                }
                                            }
                                        }
                                        if (!IsPnrValie)
                                        {
                                            Match PasCh = Regex.Match(LastPasStr, RegPasPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                            if (PasCh.Success)
                                            {
                                                TempPasName = PasCh.Groups["Name"].Value.Trim();
                                                if (TempPasName.ToUpper().EndsWith("CHD"))
                                                {
                                                    LastPasModel.PassengerName = TempPasName.Substring(0, TempPasName.LastIndexOf("CHD"));
                                                }
                                                if (TempPasName == "")
                                                {
                                                    LastPasModel.PassengerName = LastPasStr.Trim();
                                                }
                                                else
                                                {
                                                    LastPasModel.PassengerName = TempPasName;
                                                    if (this.IsBigContent)
                                                    {
                                                        if (PM._BigPnr == "")
                                                        {
                                                            PM._BigPnr = PasCh.Groups["Pnr"].Value.Trim().Replace(" ", "");
                                                        }
                                                        if (PM._BigPnr.Trim() != "" && PnrList.Count > 0)
                                                        {
                                                            //将大编码替换为小编码
                                                            PnrList[PnrList.Count - 1] = (Num - 1).ToString().PadLeft(2, ' ') + "." + TempPasName + " " + PNR;
                                                        }
                                                        if (!PM._Tuan.IsTuan && PM._Pnr == "")
                                                        {
                                                            PM._Pnr = PNR;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (!PM._Tuan.IsTuan && PM._Pnr == "")
                                                        {
                                                            PM._Pnr = PasCh.Groups["Pnr"].Value.Trim().Replace(" ", "");
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (LastPasModel.PassengerName.ToUpper().EndsWith("CHD"))
                                                {
                                                    LastPasModel.PassengerName = LastPasModel.PassengerName.Substring(0, LastPasModel.PassengerName.LastIndexOf("CHD"));
                                                }
                                            }
                                        }
                                        if (PnrList.Count > 0)
                                        {
                                            PnrList[PnrList.Count - 1] = (Num - 1).ToString().PadLeft(2, ' ') + "." + LastPasModel.PassengerName + " " + PM._Pnr;
                                        }
                                        bool IsChd = Regex.IsMatch(LastPasModel.PassengerName.ToUpper().Trim(), "[\u4e00-\u9fa5]+(?=CHD)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                        //儿童项
                                        if (LastPasModel.PassengerName.ToUpper().EndsWith(" CHD") || IsChd)
                                        {
                                            LastPasModel.PassengerType = "2";
                                            PM._PasType = "2";
                                            LastPasModel.PassengerName = LastPasModel.PassengerName.Substring(0, LastPasModel.PassengerName.LastIndexOf("CHD"));
                                        }
                                    }
                                }

                                #region //婴儿项
                                Match YinerPattern1 = Regex.Match(PnrOneStr, RegYinerPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (YinerPattern1.Success)
                                {
                                    IsYiner = true;//为婴儿
                                    PassengerInfo pasModel = new PassengerInfo();
                                    pasModel.SerialNum = Num.ToString().Trim();
                                    pasModel.PassengerType = "3";
                                    string[] YinerInfo = YinerPattern1.Groups["YinerName"].Value.Trim().Split(new string[] { "INF" }, StringSplitOptions.RemoveEmptyEntries);
                                    string YinerBirthday = "";
                                    if (YinerInfo.Length > 0)
                                    {
                                        pasModel.PassengerName = YinerInfo[0].Replace("(", "").Replace(")", "").Replace(" ", "");
                                        pasModel.YinToAdltNum = YinerPattern1.Groups["Num"].Value.Trim().Replace(" ", "");
                                        YinerBirthday = YinerPattern1.Groups["YinerBirthday"].Value.Replace(" ", "");
                                        if (YinerBirthday.Trim() == "")
                                        {
                                            Match Yer = Regex.Match(YinerInfo[0], @"(?<YinerBirthday>[A-Z]{3}\d{2})", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                            if (Yer.Success)
                                            {
                                                YinerBirthday = Yer.Groups["YinerBirthday"].Value.Trim().Replace(" ", "");
                                                pasModel.PassengerName = pasModel.PassengerName.Replace(YinerBirthday == "" ? " " : YinerBirthday, "").Trim().Replace("(", "").Replace(")", "").Replace(" ", "");
                                            }
                                        }
                                    }
                                    //婴儿出生日期
                                    pasModel.YinerBirthday = YinerBirthday;
                                    //去掉重复婴儿
                                    bool IsExist = false;
                                    for (int m = 0; m < PM._PassengerList.Count; m++)
                                    {
                                        PassengerInfo Yiner = PM._PassengerList[m];
                                        if (Yiner.PassengerType == "3" && Yiner.YinToAdltNum == pasModel.YinToAdltNum)
                                        {
                                            pasModel.YinerBirthday = PM._PassengerList[m].YinerBirthday;
                                            pasModel.YinerBirthdayDate = PM._PassengerList[m].YinerBirthdayDate;
                                            pasModel.YinToINFTName = PM._PassengerList[m].YinToINFTName;
                                            pasModel.YinToINFTNum = PM._PassengerList[m].YinToINFTNum;
                                            pasModel.YinToLegNum = PM._PassengerList[m].YinToLegNum;
                                            PM._PassengerList[m] = pasModel;
                                            IsExist = true;
                                            break;
                                        }
                                    }
                                    if (!IsExist)
                                    {
                                        PM._PassengerList.Add(pasModel);
                                    }
                                }
                                //用于取婴儿姓名
                                if (PnrOneStr.Contains("OSI YY"))
                                {
                                    OSIList.Add(PnrOneStr);
                                    IsYiner = true;//为婴儿
                                }
                                //婴儿项// OSI YY 1INF LIXIAOXIAO/P1
                                if (PnrOneStr.Contains("SSR INFT"))
                                {
                                    IsYiner = true;//为婴儿
                                    PassengerInfo pasModel = new PassengerInfo();
                                    pasModel.YinToINFTNum = Num.ToString().Trim();
                                    if (PnrOneStr.Contains("SSR INFT"))
                                    {
                                        //婴儿项 大系统提出来的
                                        //if (PnrOneStr.Split(' ').Length == 9)
                                        //{
                                        Match YinerPattern = Regex.Match(PnrOneStr, RegBigYinerPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                        if (YinerPattern.Success)
                                        {
                                            pasModel.PassengerName = YinerPattern.Groups["YinerName"].Value.Trim().Replace(" ", "");
                                            pasModel.YinToINFTName = pasModel.PassengerName;
                                            pasModel.SerialNum = Num.ToString().Trim();
                                            pasModel.PassengerType = "3";
                                            pasModel.YinToAdltNum = YinerPattern.Groups["Num"].Value.Trim().Replace(" ", "");//成人序号
                                            pasModel.YinToLegNum = YinerPattern.Groups["S"].Value.Trim().Replace(" ", "");//航段序号 默认第一行段
                                            //婴儿出生日期
                                            pasModel.YinerBirthday = YinerPattern.Groups["YinerBirthday"].Value.Replace(" ", "");
                                            //格式化日期
                                            pasModel.YinerBirthdayDate = GetYMD(pasModel.YinerBirthday, DataFormat.dayMonthYear);
                                            //去掉重复婴儿
                                            bool IsExist = PM._PassengerList.Exists(delegate(PnrAnalysis.Model.PassengerInfo pas1)
                                            {
                                                if (pas1.PassengerType == "3" && pas1.YinToAdltNum == pasModel.YinToAdltNum)
                                                {
                                                    return true;
                                                }
                                                else
                                                {
                                                    return false;
                                                }
                                            });
                                            if (!IsExist)
                                            {
                                                PM._PassengerList.Add(pasModel);
                                            }
                                        }
                                        //}
                                        else
                                        {
                                            try
                                            {
                                                int index = PnrOneStr.ToUpper().LastIndexOf("/P");
                                                string tempP = PnrOneStr;
                                                if (index != -1)
                                                {
                                                    if (PnrOneStr.Length >= index + 3)
                                                    {
                                                        tempP = PnrOneStr.Substring(0, index + 3);
                                                    }
                                                }
                                                string[] strArr = tempP.Trim().Split(' ');
                                                if (strArr.Length > 2)
                                                {
                                                    string name = strArr[strArr.Length - 2];
                                                    string strBirday = strArr[strArr.Length - 1];
                                                    if (strBirday.IndexOf("/") != -1)
                                                    {
                                                        string[] strArrbiry = strBirday.Split('/');
                                                        if (strArrbiry.Length == 2)
                                                        {
                                                            string birtday = strArrbiry[0];
                                                            string num = strArrbiry[1].ToUpper().Replace("P", "");
                                                            pasModel.PassengerName = name.Trim().Replace(" ", "");
                                                            pasModel.YinToINFTName = pasModel.PassengerName;
                                                            pasModel.SerialNum = Num.ToString().Trim();
                                                            pasModel.PassengerType = "3";
                                                            pasModel.YinToAdltNum = num.Trim().Replace(" ", "");
                                                            //婴儿出生日期
                                                            pasModel.YinerBirthday = birtday.Replace(" ", "");
                                                            //格式化日期
                                                            pasModel.YinerBirthdayDate = GetYMD(pasModel.YinerBirthday, DataFormat.dayMonthYear);
                                                            //去掉重复婴儿                                                   
                                                            bool IsExist = PM._PassengerList.Exists(delegate(PnrAnalysis.Model.PassengerInfo pas1)
                                                            {
                                                                if (pas1.PassengerType == "3" && pas1.YinToAdltNum == pasModel.YinToAdltNum)
                                                                {
                                                                    return true;
                                                                }
                                                                else
                                                                {
                                                                    return false;
                                                                }
                                                            });
                                                            if (!IsExist)
                                                            {
                                                                PM._PassengerList.Add(pasModel);
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            {
                                            }
                                        }
                                    }
                                }
                                #endregion

                                //儿童项
                                if (PnrOneStr.Contains("SSR CHLD"))
                                {
                                    childList.Add(PnrOneStr);
                                    //SSR CHLD CZ HK1 02JUL08/P1
                                    PM._PasType = "2";//儿童                                
                                }
                                #endregion

                                #region 航段项
                                if (SkyStart)
                                {
                                    //航段信息项
                                    SkyList.Add(Num + ".  " + PnrArr[i].Replace("\n", "").Replace("\r", "").Replace("\r\n", ""));
                                    if (SkyMach.Success)
                                    {
                                        LegInfo Leg = new LegInfo();
                                        Leg.SerialNum = Num.ToString();
                                        Leg.AirCodeFlightNum = SkyMach.Groups["carry"].Value.Trim().Replace("", "");
                                        //*ZH3695
                                        if (Leg.AirCodeFlightNum.StartsWith("*"))
                                        {
                                            Leg.IsShareFlight = true;
                                        }
                                        if (Leg.AirCodeFlightNum.Replace("*", "").Trim().Length == 6)
                                        {
                                            Leg.AirCode = Leg.AirCodeFlightNum.Replace("*", "").Trim().Substring(0, 2).ToUpper();
                                            Leg.FlightNum = Leg.AirCodeFlightNum.Replace("*", "").Trim().Substring(2).ToUpper();
                                        }
                                        else
                                        {
                                            if (Leg.AirCodeFlightNum.Replace("*", "").Trim().Length > 3)
                                            {
                                                Leg.AirCode = Leg.AirCodeFlightNum.Replace("*", "").Trim().Substring(0, 2).ToUpper();
                                                Leg.FlightNum = Leg.AirCodeFlightNum.Replace("*", "").Trim().Substring(2).ToUpper();
                                            }
                                        }
                                        Leg.Seat = SkyMach.Groups["seat"].Value.Replace(" ", "").ToUpper().Replace("", "");
                                        Leg.FlyDate = SkyMach.Groups["flyDate"].Value.Replace(" ", "").Replace("", "");
                                        Leg.FlyDate1 = GetYMD(Leg.FlyDate, DataFormat.weekDayMonth);//已日期格式显示 2012-06-19
                                        Leg.CityDouble = SkyMach.Groups["city"].Value.Replace(" ", "").ToUpper().Replace("", ""); //城市对
                                        if (Leg.CityDouble.Trim().Length == 6)
                                        {
                                            Leg.FromCode = Leg.CityDouble.Trim().Substring(0, 3).ToUpper();
                                            Leg.ToCode = Leg.CityDouble.Trim().Substring(3).ToUpper();
                                        }
                                        Leg.PnrStatus = SkyMach.Groups["state"].Value.Replace(" ", "").ToUpper().Replace("", "");
                                        Leg.FlyStartTime = SkyMach.Groups["startTime"].Value.Replace(" ", "").Replace("", "");
                                        Leg.FlyEndTime = SkyMach.Groups["endTime"].Value.Replace(" ", "").Replace("", "");//.Replace("+1", "");
                                        DateTime dt1 = System.DateTime.Now;
                                        DateTime.TryParse(Leg.FlyDate1, out dt1);
                                        string tempFlyEndTime = Leg.FlyEndTime;
                                        //到达日期
                                        if (Leg.FlyEndTime.IndexOf("+1") != -1)
                                        {
                                            Leg.FlyEndTime = Leg.FlyEndTime.Replace("+1", "").Replace("", "");
                                            Leg.IsAddOneDayEndTime = "1";
                                            Leg.FlyDateE = dt1.AddDays(1).ToString("yyyy-MM-dd");
                                        }
                                        else
                                        {
                                            Leg.FlyDateE = dt1.ToString("yyyy-MM-dd");
                                        }

                                        //其他的还要解析 
                                        string left = SkyMach.Groups["left"].Value;
                                        string orther = SkyMach.Groups["orther"].Value.Trim().Replace("", "");
                                        //继续解析
                                        //子舱位
                                        List<string> Otherlist = new List<string>();
                                        Otherlist.AddRange(orther.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                                        string childSeat = Leg.Seat + "1";
                                        if (Otherlist.Contains(childSeat))
                                        {
                                            Leg.ChildSeat = childSeat;
                                            _IsExistChildSapce = true;
                                            PM.IsExistChildSeat = true;
                                        }
                                        //航站楼提取数据
                                        Match skyTMah = Regex.Match(orther, skyT, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                        if (skyTMah.Success)
                                        {
                                            string E = skyTMah.Groups["E"].Value;
                                            string FT1 = skyTMah.Groups["FT1"].Value;
                                            string TT1 = skyTMah.Groups["TT1"].Value;
                                            Leg.E = E;
                                            Leg.FromCityT1 = FT1;
                                            Leg.ToCityT2 = TT1;
                                        }
                                        //预留两个空格
                                        //carry + " " + seat + " " + flyDate + " " + city + " " + state + " " + startTime + " " + endTime + " " + orther;
                                        //PnrOneStr = "  " + Leg.AirCodeFlightNum + " " + Leg.Seat + " " + Leg.FlyDate + " " + Leg.CityDouble + " " + Leg.PnrStatus + " " + Leg.FlyStartTime + " " + tempFlyEndTime + " " + orther;
                                        PnrOneStr = "  " + PnrArr[i].Replace("\n", "").Replace("\r", "").Replace("\r\n", "");
                                        PM._LegList.Add(Leg);
                                    }
                                }
                                #endregion

                                //证件号项
                                if (PnrOneStr.ToUpper().Contains("SSR FOID") && !PM._IsExistSsrFoid)
                                {
                                    ///是否存在证件号项 SSR FOID
                                    PM._IsExistSsrFoid = true;
                                }
                                Match SSRMatch = Regex.Match(PnrOneStr, RegPasSSRPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (SSRMatch.Success)
                                {
                                    SSRInfo ssr = new SSRInfo();
                                    ssr.SerialNum = Num.ToString();
                                    ssr.CardID = SSRMatch.Groups["SSRNum"].Value.Trim();
                                    ssr.PasNum = SSRMatch.Groups["Num"].Value.Trim();
                                    PM._SSRList.Add(ssr);
                                    IsExistSsr = true;//存在证件号
                                }
                                //票号项
                                Match tktnMch = Regex.Match(PnrOneStr, TIKNPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (tktnMch.Success)
                                {
                                    // TicketList.Add(Num + "." + PnrOneStr);
                                    TicketNumInfo TN = new TicketNumInfo();
                                    string TNIN = tktnMch.Groups["TNIN"].Value.Trim();
                                    if (!string.IsNullOrEmpty(TNIN))
                                    {
                                        TN.PasIsYinger = "1";
                                    }
                                    TN.SerialNum = Num.ToString();
                                    TN.TicketNum = tktnMch.Groups["TicketNum"].Value.Trim().Replace(" ", "");
                                    TN.PasNum = tktnMch.Groups["Num"].Value.Trim().Replace(" ", "");
                                    PM._TicketNumList.Add(TN);
                                }

                                //价格项
                                Match PriceMch = Regex.Match(PnrOneStr, RegPricePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (PriceMch.Success)
                                {
                                    PatInfo pat = new PatInfo();
                                    pat.SerialNum = Num.ToString();
                                    pat.Fare = PriceMch.Groups["Fare"].Value.Trim().Replace(" ", "");
                                    pat.TAX = PriceMch.Groups["JJ"].Value.Trim().Replace("TEXEMPTCN", "0").Replace("TEXEMPT", "0").Replace(" ", "");
                                    pat.RQFare = PriceMch.Groups["YQ"].Value.Trim().Replace("TEXEMPTYQ", "0").Replace("TEXEMPT", "0").Replace(" ", "");
                                    pat.Price = PriceMch.Groups["Price"].Value.Replace(" ", "");
                                    if (PnrOneStr.Contains("FN/IN/FCNY") || PnrOneStr.Contains("/IN/"))
                                    {
                                        pat.PriceType = "3";
                                    }
                                    PM._PatList.Add(pat);
                                }
                                //Offfice
                                //if (this.IsBigContent)
                                //{
                                //    RegOfficePattern = RegBigCodeOfficePattern;
                                //}
                                TempStr = PnrOneStr.Replace(" ", "");
                                //Office
                                Match OfficeMch = Regex.Match(TempStr.Replace("+", "").Replace("-", "").Replace(">", "").Trim(), RegOfficePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (OfficeMch.Success)
                                {
                                    PM._Office = OfficeMch.Groups["Office"].Value.Trim();
                                    //OfficeSource = 1;
                                    if (!PM.PnrConIsOver)
                                    {
                                        PM.PnrConIsOver = true;
                                    }
                                }
                                if (!PnrIsBigCode)
                                {
                                    Match OfficeMch1 = Regex.Match(TempStr, RegBigCodeOfficePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                    if (OfficeMch1.Success)
                                    {
                                        PM._Office = OfficeMch1.Groups["Office"].Value.Trim();
                                    }
                                    //大编码
                                    //if (PM._BigPnr == "")
                                    //{
                                    Match BigCodeMch = Regex.Match(PnrOneStr, RegBigCodePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                    if (BigCodeMch.Success)
                                    {
                                        PM._BigPnr = BigCodeMch.Groups["BigCode"].Value.Trim().Replace(" ", "");
                                    }
                                }
                                else
                                {
                                    MatchCollection mchcol = Regex.Matches(TempStr, @"\/", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                    if (mchcol.Count == 2)
                                    {
                                        //"PEK1E/JFDHVZ/SHA325"
                                        //传入编码为大编码
                                        Match OfficeMch1 = Regex.Match(TempStr, RegGetSmallCodeOfficePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                        if (OfficeMch1.Success)
                                        {
                                            //@".*?\/(?<Pnr>\w{5,6})\/(?<Office>[A-Za-z]{3}\s*\d{3})";
                                            PM._Office = OfficeMch1.Groups["Office"].Value.Trim();
                                            PM._Pnr = OfficeMch1.Groups["Pnr"].Value.Trim();
                                            PM._BigPnr = PNR;
                                            // OfficeSource = 2;
                                        }
                                    }
                                }
                                //出票时限中的Office
                                Match detrch = Regex.Match(TempStr, TOfficePatern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (detrch.Success)
                                {
                                    TOffice = detrch.Groups["TOffice"].Value;
                                    PM._Other.StrTL = TempStr;
                                    if (!PM._Other.IsTL)
                                    {
                                        PM._Other.IsTL = true;
                                    }
                                    if (PM._Office == "")
                                    {
                                        PM._Office = TOffice;
                                    }
                                    if (PM._Other.TKTL == null)
                                    {
                                        PM._Other.TKTL = new Tktl();
                                        PM._Other.TKTL.SerialNum = Num.ToString();
                                        PM._Other.TKTL.TkTime = detrch.Groups["TTime"].Value;
                                        PM._Other.TKTL.TkDate = detrch.Groups["TDate"].Value;
                                        PM._Other.TKTL.TkOffice = TOffice;
                                        PM._Other.IsTL = true;
                                    }
                                }
                                if (!PM._Other.IsTL && TempStr == "T")
                                {
                                    PM._Other.IsTL = true;
                                }

                                //CTCT项
                                Match CTCTMch = Regex.Match(PnrOneStr, CTCTPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (CTCTMch.Success)
                                {
                                    string strCTCT = CTCTMch.Groups["phone"].Value.Trim().Replace(" ", "");
                                    PM._Other.CTCT = strCTCT;
                                    if (!PM._Other.CTCTList.Contains(strCTCT))
                                    {
                                        PM._Other.CTCTList.Add(strCTCT);
                                    }
                                }
                                //CTCM
                                Match CTCMMach = Regex.Match(PnrOneStr, CTCMPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (CTCMMach.Success)
                                {
                                    OSICTCMList.Add(PnrOneStr);
                                }
                                //}
                                //RMK项
                                /*
                                if (PnrOneStr.StartsWith("RMK"))
                                {
                                    RMKList.Add(PnrOneStr);
                                }*/

                                //授权Office
                                Match AuthMch = Regex.Match(PnrOneStr, RMK_AUTHPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (AuthMch.Success)
                                {
                                    PM._RMK_TJ_AUTH_List.Add(AuthMch.Groups["AuthOffice"].Value.Trim().Replace(" ", ""));
                                }
                                #endregion
                                //---------------------end--------------------------------------
                                if (Num > 2)
                                {
                                    IsStartStr = true;
                                }
                                if (IsStartStr)
                                {
                                    if (Num == 0 || Num == 1)
                                    {
                                        if (PnrOneStr.Trim().Length == 1)
                                        {
                                            break;
                                        }
                                    }
                                }


                                PnrOneStr = Num.ToString().PadLeft(2, ' ') + "." + PnrOneStr;
                                PnrList.Add(PnrOneStr);

                                //加入婴儿
                                if (IsYiner && !YinerList.Contains(PnrOneStr))
                                {
                                    YinerList.Add(PnrOneStr);
                                }
                                //b2B票号段
                                //OSI 1E ET-3UET TN/8762313070396-8762313070398
                                //OSI 1E TKNO/833-2432160379 1RENMEI                                 
                                if (Regex.IsMatch(PnrOneStr, @"OSI 1E\s*.*?\s*(TN|TKNO)/", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                                {
                                    NewTNList.Add(PnrOneStr);
                                }
                                Isadd = false;
                            }
                            catch (Exception)
                            {
                                Isadd = false;
                            }
                        }
                        catch
                        {
                            // PnrList.Add(PnrOneStr);
                            PnrList.Add(PnrArr[i]);
                        }
                    }//for结束
                }

                #region 实体处理
                //最大序号
                PM.GetMaxNumber = Num;
                //两个编码是否一样
                if (PM._Pnr.ToUpper().Trim() == PM.Pnr.ToUpper().Trim())
                {
                    PM._IsPnrSame = true;
                }
                //大编码
                if (PM._BigPnr != "")
                {
                    for (int i = 0; i < PM._LegList.Count; i++)
                    {
                        PM._LegList[i].BigCode = PM._BigPnr;
                    }
                }
                //大系统Office处理
                for (int i = 0; i < PM._LegList.Count; i++)
                {
                    if (this.IsBigContent && PM._LegList[0].PnrStatus.Contains("RR"))
                    {
                        PM._Office = "";
                    }
                    PM._CarryCode = PM._LegList[0].AirCode.ToUpper();
                }
                //乘客英文名负值
                for (int k = 0; k < PM._PassengerList.Count; k++)
                {
                    PassengerInfo PasInfo = PM._PassengerList[k];
                    PasInfo.PassengerNameEnsh = PinYingMange.GetSpellByChinese(PasInfo.PassengerName).ToUpper();
                }
                bool IsB2BTicket = false;
                //B2B票号处理
                if (NewTNList.Count > 0)
                {
                    //  19.OSI 1E HUET TN/880-2148030129 1MAYUYING
                    //  19.OSI 1E MUET TN/7812117118176-7812117118183       
                    //  19.OSI 1E MUET TN/7812117150566        只有一个人      
                    //  19.OSI 1E MUET TN/7812116939024-7812116939026 

                    //票号段处理
                    string NewTN = @"(?<=\s*(?<num>\d{1,3})\s*\.\s*OSI 1E\s*.*?\s*TN/)(?<NewTN>\d{13,14}(\-\d{13,14})?)\s*";
                    foreach (string item in NewTNList)
                    {
                        Match mch = Regex.Match(item, NewTN, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        if (mch.Success)
                        {
                            string strB2BTN = mch.Groups["NewTN"].Value.Trim().Split('-')[0].Trim();
                            //票号长度
                            int TicketLen = strB2BTN.Length;
                            long lB2BTN = 0;
                            long.TryParse(strB2BTN, out lB2BTN);
                            //票号进一步处理
                            for (int i = 0; i < PM._PassengerList.Count; i++)
                            {
                                if (PM._PassengerList[i].PassengerType != "3")
                                {
                                    TicketNumInfo TN = new TicketNumInfo();
                                    TN.SerialNum = mch.Groups["num"].Value.Trim();
                                    TN.TicketNum = (lB2BTN + i).ToString().PadLeft(TicketLen, '0');
                                    PM._PassengerList[i].TicketNum = TN.TicketNum;
                                    TN.PasName = PM._PassengerList[i].PassengerName;
                                    TN.PasNum = PM._PassengerList[i].SerialNum;
                                    TN.PassengerNameEnsh = PM._PassengerList[i].PassengerNameEnsh;
                                    PM._TicketNumList.Add(TN);
                                }
                            }
                        }
                    }
                    //单个票号处理
                    //10.OSI 1E TKNO/833-2432160379 1RENMEI   
                    NewTN = @"(?<=\s*(?<num>\d{1,3})\s*\.\s*OSI 1E\s*.*?\s*(TN|TKNO)/)(?<NewTN>\d{3}\-\d{10})\s*\d(?<pname>[A-Za-z|\s]+)\s*";
                    foreach (string item in NewTNList)
                    {
                        Match mch = Regex.Match(item, NewTN, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        if (mch.Success)
                        {
                            TicketNumInfo TN = new TicketNumInfo();
                            TN.SerialNum = mch.Groups["num"].Value.Trim();
                            TN.TicketNum = mch.Groups["NewTN"].Value.Trim();
                            TN.PassengerNameEnsh = mch.Groups["pname"].Value.Trim().ToUpper().Trim();
                            IsB2BTicket = true;
                            bool exist = PM._TicketNumList.Exists(delegate(TicketNumInfo _TN)
                            {
                                return (_TN.PassengerNameEnsh.Replace(" ", "").Trim() == TN.PassengerNameEnsh);
                            });
                            if (!exist)
                            {
                                PM._TicketNumList.Add(TN);
                            }
                        }
                    }
                }
                //循环乘客
                for (int k = 0; k < PM._PassengerList.Count; k++)
                {
                    PassengerInfo PasInfo = PM._PassengerList[k];
                    //乘机人名字处理
                    PasInfo.PassengerName = Regex.Replace(PasInfo.PassengerName, @"\(\d+\)", "");

                    //修改儿童类型
                    foreach (string childStr in childList)
                    {
                        //SSR CHLD CZ HK1 02JUL08/P1
                        string TempStrP = Regex.Replace(childStr, @"SSR\s*CHLD\s*\w{2}", "");
                        //childStr.Replace("SSR CHLD CZ", "").Replace("SSR CHLD MU", "");
                        string SerialNum = "", SsrCard = "";
                        Match childCh = Regex.Match(TempStrP, @"\s*\w{2}\d+\s*(?<ssrCard>.*?)\/P(?<P>\d+)\s*", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        if (childCh.Success)
                        {
                            SerialNum = childCh.Groups["P"].Value.Trim();
                            SsrCard = childCh.Groups["ssrCard"].Value.Trim();
                            if (SerialNum != "" && PasInfo.SerialNum == SerialNum)
                            {
                                PasInfo.PassengerType = "2";
                                PasInfo.SsrCardID = SsrCard;
                                break;
                            }
                        }
                    }
                    if (PasInfo.PassengerType == "3")
                    {
                        //该婴儿所有序号
                        List<string> YNumList = new List<string>();
                        //删除该婴儿数据
                        List<string> delList = new List<string>();
                        string YNum = "", PNum = "";
                        for (int i = 0; i < YinerList.Count; i++)
                        {
                            YNum = ""; PNum = "";
                            Match mchYiner = Regex.Match(YinerList[i], @"\s*(?<YNum>\d{1,3})\s*\.\s*(.*?)\/P(?<PNum>\d+)\s*", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            if (mchYiner.Success)
                            {
                                YNum = mchYiner.Groups["YNum"].Value.Trim();
                                PNum = mchYiner.Groups["PNum"].Value.Trim();
                                if (PNum == PasInfo.YinToAdltNum.Trim())
                                {
                                    if (YinerList[i].ToUpper().Contains("SSR INFT"))
                                    {
                                        PasInfo.YinToINFTNum = YNum.ToString().Trim();
                                    }
                                    YNumList.Add(YNum);
                                    delList.Add(YinerList[i]);
                                }
                            }
                        }
                        PasInfo.SsrCardIDSerialNum = PasInfo.SerialNum;
                        //负值
                        if (YNumList.Count > 0)
                        {
                            PasInfo.SerialNum = string.Join(",", YNumList.ToArray());
                            PasInfo.SsrCardIDSerialNum = string.Join(",", YNumList.ToArray());
                        }
                        //删除
                        if (delList.Count > 0)
                        {
                            for (int i = 0; i < delList.Count; i++)
                            {
                                YinerList.Remove(delList[i]);
                            }
                        }
                        delList.Clear();
                        //修改婴儿姓名
                        foreach (string Ystr in OSIList)
                        {
                            Match YinerPattern = Regex.Match(Ystr, @"(?<=INF)\s*(?<YinerName>.*?)(INF)?\s*\/P(?<Num>\d+)\s*", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            if (YinerPattern.Success)
                            {
                                string YinerName = YinerPattern.Groups["YinerName"].Value.Trim().Replace(" ", "");
                                string YinToAdltNum = YinerPattern.Groups["Num"].Value.Trim().Replace(" ", "");
                                if (YinToAdltNum == PasInfo.YinToAdltNum)
                                {
                                    if (IsExistLetter(PasInfo.PassengerName))
                                    {
                                        PasInfo.PassengerName = YinerName;
                                    }
                                    break;
                                }
                            }
                        }
                        //婴儿跟随航段序号
                        if (string.IsNullOrEmpty(PasInfo.YinToLegNum))
                        {
                            PasInfo.YinToLegNum = PM._LegList[0].SerialNum;
                        }
                        //婴儿证件号
                        if (!string.IsNullOrEmpty(PasInfo.YinerBirthdayDate))
                        {
                            PasInfo.SsrCardID = PasInfo.YinerBirthdayDate;
                        }
                    }
                    //乘机人名字全是中文去掉空格
                    if (!IsExistLetter(PasInfo.PassengerName.Trim().Replace("\r", "").Replace("\n", "").Replace(" ", "")))
                    {
                        PasInfo.PassengerName = PasInfo.PassengerName.Trim().Replace("\r", "").Replace("\n", "").Replace(" ", "");
                    }
                    //票号进一步处理
                    for (int i = 0; i < PM._TicketNumList.Count; i++)
                    {
                        TicketNumInfo TN = PM._TicketNumList[i];
                        if (TN.PassengerNameEnsh == "" && TN.PasIsYinger == "1" && TN.PasNum == PasInfo.YinToAdltNum)
                        {
                            TN.PassengerNameEnsh = PasInfo.PassengerNameEnsh;
                            TN.PasName = PasInfo.PassengerName;
                        }
                    }
                    if (NewTNList.Count == 0)
                    {
                        //票号进一步处理
                        for (int i = 0; i < PM._TicketNumList.Count; i++)
                        {
                            TicketNumInfo TN = PM._TicketNumList[i];
                            if (TN.PasIsYinger != "1")
                            {
                                if (TN.PasNum.Trim() == PasInfo.SerialNum.Trim())
                                {
                                    TN.PasName = PasInfo.PassengerName;
                                    PasInfo.TicketNum = TN.TicketNum;
                                }
                            }
                            else
                            {
                                if (PasInfo.PassengerType == "3" && TN.PasNum == PasInfo.YinToAdltNum)
                                {
                                    TN.PasName = PasInfo.PassengerName;
                                    PasInfo.TicketNum = TN.TicketNum;
                                }
                            }
                        }
                    }
                    else
                    {
                        //B2B票号处理
                        if (IsB2BTicket)
                        {
                            //票号进一步处理
                            for (int i = 0; i < PM._TicketNumList.Count; i++)
                            {
                                TicketNumInfo TN = PM._TicketNumList[i];
                                if (TN.PassengerNameEnsh.Replace(" ", "") == PasInfo.PassengerNameEnsh.Replace(" ", ""))
                                {
                                    PasInfo.TicketNum = TN.TicketNum;
                                    PM._TicketNumList[i].PasName = PasInfo.PassengerName;
                                    PM._TicketNumList[i].PasNum = PasInfo.SerialNum;
                                }
                            }
                        }
                    }
                    //证件号进一步处理
                    for (int i = 0; i < PM._SSRList.Count; i++)
                    {
                        SSRInfo ssr = PM._SSRList[i];
                        if (ssr.PasNum.Trim() == PasInfo.SerialNum.Trim())
                        {
                            ssr.PasName = PasInfo.PassengerName;
                            ssr.PassType = PasInfo.PassengerType;
                            PasInfo.SsrCardIDSerialNum = ssr.SerialNum;
                            if (PasInfo.PassengerType == "3")
                            {
                                //婴儿
                                PasInfo.SsrCardID = PasInfo.YinerBirthday;
                            }
                            else if (PasInfo.PassengerType == "2")
                            {
                                if (Regex.IsMatch(ssr.CardID, @"^(\d{2}[A-Za-z]{3}\d{2})$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                                {
                                    //儿童02AUG12     
                                    PasInfo.SsrCardID = GetYMD(ssr.CardID, DataFormat.dayMonthYear);
                                }
                                else if (Regex.IsMatch(ssr.CardID, @"^(20\d{2}\d{2}\d{2})$", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase))
                                {
                                    PasInfo.SsrCardID = ssr.CardID.Insert(4, "-").Insert(7, "-");
                                    ssr.CardID = PasInfo.SsrCardID;
                                }
                                else
                                {
                                    if (!string.IsNullOrEmpty(ssr.CardID))
                                        PasInfo.SsrCardID = ssr.CardID;
                                }
                            }
                            else
                            {
                                if (PasInfo.SsrCardID.Trim() == "")
                                {
                                    PasInfo.SsrCardID = ssr.CardID;
                                }
                            }
                        }
                    }
                    //乘机人名字检查
                    if (PassengerNameIsCorrent(PasInfo.PassengerName))
                    {
                        PM.ErrorPassengerNameList.Add(PasInfo.PassengerName);
                        PasInfo.PassengerNameIsCorrent = false;
                        if (PM.PassengerNameIsCorrent)
                        {
                            PM.PassengerNameIsCorrent = false;
                        }
                    }
                    //处理乘客电话CTCM
                    foreach (string pasItem in OSICTCMList)
                    {
                        Match CTCMMach = Regex.Match(pasItem, CTCMPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        if (CTCMMach.Success)
                        {
                            string phone = CTCMMach.Groups["phone"].Value.Trim();
                            string PNum = CTCMMach.Groups["PNum"].Value.Trim();
                            if (!string.IsNullOrEmpty(PNum) && PNum == PasInfo.SerialNum)
                            {
                                PasInfo.PassengerTel = phone;
                                break;
                            }
                        }
                    }
                }
                //去掉重复票号
                PM._TicketNumList = PM.MoveRepeatTicketNum(PM._TicketNumList);
                //这两项没有解析失败
                if (PM._LegList.Count == 0 || PM._PassengerList.Count == 0)
                {
                    PM._ErrMsg = "PNR内容解析失败";
                }
                string RMKBIGCode = "";
                if (!string.IsNullOrEmpty(PM._BigPnr))
                {
                    RMKBIGCode = (++Num) + ".RMK CA/" + PM._BigPnr;
                }
                if (this.IsBigContent)
                {
                    if (!PNRData.Contains(".RMK CA/" + PM._BigPnr) && RMKBIGCode != "")
                    {
                        PnrList.Add(RMKBIGCode);
                    }
                    //处理Office
                    if (PM._Office != "")
                    {
                        string office = (++Num) + "." + PM._Office;
                        if (!PnrList.Contains(office))
                        {
                            if (!PnrList[PnrList.Count - 1].Contains("." + PM._Office))
                            {
                                PnrList.Add(office);
                            }
                        }
                    }
                }
                else
                {
                    if (!PNRData.Contains(".RMK CA/" + PM._BigPnr) && RMKBIGCode != "")
                    {
                        PnrList.Insert(PnrList.Count - 1, RMKBIGCode);
                    }
                }
                if (PnrIsBigCode)
                {
                    //处理编码
                    for (int i = 0; i < PnrList.Count; i++)
                    {
                        if (PnrList[i].Contains(PnrPoint))
                        {
                            PnrList[i] = PnrList[i].Replace(PNR, PM._Pnr);
                            break;
                        }
                    }
                }
                #region 动态添加一些内容

                if (!IsExistSsr)
                {
                    if (PnrList.Count > 1)
                    {
                        string LastStr = PnrList[PnrList.Count - 1].Split('.')[1];
                        int prevNum = PnrList.Count - 1;
                        int.TryParse(PnrList[PnrList.Count - 2].Split('.')[0], out prevNum);
                        prevNum++;
                        PnrList.Insert(PnrList.Count - 1, prevNum + "." + "SSR FOID");
                        prevNum++;
                        PnrList[PnrList.Count - 1] = prevNum + "." + LastStr;
                    }
                }
                #endregion
                #endregion
                if (PnrList.Count > 0)
                {
                    //加了一个空格
                    PNRData = string.Join(" \r\n", PnrList.ToArray());
                }
            }
            catch (Exception ex)
            {
                PM._ErrMsg = ex.Message + "|\r\n" + ex.StackTrace.ToString();
            }
            finally
            {
                watch.Stop();
                TimeSpan ts = watch.Elapsed;
                //解析该字符串所用时间
                PM._AnalysisTime = String.Format("{0}|{1:00}:{2:00}:{3:00}.{4:000}", watch.ElapsedTicks,
                ts.Hours, ts.Minutes, ts.Seconds,
                ts.Milliseconds);
            }
            if (!string.IsNullOrEmpty(PNRData))
            {
                PM._FormatPnrContent = PNRData.ToUpper();
            }
            PM._OldPnrContent = this.oldPnrContent;
            return PM._FormatPnrContent;
        }
        /// <summary>
        /// 将日期格式转换为与定编码日期格式 如:2012-08-30 =>26SEP12
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public static string DateToStr(string date, DataFormat ddate)
        {
            if (date.Length == 4 || date.Length == 5)
            {
                return date;
            }
            else
            {
                string temp = date;
                //日期识别正则
                string strPattern = @"^(?<year>(19|20)\d{2})[\-|\/]?(?<month>0?[1-9]|10|11|12)[\-|\/]?(?<day>0?[1-9]|1\d|2\d|30|31)$";
                try
                {
                    Match ch = Regex.Match(temp, strPattern);
                    if (ch.Success)
                    {
                        string _year = ch.Groups["year"].Value.ToString();
                        string _month = ch.Groups["month"].Value.ToString();
                        string _day = ch.Groups["day"].Value.ToString();
                        date = _year + "-" + _month.PadLeft(2, '0') + "-" + _day.PadLeft(2, '0');
                    }
                    DateTime time = DateTime.Parse(date);
                    string month = "";
                    int day = time.Day;
                    string[] sEnMonsthsInfo = { "", "January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December" };
                    month = sEnMonsthsInfo[time.Month].ToLower().Substring(0, 3);
                    if (ddate == DataFormat.dayMonth)
                    {
                        date = day.ToString().PadLeft(2, '0') + month;
                    }
                    else if (ddate == DataFormat.dayMonthYear)
                    {
                        date = day.ToString().PadLeft(2, '0') + month + time.Year.ToString().Substring(2, 2);
                    }
                    else if (ddate == DataFormat.monthYear)
                    {
                        date = month + time.Year.ToString().Substring(2, 2);
                    }
                }
                catch (Exception)
                {
                    date = temp;
                }
            }
            return date;
        }
        //TU19JUN 默认年为当年
        //30JAN12
        public static string GetYMD(string strData, DataFormat dateformat)
        {
            if (!string.IsNullOrEmpty(strData))
            {
                string tempdate = strData;
                if (dateformat == DataFormat.weekDayMonth)
                {
                    #region weekDayMonth TU19JUN 默认年为当年
                    strData = strData.Replace(" ", "").Trim();
                    if (strData.Length == 7 || strData.Length == 9)
                    {
                        string week = "", day = "", month = "", year = System.DateTime.Now.ToString("yyyy");
                        Match ch = Regex.Match(strData, @"\s*(?<week>\w{2})\s*(?<day>\d{1,2})\s*(?<month>\w{3})\s*(?<year>\d{2})?\s*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                        if (ch.Success)
                        {
                            week = WeekStrToNum(ch.Groups["week"].Value.Trim(), 4);
                            day = ch.Groups["day"].Value.Trim().PadLeft(2, '0');
                            month = MonthStrToNum(ch.Groups["month"].Value.Trim()).PadLeft(2, '0');
                            year = ch.Groups["year"].Value.Trim();
                            if (!string.IsNullOrEmpty(year))
                            {
                                year = System.DateTime.Now.Year.ToString().Substring(0, 2) + year.Trim().PadLeft(2, '0');
                            }
                            if (year == "")
                            {
                                try
                                {
                                    strData = string.Format("{0}-{1}-{2}", DateTime.Now.Year, month, day);
                                    DateTime dt = System.DateTime.Parse(strData);
                                    if (((int)dt.DayOfWeek).ToString() != week)
                                    {
                                        while (dt.Year < 2999)
                                        {
                                            dt = dt.AddYears(1);
                                            if (((int)dt.DayOfWeek).ToString() == week)
                                            {
                                                year = dt.Year.ToString();
                                                break;
                                            }
                                        }
                                    }
                                    if (year == "")
                                    {
                                        year = System.DateTime.Now.Year.ToString();
                                    }
                                }
                                catch (Exception)
                                {
                                    year = System.DateTime.Now.Year.ToString();
                                }
                            }
                        }
                        strData = string.Format("{0}-{1}-{2}", year, month, day);
                        try
                        {
                            DateTime.Parse(strData);
                        }
                        catch
                        {
                            strData = tempdate;
                        }
                    }
                    #endregion
                }
                else if (dateformat == DataFormat.dayMonthYear)
                {
                    //30JAN12
                    strData = strData.Replace(" ", "").Trim();
                    if (strData.Length == 7)
                    {
                        Match ch = Regex.Match(strData, @"\s*(?<day>\d{2})\s*(?<Month>[A-Za-z]{3})\s*(?<Year>\d{2})\s*", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
                        if (ch.Success)
                        {
                            strData = System.DateTime.Now.Year.ToString().Substring(0, 2) + ch.Groups["Year"].Value.Trim() + "-" + MonthStrToNum(ch.Groups["Month"].Value.Trim()).PadLeft(2, '0') + "-" + ch.Groups["day"].Value.Trim().PadLeft(2, '0');
                        }
                    }
                }
                else if (dateformat == DataFormat.dayMonth)//29AUG
                {
                    //30JAN12
                    strData = strData.Replace(" ", "").Trim();
                    if (strData.Length == 5)
                    {
                        string year = System.DateTime.Now.Year.ToString();
                        string month = MonthStrToNum(strData.Substring(2));
                        string day = strData.Substring(0, 2);
                        strData = year + "-" + month + "-" + day;
                    }
                }
            }
            return strData;
        }

        public static string GetLiuPnr(string strData, int PnrLen)
        {
            if (strData.Replace(" ", "").Replace("\r", "").Replace("\n", "").Length > 6 && strData.Replace(" ", "").Replace("\r", "").Replace("\n", "").Substring(strData.Replace(" ", "").Replace("\r", "").Replace("\n", "").Length - 6).IndexOf("/") != -1)
            {
                strData = strData.Substring(0, strData.LastIndexOf("/"));
            }
            int len = PnrLen;
            bool IsPnr = false;
            List<Char> list = new List<char>();
            List<Char> Ortherlist = new List<char>();
            for (int i = strData.Length - 1; i >= 0; i--)
            {
                if (!IsPnr && (Char.IsLetter(strData, i) || Char.IsDigit(strData, i)))
                {
                    list.Add(strData[i]);
                    if (list.Count == 6)
                    {
                        IsPnr = true;
                        continue;
                    }
                }
                if (IsPnr)
                {
                    Ortherlist.Add(strData[i]);
                }
            }
            Ortherlist.Reverse();
            list.Reverse();
            return new string(Ortherlist.ToArray()) + " " + new string(list.ToArray());
        }
        /// <summary>
        /// 是否存在中文汉字
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        public static bool IsChina(string content)
        {
            string patern = @"[\u4e00-\u9fa5]";
            return Regex.IsMatch(content, patern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 字符串中是否存在英文字母
        /// </summary>
        /// <returns></returns>
        private static bool IsExistLetter(string strCon)
        {
            return Regex.IsMatch(strCon, "[A-Za-z]", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
        }
        /// <summary>
        /// 月份字符串转换到数字
        /// </summary>
        /// <param name="month"></param>
        /// <returns></returns>
        public static string MonthStrToNum(string month)
        {
            if (!string.IsNullOrEmpty(month))
            {
                switch (month)
                {
                    case "JAN": month = "01"; break;
                    case "FEB": month = "02"; break;
                    case "MAR": month = "03"; break;
                    case "APR": month = "04"; break;
                    case "MAY": month = "05"; break;
                    case "JUN": month = "06"; break;
                    case "JUL": month = "07"; break;
                    case "AUG": month = "08"; break;
                    case "SEP": month = "09"; break;
                    case "OCT": month = "10"; break;
                    case "NOV": month = "11"; break;
                    case "DEC": month = "12"; break;
                    default: break;
                }
            }
            return month;
        }


        /// <summary>
        /// 星期字符串到数字
        /// </summary>
        /// <param name="week"></param>
        /// <returns></returns>
        public static string WeekStrToNum(string week, int len)
        {
            if (!string.IsNullOrEmpty(week))
            {
                if (len == 1)
                {
                    switch (week)
                    {
                        case "MON": week = "星期一"; break;
                        case "TUE": week = "星期二"; break;
                        case "WED": week = "星期三"; break;
                        case "THU": week = "星期四"; break;
                        case "FRI": week = "星期五"; break;
                        case "SAT": week = "星期六"; break;
                        case "SUN": week = "星期日"; break;
                        default: break;
                    }
                }
                else if (len == 2)
                {
                    switch (week)
                    {
                        case "MO": week = "1"; break;
                        case "TU": week = "2"; break;
                        case "WE": week = "3"; break;
                        case "TH": week = "4"; break;
                        case "FR": week = "5"; break;
                        case "SA": week = "6"; break;
                        case "SU": week = "7"; break;
                        default: break;
                    }
                }
                else if (len == 3)
                {
                    switch (week)
                    {
                        case "MON": week = "1"; break;
                        case "TUE": week = "2"; break;
                        case "WED": week = "3"; break;
                        case "THU": week = "4"; break;
                        case "FRI": week = "5"; break;
                        case "SAT": week = "6"; break;
                        case "SUN": week = "7"; break;
                        default: break;
                    }
                }
                else if (len == 4)
                {
                    switch (week)
                    {
                        case "MO": week = "1"; break;
                        case "TU": week = "2"; break;
                        case "WE": week = "3"; break;
                        case "TH": week = "4"; break;
                        case "FR": week = "5"; break;
                        case "SA": week = "6"; break;
                        case "SU": week = "0"; break;
                        default: break;
                    }
                }
            }
            return week;
        }



        /// <summary>
        /// 踢掉隐藏字符
        /// </summary>
        /// <param name="strCh"></param>
        /// <returns></returns>
        //private string RemoveHideChar(string strCh)
        //{
        //    string charrs = "                ";
        //    foreach (char item in charrs.ToCharArray())
        //    {
        //        strCh = strCh.Replace(item.ToString(), " ").Replace("", "");
        //    }
        //    strCh = RemoveHideChar(strCh.Replace("NO PNR", " "));
        //    return strCh;
        //}

        /// <summary>
        /// 踢掉含有以下列表项
        /// </summary>
        /// <param name="strPnrN"></param>
        /// <returns></returns>
        private bool IsTiDiaoStr(string strPnrN)
        {
            bool IsContine = false;
            List<string> NotPnrlist = new List<string>();
            NotPnrlist.Add("超时");
            NotPnrlist.Add("服务器异步返回");
            NotPnrlist.Add("服务器忙");
            //手动加
            //。。。。
            foreach (string str in NotPnrlist)
            {
                if (strPnrN.Contains(str))
                {
                    IsContine = true;
                    break;
                }
            }
            return IsContine;
        }
        /// <summary>
        /// 计算折扣 传入Y舱价格 和舱位价 返回折扣(保留2为小数)
        /// </summary>
        /// <param name="strYFare"></param>
        /// <param name="strCabinFare"></param>
        /// <returns></returns>
        public static decimal GetZk(string strYFare, string strCabinFare)
        {
            decimal YFare = 0m, CabinFare = 0m;
            decimal.TryParse(strYFare, out YFare);
            decimal.TryParse(strCabinFare, out CabinFare);
            if (YFare == 0m)
            {
                return 0m;
            }
            decimal zk = CabinFare * 100M / YFare;
            int point = zk.ToString().IndexOf(".");
            if (point != -1)
            {
                int p = int.Parse(zk.ToString().Substring(point + 1, 1));
                if (p > 4)
                {
                    zk = Math.Round(zk, MidpointRounding.AwayFromZero);
                }
                else
                {
                    zk = Math.Round(zk, 2);
                }
            }
            else
            {
                zk = Math.Round(zk, MidpointRounding.AwayFromZero);
            }
            return zk;
        }


        private string FilterRowData(string rtItem)
        {
            if (rtItem.IndexOf("PAT:A") != -1)
            {
                rtItem = rtItem.Substring(0, rtItem.IndexOf("PAT:A"));
            }
            return rtItem;
        }

        /// <summary>
        /// 重组Pnr信息 超过80字符自动换行
        /// </summary>
        /// <param name="PNRData"></param>
        /// <returns></returns>
        public string SplitPnrAutoLine(string PNRData)
        {
            //分割PNR项正则
            string RegSplit = @"(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+)";
            //规范后的Pnr项列表
            List<string> PnrList = new List<string>();
            string PnrOneStr = "";
            int Num = 1;
            string result = string.Empty;
            bool Isadd = false;
            try
            {
                if (!string.IsNullOrEmpty(PNRData))
                {
                    PNRData = RemoveHideChar(PNRData);
                    string[] PnrArr = Regex.Split(PNRData, RegSplit, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    string tempCon = "";
                    string RegSkyPattern = @"\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d{4})\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*(?<orther>[\w\S\.\s]*)";
                    if (PnrArr != null && PnrArr.Length > 0)
                    {
                        int NextIndex = 0;
                        bool IsTuan = false;
                        int a = 0;
                        for (int i = 0; i < PnrArr.Length; i++)
                        {
                            tempCon = PnrArr[i];
                            PnrOneStr = PnrArr[i].Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").ToUpper();
                            try
                            {
                                if (PnrOneStr.Trim() == "") continue;
                                if (!Isadd)
                                {
                                    Num = int.Parse(PnrOneStr);
                                    Isadd = true;
                                    continue;
                                }
                                if (tempCon.Trim().Length > 80 && tempCon.Trim().IndexOf("\n") != -1)
                                {
                                    PnrOneStr = tempCon.Replace("\r", " ").Replace("\n", " ").Trim().Substring(0, 80) + "\r\n" + tempCon.Replace("\r", " ").Replace("\n", " ").Trim().Substring(80);
                                }
                                Match SkyMach = Regex.Match(PnrOneStr.Trim(), RegSkyPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (SkyMach.Success)
                                {
                                    PnrOneStr = Num.ToString().PadLeft(2, ' ') + ".  " + tempCon.Trim();
                                }
                                else
                                {
                                    PnrOneStr = Num.ToString().PadLeft(2, ' ') + "." + tempCon.Trim();
                                }
                                if (Num == 0 && a == 0)
                                {
                                    IsTuan = true;
                                    NextIndex = Num;
                                    PnrList.Add(PnrOneStr);
                                    NextIndex++;
                                }
                                if (NextIndex == Num)
                                {
                                    a++;
                                    PnrList.Add(PnrOneStr);
                                    NextIndex++;
                                }
                                Isadd = false;
                            }
                            catch
                            {
                                try
                                {
                                    PnrList.Add(PnrOneStr);
                                }
                                catch (Exception)
                                {
                                }
                                NextIndex++;
                            }
                        }
                        //处理第一项和最后一项                       
                        if (PnrList.Count > 2)
                        {
                            string First = PnrList[0];
                            List<int> removeList = new List<int>();
                            for (int i = 0; i < PnrList.Count; i++)
                            {
                                if (IsTuan)
                                {
                                    //string RegTuan = @"(?<First>\s*0\s*\.\s*(?<TuanName>[\d|\w|\.|\S]+)\s*(NM(?<PCount>\d{1,3}))?\s*(?<TPNR>\w{5,6})\s*)";
                                    string RegTuan = @"(?<First>\s*0\s*\.\s*[\d|\w|\.|\S]+\s*)";
                                    Match mch = Regex.Match(PnrList[i], RegTuan, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                    if (mch.Success)
                                    {
                                        PnrList[0] = mch.Groups["First"].Value.Trim();
                                        break;
                                    }
                                    else
                                    {
                                        removeList.Add(i);
                                    }
                                }
                                else
                                {
                                    string RegTuan = @"(?<First>\s*1\s*\.\s*[\d|\w|\.|\S]+\s*)";
                                    Match mch = Regex.Match(PnrList[i], RegTuan, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                    if (mch.Success)
                                    {
                                        PnrList[0] = mch.Groups["First"].Value.Trim();
                                        break;
                                    }
                                    else
                                    {
                                        removeList.Add(i);
                                    }
                                }
                            }
                            for (int i = 0; i < removeList.Count; i++)
                            {
                                PnrList.RemoveAt(removeList[i]);
                            }

                            string Last = PnrList[PnrList.Count - 1];
                            string lastPattern = @"(?<last>(\d{1,3}\s*\.\s*.*?\/\w{6}\/[A-Za-z0-9]{6})|(\d{1,3}\s*\.\s*[A-Za-z]{3}\s*\d{3}))";
                            Match mchLast = Regex.Match(Last, lastPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            if (mchLast.Success)
                            {
                                PnrList[PnrList.Count - 1] = mchLast.Groups["last"].Value.Trim();
                            }
                        }
                    }
                }
            }
            finally
            {
                result = string.Join("\r\n", PnrList.ToArray());
                //日志
                LogText.LogWrite(result, "PNRContent");
            }
            return result;
        }



        /// <summary>
        /// 重组Pnr信息
        /// </summary>
        /// <param name="PNRData"></param>
        /// <returns></returns>
        public List<string> SplitPnr(string PNRData)
        {
            //分割PNR项正则
            string RegSplit = @"(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+)";
            //规范后的Pnr项列表
            List<string> PnrList = new List<string>();
            string PnrOneStr = "";
            int Num = 1;
            bool Isadd = false;
            if (!string.IsNullOrEmpty(PNRData))
            {
                PNRData = RemoveHideChar(PNRData);
                string[] PnrArr = Regex.Split(PNRData, RegSplit, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (PnrArr != null && PnrArr.Length > 0)
                {
                    for (int i = 0; i < PnrArr.Length; i++)
                    {
                        PnrOneStr = PnrArr[i].Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").ToUpper();
                        try
                        {
                            if (PnrOneStr.Trim() == "") continue;
                            if (!Isadd)
                            {
                                Num = int.Parse(PnrOneStr);
                                Isadd = true;
                                continue;
                            }
                            PnrOneStr = Num + "." + PnrOneStr;
                            PnrList.Add(PnrOneStr);
                            Isadd = false;
                        }
                        catch
                        {
                            PnrList.Add(PnrOneStr);
                        }
                    }
                }
            }
            return PnrList;
        }

        /// <summary>
        /// 筛选指定编码内容 多个编码内容避免串 
        /// </summary>
        /// <param name="Pnr"></param>
        /// <param name="PnrContent"></param>
        /// <returns></returns>
        public string GetFilterPnrContent(string Pnr, string PNRData, out string errMsg)
        {
            string ReturnData = "";
            errMsg = "";
            //string RegSplit = @"(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+)";
            string RegSplit = @"(\b|\s*)(?<num>[X]?(?<!\:|(ACNY\d{1,4}))\d{1,3})\s*\.\s*(?!(00|\/)\w+)";
            //string RTPattern = @"\s*RT\s*" + Pnr + @"(|PN){0,6}(?=\d{1,3}\.)";
            //string RTPattern = @"(\s|\n|\+)*RT\s*" + Pnr + @"(\|PN)?(\s|\n|\+){0,}(?=\d{1,3}\.)";
            string RTPattern = @"(\s|\n|\+)*RT" + Pnr + @"(\|PN)*?(?=\d{1,3}\.)?";

            if (!string.IsNullOrEmpty(PNRData))
            {
                PNRData = Regex.Replace(PNRData.Replace("|PN", ""), RTPattern, "", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                //规范后的Pnr项列表
                List<string> PnrList = new List<string>();
                List<List<string>> AllPnrList = new List<List<string>>();
                bool IsStartStr = false;
                //分割
                string[] PnrArr = Regex.Split(PNRData, RegSplit, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                string PnrOneStr = string.Empty;
                bool Isadd = false;
                int Num = 1;
                for (int i = 0; i < PnrArr.Length; i++)
                {
                    PnrOneStr = PnrArr[i].Replace("\r\n", " ").Replace("\r", " ").Replace("\n", " ").Trim().ToUpper();
                    try
                    {
                        try
                        {
                            if (PnrOneStr == "" || IsTiDiaoStr(PnrOneStr)) continue;
                            if (!Isadd)
                            {
                                Num = int.Parse(PnrOneStr.Replace("X", "").Trim());
                                Isadd = true;
                                continue;
                            }
                            if (Num > 2)
                            {
                                IsStartStr = true;
                            }
                            if (IsStartStr)
                            {
                                if (Num == 0 || Num == 1)
                                {
                                    AllPnrList.Add(PnrList);
                                    PnrList = new List<string>();
                                    IsStartStr = false;
                                }
                            }
                            PnrOneStr = Num.ToString() + "." + PnrArr[i];
                            // PnrOneStr = Num.ToString() + "." + PnrOneStr;
                            PnrList.Add(PnrOneStr);
                            Isadd = false;
                        }
                        catch (Exception ex)
                        {
                            Isadd = false;
                            errMsg = ex.Message;
                            PnrList.Add(PnrArr[i]);
                        }
                    }
                    catch (Exception ep)
                    {
                        PnrList.Add(PnrArr[i]);
                        errMsg = ep.Message;
                    }
                }
                AllPnrList.Add(PnrList);
                if (AllPnrList.Count > 0)
                {
                    string strData = "";
                    for (int i = 0; i < AllPnrList.Count; i++)
                    {
                        foreach (string item in AllPnrList[i])
                        {
                            strData = item.Replace(" ", "").Replace("\r", "").Replace("\n", "").Trim();
                            if (strData.Contains(Pnr.ToUpper()))
                            {
                                //ReturnData = string.Join(" \r\n", AllPnrList[i].ToArray()).ToUpper();  
                                AllPnrList[i] = RepeatLine(AllPnrList[i]);
                                ReturnData = string.Join(" ", AllPnrList[i].ToArray()).ToUpper();
                                break;
                            }
                        }
                    }
                }
            }
            return ReturnData;
        }

        /// <summary>
        /// 编码内容处理 去掉RT 和 pat
        /// </summary>
        /// <param name="pnrCon"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public string PnrHandle(string pnrCon, out string errmsg)
        {
            pnrCon = RemoveHideChar(pnrCon);
            pnrCon = pnrCon.ToUpper().Replace("NO PNR", " ").Replace(">", "").Replace(" RT ", "");
            string Pattern = "";
            string end = @"(?=\b(?<num>(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+))";
            string RTPattern = @"(?<First>.*?)" + end;
            string ConPattern = @"\s*(?<PnrCon>[\S|\s]+)\s*";
            string PATPattern = @"(?<=\b(?<num>(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+))(?<Last>[\S|\s]+(pat[:| ]a[\S|\s]+)?)";
            Pattern = RTPattern + ConPattern + PATPattern;
            errmsg = "";
            string RTFilter = @"(\s*RT\w{5,6}\s*)|\|";
            string AddFilter = @"\+[\s|\S]+";
            string NotFilter = @"\d{4}\+1";
            string RTPattern1 = @"(\s|\n|\+)*RT\w{5,6}(\|PN)*?(?=\d{1,3}\.)?";
            try
            {
                pnrCon = Regex.Replace(pnrCon.Replace("|PN", "").Replace(" PN  ", ""), RTPattern1, "", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

                Match ch = Regex.Match(pnrCon, Pattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (ch.Success)
                {
                    string First = ch.Groups["First"].Value;
                    if (First.Trim() != "")
                    {
                        First = Regex.Replace(First, RTFilter, "", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    }
                    else
                    {
                        First = " ";
                    }
                    string _pnrCon = ch.Groups["PnrCon"].Value;
                    string Last = ch.Groups["Last"].Value;
                    Last = Regex.Replace(Last, @"\s*[\>]?pat[:| ]a[\S|\s]+", "", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    Last = Regex.Replace(Last, RTFilter, "", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    pnrCon = First + _pnrCon + Last;
                }
                if (pnrCon.IndexOf(">") != -1)
                {
                    pnrCon = pnrCon.Substring(0, pnrCon.IndexOf('>'));
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            /*
            List<string> PnrList = NewSplitPnr(pnrCon);
            PnrList = RepeatLine(PnrList);
            for (int i = 0; i < PnrList.Count; i++)
            {
                Match mch = Regex.Match(PnrList[i], NotFilter, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (!mch.Success)
                {
                    mch = Regex.Match(PnrList[i], AddFilter, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    if (mch.Success)
                    {
                        PnrList[i] = Regex.Replace(PnrList[i], AddFilter, "", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase) + "+\n";
                    }
                }
            }
            pnrCon = string.Join("", PnrList.ToArray());
            */
            return pnrCon;
        }
        /// <summary>
        /// Pat处理 去掉前面 pat:a
        /// </summary>
        /// <param name="PatCon"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public string PatHandle(string PatCon, out string errmsg)
        {
            errmsg = "";
            try
            {
                PatCon = RemoveHideChar(PatCon);
                PatCon = PatCon.ToUpper().Replace("NO PNR", " ");
                string PatPtern = @"[\S|\s]+(?<pat>\>PAT:A[\S|\s]+)";
                Match ch = Regex.Match(PatCon, PatPtern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (ch.Success)
                {
                    PatCon = ch.Groups["pat"].Value;
                }
            }
            catch (Exception ex)
            {
                errmsg = ex.Message;
            }
            return PatCon;
        }

        /// <summary>
        /// 预订编码返回字符串或者RT编码字符串 解析出编码
        /// </summary>
        /// <returns></returns>
        public string GetPNR(string Data, out string errMsg)
        {
            errMsg = "";
            string Pnr = "";
            string strTempData = Data;
            if (!string.IsNullOrEmpty(Data))
            {
                try
                {
                    Data = Data.Replace("\r\n", "\r");
                    bool IsYding = false;
                    if (Data.Contains("航空公司使用自动出票时限"))
                    {
                        IsYding = true;
                    }
                    string RTPattern = @"\b(?<num>(?<!\:)\d{1,3})\s*\.\s*(?!00\w+)";
                    if (!IsYding && Regex.Match(Data, RTPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase).Success)
                    {
                        #region RT编码信息中取编码
                        List<string> Pnrlist = SplitPnr(Data);
                        string[] strList = Pnrlist.ToArray();
                        //航段正则
                        string RegSkyPattern = @"\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}\s*(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d\s*\d\s*\d\s*\d)\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*(?<orther>[\w\S\.\s]*)";
                        //编码正则 
                        string RegPasPattern = @"\s*(?<Name>[a-zA-Z\u4e00-\u9fa5/]+(?=[\u4e00-\u9fa5|\s])?)\s*(?<=[\u4e00-\u9fa5/|\s])(?<Pnr>([a-zA-Z0-9]{3}\s*[a-zA-Z0-9]{3})|.*?)\s*";
                        // string RegPasPattern = @"\s*(?<Name>.+(?=[\u4e00-\u9fa5|\s]))\s*(?<=[\u4e00-\u9fa5|\s])(?<Pnr>[A-Z0-9]{5,6})\s*";
                        //团编码
                        string RegTuan = @"\s*(?<TuanName>[\d|\w|\.|\S]+)\s*(NM(?<PCount>\d{1,3}))?\s*(?<TPNR>\w{5,6})\s*";// @"(?<PCount>\d{1,3})\s*(?<TuanName>\w+)";
                        bool IsStart = false;
                        bool IsEnd = false;
                        string str = string.Empty;
                        string LastStrPas = string.Empty;

                        for (int i = 0; i < strList.Length; i++)
                        {
                            str = strList[i];
                            Match SkyMach = Regex.Match(str.Trim(), RegSkyPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            if (SkyMach.Success)
                            {
                                if (strList.Length > 2 && !IsStart && i > 0)
                                {
                                    LastStrPas = strList[i - 1].Trim();
                                }
                                IsStart = true;
                            }
                            else
                            {
                                if (str.Trim().StartsWith("0."))
                                {
                                    Match mch = Regex.Match(str.Trim(), RegTuan, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                    if (mch.Success)
                                    {
                                        Pnr = mch.Groups["TPNR"].Value;
                                        break;
                                    }
                                }
                                else
                                {
                                    if (IsStart && !IsEnd)
                                    {
                                        IsEnd = true;
                                        IsStart = false;
                                        bool IsPnrValie = false;
                                        if (LastStrPas != "" && LastStrPas.IndexOf(" ") != -1)
                                        {
                                            LastStrPas = GetLiuPnr(LastStrPas, 6);
                                            LastStrPas = Regex.Replace(LastStrPas, RTPattern, "");
                                            int index = LastStrPas.LastIndexOf(' ');
                                            string _Pnr = LastStrPas.Substring(index).Trim().Replace(" ", "");
                                            string PasName = LastStrPas.Substring(0, index).Trim();
                                            if (_Pnr.Length == 5 || _Pnr.Length == 6)
                                            {
                                                IsPnrValie = true;
                                                Pnr = _Pnr;
                                                break;
                                            }
                                        }
                                        if (!IsPnrValie && Pnr.Length != 5 && Pnr.Length != 6)
                                        {
                                            LastStrPas = GetLiuPnr(LastStrPas, 6);
                                            LastStrPas = Regex.Replace(LastStrPas, RTPattern, "");
                                            Match PasCh = Regex.Match(LastStrPas.Trim(), RegPasPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                            if (PasCh.Success)
                                            {
                                                IsPnrValie = true;
                                                Pnr = PasCh.Groups["Pnr"].Value.Trim().Replace(" ", "");
                                            }
                                        }
                                        if (!IsPnrValie)
                                        {
                                            Match PasCh = Regex.Match(str.Trim(), RegPasPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                            if (PasCh.Success)
                                            {
                                                Pnr = PasCh.Groups["Pnr"].Value.Trim().Replace(" ", "");
                                                break;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (Pnr == "NO PNR" || Pnr == "")
                        {
                            Pnr = "";
                            errMsg = "取编码失败0";
                        }
                        #endregion
                    }
                    else
                    {
                        #region 定编码返回数据中取编码
                        string dingPnrData = Data.Replace("NO PNR", " ").Replace("\r\n", "^").Replace("\n", "^").Replace("\r", "^");
                        string _pnr = string.Empty;
                        _pnr = _getPnr(dingPnrData.Trim());
                        if (_pnr.Trim().Length < 6 || !IsPnr(_pnr.Trim()))
                        {
                            string pnrPattern0 = @"\s*(?<leg>(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d{4})\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*)+\s*(?<Pnr>\w{5,6})\s*(?=-EOT)";
                            Match mch = Regex.Match(dingPnrData.Replace("^", "\r"), pnrPattern0, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            if (mch.Success)
                            {
                                _pnr = mch.Groups["Pnr"].Value.Trim();
                            }

                            if (_pnr.Trim().Length < 6 || !IsPnr(_pnr.Trim()))
                            {
                                //第一次取
                                if (dingPnrData.IndexOf("-EOT") != -1)
                                {
                                    _pnr = dingPnrData.Substring(0, dingPnrData.IndexOf("-EOT")).Trim();
                                }
                            }
                            if (_pnr.Trim().Length < 6 || !IsPnr(_pnr.Trim()))
                            {
                                //第二次取
                                if (dingPnrData.IndexOf("-") != -1)
                                {
                                    if (dingPnrData.IndexOf("-") - 8 > 0)
                                    {
                                        _pnr = dingPnrData.Substring(dingPnrData.IndexOf("-") - 8, 8).Trim();
                                    }
                                    else
                                    {
                                        if (dingPnrData.Length > 7)
                                        {
                                            _pnr = dingPnrData.Substring(0, 7).Trim();
                                        }
                                    }
                                }
                            }
                            if (_pnr.Trim().Length < 6 || !IsPnr(_pnr.Trim()))
                            {
                                //第三次取
                                string pnrPattern = @"\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d{4})\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*(?<Pnr>\w{5,6})\s*";
                                Match mch0 = Regex.Match(dingPnrData.Replace("^", "\r"), pnrPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                                if (mch0.Success)
                                {
                                    _pnr = mch0.Groups["Pnr"].Value.Trim();
                                }
                            }
                            ////再取
                            if (_pnr.Trim().Length < 6 || !IsPnr(_pnr.Trim()))
                            {
                                string pnrPattern = @"(?<left>(.|\r|\n)+?)(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d{4})\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})";
                                Match mch4 = Regex.Match(dingPnrData.Replace("^", "\r"), pnrPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                                if (mch4.Success)
                                {
                                    string data = mch4.Groups["left"].Value.Trim();
                                    pnrPattern = @"(?<pnr>[A-Z0-9a-z]{5,6}).*?";
                                    mch = Regex.Match(data, pnrPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                                    if (mch.Success)
                                    {
                                        _pnr = mch.Groups["pnr"].Value.Trim();
                                    }
                                }
                            }
                        }
                        Pnr = _pnr;
                        #endregion
                    }
                }
                catch (Exception ex)
                {
                    LogText.LogWrite(string.Format("[GetPNR]解析数据:{0}\r\n", strTempData), "Err_GetPNR");
                    throw ex;
                }
            }
            else
            {
                errMsg = "取编码失败1";
            }
            if ((Pnr.Trim().Length != 5 && Pnr.Trim().Length != 6) || Pnr.Trim() == "NO PNR")
            {
                Pnr = "";
                errMsg = "取编码失败2";
            }
            if (Pnr.ToUpper() == "AIRLIN" || Pnr.ToUpper() == "DATE^^" || Pnr.Contains("^") || !IsPnr(Pnr))
            {
                Pnr = "";
            }
            return Pnr;
        }



        #region 格式化内容，每行80
        private string FormatResContent(string rescontent)
        {
            string tmpcontent = "";
            string tmpcontent1 = rescontent;
            string tmpstr = "";
            int pos = -1;
            pos = tmpcontent1.IndexOf('\r');

            int pos1 = tmpcontent1.IndexOf('^');
            if ((pos1 == -1) && (pos == -1))
            {
                tmpstr = tmpcontent1;
                while (tmpstr.Length > 80)
                {
                    tmpcontent += tmpstr.Substring(0, 80) + "^";
                    tmpstr = tmpstr.Substring(80);
                }

                tmpcontent += tmpstr;
            }
            else if (pos1 != -1)
            {
                tmpcontent = tmpcontent1;
            }
            else
            {
                if (pos == -1)
                {
                    tmpstr = tmpcontent1;
                    while (tmpstr.Length > 80)
                    {
                        tmpcontent += tmpstr.Substring(0, 80) + "^";
                        tmpstr = tmpstr.Substring(80);
                    }

                    tmpcontent += tmpstr;
                }
                else
                {
                    while (pos != -1)
                    {
                        tmpstr = tmpcontent1.Substring(0, pos);
                        if (tmpstr.Length > 80)
                        {
                            string tmpstr2 = tmpstr;
                            while (tmpstr2.Length > 80)
                            {
                                //添加换行符
                                tmpcontent += tmpstr2.Substring(0, 80) + "^";// + tmpstr.Substring(80) + "\r");
                                tmpstr2 = tmpstr2.Substring(80);
                            }
                            if (tmpstr2.Length > 0)
                            {
                                tmpcontent += tmpstr2 + "\r";
                            }
                        }
                        else
                        {
                            tmpcontent += (tmpstr + "\r");
                        }

                        tmpcontent1 = tmpcontent1.Substring(pos + 1);
                        pos = tmpcontent1.IndexOf('\r');
                    }
                }
            }
            if (tmpcontent == "")
            {
                tmpcontent = rescontent;
            }
            return tmpcontent;
        }
        #endregion 格式化内容，每行80

        private string _getPnr(string MessageInfo)
        {
            //航空公司配置
            //FVTWY - 请及时出票, 自动出票时限: 12SEP09/2140
            //3U8881  D TU22SEP  CTUPEK HK1   0720 0940 
            //- ADD SSR TKNE FOR NEW ET SEGMENT 
            //|1
            //代理人配置
            //SGP7V -EOT SUCCESSFUL, BUT ASR UNUSED FOR 1 OR MORE SEGMENTS
            //CA4117  Y SA12SEP  CTUPEK DK1   1700 1920 
            //航空公司使用自动出票时限, 请检查PNR  

            //  CA4231  Y MO22OCT  CTUINC HK1   0840 1005 
            //HNJWBM -  航空公司使用自动出票时限, 请检查PNR 
            //*** 预订酒店指令HC, 详情  HC:HELP   ***  

            MessageInfo = FormatResContent(MessageInfo);
            int tmpi = -1;
            tmpi = MessageInfo.ToUpper().IndexOf("EOT SUCCESSFUL");
            int tmpi2 = -1;
            tmpi2 = MessageInfo.ToUpper().IndexOf("- 请及时出票");
            int tmpi3 = -1;
            tmpi3 = MessageInfo.ToUpper().IndexOf("-  航空公司使用");
            if (tmpi != -1)
            {
                if (tmpi - 8 >= 0)
                {
                    MessageInfo = MessageInfo.Substring(tmpi - 8, 6).Trim();     // 返回PNR编号
                }
                else
                {
                    MessageInfo = MessageInfo.Substring(tmpi - 7, 6).Trim();     // 返回PNR编号
                }
            }
            else if (tmpi2 != -1)
            {
                MessageInfo = MessageInfo.Substring(0, 6).Trim();
            }
            else if (tmpi3 != -1)
            {
                MessageInfo = MessageInfo.Substring(tmpi3 - 7, 6).Trim();
            }
            else
            {
                string[] PNRInfoList = MessageInfo.Split(new char[1] { '^' });
                if (PNRInfoList.Length > 2)
                {
                    for (int k = PNRInfoList.Length - 1; k > 0; k--)
                    {
                        if ((PNRInfoList[k].Trim().Length > 5) && (PNRInfoList[k].Trim().IndexOf("*") == -1))
                        {
                            MessageInfo = PNRInfoList[k].Trim().Substring(0, 6).Trim();
                            break;
                        }
                    }
                }
                else
                {
                    MessageInfo = "";    //订座失败
                }
            }
            return MessageInfo;
        }
        /// <summary>
        /// 获取RT数据中的大编码
        /// </summary>
        /// <param name="rtData"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string GetBigCode(string rtData, out string msg)
        {
            msg = "";
            if (string.IsNullOrEmpty(rtData))
            {
                msg = "rt数据不能为空！";
                return "";
            }
            string BigCode = "";
            string RegBigCodePattern = @"\s*(?<=-CA-|RMK\s*CA\/)(?<BigCode>\w{5,6})\s*";
            Match mch = Regex.Match(rtData, RegBigCodePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (mch.Success)
            {
                BigCode = mch.Groups["BigCode"].Value.Trim().Replace("+", "");
            }
            else
            {
                msg = "未能取到大编码！";
            }
            return BigCode;
        }

        /// <summary>
        /// 获取大配置RT数据Office
        /// </summary>
        /// <param name="rtData"></param>
        /// <returns></returns>
        public string GetOffice(string PNR, string BigRTData, out string msg)
        {
            string Office = "";
            msg = "";
            //大系统内容提取Office正则
            //PEK1E/HXH4DF/CTU324
            string RegBigCodeOfficePattern = @".*?\/" + PNR.Trim().Replace(" ", "") + @"\/(?<Office>[A-Za-z]{3}\s*\d{3})";
            Match OfficeMach = Regex.Match(BigRTData, RegBigCodeOfficePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (OfficeMach.Success)
            {
                Office = OfficeMach.Groups["Office"].Value.Trim();
            }
            if (Office == "")
            {
                msg = PNR + "未能取到Office";
            }
            return Office;
        }

        /// <summary>
        /// 提取编码中授权的Office
        /// </summary>
        /// <param name="rtData"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public List<string> GetAuthOffice(string rtData, out string msg)
        {
            msg = "";
            //授权的Office号
            string RMK_AUTHPattern = @"(?<=RMK\s*TJ\s*AUTH)\s*(?<AuthOffice>\w{3}\d{3})\s*";
            List<string> listAUTH = new List<string>();
            Match OfficeMach = Regex.Match(rtData, RMK_AUTHPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (OfficeMach.Success)
            {
                listAUTH.Add(OfficeMach.Groups["AuthOffice"].Value.Trim());
            }
            if (listAUTH.Count == 0)
            {
                msg = "未能取到授权Office";
            }
            return listAUTH;
        }
        /// <summary>
        /// 获取Office
        /// </summary>
        /// <param name="rtData"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string GetOffcie(string rtData, out string msg)
        {
            string Office = "";
            msg = "";
            string OfficePattern = @"^(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?<Office>[A-Za-z]{3}\s*\d{3})\s*$";
            List<string> pnrList = SplitPnr(rtData);
            pnrList.Reverse();//倒序
            foreach (string strItem in pnrList)
            {
                Match OfficeMach = Regex.Match(strItem.Replace(" ", "").Trim(), OfficePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (OfficeMach.Success)
                {
                    Office = OfficeMach.Groups["Office"].Value.Trim();
                    break;
                }
            }
            if (Office == "")
            {
                msg = "未能取到Office";
            }
            return Office;
        }
        /// <summary>
        /// 获取Pnr状态用"|"隔开 HK1|HK2
        /// </summary>
        /// <param name="rtData"></param>
        /// <returns></returns>
        public string GetPnrStatus(string rtData, out string msg)
        {
            string Status = "";
            msg = "";
            string RegSkyPattern = @"\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}\s*(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d{4})\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*(?<orther>[\w\S\.\s]*)";
            List<string> Pnrlist = this.SplitPnr(rtData);
            if (Pnrlist.Count > 0)
            {
                List<string> statuslist = new List<string>();
                foreach (string strItem in Pnrlist)
                {
                    Match SkyMach = Regex.Match(strItem, RegSkyPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    if (SkyMach.Success)
                    {
                        statuslist.Add(SkyMach.Groups["state"].Value.Trim());
                    }
                }
                Status = string.Join("|", statuslist.ToArray());
            }
            if (Status == "")
            {
                msg = "未能取到PNR状态";
            }
            return Status.ToUpper();
        }

        /// <summary>
        /// 判断婴儿备注是否成功 INFData为备注婴儿返回数据
        /// </summary>
        /// <param name="INFData"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool INFMarkIsOK(string INFData, out string msg)
        {
            msg = "";
            string RegSkyPattern = @"\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1})\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d{4})\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*(?<orther>[\w\S\.\s]*)";
            Match mchFd = Regex.Match(INFData, RegSkyPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return mchFd.Success;
        }

        /// <summary>
        /// Pnr数据解析
        /// </summary>
        /// <param name="PNR">小编码</param>
        /// <param name="PNRData">Pnr内容</param>
        /// <param name="IsBigContent">是否为大系统内容 默认false</param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public PnrModel GetPNRInfo(string PNR, string PNRData, bool IsBigContent, out string errMsg)
        {
            oldPnrContent = PNRData;
            errMsg = "";
            PnrModel PM = new PnrModel();
            if (!string.IsNullOrEmpty(PNRData) && PNRData.Trim().Replace("\r", "").Replace("\n", "").ToUpper() == "NO PNR")
            {
                PM._FormatPnrContent = "NO PNR";
                errMsg = "NO PNR";
                return PM;
            }
            if (PNRData.Contains("值不能为空"))//错误信息
            {
                PM._ErrMsg = PNRData;
                return PM;
            }
            if (IsValidateData(PNR, PNRData, IsBigContent, PM, out errMsg))
            {
                PNR = RemoveHideChar(PNR.ToUpper());
                PNRData = RemoveHideChar(PNRData.ToUpper());
                PM._FormatPnrContent = PNRData;
                //if (false)//(this.IsFilterCon)
                //{
                //    //过滤多个rt内容字符串
                //    PNRData = GetFilterPnrContent(PNR, PNRData, out errMsg);
                //}
                PNRData = FormatPnr(PNR, PNRData, PM);
                errMsg = PM._ErrMsg;
            }
            else
            {
                PM._ErrMsg = errMsg;
            }
            errMsg = errMsg.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            return PM;
        }

        /// <summary>
        /// Pnr数据解析 输出DataSet 和PnrModel 实体
        /// </summary>
        /// <param name="PNR"></param>
        /// <param name="PNRData"></param>
        /// <param name="IsBigContent"></param>
        /// <param name="dataset"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public PnrModel GetPNRInfo(string PNR, string PNRData, bool IsBigContent, out DataSet dataset, out string errMsg)
        {
            oldPnrContent = PNRData;
            errMsg = "";
            dataset = new DataSet();
            PnrModel PM = new PnrModel();
            if (!string.IsNullOrEmpty(PNRData) && PNRData.Trim().Replace("\r", "").Replace("\n", "").ToUpper() == "NO PNR")
            {
                PM._FormatPnrContent = "NO PNR";
                errMsg = "NO PNR";
                return PM;
            }
            if (IsValidateData(PNR, PNRData, IsBigContent, PM, out errMsg))
            {
                PNR = RemoveHideChar(PNR.ToUpper());
                PNRData = RemoveHideChar(PNRData.ToUpper());
                PM._FormatPnrContent = PNRData;
                if (this.IsFilterCon)
                {
                    //过滤多个rt内容字符串
                    PNRData = GetFilterPnrContent(PNR, PNRData, out errMsg);
                }
                PNRData = FormatPnr(PNR, PNRData, PM);
                dataset = CreateDatatable(PM);
                errMsg = PM._ErrMsg;
            }
            errMsg = errMsg.Replace(" ", "").Replace("\r", "").Replace("\n", "");
            return PM;
        }
        private bool IsValidateData(string PNR, string PNRData, bool IsBigContent, PnrModel PM, out string errMsg)
        {
            bool IsOk = false;
            errMsg = "";
            this.IsBigContent = IsBigContent;
            //开始解析
            //...            
            if (string.IsNullOrEmpty(PNR) || string.IsNullOrEmpty(PNRData))
            {
                errMsg = "编码为空或者编码内容为空";
                return IsOk;
            }
            PNR = RemoveHideChar(PNR.ToUpper());
            PNRData = RemoveHideChar(PNRData.ToUpper());
            if (PNRData.Contains("NO PNR"))
            {
                if (PNRData.Replace("NO PNR", "").Trim().Length == 0)
                {
                    errMsg = "该编码不存在,无法进行解析";
                    //PM._FormatPnrContent = PNRData.Replace("NO PNR", "");
                    return IsOk;
                }
            }
            if (PNR.Length < 5 || PNRData.Length < 30)
            {
                errMsg = "配置异常，提取编码内容失败,请稍后再试!";//"编码格式错误或者内容为非RT数据";
                return IsOk;
            }
            //|| PNRData.ToUpper().Contains("TRANSACTION IN PROGRESS")
            //未找到可用配置...
            if (PNRData.IndexOf("未找到可用配置") != -1 || PNRData.IndexOf("地址无效") != -1 || PNRData.IndexOf("无法从传输连接中读取数据") != -1 || PNRData.IndexOf("无法连接") != -1 || PNRData.IndexOf("强迫关闭") != -1 || PNRData.IndexOf("由于") != -1 || PNRData.IndexOf("超时") != -1 || PNRData.IndexOf("服务器忙") != -1 || PNRData.ToUpper().Contains("WSACancelBlockingCall") || PNRData.ToUpper().Contains(" FORMAT ") || PNRData.ToUpper().Contains("为空") || PNRData.ToUpper().Contains("对象的实例"))
            {
                errMsg = "提取编码（" + PNR + "）信息失败,错误信息:" + PNRData;
                PM._ErrMsg = errMsg;
                return IsOk;
            }
            if (!IsBigContent && !PNRData.Replace(" ", "").Replace("\r", "").Replace("\n", "").Trim().Contains(PNR))
            {
                // errMsg = "该RT数据不是该编码" + PNR + "的RT数据";
                errMsg = "该" + PNR + "提取编码内容异常,请稍后重试！";
                return IsOk;
            }
            //if (PNRData.Contains("CANCELLED") && PNRData.Contains(PNR))
            //{
            //    errMsg = "该编码(" + PNR + ")已被取消,无法进行解析";
            //    PM._FormatPnrContent = PNRData;
            //    return IsOk;
            //}
            IsOk = true;
            return IsOk;
        }

        private DataSet CreateDatatable(PnrModel PM)
        {
            DataSet ds = new DataSet();
            DataTable tb1 = new DataTable();
            List<string> colNameList = new List<string>();
            colNameList.Add("乘客姓名");
            colNameList.Add("乘客类型");
            colNameList.Add("乘客证件号");
            colNameList.Add("乘客票号");
            colNameList.Add("航空公司");
            colNameList.Add("航班号");
            colNameList.Add("舱位");
            colNameList.Add("子舱位");
            colNameList.Add("起飞日期");
            colNameList.Add("到达日期");
            colNameList.Add("起飞时间");
            colNameList.Add("到达时间");
            colNameList.Add("出发城市");
            colNameList.Add("到达城市");
            colNameList.Add("小编码");
            colNameList.Add("大编码");
            colNameList.Add("编码类型");
            colNameList.Add("团编码名称");
            colNameList.Add("编码状态");
            colNameList.Add("Office");
            colNameList.Add("授权Office");
            tb1.TableName = "乘客表";
            foreach (string ColName in colNameList)
            {
                tb1.Columns.Add(ColName, typeof(string));
            }
            string carry = string.Empty;
            string Flight = string.Empty;
            string Seat = string.Empty;
            string childSeat = string.Empty;
            string startDate = string.Empty;
            string endDate = string.Empty;
            string startTime = string.Empty;
            string endTime = string.Empty;
            string fromCity = string.Empty;
            string toCity = string.Empty;
            string pnrStatus = string.Empty;
            foreach (PassengerInfo PInfo in PM._PassengerList)
            {
                DataRow dr = tb1.NewRow();
                dr["乘客姓名"] = PInfo.PassengerName;
                dr["乘客类型"] = (PInfo.PassengerType == "1") ? "成人" : (PInfo.PassengerType == "2" ? "儿童" : PInfo.PassengerType == "3" ? "婴儿" : "");
                dr["乘客证件号"] = PInfo.SsrCardID;
                dr["乘客票号"] = PInfo.TicketNum;
                dr["小编码"] = PM.Pnr;
                dr["大编码"] = PM._BigPnr;
                dr["编码类型"] = PM._PnrType == "1" ? "普通编码" : PM._PnrType == "2" ? "团编码" : "";
                dr["Office"] = PM._Office;
                dr["团编码名称"] = PM._Tuan.TuanName;
                if (PM._RMK_TJ_AUTH_List.Count > 0)
                {
                    dr["授权Office"] = string.Join("|", PM._RMK_TJ_AUTH_List.ToArray());
                }
                else
                {
                    dr["授权Office"] = "";
                }
                carry = string.Empty;
                Flight = string.Empty;
                Seat = string.Empty;
                childSeat = string.Empty;
                startDate = string.Empty;
                endDate = string.Empty;
                startTime = string.Empty;
                endTime = string.Empty;
                fromCity = string.Empty;
                toCity = string.Empty;
                pnrStatus = string.Empty;
                foreach (LegInfo leg in PM._LegList)
                {
                    carry = carry + leg.AirCode + "|";
                    Flight = Flight + leg.FlightNum + "|";
                    Seat = Seat + leg.Seat + "|";
                    childSeat = childSeat + leg.ChildSeat + "|";
                    startDate = startDate + leg.FlyDate1 + "|";
                    endDate = endDate + leg.FlyDateE + "|";
                    startTime = startTime + leg.FlyStartTime + "|";
                    endTime = endTime + leg.FlyEndTime + "|";
                    fromCity = fromCity + leg.FromCode + "|";
                    toCity = toCity + leg.ToCode + "|";
                    pnrStatus = pnrStatus + leg.PnrStatus + "|";
                }
                dr["航空公司"] = carry.Trim('|');
                dr["航班号"] = Flight.Trim('|');
                dr["舱位"] = Seat.Trim('|');
                dr["子舱位"] = childSeat.Trim('|');
                dr["起飞日期"] = startDate.Trim('|');
                dr["到达日期"] = endDate.Trim('|');
                dr["起飞时间"] = startTime.Trim('|');
                dr["到达时间"] = endTime.Trim('|');
                dr["出发城市"] = fromCity.Trim('|');
                dr["到达城市"] = toCity.Trim('|');
                dr["编码状态"] = pnrStatus.Trim('|');
                tb1.Rows.Add(dr);
            }
            ds.Tables.Add(tb1);
            return ds;
        }

        /// <summary>
        /// pat数据解析 
        /// </summary>
        /// <param name="PatData"></param>
        /// <returns></returns>
        public PatModel GetPATInfo(string PatData, out string ErrMsg)
        {
            ErrMsg = "";
            string oldPatCon = string.Empty;
            PatModel PAT = new PatModel();
            List<PatInfo> childPat = new List<PatInfo>();
            try
            {
                if (string.IsNullOrEmpty(PatData) || !PatData.Contains("PAT:A"))
                {
                    ErrMsg = "该PAT内容格式不规范,无法进行解析！";
                    return PAT;
                }
                PatData = RemoveHideChar(PatData);
                oldPatCon = PatData;
                PatData = PatData.ToUpper().Replace("\r\n", " ").Replace("\n", " ").Replace("\r", " ").Replace("NO PNR", "");
                if (PatData.Contains("PAT:A*CH"))
                {
                    PAT.PatType = "2";
                }
                else if (PatData.Contains("PAT:A*IN"))
                {
                    PAT.PatType = "3";
                }
                //pat价格提取正则
                //string pattern = @"\s*.*?(?<Num>\d{2})\s*(?<Seat>.+?)\s*FARE:(?<Fare>CNY[\d|\r|\n| ]+\.00|TEXEMPT|TEXEMPTCN)\s*TAX:(?<TAX>CNY[\d|\r|\n| ]+\.00|TEXEMPT|TEXEMPTCN)\s*YQ:(?<YQ>CNY[\d|\r|\n| ]+\.00|TEXEMPT|TEXEMPTCN|TEXEMPTYQ)\s*TOTAL:(?<TOTAL>[\d|\r|\n| ]+\.00)\s*.*?SFC:(?<SFCNum>\d{2})\s*";
                string pattern = @"\s*.*?(?<Num>\d{2})\s*(?<Seat>.+?)\s*FARE:(?<Fare>CNY[\d|\r|\n|\-| ]+\.(00|[\d|\r|\n|\-| ]+)|TEXEMPT|TEXEMPTCN)\s*TAX:(?<TAX>CNY[\d|\r|\n|\-| ]+\.(00|[\d|\r|\n|\-| ]+)|TEXEMPT|TEXEMPTCN)\s*YQ:(?<YQ>CNY[\d|\r|\n|\-| ]+\.(00|[\d|\r|\n|\-| ]+)|TEXEMPT|TEXEMPTCN|TEXEMPTYQ)\s*TOTAL:(?<TOTAL>[\d|\r|\n|\-| ]+\.(00|[\d|\r|\n|\-| ]+))\s*.*?SFC:(?<SFCNum>\d{1,2})\s*(.*?SFN\:\d{1,2})?\s*";
                MatchCollection matches = Regex.Matches(PatData, pattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

                bool IsDobleChild = false;//是否纯子舱位
                foreach (Match ch in matches)
                {
                    if (ch.Success)
                    {
                        PatInfo pat = new PatInfo();
                        pat.SerialNum = ch.Groups["Num"].Value;
                        string Seat = ch.Groups["Seat"].Value.ToUpper();//.Split('/')[0];
                        Match m = Regex.Match(Seat, @"(?<child>[\s|\+]?[A-Za-z]1[\+|\s]?)(?!\w+)", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        bool isChild = false;
                        if (m.Success)
                        {
                            isChild = true;
                            if (Seat.Contains("+"))
                            {
                                string[] strArr = Seat.Split('+');
                                if (strArr.Length == 2)
                                {
                                    if (!IsDobleChild)
                                    {
                                        //两程都是子舱位 是否纯子舱位
                                        if (strArr[0].EndsWith("1") && strArr[1].EndsWith("1"))
                                        {
                                            isChild = true;
                                            IsDobleChild = true;
                                        }
                                        else
                                        {
                                            if (!IsDobleChild)
                                            {
                                                Seat = m.Groups["child"].Value.Replace("+", "").Replace("/", "").Trim();
                                            }
                                        }
                                    }
                                    else
                                    {
                                        isChild = false;
                                    }
                                }
                            }
                            else
                            {
                                Seat = m.Groups["child"].Value.Replace("+", "").Replace("/", "").Trim();
                            }
                        }
                        pat.SeatGroup = Seat.ToUpper();
                        pat.Fare = ch.Groups["Fare"].Value.Replace("CNY", "").Replace("TEXEMPTCN", "0").Replace("TEXEMPT", "0").Replace("-", "0");
                        pat.TAX = ch.Groups["TAX"].Value.Replace("CNY", "").Replace("TEXEMPTCN", "0").Replace("TEXEMPT", "0").Replace("-", "0");
                        pat.RQFare = ch.Groups["YQ"].Value.Replace("CNY", "").Replace("TEXEMPTYQ", "0").Replace("TEXEMPTCN", "0").Replace("TEXEMPT", "0").Replace("-", "0");
                        pat.Price = ch.Groups["TOTAL"].Value.Replace("CNY", "").Replace("TEXEMPTCN", "0").Replace("TEXEMPT", "0").Replace("-", "0");
                        pat.SFC = "SFC:" + pat.SerialNum;
                        pat.PriceType = PAT.PatType;
                        decimal _p = 0m;
                        decimal.TryParse(pat.Price, out _p);
                        if (_p >= PAT.MaxPrice || _p < 0)
                        {
                            pat.IsOverMaxPrice = true;
                            if (!PAT.IsOverMaxPrice)
                            {
                                PAT.IsOverMaxPrice = true;
                            }
                        }
                        PAT.PatList.Add(pat);
                        if (IsDobleChild && PAT.ChildPat == null)
                        {
                            // PAT.ChildPat = pat;
                            childPat.Add(pat);
                        }
                        else
                        {
                            if (!IsDobleChild)
                            {
                                if ((pat.SeatGroup.Trim().Length == 2 && pat.SeatGroup.Trim().EndsWith("1")) || (Seat.EndsWith("1") && isChild))
                                {
                                    //PAT.ChildPat = pat;
                                    childPat.Add(pat);
                                }
                            }
                        }
                        if (pat.SeatGroup.ToUpper() == "YCH")
                        {
                            //PAT.ChildYPat = pat;
                            childPat.Add(pat);
                        }
                    }
                }
                //排序从低到高排序
                PAT.PatList.Sort(delegate(PatInfo pat1, PatInfo pat2)
                {
                    decimal d1 = 0m;
                    decimal d2 = 0m;
                    decimal.TryParse(pat1.Fare, out d1);
                    decimal.TryParse(pat2.Fare, out d2);
                    return decimal.Compare(d1, d2);
                });
                //PAT内容
                PAT.PatCon = oldPatCon;
                //多个子舱位取低价格
                if (childPat.Count > 0)
                {
                    PAT.ChildPat = childPat[0];
                    foreach (PatInfo item in childPat)
                    {
                        if (decimal.Parse(item.Fare) < decimal.Parse(PAT.ChildPat.Fare))
                        {
                            PAT.ChildPat = item;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ErrMsg = ex.Message + "|\r\n" + ex.StackTrace.ToString();
            }
            return PAT;
        }


        /// <summary>
        /// 过滤PAT中不符合儿童的PAT价格（儿童中包含成人的特殊产品的价格）
        /// </summary>
        /// <param name="pat"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public List<PatInfo> FilterChdPAT(List<PatInfo> ChdPatList)
        {
            List<PatInfo> resultList = new List<PatInfo>();
            if (ChdPatList != null && ChdPatList.Count > 1)
            {
                List<PatInfo> lstPat = new List<PatInfo>();
                foreach (PatInfo pat in ChdPatList)
                {
                    //儿童中不包含成人的特殊产品
                    if (!(pat.SeatGroup.Contains("/") && pat.SeatGroup.Replace("+", "").Trim().Length > 6))
                    {
                        resultList.Add(pat);
                    }
                }
                if (resultList.Count == 0)
                {
                    resultList.Add(ChdPatList[0]);
                }
            }
            else
            {
                if (ChdPatList != null && ChdPatList.Count == 1)
                {
                    resultList.Add(ChdPatList[0]);
                }
            }
            return resultList;
        }

        /// <summary>
        /// Pnr内容中获取航段字符串 返回一个集合
        /// </summary>
        /// <returns></returns>
        public List<string> GetPnrConLegStr(string PnrCon)
        {
            List<string> leglist = new List<string>();
            string left = @"(?<left>(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+)\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d\s*\d\s*\d\s*\d)\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*)";
            string right = @"(?<right>(.|\n|\r)*?)";
            string RegSkyPattern = @"(?<Leg>" + left + right + @")(?=(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+))";
            try
            {
                MatchCollection matches = Regex.Matches(PnrCon, RegSkyPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (Match ch in matches)
                {
                    if (ch.Success)
                    {
                        ////找到的航段     
                        string strSky = ch.Groups["Leg"].Value;
                        leglist.Add(strSky);
                    }
                }
            }
            catch (Exception)
            {
            }
            return leglist;
        }


        /// <summary>
        /// 移除Pnr中如果存在子舱位则移除 不存在不处理
        /// </summary>
        /// <param name="PnrCon">Pnr内容</param>
        /// <param name="IsChildSeat">该编码中是否存在子舱位</param>
        /// <returns></returns>
        public string RemoveChildSeat(string PnrCon, out bool IsChildSeat)
        {
            IsChildSeat = false;
            string _PnrCon = PnrCon;
            string Pleft = @"(?<left>(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+)\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d\s*\d\s*\d\s*\d)\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*)";
            string Pright = @"(?<right>(.|\n|\r)*?)";
            string RegSkyPattern = @"(?<Leg>" + Pleft + Pright + @")(?=(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+))";
            try
            {
                MatchCollection matches = Regex.Matches(PnrCon, RegSkyPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                foreach (Match ch in matches)
                {
                    if (ch.Success)
                    {
                        //处理航段子舱位信息
                        string strSky = ch.Groups["Leg"].Value;//找到的航段
                        string left = ch.Groups["left"].Value;//航段左边部分
                        string right = ch.Groups["right"].Value;//航段右边部分
                        int rightLen = right.Length;//右边部分字符数
                        string seat = ch.Groups["seat"].Value;//航段舱位
                        List<string> Otherlist = new List<string>();
                        Otherlist.AddRange(strSky.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries));
                        string childSeat = seat + "1";
                        if (Otherlist.Contains(childSeat))
                        {
                            //存在子舱位 替换子舱位为空
                            right = right.Replace(" " + childSeat + " ", "").PadRight(rightLen, ' ');
                            IsChildSeat = true;
                        }
                        string NewSky = left + right;
                        _PnrCon = _PnrCon.Replace(strSky, NewSky);
                    }
                }
            }
            catch (Exception)
            {
            }
            return _PnrCon;
        }
        /// <summary>
        /// 根据价格拼接成的Pat字符串 一个价格
        /// </summary>
        /// <param name="patInfo"></param>
        /// <returns></returns>
        public string NewPatData(PatInfo patInfo)
        {
            string strNewPat = null;
            StringBuilder sbPat = new StringBuilder();
            if (patInfo != null)
            {
                if (patInfo.PriceType == "1")
                {
                    sbPat.Append(">PAT:A                                                                         ");
                }
                else if (patInfo.PriceType == "2")
                {
                    sbPat.Append(">PAT:A*CH                                                                       ");
                }
                else if (patInfo.PriceType == "3")
                {
                    sbPat.Append(">PAT:A*IN                                                                       ");
                }
                sbPat.AppendFormat("01 {0} FARE:CNY{1} TAX:CNY{2} YQ:CNY{3}  TOTAL:{4}                  ",
                    patInfo.SeatGroup,
                    patInfo.Fare.IndexOf(".0") != -1 ? patInfo.Fare : patInfo.Fare + ".00",
                    patInfo.TAX.IndexOf(".0") != -1 ? patInfo.TAX : patInfo.TAX + ".00",
                    patInfo.RQFare.IndexOf(".0") != -1 ? patInfo.RQFare : patInfo.RQFare + ".00",
                    patInfo.Price.IndexOf(".0") != -1 ? patInfo.Price : patInfo.Price + ".00"
                    );
                sbPat.Append("SFC:01");
            }
            strNewPat = sbPat.ToString();
            return strNewPat;
        }

        /// <summary>
        /// 替换除了\r|\n|\t外的不可见字符 
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public string RemoveHideChar(string strData)
        {
            //替换除了\r|\n|\t外的不可见字符
            strData = Regex.Replace(strData, @"[^A-Z0-9a-z|\x21-\x7E|\u4e00-\u9fa5|\r|\n|\t]", " ");
            //[^\x0D\x20-\xD7FF\xE000-\xFFFD\x10000-x10FFFF\u4e00-\u9fa5]
            return strData;
        }

        /// <summary>
        /// 获取票号状态
        /// </summary>
        /// <param name="TicketCon"></param>
        /// <returns></returns>
        public string GetTicketStatus(string TicketCon)
        {
            string Stat = "";
            if (string.IsNullOrEmpty(TicketCon))
            {
                return Stat;
            }
            //票号状态提取正则
            string pattern = @"(?:(?:\d{1,2}[A-Z]{1})?\s*(?<Stat>VOID|CHANGE|USED/FLOWN|USED/CLOSED|REFUNDED|OPEN FOR USE|CHECKED IN|CHECK IN|LIFT/BOARDED|PRINT/EXCH|EXCHANGED|PAPER TICKET|EXCH(?!\:|ANGED)|SUSPENDED)\s*)";
            Match match = Regex.Match(TicketCon, pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                //票号中的状态提取
                Stat = match.Groups["Stat"].Value.Trim().ToUpper();
            }
            return Stat;
        }
        /// <summary>
        /// 获取Detr信息 detr:Tn/票号,F
        /// </summary>
        /// <param name="recvData"></param>
        /// <returns></returns>
        public List<DetrInfo> GetDetrF(string recvData)
        {
            recvData = recvData.Replace("\r\n", "\r").Replace("\r", "\n");
            List<DetrInfo> detrList = new List<DetrInfo>();
            if (!string.IsNullOrEmpty(recvData))
            {
                //"DETR TN/9992118214188,F NAME: 潘丰 TKTN:9992118214188    RCPT:      1         XX6307095963   2         FFCA585932686/C    3         NI110227196705120031"
                string DetrPattern = @"^.*\n?NAME:\s*(?<Name>[A-Z\u4e00-\u9fa5/ ]+)(INF)?.*?\s+TKTN:\s*(?<Tktn>\d{13})\s+RCPT:\s+\d\s+(?<Number1>.*?)\s+(?<last>.+)";
                Match DetrMatch = Regex.Match(recvData, DetrPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (DetrMatch.Success)
                {
                    DetrInfo detrf = new DetrInfo();
                    detrf.PassengerName = DetrMatch.Groups["Name"].Value.Trim();
                    detrf.TicketNum = DetrMatch.Groups["Tktn"].Value.Trim();
                    string last = DetrMatch.Groups["last"].Value;
                    List<string> lstArr = new List<string>();
                    lstArr.Add(DetrMatch.Groups["Number1"].Value.Trim());

                    /*
                        string pattern = @"\s*\d\s+(?<Number2>(\w|\/)+)\s*";                      
                        MatchCollection chColl = Regex.Matches(last, pattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        foreach (Match item in chColl)
                        {
                            lstArr.Add(item.Groups["Number2"].Value.ToUpper().Trim());
                        }
                    */

                    DetrPattern = @"\d\s+";
                    string[] strArr = Regex.Split(last, DetrPattern);
                    foreach (string item in strArr)
                    {
                        if (item.Trim() != "")
                        {
                            lstArr.Add(item.Trim());
                        }
                    }

                    string[] strArray = lstArr.ToArray();
                    foreach (string str2 in strArray)
                    {
                        if (str2.Length > 2)
                        {
                            if (str2.StartsWith("RP") || str2.StartsWith("XX"))
                            {
                                if (str2.StartsWith("RP"))
                                {
                                    detrf.CreateSerialNumber = str2.Substring(2).Trim(new char[] { ' ', '\r', '\n' });
                                }
                                if (str2.StartsWith("XX"))
                                {
                                    detrf.VoidSerialNumber = str2.Substring(2).Trim(new char[] { ' ', '\r', '\n' });
                                }
                            }
                            else if ((str2.StartsWith("NI") || str2.StartsWith("PP")) || str2.StartsWith("ID"))
                            {
                                detrf.PasCardID = str2.Substring(2).Trim(new char[] { ' ', '\r', '\n' });
                            }
                            else if (str2.Trim().StartsWith("FF"))
                            {
                                detrf.PasCardID = str2.Substring(2).Trim(new char[] { ' ', '\r', '\n' }).Replace("/S", "");
                            }
                        }
                    }
                    detrList.Add(detrf);
                }
            }
            return detrList;
        }
        /// <summary>
        /// 获取票号的有关信息 英文
        /// </summary>
        /// <param name="recvData"></param>
        /// <returns></returns>
        public TicketInfo GetDetr(string recvData)
        {
            recvData = recvData.Replace("\r\n", "");
            TicketInfo ticketInfo = new TicketInfo();
            //提取票号
            string strTn = @"DETR\:TN\/(?<tk>\d{3,4}[-]?\d{10})";
            //乘客           
            string strPName = @"(?<=\s*ISSUED\s*BY\:\s*)(?<addr>[a-zA-Z\u4e00-\u9fa5\s|\/]*?)?\s*.{8}\s*(?<dblcity>[a-zA-Z]{3}\/[a-zA-Z]{3})?\s*(?<st>[\w|\-]*)\s*.{4}\s*(?<ER>.*)?\s*TOUR\s*CODE\:\s*(?<print>[a-zA-Z\s|\/|\-]*)?\s*PASSENGER\:\s*(?<pname>[a-zA-Z\s|\/|\u4e00-\u9fa5]*)\s*(?=EXCH)";
            //航段
            string strExchData = @"(?<=\s*EXCH\:\s*(\d{3,4}[-]?\d{10})\s*CONJ\s*TKT\:.*?\s*)(?<exchData>[\s|\S]+)\s*TO\:\s*(?<to>[A-Za-z]{3})\s*";
            string strSky = @"(\s*O\s*(?<dentity>FM|TO)\:(?<fnum>\d)(?<from>[A-Za-z]{3})\s*(?<carray>[a-zA-Z0-9]{2})\s*(?<flightno>\d{3,5})\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<date>\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<startTime>[^\d]{0,}\d{4})\s*OK\s*[A-Za-z]{1}\d?\s*((.*\/.*)|[a-zA-Z]{3}\d{1,2}|[a-zA-Z]{2,3})?\s*(?<packet>\d{2}K)\s*(?<status>VOID|CHANGE|USED/FLOWN|USED/CLOSED|REFUNDED|OPEN FOR USE|CHECKED IN|CHECK IN|LIFT/BOARDED|PRINT/EXCH|EXCHANGED|PAPER TICKET|EXCH(?!\:|ANGED)|SUSPENDED)\s*(?<eterm>[T|\d|\-]{4})?\s*(.{3})(?<bigpnr>[a-zA-Z0-9]{6})?\s*.((?<pnr>[a-zA-Z0-9]{6})\s*.{2})?)";
            //价格FC
            string strFC = @"\s*FC\:\s*((?<date>\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<from>[A-Za-z]{3})\s*(?<carray>[a-zA-Z0-9]{2})\s*(?<to>[A-Za-z]{3})\s*(?<fare>\d+(\.00)?)\s*CNY(?<fare1>\d+(\.00)?)END)?\s*";
            //税费
            string strTax = @"(\s*TAX\:\s*(CNY\s*(?<tax>\d+(\.00)?)\s*(?<type>CN|YQ))?\|.*?)";
            //总价格
            string strTotal = @"\s*TOTAL\:\s*(CNY\s*(?<total>\d+(\.00)?))?\|TKTN\:\s*(?<tk>\d{3,4}[-]?\d{10})\s*";


            //票号
            Match match = Regex.Match(recvData, strTn, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            string tk1 = string.Empty;
            string tk2 = string.Empty;
            if (match.Success)
            {
                tk1 = match.Groups["tk"].Value.Trim();
            }
            //总价格
            match = Regex.Match(recvData, strTotal, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string total = match.Groups["total"].Value.Trim();
                tk2 = match.Groups["tk"].Value.Trim();
                ticketInfo.PayTotalMoney = total;
            }
            ticketInfo.TicketNumber = string.IsNullOrEmpty(tk2) ? tk1 : tk2;
            //乘客
            match = Regex.Match(recvData, strPName, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string addr = match.Groups["addr"].Value.Trim();
                string dblcity = match.Groups["dblcity"].Value.Trim();
                string st = match.Groups["st"].Value.Trim();
                string ER = match.Groups["ER"].Value.Trim();
                string print = match.Groups["print"].Value.Trim();
                string pname = match.Groups["pname"].Value.Trim();

                ticketInfo.IssueAddress = addr;
                ticketInfo.TicketStatus = st;
                ticketInfo.Remark = ER;
                ticketInfo.PrintStatus = print;
                ticketInfo.PassengerName = pname;
                ticketInfo.ORG_DST = dblcity;
            }
            //航段
            match = Regex.Match(recvData, strExchData, RegexOptions.Compiled | RegexOptions.Multiline |
              RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string sky = match.Groups["exchData"].Value.Trim();
                string to = match.Groups["to"].Value.Trim();
                MatchCollection _matches = Regex.Matches(sky, strSky, RegexOptions.Compiled | RegexOptions.Multiline |
                 RegexOptions.IgnoreCase);
                foreach (Match item in _matches)
                {
                    if (item.Success)
                    {
                        string dentity = item.Groups["dentity"].Value.Trim();
                        string fnum = item.Groups["fnum"].Value.Trim();
                        string from = item.Groups["from"].Value.Trim();

                        string carray = item.Groups["carray"].Value.Trim();
                        string flightno = item.Groups["flightno"].Value.Trim();
                        string seat = item.Groups["seat"].Value.Trim();
                        string date = item.Groups["date"].Value.Trim();
                        string startTime = item.Groups["startTime"].Value.Trim();
                        string eterm = item.Groups["eterm"].Value.Trim();

                        string packet = item.Groups["packet"].Value.Trim();
                        string status = item.Groups["status"].Value.Trim();
                        string pnr = item.Groups["pnr"].Value.Trim();
                        string bigpnr = item.Groups["bigpnr"].Value.Trim();


                        LegInfo leg = new LegInfo();
                        leg.AirCode = carray;
                        leg.FlightNum = flightno;
                        leg.Seat = seat;
                        leg.FlyDate = date;
                        leg.FlyStartTime = startTime;
                        leg.T1T2 = eterm;
                        leg.Packet = packet;
                        leg.TicketStatus = status;
                        leg.SerialNum = fnum;
                        leg.FromCode = from;
                        leg.ToCode = to;
                        leg.FOrT = dentity;
                        ticketInfo.CRS_Pnr = pnr;
                        ticketInfo.ICS_Pnr = bigpnr;
                        ticketInfo.TLegInfo.Add(leg);
                    }
                }
            }
            //FC
            MatchCollection matches = Regex.Matches(recvData, strFC, RegexOptions.Compiled | RegexOptions.Multiline |
                RegexOptions.IgnoreCase);
            foreach (Match ch in matches)
            {
                if (ch.Success)
                {
                    string date = ch.Groups["date"].Value.Trim();
                    string from = ch.Groups["from"].Value.Trim();
                    string carray = ch.Groups["carray"].Value.Trim();
                    string fare = ch.Groups["fare"].Value.Trim();
                    string fare1 = ch.Groups["fare1"].Value.Trim();

                    ticketInfo.Fare = fare;
                }
            }
            //TAX
            matches = Regex.Matches(recvData, strTax, RegexOptions.Compiled | RegexOptions.Multiline |
               RegexOptions.IgnoreCase);
            foreach (Match ch in matches)
            {
                if (ch.Success)
                {
                    string tax = ch.Groups["tax"].Value.Trim();
                    string type = ch.Groups["type"].Value.Trim();
                    if (type == "CN")
                    {
                        ticketInfo.TAX = tax;
                    }
                    else if (type == "YQ")
                    {
                        ticketInfo.RQFare = tax;
                    }
                }
            }
            //if (string.IsNullOrEmpty(ticketInfo.AirCode) && ticketInfo.TLegInfo.Count > 0)
            //{
            //    LegInfo first = ticketInfo.TLegInfo[0];
            //    LegInfo last = ticketInfo.TLegInfo[ticketInfo.TLegInfo.Count - 1];
            //    ticketInfo.AirCode = first.AirCode + "/" + last.AirCode;
            //}
            return ticketInfo;
        }

        /// <summary>
        ///  获取票号的有关信息 中/英文
        /// </summary>
        /// <param name="recvData"></param>
        /// <returns></returns>
        public TicketInfo GetDetrS(string recvData)
        {
            TicketInfo ticketInfo = new TicketInfo();
            recvData = recvData.Replace("\r\n", "").Replace("*", "").Replace("{", "");
            string strDuan = @"\s*(?<date>\d{2}[A-Za-z]{3}(\d{2})?)(?<one>[\s|\S]+?)\s*-{10,}";
            Match match = Regex.Match(recvData, strDuan, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            //客票第一屏信息
            string KpInfo = "";
            if (match.Success)
            {
                KpInfo = match.Groups["one"].Value.Trim();
                ticketInfo.StrGetTciketDate = match.Groups["date"].Value.Trim();
                ticketInfo.GetTciketDate = GetYMD(ticketInfo.StrGetTciketDate, DataFormat.dayMonthYear);
            }
            string strTkPatern = @"电子客票票号\s*(?<tk>\d{3,4}[-]?\d{10})\s*后续客票号\s*(?<backtk>[A-Za-z0-9|\-]*)\s*出票航空公司\s*(?<cpair>[a-zA-Z\s0-9]*)\s*售票处信息\s*(?<salefi>[a-zA-Z0-9|\s|\-]*)\s*出票时间\/地点\s*(?<cpaddr>.*)\s*旅客姓名\s*(?<pname>[a-zA-Z\s|\u4e00-\u9fa5]*)\s*身份识别号码\s*(?<card>.*)\s*票价\s*货币\s*CNY\s*金额\s*(?<fare>\d+(\.00)?)\s*\+?\s*实付等值货币\s*CNY\s*金额\s*(?<payfare>\d+(\.00)?)\s*付款方式\s*(?<payair>\w{2})\s*\-?\s*税款\s*(CNY|PD)?\s*(?<tax>\d+(\.00)?)(CN)?\s*(CNY|PD)?\s*(?<rq>\d+(\.00)?)(YQ)?\s*付款总额\s*(CNY|PD)?\s*(?<paytotal>\d+(\.00)?)\s*使用限制\s*(?<remark>[a-zA-Z0-9\s|\/|\u4e00-\u9fa5]*)\s*签注信息\s*(?<endorsement>[a-zA-Z0-9|\/|\s|\u4e00-\u9fa5]*)";
            match = Regex.Match(KpInfo, strTkPatern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (match.Success)
            {
                string tk = match.Groups["tk"].Value.Trim();
                string backtk = match.Groups["backtk"].Value.Trim();
                string cpair = match.Groups["cpair"].Value.Trim();
                string salefi = match.Groups["salefi"].Value.Trim();
                string cpaddr = match.Groups["cpaddr"].Value.Trim();
                string pname = match.Groups["pname"].Value.Trim();
                string card = match.Groups["card"].Value.Trim();
                string fare = match.Groups["fare"].Value.Trim();
                string payfare = match.Groups["payfare"].Value.Trim();
                string payair = match.Groups["payair"].Value.Trim();
                string tax = match.Groups["tax"].Value.Trim();
                string rq = match.Groups["rq"].Value.Trim();
                string paytotal = match.Groups["paytotal"].Value.Trim();
                string remark = match.Groups["remark"].Value.Trim();
                string endorsement = match.Groups["endorsement"].Value.Trim();


                ticketInfo.TicketNumber = tk;
                ticketInfo.FollowTicketNumber = backtk;
                ticketInfo.IssueAirLine = cpair;
                string[] strArr = salefi.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                ticketInfo.SaleInfo = string.Join("|", strArr);
                //17JUL14/URUMQI(302)<08078221>
                ticketInfo.IssueDate = cpaddr.Split('/')[0];
                ticketInfo.IssueAddress = cpaddr;
                if (string.IsNullOrEmpty(ticketInfo.IssueOffice) && strArr.Length >= 1)
                {
                    ticketInfo.IssueOffice = strArr[1];
                }

                ticketInfo.PassengerName = pname;
                ticketInfo.SsrCardID = card;
                ticketInfo.Fare = fare;
                ticketInfo.TAX = tax;
                ticketInfo.RQFare = rq;
                ticketInfo.PayTotalMoney = payfare;
                ticketInfo.PayAir = payair;
                ticketInfo.Remark = remark;
                ticketInfo.UseLimitRemark = endorsement;

                if (string.IsNullOrEmpty(ticketInfo.AirCode) && !string.IsNullOrEmpty(payair))
                {
                    ticketInfo.AirCode = payair;
                }
            }
            //经停信息
            string skyStopPattern = @"经停\s*起飞城市机场\s*日期\s*星期\s*时间\s*航班\s*舱位\s*(?<one>[\s|\S]+?)-{10,}";
            MatchCollection _matches = Regex.Matches(recvData, skyStopPattern, RegexOptions.Compiled | RegexOptions.Multiline |
               RegexOptions.IgnoreCase);
            string sky0 = @"经停\s*起飞城市机场\s*日期\s*星期\s*时间\s*航班\s*舱位\s*[\-|\+]?\s*(?<sfdep>[A-Za-z]{3})\s*(?<sfairport>[a-zA-Z\-]*)\s*(?<sfdate>\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<sfweek>[a-zA-Z]{3})\s*(?<sftime>\d\s*\d\s*\d\s*\d\s*)\s*(?<sfcarray>[a-zA-Z0-9]{2})\s*(?<sfflightno>\d{3,4})\s*(?<sfseat>[A-Za-z]{1}\d?)\s*(?<sfsl>[a-zA-Z]{3,}\d{1,2}\s*)\s*(?<sfpacket>\d{2}K)\s*经停\s*到达城市机场\s*日期\s*星期\s*时间\s*机型\s*订座\s*(?<starr>[A-Za-z]{3})\s*(?<stairport>[a-zA-Z\-]*)\s*(?<stdate>\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<stweek>[a-zA-Z]{3})\s*(?<sttime>\d\s*\d\s*\d\s*\d\s*)\s*\(?(?<smode>\d{3,4})\)?\s*(?<sconfirm>.*?)\s*订座记录编号\s*((?<pnr>[a-zA-Z0-9]{6})\/\dE)?\s*客票状态\s*(?<tstatus>VOID|CHANGE|USED/FLOWN|USED/CLOSED|REFUNDED|OPEN FOR USE|CHECKED IN|CHECK IN|LIFT/BOARDED|PRINT/EXCH|EXCHANGED|PAPER TICKET|EXCH(?!\:|ANGED)|SUSPENDED)\s*-{10,}";

            string data = string.Empty;
            foreach (Match item in _matches)
            {
                if (item.Success)
                {
                    data = item.Value;
                    match = Regex.Match(data, sky0, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                    if (match.Success)
                    {
                        string sfdep = match.Groups["sfdep"].Value.Trim();
                        string sfairport = match.Groups["sfairport"].Value.Trim();
                        string sfdate = match.Groups["sfdate"].Value.Trim();
                        string sfweek = match.Groups["sfweek"].Value.Trim();
                        string sftime = match.Groups["sftime"].Value.Trim();
                        string sfcarray = match.Groups["sfcarray"].Value.Trim();
                        string sfflightno = match.Groups["sfflightno"].Value.Trim();
                        string sfseat = match.Groups["sfseat"].Value.Trim();
                        string sfsl = match.Groups["sfsl"].Value.Trim();
                        string sfpacket = match.Groups["sfpacket"].Value.Trim();
                        string starr = match.Groups["starr"].Value.Trim();
                        string stairport = match.Groups["stairport"].Value.Trim();
                        string stdate = match.Groups["stdate"].Value.Trim();
                        string stweek = match.Groups["stweek"].Value.Trim();
                        string sttime = match.Groups["sttime"].Value.Trim();
                        string smode = match.Groups["smode"].Value.Trim();
                        string sconfirm = match.Groups["sconfirm"].Value.Trim();
                        string pnr = match.Groups["pnr"].Value.Trim();
                        string tstatus = match.Groups["tstatus"].Value.Trim();

                        LegInfo legInfo = new LegInfo();
                        legInfo.DEP = sfdep;
                        legInfo.DEPAirPort = sfairport;
                        legInfo.FlyDate = sfdate;
                        legInfo.StartWeek = sfweek;
                        if (sfweek.Length >= 2)
                        {
                            legInfo.FlyDate1 = GetYMD(sfweek.Substring(0, 2) + sfdate, DataFormat.weekDayMonth);
                        }
                        legInfo.FlyStartTime = sftime;
                        legInfo.AirCode = sfcarray;
                        legInfo.FlightNum = sfflightno;
                        legInfo.AirCodeFlightNum = (sfcarray + sfflightno).ToUpper();
                        legInfo.Seat = sfseat;
                        legInfo.VMMB_VEEB = sfsl;
                        legInfo.Packet = sfpacket;
                        legInfo.ARR = starr;
                        legInfo.ARRAirPort = stairport;
                        legInfo.FlyToDate = stdate;
                        legInfo.EndWeek = stweek;
                        if (stweek.Length >= 2)
                        {
                            legInfo.FlyDateE = GetYMD(stweek.Substring(0, 2) + stdate, DataFormat.weekDayMonth);
                        }
                        legInfo.FlyEndTime = sttime;
                        legInfo.Model = smode;
                        legInfo.ConfirmStatus = sconfirm;
                        legInfo.Pnr = pnr;
                        ticketInfo.CRS_Pnr = pnr;
                        legInfo.TicketStatus = tstatus;
                        if (string.IsNullOrEmpty(ticketInfo.TicketStatus) && !string.IsNullOrEmpty(tstatus))
                        {
                            ticketInfo.TicketStatus = tstatus;
                        }
                        ticketInfo.TLegInfo.Add(legInfo);
                    }
                    else
                    {
                        #region 简单处理 特殊
                        string tempData = Regex.Replace(data, @"-{10,}", "");//-{10,}
                        tempData = Regex.Replace(tempData, @"经停\s*(起飞|到达)城市机场\s*日期\s*星期\s*时间\s*", "");
                        tempData = Regex.Replace(tempData, @"航班\s*舱位", "");
                        string[] strArr = Regex.Split(tempData, @"机型\s*订座");
                        if (strArr.Length >= 2)
                        {
                            string sfdep = "";
                            string sfairport = "";
                            string sfdate = "";
                            string sfweek = "";
                            string sftime = "";
                            string sfcarray = "";
                            string sfflightno = "";
                            string sfseat = "";
                            string sfsl = "";
                            string sfpacket = "";
                            string starr = "";
                            string stairport = "";

                            string pnr = "";
                            string tstatus = "";
                            string[] strDada = strArr[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                            string[] strskyArr = new string[7];
                            for (int i = 0; i < 7; i++)
                            {
                                if (i < strDada.Length)
                                {
                                    if (i == 0) sfdep = strDada[i];
                                    if (i == 1) sfairport = strDada[i];
                                    if (i == 2) sfdate = strDada[i];
                                    if (i == 3) sfweek = strDada[i];
                                    if (i == 4) sftime = strDada[i];
                                    if (i == 5)
                                    {
                                        if (!Regex.IsMatch(strDada[i], @"(\d+K)"))
                                        {
                                            sfdate = strDada[i];
                                            sfcarray = strDada[i].Length >= 2 ? strDada[i].Substring(0, 2) : "";
                                            sfflightno = strDada[i].Length >= 2 ? strDada[i].Substring(2) : "";
                                        }
                                        else
                                        {
                                            sfpacket = strDada[i];
                                        }
                                    };
                                    if (i == 6) sfpacket = strDada[i];
                                }
                            }
                            string[] strTempData = Regex.Split(strArr[1], @"订座记录编号");
                            if (strTempData.Length >= 2)
                            {
                                strDada = strTempData[0].Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);
                                for (int i = 0; i < 7; i++)
                                {
                                    if (i < strDada.Length)
                                    {
                                        if (i == 0) starr = strDada[i];
                                        if (i == 1) stairport = strDada[i];
                                    }
                                }
                                strTempData = Regex.Split(strTempData[1], @"客票状态");
                                if (strTempData.Length >= 2)
                                {
                                    pnr = strTempData[0].Trim();
                                    tstatus = strTempData[1].Trim();
                                }
                            }
                            LegInfo legInfo = new LegInfo();
                            ticketInfo.TLegInfo.Add(legInfo);
                            legInfo.DEP = sfdep;
                            legInfo.DEPAirPort = sfairport;
                            legInfo.FlyDate = sfdate;
                            legInfo.StartWeek = sfweek;
                            legInfo.FlyStartTime = sftime;
                            legInfo.AirCode = sfcarray;
                            legInfo.FlightNum = sfflightno;
                            legInfo.AirCodeFlightNum = (sfcarray + sfflightno).ToUpper();
                            legInfo.Seat = sfseat;
                            legInfo.VMMB_VEEB = sfsl;
                            legInfo.Packet = sfpacket;
                            legInfo.ARR = starr;
                            legInfo.ARRAirPort = stairport;

                            legInfo.Pnr = pnr;
                            ticketInfo.CRS_Pnr = pnr;
                            legInfo.TicketStatus = tstatus;
                        }
                        #endregion
                    }
                }
            }
            //航段信息
            string strSkyPattern = @"[\+|\-]?\s*\.?\s*(?<pname>[a-zA-Z\s|\u4e00-\u9fa5]*)\s*(?<from>[A-Za-z]{3})\s*(?<to>[A-Za-z]{3})\s*(?<carray>[a-zA-Z0-9]{2})\s*(?<flightno>\d{3,5})\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<date>\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<tk>\d{3,4}[-]?\d{10})\s*CNY\s*(?<price>\d+(\.00)?)\s*[\+|\-]?";
            _matches = Regex.Matches(recvData, strSkyPattern, RegexOptions.Compiled | RegexOptions.Multiline |
               RegexOptions.IgnoreCase);
            foreach (Match item in _matches)
            {
                if (item.Success)
                {
                    string pname = item.Groups["pname"].Value.Trim();
                    string from = item.Groups["from"].Value.Trim();
                    string to = item.Groups["to"].Value.Trim();
                    string carray = item.Groups["carray"].Value.Trim();
                    string flightno = item.Groups["flightno"].Value.Trim();
                    string seat = item.Groups["seat"].Value.Trim();
                    string date = item.Groups["date"].Value.Trim();
                    string tk = item.Groups["tk"].Value.Trim();
                    string price = item.Groups["price"].Value.Trim();
                    LegInfo legInfo = ticketInfo.TLegInfo.Find(p => (p.AirCode + p.FlightNum).ToUpper() == (carray + flightno).ToUpper());
                    if (legInfo == null)
                    {
                        legInfo = new LegInfo();
                        ticketInfo.TLegInfo.Add(legInfo);
                    }
                    legInfo.FromCode = from;
                    legInfo.ToCode = to;
                    legInfo.CityDouble = (from + to).ToUpper();
                    legInfo.Seat = seat;
                    legInfo.FlyDate = date;
                    legInfo.TicketNumber = tk;
                    if (string.IsNullOrEmpty(ticketInfo.PassengerName) && !string.IsNullOrEmpty(pname))
                    {
                        ticketInfo.PassengerName = pname;
                    }
                }
            }
            if (string.IsNullOrEmpty(ticketInfo.ORG_DST) && ticketInfo.TLegInfo.Count > 0)
            {
                LegInfo first = ticketInfo.TLegInfo[0];
                LegInfo last = ticketInfo.TLegInfo[ticketInfo.TLegInfo.Count - 1];
                ticketInfo.ORG_DST = first.FromCode + "/" + last.ToCode;
            }
            return ticketInfo;
        }


        /// <summary>
        /// 格式化经停字符串信息
        /// </summary>
        /// <param name="strStopCon"></param>
        /// <returns></returns>
        public LegStop GetStop(string strStopCon, out  string errMsg)
        {
            errMsg = "";
            LegStop Leg = new LegStop();
            PnrResource pnrResource = new PnrResource();
            try
            {
                if (strStopCon == null)
                {
                    strStopCon = "";
                }
                strStopCon = RemoveHideChar(strStopCon);
                string strPattern = @"\s*FF\s*:?\s*(?<CarryCode>[A-Za-z0-9]+)\s*\/\s*(?<date>\d{2}[A-Za-z]{3}\d{2})\s*(?<fromcode>[A-Za-z]{3})\s*(?<fromtime>\d{4})\s*(?<Mode>\w{3,5})\s*(?<middle>(?<MCode>[A-Za-z]{3})\s*(?<MTime1>\d{4})\s*(?<MTime2>\d{4}))?\s*(?<tocode>[A-Za-z]{3})\s*(?<totime>\d{4})\s*";
                Match mchLeg = Regex.Match(strStopCon, strPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (mchLeg.Success)
                {
                    string CarryCode = mchLeg.Groups["CarryCode"].Value.ToUpper().Trim().Replace(" ", "");
                    string date = mchLeg.Groups["date"].Value.ToUpper().Trim().Replace(" ", "");
                    string fromcode = mchLeg.Groups["fromcode"].Value.Trim().Replace(" ", "");
                    string fromtime = mchLeg.Groups["fromtime"].Value.Trim().Replace(" ", "");
                    string Mode = mchLeg.Groups["Mode"].Value.Trim().Replace(" ", "");
                    string middle = mchLeg.Groups["middle"].Value;
                    string MCode = mchLeg.Groups["MCode"].Value.ToUpper().Trim().Replace(" ", "");
                    string MTime1 = mchLeg.Groups["MTime1"].Value.Trim().Replace(" ", "");
                    string MTime2 = mchLeg.Groups["MTime2"].Value.Trim().Replace(" ", "");
                    string tocode = mchLeg.Groups["tocode"].Value.Trim().Replace(" ", "");
                    string totime = mchLeg.Groups["totime"].Value.Trim().Replace(" ", "");
                    Leg.CarrayCodeFlightNum = CarryCode;
                    if (CarryCode.Length > 2)
                    {
                        Leg.CarrayCode = CarryCode.Substring(0, 2);
                        CarryInfo carryInfo = pnrResource.CarrayDictionary.CarrayList.Find(p => p.AirCode == Leg.CarrayCode);
                        Leg.CarrayName = carryInfo != null ? carryInfo.Carry.AirName : "";
                        Leg.AirShortName = carryInfo != null ? carryInfo.Carry.AirShortName : "";
                        Leg.FlightNum = CarryCode.Substring(2);
                    }
                    Leg.StrStopDate = date;
                    Leg.StopDate = GetYMD(date, DataFormat.dayMonthYear);
                    Leg.FromCityCode = fromcode;
                    CityInfo fromCity = pnrResource.CityDictionary.CityList.Find(p => p.key == Leg.FromCityCode);
                    Leg.FromCity = fromCity != null ? fromCity.city.Name : "";

                    if (fromtime.Length > 2)
                    {
                        Leg.FromTime = fromtime.Insert(2, ":");
                    }
                    Leg.Model = Mode;
                    Leg.MiddleCityCode = MCode;
                    CityInfo middleCity = pnrResource.CityDictionary.CityList.Find(p => p.key == MCode);
                    Leg.MiddleCity = middleCity != null ? middleCity.city.Name : "";

                    if (MTime1.Length > 2)
                    {
                        Leg.MiddleTime1 = MTime1.Insert(2, ":");
                    }
                    if (MTime2.Length > 2)
                    {
                        Leg.MiddleTime2 = MTime2.Insert(2, ":");
                    }
                    Leg.ToCityCode = tocode;
                    CityInfo toCity = pnrResource.CityDictionary.CityList.Find(p => p.key == tocode);
                    Leg.ToCity = toCity != null ? toCity.city.Name : "";
                    if (totime.Length > 2)
                    {
                        Leg.ToTime = totime.Insert(2, ":");
                    }
                }
            }
            catch (Exception ex)
            {
                errMsg = ex.Message;
            }
            return Leg;
        }


        /// <summary>
        /// 检测乘机人名字是否正确 true 不正确 false正确
        /// </summary>
        /// <returns></returns>
        public bool PassengerNameIsCorrent(string PassengerName)
        {
            if (string.IsNullOrEmpty(PassengerName)) PassengerName = "";
            PassengerName = PassengerName.Replace(" ", "");
            string Pattern = @"([^\u4e00-\u9fa5]+)(?=[\u4e00-\u9fa5]+)";
            bool isCorrent = Regex.IsMatch(PassengerName, Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            return isCorrent;
        }

        //  MU2283  W FR16NOV  XIYDSN RR1   1450 1610  HFDTKE SPLIT FROM JF7RDN   *** 预订酒店指令HC, 详情  HC:HELP   ***   
        //PNR ALREADY ON THIS SYSTEM. RLOC IS JGE18C  
        /// <summary>
        /// 获取大编码转换小编码内容取小编码 如：PNR ALREADY ON THIS SYSTEM. RLOC IS JGE18C  
        /// </summary>
        /// <param name="BigCon"></param>
        /// <returns></returns>
        public string GetRRTPnr(string BigCon)
        {
            string Pnr = "";
            if (BigCon.ToUpper().Contains("RLOC IS"))
            {
                var patern = @"(?<=RLOC\s*IS)(?<pnr>\s*[A-Za-z0-9]{6}\s*)";
                Match mch = Regex.Match(BigCon, patern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                if (mch.Success)
                {
                    Pnr = mch.Groups["pnr"].Value.Trim();
                }
            }
            return Pnr;
        }
        /// <summary>
        /// 分离编码返回内容获取分离的新编码
        /// </summary>
        /// <param name="PNR"></param>
        /// <param name="SplitPnrRecvData"></param>
        /// <param name="ErrMsg"></param>
        /// <returns></returns>
        public string GetSplitPnr(string PNR, string SplitPnrRecvData, out string ErrMsg)
        {
            string NewPnr = string.Empty;
            ErrMsg = "";
            string SplitPattern = @"\s*(?<NewPnr>[^\w]*\w{5,6})\s*SPLIT FROM\s*" + PNR + @"\s*";
            Match mch = Regex.Match(SplitPnrRecvData, SplitPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (mch.Success)
            {
                NewPnr = mch.Groups["NewPnr"].Value.Trim();//分离 后的 新编码
            }
            if (NewPnr == "")
            {
                ErrMsg = "分离编码内容，未能取到新编码！";
            }
            return NewPnr;
        }


        /// <summary>
        /// 将Pnr内容分割成每一项不处理格式
        /// </summary>
        /// <param name="pnrCon"></param>
        /// <returns></returns>
        public List<string> NewSplitPnr(string pnrCon)
        {
            List<string> pnrList = new List<string>();
            //分割PNR项正则
            string midPattern = @"(?<Middle>(\b|\s*)(?<num>[X]?(?<!\:|\d)(\d{1,2}|1\d{1,2}))\s*\.\s*(?!(00|\/|\d%|\d0|NET)\w+))";
            string midPattern1 = @"(?<Middle1>(\b|\s*)(?<num>[X]?(?<!\:|\d)(\d{1,2}|1\d{1,2}))\s*\.\s*(?!(00|\/|\d%|\d0|NET)\w+))";
            string RegPattern = @"(?<first>[\s|\S]*?(?=" + midPattern + "))" + midPattern + @"(?<last>[\s|\S]*?(?=" + midPattern1 + @"))(?<tempPnrCon>[\s|\S]+)";
            string first = "", last = "", Middle = "", tempPnrCon = pnrCon;
            Match mch = Regex.Match(tempPnrCon, RegPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
            bool IsFirst = false;
            while (true)
            {
                if (mch.Success)
                {
                    first = mch.Groups["first"].Value;
                    Middle = mch.Groups["Middle"].Value;
                    last = mch.Groups["last"].Value;
                    tempPnrCon = mch.Groups["tempPnrCon"].Value;
                    //处理
                    if (Middle.Replace("\r", "").Replace("\n", "").Replace(" ", "").IndexOf("1.") != -1 && !IsFirst)
                    {
                        if (first.Replace("\r", "").Replace("\n", "").Replace(" ", "") != "")
                        {
                            pnrList.Add(first);
                            pnrList.Add(Middle + last);
                        }
                        else
                        {
                            pnrList.Add(first + Middle + last);
                        }
                        IsFirst = true;
                    }
                    else
                    {
                        pnrList.Add(first + Middle + last);
                    }
                    mch = Regex.Match(tempPnrCon, RegPattern, RegexOptions.Compiled | RegexOptions.IgnoreCase);
                }
                else
                {
                    pnrList.Add(tempPnrCon);
                    break;
                }
            }
            return pnrList;
        }
        /// <summary>
        /// 去掉重复序号项
        /// </summary>
        /// <returns></returns>
        public List<string> RepeatLine(List<string> PnrItemList)
        {
            string PatNum = @"\s*(?<left>(\b|\s*)(?<num>[X]?(?<!\:)\d{1,3})\s*\.\s*(?!(00|\/)\w+))\s*";
            List<string> tempList = new List<string>();
            List<string> tempNum = new List<string>();
            string Num = "";
            foreach (string item in PnrItemList)
            {
                Num = "";
                Match mch = Regex.Match(item, PatNum, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (mch.Success)
                {
                    Num = mch.Groups["left"].Value.Trim();
                    if (!tempNum.Contains(Num))
                    {
                        tempList.Add(item);
                        tempNum.Add(Num);
                    }
                }
            }
            if (tempList.Count == 0)
            {
                tempList = PnrItemList;
            }
            return tempList;
        }


        /// <summary>
        /// 获取BSP自动出票后返回数据 解析出票号 但不知票号所对应的乘客姓名
        /// </summary>
        /// <param name="pnr"></param>
        /// <param name="strEtdzCon"></param>
        /// <returns></returns>
        public List<string> GetEtdzTicketNumber(string pnr, string strEtdzCon)
        {
            /*
             * 
             CNY1050.00      HRBX6P 
             088-3952353849 
             * */
            List<string> lstTicketNumber = new List<string>();
            if (!string.IsNullOrEmpty(pnr) && !string.IsNullOrEmpty(strEtdzCon))
            {
                if (strEtdzCon.ToUpper().Contains(pnr.ToUpper()))
                {
                    string Pattern = @"\s*(?<=CNY)(?<money>\d{1,7}(\.\d+)?)\s*[A-Za-z0-9]{6}\s*(?<ticketnum>\d{3,4}(\-?|\s+)\d{10}\s*){1,}\s*";
                    Match mch = Regex.Match(strEtdzCon, Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    if (mch.Success)
                    {
                        CaptureCollection cc = mch.Groups["ticketnum"].Captures;
                        foreach (Capture item in cc)
                        {
                            lstTicketNumber.Add(item.Value.Replace("\r", "").Replace("\n", "").Trim());
                        }
                    }
                }
            }
            return lstTicketNumber;
        }

        /// <summary>
        /// 解析编码内容中需要Xe的SFC价格项序号
        /// </summary>
        /// <returns></returns>
        public List<string> GetXeSFCNumber(string pnrContent)
        {
            List<string> xeSFCList = new List<string>();
            string[] rtArr = pnrContent.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
            //sfc价格时所要叉掉的项列表
            string PPatern = @"\s*(?<Xe>\d+)\s*\.\s*(?=(FN\/|FC\/|FP\/|EI\/|RMK AUTOMATIC FARE))";
            foreach (var item in rtArr)
            {
                Match mch2 = Regex.Match(item, PPatern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (mch2.Success)
                {
                    xeSFCList.Add(mch2.Groups["Xe"].Value.Trim());//"XE" +
                }
            }
            return xeSFCList;
        }
        /// <summary>
        /// 获取BSP出票时限序号
        /// </summary>
        /// <param name="pnrContent"></param>
        /// <returns></returns>
        public List<string> GetTKTLNumber(string pnrContent)
        {
            //提取XE出票时限序号正则5.TL/2100/23SEP/CTU120
            string XePattern = @"\s*(?<Xe>\d+)\s*\.\s*(TL|TKTL).*?(?=\w{3}\s*\d{3})\s*";
            List<string> xeSFCList = new List<string>();
            string[] rtArr = pnrContent.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var item in rtArr)
            {
                Match mch2 = Regex.Match(item, XePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                if (mch2.Success)
                {
                    xeSFCList.Add(mch2.Groups["Xe"].Value.Trim());//"XE" +
                }
            }
            return xeSFCList;
        }


        /*
         * NAME LIST   
            HO1336/20JUL
             001   1ZHANGSUFANG      HWW154 V RR2  KWE140 17JUL 
             002   0ZHANGSUFANG      HWW18J V HX2  KWE140 17JUL 
            END                  
         */
        //根据旅客名单提取PNR >RT旅客姓名（拼音）/航班号/日期
        /// <summary>
        /// 解析出编码
        /// </summary>
        /// <param name="strData"></param>
        /// <returns></returns>
        public List<LEASEWAIT_Info> GetPnrList(string strData)
        {
            List<LEASEWAIT_Info> lstPnr = new List<LEASEWAIT_Info>();
            string Item = @"(?<num>\d{1,3})\s*(?<pnum>\d+)?(?<pasName>.*?)\s*(?<pnr>[A-Za-z0-9]{6})\s*(?<seat>[A-Za-z](\d)?)\s*(?<status>[^\w]{0,}[A-Za-z]{2}(?<pasCount>\d{0,3})?)\s*(?<office>[a-zA-Z]{3}\d{3})\s*(?<date>\d{2}[a-zA-Z]{3}\d{0,2})\s*";
            string Pattern = @"(?<flightNum>[a-zA-z0-9]{2}\w{3,4})\/(\d{2}[a-zA-Z]{3}\d{0,2})\s*(?<Item>(" + Item + @")+?)END\s*";
            MatchCollection mchs = Regex.Matches(strData, Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string pnr = "", pasName = "", seat = "", date = "", FlightNum = "", num = "", status = "", office = "", pasCount = "0", pnum = "";
            string strItem = "";
            List<string> pnrList = new List<string>();
            for (int i = 0; i < mchs.Count; i++)
            {
                if (mchs[i].Success)
                {
                    FlightNum = mchs[i].Groups["flightNum"].Value.Trim().ToUpper();
                    strItem = mchs[i].Groups["Item"].Value.Trim().ToUpper();
                    MatchCollection mchs1 = Regex.Matches(strItem, Item, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    if (mchs1.Count > 0)
                    {
                        foreach (Match ch in mchs1)
                        {
                            num = ch.Groups["num"].Value.Trim();
                            date = ch.Groups["date"].Value.Trim();
                            pnr = ch.Groups["pnr"].Value.Trim().ToUpper();
                            seat = ch.Groups["seat"].Value.Trim().ToUpper();
                            status = ch.Groups["status"].Value.Trim().ToUpper();
                            office = ch.Groups["office"].Value.Trim().ToUpper();
                            pasCount = ch.Groups["pasCount"].Value.Trim();
                            pnum = ch.Groups["pnum"].Value.Trim();
                            pasName = ch.Groups["pasName"].Value.Trim().ToUpper();
                            if (pnr != "" && pnr.Length == 6)
                            {
                                if (!pnrList.Contains(pnr.ToUpper()))
                                {
                                    pnrList.Add(pnr.ToUpper());
                                    LEASEWAIT_Info model = new LEASEWAIT_Info();
                                    model.SerialNum = num;
                                    model.dayMonth = date;
                                    model.FlightNum = FlightNum;
                                    model.Office = office;
                                    model.PasCount = pasCount;
                                    model.PasPinYing = pasName;
                                    model.PasForwardNum = pnum;
                                    model.PNR = pnr;
                                    model.PnrStatus = status;
                                    model.Seat = seat;
                                    lstPnr.Add(model);
                                }
                            }
                        }
                    }
                }
            }
            pnrList.Clear();
            lstPnr.Sort(delegate(LEASEWAIT_Info w1, LEASEWAIT_Info w2)
            {
                int SerialNum1 = 0;
                int SerialNum2 = 0;
                int.TryParse(w1.SerialNum, out SerialNum1);
                int.TryParse(w2.SerialNum, out SerialNum2);
                return SerialNum1 > SerialNum2 ? 1 : 0;
            });
            return lstPnr;
        }

        //发送:AB/26JUN/PEKHGH
        //返回: 可能有多屏
        //AGENT BOOKING LIST  
        //26JUN   PEKHGH  
        // 001   1LVQING           HV078K R HK1  URC302 FM 9158 03JUN 
        //TOTAL NUMBER    1  
        public List<LEASEWAIT_Info> ABGetPnrList(string strData)
        {
            strData = Regex.Replace(strData, @"TOTAL\s*NUMBER\s*\d+", "");
            strData = strData.Replace("+", "").Replace("-", "");
            List<LEASEWAIT_Info> lstPnr = new List<LEASEWAIT_Info>();
            string Item = @"(?<num>\d{1,3})\s*(?<pnum>\d+)?(?<pasName>.*?)\s*(?<pnr>[A-Za-z0-9]{6})\s*(?<seat>[A-Za-z](\d)?)\s*(?<status>[^\w]{0,}[A-Za-z]{2}(?<pasCount>\d{0,3})?)\s*(?<office>[a-zA-Z]{3}\d{3})\s*((?<carrayCode>[a-zA-Z0-9]{2})\s*(?<fnum>\d{3,4}))?\s*(?<date>\d{2}[a-zA-Z]{3}\d{0,2})\s*";
            string Pattern = @"AGENT\s*BOOKING\s*LIST\s*(?<gdate>\d{2}[a-zA-Z]{3}\d{0,2})\s*(?<fcity>[a-zA-Z]{3})(?<tcity>[a-zA-Z]{3})\s*(?<Item>(" + Item + @")+)\s*";
            MatchCollection mchs = Regex.Matches(strData, Pattern, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
            string pnr = "", pasName = "", seat = "", date = "", num = "", status = "", office = "", pasCount = "0", pnum = "";
            string strItem = "", GobalDate = "", fCity = "", tCity = "", carrayCode = "", fnumber = "";
            List<string> pnrList = new List<string>();
            for (int i = 0; i < mchs.Count; i++)
            {
                if (mchs[i].Success)
                {
                    strItem = mchs[i].Groups["Item"].Value.Trim().ToUpper();
                    GobalDate = mchs[i].Groups["gdate"].Value.Trim().ToUpper();
                    fCity = mchs[i].Groups["fcity"].Value.Trim().ToUpper();
                    tCity = mchs[i].Groups["tcity"].Value.Trim().ToUpper();

                    MatchCollection mchs1 = Regex.Matches(strItem, Item, RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.Multiline);
                    if (mchs1.Count > 0)
                    {
                        foreach (Match ch in mchs1)
                        {
                            num = ch.Groups["num"].Value.Trim();
                            date = ch.Groups["date"].Value.Trim();
                            pnr = ch.Groups["pnr"].Value.Trim().ToUpper();
                            seat = ch.Groups["seat"].Value.Trim().ToUpper();
                            status = ch.Groups["status"].Value.Trim().ToUpper();
                            office = ch.Groups["office"].Value.Trim().ToUpper();
                            pasCount = ch.Groups["pasCount"].Value.Trim();
                            pnum = ch.Groups["pnum"].Value.Trim();
                            pasName = ch.Groups["pasName"].Value.Trim().ToUpper();
                            carrayCode = ch.Groups["carrayCode"].Value.Trim().ToUpper();
                            fnumber = ch.Groups["fnum"].Value.Trim().ToUpper();

                            if (pnr != "" && pnr.Length == 6)
                            {
                                if (!pnrList.Contains(pnr.ToUpper()))
                                {
                                    pnrList.Add(pnr.ToUpper());
                                    LEASEWAIT_Info model = new LEASEWAIT_Info();
                                    model.SerialNum = num;
                                    model.dayMonth = date;
                                    model.FlightNum = carrayCode + fnumber;
                                    model.FlightNo = fnumber;
                                    model.CarryCode = carrayCode;
                                    model.Office = office;
                                    model.PasCount = pasCount;
                                    model.PasPinYing = pasName;
                                    model.PasForwardNum = pnum;
                                    model.PNR = pnr;
                                    model.PnrStatus = status;
                                    model.Seat = seat;
                                    model.GobalDate = GobalDate;
                                    model.FromCode = fCity;
                                    model.ToCode = tCity;
                                    lstPnr.Add(model);
                                }
                            }
                        }
                    }
                }
            }
            pnrList.Clear();
            //从大到小排序
            lstPnr.Sort(delegate(LEASEWAIT_Info w1, LEASEWAIT_Info w2)
            {
                int SerialNum1 = 0;
                int SerialNum2 = 0;
                int.TryParse(w1.SerialNum, out SerialNum1);
                int.TryParse(w2.SerialNum, out SerialNum2);
                return SerialNum1 > SerialNum2 ? 1 : 0;
            });
            return lstPnr;
        }


        /// <summary>
        /// 将字符串分割成指定长度 每段末尾插入指定字符串 可用于格式化内容 每行80个字符那个方法
        /// </summary>
        /// <param name="strData">分割源字符串（含有隐藏字符）</param>
        /// <param name="Len">每段长度</param>
        /// <param name="InsertStr">每段中末尾插入字符串</param>
        /// <returns></returns>
        public string SplitInsertChar(string strData, int Len, string InsertStr)
        {
            MatchCollection mchs = Regex.Matches(strData, @"(.|\n){" + Len + "}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            List<string> lstLine = new List<string>();
            for (int i = 0; i < mchs.Count; i++)
            {
                Match item = mchs[i];
                if (item.Success)
                {
                    lstLine.Add(item.Value);
                }
            }
            if (lstLine.Count > 0)
            {
                string strtemp = string.Join("", lstLine.ToArray());
                if (strData.Length > strtemp.Length)
                {
                    //添加最后一段
                    lstLine.Add(strData.Substring(strtemp.Length));
                }
                strData = string.Join(InsertStr, lstLine.ToArray());
                lstLine.Clear();
            }
            return strData;
        }
        /// <summary>
        /// 获取预订编码返回内容中的解析信息
        /// </summary>
        /// <param name="yudingCon"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public YDInfo GetYuDingInfo(string yudingCon, out  string msg)
        {
            msg = "";
            YDInfo yd = null;
            PnrAnalysis.FormatPNR format = new FormatPNR();
            string Pnr = format.GetPNR(yudingCon, out msg);
            // CA1482  H SU18AUG  YCUTSN DK1   1715 1835
            // CA 406  L SU18AUG  PVGCTU DK1   1825 2135                         
            string pGroup = @"(?<carry>\*?[A-Za-z0-9]{2}\s*\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d\s*\d\s*\d\s*\d)\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*";
            string ydPattern = @"\s*(?<leg>" + pGroup + @")\s*";
            MatchCollection matches = Regex.Matches(yudingCon, ydPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (matches.Count > 0 && !string.IsNullOrEmpty(Pnr))
            {
                yd = new YDInfo();
                yd.Pnr = Pnr;
                string strCarry = string.Empty;
                string strCity = string.Empty;
                List<string> lstStatus = new List<string>();
                foreach (Match ch in matches)
                {
                    if (ch.Success)
                    {
                        LegInfo leg = new LegInfo();
                        strCarry = ch.Groups["carry"].Value.Trim().ToUpper().Replace(" ", "");
                        leg.AirCode = strCarry;
                        if (strCarry.Length > 2)
                        {
                            leg.AirCode = strCarry.Substring(0, 2);
                            leg.FlightNum = strCarry.Substring(2);
                        }
                        leg.Seat = ch.Groups["seat"].Value.Trim().ToUpper();
                        leg.FlyDate = ch.Groups["flyDate"].Value.Trim().ToUpper().Replace(" ", "").Replace("", "");
                        leg.FlyDate1 = FormatPNR.GetYMD(leg.FlyDate, DataFormat.weekDayMonth);//已日期格式显示 2012-06-19
                        leg.PnrStatus = ch.Groups["state"].Value.Trim().ToUpper();
                        lstStatus.Add(leg.PnrStatus);
                        strCity = ch.Groups["city"].Value.ToUpper().Replace(" ", "").Trim();
                        leg.CityDouble = strCity;
                        if (strCity.Length == 6)
                        {
                            leg.FromCode = strCity.Substring(0, 3);
                            leg.ToCode = strCity.Substring(3, 3);
                        }
                        leg.FlyStartTime = ch.Groups["startTime"].Value.Trim().ToUpper();
                        leg.FlyEndTime = ch.Groups["endTime"].Value.Trim().ToUpper();

                        DateTime dt1 = System.DateTime.Now;
                        DateTime.TryParse(leg.FlyDate1, out dt1);
                        //到达日期
                        if (leg.FlyEndTime.IndexOf("+1") != -1)
                        {
                            leg.FlyEndTime = leg.FlyEndTime.Replace("+1", "").Replace("", "");
                            leg.IsAddOneDayEndTime = "1";
                            leg.FlyDateE = dt1.AddDays(1).ToString("yyyy-MM-dd");
                        }
                        else
                        {
                            leg.FlyDateE = dt1.ToString("yyyy-MM-dd");
                        }
                        //添加到集合
                        yd.YDLegInfo.Add(leg);
                    }
                    //状态集合
                    yd.YDPnrStatus = string.Join("|", lstStatus.ToArray());
                }
            }
            return yd;
        }


        /// <summary>
        /// 从混乱的字符串内容中获取各自RT PAT内容  RT内容一定要在PAT内容之上 
        /// </summary>
        /// <param name="Con"></param>
        /// <returns></returns>
        public SplitPnrCon GetSplitPnrCon(string Con)
        {
            SplitPnrCon pc = new SplitPnrCon();
            #region 内容处理
            //处理隐藏字符
            Con = RemoveHideChar(Con);
            Con = Con.ToUpper();
            // 服务器耗时 131毫秒/0毫秒 网络传输总耗时 218毫秒
            string pattern = @"服务器耗时\s*\d+毫秒\/\d+毫秒\s*网络传输总耗时\s*\d+毫秒";
            Con = Regex.Replace(Con, pattern, "");
            #endregion
            pc.ALLCon = Con;
            pc.AdultPATCon = GetPATCon(Con, "1");
            pc.ChdPATCon = GetPATCon(Con, "2");
            pc.INFPATCon = GetPATCon(Con, "3");
            if (Con.IndexOf("PAT:A") != -1)
            {
                Con = Con.Substring(0, Con.IndexOf("PAT:A"));
            }
            pc.RTCon = Con.Replace("PAT:A", "").Replace(">", "").Replace("PAT A", "").Trim(new char[] { '-', ' ' });
            //过滤RT数据
            if (pc.AdultPATCon.IndexOf("PAT:A") != -1)
            {
                pc.AdultPATCon = pc.AdultPATCon.Substring(pc.AdultPATCon.IndexOf("PAT:A"));
                if (!pc.AdultPATCon.StartsWith(">"))
                {
                    pc.AdultPATCon = ">" + pc.AdultPATCon;
                }
            }
            if (pc.ChdPATCon.IndexOf("PAT:A*CH") != -1)
            {
                pc.ChdPATCon = pc.ChdPATCon.Substring(pc.ChdPATCon.IndexOf("PAT:A*CH"));
                if (!pc.ChdPATCon.StartsWith(">"))
                {
                    pc.ChdPATCon = ">" + pc.ChdPATCon;
                }
            }
            if (pc.INFPATCon.IndexOf("PAT:A*IN") != -1)
            {
                pc.INFPATCon = pc.INFPATCon.Substring(pc.INFPATCon.IndexOf("PAT:A*IN"));
                if (!pc.INFPATCon.StartsWith(">"))
                {
                    pc.INFPATCon = ">" + pc.INFPATCon;
                }
            }
            return pc;
        }

        /// <summary>
        /// 获取对应乘客类型的PAT字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        //private string GetPATCon(string SourcePnrCon, string PatType)
        //{
        //    string strSource = SourcePnrCon;
        //    string pattern = "";
        //    string itemPterm = @"(?<Num>\d{2})\s*(?<Seat>.+?)\s*FARE:(?<Fare>CNY[\d|\r|\n|\-| ]+\.(00|[\d|\r|\n|\-| ]+)|TEXEMPT|TEXEMPTCN)\s*TAX:(?<TAX>CNY[\d|\r|\n|\-| ]+\.(00|[\d|\r|\n|\-| ]+)|TEXEMPT|TEXEMPTCN)\s*YQ:(?<YQ>CNY[\d|\r|\n|\-| ]+\.(00|[\d|\r|\n|\-| ]+)|TEXEMPT|TEXEMPTCN|TEXEMPTYQ)\s*TOTAL:(?<TOTAL>[\d|\r|\n|\-| ]+\.(00|[\d|\r|\n|\-| ]+))\s*.*?SFC:(?<SFCNum>\d{1,2})\s*(SFN\:\d{1,2})?";
        //    if (PatType == "1")
        //    {
        //        pattern = @"(?<pat>PAT:A(?!\*(CH|IN))\s*((?<num>0\d{1,2})\s*[0-9A-Za-z\+\/\-]{1,}\s*FARE:(\n|.)*?SFC\:\d{1,2}\s*(SFN\:\d{1,2})?)+)";
        //    }
        //    else if (PatType == "2")
        //    {
        //        pattern = @"PAT\:A\*CH\s*" + itemPterm + @"(" + itemPterm + @"){0,}";
        //    }
        //    else if (PatType == "3")
        //    {
        //        pattern = @"PAT\:A\*IN\s*" + itemPterm + @"(" + itemPterm + @"){0,}";
        //    }
        //    Match ch = Regex.Match(strSource, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
        //    if (ch.Success)
        //    {
        //        strSource = ch.Value;
        //    }
        //    return strSource.ToUpper();
        //}

        /// <summary>
        /// 获取对应乘客类型的PAT字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetPATCon(string SourcePnrCon, string PatType)
        {
            string strSource = SourcePnrCon;
            string pattern = "";
            if (PatType == "1")
            {
                pattern = @"(?<=PAT\:A(?!\*(CH|IN))\s*0\d{1,2})\s*(?<item>[\s|\S]+)";
                Match ch = Regex.Match(strSource, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                if (ch.Success)
                {
                    strSource = ch.Value;
                    if (strSource.Contains("PAT:A"))
                    {
                        strSource = strSource.Substring(0, strSource.IndexOf("PAT:A"));
                    }
                    strSource = "PAT:A \r01" + strSource.Replace(">", "");
                }
                else
                {
                    strSource = "";
                }
            }
            else if (PatType == "2")
            {
                pattern = @"(?<=PAT\:A\*CH\s*0\d{1,2})\s*(?<item>[\s|\S]+)";
                Match ch = Regex.Match(strSource, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                if (ch.Success)
                {
                    strSource = ch.Value;
                    if (strSource.Contains("PAT:A"))
                    {
                        strSource = strSource.Substring(0, strSource.IndexOf("PAT:A"));
                    }
                    strSource = "PAT:A*CH \r01" + strSource.Replace(">", "");
                }
                else
                {
                    strSource = "";
                }
            }
            else if (PatType == "3")
            {
                pattern = @"(?<=PAT\:A\*IN\s*0\d{1,2})\s*(?<item>[\s|\S]+)";
                Match ch = Regex.Match(strSource, pattern, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                if (ch.Success)
                {
                    strSource = ch.Value;
                    if (strSource.Contains("PAT:A"))
                    {
                        strSource = strSource.Substring(0, strSource.IndexOf("PAT:A"));
                    }
                    strSource = "PAT:A*IN \r01" + strSource.Replace(">", "");
                }
                else
                {
                    strSource = "";
                }
            }
            strSource = strSource.Trim().ToUpper();
            return strSource;
        }


        /// <summary>
        /// 去掉重复的RT内容 及其编码后面的备注信息 已最先出现的序号为一条内容
        /// </summary>
        /// <returns></returns>
        public string DelRepeatRTCon(string rtCon, ref string Remark)
        {
            string RegSkyPattern = @"(?<left>\s*(?<carry>\*?\w{2}\d{3,4}\w?)\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<flyDate>[A-Za-z]{2}\s*\d{2}\s*[A-Za-z]{3}(\d{2})?)\s*(?<city>[A-Za-z]{6})\s*(?<state>[^\w]{0,}[A-Za-z]{2}\d{0,3})\s*(?<startTime>[^\d]{0,}\d\s*\d\s*\d\s*\d)\s*(?<endTime>[^\d]{0,}\d{4}\+{0,1}\d{0,1})\s*)(?<orther>[\w\S\.\s]*)";
            List<string> rtList = NewSplitPnr(rtCon);
            //单项处理 找出备注信息
            string strReplaceData = string.Empty;
            for (int i = 0; i < rtList.Count; i++)
            {
                if (!Regex.Match(rtList[i], RegSkyPattern, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled).Success)
                {
                    if (rtList[i].Contains("+"))
                    {
                        Remark = rtList[i].Substring(rtList[i].IndexOf("+") + 1);
                        break;
                    }
                }
            }
            string result = string.Join("", RepeatLine(rtList).ToArray());
            if (!string.IsNullOrEmpty(Remark))
            {
                //去掉备注信息
                result = result.Replace(Remark, "");
            }
            return result;
        }


        /// <summary>
        /// 判断字符串是否为6位编码格式
        /// </summary>
        /// <returns></returns>
        public bool IsPnr(string Pnr)
        {
            bool _IsPnr = false;
            bool IsNumber = Regex.Match(Pnr, @"\d{6}").Success;
            // bool IsLetter = Regex.Match(Pnr, @"[A-Za-z]{6}").Success;
            if (!string.IsNullOrEmpty(Pnr)
                && Pnr.Trim().Length == 6
                && Pnr.Trim().ToLower() != "oooooo"
                && Pnr.Trim().ToLower() != "unable"
                && !IsNumber
                // && !IsLetter
                )
            {

                char First = ' ';
                Pnr = Pnr.Trim().Replace(" ", "").Replace("\r", "").Replace("\n", "").Replace("\t", "");
                //1.编码字符不能全部一样 2.不能全部是数字
                int _n = 0;
                if (!int.TryParse(Pnr, out _n))
                {
                    char[] chArr = Pnr.ToCharArray();
                    foreach (Char chr in chArr)
                    {
                        if (First == ' ')
                        {
                            First = chr;
                        }
                        else
                        {
                            if (First != chr)
                            {
                                if (Regex.Match(Pnr, "[A-Za-z0-9]{6}").Success)
                                {
                                    _IsPnr = true;
                                }
                                break;
                            }
                        }
                    }
                }
            }
            return _IsPnr;
        }

        /// <summary>
        /// 编码内容是否存在大编码
        /// </summary>
        /// <param name="PnrContent"></param>
        /// <returns></returns>
        public bool IsExistBigCode(string PnrContent)
        {
            bool IsExist = false;
            string strMsg = string.Empty;
            string strBigCode = GetBigCode(PnrContent, out strMsg);
            if (IsPnr(strBigCode))
            {
                IsExist = true;
            }
            return IsExist;
        }
        /// <summary>
        /// 开始时间段和结束时间段比较 1开始时间比结束时间大 0开始时间比结束时间相等 -1开始时间比结束时间小
        /// </summary>
        /// <param name="start">00:00</param>
        /// <param name="end">23:59</param>
        /// <returns></returns>
        public static int CompareTime(string start, string end)
        {
            int compare = -1;
            if (!string.IsNullOrEmpty(start) &&
                !string.IsNullOrEmpty(end) &&
                start.Split(':').Length == 2 && end.Split(':').Length == 2)
            {
                compare = DateTime.Compare(DateTime.Parse("1900-01-01 " + start + ":00"), DateTime.Parse("1900-01-01 " + end + ":00"));
            }
            return compare;
        }

        /// <summary>
        /// 获取两个时间段的交集 没有交集返回第一段时间 返回 格式:HH:mm-HH:mm
        /// </summary>
        /// <param name="Time1">格式:HH:mm-HH:mm</param>
        /// <param name="Time2">格式:HH:mm-HH:mm</param>
        /// <returns></returns>
        public static string GetIntersectionTimeSlot(string Time1, string Time2)
        {
            string result = Time1;
            string[] strTimeArr1 = Time1.Split('-');
            string[] strTimeArr2 = Time2.Split('-');
            if (strTimeArr1.Length == 2 && strTimeArr2.Length == 2
                && strTimeArr1[0].Trim() != "" && strTimeArr1[1].Trim() != ""
                && strTimeArr2[0].Trim() != "" && strTimeArr2[1].Trim() != ""
                )
            {
                #region 第一段
                string defaultTime = "";
                string[] startime = strTimeArr1[0].Split(':');
                string start_h1 = string.Empty;
                string start_m1 = string.Empty;
                if (startime.Length >= 2)
                {
                    start_h1 = startime[0];
                    start_m1 = startime[1];
                }
                DateTime start1 = DateTime.Parse("1900-01-01 " + start_h1 + ":" + start_m1 + ":00");
                string[] endtime = strTimeArr1[1].Split(':');
                string end_h1 = string.Empty;
                string end_m1 = string.Empty;
                if (endtime.Length >= 2)
                {
                    end_h1 = endtime[0];
                    end_m1 = endtime[1];
                }
                DateTime end1 = DateTime.Parse("1900-01-01 " + end_h1 + ":" + end_m1 + ":00");
                defaultTime = start_h1 + ":" + start_m1 + "-" + end_h1 + ":" + end_m1;
                #endregion

                #region 第二段
                startime = strTimeArr2[0].Split(':');
                start_h1 = string.Empty;
                start_m1 = string.Empty;
                if (startime.Length >= 2)
                {
                    start_h1 = startime[0];
                    start_m1 = startime[1];
                }
                DateTime start2 = DateTime.Parse("1900-01-01 " + start_h1 + ":" + start_m1 + ":00");
                endtime = strTimeArr2[1].Split(':');
                end_h1 = string.Empty;
                end_m1 = string.Empty;
                if (endtime.Length >= 2)
                {
                    end_h1 = endtime[0];
                    end_m1 = endtime[1];
                }
                DateTime end2 = DateTime.Parse("1900-01-01 " + end_h1 + ":" + end_m1 + ":00");
                #endregion

                #region 筛选取值
                string ReStart = string.Empty;
                string ReEnd = string.Empty;

                if (DateTime.Compare(start1, start2) > 0)
                {
                    ReStart = start1.ToString("HH:mm");
                }
                else
                {
                    ReStart = start2.ToString("HH:mm");
                }

                if (DateTime.Compare(end2, end1) > 0)
                {
                    ReEnd = end1.ToString("HH:mm");
                }
                else
                {
                    ReEnd = end2.ToString("HH:mm");
                }
                if (DateTime.Compare(DateTime.Parse("1900-01-01 " + ReStart + ":00"), DateTime.Parse("1900-01-01 " + ReEnd + ":00")) > 0)
                {
                    //开始时间比结束时间大 没有交集 默认取第一段时间
                    result = defaultTime;
                }
                else
                {
                    result = ReStart + "-" + ReEnd;
                }
                #endregion
            }
            return result;
        }

        /// <summary>
        /// 获取Pnr内容的字节长度
        /// </summary>
        /// <param name="pnrContent"></param>
        /// <returns></returns>
        public int GetPnrContentByteLength(string pnrContent)
        {
            int result = pnrContent.Length;
            if (!string.IsNullOrEmpty(pnrContent))
            {
                result = Encoding.Default.GetByteCount(pnrContent);
            }
            return result;
        }

        /// <summary>
        /// 格式化QT指令的返回数据
        /// </summary>
        /// <param name="strContent"></param>
        /// <returns></returns>
        public QT_Queue FormatQT(string strQt)
        {
            QT_Queue QT = null;
            string reg0 = @"QT\s(?<office>\w{6})";
            string reg1 = @"(?<qtype>[a-zA-Z]{2})\s(?<count>\d{4})\s(?<maxcount>\d{4})";
            Match ch = Regex.Match(strQt, reg0, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (ch.Success)
            {
                QT = new QT_Queue();
                QT.Office = ch.Groups["office"].Value.Trim();
                MatchCollection chs = Regex.Matches(strQt, reg1, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
                foreach (Match item in chs)
                {
                    if (item.Success)
                    {
                        QT.QTList.Add(new QT_Queue.QT_Item(item.Groups["qtype"].Value.ToUpper().Trim(), item.Groups["count"].Value.Trim(), item.Groups["maxcount"].Value.Trim()));
                    }
                }
            }
            return QT;
        }
        public string GetCTCT(string QnResult)
        {
            string strCTCT = string.Empty;
            string reg0 = @"\s*(?<=.*?CTCT)\s*(?<phone>[\d|\-]+)?\s*";
            Match ch = Regex.Match(QnResult, reg0, RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.Compiled);
            if (ch.Success)
            {
                strCTCT = ch.Groups["phone"].Value.Trim();
            }
            return strCTCT;
        }
    }
}
