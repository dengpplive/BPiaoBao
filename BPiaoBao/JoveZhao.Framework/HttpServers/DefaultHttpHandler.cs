using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace JoveZhao.Framework.HttpServers
{
    [HttpCode("index")]
    public class DefaultHttpHandler : IHttpHandler
    {
        
        public void Process(HttpRequest request, HttpResponse writer)
        {
            
            writer.WriteLine("没有找到");
        }
    }
}
