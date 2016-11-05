using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using JoveZhao.Framework;

namespace BPiaoBao.DomesticTicket.Domain.Services
{
    public class CommLog
    {
        //记录日志
        public void WriteLog(string Method, string Content)
        {
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("Start====================================================================\r\n");
            sbLog.Append("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n");
            try
            {
                OperationContext context = OperationContext.Current;
                //获取传进的消息属性
                MessageProperties properties = context.IncomingMessageProperties;
                //获取消息发送的远程终结点IP和端口
                RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                if (endpoint != null)
                {
                    string strIp = endpoint.Address + ":" + endpoint.Port;
                    sbLog.AppendFormat("客户端IP:{0}\r\n", strIp);
                }
            }
            catch
            {
            }
            sbLog.AppendFormat("调用方法名:{0}\r\n", Method);
            sbLog.AppendFormat("内容:{0}\r\n", Content);
            sbLog.Append("End=====================================================================\r\n\r\n");
            //Logger.WriteLog(LogType.INFO, sbLog.ToString());
            PnrAnalysis.LogText.LogWrite(sbLog.ToString(), Method);
        }
    }
}
