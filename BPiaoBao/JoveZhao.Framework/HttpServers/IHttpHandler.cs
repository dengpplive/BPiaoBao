using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace JoveZhao.Framework.HttpServers
{
    public interface IHttpHandler
    {
        void Process(HttpRequest request, HttpResponse response);
    }
}
