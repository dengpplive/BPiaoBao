using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Text;
using System.IO;
using JoveZhao.Framework.Expand;

namespace JoveZhao.Framework.HttpServers
{
    public class HttpRequest
    {
        public HttpRequest(HttpListenerRequest request)
        {
            this.Url = request.Url;
            this.RawUrl = request.RawUrl;
            this.RemoteEndPoint = request.RemoteEndPoint;
            this.Form = new NameValueCollection();
            this.QueryString = new NameValueCollection();
            StreamReader sr = new StreamReader(request.InputStream);
            var formStr = sr.ReadToEnd();
            if (!string.IsNullOrEmpty(formStr))
            {
                formStr.Split('&').Select(p =>
                {
                    var ps = p.Split('=');
                    return new
                    {
                        key = ps[0],
                        value = ps[1]
                    };
                }).ForEach(p =>
                {
                    Form.Add(p.key, p.value);
                });
            }
            if (!string.IsNullOrEmpty(request.Url.Query))
            {
                var query = request.Url.Query.Substring(1, request.Url.Query.Length - 1);
                query.Split('&').Select(p =>
                {
                    var ps = p.Split('=');
                    return new
                    {
                        key = ps[0],
                        value = ps[1].UrlDecode()
                    };
                }).ForEach(p =>
                {
                    QueryString.Add(p.key, p.value);
                });
            }

        }
        public Uri Url { get; private set; }
        public NameValueCollection Form { get; private set; }
        public NameValueCollection QueryString { get; private set; }
        public IPEndPoint RemoteEndPoint { get; private set; }
        public string RawUrl { get; private set; }

    }

    public class HttpResponse : IDisposable
    {
        HttpListenerResponse response;
        StreamWriter sw;
        public HttpResponse(HttpListenerResponse response)
        {
            this.response = response;
            sw = new StreamWriter(response.OutputStream); ;
        }

        public void WriteLine(string value)
        {
            sw.WriteLine(value);
        }
        public void WriteLine(int value)
        {
            sw.WriteLine(value);
        }
        public void WriteLine(bool value)
        {
            sw.WriteLine(value);
        }
        public void Flush()
        {
            sw.Flush();
        }
        public void Close()
        {
            sw.Close();
        }
        public void Dispose()
        {
            sw.Dispose();
        }
    }
}
