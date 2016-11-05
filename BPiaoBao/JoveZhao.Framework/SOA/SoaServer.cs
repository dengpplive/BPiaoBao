using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net.Sockets;
using JoveZhao.Framework.Expand;
using System.Configuration;
using System.Net;

namespace JoveZhao.Framework.SOA
{
    /// <summary>
    /// 服务侦听
    /// </summary>
    public class SoaServer
    {
        ServerConfigurationElement configSection;
        List<ProcessThread> linkList = new List<ProcessThread>();

        public SoaServer()
        {
            configSection = JZFSection.GetInstances().Soa.Server;
        }
        


        volatile bool serverState = false;
        /// <summary>
        /// 启动侦听
        /// </summary>
        public void Start()
        {
            serverState = true;
            
            Action l = listen;
            l.BeginInvoke(null, null);
            
        }

        void listen()
        {
            var tcplistener =new TcpListener(IPAddress.Any,configSection.Connection.Port);
            tcplistener.Start();
            Logger.WriteLog(LogType.INFO, "启动侦听");
            while (serverState)
            {
                var tcpClient = tcplistener.AcceptTcpClient();

                Logger.WriteLog(LogType.INFO, "接收到连接");
                ProcessThread process = new ProcessThread(tcpClient, configSection);
                linkList.Add(process);
                process.Start();
            }
            tcplistener.Stop();
            Logger.WriteLog(LogType.INFO, "关闭侦听");
        }
        /// <summary>
        /// 停止侦听
        /// </summary>
        public void Stop()
        {
            serverState = false;
            foreach (var thread in linkList)
            {
                thread.Stop();
            }
        }
    }
}
