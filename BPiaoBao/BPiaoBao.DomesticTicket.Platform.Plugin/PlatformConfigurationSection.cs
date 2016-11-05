using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using JoveZhao.Framework;

namespace BPiaoBao.DomesticTicket.Platform.Plugin
{

    //<platformSection>
    //  <platforms>
    //    <platform name="517" provider="PiaoBao.BTicket.Platform._517._517Platform,PiaoBao.BTicket.Platform">
    //      <areas defaultCity="ctu">
    //        <area city="ctu">
    //          <parameters>
    //            <parameter name="aa" value="aa" description="a的参数" />
    //          </parameters>
    //        </area>
    //      </areas>
    //    </platform>

    //  </platforms>

    //</platformSection>

    public class PlatformConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("platforms")]
        public PlatformElementCollection Platforms
        {
            get { return (PlatformElementCollection)base["platforms"]; }
            set { base["platforms"] = value; }
        }
    }

    public class PlatFormElement : ConfigurationElement
    {
        [ConfigurationProperty("code")]
        public string Code
        {
            get { return (string)base["code"]; }
            set { base["code"] = value; }
        }
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("showcount")]
        public int ShowCount
        {
            get { return (int)base["showcount"]; }
            set { base["showcount"] = value; }
        }

        [ConfigurationProperty("areas")]
        public AreaElementCollection Areas
        {
            get { return (AreaElementCollection)base["areas"]; }
            set { base["areas"] = value; }
        }
        [ConfigurationProperty("issueSpeed")]
        public string IssueSpeed
        {
            get { return (string)base["issueSpeed"]; }
            set { base["issueSpeed"] = value; }
        }
        [ConfigurationProperty("isClosed")]
        public bool IsClosed
        {
            get { return (bool)base["isClosed"]; }
            set { base["isClosed"] = value; }
        }
        [ConfigurationProperty("paidIsTest")]
        public bool paidIsTest
        {
            get { return (bool)base["paidIsTest"]; }
            set { base["paidIsTest"] = value; }
        }

        [ConfigurationProperty("b2bClose")]
        public string b2bClose
        {
            get { return (string)base["b2bClose"]; }
            set { base["b2bClose"] = value; }
        }

        [ConfigurationProperty("bspClose")]
        public string bspClose
        {
            get { return (string)base["bspClose"]; }
            set { base["bspClose"] = value; }
        }
    }
    public class PlatformElementCollection : BaseElementCollection<PlatFormElement>
    {
        public PlatFormElement this[string name]
        {
            get
            {
                return this.OfType<PlatFormElement>().Where(p => string.Compare(p.Name, name, true) == 0).FirstOrDefault();
            }
        }


        protected override object GetKey(PlatFormElement t)
        {
            return t.Name;
        }

        protected override string ItemName
        {
            get { return "platform"; }
        }
    }
    public class ParameterElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }
        [ConfigurationProperty("value")]
        public string Value
        {
            get { return (string)base["value"]; }
            set { base["value"] = value; }
        }
        [ConfigurationProperty("description")]
        public string Description
        {
            get { return (string)base["description"]; }
            set { base["description"] = value; }
        }
    }
    public class ParameterElementCollection : BaseElementCollection<ParameterElement>
    {
        protected override object GetKey(ParameterElement t)
        {
            return t.Name;
        }

        protected override string ItemName
        {
            get { return "parameter"; }
        }
        public ParameterElement this[string name]
        {
            get
            {
                return this.OfType<ParameterElement>().Where(p => p.Name == name).FirstOrDefault();
            }
        }

    }
    public class AreaElement : ConfigurationElement
    {
        [ConfigurationProperty("city")]
        public string City
        {
            get { return (string)base["city"]; }
            set { base["city"] = value; }
        }
        [ConfigurationProperty("parameters")]
        public ParameterElementCollection Parameters
        {
            get { return (ParameterElementCollection)base["parameters"]; }
            set { base["parameters"] = value; }
        }
    }
    public class AreaElementCollection : BaseElementCollection<AreaElement>
    {
        [ConfigurationProperty("defaultCity")]
        public string DefaultCity
        {
            get { return (string)base["defaultCity"]; }
            set { base["defaultCity"] = value; }
        }

        protected override object GetKey(AreaElement t)
        {
            return t.City;
        }

        protected override string ItemName
        {
            get { return "area"; }
        }


        public ParameterElementCollection GetByArea(ref string area)
        {
            var areaCity = area;
            var o = this.OfType<AreaElement>().Where(p => p.City == areaCity).FirstOrDefault();
            if (o == null)
            {
                o = this.OfType<AreaElement>().Where(p => p.City == DefaultCity).FirstOrDefault();
                area = DefaultCity;
            }
            if (o == null)
                throw new NotFoundResourcesException(string.Format("没有找到区域为：{0}或默认区域{1}的接口配置项", area, DefaultCity));
            return o.Parameters;
        }
    }

    public class PlatformSection
    {
        public static PlatformConfigurationSection GetInstances()
        {
            return SectionManager.GetConfigurationSection<PlatformConfigurationSection>("platformSection");
        }

        public static void Save()
        {
            SectionManager.SaveConfigurationSection("platformSection");
        }
    }
}