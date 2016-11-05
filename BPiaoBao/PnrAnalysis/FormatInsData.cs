using System;
using System.Collections.Generic;
using System.Text;
using PnrAnalysis.Model;
using System.Text.RegularExpressions;
using System.Threading;
namespace PnrAnalysis
{
    /// <summary>
    /// 指令类型
    /// </summary>
    public enum InsType
    {
        RT = 0,
        PAT = 1,
        Detr = 2,
        AVH = 3,
        FD = 4
    }
    /// <summary>
    /// 参数对象
    /// </summary>
    public class ParamObject
    {
        /// <summary>
        /// 使用PID用户
        /// </summary>
        public string WebUserName = "abc";
        /// <summary>
        /// 使用PID密码
        /// </summary>
        public string WebPwd = "123";
        /// <summary>
        /// 服务器IP
        /// </summary>
        public string ServerIP = "127.0.0.1";
        /// <summary>
        /// 服务器端口
        /// </summary>
        public int ServerPort = 350;
        /// <summary>
        /// 发送指令
        /// </summary>
        public string code = string.Empty;
        /// <summary>
        /// 返回内容
        /// </summary>
        public string recvData = string.Empty;
        /// <summary>
        /// Office
        /// </summary>
        public string Office = string.Empty;
        /// <summary>
        /// 是否需要PN
        /// </summary>
        public bool IsPn = false;
        /// <summary>
        /// 是否需要返回RT所有内容
        /// </summary>
        public bool IsAllResult = false;
        /// <summary>
        /// 是否使用大配置
        /// </summary>
        public bool UseBigCode = false;
        /// <summary>
        /// 编码是否为大编码
        /// </summary>
        public bool IsBigCode = false;
        /// <summary>
        /// PN指令时间间隔
        /// </summary>
        public int PNSleep = 100;

        /// <summary>
        /// Pid
        /// </summary>
        public string PID = string.Empty;
        /// <summary>
        /// KeyNo
        /// </summary>
        public string KeyNo = string.Empty;

