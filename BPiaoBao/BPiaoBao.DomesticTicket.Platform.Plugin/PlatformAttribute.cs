using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PlatformAttribute:Attribute
    {
        public string Code { get; set; }
        public PlatformAttribute(string code)
        {
            this.Code = code;
        }
    }
}
