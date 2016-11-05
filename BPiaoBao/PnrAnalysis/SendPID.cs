﻿using System;
using System.Collections.Generic;
using System.Text;
using PBPid.WebManage;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using PnrAnalysis.Model;
using System.Web.Security;
using System.IO;
using System.Text.RegularExpressions;
namespace PnrAnalysis
{
    /// <summary>
    /// PID客户端发送指令类
    /// </summary>
    public class SendNewPID
    {
        /// <summary>
        /// 发送指令 新版PID
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string SendCommand(ParamObject param)
        {
            string recvData = string.Empty;
            //用于调试 需屏蔽            
            //=======================
            //if (param.ServerIP.Contains("192.168.8.3"))
            //{
            //    param.ServerIP = param.ServerIP.Replace("192.168.8.3", "203.88.210.227");
            //}
            //=======================
            //设置参数
            WebManage.ServerIp = param.ServerIP;//"192.168.2.107";// 
            WebManage.ServerPort = param.ServerPort;// 360;//
            //WebManage.WebUserName = param.WebUserName;
            //WebManage.WebPwd = param.WebPwd;
            param.code = param.code.Replace("\n", "").Replace("\r", "^");
            //去掉ig
            param.code = param.code.ToLower().StartsWith("ig|") ? param.code.Trim().Substring(3).ToLower() : param.code.ToLower();
            //去掉封口
            //param.code = (param.code.Trim().EndsWith("@") || param.code.Trim().EndsWith(@"\/")) ? param.code.Trim().Substring(0, param.code.Trim().LastIndexOf("|")) : param.code.Trim();
            //发送
            //recvData = WebManage.SendCommand(param.code, param.Office, param.IsPn, param.IsAllResult, param.ServerIP, param.ServerPort);

            //发送
            if (param.IsUseExtend)
            {
                recvData = WebManage.SendCommand(param.code, param.WebUserName, param.Office, param.IsPn, param.IsAllResult, param.ExtendData, param.ServerIP, param.ServerPort);
            }
            else
            {
                recvData = WebManage.SendCommand(param.code, param.WebUserName, param.Office, param.IsPn, param.IsAllResult, param.ServerIP, param.ServerPort);
            }
            if (recvData == null)
            {
                recvData = "";
            }
            if (param.IsHandResult)
            {
                recvData = recvData.Replace("^", "\r").ToUpper();
            }
            //在发送一次  分离编码,预订编码 还有xe项的指令不在重发
            if (recvData.Contains("LEASE WAIT - TRANSACTION IN PROGRESS") && !param.code.ToLower().Contains("|sp") && !param.code.ToLower().Contains("nm1") && !param.code.ToLower().Contains("|xe"))
            {
                StringBuilder sbLog = new StringBuilder();
                sbLog.Append("\r\n时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") + "\r\nIP:" + param.ServerIP + ":" + param.ServerPort + "\r\nOffice：" + param.Office + "\r\n发送指令：" + param.code.ToLower() + "\r\n接收数据:\r\n");
                if (param.IsUseExtend)
                {
                    recvData = WebManage.SendCommand(param.code, param.WebUserName, param.Office, param.IsPn, param.IsAllResult, param.ExtendData, param.ServerIP, param.ServerPort);
                }
                else
                {
                    recvData = WebManage.SendCommand(param.code, param.WebUserName, param.Office, param.IsPn, param.IsAllResult, param.ServerIP, param.ServerPort);
                }
                if (recvData == null)
                {
                    recvData = "";
                }
                if (param.IsHandResult)
                {
                    recvData = recvData.Replace("^", "\r").ToUpper();
                }
                sbLog.Append(recvData + "\r\n\r\n");
                //记录这个日志
                PnrAnalysis.LogText.LogWrite(sbLog.ToString(), "LEASE_WAIT");
            }
            return recvData;
        }

        #region 发送PID  旧版PID
        private int _ReceiveTimeout = 100 * 1000;//接受数据超时时间
        private int _SendTimeout = 100 * 1000;//发送数据超时时间
        /// <summary>
        /// 登陆Pid
        /// </summary> 
        private Socket Login(string BlankIP, string BlankPort, string piduser, string pidpwd)
        {
            Socket sock = null;
            //发送4次数据包
            try
            {
                Byte[] sendL1 = new Byte[162];
                Byte[] sendL2 = new Byte[162];
                Byte[] sendL3 = new Byte[162];
                Byte[] sendL4 = new Byte[162];

                IPAddress test1 = IPAddress.Parse(BlankIP);
                IPEndPoint point1 = new IPEndPoint(test1, int.Parse(BlankPort));

                sock = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                sock.SetSocketOption(SocketOptionLevel.Socket, SocketOptionName.SendTimeout, this._SendTimeout);
                sock.SendTimeout = this._SendTimeout;
                sock.ReceiveTimeout = this._ReceiveTimeout;
                sock.Connect(point1);
                if (sock.Connected)
                {
                    sendL1 = new EtermChar.LoginEterm().Login1(piduser, pidpwd);
                    //第一次发送数据
                    sock.Send(sendL1, sendL1.Length, 0);
                    Byte[] RecvL1 = new Byte[5000];
                    int btL1 = sock.Receive(RecvL1, RecvL1.Length, 0);

                    Thread.Sleep(1);


                    //第二次发送数据 ?
                    Byte[] sendOne2 = new EtermChar.LoginEterm().Login2();
                    System.Array.Copy(sendOne2, 0, sendL2, 0, sendOne2.Length);

                    int sLen = sock.Send(sendOne2, 0, sendOne2.Length, 0); //sock.Send(sendL2, sendL2.Length, 0);
                    byte[] recv1 = new byte[1024 * 10];
                    int btL2 = sock.Receive(recv1);

                    Thread.Sleep(1);

                    //第三次发送数据                 
                    Byte[] sendOne3 = new EtermChar.LoginEterm().Login3();
                    System.Array.Copy(sendOne3, 0, sendL3, 0, sendOne3.Length);
                    sLen = sock.Send(sendL3, sendL3.Length, 0);
                    recv1 = new byte[1024 * 10];
                    int btL3 = sock.Receive(recv1);

                    Thread.Sleep(1);

                    //第四次发送数据
                    Byte[] sendOne4 = new EtermChar.LoginEterm().Login4();
                    System.Array.Copy(sendOne4, 0, sendL4, 0, sendOne4.Length);
                    sock.Send(sendL4, sendL4.Length, 0);
                    recv1 = new byte[1024 * 10];
                    int btL4 = sock.Receive(recv1);

                    Thread.Sleep(10);
                }
            }
            catch (SocketException)
            {
                sock.Close();
                sock = null;
            }
            return sock;
        }

        public string Send(ParamObject param)
        {
            Socket skt = null;
            string value = "";
            try
            {
                skt = Login(param.ServerIP, param.ServerPort.ToString(), param.WebUserName, param.WebPwd);
                skt.SendTimeout = 5000;
                skt.ReceiveTimeout = 5000;
                param.code = param.code.Replace("\r\n", "\r").Replace("\n", "\r").Trim('\r');
                //汉字转换成拼音
                param.code = new EtermChar.CharSet().GetPinyin(param.code);

                Byte[] send1 = new EtermChar.CodeOperation().ReturnBlackCode(param.code);
                //2 接收
                Byte[] Recv1 = new Byte[3000];
                Byte[] oldReCode = new Byte[3000];
                if (skt.Connected)
                {
                    int available = skt.Available;
                    //1 发送
                    skt.Send(send1, send1.Length, 0);

                    Byte[] byteMessage = new Byte[3000];
                    try
                    {
                        Recv1 = new Byte[5000];
                        int counts = 0, c = 0;
                        try
                        {
                            byteMessage = new Byte[3000];
                            while (c < 3)
                            {
                                counts = skt.Receive(byteMessage, byteMessage.Length, 0);
                                Array.Copy(byteMessage, 0, Recv1, 0, counts);
                                value = value + new EtermChar.CharSet().CodeStr(Recv1, System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "ChineseFEC2.xml");
                                if (new EtermChar.CodeOperation().IsEnd(Recv1))
                                {
                                    c = 5;
                                }
                            }
                            value = ReplaceCode(value);
                        }
                        catch (SocketException ex)
                        {
                            skt.Close();
                        }
                    }
                    catch (SocketException ex)
                    {
                        skt.Close();
                    }
                }
            }
            finally
            {
                if (skt != null)
                {
                    if (skt.Connected)
                    {
                        skt.Close();
                    }
                    skt = null;
                }
            }
            return value;
        }

        //替换错误字符
        private string ReplaceCode(string str)
        {
            string strNew = "";
            strNew = str.Replace("正确的记录洁号", "正确的记录编号");
            strNew = str.Replace("因考虑到鹿享效率问题暚除AV指令后的PB可以支持外暚其他指令的的PB暂潦净支持摚", "因考虑到共享效率问题，除AV指令后的PB可以支持外，其他指令的的PB暂时不支持");
            strNew = str.Replace("航空公司使用自慷出疗潦限, 请检鹃PNR", "航空公司使用自动出票时限, 请检查PNR");
            return strNew;
        }

        #endregion

        #region 最新客户端发指令 需要登陆后才可使用
        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="UserName"></param>
        /// <param name="Passord"></param>
        /// <param name="ServerIP"></param>
        /// <param name="ServerPort"></param>
        /// <param name="strResult"></param>
        /// <returns></returns>
        public static bool Login(string UserName, string Passord, string ServerIP, int ServerPort, ref string strResult)
        {
            return PidSend.PidSend.Login(UserName, Passord, ServerIP, ServerPort, ref strResult);
        }
        /// <summary>
        /// 发送指令 需要登陆后才可使用
        /// </summary>
        /// <param name="cmd">发送指令</param>        
        /// <param name="cmdType">指令类型 1.标准Eterm指令 2.自定义指令</param>
        /// <param name="strResult">返回结果</param>
        /// <returns></returns>
        public static bool SendCommand(string cmd, int cmdType, ref string strResult)
        {
            bool IsSuc = false;
            if (PidSend.PidSend.PidUser != null)
            {
                IsSuc = PidSend.PidSend.PidUser.SendCommand(cmd, cmdType, ref strResult);
            }
            else
            {
                strResult = "用户未登录！";
            }
            return IsSuc;
        }
        #endregion


