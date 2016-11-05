using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace JoveZhao.Framework.Email
{
    public class EmailConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("account")]
        public AccountConfigurationElement Account
        {
            get { return (AccountConfigurationElement)base["account"]; }
            set { base["account"] = value; }
        }
        [ConfigurationProperty("provider")]
        public string Provider
        {
            get { return (string)base["provider"]; }
            set { base["provider"] = value; }
        }
        public Type Type
        {
            get { return Type.GetType(Provider); }
        }
    }
    public class AccountConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("host")]
        public string Host
        {
            get { return (string)base["host"]; }
            set { base["host"] = value; }
        }
        [ConfigurationProperty("port")]
        public int Port
        {
            get { return (int)base["port"]; }
            set { base["port"] = value; }
        }
        [ConfigurationProperty("account")]
        public string Account
        {
            get { return (string)base["account"]; }
            set { base["account"] = value; }
        }
        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }
    }
}
