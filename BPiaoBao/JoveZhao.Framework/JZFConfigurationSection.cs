using JoveZhao.Framework.Cache;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Email;
using JoveZhao.Framework.SMS;
using JoveZhao.Framework.SOA;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework
{
    public class JZFConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("email")]
        public EmailConfigurationElement Email
        {
            get { return (EmailConfigurationElement)base["email"]; }
            set { base["email"] = value; }
        }[ConfigurationProperty("sms")]
        public SMSConfigurationElement SMS
        {
            get { return (SMSConfigurationElement)base["sms"]; }
            set { base["sms"] = value; }
        }
        [ConfigurationProperty("cache")]
        public CacheConfigurationElement Cache
        {
            get { return (CacheConfigurationElement)base["cache"]; }
            set { base["cache"] = value; }
        }
        [ConfigurationProperty("soa")]
        public SoaConfigurationElement Soa
        {
            get { return (SoaConfigurationElement)base["soa"]; }
            set { base["soa"] = value; }
        }
        [ConfigurationProperty("ddd")]
        public DDDConfigurationElement DDD
        {
            get { return (DDDConfigurationElement)base["ddd"]; }
            set { base["ddd"] = value; }
        }
    }

    public class JZFSection
    {
        public static JZFConfigurationSection GetInstances()
        {
            return SectionManager.GetConfigurationSection<JZFConfigurationSection>("jzfSection");
        }
    }
}
