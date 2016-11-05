using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;

namespace JoveZhao.Framework.HttpServers
{
    public class HttpServer
    {
        HttpListener listener;

        public HttpServer(string host, int port)
        {
            listener = new HttpListener();
            listener.Prefixes.Add(string.Format("http://{0}:{1}/", host, port));
        }
        volatile bool status = false;
        public void Start()
        {
            ObjectFactory.Configure(x =>
            {
                x.Scan(p =>
                {
                    p.AssembliesFromApplicationBaseDirectory();
                    p.AddAllTypesOf<IHttpHandler>().NameBy(q =>
                    {
                        var att = q.GetCustomAttributes(typeof(HttpCodeAttribute), true).FirstOrDefault();
                        return (att as HttpCodeAttribute).Code.ToLower();
                    });
                });
            });

            Action<HttpListener> act = p =>
            {
                p.Start();
                //Logger.WriteLog(LogType.INFO, "http服务器监听开启");
                status = true;
                while (status)
                {
                    var httpContext = p.GetContext();
                    ThreadPool.QueueUserWorkItem(Process, httpContext);
                }
            };
            act.BeginInvoke(listener, null, null);
        }

        public void Stop()
        {
            listener.Stop();
            status = false;
        }
        void Process(object context)
        {
            try
            {
                var httpContext = context as HttpListenerContext;

                IHttpHandler httpHandler = HttpDistributer.Distribute(httpContext);

                HttpResponse response = new HttpResponse(httpContext.Response);
                HttpRequest request = new HttpRequest(httpContext.Request);

                httpHandler.Process(request, response);
                response.Flush();
                response.Close();
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.Message, ex);
            }
            
        }


    }


}
