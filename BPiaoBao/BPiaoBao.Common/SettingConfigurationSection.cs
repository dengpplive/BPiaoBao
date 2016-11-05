using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using JoveZhao.Framework;

namespace BPiaoBao.Common
{
    public class SettingConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("sms")]
        public SmsConfigurationElement Sms
        {
            get { return (SmsConfigurationElement)base["sms"]; }
            set { base["sms"] = value; }
        }
        [ConfigurationProperty("cashbag")]
        public CashbagConfigurationElement Cashbag
        {
            get { return (CashbagConfigurationElement)base["cashbag"]; }
            set { base["cashbag"] = value; }
        }
        [ConfigurationProperty("httpServer")]
        public HttpServerConfigurationElement HttpServer
        {
            get { return (HttpServerConfigurationElement)base["httpServer"]; }
            set { base["httpServer"] = value; }
        }
        [ConfigurationProperty("payment")]
        public PaymentConfigurationElement Payment
        {
            get { return (PaymentConfigurationElement)base["payment"]; }
            set { base["payment"] = value; }
        }
        [ConfigurationProperty("autoIssue")]
        public AutoIssueConfigurationElement AutoIssue
        {
            get { return (AutoIssueConfigurationElement)base["autoIssue"]; }
            set { base["autoIssue"] = value; }
        }
    }

    public class PaymentConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("ticketNotify")]
        public string TicketNotify
        {
            get { return (string)base["ticketNotify"]; }
            set { base["ticketNotify"] = value; }
        }
        [ConfigurationProperty("smsNotify")]
        public string SmsNotify
        {
            get { return (string)base["smsNotify"]; }
            set { base["smsNotify"] = value; }
        }
        [ConfigurationProperty("saleNotify")]
        public string SaleNotify
        {
            get { return (string)base["saleNotify"]; }
            set { base["saleNotify"] = value; }
        }
    }
    public class CashbagConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("baseUrl")]
        public string BaseUrl
        {
            get { return (string)base["baseUrl"]; }
            set { base["baseUrl"] = value; }
        }
        [ConfigurationProperty("notifyCode")]
        public string NotifyCode
        {
            get { return (string)base["notifyCode"]; }
            set { base["notifyCode"] = value; }
        }
        [ConfigurationProperty("partnerKey")]
        public string PartnerKey
        {
            get { return (string)base["partnerKey"]; }
            set { base["partnerKey"] = value; }
        }
        [ConfigurationProperty("cashbagCode")]
        public string CashbagCode
        {
            get { return (string)base["cashbagCode"]; }
            set { base["cashbagCode"] = value; }
        }

        [ConfigurationProperty("cashbagKey")]
        public string CashbagKey
        {
            get { return (string)base["cashbagKey"]; }
            set { base["cashbagKey"] = value; }
        }
    }
    public class SmsConfigurationElement : ConfigurationElement
    {

        [ConfigurationProperty("smsPrice")]
        public decimal SmsPrice
        {
            get { return (decimal)base["smsPrice"]; }
            set { base["smsPrice"] = value; }
        }
        [ConfigurationProperty("smsLKAccount")]
        public string smsLKAccount
        {
            get { return (string)base["smsLKAccount"]; }
            set { base["smsLKAccount"] = value; }
        }
        [ConfigurationProperty("smsLKPwd")]
        public string smsLKPwd
        {
            get { return (string)base["smsLKPwd"]; }
            set { base["smsLKPwd"] = value; }
        }
    }
    public class HttpServerConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("port")]
        public int Port
        {
            get { return (int)base["port"]; }
            set { base["port"] = value; }
        }
        [ConfigurationProperty("host")]
        public string Host
        {
            get { return (string)base["host"]; }
            set { base["host"] = value; }
        }
    }
    public class SettingSection
    {
        public static SettingConfigurationSection GetInstances()
        {
            return SectionManager.GetConfigurationSection<SettingConfigurationSection>("settingSection");
        }
    }
    public class AutoIssueConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("alipayAutoCPUrl")]
        public string AlipayAutoCPUrl
        {
            get { return (string)base["alipayAutoCPUrl"]; }
            set { base["alipayAutoCPUrl"] = value; }
        }
        [ConfigurationProperty("alipayTicketNotifyUrl")]
        public string AlipayTicketNotifyUrl
        {
            get { return (string)base["alipayTicketNotifyUrl"]; }
            set { base["alipayTicketNotifyUrl"] = value; }
        }
        [ConfigurationProperty("alipayPayNotifyUrl")]
        public string AlipayPayNotifyUrl
        {
            get { return (string)base["alipayPayNotifyUrl"]; }
            set { base["alipayPayNotifyUrl"] = value; }
        }
    }


    public class CustomerSection
    {
        public static CustomerConfigurationSection GetInstances()
        {
            return SectionManager.GetConfigurationSection<CustomerConfigurationSection>("customerSection");
        }

        public static void Save()
        {
            SectionManager.SaveConfigurationSection("customerSection");
        }
    }
    public class CustomerConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("customPhone")]
        public string CustomPhone
        {
            get { return (string)base["customPhone"]; }
            set { base["customPhone"] = value; }
        }
        [ConfigurationProperty("advisoryQQs")]
        public KeyAndValueElementCollection AdvisoryQQs
        {
            get { return (KeyAndValueElementCollection)base["advisoryQQs"]; }
            set { base["advisoryQQs"] = value; }
        }
        [ConfigurationProperty("hotlinePhones")]
        public KeyAndValueElementCollection HotlinePhones
        {
            get
            {
                return (KeyAndValueElementCollection)base["hotlinePhones"];
            }
            set { base["hotlinePhones"] = value; }
        }
        [ConfigurationProperty("linkMethods")]
        public KeyAndValueElementCollection LinkMethods
        {
            get
            {
                return (KeyAndValueElementCollection)base["linkMethods"];
            }
            set { base["linkMethods"] = value; }
        }
    }


    public class KeyAndValueElementCollection : BaseElementCollection<KeyAndValueElement>
    {
        protected override object GetKey(KeyAndValueElement t)
        {
            return t.Key;
        }
        protected override string ItemName
        {
            get { return "item"; }
        }

    }
    public class KeyAndValueElement : ConfigurationElement
    {
        [ConfigurationProperty("key")]
        public string Key
        {
            get { return (string)base["key"]; }
            set { base["key"] = value; }
        }
        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }
    }
}
