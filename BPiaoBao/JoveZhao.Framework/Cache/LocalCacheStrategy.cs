using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.Cache
{
    public class LocalCacheStrategy : ICacheStrategy
    {
        MemoryCache mc;
        TimeSpan interval;
        public LocalCacheStrategy(string name)
        {
            var cge = JZFSection.GetInstances().Cache.CacheGroups[name];
            mc = new MemoryCache(cge.Name);
            interval = cge.Interval;
        }
         
         
        public void Set(string key, object value)
        {
           var  cp = new CacheItemPolicy();
            var now = System.DateTime.Now;
            //cp.AbsoluteExpiration = new DateTimeOffset(now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second, interval);
            cp.SlidingExpiration = interval;
            mc.Set(key, value, cp);
        }

        public void Set(string key, object value, DateTime expiryTime)
        {
            mc.Set(key, value, new DateTimeOffset(expiryTime));
        }

        public object Get(string key)
        {
            return mc.Get(key);
        }

        public void Remove(string key)
        {
            mc.Remove(key);
        }

        public void RemoveAll()
        {
            mc.Dispose();
        }




        public object[] FindAll()
        {
            return mc.Select(p => p.Value).ToArray();
        }
    }
}
