using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace JoveZhao.Framework.SMS
{
    public class SMSConfigurationElement : ConfigurationElement
    {
        
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
   
}
