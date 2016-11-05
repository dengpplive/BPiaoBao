using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using JoveZhao.Framework;

namespace BPiaoBao.Common
{
    public class HttpWebRequestHelper
    {
        /// <summary>
        /// Get方法
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public static string Get(string strUrl)
        {
            try
            {

                HttpWebRequest requestScore = (HttpWebRequest)WebRequest.Create(strUrl);
                HttpWebResponse responseSorce = (HttpWebResponse)requestScore.GetResponse();
                StreamReader reader = new StreamReader(responseSorce.GetResponseStream(), Encoding.GetEncoding("GBK"));
                string content = reader.ReadToEnd();
                reader.Close();
                responseSorce.Close();
                return content;
            }
            catch (Exception e)
            {
                throw new CustomException(100001, e.Message);
            }
        }

        /// <summary>
        /// Post方法
        /// </summary>
        /// <param name="strUrl"></param>
        /// <param name="postData"></param>
        /// <returns></returns>
        public static string Post(string strUrl, string postData)
        {
            try
            {

                HttpWebRequest requestScore = (HttpWebRequest)WebRequest.Create(strUrl);
                byte[] data = Encoding.GetEncoding("GBK").GetBytes(postData);
                requestScore.Method = "Post";
                requestScore.ContentType = "application/x-www-form-urlencoded";
                requestScore.ContentLength = data.Length;
                requestScore.KeepAlive = true;

                //使用cookies
                //requestScore.CookieContainer = ...;
                Stream stream = requestScore.GetRequestStream();
                stream.Write(data, 0, data.Length);
                HttpWebResponse responseSorce = (HttpWebResponse)requestScore.GetResponse();
                StreamReader reader = new StreamReader(responseSorce.GetResponseStream(), Encoding.GetEncoding("GBK"));
                string content = reader.ReadToEnd();
                stream.Close();
                reader.Close();
                responseSorce.Close();
                return content;
            }
            catch (Exception e)
            {
                //throw new CustomException(100001, e.Message);
                Logger.WriteLog(LogType.ERROR, "Request Exception(url--->"+strUrl+"  data--->"+postData+") "+ e.Message);
                return "";
            }
        }

        /// <summary>
        /// HTML标签过滤
        /// </summary>
        /// <param name="html"></param>
        /// <param name="lable"></param>
        /// <returns></returns>
        public static string CheckStr(string html, string[] lable)
        {
            return lable.Select(lb => String.Format(@"</?{0}[^>]*?>", lb)).Aggregate(html, (current, reg) => System.Text.RegularExpressions.Regex.Replace(current, reg, "", System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }
    }
}
