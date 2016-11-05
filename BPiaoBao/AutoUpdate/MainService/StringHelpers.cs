using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace AutoUpdate.MainService
{
    public static class StringHelpers
    {
        public static string Get(this string str)
        {
            if (string.IsNullOrEmpty(str))
                return string.Empty;
            try
            {
                var request = HttpWebRequest.Create(str) as HttpWebRequest;
                request.Accept = "text/html, application/xhtml+xml, */*"; //接受文件   
                request.UserAgent = " Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; EmbeddedWB 14.52 from: http://www.bsalsa.com/ EmbeddedWB 14.52; .NET CLR 2.0.50727)"; // 模拟使用IE在浏览 
                request.UserAgent = "Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; Trident/5.0)";
                request.Method = "GET";

                var r = request.GetResponse();
                var p = r.GetResponseStream();
                StreamReader reader = new StreamReader(p, Encoding.UTF8);
                var result = reader.ReadToEnd();
                p.Close();
                return result;
            }
            catch (Exception)
            {

            }
            return string.Empty;
        }
    }
}
