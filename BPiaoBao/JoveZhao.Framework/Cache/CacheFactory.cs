using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using StructureMap;

namespace JoveZhao.Framework.Cache
{
    public class CacheFactory
    {

        static CacheFactory()
        {
            var cacheConfig = JZFSection.GetInstances().Cache;
            ObjectFactory.Configure(x =>
            {
                foreach (CacheGroupElement p in cacheConfig.CacheGroups)
                {
                    x.For(typeof(ICacheStrategy)).Singleton().Use(p.Type).CtorDependency<CacheGroupElement>("name").Is(p.Name).Name = p.Name;
                        //.Name = p.Name;
                }
                
            });
        }
        public static ICacheStrategy CreateCacheStrategy(string cacheGroup)
        {
            return ObjectFactory.GetNamedInstance<ICacheStrategy>(cacheGroup);
        }
    }

   
}