        /// <summary>
        /// 是否使用扩展发送指令的方法
        /// </summary>
        public bool IsUseExtend = false;
        /// <summary>
        /// 扩展数据
        /// </summary>
        public string ExtendData = string.Empty;
        /// <summary>
        /// 是否处理返回结果 如变大写 ^替换为\r
        /// </summary>
        public bool IsHandResult = true;
        /// <summary>
        /// 返回属性值
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sbStr = new StringBuilder();
            sbStr.AppendFormat("时间={0}\r\nServer={1}\r\nOffice={2}\r\n是否需要PN={3}\r\n是否所有内容={4}\r\n是否使用大配置={5}\r\n是否为大编码={6}\r\n指令时间间隔={7}\r\n指令={8}\r\n内容={9}", System.DateTime.Now, this.ServerIP + ":" + this.ServerPort, this.Office, this.IsPn, this.IsAllResult, this.UseBigCode, this.IsBigCode, this.PNSleep, this.code, this.recvData);
            return sbStr.ToString();
        }
    }
    /// <summary>
    /// 指向发送指令的方法的指针 即委托
    /// </summary>
    /// <param name="code"></param>
    /// <returns></returns>
    public delegate string SendDataHandler(ParamObject ParamM);

    /// <summary>
    /// 不同指令返回数据格式化类
    /// </summary>
    public class FormatInsData
    {
        /// <summary>
        /// 格式化具体指令返回
        /// </summary>
        /// <param name="SendIns"></param>
        /// <param name="code"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public InsModel FormatIns(SendDataHandler SendIns, ParamObject pm, InsType type)
        {
            InsModel IModel = new InsModel();

            if (SendIns != null)
            {
                if (!pm.code.ToLower().Trim().StartsWith("detr:"))
                {
                    //发送指令
                    string strRecvData = SendIns(pm);
                    int MaxPNCount = 6;//最大PN数
                    if (type == InsType.AVH)
                    {
                        List<string> avhList = new List<string>();
                        avhList.Add(strRecvData);
                        pm.code = "PN";
                        Thread.Sleep(pm.PNSleep);
                        strRecvData = SendIns(pm);
                        int i = 0;
                        while (i < MaxPNCount)
                        {
                            if (strRecvData.Contains("下一页") || strRecvData == avhList[avhList.Count - 1])
                            {
                                break;
                            }
                            else
                            {
                                if (!strRecvData.Contains("指令频繁") && !strRecvData.Contains("没有可供显示的页面"))
                                {
                                    avhList.Add(strRecvData);
                                }
                                strRecvData = SendIns(pm);
                                Thread.Sleep(pm.PNSleep);
                                i++;
                            }
                        }
                        IModel._avh = GetAVHData(avhList);
                    }
                    else if (type == InsType.FD)
                    {
                        #region 获取FD数据
                        if (!strRecvData.Contains("没有适用运价"))
                        {
                            //FD数据列表
                            List<string> FdList = new List<string>();
                            FdList.Add(strRecvData);
                            int PageIndex = 1, PageCount = 1;
                            string PagePattern = @"(?<=PAGE)\s*(?<PageIndex>\d+)\/(?<PageCount>\d+)\s*";
                            Match PageMch = Regex.Match(strRecvData, PagePattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            if (PageMch.Success)
                            {
                                int.TryParse(PageMch.Groups["PageIndex"].Value.Trim(), out PageIndex);
                                int.TryParse(PageMch.Groups["PageCount"].Value.Trim(), out PageCount);
                            }
                            if (PageIndex != PageCount)
                            {
                                for (int i = PageIndex; i < PageCount; i++)
                                {
                                    pm.code = "PN";
                                    strRecvData = SendIns(pm);
                                    FdList.Add(strRecvData);
                                }
                            }
                            IModel._fd = GetFDData(FdList);
                        }
                        #endregion
                    }
                    else if (type == InsType.PAT)
                    {
                        FormatPNR pnr = new FormatPNR();
                        string errMsg = "";
                        IModel._pat = pnr.GetPATInfo(strRecvData, out errMsg);
                    }
                }
                else
                {
                    if (type == InsType.Detr)
                    {
                        List<string> avhList = new List<string>();
                        IModel._detr = GetDetrModel(avhList, SendIns, pm);
                    }
                }
            }
            return IModel;
        }

        /// <summary>
        /// 解析FD数据
        /// </summary>
        /// <param name="FdDataList"></param>
        /// <returns></returns>
        public FDModel GetFDData(List<string> FdDataList)
        {
            bool IsGetHeader = false;
            //是否计算折扣
            bool IsJSPrice = true;
            FDModel fdmodel = new FDModel();
            foreach (string item in FdDataList)
            {
                string[] strArr = item.Split(new string[] { "\r" }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var fdItem in strArr)
                {
                    if (!fdItem.StartsWith("PFD") && !fdItem.StartsWith("FD:"))
                    {
                        //每项数据处理
                        string FdPattern = @"\s*(?<num>\d{1,3})\s*(?<Carry>\w{2})\/(?<Seat>\w+)\s*\/\s*(?<Fare1>[\d|\.]+)\=\s*(?<Fare2>[\d|\.]+)\s*\/(?<seat1>\w{1,2})\/(?<seat2>\w{1,2})\/\s*\/\s*\.\s*\/(?<date>.*?)\s*\/(?<Orther>\w+)\s*";
                        Match mchFd = Regex.Match(fdItem, FdPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                        if (mchFd.Success)
                        {
                            FdItemData fditem = new FdItemData();
                            fditem.num = mchFd.Groups["num"].Value.Replace("", "");
                            fditem.Carry = mchFd.Groups["Carry"].Value.Replace("", "");
                            fditem.Seat = mchFd.Groups["Seat"].Value.Replace("", "");
                            fditem.Fare1 = mchFd.Groups["Fare1"].Value.Replace("", "");
                            fditem.Fare2 = mchFd.Groups["Fare2"].Value.Replace("", "");
                            fditem.seat1 = mchFd.Groups["seat1"].Value.Replace("", "");
                            fditem.seat2 = mchFd.Groups["seat2"].Value.Replace("", "");
                            fditem.date = FormatPNR.GetYMD(mchFd.Groups["date"].Value.Replace("", ""), DataFormat.dayMonthYear);
                            fditem.Orther = mchFd.Groups["Orther"].Value.Replace("", "");
                            fdmodel.FdDataList.Add(fditem);
                            if (fditem.Seat.Trim().ToUpper() == "Y")
                            {
                                fdmodel.FdYDataList.Add(fditem);
                            }
                        }
                    }
                    else
                    {
                        if (!IsGetHeader)
                        {
                            string FdPattern = @"(?<=FD:)\s*(?<city>.*?)\/(?<date>.*?)\/\s*.*?(?<=\/TPM)\s*(?<Mileage>\d+)\/\s*";
                            Match mchFd = Regex.Match(fdItem, FdPattern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                            if (mchFd.Success)
                            {
                                string city = mchFd.Groups["city"].Value.Trim().ToUpper().Replace("", "");
                                string date = mchFd.Groups["date"].Value.Trim().Replace("", "");
                                string Mileage = mchFd.Groups["Mileage"].Value.Trim() == "" ? "0" : mchFd.Groups["Mileage"].Value.Trim();
                                fdmodel.fdMileage = Mileage.Replace("", "");
                                fdmodel.fdDate = FormatPNR.GetYMD(date, DataFormat.dayMonthYear);
                                fdmodel.strDate = date;
                                fdmodel.fromCode = city.Substring(0, 3);
                                fdmodel.toCode = city.Substring(3);
                                IsGetHeader = true;
                            }
                        }
                    }
                }//数据
            }//PN
            if (IsJSPrice)
            {
                for (int i = 0; i < fdmodel.FdDataList.Count; i++)
                {
                    JiSuanPrice(fdmodel.FdDataList[i], fdmodel.FdYDataList);
                }
            }
            fdmodel.TotalCount = fdmodel.FdDataList.Count.ToString();
            return fdmodel;
        }

        /// <summary>
        /// FD计算折扣 保留两位小数
        /// </summary>
        /// <param name="fditem"></param>
        /// <param name="YFdItem"></param>
        /// <returns></returns>
        private FdItemData JiSuanPrice(FdItemData fditem, List<FdItemData> YFdItem)
        {
            if (YFdItem != null && YFdItem.Count > 0)
            {
                FdItemData fdYitem = YFdItem.Find(delegate(FdItemData yfd)
                {
                    if (yfd.Carry.Trim().ToUpper() == fditem.Carry.Trim().ToUpper())
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                });
                if (fdYitem != null)
                {
                    fditem.DiscountRate = FormatPNR.GetZk(fdYitem.Fare1, fditem.Fare1).ToString();
                }
            }
            return fditem;
        }


        /// <summary>
        /// 获取AVH数据
        /// </summary>
        /// <returns></returns>
        public AVHModel GetAVHData(List<string> AVHDataList)
        {
            foreach (string AVHItem in AVHDataList)
            {
                string[] strArr = Regex.Split(AVHItem, @"\r\d{1,3}", RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
                bool IsStart = false;
                foreach (string item in strArr)
                {
                    if (item.StartsWith("-"))
                    {
                        IsStart = true;
                        //处理
                        continue;
                    }
                    else if (item.StartsWith("+"))
                    {
                        IsStart = false;
                        //处理
                        continue;
                    }
                    if (IsStart)
                    {
                        //处理                       
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Detr指令过去数据
        /// </summary>
        /// <param name="detrList"></param>
        /// <param name="SendIns"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        public DetrModel GetDetrModel(List<string> detrList, SendDataHandler SendIns, ParamObject pm)
        {
            DetrModel DM = new DetrModel();
            string DetrFPatern = @"^\s*(?<=detr)[\s|\:|\：]{1}TN\/\d{3,4}(\-?|\s+)\d{10}\,F\s*$";
            Match detrch = Regex.Match(pm.code, DetrFPatern, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);
            if (detrch.Success)
            {
                #region Detr ,F
                string pattern = @"^.*\nNAME:\s*(?<Name>[A-Z\u4e00-\u9fa5/ ]+)(INF)?.*?\s+TKTN:\s*(?<Tktn>\d{13})\s+RCPT:\s+\d\s+(?<Number1>.+)\s+(\d\s+(?<Number2>.+))?";
                Match match = Regex.Match(detrList[0], pattern);
                if (match.Success)
                {
                    DM.PassengerName = match.Groups["Name"].Value;
                    DM.TicketNumber = match.Groups["Tktn"].Value;
                    string[] strArray = new string[] { match.Groups["Number1"].Value, match.Groups["Number2"].Value };
                    foreach (string str2 in strArray)
                    {
                        if (str2.Length > 2)
                        {
                            if (str2.StartsWith("RP"))
                            {
                                DM.SerialNumber = str2.Substring(2).Trim(new char[] { ' ', '\r', '\n' });
                                if (DM.SerialNumber.Length > 4)
                                {
                                    DM.CheckVate = DM.SerialNumber.Substring(DM.SerialNumber.Length - 4, 4);
                                }
                            }
                            else if ((str2.StartsWith("NI") || str2.StartsWith("PP")) || str2.StartsWith("ID"))
                            {
                                DM.SsrCard = str2.Substring(2).Trim(new char[] { ' ', '\r', '\n' });
                            }
                        }
                    }
                }
                #endregion
            }
            else
            {
                //TakeTicketInfo();

            }
            return null;
        }

        /// <summary>
        /// 返回全部内容：DETR TN/票号,S
        /// </summary>
        /// <param name="strTicketNumber"></param>
        /// <param name="SendIns"></param>
        /// <param name="pm"></param>
        /// <returns></returns>
        private string TakeTicketInfo(string strTicketNumber, SendDataHandler SendIns, ParamObject pm)
        {
            string str3 = string.Empty; ;
            try
            {
                pm.code = string.Format("DETR TN/{0},S", strTicketNumber);
                int num = 0;
                string str2 = string.Empty;
                StringBuilder builder = new StringBuilder();
                while (num < 3)
                {
                    str2 = SendIns(pm);
                    if (string.IsNullOrEmpty(str2))
                    {
                        throw new Exception("获取数据时发生异常!");
                    }
                    if (((str2 == "通道忙") || str2.Contains("INCORRECT MESSAGE BACK,RETRY FOR ITINERARY PRINT")) || str2.Contains("ELECTRONIC TICKET TRANSACTION TIME OUT!!"))
                    {
                        Thread.Sleep(pm.PNSleep);
                        num++;
                    }
                    else
                    {
                        builder.Append(str2);
                        if (Regex.IsMatch(str2, @"\+\s+"))
                        {
                            pm.code = "PN1";
                        }
                        else
                        {
                            break;
                        }
                    }
                    Thread.Sleep(pm.PNSleep);
                }
                str3 = builder.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
            return str3;
        }


    }
}

