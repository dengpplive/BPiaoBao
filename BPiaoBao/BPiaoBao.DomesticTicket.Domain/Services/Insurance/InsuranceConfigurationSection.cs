using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using JoveZhao.Framework;

namespace BPiaoBao.DomesticTicket.Domain.Services.Insurance
{

    /// <summary>
    /// 获取全部
    /// </summary>
    public class InsuranceConfigurationSection : ConfigurationSection
    {
        //[ConfigurationProperty("ctrlinsuranceinter")]
        //public InsuranceCollection CtrlInsuranceInterCollection
        //{
        //    get { return (InsuranceCollection)base["ctrlinsuranceinter"]; }
        //    set { base["ctrlinsuranceinter"] = value; }
        //}

        [ConfigurationProperty("ctrlinsurance")]
        public InsuranceCollection CtrlInsuranceCollection
        {
            get { return (InsuranceCollection)base["ctrlinsurance"]; }
            set { base["ctrlinsurance"] = value; }
        }

        [ConfigurationProperty("insurancerefund")]
        public InsuranceRefundElement InsuranceRefund
        {
            get { return (InsuranceRefundElement)base["insurancerefund"]; }
            set { base["insurancerefund"] = value; }
        }

        [ConfigurationProperty("insurance_platform_config")]
        public Insurance_Platform_ConfigElement Insurance_Platform_Config
        {
            get { return (Insurance_Platform_ConfigElement)base["insurance_platform_config"]; }
            private set { ; }
        }
    }


    public class InsuranceElement : ConfigurationElement
    {
        [ConfigurationProperty("iscurrent")]
        public bool IsCurrent
        {
            get { return (bool)base["iscurrent"]; }
            set { base["iscurrent"] = value; }
        }

        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }

        //[ConfigurationProperty("url")]
        //public string Url
        //{
        //    get { return (string)base["url"]; }
        //    set { base["url"] = value; }
        //}

        [ConfigurationProperty("leavecount")]
        public decimal LeaveCount
        {
            get { return (decimal)base["leavecount"]; }
            set { base["leavecount"] = value; }
        }
        
        //[ConfigurationProperty("singleprice")]
        //public decimal SinglePrice
        //{
        //    get { return (decimal)base["singleprice"]; }
        //    set { base["singleprice"] = value; }
        //}
    }


    public class InsuranceCollection : BaseElementCollection<InsuranceElement>
    {
        //public InsuranceElement this[string value]
        //{
        //    get
        //    {
        //        return this.OfType<InsuranceElement>().Where(p => string.Compare(p.Value, value, true) == 0).FirstOrDefault();
        //    }
        //}
        [ConfigurationProperty("isenabled")]
        public bool IsEnabled
        {
            get { return (bool)base["isenabled"]; }
            set { base["isenabled"] = value; }
        }

        [ConfigurationProperty("singleprice")]
        public decimal SinglePrice
        {
            get { return (decimal)base["singleprice"]; }
            set { base["singleprice"] = value; }
        }
        protected override object GetKey(InsuranceElement t)
        {
            return t.Value;
        }

        protected override string ItemName
        {
            get { return "insurance"; }
        }
    }



    public class InsuranceRefundElement : ConfigurationElement
    {
        [ConfigurationProperty("isenabled")]
        public bool IsEnabled
        {
            get { return (bool)base["isenabled"]; }
            set { base["isenabled"] = value; }
        }


        [ConfigurationProperty("singleprice")]
        public decimal SinglePrice
        {
            get { return (decimal)base["singleprice"]; }
            set { base["singleprice"] = value; }
        }
    }


    public class Insurance_Platform_ConfigElement : ConfigurationElement
    {
        [ConfigurationProperty("Insurance_ZK")]
        public Insurance_ZKElement Insurance_ZK
        {
            get { return (Insurance_ZKElement)base["Insurance_ZK"]; }
            set { base["Insurance_ZK"] = value; }
        }


        [ConfigurationProperty("Insurance_TPRS")]
        public Insurance_TPRSElement Insurance_TPRS
        {
            get { return (Insurance_TPRSElement)base["Insurance_TPRS"]; }
            set { base["Insurance_TPRS"] = value; }
        }
    }


    /// <summary>
    /// 中科保险验证信息
    /// </summary>
    public class Insurance_ZKElement : ConfigurationElement
    {
        [ConfigurationProperty("username")]
        public string UserName
        {
            get { return (string)base["username"]; }
            set { base["username"] = value; }
        }
        [ConfigurationProperty("password")]
        public string Password
        {
            get { return (string)base["password"]; }
            set { base["password"] = value; }
        }


        [ConfigurationProperty("productcode")]
        public string ProductCode
        {
            get { return (string)base["productcode"]; }
            set { base["productcode"] = value; }
        }


    }

    /// <summary>
    /// 太平人寿保险
    /// </summary>
    public class Insurance_TPRSElement : ConfigurationElement
    {
        [ConfigurationProperty("dlbh")]
        public string DLBH
        {
            get { return (string)base["dlbh"]; }
            set { base["dlbh"] = value; }
        }
        [ConfigurationProperty("tbbxddh")]
        public string TBBXDDH
        {
            get { return (string)base["tbbxddh"]; }
            set { base["tbbxddh"] = value; }
        }
        [ConfigurationProperty("key")]
        public string Key
        {
            get { return (string)base["key"]; }
            set { base["key"] = value; }
        }
        [ConfigurationProperty("url")]
        public string Url
        {
            get { return (string)base["url"]; }
            set { base["url"] = value; }
        }
         [ConfigurationProperty("jsbf")]
        public decimal JSBF
        {
            get { return (decimal)base["jsbf"]; }
            set { base["jsbf"] = value; }
        }

        
    }



}
