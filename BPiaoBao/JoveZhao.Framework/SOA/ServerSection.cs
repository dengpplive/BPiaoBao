using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace JoveZhao.Framework.SOA
{
    /// <summary>
    /// 服务端侦听
    /// </summary>
    public class ServerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("security")]
        public SecurityElement Security
        {
            get { return (SecurityElement)base["security"]; }
            set { base["security"] = value; }
        }
        

        [ConfigurationProperty("connection")]
        public ConnectionElement Connection
        {
            get { return (ConnectionElement)base["connection"]; }
            set { base["connection"] = value; }
        }

        [ConfigurationProperty("commands")]
        public CommandsElementCollection Commands
        {
            get { return (CommandsElementCollection)base["commands"]; }
        }

        #region 子类
        public class SecurityElement : ConfigurationElement
        {


            [ConfigurationProperty("whiteList")]
            public ServersElementCollection WhiteList
            {
                get { return (ServersElementCollection)base["whiteList"]; }
                set { base["whiteList"] = value; }
            }

            [ConfigurationProperty("blackList")]
            public ServersElementCollection BlackList
            {
                get { return (ServersElementCollection)base["blackList"]; }
                set { base["blackList"] = value; }
            }
        }
        public class ServerElement : ConfigurationElement
        {
            [ConfigurationProperty("ip")]
            public string IP
            {
                get { return (string)base["ip"]; }
                set { base["ip"] = value; }
            }
        }
        public class ServersElementCollection : BaseElementCollection<ServerElement>
        {
            [ConfigurationProperty("enabled")]
            public bool Enabled
            {
                get { return (bool)base["enabled"]; }
                set { base["enabled"] = value; }
            }

            protected override object GetKey(ServerElement element)
            {
                return element.IP;
            }

            protected override string ItemName
            {
                get { return "server"; }
            }
        }
        
                
        public class ConnectionElement : ConfigurationElement
        {
            [ConfigurationProperty("port")]
            public int Port
            {
                get { return (int)base["port"]; }
                set { base["port"] = value; }
            }
        }

        public class CommandElement : ConfigurationElement
        {
            [ConfigurationProperty("cmd")]
            public string Command
            {
                get { return (string)base["cmd"]; }
                set { base["cmd"] = value; }
            }

            [ConfigurationProperty("processor")]
            public string ProcessorType
            {
                get { return (string)base["processor"]; }
                set { base["processor"] = value; }
            }

            private ICommandProcess process;
            public ICommandProcess Processor
            {
                get
                {
                    if (process == null)
                    {
                        Type type = Type.GetType(this.ProcessorType);
                        process = Activator.CreateInstance(type) as ICommandProcess;
                    }
                    return process;
                }
            }
        }
        public class CommandsElementCollection : BaseElementCollection<CommandElement>
        {

            public CommandElement this[string command]
            {
                get
                {
                    return this.OfType<CommandElement>().Where(p => p.Command == command).FirstOrDefault();
                }
            }


            protected override object GetKey(CommandElement t)
            {
                return t.Command;
            }

            protected override string ItemName
            {
                get { return "command"; }
            }
        }
        #endregion
    }
    
}
