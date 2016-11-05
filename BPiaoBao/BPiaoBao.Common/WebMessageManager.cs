using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using Microsoft.AspNet.SignalR.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.Common
{
    public class WebMessageManager
    {
        private static WebMessageManager _webMessage = null;

        private IHubProxy clientHub;
        private string url = System.Configuration.ConfigurationManager.AppSettings["WebMsgUrl"];
        private WebMessageManager()
        {
            var connection = new HubConnection(url);
            clientHub = connection.CreateHubProxy("MessageRemind");
            connection.Start().ContinueWith(t =>
            {
                if (t.IsFaulted)
                    Logger.WriteLog(LogType.ERROR, "web消息连接错误" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), t.Exception);
            });
            System.Threading.Thread.Sleep(2000);
        }
        public static WebMessageManager GetInstance()
        {
            if (_webMessage == null)
                _webMessage = new WebMessageManager();
            return _webMessage;
        }
        /// <summary>
        /// 发送给指定的商户号
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="code">商户号</param>
        /// <param name="message">消息内容</param>
        public void Send(EnumMessageCommand command, string code, string message)
        {
            try
            {
                Logger.WriteLog(LogType.ERROR, string.Format("Command:{0},Code:{1},Message:{2}", command, code, message));
                clientHub.Invoke("sendGroupMessage", command, command.ToEnumDesc(), message, code).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        Logger.WriteLog(LogType.ERROR, "web发送消息错误" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), t.Exception);
                });
            }
            catch (Exception e)
            {

                Logger.WriteLog(LogType.ERROR, "web发送消息异常" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), e);
            }

        }
        /// <summary>
        /// 发送给指定商户号组
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="codes">商户号组</param>
        /// <param name="message">消息</param>
        public void Send(EnumMessageCommand command, string[] codes, string message)
        {
            try
            {
                clientHub.Invoke("sendGroupList", command, command.ToEnumDesc(), message, codes).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        Logger.WriteLog(LogType.ERROR, "web发送消息错误" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), t.Exception);
                });
            }
            catch (Exception e)
            {

                Logger.WriteLog(LogType.ERROR, "web发送消息异常" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), e);
            }

        }
        /// <summary>
        /// 发送给所有商户
        /// </summary>
        /// <param name="command">命令</param>
        /// <param name="message">消息</param>
        public void Send(EnumMessageCommand command, string message)
        {
            try
            {
                clientHub.Invoke("sendAllMessage", command, command.ToEnumDesc(), message).ContinueWith(t =>
                {
                    if (t.IsFaulted)
                        Logger.WriteLog(LogType.ERROR, "web发送消息错误" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), t.Exception);
                });
            }
            catch (Exception e)
            {

                Logger.WriteLog(LogType.ERROR, "web发送消息异常" + DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss fff"), e);
            }

        }
    }
}
