using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;

namespace PnrAnalysis
{
    public class ECParam
    {
        public string PID = string.Empty;
        public string KeyNo = string.Empty;
        public string UserName = string.Empty;
        public string PassWord = string.Empty;
        public string ECIP = string.Empty;
        public string ECPort = "450";
        public int TimeOut = 120 * 1000;
    }

    /// <summary>
    /// 老版本客户端发送指令类
    /// </summary>
    public class SendEC
    {
        #region 构造函数
        private ECParam __ECParam;
        public SendEC(ECParam _ECParam)
        {
            __ECParam = _ECParam;
        }
        #endregion 构造函数

        #region 属性
        /// <summary>
        /// 超时时间
        /// </summary>
        private int _timeOut;

        private int TimeOut
        {
            get { return _timeOut; }
            set { _timeOut = value; }
        }
        private const int Len = 1024 * 2;
        private byte[] byteBuffer = new byte[Len];

        private Socket _skt;
        /// <summary>
        /// 客户端Socket
        /// </summary>
        private Socket Skt
        {
            get { return _skt; }
            set { _skt = value; }
        }

        private IPEndPoint _IPEndPoint;
        /// <summary>
        /// 客户端IPEndPoint
        /// </summary>
        private IPEndPoint IPEndPoint
        {
            get { return _IPEndPoint; }
            set { _IPEndPoint = value; }
        }

        #endregion


