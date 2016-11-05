using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.HttpServers
{
    public class HttpCodeAttribute : Attribute
    {
        public HttpCodeAttribute(string code)
        {
            this.Code = code;
        }
        public string Code { get; set; }
    }
}
