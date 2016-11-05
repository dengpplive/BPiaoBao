using JoveZhao.Framework.MemcachedClientLibrary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.Cache
{
    public class MemcacheStrategy : ICacheStrategy
    {
        Queue<string> keys = new Queue<string>();
        private string getKey(string key)
        {
            return cge.Name + "_" + key;
        }
        private CacheGroupElement cge;
        private MemcachedClient mc;
        public MemcacheStrategy(string name)
        {
            this.cge = JZFSection.GetInstances().Cache.CacheGroups[name];
            SockIOPool pool = SockIOPool.GetInstance();
            string[] serverlist = cge.Parameters["serverList"].Value.Split(',');
            pool.SetServers(serverlist);
            pool.InitConnections = 3;
            pool.MinConnections = 3;
            pool.MaxConnections = 5;
            pool.SocketConnectTimeout = 1000;
            pool.SocketTimeout = 3000;

            pool.MaintenanceSleep = 30;
            pool.Failover = true;

            pool.Nagle = false;
            pool.Initialize();

            mc = new MemcachedClient();
            mc.EnableCompression = false;

        }



        public void Set(string key, object value)
        {
            Set(key, value, DateTime.Now.Add(cge.Interval));
        }

        public void Set(string key, object value, DateTime expiryTime)
        {
            keys.Enqueue(key);
            mc.Set(getKey(key), value,expiryTime);

        }

        public object Get(string key)
        {
            return mc.Get(getKey(key));
        }

        public void Remove(string key)
        {
            var thiskey=getKey(key);
            if (mc.KeyExists(thiskey))
                mc.Delete(thiskey);
        }

        public void RemoveAll()
        {
            while (keys.Count > 0)
            {
                Remove(keys.Dequeue());
            }
        }


        public object[] FindAll()
        {
            return keys.Select(p => mc.Get(getKey(p))).ToArray();
            
        }
    }
}
