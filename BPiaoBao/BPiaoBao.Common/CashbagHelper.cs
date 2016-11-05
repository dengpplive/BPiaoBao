using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using JoveZhao.Framework.Expand;
using Newtonsoft.Json;
using JoveZhao.Framework;
namespace BPiaoBao.Common
{
    public class CashbagHelper
    {
        private string m_baseurl;
        private string m_doMetthod;
        public CashbagHelper()
        {

        }
        public CashbagHelper(string m_baseurl)
        {
            this.m_baseurl = m_baseurl;
        }
        public CashbagHelper(string m_baseurl, string m_doMetthod)
        {
            this.m_baseurl = m_baseurl;
            this.m_doMetthod = m_doMetthod;
        }

        
        /// <summary>
        /// 获取接口数据
        /// </summary>
        /// <param name="url"></param>
        /// <param name="Data"></param>
        /// <param name="doMethod"></param>
        /// <returns></returns>
        private string GetBackData(string Data)
        {
            try
            {
                byte[] dt = Encoding.UTF8.GetBytes(Data);
                if (m_doMetthod == "GET")
                {
                    if (!string.IsNullOrEmpty(Data))
                        m_baseurl = m_baseurl + "?" + Data;
                    //if (url.Contains("businessman/info") == false && url.Contains("funds/GetValidateCode") == false)
                    //{
                    //    PbProject.WebCommon.Log.Log.RecordLog("钱袋子接口调用字符串", url, true, System.Web.HttpContext.Current.Request);
                    //}
                }
                Uri uRI = new Uri(m_baseurl);
                PnrAnalysis.LogText.LogWrite("时间:"+System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff")+"  "+m_baseurl + "\r\n", "钱袋子接口地址");
                HttpWebRequest req = WebRequest.Create(uRI) as HttpWebRequest;

                Logger.WriteLog(LogType.DEBUG, m_baseurl);
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
                return sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                throw new CashBagPayException(ex.Message);
            }
           
        }

        public dynamic GetBackJsonData(string data)
        {
            var json = GetBackData(data);
            return JsonConvert.DeserializeObject<dynamic>(json);

        }
        /// <summary>
        /// 获取加密签名数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="token"></param>
        /// <param name="rqurl"></param>
        /// <param name="nowtime"></param>
        /// <returns></returns>
        public string GetSignaTure(List<string> list)
        {
            return string.Join("&", list.OrderByDescending(s => s)).Md5(); ;//降序排列加密
        }
        /// <summary>
        /// 获取加密签名数据
        /// </summary>
        /// <param name="code"></param>
        /// <param name="token"></param>
        /// <param name="rqurl"></param>
        /// <param name="nowtime"></param>
        /// <returns></returns>
        public string GetSignaTure(Dictionary<string, string> dictionary)
        {
            var list = dictionary.Select(p => p.Value.UrlEncode()).ToList();
            return GetSignaTure(list);
        }
        /// <summary>
        /// URL参数加码
        /// </summary>
        /// <param name="dictionary"></param>
        /// <returns></returns>
        public string ParamsURLEncode(Dictionary<string, string> dictionary)
        {
            dictionary.Add("partnerKey" ,SettingSection.GetInstances().Cashbag.PartnerKey);

            List<string> list = new List<string>();
            
            dictionary.ForEach(p => {

                if (string.Compare(p.Key,"key",true)!=0)//参数不要key
                {
                    list.Add(p.Key + "=" + p.Value.UrlEncode());
                   
                }
               
            });
            string signature =GetSignaTure(dictionary);//获取签名
            list.Add("signature=" + signature);//URL加上签名
            
            return string.Join("&", list);//组合URL参数
        }

        /// <summary>
        /// 获取编码参数(一部分用)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public string GetURLEncodeData(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count,string OutTradeNo = null)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            if (startTime.HasValue)
            {
                dictionary.Add("StartDate", startTime.Value.ToString("yyyyMMddHHmmss"));
            }
            if (endTime.HasValue)
            {
                dictionary.Add("EndDate", endTime.Value.ToString("yyyyMMddHHmmss"));
            }
            var page = Math.Ceiling((double)startIndex / count)+1;
            dictionary.Add("CurrentPage",page.ToString());
            dictionary.Add("PageSize", count.ToString());

            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            if (!string.IsNullOrWhiteSpace(OutTradeNo))
                dictionary.Add("OutTradeNo", OutTradeNo.Trim());
            return ParamsURLEncode(dictionary);
        }
    }
}
