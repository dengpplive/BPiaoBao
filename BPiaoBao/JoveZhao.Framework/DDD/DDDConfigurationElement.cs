using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.DDD
{
    public class DDDConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("unitOfWork")]
        public UnitOfWorkConfigurationElement UnitOfWork
        {
            get { return (UnitOfWorkConfigurationElement)base["unitOfWork"]; }
            set { base["unitOfWork"] = value; }
        }

        [ConfigurationProperty("repositories")]
        public RepositoryElementCollection Repositories
        {
            get { return (RepositoryElementCollection)base["repositories"]; }
            set { base["repositories"] = value; }
        }
        [ConfigurationProperty("events")]
        public EventElementCollection Events
        {
            get { return (EventElementCollection)base["events"]; }
            set { base["events"] = value; }
        }
        
    }
    public class UnitOfWorkConfigurationElement : ConfigurationElement
    {


        [ConfigurationProperty("unitOfWorkProvider")]
        public string UnitOfWorkProvider
        {
            get { return (string)base["unitOfWorkProvider"]; }
            set { base["unitOfWorkProvider"] = value; }
        }
        public Type UnitOfWorkType
        {
            get
            {
                return Type.GetType(UnitOfWorkProvider);
            }
        }
        [ConfigurationProperty("repositoryProvider")]
        public string RepositoryProvider
        {
            get { return (string)base["repositoryProvider"]; }
            set { base["repositoryProvider"] = value; }
        }
        public Type RepositoryType
        {
            get
            {
                return Type.GetType(RepositoryProvider);
            }
        }
    }
    public class IocConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get { return (string)base["name"]; }
            set { base["name"] = value; }
        }


        [ConfigurationProperty("forProvider")]
        public string ForProvider
        {
            get { return (string)base["forProvider"]; }
            set { base["forProvider"] = value; }
        }
        public Type ForType
        {
            get
            {
                return Type.GetType(ForProvider);
            }
        }
        [ConfigurationProperty("useProvider")]
        public string UseProvider
        {
            get { return (string)base["useProvider"]; }
            set { base["useProvider"] = value; }
        }
        public Type UseType
        {
            get
            {
                return Type.GetType(UseProvider);
            }
        }
    }
    public class RepositoryElementCollection : BaseElementCollection<IocConfigurationElement>
    {
        protected override object GetKey(IocConfigurationElement t)
        {
            return t.Name;
        }

        protected override string ItemName
        {
            get { return "repository"; }
        }
    }
    public class EventElementCollection : BaseElementCollection<IocConfigurationElement>
    {

        protected override object GetKey(IocConfigurationElement t)
        {
            return t.Name;
        }

        protected override string ItemName
        {
            get { return "event"; }
        }
    }
}
