using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace JoveZhao.Framework.HttpServers
{
    public class HttpDistributer
    {
        public static IHttpHandler Distribute(HttpListenerContext context)
        {
            
            var path = context.Request.Url.AbsolutePath; // "/abc/aaa"
            var code = path.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries)[0];
            IHttpHandler httpHandler = null;
            try
            {
                httpHandler = ObjectFactory.GetNamedInstance<IHttpHandler>(code.ToLower());
            }
            catch
            {
                httpHandler = new DefaultHttpHandler();
            }
            return httpHandler;
        }
    }
}
