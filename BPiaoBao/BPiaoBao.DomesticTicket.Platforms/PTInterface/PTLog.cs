using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platforms.PTInterface
{
    public class PTLog
    {
        public static long maxFileSize = 10 * 1024 * 1024;//默认10M
        [MethodImpl(MethodImplOptions.Synchronized)]
        public static void LogWrite(string content, string dir)
        {
            if (!string.IsNullOrEmpty(content))
            {
                string fileNamePath = string.Empty;
                StringBuilder sbLog = new StringBuilder();
                sbLog.Append("Start====================================================================\r\n");
                sbLog.Append("记录时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n");
                try
                {
                    OperationContext context = OperationContext.Current;
                    if (context != null)
                    {
                        //获取传进的消息属性
                        MessageProperties properties = context.IncomingMessageProperties;
                        //获取消息发送的远程终结点IP和端口
                        RemoteEndpointMessageProperty endpoint = properties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
                        if (endpoint != null)
                        {
                            string strIp = endpoint.Address + ":" + endpoint.Port;
                            sbLog.AppendFormat("访问IP:{0}\r\n", strIp);
                        }
                    }
                }
                catch
                {
                }
                sbLog.AppendFormat("内容:{0}\r\n", content);
                sbLog.Append("End=====================================================================\r\n\r\n");
                try
                {
                    string basePath = System.AppDomain.CurrentDomain.SetupInformation.ApplicationBase;
                    if (!basePath.EndsWith("\\"))
                    {
                        basePath = string.Format(@"{0}\Log", basePath);
                    }
                    else
                    {
                        basePath = string.Format(@"{0}Log", basePath);
                    }
                    if (!string.IsNullOrEmpty(dir))
                    {
                        basePath = string.Format(@"{0}\{1}\", basePath, dir.Trim(new char[] { '\\' }));
                    }
                    if (!Directory.Exists(basePath))
                    {
                        Directory.CreateDirectory(basePath);
                    }
                    //每小时一个
                    fileNamePath = basePath + System.DateTime.Now.ToString("yyyy-MM-dd HH") + ".txt";
                    if (File.Exists(fileNamePath))
                    {
                        FileInfo fi = new FileInfo(fileNamePath);
                        if (fi.Length > maxFileSize)
                        {
                            //转为每分钟一个文件
                            fileNamePath = basePath + System.DateTime.Now.ToString("yyyy-MM-dd HHmm") + ".txt";
                        }
                    }
                    //写文件
                    File.AppendAllText(fileNamePath, sbLog.ToString());
                }
                catch (Exception ex)
                {
                    content += "\r\n写文件异常[Log]::" + ex.Message + "\r\n";
                    //写文件
                    File.AppendAllText(fileNamePath, sbLog.ToString());
                }
            }
        }
    }
}
