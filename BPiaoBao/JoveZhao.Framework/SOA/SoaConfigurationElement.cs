using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.SOA
{
    public class SoaConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("client")]
        public ClientConfigurationElement Client
        {
            get { return (ClientConfigurationElement)base["client"]; }
            set { base["client"] = value; }
        }
        [ConfigurationProperty("server")]
        public ServerConfigurationElement Server
        {
            get { return (ServerConfigurationElement)base["server"]; }
            set { base["server"] = value; }
        }
    }
}
