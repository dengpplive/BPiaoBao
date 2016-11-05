using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Text;
using JoveZhao.Framework.Expand;
namespace JoveZhao.Framework.Helper
{
    public enum MethodType : int
    {
        [Description("GET请求")]
        GET = 0,
        [Description("POST请求")]
        POST = 1
    }
    public enum ResponseStatus : int
    {
        [Description("成功")]
        OK = 0,
        [Description("失败")]
        Fail = 1
    }
    public class DataResponse
    {
        public string Data = string.Empty;
        public HttpStatusCode Status = HttpStatusCode.Continue;
    }
    public class WebHttp
    {
        /// <summary>
        /// 发送请求数据
        /// </summary>
        /// <param name="target">请求Url</param>
        /// <param name="method">GET/POST</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeOut">请求超时（秒）</param>
        /// <param name="parameters">请求参数key=value</param>
        /// <returns></returns>
        public DataResponse SendRequest(string target, MethodType method, System.Text.Encoding encoding, int timeOut, params string[] parameters)
        {
            HttpWebResponse response = null;
            DataResponse dataResponse = new DataResponse();
            HttpWebRequest request = null;
            Stream responseStream = null;
            string result = string.Empty;
            try
            {
                List<string> paramList = new List<string>();
                if ((parameters != null) && (parameters.Length >= 1))
                {
                    paramList.AddRange(parameters);
                }
                if (target.Contains("?"))
                {
                    string strQuery = target.Substring(target.IndexOf("?") + 1);
                    string[] queryArr = strQuery.Split(new string[] { "&" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string item in queryArr)
                    {
                        if (!paramList.Contains(item))
                        {
                            paramList.Add(item);
                        }
                    }
                }
                string str = "";
                if (paramList.Count > 0)
                {
                    str = string.Join("&", paramList.ToArray());
                }
                byte[] bytes = encoding.GetBytes(str);
                if (method == MethodType.GET)
                {
                    if (target.Contains("?"))
                    {
                        target = target.Substring(0, target.IndexOf("?")).Trim(new char[] { '&' }) + "?" + str;
                    }
                    else
                    {
                        target = target.Trim(new char[] { '&' }) + "?" + str;
                    }
                }
                string requestUriString = target;
                request = (HttpWebRequest)WebRequest.Create(requestUriString);
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.CachePolicy = policy;
                request.Timeout = timeOut * 0x3e8;
                request.KeepAlive = false;
                request.Method = method.ToString().ToUpper();
                bool flag = false;
                flag = request.Method.ToUpper() == "POST";
                if (flag)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bytes.Length;
                }
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)";
                request.Headers.Add("Cache-Control", "no-cache");
                request.Accept = "*/*";
                request.Credentials = CredentialCache.DefaultCredentials;
                if (flag)
                {
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                List<byte> list = new List<byte>();
                for (int i = responseStream.ReadByte(); i != -1; i = responseStream.ReadByte())
                {
                    list.Add((byte)i);
                }
                Stream stream2 = new MemoryStream(list.ToArray());
                StreamReader sr = new StreamReader(stream2, encoding);
                result = sr.ReadToEnd();
                sr.Close();
                stream2.Close();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    dataResponse.Status = response.StatusCode;
                }
            }
            catch (WebException ex)
            {
                result = ex.Message;
            }
            catch (Exception ex1)
            {
                result = ex1.Message;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
                dataResponse.Data = result;
            }
            return dataResponse;
        }

        /// <summary>
        /// 发送请求数据
        /// </summary>
        /// <param name="target">请求Url</param>
        /// <param name="method">GET/POST</param>
        /// <param name="encoding">编码</param>
        /// <param name="timeOut">请求超时（秒）</param>
        /// <param name="parameters">请求参数key=value</param>
        /// <returns></returns>
        public string SendRequest(string target, string method, System.Text.Encoding encoding, int timeOut, ref bool Status, params string[] parameters)
        {
            HttpWebResponse response = null;
            HttpWebRequest request = null;
            Stream responseStream = null;
            string result = string.Empty;
            Status = false;
            try
            {
                string str = "";
                if ((parameters != null) && (parameters.Length >= 1))
                {
                    str = string.Join("&", parameters);
                }
                byte[] bytes = encoding.GetBytes(str);
                string requestUriString = target;
                request = (HttpWebRequest)WebRequest.Create(requestUriString);
                HttpRequestCachePolicy policy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                request.CachePolicy = policy;
                request.Timeout = timeOut * 0x3e8;
                request.KeepAlive = false;
                request.Method = method.ToString().ToUpper();
                bool flag = false;
                flag = request.Method.ToUpper() == "POST";
                if (flag)
                {
                    request.ContentType = "application/x-www-form-urlencoded";
                    request.ContentLength = bytes.Length;
                }
                else
                {
                    if (target.Contains("?"))
                    {
                        target = target.Trim(new char[] { '&' }) + "&" + str;
                    }
                    else
                    {
                        target = target.Trim(new char[] { '?' }) + "?" + str;
                    }
                }
                request.UserAgent = "Mozilla/4.0 (compatible; MSIE 7.0; Windows NT 5.1; SV1; .NET CLR 2.0.1124)";
                request.Headers.Add("Cache-Control", "no-cache");
                request.Accept = "*/*";
                request.Credentials = CredentialCache.DefaultCredentials;
                if (flag)
                {
                    Stream requestStream = request.GetRequestStream();
                    requestStream.Write(bytes, 0, bytes.Length);
                    requestStream.Close();
                }
                response = (HttpWebResponse)request.GetResponse();
                responseStream = response.GetResponseStream();
                List<byte> list = new List<byte>();
                for (int i = responseStream.ReadByte(); i != -1; i = responseStream.ReadByte())
                {
                    list.Add((byte)i);
                }
                Stream stream2 = new MemoryStream(list.ToArray());
                StreamReader sr = new StreamReader(stream2, encoding);
                result = sr.ReadToEnd();
                sr.Close();
                stream2.Close();
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    Status = true;
                }
            }
            catch (WebException ex)
            {
                result = ex.Message;
            }
            catch (Exception ex1)
            {
                result = ex1.Message;
            }
            finally
            {
                if (request != null)
                {
                    request.Abort();
                }
                if (responseStream != null)
                {
                    responseStream.Close();
                }
                if (response != null)
                {
                    response.Close();
                }
            }
            return result;
        }
    }
}
