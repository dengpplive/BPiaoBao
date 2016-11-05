using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace JoveZhao.Framework.SOA
{
    public class ClientConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("servers")]
        public ServersElementCollection Servers
        {
            get { return (ServersElementCollection)base["servers"]; }
            set { base["servers"] = value; }
        }
        [ConfigurationProperty("socketPool")]
        public PoolElement SocketPool
        {
            get { return (PoolElement)base["socketPool"]; }
            set { base["socketPool"] = value; }
        }
    }

    public class PoolElement : ConfigurationElement
    {
        [ConfigurationProperty("timeout")]
        public int Timeout
        {
            get { return (int)base["timeout"]; }
            set { base["timeout"] = value; }
        }

         [ConfigurationProperty("minPoolSize")]
        public int MinPoolSize
        {
            get { return (int)base["minPoolSize"]; }
            set { base["minPoolSize"] = value; }
        }

         [ConfigurationProperty("maxPoolSize")]
         public int MaxPoolSize
         {
             get { return (int)base["maxPoolSize"]; }
             set { base["maxPoolSize"] = value; }
         }

         [ConfigurationProperty("tryCount")]
         public int TryCount
         {
             get { return (int)base["tryCount"]; }
             set { base["tryCount"] = value; }
         }
         [ConfigurationProperty("waitTime")]
         public int WaitTime
         {
             get { return (int)base["waitTime"]; }
             set { base["waitTime"] = value; }
         }
    }

    public class ServersElementCollection : BaseElementCollection<ServerElement>
    {
        protected override object GetKey(ServerElement t)
        {
            return t.Name;
        }

        protected override string ItemName
        {
            get { return "server"; }
        }
    }

    public class ServerElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }
        [ConfigurationProperty("port")]
        public int Port
        {
            get { return (int)base["port"]; }
            set { base["port"] = value; }
        }
        [ConfigurationProperty("ip")]
        public string IP
        {
            get { return (string)base["ip"]; }
            set { base["ip"] = value; }
        }


        private int useTimes;
        internal int UseCount
        {
            get { return useTimes; }

        }
        internal void Use()
        {
            this.useTimes++;
        }
    }
}
