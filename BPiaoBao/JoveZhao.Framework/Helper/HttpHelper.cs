using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace JoveZhao.Framework.Helper
{
    public enum HttpMethod
    {
        GET,
        POST
    }
    public class HttpHelper
    {
        public static void Ajax(string url, HttpMethod method, string data, Action<string> successCallback, Action<string> errorCallback)
        {

            Action act = () =>
            {
                try
                {
                    string result;
                    if (method == HttpMethod.POST)
                        result = Post(url, data);
                    else
                        result = Get(url);
                    successCallback(result);
                }
                catch (Exception ex)
                {
                    errorCallback(ex.ToString());
                }
            };
            act.BeginInvoke(null, null);

        }


        public static string Post(string url, string data,int timeOut=60000)
        {
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Timeout = timeOut;
            request.Accept = "text/html, application/xhtml+xml, */*"; //接受文件   
            request.UserAgent = " Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; EmbeddedWB 14.52 from: http://www.bsalsa.com/ EmbeddedWB 14.52; .NET CLR 2.0.50727)"; // 模拟使用IE在浏览 
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            //request.KeepAlive = true;
            request.Method ="POST";
            byte[] b = Encoding.UTF8.GetBytes(data);
            request.ContentLength = b.Length;
            Stream sw = request.GetRequestStream();
            sw.Write(b, 0, b.Length);
            sw.Flush();
            var r = request.GetResponse();
            var p = r.GetResponseStream();
            StreamReader reader = new StreamReader(p, Encoding.UTF8);
            var result = reader.ReadToEnd();
            sw.Close();
            p.Close();
            return result;
        }

        public static string Get(string url, int timeOut = 60000)
        {
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Timeout = timeOut;
            request.Accept = "text/html, application/xhtml+xml, */*"; //接受文件   
            request.UserAgent = " Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; EmbeddedWB 14.52 from: http://www.bsalsa.com/ EmbeddedWB 14.52; .NET CLR 2.0.50727)"; // 模拟使用IE在浏览 
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            //request.KeepAlive = true;
            request.Method = "GET";
            
            var r = request.GetResponse();
            var p = r.GetResponseStream();
            StreamReader reader = new StreamReader(p, Encoding.UTF8);
            var result = reader.ReadToEnd();
            p.Close();
            return result;
        }
        public static string Get(string url, int timeOut ,Encoding econding)
        {
            var request = HttpWebRequest.Create(url) as HttpWebRequest;
            request.Timeout = timeOut;
            request.Accept = "text/html, application/xhtml+xml, */*"; //接受文件   
            request.UserAgent = " Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; EmbeddedWB 14.52 from: http://www.bsalsa.com/ EmbeddedWB 14.52; .NET CLR 2.0.50727)"; // 模拟使用IE在浏览 
            request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
            //request.KeepAlive = true;
            request.Method = "GET";

            var r = request.GetResponse();
            var p = r.GetResponseStream();
            StreamReader reader = new StreamReader(p, econding);
            var result = reader.ReadToEnd();
            p.Close();
            return result;
        }
    }
}
