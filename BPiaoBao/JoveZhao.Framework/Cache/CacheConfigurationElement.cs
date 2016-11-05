using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.Cache
{
    public class CacheConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("cacheGroups")]
        public CacheGroupElementCollection CacheGroups
        {
            get { return (CacheGroupElementCollection)base["cacheGroups"]; }
        }
    }

    public class CacheGroupElementCollection : BaseElementCollection<CacheGroupElement>
    {
        protected override object GetKey(CacheGroupElement t)
        {
            return t.Name;
        }

        protected override string ItemName
        {
            get { return "cacheGroup"; }
        }
         public CacheGroupElement this[string name]
        {
            get
            {

                return this.OfType<CacheGroupElement>().Where(p => p.Name == name).FirstOrDefault();

            }
        }
    }
    

    public class CacheGroupElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }

        [ConfigurationProperty("provider")]
        public string Provider
        {
            get { return (string)base["provider"]; }
            set { base["provider"] = value; }
        }
        public Type Type
        {
            get
            {
                return Type.GetType(Provider);
            }
        }

        [ConfigurationProperty("interval")]
        public TimeSpan Interval
        {
            get { return (TimeSpan)base["interval"]; }
            set { base["interval"] = value; }
        }

        [ConfigurationProperty("notes")]
        public string Notes
        {
            get { return (string)base["notes"]; }
            set { base["notes"] = value; }
        }

        [ConfigurationProperty("parameters")]
        public ParameterElementCollection Parameters
        {
            get { return (ParameterElementCollection)base["parameters"]; }
            set { base["parameters"] = value; }
        }

        
    }
    public class ParameterElement:ConfigurationElement
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
}