        #region 接口发送指令
        /// <summary>
        /// 预订编码发送指令
        /// </summary>
        /// <param name="InputParam"></param>
        /// <returns></returns>
        public RePnrObj ISendIns(PnrParamObj InputParam)
        {
            RePnrObj PnrObj = new RePnrObj();
            //指令日志
            StringBuilder sbLog = new StringBuilder();
            sbLog.AppendFormat("================时间:{0}  IP：{1} 端口:{2}  PID通道:{3}======================\r\n", System.DateTime.Now, InputParam.ServerIP, InputParam.ServerPort, InputParam.UsePIDChannel);

            List<IPassenger> AdultPasList = new List<IPassenger>();
            List<IPassenger> ChildPasList = new List<IPassenger>();
            List<IPassenger> YingerPasList = new List<IPassenger>();
            foreach (IPassenger pas in InputParam.PasList)
            {
                if (pas.PassengerType == 1)
                {
                    AdultPasList.Add(pas);
                }
                else if (pas.PassengerType == 2)
                {
                    ChildPasList.Add(pas);
                }
                else if (pas.PassengerType == 3)
                {
                    YingerPasList.Add(pas);
                }
            }
            try
            {
                #region 发送指令
                //拼英管理
                PinYingMange Pinyin = new PinYingMange();
                FormatPNR pnrformat = new FormatPNR();
                ECParam ecParam = new ECParam();
                ecParam.ECIP = InputParam.ServerIP;
                ecParam.ECPort = InputParam.ServerPort.ToString();
                ecParam.PID = InputParam.PID;
                ecParam.KeyNo = InputParam.KeyNo;
                ecParam.UserName = InputParam.UserName;
                string Office = (InputParam.Office == "" ? "" : "&" + InputParam.Office + "$");
                SendEC sendec = new SendEC(ecParam);
                //设置IP port
                WebManage.ServerIp = InputParam.ServerIP;
                WebManage.ServerPort = InputParam.ServerPort;
                //设置返回数据
                PnrObj.Office = InputParam.Office;
                PnrObj.ServerIP = InputParam.ServerIP;
                PnrObj.ServerPort = InputParam.ServerPort.ToString();
                ParamObject pm = new ParamObject();
                pm.ServerIP = InputParam.ServerIP;
                pm.ServerPort = InputParam.ServerPort;
                pm.Office = InputParam.Office;
                pm.WebUserName = InputParam.UserName;

                //格式化成人定编码字符串               
                string errMsg = "", AdultIns = "";
                string sendTime = "", recvTime = "";
                string pnrRemark = string.Empty;
                //不是
                if (InputParam.IsInterface == 0)
                {
                    //成人定编码指令
                    if (AdultPasList.Count > 0)
                    {
                        AdultIns = GetYuDingIns(InputParam, AdultPasList);
                        if (!InputParam.FlyTimeIsOverCurrTime)
                        {
                            if (InputParam.UsePIDChannel == 2)
                            {
                                AdultIns = AdultIns.Split('@')[0] + "@";
                                AdultIns = AdultIns.Replace("\n", "").Replace("\r", "^");
                                sbLog.AppendFormat("时间:{0}\r\n发送成人预订编码指令:\r\n{1}\r\n", System.DateTime.Now, AdultIns);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                pm.code = AdultIns;
                                pm.IsPn = false;
                                //发送
                                string recvData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                                //指令信息                                
                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                {
                                    InsType = NewInsType._YD_Adult,
                                    SendCmd = AdultIns,
                                    SendTime = sendTime,
                                    RecvData = recvData,
                                    RecvTime = recvTime
                                });

                                PnrObj.AdultYudingList.Add(AdultIns + " Office:" + InputParam.Office, recvData);

                                if (!recvData.ToUpper().Contains("UNABLE") && !recvData.Contains("超时") && !recvData.Contains("无法从传输连接中读取数据") && !recvData.Contains("目标机器") && !recvData.Contains("服务器忙"))
                                {
                                    //成人编码
                                    PnrObj.AdultPnr = pnrformat.GetPNR(recvData, out errMsg);
                                    sbLog.AppendFormat("时间:{0}\r\n解析出成人PNR:{1}\r\n", System.DateTime.Now, PnrObj.AdultPnr);
                                    if (InputParam.IsGetSpecialPrice == 0)
                                    {
                                        //预订成人PNR状态信息      
                                        PnrObj.YDAdult = pnrformat.GetYuDingInfo(recvData, out errMsg);
                                        sbLog.AppendFormat("时间:{0}\r\n解析出预订成人PNR:{1}状态:{2}\r\n", System.DateTime.Now, PnrObj.AdultPnr, PnrObj.YDAdult != null ? PnrObj.YDAdult.YDPnrStatus : "");
                                    }
                                }


                                #region   //检测成人编码是否生成
                                if (recvData.Contains("LEASE WAIT") && PnrObj.AdultPnr == "")
                                {
                                    //获取成人编码超时
                                    PnrObj.GetAdultPnrIsTimeOut = true;
                                    //检测编码生成功没有                               
                                    string m_PasPinYin = WebManage.GetPinYin(AdultPasList[0].PassengerName);
                                    m_PasPinYin = string.IsNullOrEmpty(m_PasPinYin) ? WebManage.GetPinYin(AdultPasList[0].PassengerName) : "";
                                    if (string.IsNullOrEmpty(m_PasPinYin))
                                    {
                                        m_PasPinYin = PinYingMange.GetSpellByChinese(AdultPasList[0].PassengerName);
                                    }
                                    string date = FormatPNR.DateToStr(InputParam.SkyList[0].FlyStartDate, DataFormat.dayMonth);
                                    string FlightNum = InputParam.SkyList[0].CarryCode.Replace("*", "") + InputParam.SkyList[0].FlightCode.Replace("*", "");
                                    string DisaplySeat = InputParam.SkyList[0].Space.ToUpper();
                                    AdultIns = "RT" + m_PasPinYin + "/" + FlightNum + "/" + date;
                                    sbLog.AppendFormat("时间:{0}\r\n发送成人检测编码指令:\r\n{1}\r\n", System.DateTime.Now, AdultIns);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    pm.code = AdultIns;
                                    pm.IsPn = false;
                                    //发送
                                    recvData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收成人检测数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                                    //指令信息                                    
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._RT_Name,
                                        SendCmd = AdultIns,
                                        SendTime = sendTime,
                                        RecvData = recvData,
                                        RecvTime = recvTime
                                    });
                                    //解析数据
                                    List<LEASEWAIT_Info> Waitlst = pnrformat.GetPnrList(recvData);
                                    string tempPnr = "";
                                    if (Waitlst.Count > 0)
                                    {
                                        foreach (LEASEWAIT_Info info in Waitlst)
                                        {
                                            //判断编码状态和人数   添加舱位
                                            if (info.PnrStatus.Contains("HK") && info.PasCount == AdultPasList.Count.ToString())
                                            {
                                                PnrObj.AdultPnr_LEASEWAIT.Add(info.PNR);
                                                //添加舱位比较 添加Office
                                                if (info.Seat == DisaplySeat && pm.Office.ToUpper() == info.Office)
                                                {
                                                    tempPnr = info.PNR;

                                                    #region  //检测到再次检测乘客名字

                                                    AdultIns = "RT" + tempPnr;
                                                    sbLog.AppendFormat("时间:{0}\r\n发送成人再次检测乘客名字指令:\r\n{1}\r\n", System.DateTime.Now, AdultIns);
                                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    pm.code = AdultIns;
                                                    pm.IsPn = false;
                                                    //发送
                                                    recvData = SendCommand(pm);
                                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n接收成人再次检测乘客名字返回数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                                                    //指令信息                                                    
                                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                    {
                                                        InsType = NewInsType._RT,
                                                        SendCmd = AdultIns,
                                                        SendTime = sendTime,
                                                        RecvData = recvData,
                                                        RecvTime = recvTime
                                                    });
                                                    string strInnerMsg = "";
                                                    //是否存在该乘客名字
                                                    bool IsExist = false;
                                                    bool skyExist = false;
                                                    PnrModel JCPnrModel = pnrformat.GetPNRInfo(tempPnr, recvData, false, out strInnerMsg);
                                                    if (JCPnrModel != null)
                                                    {
                                                        if (JCPnrModel._PassengerList.Count > 0)
                                                        {
                                                            foreach (PassengerInfo item in JCPnrModel._PassengerList)
                                                            {
                                                                if (item.PassengerName == AdultPasList[0].PassengerName)
                                                                {
                                                                    IsExist = true;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                        //乘客名字相同在验证航段信息
                                                        if (IsExist && JCPnrModel._LegList.Count > 0)
                                                        {
                                                            //排序
                                                            JCPnrModel._LegList.Sort(delegate(LegInfo sky1, LegInfo sky2)
                                                            {
                                                                DateTime d1 = DateTime.Parse(sky1.FlyDate1 + " " + sky1.FlyStartTime.Insert(2, ":") + ":00");
                                                                DateTime d2 = DateTime.Parse(sky2.FlyDate1 + " " + sky2.FlyStartTime.Insert(2, ":") + ":00");
                                                                return (DateTime.Compare(d1, d2));
                                                            });
                                                            //比较起飞日期
                                                            //比较城市
                                                            //比较航空公司   
                                                            for (int i = 0; i < JCPnrModel._LegList.Count; i++)
                                                            {
                                                                LegInfo leg = JCPnrModel._LegList[i];
                                                                ISkyLeg sky = InputParam.SkyList[i];
                                                                DateTime legFlyDate = DateTime.Parse(leg.FlyDate1 + " " + leg.FlyStartTime.Insert(2, ":") + ":00");
                                                                DateTime skyFlyDate = DateTime.Parse(sky.FlyStartDate + " " + sky.FlyStartTime.Insert(2, ":") + ":00");
                                                                if (legFlyDate == skyFlyDate
                                                                    && leg.FromCode.ToUpper() == sky.fromCode.ToUpper()
                                                                    && leg.ToCode.ToUpper() == sky.toCode.ToUpper()
                                                                    && leg.AirCode.ToUpper() == sky.CarryCode.ToUpper()
                                                                    )
                                                                {
                                                                    skyExist = true;
                                                                }
                                                                else
                                                                {
                                                                    skyExist = false;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (IsExist && skyExist)
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        tempPnr = "";
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }//endfor
                                    }
                                    //赋值给实体                              
                                    if (!string.IsNullOrEmpty(tempPnr))
                                    {
                                        //成人编码
                                        PnrObj.AdultPnr = tempPnr;
                                        if (PnrObj.YDAdult != null)
                                        {
                                            PnrObj.YDAdult.YDPnrStatus = "HK";
                                        }
                                        sbLog.AppendFormat("时间:{0}\r\n解析出编码:{1}\r\n舱位比较得出编码PNR:{2}\r\n", System.DateTime.Now, string.Join("|", PnrObj.AdultPnr_LEASEWAIT.ToArray()), tempPnr);
                                    }
                                    else //直接取RT信息中的编码
                                    {
                                        string m_pnr = pnrformat.GetPNR(recvData, out errMsg);

                                        #region 检测PNR是对应所预订的编码信息
                                        //对比乘客姓名，人数 航段信息 避免串数据(返回的是RT数据)
                                        PnrModel tempPnrModel = pnrformat.GetPNRInfo(m_pnr, recvData, false, out errMsg);
                                        bool PnrIsCorrent = true;//检测编码是否对
                                        //检测乘客姓名是否一致
                                        if (tempPnrModel._PassengerList.Count > 0 && tempPnrModel._PassengerList.Count == AdultPasList.Count
                                            )
                                        {
                                            foreach (IPassenger item in AdultPasList)
                                            {
                                                if (!tempPnrModel._PassengerList.Exists(delegate(PassengerInfo p)
                                                {
                                                    return p.PassengerName == item.PassengerName;
                                                }))
                                                {
                                                    PnrIsCorrent = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            PnrIsCorrent = false;
                                        }
                                        //检测Office是否一致
                                        if (PnrIsCorrent && pm.Office.ToUpper() != tempPnrModel._Office.ToUpper())
                                        {
                                            PnrIsCorrent = false;
                                        }
                                        if (PnrIsCorrent)
                                        {
                                            //排序
                                            tempPnrModel._LegList.Sort(delegate(LegInfo sky1, LegInfo sky2)
                                            {
                                                DateTime d1 = DateTime.Parse(sky1.FlyDate1 + " " + sky1.FlyStartTime.Insert(2, ":") + ":00");
                                                DateTime d2 = DateTime.Parse(sky2.FlyDate1 + " " + sky2.FlyStartTime.Insert(2, ":") + ":00");
                                                return (DateTime.Compare(d1, d2));
                                            });
                                            //比较起飞日期
                                            //比较城市
                                            //比较航空公司   
                                            for (int i = 0; i < tempPnrModel._LegList.Count; i++)
                                            {
                                                LegInfo leg = tempPnrModel._LegList[i];
                                                ISkyLeg sky = InputParam.SkyList[i];
                                                DateTime legFlyDate = DateTime.Parse(leg.FlyDate1 + " " + leg.FlyStartTime.Insert(2, ":") + ":00");
                                                DateTime skyFlyDate = DateTime.Parse(sky.FlyStartDate + " " + sky.FlyStartTime.Insert(2, ":") + ":00");
                                                if (!(legFlyDate == skyFlyDate
                                                    && leg.FromCode.ToUpper() == sky.fromCode.ToUpper()
                                                    && leg.ToCode.ToUpper() == sky.toCode.ToUpper()
                                                    && leg.AirCode.ToUpper() == sky.CarryCode.ToUpper()
                                                    && leg.Seat.ToUpper() == sky.Space.ToUpper()
                                                    && leg.AirCodeFlightNum.ToUpper() == (sky.CarryCode.ToUpper() + sky.FlightCode)
                                                    ))
                                                {
                                                    PnrIsCorrent = false;
                                                    break;
                                                }
                                            }
                                        }
                                        #endregion

                                        if (m_pnr != "" && recvData.Trim() != "" && m_pnr.Trim().Length == 6 && PnrIsCorrent)
                                        {
                                            //成人编码
                                            PnrObj.AdultPnr = m_pnr;
                                            sbLog.AppendFormat("时间:{0}\r\npnrformat.GetPNR解析出成人检测编码PNR:{1}\r\n", System.DateTime.Now, PnrObj.AdultPnr);
                                        }
                                    }
                                }
                                #endregion

                            }
                            else
                            {
                                sbLog.AppendFormat("时间:{0}\r\n发送成人预订编码指令:\r\n{1}\r\n", System.DateTime.Now, AdultIns + Office);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                string recvData = sendec.SendData(AdultIns, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                //指令信息                                
                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                {
                                    InsType = NewInsType._YD_Adult,
                                    SendCmd = AdultIns,
                                    SendTime = sendTime,
                                    RecvData = recvData,
                                    RecvTime = recvTime
                                });
                                PnrObj.AdultYudingList.Add(AdultIns + " Office:" + InputParam.Office, recvData);

                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                                if (!recvData.ToUpper().Contains("UNABLE") && !recvData.Contains("超时") && !recvData.Contains("无法从传输连接中读取数据") && !recvData.Contains("目标机器") && !recvData.Contains("服务器忙"))
                                {
                                    //成人编码
                                    PnrObj.AdultPnr = pnrformat.GetPNR(recvData, out errMsg);
                                    sbLog.AppendFormat("时间:{0}\r\n解析出成人PNR:{1}\r\n", System.DateTime.Now, PnrObj.AdultPnr);
                                    if (InputParam.IsGetSpecialPrice == 0)
                                    {
                                        //预订成人PNR状态信息      
                                        PnrObj.YDAdult = pnrformat.GetYuDingInfo(recvData, out errMsg);
                                        sbLog.AppendFormat("时间:{0}\r\n解析出预订成人PNR:{1}状态:{2}\r\n", System.DateTime.Now, PnrObj.AdultPnr, PnrObj.YDAdult != null ? PnrObj.YDAdult.YDPnrStatus : "");
                                    }
                                }
                            }
                            if (string.IsNullOrEmpty(PnrObj.AdultPnr) || PnrObj.AdultPnr.Trim().Length != 6)
                            {
                                //成人编码生成失败
                                PnrObj.AdultPnr = "";
                                sbLog.AppendFormat("时间:{0}\r\n成人编码生成失败\r\n", System.DateTime.Now);
                            }
                            else
                            {
                                #region HU特殊舱位 添加备注指令

                                if (InputParam.CarryCode.ToUpper().Trim() == "HU")
                                {
                                    string str_Space = "", addIns = "";
                                    foreach (var item in InputParam.SkyList)
                                    {
                                        str_Space += item.Space.Trim().ToUpper() + "/";
                                    }
                                    if (str_Space.Contains("A") || str_Space.Contains("D"))
                                    {
                                        addIns = "OSI HU CKIN SSAC/S1";
                                    }
                                    else if (str_Space.Contains("I"))
                                    {
                                        addIns = "OSI HU CKIN TQGP/S1";
                                    }
                                    else if (str_Space.Contains("W"))
                                    {
                                        addIns = "OSI HU CKIN HJSF/S1";
                                    }
                                    string rmkCmd = string.Format("RT{0}|{1}^\\", PnrObj.AdultPnr.Trim(), addIns);
                                    if (!string.IsNullOrEmpty(addIns))
                                    {
                                        if (InputParam.UsePIDChannel == 2)
                                        {
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\n{1}\r\n", System.DateTime.Now, rmkCmd);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            pm.code = rmkCmd;
                                            pm.IsPn = false;
                                            //发送
                                            string recvData = SendCommand(pm);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                                            //指令信息                                            
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._HU_SpRmk,
                                                SendCmd = rmkCmd,
                                                SendTime = sendTime,
                                                RecvData = recvData,
                                                RecvTime = recvTime
                                            });
                                        }
                                        else
                                        {
                                            rmkCmd = string.Format("RT{0}|{1}|@", PnrObj.AdultPnr.Trim(), addIns);
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, rmkCmd + Office);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            string recvData = sendec.SendData(rmkCmd, out errMsg);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            //指令信息                                            
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._HU_SpRmk,
                                                SendCmd = rmkCmd,
                                                SendTime = sendTime,
                                                RecvData = recvData,
                                                RecvTime = recvTime
                                            });
                                            PnrObj.AdultYudingList.Add(AdultIns + " Office:" + InputParam.Office, recvData);
                                        }
                                    }
                                }

                                #endregion
                            }
                        }
                        else
                        {
                            sbLog.AppendFormat("时间:{0}\r\n成人起飞时间已小于预定时间,不预定！\r\n", System.DateTime.Now);
                        }
                    }

                    #region  //儿童
                    if (ChildPasList != null && ChildPasList.Count > 0)
                    {
                        //格式化成人定编码字符串
                        string _ChildIns = GetYuDingIns(InputParam, ChildPasList);
                        if (!InputParam.FlyTimeIsOverCurrTime)
                        {
                            string recvChildData = "";
                            if (InputParam.UsePIDChannel == 2)
                            {
                                _ChildIns = _ChildIns.Split('@')[0] + "@";
                                _ChildIns = _ChildIns.Replace("\n", "").Replace("\r", "^");
                                sbLog.AppendFormat("时间:{0}\r\n发送儿童预订编码指令:\r\n{1}\r\n", System.DateTime.Now, _ChildIns);
                                //发送
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                pm.code = _ChildIns;
                                pm.IsPn = false;
                                //发送
                                recvChildData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvChildData);
                                //指令信息                                
                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                {
                                    InsType = NewInsType._YD_Chd,
                                    SendCmd = _ChildIns,
                                    SendTime = sendTime,
                                    RecvData = recvChildData,
                                    RecvTime = recvTime
                                });
                                PnrObj.ChildYudingList.Add(_ChildIns + " Office:" + InputParam.Office, recvChildData);
                            }
                            else
                            {
                                sbLog.AppendFormat("时间:{0}\r\n发送儿童预订编码指令:\r\n{1}\r\n", System.DateTime.Now, _ChildIns + Office);
                                //返回儿童编码字符串
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                recvChildData = sendec.SendData(_ChildIns, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvChildData);

                                //指令信息                                
                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                {
                                    InsType = NewInsType._YD_Chd,
                                    SendCmd = _ChildIns,
                                    SendTime = sendTime,
                                    RecvData = recvChildData,
                                    RecvTime = recvTime
                                });
                                PnrObj.ChildYudingList.Add(_ChildIns + " Office:" + InputParam.Office, recvChildData);
                            }
                            if (!recvChildData.ToUpper().Contains("UNABLE") && !recvChildData.Contains("超时") && !recvChildData.Contains("无法从传输连接中读取数据") && !recvChildData.Contains("目标机器") && !recvChildData.Contains("服务器忙"))
                            {
                                //儿童编码
                                PnrObj.childPnr = pnrformat.GetPNR(recvChildData, out errMsg);

                                if (InputParam.IsGetSpecialPrice == 0)
                                {
                                    //预订儿童PNR状态信息   
                                    PnrObj.YDChd = pnrformat.GetYuDingInfo(recvChildData, out errMsg);
                                    sbLog.AppendFormat("时间:{0}\r\n解析出预订儿童PNR:{1}状态:{2}\r\n", System.DateTime.Now, PnrObj.childPnr, PnrObj.YDChd != null ? PnrObj.YDChd.YDPnrStatus : "");
                                }

                                #region 检测儿童编码是否生成
                                if (recvChildData.Contains("LEASE WAIT") && PnrObj.childPnr == "")
                                {
                                    //获取儿童编码超时
                                    PnrObj.GetChildPnrIsTimeOut = true;
                                    //检测编码生成功没有 
                                    string strPassengerName = ChildPasList[0].PassengerName.Trim();
                                    if (PinYingMange.IsChina(strPassengerName))
                                    {
                                        if (strPassengerName.EndsWith("CHD"))
                                        {
                                            strPassengerName = strPassengerName.Substring(0, strPassengerName.LastIndexOf("CHD"));
                                        }
                                    }
                                    else
                                    {
                                        if (strPassengerName.EndsWith(" CHD"))
                                        {
                                            strPassengerName = strPassengerName.Substring(0, strPassengerName.LastIndexOf(" CHD"));
                                        }
                                    }
                                    string m_PasPinYin = WebManage.GetPinYin(strPassengerName);
                                    m_PasPinYin = string.IsNullOrEmpty(m_PasPinYin) ? WebManage.GetPinYin(strPassengerName) : "";
                                    if (string.IsNullOrEmpty(m_PasPinYin))
                                    {
                                        m_PasPinYin = PinYingMange.GetSpellByChinese(strPassengerName);
                                    }
                                    string date = FormatPNR.DateToStr(InputParam.SkyList[0].FlyStartDate, DataFormat.dayMonth);
                                    string FlightNum = InputParam.SkyList[0].CarryCode.Replace("*", "") + InputParam.SkyList[0].FlightCode.Replace("*", "");
                                    string DisaplySeat = InputParam.SkyList[0].Space.ToUpper();
                                    _ChildIns = "RT" + m_PasPinYin + "/" + FlightNum + "/" + date;
                                    sbLog.AppendFormat("时间:{0}\r\n发送儿童检测编码指令:\r\n{1}\r\n", System.DateTime.Now, _ChildIns);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    pm.code = _ChildIns;
                                    pm.IsPn = false;
                                    //发送
                                    recvChildData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收儿童检测数据:\r\n{1}\r\n", System.DateTime.Now, recvChildData);
                                    //指令信息                                    
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._RT_Name,
                                        SendCmd = _ChildIns,
                                        SendTime = sendTime,
                                        RecvData = recvChildData,
                                        RecvTime = recvTime
                                    });
                                    //解析数据
                                    List<LEASEWAIT_Info> Waitlst = pnrformat.GetPnrList(recvChildData);
                                    string tempPnr = "";
                                    if (Waitlst.Count > 0)
                                    {
                                        foreach (LEASEWAIT_Info info in Waitlst)
                                        {
                                            //判断编码状态和人数 添加舱位
                                            if (info.PnrStatus.Contains("HK") && info.PasCount == ChildPasList.Count.ToString())
                                            {
                                                PnrObj.ChildPnr_LEASEWAIT.Add(info.PNR);
                                                //添加舱位比较 添加Office
                                                if (info.Seat == DisaplySeat && pm.Office.ToUpper() == info.Office)
                                                {
                                                    tempPnr = info.PNR;
                                                    #region 再次检测编码中是否存在该乘客名字(中文)
                                                    _ChildIns = "RT" + tempPnr;
                                                    sbLog.AppendFormat("时间:{0}\r\n再次检测儿童乘客名字指令:\r\n{1}\r\n", System.DateTime.Now, _ChildIns);
                                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    pm.code = _ChildIns;
                                                    pm.IsPn = false;
                                                    //发送
                                                    recvChildData = SendCommand(pm);
                                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n接收再次检测儿童乘客名字返回数据:\r\n{1}\r\n", System.DateTime.Now, recvChildData);
                                                    //指令信息                                                    
                                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                    {
                                                        InsType = NewInsType._RT,
                                                        SendCmd = _ChildIns,
                                                        SendTime = sendTime,
                                                        RecvData = recvChildData,
                                                        RecvTime = recvTime
                                                    });

                                                    string strInnerMsg = "";
                                                    //是否存在该乘客名字
                                                    bool IsExist = false;
                                                    //航段验证是否通过
                                                    bool skyExist = false;
                                                    PnrModel JCPnrModel = pnrformat.GetPNRInfo(tempPnr, recvChildData, false, out strInnerMsg);
                                                    if (JCPnrModel != null)
                                                    {
                                                        if (JCPnrModel._PassengerList.Count > 0)
                                                        {
                                                            foreach (PassengerInfo item in JCPnrModel._PassengerList)
                                                            {
                                                                if (item.PassengerName == ChildPasList[0].PassengerName.Replace(" ", ""))
                                                                {
                                                                    IsExist = true;
                                                                    break;
                                                                }
                                                            }
                                                        }

                                                        //乘客名字相同在验证航段信息
                                                        if (IsExist && JCPnrModel._LegList.Count > 0)
                                                        {
                                                            //排序
                                                            JCPnrModel._LegList.Sort(delegate(LegInfo sky1, LegInfo sky2)
                                                            {
                                                                DateTime d1 = DateTime.Parse(sky1.FlyDate1 + " " + sky1.FlyStartTime.Insert(2, ":") + ":00");
                                                                DateTime d2 = DateTime.Parse(sky2.FlyDate1 + " " + sky2.FlyStartTime.Insert(2, ":") + ":00");
                                                                return (DateTime.Compare(d1, d2));
                                                            });
                                                            //比较起飞日期
                                                            //比较城市
                                                            //比较航空公司   
                                                            for (int i = 0; i < JCPnrModel._LegList.Count; i++)
                                                            {
                                                                LegInfo leg = JCPnrModel._LegList[i];
                                                                ISkyLeg sky = InputParam.SkyList[i];
                                                                DateTime legFlyDate = DateTime.Parse(leg.FlyDate1 + " " + leg.FlyStartTime.Insert(2, ":") + ":00");
                                                                DateTime skyFlyDate = DateTime.Parse(sky.FlyStartDate + " " + sky.FlyStartTime.Insert(2, ":") + ":00");
                                                                if (legFlyDate == skyFlyDate
                                                                    && leg.FromCode.ToUpper() == sky.fromCode.ToUpper()
                                                                    && leg.ToCode.ToUpper() == sky.toCode.ToUpper()
                                                                    && leg.AirCode.ToUpper() == sky.CarryCode.ToUpper()
                                                                    )
                                                                {
                                                                    skyExist = true;
                                                                }
                                                                else
                                                                {
                                                                    skyExist = false;
                                                                    break;
                                                                }
                                                            }
                                                        }
                                                    }
                                                    if (IsExist && skyExist)
                                                    {
                                                        break;
                                                    }
                                                    else
                                                    {
                                                        tempPnr = "";
                                                    }
                                                    #endregion
                                                }
                                            }
                                        }
                                    }//end解析编码
                                    //赋值给实体                              
                                    if (!string.IsNullOrEmpty(tempPnr))
                                    {
                                        //儿童编码
                                        PnrObj.childPnr = tempPnr;
                                        if (PnrObj.YDChd != null)
                                        {
                                            PnrObj.YDChd.YDPnrStatus = "HK";
                                        }
                                        sbLog.AppendFormat("时间:{0}\r\n解析出儿童编码:{1}舱位比较得出编码PNR:{2}\r\n", System.DateTime.Now, string.Join("|", PnrObj.ChildPnr_LEASEWAIT.ToArray()), tempPnr);
                                    }
                                    else //直接取RT信息中的编码
                                    {
                                        string m_pnr = pnrformat.GetPNR(recvChildData, out errMsg);

                                        #region 检测PNR是对应所预订的编码信息
                                        //对比乘客姓名，人数 航段信息 避免串数据(返回的是RT数据)
                                        PnrModel tempPnrModel = pnrformat.GetPNRInfo(m_pnr, recvChildData, false, out errMsg);
                                        bool PnrIsCorrent = true;//检测编码是否对
                                        //检测乘客姓名是否一致
                                        if (tempPnrModel._PassengerList.Count > 0 && tempPnrModel._PassengerList.Count == ChildPasList.Count
                                            )
                                        {
                                            foreach (IPassenger item in ChildPasList)
                                            {
                                                if (!tempPnrModel._PassengerList.Exists(delegate(PassengerInfo p)
                                                {
                                                    return p.PassengerName == item.PassengerName.Replace(" ", "");
                                                }))
                                                {
                                                    PnrIsCorrent = false;
                                                    break;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            PnrIsCorrent = false;
                                        }

                                        //检测Office是否一致
                                        if (PnrIsCorrent && pm.Office.ToUpper() != tempPnrModel._Office.ToUpper())
                                        {
                                            PnrIsCorrent = false;
                                        }
                                        if (PnrIsCorrent)
                                        {
                                            //排序
                                            tempPnrModel._LegList.Sort(delegate(LegInfo sky1, LegInfo sky2)
                                            {
                                                DateTime d1 = DateTime.Parse(sky1.FlyDate1 + " " + sky1.FlyStartTime.Insert(2, ":") + ":00");
                                                DateTime d2 = DateTime.Parse(sky2.FlyDate1 + " " + sky2.FlyStartTime.Insert(2, ":") + ":00");
                                                return (DateTime.Compare(d1, d2));
                                            });
                                            //比较起飞日期
                                            //比较城市
                                            //比较航空公司   
                                            for (int i = 0; i < tempPnrModel._LegList.Count; i++)
                                            {
                                                LegInfo leg = tempPnrModel._LegList[i];
                                                ISkyLeg sky = InputParam.SkyList[i];
                                                DateTime legFlyDate = DateTime.Parse(leg.FlyDate1 + " " + leg.FlyStartTime.Insert(2, ":") + ":00");
                                                DateTime skyFlyDate = DateTime.Parse(sky.FlyStartDate + " " + sky.FlyStartTime.Insert(2, ":") + ":00");
                                                if (!(legFlyDate == skyFlyDate
                                                       && leg.FromCode.ToUpper() == sky.fromCode.ToUpper()
                                                       && leg.ToCode.ToUpper() == sky.toCode.ToUpper()
                                                       && leg.AirCode.ToUpper() == sky.CarryCode.ToUpper()
                                                       && leg.Seat.ToUpper() == sky.Space.ToUpper()
                                                       && leg.AirCodeFlightNum.ToUpper() == (sky.CarryCode.ToUpper() + sky.FlightCode)
                                                       ))
                                                {
                                                    PnrIsCorrent = false;
                                                    break;
                                                }
                                            }
                                        }
                                        #endregion

                                        if (m_pnr != "" && recvChildData.Trim() != "" && m_pnr.Trim().Length == 6 && PnrIsCorrent)
                                        {
                                            //儿童编码
                                            PnrObj.childPnr = m_pnr;
                                            sbLog.AppendFormat("时间:{0}\r\n解析出儿童检测编码PNR:{1}\r\n", System.DateTime.Now, PnrObj.childPnr);
                                        }
                                    }
                                }
                                #endregion


                                if (!string.IsNullOrEmpty(PnrObj.childPnr) && PnrObj.childPnr.Trim().Length == 6)
                                {
                                    #region HU特殊舱位 添加备注指令

                                    if (InputParam.CarryCode.ToUpper().Trim() == "HU")
                                    {
                                        string str_Space = "", addIns = "";
                                        foreach (var item in InputParam.SkyList)
                                        {
                                            str_Space += item.Space.Trim().ToUpper() + "/";
                                        }
                                        if (str_Space.Contains("A") || str_Space.Contains("D"))
                                        {
                                            addIns = "OSI HU CKIN SSAC/S1";
                                        }
                                        else if (str_Space.Contains("I"))
                                        {
                                            addIns = "OSI HU CKIN TQGP/S1";
                                        }
                                        else if (str_Space.Contains("W"))
                                        {
                                            addIns = "OSI HU CKIN HJSF/S1";
                                        }
                                        string rmkCmd = string.Format("RT{0}|{1}^\\", PnrObj.childPnr.Trim(), addIns);
                                        if (!string.IsNullOrEmpty(addIns))
                                        {
                                            if (InputParam.UsePIDChannel == 2)
                                            {
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\n{1}\r\n", System.DateTime.Now, rmkCmd);
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                pm.code = rmkCmd;
                                                pm.IsPn = false;
                                                //发送
                                                string recvData = SendCommand(pm);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvData);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._HU_SpRmk,
                                                    SendCmd = rmkCmd,
                                                    SendTime = sendTime,
                                                    RecvData = recvData,
                                                    RecvTime = recvTime
                                                });
                                            }
                                            else
                                            {
                                                rmkCmd = string.Format("RT{0}|{1}|@", PnrObj.childPnr.Trim(), addIns);
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, rmkCmd + Office);
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                string recvData = sendec.SendData(rmkCmd, out errMsg);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._HU_SpRmk,
                                                    SendCmd = rmkCmd,
                                                    SendTime = sendTime,
                                                    RecvData = recvData,
                                                    RecvTime = recvTime
                                                });
                                                PnrObj.AdultYudingList.Add(AdultIns + " Office:" + InputParam.Office, recvData);
                                            }
                                        }
                                    }

                                    #endregion

                                    //没有预订成人编码直接取输入的成人编码
                                    if (string.IsNullOrEmpty(PnrObj.AdultPnr) && !string.IsNullOrEmpty(InputParam.AdultPnr) && InputParam.AdultPnr.Trim().Length == 6)
                                    {
                                        PnrObj.AdultPnr = InputParam.AdultPnr.Trim();
                                    }
                                    sbLog.AppendFormat("时间:{0}\r\n解析儿童PNR:\r\n{1}\r\n", System.DateTime.Now, PnrObj.childPnr);
                                    //有成人编码时备注
                                    if (!string.IsNullOrEmpty(PnrObj.AdultPnr) && PnrObj.AdultPnr.Trim().Length == 6)
                                    {
                                        RemarkInfo remark = new RemarkInfo();
                                        //儿童编码备注成人编码信息
                                        if (InputParam.UsePIDChannel == 2)
                                        {
                                            string rmkIns = string.Format("RT{0}|SSR OTHS {1} ADULT PNR IS {2}|@", PnrObj.childPnr.ToUpper(), InputParam.CarryCode, PnrObj.AdultPnr.ToUpper());
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\n{1}\r\n", System.DateTime.Now, rmkIns);
                                            //发送
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            remark.RemarkCmd = rmkIns;
                                            pm.code = rmkIns;
                                            pm.IsPn = false;
                                            string recvrmk = SendCommand(pm);
                                            remark.RemarkRecvData = recvrmk;
                                            remark.PassengerType = "2";
                                            remark.RemarkType = 1;
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvrmk);
                                            //指令信息                                            
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._RT_ChdToAdult,
                                                SendCmd = rmkIns,
                                                SendTime = sendTime,
                                                RecvData = recvrmk,
                                                RecvTime = recvTime
                                            });

                                            if (ISReSendData(recvChildData))
                                            {
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\n{1}\r\n", System.DateTime.Now, rmkIns);
                                                //发送
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                recvrmk = SendCommand(pm);
                                                remark.RemarkRecvData = recvrmk;
                                                remark.PassengerType = "2";
                                                remark.RemarkType = 1;
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvrmk);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._RT_ChdToAdult,
                                                    SendCmd = rmkIns,
                                                    SendTime = sendTime,
                                                    RecvData = recvrmk,
                                                    RecvTime = recvTime
                                                });
                                            }
                                            //EU航空单独处理备注信息

                                            //添加备注
                                            PnrObj.RemarkList.Add(remark);
                                        }
                                        else
                                        {
                                            string rmkIns = string.Format("IG|RT{0}|SSR OTHS {1} ADULT PNR IS {2}|@{3}#1", PnrObj.childPnr.ToUpper(), InputParam.CarryCode, PnrObj.AdultPnr.ToUpper(), Office);
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, rmkIns);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            string recvrmk = sendec.SendData(rmkIns, out errMsg);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, recvrmk);

                                            //指令信息                                            
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._RT_ChdToAdult,
                                                SendCmd = rmkIns,
                                                SendTime = sendTime,
                                                RecvData = recvrmk,
                                                RecvTime = recvTime
                                            });
                                        }
                                    }
                                    if (InputParam.IsGetSpecialPrice == 0)
                                    {
                                        #region //获取儿童RT数据
                                        string sendRT = "", RTRecvData = "";
                                        if (InputParam.UsePIDChannel == 2)
                                        {
                                            sendRT = string.Format("RT{0}", PnrObj.childPnr);
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                            pm.code = sendRT;
                                            pm.IsPn = true;
                                            //发送
                                            RTRecvData = SendCommand(pm);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                            //指令信息                                            
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._RT,
                                                SendCmd = sendRT,
                                                SendTime = sendTime,
                                                RecvData = RTRecvData,
                                                RecvTime = recvTime
                                            });
                                            if (ISReSendData(RTRecvData))
                                            {
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                                pm.code = sendRT;
                                                pm.IsPn = true;
                                                //发送
                                                RTRecvData = SendCommand(pm);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._RT,
                                                    SendCmd = sendRT,
                                                    SendTime = sendTime,
                                                    RecvData = RTRecvData,
                                                    RecvTime = recvTime
                                                });
                                            }

                                            //儿童编码对应的大编码
                                            PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(RTRecvData, out errMsg);
                                            //获取儿童编码对应的大编码
                                            if (PnrObj.childPnrToBigPNR == "" || PnrObj.childPnrToBigPNR.Length != 6)
                                            {
                                                string sendChildRTR = string.Format("rt{0}|RTR", PnrObj.childPnr);
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendChildRTR);
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                                pm.code = sendChildRTR;
                                                pm.IsPn = true;
                                                //发送
                                                string ChildRTRRecvData = SendCommand(pm);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, ChildRTRRecvData);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._RT_RTR,
                                                    SendCmd = sendChildRTR,
                                                    SendTime = sendTime,
                                                    RecvData = ChildRTRRecvData,
                                                    RecvTime = recvTime
                                                });
                                                //儿童编码对应的大编码
                                                PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(ChildRTRRecvData, out errMsg);
                                                PnrObj.CHDRTRContent = pnrformat.DelRepeatRTCon(ChildRTRRecvData, ref pnrRemark);
                                                if (ISReSendData(ChildRTRRecvData))
                                                {
                                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendChildRTR);
                                                    pm.code = sendChildRTR;
                                                    pm.IsPn = true;
                                                    //发送
                                                    ChildRTRRecvData = SendCommand(pm);
                                                    //if (ChildRTRRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS"))
                                                    //{
                                                    //    ChildRTRRecvData = SendCommand(pm);
                                                    //}
                                                    PnrObj.CHDRTRContent = pnrformat.DelRepeatRTCon(ChildRTRRecvData, ref pnrRemark);
                                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, ChildRTRRecvData);
                                                    //指令信息                                                    
                                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                    {
                                                        InsType = NewInsType._RT_RTR,
                                                        SendCmd = sendChildRTR,
                                                        SendTime = sendTime,
                                                        RecvData = ChildRTRRecvData,
                                                        RecvTime = recvTime
                                                    });
                                                    //儿童编码对应的大编码
                                                    PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(ChildRTRRecvData, out errMsg);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            sendRT = string.Format("IG|(eas)rt{0}{1}#1", PnrObj.childPnr, Office);
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            RTRecvData = sendec.SendData(sendRT, out errMsg);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                            //指令信息                                            
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._RT,
                                                SendCmd = sendRT,
                                                SendTime = sendTime,
                                                RecvData = RTRecvData,
                                                RecvTime = recvTime
                                            });

                                            if (ISReSendData(RTRecvData))
                                            {
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                                RTRecvData = sendec.SendData(sendRT, out errMsg);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._RT,
                                                    SendCmd = sendRT,
                                                    SendTime = sendTime,
                                                    RecvData = RTRecvData,
                                                    RecvTime = recvTime
                                                });
                                            }
                                            //儿童编码对应的大编码
                                            PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(RTRecvData, out errMsg);
                                            //获取儿童编码对应的大编码
                                            if (PnrObj.childPnrToBigPNR == "" || PnrObj.childPnrToBigPNR.Length != 6)
                                            {
                                                string sendRTR = string.Format("IG|(eas)rt{0}|RTR{1}#1", PnrObj.childPnr, Office);
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                string RTRRecvData = sendec.SendData(sendRTR, out errMsg);
                                                PnrObj.CHDRTRContent = pnrformat.DelRepeatRTCon(RTRRecvData, ref pnrRemark);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRRecvData);

                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._RT_RTR,
                                                    SendCmd = sendRT,
                                                    SendTime = sendTime,
                                                    RecvData = RTRRecvData,
                                                    RecvTime = recvTime
                                                });

                                                if (ISReSendData(RTRRecvData))
                                                {
                                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                                    RTRRecvData = sendec.SendData(sendRTR, out errMsg);
                                                    PnrObj.CHDRTRContent = pnrformat.DelRepeatRTCon(RTRRecvData, ref pnrRemark);
                                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRRecvData);
                                                    //指令信息                                                    
                                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                    {
                                                        InsType = NewInsType._RT_RTR,
                                                        SendCmd = sendRTR,
                                                        SendTime = sendTime,
                                                        RecvData = RTRRecvData,
                                                        RecvTime = recvTime
                                                    });
                                                }
                                                //儿童编码对应的大编码
                                                PnrObj.childPnrToBigPNR = pnrformat.GetBigCode(RTRRecvData, out errMsg);
                                            }
                                        }
                                        //儿童RT数据
                                        PnrObj.childPnrRTContent = RTRecvData;

                                        #endregion
                                    }

                                    #region //获取儿童Pat数据
                                    if (InputParam.IsGdNotPAT != 1)//固定特价不用PAT
                                    {
                                        string sendPat = "", PATRecvData = "";
                                        if (InputParam.UsePIDChannel == 2)
                                        {
                                            sendPat = string.Format("rt{0}|pat:a*ch", PnrObj.childPnr);
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);

                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                            pm.code = sendPat;
                                            pm.IsPn = false;
                                            //发送
                                            PATRecvData = SendCommand(pm);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                            //指令信息                                            
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._PAT_A_CH,
                                                SendCmd = sendPat,
                                                SendTime = sendTime,
                                                RecvData = PATRecvData,
                                                RecvTime = recvTime
                                            });
                                            if (ISReSendData(PATRecvData) || !PATRecvData.Contains(">PAT:A"))
                                            {
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                                pm.code = sendPat;
                                                pm.IsPn = false;
                                                //发送
                                                PATRecvData = SendCommand(pm);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._PAT_A_CH,
                                                    SendCmd = sendPat,
                                                    SendTime = sendTime,
                                                    RecvData = PATRecvData,
                                                    RecvTime = recvTime
                                                });
                                            }


                                            //是否儿童出成人票
                                            if (InputParam.ChildIsAdultEtdz == "1")
                                            {
                                                //赋值儿童pat内容
                                                PnrObj.CHDToAdultPatCon = PATRecvData;

                                                //儿童出成人票 成人PAT内容获取
                                                sendPat = string.Format("rt{0}|pat:a", PnrObj.childPnr);
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令【儿童出成人票】:\r\n{1}\r\n", System.DateTime.Now, sendPat);

                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                                pm.code = sendPat;
                                                pm.IsPn = false;
                                                //发送
                                                PATRecvData = SendCommand(pm);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._PAT_A_CH,
                                                    SendCmd = sendPat,
                                                    SendTime = sendTime,
                                                    RecvData = PATRecvData,
                                                    RecvTime = recvTime
                                                });
                                                if (ISReSendData(PATRecvData))
                                                {
                                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                                    pm.code = sendPat;
                                                    pm.IsPn = false;
                                                    //发送
                                                    PATRecvData = SendCommand(pm);
                                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                                    //指令信息                                                    
                                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                    {
                                                        InsType = NewInsType._PAT_A_CH,
                                                        SendCmd = sendPat,
                                                        SendTime = sendTime,
                                                        RecvData = PATRecvData,
                                                        RecvTime = recvTime
                                                    });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            sendPat = string.Format("IG|rt{0}|pat:a*ch{1}#1", PnrObj.childPnr, Office);
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            PATRecvData = sendec.SendData(sendPat, out errMsg);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                            //指令信息                                            
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._PAT_A_CH,
                                                SendCmd = sendPat,
                                                SendTime = sendTime,
                                                RecvData = PATRecvData,
                                                RecvTime = recvTime
                                            });

                                            if (ISReSendData(PATRecvData))
                                            {
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                PATRecvData = sendec.SendData(sendPat, out errMsg);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._PAT_A_CH,
                                                    SendCmd = sendPat,
                                                    SendTime = sendTime,
                                                    RecvData = PATRecvData,
                                                    RecvTime = recvTime
                                                });
                                            }
                                            //是否儿童出成人票
                                            if (InputParam.ChildIsAdultEtdz == "1")
                                            {
                                                //赋值儿童pat内容
                                                PnrObj.CHDToAdultPatCon = PATRecvData;

                                                sendPat = string.Format("IG|rt{0}|pat:a{1}#1", PnrObj.childPnr, Office);
                                                sbLog.AppendFormat("时间:{0}\r\n发送指令【儿童出成人票】:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                PATRecvData = sendec.SendData(sendPat, out errMsg);
                                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                                //指令信息                                                
                                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                {
                                                    InsType = NewInsType._PAT_A_CH,
                                                    SendCmd = sendPat,
                                                    SendTime = sendTime,
                                                    RecvData = PATRecvData,
                                                    RecvTime = recvTime
                                                });
                                                if (ISReSendData(PATRecvData))
                                                {
                                                    sbLog.AppendFormat("时间:{0}\r\n发送指令【儿童出成人票】:\r\n{1}\r\n", System.DateTime.Now, sendPat);
                                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    PATRecvData = sendec.SendData(sendPat, out errMsg);
                                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, PATRecvData);
                                                    //指令信息                                                    
                                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                    {
                                                        InsType = NewInsType._PAT_A_CH,
                                                        SendCmd = sendPat,
                                                        SendTime = sendTime,
                                                        RecvData = PATRecvData,
                                                        RecvTime = recvTime
                                                    });
                                                }
                                            }
                                        }
                                        PnrObj.PatList[1] = PATRecvData;
                                    }//PAT  CH

                                    #endregion
                                }
                                else
                                {
                                    //儿童编码生成失败
                                    PnrObj.childPnr = "未能解析出儿童编码！";
                                    sbLog.AppendFormat("时间:{0}\r\n未能解析出儿童编码\r\n", System.DateTime.Now);
                                }
                            }
                            else
                            {
                                //儿童编码生成失败
                                PnrObj.childPnr = "儿童编码生成失败！";
                                sbLog.AppendFormat("时间:{0}\r\n儿童编码生成失败\r\n", System.DateTime.Now);
                            }
                        }
                        else
                        {
                            sbLog.AppendFormat("时间:{0}\r\n儿童起飞时间已小于预定时间,不预定！\r\n", System.DateTime.Now);
                        }
                    }

                    #endregion

                    #region 婴儿
                    if (!InputParam.FlyTimeIsOverCurrTime)
                    {
                        if (!string.IsNullOrEmpty(PnrObj.AdultPnr) && PnrObj.AdultPnr.Trim().Length == 6)
                        {
                            //婴儿
                            if (YingerPasList != null && YingerPasList.Count > 0)
                            {
                                string RTAdult = "", RTContent = "";
                                if (InputParam.UsePIDChannel == 2)
                                {
                                    RTAdult = string.Format("RT{0}", PnrObj.AdultPnr);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, RTAdult);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    pm.code = RTAdult;
                                    pm.IsPn = true;
                                    //发送
                                    RTContent = SendCommand(pm);
                                    if (ISReSendData(RTContent))
                                    {
                                        RTContent = SendCommand(pm);
                                    }
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTContent);
                                    //指令信息                                
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._RT,
                                        SendCmd = RTAdult,
                                        SendTime = sendTime,
                                        RecvData = RTContent,
                                        RecvTime = recvTime
                                    });
                                }
                                else
                                {
                                    RTAdult = string.Format("IG|(eas)RT{0}{1}#1", PnrObj.AdultPnr, Office);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, RTAdult);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    RTContent = sendec.SendData(RTAdult, out errMsg);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTContent);

                                    //指令信息                                
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._RT,
                                        SendCmd = RTAdult,
                                        SendTime = sendTime,
                                        RecvData = RTContent,
                                        RecvTime = recvTime
                                    });
                                }
                                //解析
                                PnrModel PnrModel = pnrformat.GetPNRInfo(PnrObj.AdultPnr, RTContent, false, out errMsg);
                                if (PnrModel != null && PnrModel._LegList.Count > 0)
                                {
                                    string pinyin = "";
                                    string YIns = "";//婴儿备注指令
                                    //循环婴儿
                                    foreach (IPassenger IYingerPas in YingerPasList)
                                    {
                                        if (PnrModel._PassengerList.Count > 0)
                                        {
                                            //循环成人
                                            for (int i = 0; i < PnrModel._PassengerList.Count; i++)
                                            {
                                                PassengerInfo _AdultPas = PnrModel._PassengerList[i];
                                                //循环航段
                                                for (int j = 0; j < InputParam.SkyList.Count; j++)
                                                {
                                                    ISkyLeg skyLeg = InputParam.SkyList[j];
                                                    //解析出来的航段序号需要进行备注的航段
                                                    string SkyNum = PnrModel._LegList.Count > j ? PnrModel._LegList[j].SerialNum : PnrModel._LegList[0].SerialNum;
                                                    //成人编码备注婴儿信息
                                                    StringBuilder sbYingerRMK = new StringBuilder();
                                                    if (PinYingMange.IsChina(IYingerPas.PassengerName))
                                                    {
                                                        pinyin = PinYingMange.GetSpellByChinese(IYingerPas.PassengerName.Substring(0, 1)) + "/" + PinYingMange.GetSpellByChinese(PinYingMange.RepacePinyinChar(IYingerPas.PassengerName.Substring(1)));
                                                    }
                                                    else
                                                    {
                                                        pinyin = IYingerPas.PassengerName;
                                                    }
                                                    RemarkInfo remark = new RemarkInfo();
                                                    if (InputParam.UsePIDChannel == 2)
                                                    {
                                                        if (j == 0)
                                                        {
                                                            sbYingerRMK.AppendFormat("RT{0}|XN:IN/{1}INF({2})/p{3}\r", PnrObj.AdultPnr, IYingerPas.PassengerName, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.monthYear), _AdultPas.SerialNum);
                                                            sbYingerRMK.AppendFormat("SSR INFT {0} NN1 {1} {2} {3} {4} {5} {6}/P{7}/S{8}\r", InputParam.CarryCode, skyLeg.fromCode + skyLeg.toCode, skyLeg.CarryCode.Replace("*", "") + skyLeg.FlightCode, skyLeg.Space.Substring(0, 1), FormatPNR.DateToStr(skyLeg.FlyStartDate, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.dayMonthYear), _AdultPas.SerialNum, SkyNum);
                                                        }
                                                        else
                                                        {
                                                            sbYingerRMK.AppendFormat("RT{0}|SSR INFT {1} NN1 {2} {3} {4} {5} {6} {7}/P{8}/S{9}\r", PnrObj.AdultPnr, InputParam.CarryCode, skyLeg.fromCode + skyLeg.toCode, skyLeg.CarryCode.Replace("*", "") + skyLeg.FlightCode, skyLeg.Space.Substring(0, 1), FormatPNR.DateToStr(skyLeg.FlyStartDate, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.dayMonthYear), _AdultPas.SerialNum, SkyNum);
                                                        }
                                                        sbYingerRMK.Append("@");
                                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sbYingerRMK.ToString());
                                                        YIns = sbYingerRMK.ToString().Replace("\r", "^");
                                                        //发送备注
                                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                        //备注指令
                                                        remark.RemarkCmd = YIns;
                                                        pm.code = YIns;
                                                        pm.IsPn = false;
                                                        //发送
                                                        string yingerRMKRecv = SendCommand(pm);
                                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, yingerRMKRecv);
                                                        //指令信息                                                
                                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                        {
                                                            InsType = NewInsType._RT_XN_INF,
                                                            SendCmd = YIns,
                                                            SendTime = sendTime,
                                                            RecvData = yingerRMKRecv,
                                                            RecvTime = recvTime
                                                        });
                                                        remark.RemarkRecvData = yingerRMKRecv;
                                                        remark.PassengerType = PnrModel._PassengerList[i].PassengerType;
                                                        remark.RemarkType = 0;

                                                        //检测备注是否成功
                                                        if (!pnrformat.INFMarkIsOK(yingerRMKRecv, out errMsg))
                                                        {
                                                            //有可能没有备注进去 在发一次
                                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sbYingerRMK.ToString());
                                                            //强制封口
                                                            YIns = YIns.Replace("@", @"\ki");
                                                            remark.RemarkCmd = YIns;
                                                            pm.code = YIns;
                                                            pm.IsPn = false;
                                                            //发送
                                                            yingerRMKRecv = SendCommand(pm);
                                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, yingerRMKRecv);
                                                            //指令信息                                                    
                                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                            {
                                                                InsType = NewInsType._RT_XN_INF,
                                                                SendCmd = YIns,
                                                                SendTime = sendTime,
                                                                RecvData = yingerRMKRecv,
                                                                RecvTime = recvTime
                                                            });
                                                            remark.RemarkRecvData = yingerRMKRecv;
                                                        }
                                                        PnrObj.RemarkList.Add(remark);
                                                    }
                                                    else
                                                    {
                                                        if (j == 0)
                                                        {
                                                            sbYingerRMK.AppendFormat("IG|RT{0}|XN:IN/{1}INF({2})/p{3}\r\n", PnrObj.AdultPnr, IYingerPas.PassengerName, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.monthYear), _AdultPas.SerialNum);
                                                            sbYingerRMK.AppendFormat("SSR INFT {0} NN1 {1} {2} {3} {4} {5} {6}/P{7}/S{8}\r\n", InputParam.CarryCode, skyLeg.fromCode + skyLeg.toCode, skyLeg.CarryCode.Replace("*", "") + skyLeg.FlightCode, skyLeg.Space.Substring(0, 1), FormatPNR.DateToStr(skyLeg.FlyStartDate, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.dayMonthYear), _AdultPas.SerialNum, SkyNum);
                                                        }
                                                        else
                                                        {
                                                            sbYingerRMK.AppendFormat("IG|RT{0}|SSR INFT {1} NN1 {2} {3} {4} {5} {6} {7}/P{8}/S{9}\r\n", PnrObj.AdultPnr, InputParam.CarryCode, skyLeg.fromCode + skyLeg.toCode, skyLeg.CarryCode.Replace("*", "") + skyLeg.FlightCode, skyLeg.Space.Substring(0, 1), FormatPNR.DateToStr(skyLeg.FlyStartDate, DataFormat.dayMonth), pinyin, FormatPNR.DateToStr(IYingerPas.PasSsrCardID, DataFormat.dayMonthYear), _AdultPas.SerialNum, SkyNum);
                                                        }
                                                        sbYingerRMK.AppendFormat("@{0}#1", Office);
                                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sbYingerRMK.ToString());
                                                        YIns = sbYingerRMK.ToString();
                                                        //发送备注
                                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                        string yingerRMKRecv = sendec.SendData(YIns, out errMsg);
                                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                                        //指令信息                                                
                                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                        {
                                                            InsType = NewInsType._RT_XN_INF,
                                                            SendCmd = YIns,
                                                            SendTime = sendTime,
                                                            RecvData = yingerRMKRecv,
                                                            RecvTime = recvTime
                                                        });

                                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, yingerRMKRecv);
                                                        //检测备注是否成功
                                                        if (!pnrformat.INFMarkIsOK(yingerRMKRecv, out errMsg))
                                                        {
                                                            //有可能没有备注进去 在发一次
                                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sbYingerRMK.ToString());
                                                            //强制封口
                                                            YIns = YIns.Replace("@", @"\ki");
                                                            yingerRMKRecv = sendec.SendData(YIns, out errMsg);
                                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, yingerRMKRecv);
                                                            //指令信息                                                    
                                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                                            {
                                                                InsType = NewInsType._RT_XN_INF,
                                                                SendCmd = YIns,
                                                                SendTime = sendTime,
                                                                RecvData = yingerRMKRecv,
                                                                RecvTime = recvTime
                                                            });
                                                        }
                                                    }
                                                }//end循环航段
                                                //移除该成人
                                                PnrModel._PassengerList.Remove(_AdultPas);
                                                break;
                                            }//end循环成人
                                        }
                                    }
                                }
                                else
                                {
                                    sbLog.AppendFormat("数据解析失败 时间:{0}\r\n编码{1}数据:\r\n{2}\r\n", System.DateTime.Now, PnrObj.AdultPnr, RTContent);
                                }
                                //获取婴儿Pat内容    
                                string sendYinerPat = "", YinerPATRecvData = "";
                                if (InputParam.UsePIDChannel == 2)
                                {
                                    sendYinerPat = string.Format("rt{0}|pat:a*in", PnrObj.AdultPnr);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendYinerPat);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    pm.code = sendYinerPat;
                                    pm.IsPn = false;
                                    //发送
                                    YinerPATRecvData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, YinerPATRecvData);
                                    //指令信息                                
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._PAT_A_IN,
                                        SendCmd = sendYinerPat,
                                        SendTime = sendTime,
                                        RecvData = YinerPATRecvData,
                                        RecvTime = recvTime
                                    });
                                    if (ISReSendData(YinerPATRecvData) || !YinerPATRecvData.Contains(">PAT:A"))
                                    {
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendYinerPat);
                                        pm.code = sendYinerPat;
                                        pm.IsPn = false;
                                        //发送
                                        YinerPATRecvData = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, YinerPATRecvData);
                                        //指令信息                                    
                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                        {
                                            InsType = NewInsType._PAT_A_IN,
                                            SendCmd = sendYinerPat,
                                            SendTime = sendTime,
                                            RecvData = YinerPATRecvData,
                                            RecvTime = recvTime
                                        });
                                    }
                                }
                                else
                                {
                                    sendYinerPat = string.Format("IG|rt{0}|pat:a*in{1}#1", PnrObj.AdultPnr, Office);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendYinerPat);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    YinerPATRecvData = sendec.SendData(sendYinerPat, out errMsg);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, YinerPATRecvData);
                                    //指令信息                                
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._PAT_A_IN,
                                        SendCmd = sendYinerPat,
                                        SendTime = sendTime,
                                        RecvData = YinerPATRecvData,
                                        RecvTime = recvTime
                                    });
                                    if (ISReSendData(YinerPATRecvData))
                                    {
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendYinerPat);
                                        YinerPATRecvData = sendec.SendData(sendYinerPat, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, YinerPATRecvData);
                                        //指令信息                                    
                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                        {
                                            InsType = NewInsType._PAT_A_IN,
                                            SendCmd = sendYinerPat,
                                            SendTime = sendTime,
                                            RecvData = YinerPATRecvData,
                                            RecvTime = recvTime
                                        });
                                    }
                                }
                                PnrObj.PatList[2] = YinerPATRecvData;
                            }
                            if (InputParam.IsGetSpecialPrice == 0)
                            {
                                //获取成人RT内容   
                                string sendAdultRT = "", AdultRTRecvData = "";
                                if (InputParam.UsePIDChannel == 2)
                                {
                                    sendAdultRT = string.Format("rt{0}", PnrObj.AdultPnr);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRT);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    pm.code = sendAdultRT;
                                    pm.IsPn = true;
                                    //发送
                                    AdultRTRecvData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRecvData);
                                    //指令信息                                
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._RT,
                                        SendCmd = sendAdultRT,
                                        SendTime = sendTime,
                                        RecvData = AdultRTRecvData,
                                        RecvTime = recvTime
                                    });
                                    if (ISReSendData(AdultRTRecvData))
                                    {
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRT);
                                        pm.code = sendAdultRT;
                                        pm.IsPn = true;
                                        //发送
                                        AdultRTRecvData = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRecvData);
                                        //指令信息                                    
                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                        {
                                            InsType = NewInsType._RT,
                                            SendCmd = sendAdultRT,
                                            SendTime = sendTime,
                                            RecvData = AdultRTRecvData,
                                            RecvTime = recvTime
                                        });
                                    }

                                    //成人编码对应的大编码
                                    PnrObj.AdultPnrToBigPNR = pnrformat.GetBigCode(AdultRTRecvData, out errMsg);
                                    //获取成人编码对应的大编码
                                    if (PnrObj.AdultPnrToBigPNR == "" || PnrObj.AdultPnrToBigPNR.Length != 6)
                                    {
                                        string sendAdultRTR = string.Format("rt{0}|RTR", PnrObj.AdultPnr);
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRTR);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                        pm.code = sendAdultRTR;
                                        pm.IsPn = true;
                                        //发送
                                        string AdultRTRRecvData = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRRecvData);
                                        //指令信息                                    
                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                        {
                                            InsType = NewInsType._RT_RTR,
                                            SendCmd = sendAdultRTR,
                                            SendTime = sendTime,
                                            RecvData = AdultRTRRecvData,
                                            RecvTime = recvTime
                                        });
                                        PnrObj.AdultRTRContent = pnrformat.DelRepeatRTCon(AdultRTRRecvData, ref pnrRemark);
                                        if (ISReSendData(AdultRTRRecvData))
                                        {
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRTR);
                                            pm.code = sendAdultRTR;
                                            pm.IsPn = true;
                                            //发送
                                            AdultRTRRecvData = SendCommand(pm);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRRecvData);
                                            //指令信息                                        
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._RT_RTR,
                                                SendCmd = sendAdultRTR,
                                                SendTime = sendTime,
                                                RecvData = AdultRTRRecvData,
                                                RecvTime = recvTime
                                            });
                                            PnrObj.AdultRTRContent = pnrformat.DelRepeatRTCon(AdultRTRRecvData, ref pnrRemark);
                                        }
                                        //成人编码对应的大编码
                                        PnrObj.AdultPnrToBigPNR = pnrformat.GetBigCode(AdultRTRRecvData, out errMsg);
                                    }
                                }
                                else
                                {
                                    sendAdultRT = string.Format("IG|(eas)rt{0}{1}#1", PnrObj.AdultPnr, Office);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRT);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    AdultRTRecvData = sendec.SendData(sendAdultRT, out errMsg);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRecvData);
                                    //指令信息                                
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._RT,
                                        SendCmd = sendAdultRT,
                                        SendTime = sendTime,
                                        RecvData = AdultRTRecvData,
                                        RecvTime = recvTime
                                    });
                                    if (ISReSendData(AdultRTRecvData))
                                    {
                                        sendAdultRT = string.Format("IG|(eas)rt{0}{1}#1", PnrObj.AdultPnr, Office);
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRT);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        AdultRTRecvData = sendec.SendData(sendAdultRT, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRecvData);
                                        //指令信息                                    
                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                        {
                                            InsType = NewInsType._RT,
                                            SendCmd = sendAdultRT,
                                            SendTime = sendTime,
                                            RecvData = AdultRTRecvData,
                                            RecvTime = recvTime
                                        });
                                    }

                                    //成人编码对应的大编码
                                    PnrObj.AdultPnrToBigPNR = pnrformat.GetBigCode(AdultRTRecvData, out errMsg);
                                    //获取成人编码对应的大编码
                                    if (PnrObj.AdultPnrToBigPNR == "" || PnrObj.AdultPnrToBigPNR.Length != 6)
                                    {
                                        string sendAdultRTR = string.Format("IG|(eas)rt{0}|RTR{1}#1", PnrObj.AdultPnr, Office);
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRTR);
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        string AdultRTRRecvData = sendec.SendData(sendAdultRTR, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRRecvData);
                                        //指令信息                                    
                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                        {
                                            InsType = NewInsType._RT,
                                            SendCmd = sendAdultRTR,
                                            SendTime = sendTime,
                                            RecvData = AdultRTRRecvData,
                                            RecvTime = recvTime
                                        });
                                        PnrObj.AdultRTRContent = pnrformat.DelRepeatRTCon(AdultRTRRecvData, ref pnrRemark);
                                        if (ISReSendData(AdultRTRRecvData))
                                        {
                                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultRTR);
                                            AdultRTRRecvData = sendec.SendData(sendAdultRTR, out errMsg);
                                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultRTRRecvData);
                                            //指令信息                                        
                                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                            {
                                                InsType = NewInsType._RT,
                                                SendCmd = sendAdultRTR,
                                                SendTime = sendTime,
                                                RecvData = AdultRTRRecvData,
                                                RecvTime = recvTime
                                            });
                                            PnrObj.AdultRTRContent = pnrformat.DelRepeatRTCon(AdultRTRRecvData, ref pnrRemark);
                                        }
                                        //成人编码对应的大编码
                                        PnrObj.AdultPnrToBigPNR = pnrformat.GetBigCode(AdultRTRRecvData, out errMsg);
                                    }
                                }
                                //成人RT数据
                                PnrObj.AdultPnrRTContent = AdultRTRecvData;
                            }

                            if (InputParam.IsGdNotPAT != 1)//固定特价时不PAT
                            {
                                //获取成人Pat数据
                                string sendAdultPat = "", AdultPATRecvData = "";
                                if (InputParam.UsePIDChannel == 2)
                                {
                                    sendAdultPat = string.Format("rt{0}|pat:a", PnrObj.AdultPnr);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultPat);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                                    pm.code = sendAdultPat;
                                    pm.IsPn = false;
                                    //发送
                                    AdultPATRecvData = SendCommand(pm);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultPATRecvData);
                                    //指令信息                                
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._PAT_A,
                                        SendCmd = sendAdultPat,
                                        SendTime = sendTime,
                                        RecvData = AdultPATRecvData,
                                        RecvTime = recvTime
                                    });
                                    if (ISReSendData(AdultPATRecvData) || !AdultPATRecvData.Contains(">PAT:A"))
                                    {
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultPat);
                                        pm.code = sendAdultPat;
                                        pm.IsPn = false;
                                        //发送
                                        AdultPATRecvData = SendCommand(pm);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultPATRecvData);
                                        //指令信息                                    
                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                        {
                                            InsType = NewInsType._PAT_A,
                                            SendCmd = sendAdultPat,
                                            SendTime = sendTime,
                                            RecvData = AdultPATRecvData,
                                            RecvTime = recvTime
                                        });
                                    }
                                }
                                else
                                {
                                    sendAdultPat = string.Format("IG|rt{0}|pat:a{1}#1", PnrObj.AdultPnr, Office);
                                    sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultPat);
                                    sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    AdultPATRecvData = sendec.SendData(sendAdultPat, out errMsg);
                                    recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                    sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultPATRecvData);
                                    //指令信息                                
                                    PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                    {
                                        InsType = NewInsType._PAT_A,
                                        SendCmd = sendAdultPat,
                                        SendTime = sendTime,
                                        RecvData = AdultPATRecvData,
                                        RecvTime = recvTime
                                    });
                                    if (AdultPATRecvData.Contains("超时") || AdultPATRecvData.Contains("服务器忙") || AdultPATRecvData.Contains("无法从传输连接中读取数据"))
                                    {
                                        sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendAdultPat);
                                        AdultPATRecvData = sendec.SendData(sendAdultPat, out errMsg);
                                        recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                        sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, AdultPATRecvData);
                                        //指令信息                                    
                                        PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                        {
                                            InsType = NewInsType._PAT_A,
                                            SendCmd = sendAdultPat,
                                            SendTime = sendTime,
                                            RecvData = AdultPATRecvData,
                                            RecvTime = recvTime
                                        });
                                    }
                                }
                                PnrObj.PatList[0] = AdultPATRecvData;
                            }
                        }
                    }
                    else
                    {
                        sbLog.AppendFormat("[INF]时间:{0}\r\n起飞时间已小于预定时间,不预定！\r\n", System.DateTime.Now);
                    }
                    #endregion
                }
                else
                {
                    #region 有编码时获取实体RePnrObj
                    if (InputParam.IPnr.Trim() != "" && InputParam.IPnr.Trim().Length == 6)
                    {
                        //获取成人RT内容   
                        string sendRT = "", RTRecvData = "";
                        PnrModel pnrModel = null;
                        //发送PAT指令
                        if (InputParam.UsePIDChannel == 2)
                        {
                            sendRT = string.Format("rt{0}", InputParam.IPnr);
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            pm.code = sendRT;
                            pm.IsPn = true;
                            //发送
                            RTRecvData = SendCommand(pm);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                            //指令信息                            
                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                            {
                                InsType = NewInsType._RT,
                                SendCmd = sendRT,
                                SendTime = sendTime,
                                RecvData = RTRecvData,
                                RecvTime = recvTime
                            });
                            if (ISReSendData(RTRecvData))
                            {
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                pm.code = sendRT;
                                pm.IsPn = true;
                                //发送
                                RTRecvData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                //指令信息                                
                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                {
                                    InsType = NewInsType._RT,
                                    SendCmd = sendRT,
                                    SendTime = sendTime,
                                    RecvData = RTRecvData,
                                    RecvTime = recvTime
                                });
                            }
                            pnrModel = pnrformat.GetPNRInfo(InputParam.IPnr, RTRecvData, false, out errMsg);
                            string BigPnrCode = pnrformat.GetBigCode(RTRecvData, out errMsg);
                            //成人编码
                            if (pnrModel._PasType == "1")
                            {
                                PnrObj.AdultPnr = InputParam.IPnr;
                                PnrObj.AdultPnrToBigPNR = BigPnrCode;
                                PnrObj.AdultPnrRTContent = RTRecvData;
                            }
                            else if (pnrModel._PasType == "2")
                            {
                                PnrObj.childPnr = InputParam.IPnr;
                                PnrObj.childPnrToBigPNR = BigPnrCode;
                                PnrObj.childPnrRTContent = RTRecvData;
                            }

                            if (pnrModel._PasType == "1")
                            {
                                sendRT = string.Format("rt{0}|pat:a", PnrObj.AdultPnr);
                            }
                            else if (pnrModel._PasType == "2")
                            {
                                sendRT = string.Format("rt{0}|pat:a*ch", PnrObj.childPnr);
                            }
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                            pm.code = sendRT;
                            pm.IsPn = false;
                            //发送
                            RTRecvData = SendCommand(pm);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                            //指令信息                            
                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                            {
                                InsType = pnrModel._PasType == "1" ? NewInsType._PAT_A : NewInsType._PAT_A_CH,
                                SendCmd = sendRT,
                                SendTime = sendTime,
                                RecvData = RTRecvData,
                                RecvTime = recvTime
                            });

                            if (ISReSendData(RTRecvData))
                            {
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                pm.code = sendRT;
                                pm.IsPn = false;
                                //发送
                                RTRecvData = SendCommand(pm);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                //指令信息                               
                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                {
                                    InsType = pnrModel._PasType == "1" ? NewInsType._PAT_A : NewInsType._PAT_A_CH,
                                    SendCmd = sendRT,
                                    SendTime = sendTime,
                                    RecvData = RTRecvData,
                                    RecvTime = recvTime
                                });
                            }
                        }
                        else
                        {
                            sendRT = string.Format("IG|(eas)rt{0}{1}#1", InputParam.IPnr, Office);
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);

                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            RTRecvData = sendec.SendData(sendRT, out errMsg);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                            //指令信息                            
                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                            {
                                InsType = NewInsType._RT,
                                SendCmd = sendRT,
                                SendTime = sendTime,
                                RecvData = RTRecvData,
                                RecvTime = recvTime
                            });

                            if (RTRecvData.Contains("超时") || RTRecvData.Contains("服务器忙") || RTRecvData.Contains("无法从传输连接中读取数据"))
                            {
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                RTRecvData = sendec.SendData(sendRT, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                //指令信息                                
                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                {
                                    InsType = NewInsType._RT,
                                    SendCmd = sendRT,
                                    SendTime = sendTime,
                                    RecvData = RTRecvData,
                                    RecvTime = recvTime
                                });
                            }

                            pnrModel = pnrformat.GetPNRInfo(InputParam.IPnr, RTRecvData, false, out errMsg);
                            string BigPnrCode = pnrformat.GetBigCode(RTRecvData, out errMsg);
                            //成人编码
                            if (pnrModel._PasType == "1")
                            {
                                PnrObj.AdultPnr = InputParam.IPnr;
                                PnrObj.AdultPnrToBigPNR = BigPnrCode;
                                PnrObj.AdultPnrRTContent = RTRecvData;
                            }
                            else if (pnrModel._PasType == "2")
                            {
                                PnrObj.childPnr = InputParam.IPnr;
                                PnrObj.childPnrToBigPNR = BigPnrCode;
                                PnrObj.childPnrRTContent = RTRecvData;
                            }

                            if (pnrModel._PasType == "1")
                            {
                                sendRT = string.Format("IG|rt{0}|pat:a{1}#1", InputParam.IPnr, Office);
                            }
                            else if (pnrModel._PasType == "2")
                            {
                                sendRT = string.Format("IG|rt{0}|pat:a*ch{1}#1", InputParam.IPnr, Office);
                            }
                            sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                            sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            RTRecvData = sendec.SendData(sendRT, out errMsg);
                            recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                            sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                            //指令信息                            
                            PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                            {
                                InsType = pnrModel._PasType == "1" ? NewInsType._PAT_A : NewInsType._PAT_A_CH,
                                SendCmd = sendRT,
                                SendTime = sendTime,
                                RecvData = RTRecvData,
                                RecvTime = recvTime
                            });

                            if (RTRecvData.ToUpper().Contains("LEASE WAIT - TRANSACTION IN PROGRESS") || RTRecvData.Contains("超时") || RTRecvData.Contains("服务器忙") || RTRecvData.Contains("无法从传输连接中读取数据"))
                            {
                                sendTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n发送指令:\r\n{1}\r\n", System.DateTime.Now, sendRT);
                                RTRecvData = sendec.SendData(sendRT, out errMsg);
                                recvTime = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                                sbLog.AppendFormat("时间:{0}\r\n接收数据:\r\n{1}\r\n", System.DateTime.Now, RTRecvData);
                                //指令信息
                                //PnrObj.InsList.Add(sendRT + PnrObj.SplitChar + sendTime + PnrObj.SplitChar + recvTime + PnrObj.SplitChar + InputParam.Office, RTRecvData);
                                PnrObj.InsList.Add(new InsParam(InputParam.ServerIP, InputParam.ServerPort, InputParam.Office)
                                {
                                    InsType = pnrModel._PasType == "1" ? NewInsType._PAT_A : NewInsType._PAT_A_CH,
                                    SendCmd = sendRT,
                                    SendTime = sendTime,
                                    RecvData = RTRecvData,
                                    RecvTime = recvTime
                                });
                            }
                        }
                        if (pnrModel._PasType == "1")
                        {
                            PnrObj.PatList[0] = RTRecvData;
                        }
                        else if (pnrModel._PasType == "2")
                        {
                            PnrObj.PatList[1] = RTRecvData;
                        }
                        PnrObj.PnrType = pnrModel._PasType;
                    }
                    #endregion
                }
                #endregion

                sbLog.AppendFormat("内容转换到实体时间1:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
                if (!InputParam.FlyTimeIsOverCurrTime)
                {
                    //内容转换到实体
                    ConToModel(PnrObj, sbLog, out errMsg);
                    //成人
                    if (PnrObj.AdultPnrRTContent != null && PnrObj.AdultPnrRTContent != "" && PnrObj.AdultPnr.Trim().Length == 6)
                    {
                        PnrObj.AdultPnrLegIsYDSame = InputParam.SkyList.Count == PnrObj.PnrList[0]._LegList.Count ? true : false;
                        int infCount = 0;
                        PnrObj.PnrList[0]._PassengerList.ForEach(p =>
                        {
                            if (p.PassengerType == "3")
                            {
                                infCount++;
                            }
                        });
                        if (infCount == YingerPasList.Count)
                        {
                            PnrObj.YdInfIsSame = true;
                        }
                        else
                        {
                            PnrObj.YdInfIsSame = false;
                        }
                    }
                    //儿童
                    if (PnrObj.childPnrRTContent != null && PnrObj.childPnrRTContent != "" && PnrObj.childPnr.Trim().Length == 6)
                    {
                        PnrObj.ChildPnrLegIsYDSame = InputParam.SkyList.Count == PnrObj.PnrList[1]._LegList.Count ? true : false;
                    }
                }
                else
                {
                    sbLog.AppendFormat("[ConToModel]时间:{0}\r\n起飞时间已小于预定时间,不预定！\r\n", System.DateTime.Now);
                }
                sbLog.AppendFormat("内容转换到实体时间2:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            catch (Exception ex)
            {
                sbLog.Append(ex.TargetSite.ToString() + "\r\n");
            }
            finally
            {
                sbLog.AppendFormat("结束================================================================\r\n");
                //记录日志
                LogText.LogWrite(sbLog.ToString(), "ILog");
            }
            return PnrObj;
        }
        /// <summary>
        /// 将编码内容解析到实体
        /// </summary>
        /// <param name="PnrObj"></param>
        /// <param name="errMsg"></param>
        public static void ConToModel(RePnrObj PnrObj, StringBuilder sbLog, out string errMsg)
        {
            sbLog.AppendFormat("ConToModel时间1:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            FormatPNR pnrformat = new FormatPNR();
            errMsg = "";
            #region Pnr内容解析
            string pnrRemark = string.Empty;
            //成人
            if (PnrObj.AdultPnrRTContent != null && PnrObj.AdultPnrRTContent != "" && PnrObj.AdultPnr.Trim().Length == 6)
            {
                //沈阳的配置去掉备注
                if (PnrObj.Office.ToUpper() == "SHE185")
                {
                    string RTCon = pnrformat.DelRepeatRTCon(PnrObj.AdultPnrRTContent, ref pnrRemark);
                    if (!string.IsNullOrEmpty(pnrRemark))
                    {
                        PnrObj.AdultPnrRTContent = PnrObj.AdultPnrRTContent.Replace(pnrRemark, "");
                    }
                }
                PnrObj.PnrList[0] = pnrformat.GetPNRInfo(PnrObj.AdultPnr, PnrObj.AdultPnrRTContent, false, out errMsg);
            }
            //儿童
            if (PnrObj.childPnrRTContent != null && PnrObj.childPnrRTContent != "" && PnrObj.childPnr.Trim().Length == 6)
            {
                //沈阳的配置去掉备注
                if (PnrObj.Office.ToUpper() == "SHE185")
                {
                    string RTCon = pnrformat.DelRepeatRTCon(PnrObj.childPnrRTContent, ref pnrRemark);
                    if (!string.IsNullOrEmpty(pnrRemark))
                    {
                        PnrObj.childPnrRTContent = PnrObj.childPnrRTContent.Replace(pnrRemark, "");
                    }
                }
                PnrObj.PnrList[1] = pnrformat.GetPNRInfo(PnrObj.childPnr, PnrObj.childPnrRTContent, false, out errMsg);
            }
            #endregion
            sbLog.AppendFormat("ConToModel时间2:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

            #region Pat内容解析
            //成人pat
            if (PnrObj.PatList[0] != null && PnrObj.PatList[0].Trim() != "" && PnrObj.PatList[0].Trim().Length > 30)
            {
                PnrObj.PatModelList[0] = pnrformat.GetPATInfo(PnrObj.PatList[0].Trim(), out errMsg);
            }
            //儿童pat
            if (PnrObj.PatList[1] != null && PnrObj.PatList[1].Trim() != "" && PnrObj.PatList[1].Trim().Length > 30)
            {
                PnrObj.PatModelList[1] = pnrformat.GetPATInfo(PnrObj.PatList[1].Trim(), out errMsg);
            }
            //婴儿pat
            if (PnrObj.PatList[2] != null && PnrObj.PatList[2].Trim() != "" && PnrObj.PatList[2].Trim().Length > 30)
            {
                PnrObj.PatModelList[2] = pnrformat.GetPATInfo(PnrObj.PatList[2].Trim(), out errMsg);
            }
            if (PnrObj.CHDToAdultPatCon != null && PnrObj.CHDToAdultPatCon.Trim() != "")
            {
                PnrObj.CHDToAdultPat = pnrformat.GetPATInfo(PnrObj.CHDToAdultPatCon.Trim(), out errMsg);
            }
            #endregion

            sbLog.AppendFormat("ConToModel时间3:{0}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"));

        }
        /// <summary>
        /// 预订编码有些指令 返回数据是否重发
        /// </summary>
        /// <param name="recvData"></param>
        /// <returns></returns>
        public bool ISReSendData(string recvData)
        {
            bool IsReSend = false;
            if (recvData.Trim().ToUpper() == "NO PNR")
            {
                IsReSend = true;
            }
            else
            {
                List<string> List = new List<string>();
                List.Add("可能通讯断开");
                List.Add("VALUE CANNOT BE NULL");
                List.Add("LEASE WAIT - TRANSACTION IN PROGRESS");
                List.Add("WSACANCELBLOCKINGCALL");
                List.Add("UNABLE TO READ DATA FROM THE TRANSPORT CONNECTION");
                List.Add("INDEX (ZERO BASED) MUST BE GREATER THAN OR EQUAL TO ZERO AND LESS THAN THE SIZE OF THE ARGUMENT LIST");
                List.Add("VALUE CANNOT BE NULL");
                List.Add("THIS OPERATION IS ONLY ALLOWED USING A SUCCESSFULLY AUTHENTICATED CONTEXT");
                List.Add("超时");
                List.Add("服务器忙");
                List.Add("NO PNR   没有可供显示的页面");
                List.Add("没有可供显示的页面");
                List.Add("无法从传输连接中读取数据");
                foreach (string item in List)
                {
                    if (recvData.ToUpper().Contains(item) || recvData.ToUpper() == "S")
                    {
                        IsReSend = true;
                        break;
                    }
                }
            }
            if (IsReSend)
            {
                //重发间隔10毫秒
                Thread.Sleep(10);
            }
            return IsReSend;
        }



        /// <summary>
        /// 组合预订编码指令
        /// </summary>
        /// <param name="InputParam"></param>
        /// <param name="pas"></param>
        /// <returns></returns>
        private string GetYuDingIns(PnrParamObj InputParam, List<IPassenger> pas)
        {
            StringBuilder sb = new StringBuilder("NM");
            bool IsAddLine = false;//姓名项字符超过80加一个\r
            int a = 0;
            bool IsChild = false;
            for (int i = 0; i < pas.Count; i++)
            {
                if (pas[i].PassengerType != 3)
                {
                    if (pas[i].PassengerType == 2)
                    {
                        IsChild = true;
                    }
                    if (pas[i].PassengerType == 2)
                    {
                        //儿童
                        if (PinYingMange.IsChina(pas[i].PassengerName.Trim()))
                        {
                            if (pas[i].PassengerName.EndsWith("CHD"))
                            {
                                pas[i].PassengerName = pas[i].PassengerName.Substring(0, pas[i].PassengerName.LastIndexOf("CHD")).Replace(" ", "");
                            }
                        }
                        else
                        {
                            if (pas[i].PassengerName.EndsWith(" CHD"))
                            {
                                pas[i].PassengerName = pas[i].PassengerName.Substring(0, pas[i].PassengerName.LastIndexOf(" CHD"));
                            }
                        }
                        //if (InputParam.CarryCode.ToUpper() != "CZ")
                        //{
                        pas[i].PassengerName += "CHD";
                        //}
                        sb.Append("1" + pas[i].PassengerName.Replace(" ", "").Trim());
                    }
                    else
                    {
                        //成人
                        sb.Append("1" + pas[i].PassengerName.Replace(" ", "").Trim());
                    }

                    //一屏幕显示不完加\r
                    int len = System.Text.Encoding.ASCII.GetByteCount(sb.ToString());
                    if (len > 80 && !IsAddLine)
                    {
                        sb.Append("\r");
                        IsAddLine = true;
                    }
                    a++;
                }
            }
            sb.Append("\r");


            decimal decDiscount = 0m;
            DateTime endTimeChd = DateTime.Parse(InputParam.EUChdValidDate);
            //起飞日期
            DateTime FlyDate = DateTime.Parse(InputParam.SkyList[0].FlyStartDate + " " + InputParam.SkyList[0].FlyStartTime.Insert(2, ":") + ":00");
            string[] Space = new string[InputParam.SkyList.Count];
            for (int i = 0; i < InputParam.SkyList.Count; i++)
            {
                decimal.TryParse(InputParam.SkyList[i].Discount, out decDiscount);
                if (IsChild)
                {
                    if (InputParam.ChildIsAdultEtdz == "1" || InputParam.SkyList[i].Space.ToUpper() == "F" || InputParam.SkyList[i].Space.ToUpper() == "C" || InputParam.SkyList[i].Space.ToUpper() == "Y" || decDiscount >= 100)
                    {
                        //排除子舱位
                        Space[i] = InputParam.SkyList[i].Space.ToUpper().Substring(0, 1);
                    }
                    else
                    {
                        Space[i] = "Y";
                    }
                    //EU只能是O舱
                    if (InputParam.CarryCode.ToUpper() == "EU" && DateTime.Compare(FlyDate, endTimeChd) < 0)
                    {
                        Space[i] = "O";
                    }
                }
                else
                {
                    Space[i] = InputParam.SkyList[i].Space.ToUpper().Substring(0, 1);
                }
                sb.Append("SS " + InputParam.SkyList[i].CarryCode.Replace("*", "") + InputParam.SkyList[i].FlightCode.Replace("*", "") + " " + Space[i] + " " + FormatPNR.DateToStr(InputParam.SkyList[i].FlyStartDate, DataFormat.dayMonthYear) + " " + InputParam.SkyList[i].fromCode.ToUpper() + InputParam.SkyList[i].toCode.ToUpper() + " " + a + "\r");
            }
            if (!string.IsNullOrEmpty(InputParam.CTTel) && InputParam.CTTel != InputParam.CTCTPhone)
            {
                sb.Append("CT" + InputParam.CTTel.Replace("－", "-").Trim() + "\r");
            }
            //落地运营商公司电话号码
            //if (!string.IsNullOrEmpty(InputParam.LuoDiCTTel) && InputParam.LuoDiCTTel != InputParam.CTTel)
            //{
            //    sb.Append("CT" + InputParam.LuoDiCTTel + "\r");
            //}

            //证件号处理
            StringBuilder sbCHLD = new StringBuilder();
            string cid = "";
            for (int i = 0; i < pas.Count; i++)
            {
                if (IsChild)//儿童
                {
                    cid = pas[i].PasSsrCardID.Replace("－", "-").Trim();
                    string regDate = @"^\d{4}-\d{2}-\d{2}$";
                    try
                    {
                        if (Regex.IsMatch(pas[i].PasSsrCardID, regDate))
                        {
                            cid = cid.Replace("-", "").Replace(":", "").Replace(" ", "");
                        }
                        if (Regex.IsMatch(pas[i].ChdBirthday, regDate))//儿童标识CHLD
                        {
                            sbCHLD.Append("SSR CHLD " + InputParam.CarryCode.ToUpper() + " HK1/" + FormatPNR.DateToStr(pas[i].ChdBirthday, DataFormat.dayMonthYear) + "/P" + (i + 1).ToString() + "\r");
                        }
                    }
                    catch
                    {
                    }
                    sb.Append("SSR FOID " + InputParam.CarryCode.ToUpper() + " HK/NI" + cid + "/P" + (i + 1) + "\r");
                }
                else
                {
                    if (pas[i].PassengerType == 1)//成人
                    {
                        cid = pas[i].PasSsrCardID.Replace("－", "-").Trim();
                        try
                        {
                            string regDate = @"^\d{4}-\d{2}-\d{2}$";
                            if (Regex.IsMatch(cid, regDate))
                            {
                                // DateTime.Parse(cid);
                                // cid = FormatPNR.DateToStr(cid, DataFormat.dayMonthYear);
                                cid = cid.Replace("-", "").Replace(":", "").Replace(" ", "");
                            }
                        }
                        catch { }
                        sb.Append("SSR FOID " + InputParam.CarryCode.ToUpper() + " HK/NI" + cid + "/P" + (i + 1) + "\r");
                    }
                }
                //航空公司卡号
                if (!string.IsNullOrEmpty(pas[i].CpyandNo))
                {
                    string strCpyandNo = pas[i].CpyandNo.Trim().ToUpper().StartsWith(InputParam.CarryCode.ToUpper()) ? pas[i].CpyandNo.Trim() : (InputParam.CarryCode.ToUpper() + pas[i].CpyandNo.Trim());
                    sb.Append("SSR FQTV " + InputParam.CarryCode.ToUpper() + " HK/" + strCpyandNo + "/P" + (i + 1) + "\r");
                }
            }
            string CarryCode = InputParam.CarryCode.ToUpper();
            //添加乘客手机号码
            for (int i = 0; i < pas.Count; i++)
            {
                if (!string.IsNullOrEmpty(pas[i].LinkPhone))
                {
                    sb.AppendFormat("OSI {0} CTCM{1}/P{2}\r", CarryCode, pas[i].LinkPhone.Trim(), (i + 1));
                }
            }
            if (!string.IsNullOrEmpty(InputParam.CTCTPhone))
            {
                string CTCTContactTel = InputParam.CTCTPhone.Replace("－", "-").Trim();
                if (CarryCode == "MF")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT {1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "ZH" || CarryCode == "HU")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT {1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "CA")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                //else if (CarryCode == "MU")
                //{
                //    //CTCM项
                //    sb.AppendFormat("OSI {0} CTCM{1}/P1\r", CarryCode, CTCTContactTel);
                //}
                //else if (CarryCode == "CZ")
                //{
                //    //CTCT项
                //    sb.AppendFormat("OSI {0} CTCP{1}\r", CarryCode, CTCTContactTel);
                //}                
                else if (CarryCode == "FM" || CarryCode == "KN")
                {
                    //CTCT项
                    //sb.AppendFormat("OSI {0} CTCTM{1}/PN\r", CarryCode, CTCTContactTel);
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                else
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                //CTCM项
                //sb.AppendFormat("OSI {0} CTCM{1}/P1\r", CarryCode, CTCTContactTel);
            }

            //落地运营商手机号码         
            if (InputParam.LuoDiCTCTPhone.Trim() != InputParam.CTCTPhone.Trim() && !string.IsNullOrEmpty(InputParam.LuoDiCTCTPhone))
            {
                string CTCTContactTel = InputParam.LuoDiCTCTPhone.Replace("－", "-").Trim();
                if (CarryCode == "MF")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT {1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "ZH" || CarryCode == "HU")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT {1}\r", CarryCode, CTCTContactTel);
                }
                else if (CarryCode == "CA")
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                //else if (CarryCode == "CZ")
                //{
                //    //CTCT项
                //    sb.AppendFormat("OSI {0} CTCP{1}\r", CarryCode, CTCTContactTel);
                //}    
                //else if (CarryCode == "MU")
                //{
                //    //CTCM项
                //    sb.AppendFormat("OSI {0} CTCM{1}/P1\r", CarryCode, CTCTContactTel);
                //}
                else if (CarryCode == "FM" || CarryCode == "KN")
                {
                    //CTCT项
                    //sb.AppendFormat("OSI {0} CTCTM{1}/PN\r", CarryCode, CTCTContactTel);
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                else
                {
                    //CTCT项
                    sb.AppendFormat("OSI {0} CTCT{1}\r", CarryCode, CTCTContactTel);
                }
                //CTCM项
                //sb.AppendFormat("OSI {0} CTCM{1}/P1\r", CarryCode, CTCTContactTel);
            }

            //儿童CHLD
            if (sbCHLD.ToString() != "")
            {
                sb.Append(sbCHLD.ToString());
            }
            //第一程出发日期小于第二程
            InputParam.SkyList.Sort(delegate(ISkyLeg sky1, ISkyLeg sky2)
            {
                DateTime d1 = DateTime.Parse(sky1.FlyStartDate + " " + sky1.FlyStartTime.Insert(2, ":") + ":00");
                DateTime d2 = DateTime.Parse(sky2.FlyStartDate + " " + sky2.FlyStartTime.Insert(2, ":") + ":00");
                return (DateTime.Compare(d1, d2));
            });
            DateTime t = DateTime.Parse(InputParam.SkyList[0].FlyStartDate + " " + InputParam.SkyList[0].FlyStartTime.Insert(2, ":") + ":00");
            //当前时间延后几小时  如果起飞时间小于延后的时间 不能预定
            DateTime tempTime = System.DateTime.Now.AddHours(InputParam.FlyAdvanceTime);
            if (DateTime.Compare(t, tempTime) <= 0)
            {
                //起飞时间小于预定时间  已起飞 已失效 不能预定
                InputParam.FlyTimeIsOverCurrTime = true;
            }
            //string time = t.ToString("HHmm");
            //TKTL/2330/26OCT11/KMG226
            string tempOffice = InputParam.Office;
            if (tempOffice.ToLower() == "lhw108")
            {
                tempOffice = "lhw148";
            }
            //当前时间
            DateTime currDate = System.DateTime.Now;
            string newTime = currDate.AddHours(2).ToString("HHmm");
            sb.Append("TKTL/" + newTime + "/" + FormatPNR.DateToStr(currDate.ToString("yyyy-MM-dd"), DataFormat.dayMonth) + "/" + tempOffice);
            sb.Append("\r");
            if (InputParam.UsePIDChannel == 0)
            {
                sb.Append("@&" + InputParam.Office + "$");
            }
            else
            {
                sb.Append("@&" + InputParam.Office);
            }
            //转半角字符
            if (InputParam.UsePIDChannel == 0)
            {
                return JZFormat.ToDBC(sb.ToString().ToUpper().Replace("\r", "\n"));
            }
            else
            {
                return JZFormat.ToDBC(sb.ToString().ToUpper());
            }
        }


        #endregion

        #region BSP自动出票
        /// <summary>
        /// BSP自动出票
        /// </summary>
        /// <returns></returns>
        public BSPResponse BSPAutoIssue(BSPParam bspParam)
        {
            //编码解析类
            FormatPNR pnrFormat = new FormatPNR();
            //出票结果
            BSPResponse bspResult = new BSPResponse();
            //日志
            StringBuilder sbLog = new StringBuilder();
            ParamObject pm = bspParam.Param;
            string Msg = string.Empty;
            sbLog.AppendFormat("START===============第{0}次======================\r\n", bspParam.TryCount);
            try
            {
                if (string.IsNullOrEmpty(pm.ServerIP)
                    || string.IsNullOrEmpty(pm.ServerPort.ToString())
                    || string.IsNullOrEmpty(pm.Office))
                {
                    bspResult.Msg = "服务器IP,端口和出票Office不能为空！";
                }
                else
                {
                    if (string.IsNullOrEmpty(bspParam.PrintNo))
                    {
                        bspResult.Msg = string.Format("{0}未设置打票机号,请手动出票或者设置打票机号！", pm.WebUserName);
                    }
                    else if (string.IsNullOrEmpty(bspParam.Pnr))
                    {
                        bspResult.Msg = "编码编码不能为空！";
                    }
                    else if (bspParam.CpPrice <= 0)
                    {
                        bspResult.Msg = string.Format("编码:{0}的出票价格不能小于0！", bspParam.Pnr);
                    }
                }
                if (string.IsNullOrEmpty(bspResult.Msg))
                {
                    pm.IsAllResult = false;
                    pm.IsUseExtend = false;
                    pm.IsHandResult = true;
                    pm.ExtendData = string.Empty;
                    string Pnr = bspParam.Pnr;

                    sbLog.Append("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n");
                    sbLog.Append("Office:" + pm.Office + "\r\n");
                    sbLog.Append("IP:" + pm.ServerIP + ":" + pm.ServerPort + "\r\n");
                    sbLog.Append("Code:" + pm.WebUserName + "\r\n");
                    sbLog.Append("Pnr:" + Pnr + "\r\n");

                    //发送指令
                    pm.code = string.Format("RT{0}", Pnr);
                    pm.IsPn = true;
                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                    pm.recvData = SendNewPID.SendCommand(pm);
                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);
                    if (ISReSendData(pm.recvData))
                    {
                        pm.code = string.Format("RT{0}", Pnr);
                        pm.IsPn = true;
                        sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                        pm.recvData = SendNewPID.SendCommand(pm);
                        sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);
                    }
                    //授权检查和取消
                    if (pm.recvData.ToUpper().Contains("授权")
                        || pm.recvData.ToUpper().Contains("CANCELLED")
                        )
                    {
                        bspResult.Msg = pm.recvData.ToUpper().Contains("授权") ? pm.recvData : "编码状态为XX,不能出票";
                    }
                    else if (pm.recvData.ToUpper().Contains("THIS OPERATION IS ONLY ALLOWED"))
                    {
                        bspResult.Msg = pm.recvData.ToUpper();
                    }
                    else
                    {
                        //检查编码状态
                        string PnrStatus = pnrFormat.GetPnrStatus(pm.recvData, out Msg);
                        if (PnrStatus.Contains("NO")
                            || PnrStatus.Contains("XX")
                            )
                        {
                            bspResult.Msg = string.Format("编码状态为{0},不能出票!", PnrStatus);
                        }
                        else
                        {
                            PnrModel PnrInfo = pnrFormat.GetPNRInfo(Pnr, pm.recvData, false, out Msg);
                            bool hasINF = PnrInfo.HasINF;
                            foreach (PnrAnalysis.Model.TicketNumInfo item in PnrInfo._TicketNumList)
                            {
                                bspResult.BspResult.Add(item.PasName, item.TicketNum);
                                sbLog.AppendFormat("乘客:{0} 票号:{1}\r\n", item.PasName, item.TicketNum);
                            }
                            if (bspResult.BspResult.Count == 0)
                            {
                                bool IsFindXe = false;
                                //继续处理                              
                                XeSFC(pnrFormat, sbLog, pm, Pnr, out IsFindXe);
                                if (IsFindXe)
                                {
                                    //发送指令
                                    pm.code = string.Format("RT{0}", Pnr);
                                    pm.IsPn = true;

                                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                    pm.recvData = SendNewPID.SendCommand(pm);
                                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                                    if (ISReSendData(pm.recvData))
                                    {
                                        pm.code = string.Format("RT{0}", Pnr);
                                        pm.IsPn = true;

                                        sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                        pm.recvData = SendNewPID.SendCommand(pm);
                                        sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);
                                    }
                                    PnrInfo = pnrFormat.GetPNRInfo(Pnr, pm.recvData, false, out Msg);
                                }
                                //航段序号
                                List<string> LegList = new List<string>();
                                foreach (LegInfo leg in PnrInfo._LegList)
                                {
                                    LegList.Add(leg.SerialNum + "RR");
                                }
                                //出票时限序号
                                List<string> TkTlList = pnrFormat.GetTKTLNumber(pm.recvData);
                                List<string> strList = new List<string>();
                                foreach (string item in TkTlList)
                                {
                                    strList.Add("XE" + item);
                                }
                                if (strList.Count == 0)
                                {
                                    bspResult.Msg = string.Format("编码:{0},没有取到出票时限项！", Pnr);
                                }
                                else if (LegList.Count == 0)
                                {
                                    bspResult.Msg = string.Format("编码:{0},没有取到航段项！", Pnr);
                                }
                                else
                                {
                                    //发送指令
                                    string patPrice = (PnrInfo._PasType == "1" ? "PAT:A" : "PAT:A*CH");
                                    if (bspParam.IssueINFTicekt)
                                    {
                                        patPrice = "PAT:A*IN";
                                    }
                                    pm.code = string.Format("RT{0}|{1}", Pnr, patPrice);
                                    pm.IsPn = false;

                                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                    pm.recvData = SendNewPID.SendCommand(pm);
                                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                                    //重发
                                    if (ISReSendData(pm.recvData))
                                    {
                                        pm.code = string.Format("RT{0}|{1}", Pnr, patPrice);
                                        pm.IsPn = false;

                                        sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                        pm.recvData = SendNewPID.SendCommand(pm);
                                        sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);
                                    }

                                    if (!pm.recvData.Contains("PAT:A"))
                                    {
                                        bspResult.Msg = string.Format("发送指令:{0},没有取到价格！", pm.code);
                                    }
                                    else
                                    {
                                        PatModel PAT = pnrFormat.GetPATInfo(pm.recvData, out Msg);
                                        string xuhao = "";
                                        decimal _xsFare = 0m;
                                        bool IsExistParice = false;
                                        sbLog.AppendFormat("【价格判断】\r\n数量{0}", PAT.UninuePatList.Count);
                                        foreach (PatInfo pat in PAT.UninuePatList)
                                        {
                                            decimal.TryParse(pat.Fare, out _xsFare);
                                            sbLog.AppendFormat("价格:{0} 原价格:{1} 真值:{2} \r\n", pat.Fare, bspParam.CpPrice, (_xsFare == bspParam.CpPrice));
                                            //存在此价格
                                            if (_xsFare == bspParam.CpPrice)
                                            {
                                                IsExistParice = true;
                                                xuhao = pat.SerialNum;
                                                break;
                                            }
                                        }
                                        sbLog.AppendFormat("IsExistParice={0}\r\n", IsExistParice);
                                        if (IsExistParice)  //存在继续出票
                                        {
                                            //做价格进去                                                          
                                            pm.code = string.Format("RT{0}|{1}|SFC:{2}|@", Pnr, patPrice, xuhao);
                                            pm.IsPn = false;

                                            sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                            pm.recvData = SendNewPID.SendCommand(pm);
                                            sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                                            //重发一次
                                            if (ISReSendData(pm.recvData))
                                            {
                                                pm.code = string.Format(@"RT{0}|{1}|SFC:{2}|\ki", Pnr, patPrice, xuhao);
                                                pm.IsPn = false;

                                                sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                pm.recvData = SendNewPID.SendCommand(pm);
                                                sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);
                                            }
                                            //检查返回数据
                                            if (ISReSendData(pm.recvData))
                                            {
                                                bspResult.Msg = pm.recvData;
                                            }
                                            else
                                            {
                                                if (pm.recvData.ToUpper().Contains("ITINERARY DOES NOT MATCH FC"))
                                                {
                                                    bspResult.Msg = string.Format("编码{0} 出票Office:{1}，原因：FC城市和行程中机场代码不符，需手工出票!\r\n", Pnr, pm.Office);
                                                }
                                                else if (pm.recvData.ToUpper().Contains("INCOMPLETE PNR/FN"))
                                                {
                                                    bspResult.Msg = string.Format("编码{0} 出票Office:{1}，原因：补价格指令：{2}失败,返回数据:\r\n", Pnr, pm.Office, pm.code);
                                                }
                                                else
                                                {
                                                    if (bspParam.IssueINFTicekt)//单独婴儿
                                                    {
                                                        //出婴儿票
                                                        pm.code = string.Format("RT{0}|{1}|ETDZ {2},INF|@", Pnr, string.Join("|", strList.ToArray()), bspParam.PrintNo);
                                                    }
                                                    else
                                                    {
                                                        //发送出票指令                                                
                                                        pm.code = string.Format("RT{0}|{1}|{2}|ETDZ {3}|@", Pnr, string.Join("|", strList.ToArray()), string.Join("|", LegList.ToArray()), bspParam.PrintNo);
                                                    }
                                                    pm.IsPn = false;

                                                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                    pm.recvData = SendNewPID.SendCommand(pm);
                                                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                                                    //成人和婴儿在一起出婴儿票
                                                    if (!bspParam.IssueINFTicekt && hasINF)
                                                    {
                                                        pm.code = string.Format("RT{0}|{1}|ETDZ {2},INF|@", Pnr, string.Join("|", strList.ToArray()), bspParam.PrintNo);
                                                        pm.IsPn = false;

                                                        sbLog.AppendFormat("【{0}】\r\n[INF]发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                        pm.recvData = SendNewPID.SendCommand(pm);
                                                        sbLog.AppendFormat("【{0}】\r\n[INF]接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);
                                                    }

                                                    #region  重发一次出票指令
                                                    if (ISReSendData(pm.recvData)
                                                        || pm.recvData.ToUpper().Contains("PLEASE CHECK TKT ELEMENT")
                                                        )
                                                    {
                                                        if (bspParam.IssueINFTicekt)//单独婴儿
                                                        {
                                                            //出婴儿票
                                                            pm.code = string.Format(@"RT{0}|{1}|ETDZ {2},INF|\ki", Pnr, string.Join("|", strList.ToArray()), bspParam.PrintNo);
                                                        }
                                                        else
                                                        {
                                                            //发送出票指令                                                
                                                            pm.code = string.Format(@"RT{0}|{1}|{2}|ETDZ {3}|\ki", Pnr, string.Join("|", strList.ToArray()), string.Join("|", LegList.ToArray()), bspParam.PrintNo);
                                                        }
                                                        pm.IsPn = false;

                                                        sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                        pm.recvData = SendNewPID.SendCommand(pm);
                                                        sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);


                                                        //成人和婴儿在一起出婴儿票
                                                        if (!bspParam.IssueINFTicekt && hasINF)
                                                        {
                                                            pm.code = string.Format(@"RT{0}|{1}|ETDZ {2},INF|\ki", Pnr, string.Join("|", strList.ToArray()), bspParam.PrintNo);
                                                            pm.IsPn = false;

                                                            sbLog.AppendFormat("【{0}】\r\n[INF]发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                            pm.recvData = SendNewPID.SendCommand(pm);
                                                            sbLog.AppendFormat("【{0}】\r\n[INF]接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                                                        }
                                                    }
                                                    #endregion

                                                    if (ISReSendData(pm.recvData))
                                                    {
                                                        bspResult.Msg = "BSP自动出票失败:" + pm.recvData;
                                                    }
                                                    else
                                                    {
                                                        if (pm.recvData.ToUpper().Contains("请输入证件信息"))
                                                        {
                                                            bspResult.Msg = string.Format("编码{0} 出票Office:{1},编码状态为{2},【编码中没有证件号,请输入证件信息,否则不能出票！】,返回数据:{3}",
                                                                Pnr, pm.Office, PnrInfo.PnrStatus, pm.recvData.ToUpper()
                                                                );
                                                        }
                                                        else if (pm.recvData.ToUpper().Contains("PLEASE CHECK TKT ELEMENT"))
                                                        {
                                                            bspResult.Msg = string.Format("编码{0} 出票Office:{1},编码状态为{2},【请核对您的机票信息,或者出票时限】,返回数据:{3}",
                                                               Pnr, pm.Office, PnrInfo.PnrStatus, pm.recvData.ToUpper()
                                                               );
                                                        }
                                                        else if (pm.recvData.ToUpper().Contains("STOCK"))
                                                        {
                                                            bspResult.Msg = string.Format("编码{0} 出票Office:{1},编码状态为{2},【没有票号了】,返回数据:{3}",
                                                              Pnr, pm.Office, PnrInfo.PnrStatus, pm.recvData.ToUpper()
                                                              );
                                                        }
                                                        else if (pm.recvData.ToUpper().Contains("INACTIVE"))
                                                        {
                                                            bspResult.Msg = string.Format("编码{0} 出票Office:{1},打票机号{2}状态错误，【未建控或者打票机各项状态错误】,返回数据:{3}",
                                                                Pnr, pm.Office, bspParam.PrintNo, pm.recvData.ToUpper()
                                                                );
                                                        }
                                                        else if (pm.recvData.ToUpper().Contains("DEVICE"))
                                                        {
                                                            bspResult.Msg = string.Format("编码{0} 出票Office:{1},打票机号{2}不存在,【请检查打票机号设置是否正确】,返回数据:{3}",
                                                                Pnr, pm.Office, bspParam.PrintNo, pm.recvData.ToUpper()
                                                                );
                                                        }
                                                        else if (pm.recvData.ToUpper().Contains("INCOMPLETE PNR/FN"))
                                                        {
                                                            bspResult.Msg = string.Format("编码{0} 出票Office:{1},编码状态为{2},【编码中出票价格不存在或者未补全】,返回数据:{3}",
                                                            Pnr, pm.Office, PnrInfo.PnrStatus, pm.recvData
                                                            );
                                                        }
                                                        else
                                                        {
                                                            #region 查看是否成功
                                                            if (pm.recvData.ToUpper().Contains("CNY") && pm.recvData.ToUpper().ToUpper().Contains(Pnr.ToUpper()))
                                                            {
                                                                //出票成功
                                                                //发送指令
                                                                pm.code = string.Format("RT{0}", Pnr);
                                                                pm.IsPn = true;

                                                                sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                                pm.recvData = SendNewPID.SendCommand(pm);
                                                                sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                                                                //重发
                                                                if (ISReSendData(pm.recvData))
                                                                {
                                                                    pm.code = string.Format("RT{0}", Pnr);
                                                                    pm.IsPn = true;

                                                                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                                    pm.recvData = SendNewPID.SendCommand(pm);
                                                                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                                                                }
                                                                //取票号
                                                                PnrInfo = pnrFormat.GetPNRInfo(Pnr, pm.recvData, false, out Msg);
                                                                if (PnrInfo._TicketNumList.Count > 0)
                                                                {
                                                                    foreach (PnrAnalysis.Model.TicketNumInfo item in PnrInfo._TicketNumList)
                                                                    {
                                                                        bspResult.BspResult.Add(item.PasName, item.TicketNum);
                                                                        sbLog.AppendFormat("乘客:{0} 票号:{1}\r\n", item.PasName, item.TicketNum);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    bspResult.Msg = string.Format("编码{0} 出票Office:{1},出票失败,未获取到票号信息!", Pnr, pm.Office);
                                                                }
                                                            }
                                                            else
                                                            {
                                                                #region   //出票失败 重试指令

                                                                if (!pm.recvData.ToUpper().Contains("**ELECTRONIC TICKET PNR**") &&
                                                                    pm.recvData.ToUpper().Contains("SSR TKNE") &&
                                                                    pm.recvData.ToUpper().Contains("/DPN") &&
                                                                    pm.recvData.ToUpper().Contains("RMK " + bspParam.CarrayCode + "/"))
                                                                {
                                                                    pm.code = "RT" + Pnr + "|ETRY:";
                                                                    pm.IsPn = false;

                                                                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                                    pm.recvData = SendNewPID.SendCommand(pm);
                                                                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);


                                                                    //发送指令
                                                                    pm.code = string.Format("RT{0}", Pnr);
                                                                    pm.IsPn = true;

                                                                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                                                                    pm.recvData = SendNewPID.SendCommand(pm);
                                                                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                                                                    //取票号
                                                                    PnrInfo = pnrFormat.GetPNRInfo(Pnr, pm.recvData, false, out Msg);
                                                                    if (PnrInfo._TicketNumList.Count > 0)
                                                                    {
                                                                        foreach (PnrAnalysis.Model.TicketNumInfo item in PnrInfo._TicketNumList)
                                                                        {
                                                                            bspResult.BspResult.Add(item.PasName, item.TicketNum);
                                                                            sbLog.AppendFormat("乘客:{0} 票号:{1}\r\n", item.PasName, item.TicketNum);
                                                                        }
                                                                    }
                                                                    else
                                                                    {
                                                                        bspResult.Msg = string.Format("编码{0} 出票Office:{1},重试出票失败,为获取到票号信息!", Pnr, pm.Office);
                                                                    }
                                                                }
                                                                else
                                                                {
                                                                    bspResult.Msg = string.Format("编码{0} 出票Office:{1},出票失败,未获取到票号信息!!!", Pnr, pm.Office);
                                                                }
                                                                #endregion
                                                            }
                                                            #endregion
                                                        }
                                                    }
                                                }
                                            }

                                        }
                                        else
                                        {
                                            bspResult.Msg = string.Format("编码:{0},PAT出来的价格中不存在出票的价格:{1}！", Pnr, bspParam.CpPrice);
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
                bspResult.Msg = ex.Message;
                sbLog.Append("异常信息:" + ex.Message + "\r\n");
            }
            finally
            {
                if (!string.IsNullOrEmpty(bspResult.Msg))
                {
                    sbLog.Append(bspResult.Msg + "\r\n");
                }
                sbLog.Append("END=====================================\r\n\r\n");
                LogText.LogWrite(sbLog.ToString(), "BSPAutoIssue");
            }
            return bspResult;
        }
        int RTCount = 0;
        private void XeSFC(FormatPNR pnrFormat, StringBuilder sbLog, ParamObject pm, string Pnr, out bool IsFindXe)
        {
            string strMsg = "";
            //sfc价格时所要叉掉的项列表
            List<string> xeSFCList = pnrFormat.GetXeSFCNumber(pm.recvData);
            if (xeSFCList.Count > 0)
            {
                IsFindXe = true;
            }
            else
            {
                IsFindXe = false;
            }
            if (RTCount < 5) RTCount++; else return;
            for (int i = 0; i < xeSFCList.Count; i++)
            {
                if (!string.IsNullOrEmpty(xeSFCList[i]))
                {
                    //发送指令
                    pm.code = string.Format("RT{0}|XE{1}|@", Pnr, xeSFCList[i].Trim());
                    pm.IsPn = false;

                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                    pm.recvData = SendNewPID.SendCommand(pm);
                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                    if (!pnrFormat.INFMarkIsOK(pm.recvData, out strMsg))
                    {
                        pm.code = string.Format("RT{0}|PN|XE{1}|@", Pnr, xeSFCList[i].Trim());
                        pm.IsPn = false;

                        sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                        pm.recvData = SendNewPID.SendCommand(pm);
                        sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);
                    }
                    //发送指令
                    pm.code = string.Format("RT{0}", Pnr);
                    pm.IsPn = true;

                    sbLog.AppendFormat("【{0}】\r\n发送:{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.code);
                    pm.recvData = SendNewPID.SendCommand(pm);
                    sbLog.AppendFormat("【{0}】\r\n接收:\r\n{1}\r\n", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"), pm.recvData);

                    XeSFC(pnrFormat, sbLog, pm, Pnr, out IsFindXe);
                    break;
                }
            }
        }
        #endregion
    }
}