        #region 方法
        private bool IsCorrenctIP(string ip)
        {
            string pattrn = @"(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])";
            if (System.Text.RegularExpressions.Regex.IsMatch(ip, pattrn))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private bool Isdigst(string con)
        {
            bool a = true;
            for (int i = 0; i < con.Length; i++)
            {
                if (!char.IsDigit(con[i]))
                {
                    a = false;
                }
            }
            return a;
        }

        /// <summary>
        /// 关闭连接
        /// </summary>
        /// 
        private void CloseServer()
        {
            if (this.Skt != null || this.Skt.Connected)
            {
                this.Skt.Shutdown(SocketShutdown.Both);
                this.Skt.Close();
                this.Skt = null;
            }
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="instruction">指令</param>
        ///  <param name="instruction">指令后缀</param>
        ///   <param name="instruction">owner</param>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        /// <param name="timeout">超时（毫秒）</param>
        ///  <param name="ErrMsg">返回错误信息ErrMsg</param>
        /// <returns></returns>
        public string SendData(string Instruction, out string ErrMsg)
        {
            string err, ResponseContent = "";
            ErrMsg = "";

            //用于发指令
            //md5^用户名@#%_#tick@#%_#内容^*%#!@Pid^*%#!@keyNo
            //md5 = 用户名@#%_#tick@#%_#内容;$@!;Pid;$@!;keyNo   ;$@!; 分割
            //原来
            Instruction = __ECParam.UserName + "@#%_#" + System.DateTime.Now.Ticks.ToString() + "@#%_#" + Instruction + ";$@!;" + this.__ECParam.PID + ";$@!;" + this.__ECParam.KeyNo;
            Instruction = md5(Instruction) + "^" + Instruction;
            ResponseContent = SendData(Instruction, this.__ECParam, out err);
            ErrMsg = err;
            return ResponseContent;
        }
        /// <summary>
        /// 发送
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <param name="instruction">指令</param>
        /// <param name="ip">ip</param>
        /// <param name="port">port</param>
        /// <param name="timeout">超时（毫秒）</param>
        ///  <param name="ErrMsg">返回错误信息ErrMsg</param>
        /// <returns></returns>
        private string SendData(string instruction, ECParam param, out string ErrMsg)
        {
            //初始化
            ErrMsg = "";
            //接收带格式的所有数据
            string content = "";
            //去掉格式后的信息数据
            string returnRevice = "";
            //发送指令
            string send = instruction;
            if (string.IsNullOrEmpty(param.ECIP))
            {
                ErrMsg = "ip地址为空";
                return "";
            }
            if (!IsCorrenctIP(param.ECIP))
            {
                ErrMsg = "ip地址格式错误";
                return "";
            }
            if (string.IsNullOrEmpty(param.ECPort))
            {
                ErrMsg = "端口号为空";
                return "";
            }
            if (!Isdigst(param.ECPort.ToString()))
            {
                ErrMsg = "端口号格式错误";
                return "";
            }
            try
            {
                try
                {
                    //this.IPEndPoint = new IPEndPoint(IPAddress.Parse("192.168.1.225"), 456);
                    this.IPEndPoint = new IPEndPoint(IPAddress.Parse(param.ECIP), int.Parse(param.ECPort));
                    this.Skt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                }
                catch (Exception ep)
                {
                    ErrMsg = "【连接失败】" + ep.Message;
                    return returnRevice;
                }
                //超时设置
                this.Skt.ReceiveTimeout = param.TimeOut;
                this.Skt.SendTimeout = param.TimeOut;
                try
                {
                    //连接服务器
                    this.Skt.Connect(this.IPEndPoint);
                }
                catch (SocketException ep)
                {
                    ErrMsg = ep.Message;
                    returnRevice = "【连接超时】";
                }
                if (this.Skt.Connected)
                {
                    try
                    {
                        #region //发送
                        byte[] sendbyte = Encoding.Default.GetBytes(send);

                        this.Skt.Send(sendbyte, 0, sendbyte.Length, SocketFlags.None);
                        #endregion

                        try
                        {
                            #region //接收数据
                            int AvailLen = this.Skt.Receive(this.byteBuffer, 0, Len, SocketFlags.None);
                            content = string.Concat(content, Encoding.Default.GetString(this.byteBuffer, 0, AvailLen));
                            //一次为读完下一次循环
                            while (true)
                            {
                                if (this.Skt.Available != 0)
                                {
                                    AvailLen = this.Skt.Receive(this.byteBuffer, 0, Len, SocketFlags.None);
                                    content = string.Concat(content, Encoding.Default.GetString(this.byteBuffer, 0, AvailLen));
                                }
                                else
                                {
                                    break;
                                }
                            }
                            #endregion
                        }
                        catch (SocketException ers)
                        {
                            ErrMsg = ers.Message;
                            returnRevice = "【发送超时】";
                            CloseServer();
                            return returnRevice;
                        }
                        if (content == "")
                        {
                            return content;
                        }

                        #region //接收完数据后处理数据
                        string[] str = null;
                        try
                        {
                            //第一次拆分
                            str = content.Split(new string[] { "^" }, StringSplitOptions.None);
                        }
                        catch (OutOfMemoryException ep)
                        {
                            ErrMsg = ep.Message;
                            returnRevice = ep.Message;
                            CloseServer();
                            return returnRevice;
                        }
                        if (str.Length > 0)
                        {
                            string tmpInfo = "";

                            string tmpmd5 = str[0];

                            string[] desTo = new string[str.Length - 1];
                            Array.Copy(str, 1, desTo, 0, str.Length - 1);
                            tmpInfo = string.Join("^", desTo);

                            string reback = md5(tmpInfo);

                            //匹配执行相应的命令
                            if (tmpmd5 == reback)
                            {
                                //第二次拆分
                                str = tmpInfo.Split(new string[] { "@#%_#" }, StringSplitOptions.None);
                                //取消息
                                returnRevice = str[str.Length - 1];
                                //关闭连接
                                CloseServer();
                            }
                        }
                        #endregion
                    }
                    catch (IOException errs)
                    {
                        ErrMsg = errs.Message;
                        returnRevice = errs.Message;
                        CloseServer();
                        return returnRevice;
                    }
                    catch (ArgumentOutOfRangeException ers)
                    {
                        ErrMsg = ers.Message;
                        returnRevice = ers.Message;
                        CloseServer();
                        return returnRevice;
                    }

                    catch (Exception esc)
                    {
                        ErrMsg = esc.Message;
                        returnRevice = esc.Message;
                        CloseServer();
                        return returnRevice;
                    }
                }
            }
            catch (Exception ep)
            {
                ErrMsg = "【连接失败】" + ep.Message;
                returnRevice = ep.Message;
                return returnRevice;
            }

            return returnRevice;
        }

        //加密
        public String md5(String str)
        {
            //key
            string md5 = "";
            md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5");
            md5 = "YYxhjPO)768_=" + md5 + "78HtPloaQQdvc";
            md5 = System.Web.Security.FormsAuthentication.HashPasswordForStoringInConfigFile(md5, "md5");
            return md5;
        }
        #endregion
    }
}
