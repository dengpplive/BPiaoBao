using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;
using System.Xml;

namespace JoveZhao.Framework.SMS
{

    public class SMSDefaultService : ISMSService
    {

        public Tuple<int, bool> SMS(string to, string message)
        {
            if (string.IsNullOrEmpty(message))
                return Tuple.Create<int, bool>(0, false);
            //得到发送条数
            int sendCount = 0;
            bool sendState = false;
            #region old
            //string url = "http://122.4.79.43:2852/sms.aspx";
            //string data = "action=send&userid=1190&account=mypb&password=mypb2014&mobile=" + to + "&content=" + message + "&sendTime=&checkcontent=1";
            //string rs = GetBackData(url, data, "POST");

            //XmlDocument xd = new XmlDocument();
            //xd.LoadXml(rs);
            //string state = xd.SelectSingleNode("returnsms/returnstatus/text()").Value;
            //string count = xd.SelectSingleNode("returnsms/successCounts/text()").Value;
            //string info = xd.SelectSingleNode("returnsms/message/text()").Value;
            #endregion
            int state = SendSms(to, message);

            if (state > 0)
            {
                sendState = true;
                sendCount = Convert.ToInt32(Math.Ceiling((double)message.Length / 64));
            }
            else
            {
                //未完成 
                Logger.WriteLog(LogType.INFO, "[" + DateTime.Now.ToString() + "]发送失败[" + to + "]");
            }
            return Tuple.Create<int, bool>(sendCount, sendState);
        }
        public string GetBackData(string m_baseurl, string Data, string m_doMetthod)
        {
            string rs = string.Empty;
            try
            {
                byte[] dt = Encoding.UTF8.GetBytes(Data);
                if (m_doMetthod == "GET")
                {
                    if (!string.IsNullOrEmpty(Data))
                        m_baseurl = m_baseurl + "?" + Data;
                }
                Uri uRI = new Uri(m_baseurl);
                HttpWebRequest req = WebRequest.Create(uRI) as HttpWebRequest;


                req.Method = m_doMetthod;
                if (m_doMetthod != "GET")
                {
                    req.KeepAlive = true;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = dt.Length;
                    req.AllowAutoRedirect = true;
                    Stream outStream = req.GetRequestStream();
                    outStream.Write(dt, 0, dt.Length);
                    outStream.Close();
                }

                HttpWebResponse res = req.GetResponse() as HttpWebResponse;

                Stream inStream = res.GetResponseStream();
                StreamReader sr = new StreamReader(inStream, Encoding.UTF8);
                rs = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.Message, ex);
            }
            return rs;
        }

        public int SendSms(string to, string message)
        {
            try
            {

                SMSService.Service1SoapClient sms = new SMSService.Service1SoapClient();
                return sms.FastSendSMS(to, message);
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "短信发送失败", e);
                throw new CustomException(500, "发送消息失败");
            }
        }
    }
}
