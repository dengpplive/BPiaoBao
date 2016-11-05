using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JoveZhao.Framework.SOA
{
    public class Pool
    {

        private static Dictionary<string, Pool> pools = new Dictionary<string, Pool>();
        
        public static Pool Init()
        {
            var config = JZFSection.GetInstances().Soa.Client;
            return Init(config);
        }
        private static Pool Init(ClientConfigurationElement config)
        {
            string key = config.GetHashCode().ToString();
            if (!pools.ContainsKey(key))
            {
                var p = new Pool(config);
                pools[key] = p;
            }
            return pools[key];
        }
        private ClientConfigurationElement config;
        private Pool(ClientConfigurationElement config)
        {
            this.config = config;

            //初始化连接池
            for (var i = 0; i < config.SocketPool.MinPoolSize; i++)
            {
                Create();
            }
        }

        volatile List<TcpClient> clientList = new List<TcpClient>();
        volatile Queue<PoolItem> pool = new Queue<PoolItem>();
        static private object lockp = new object();
        public PoolItem Dequeue()
        {
            if (pool.Count > 0)
            {
                lock (lockp)
                {
                    if (pool.Count > 0)
                        return pool.Dequeue();
                }
            }

            if (clientList.Count < config.SocketPool.MaxPoolSize)
            {
                //创建连接
                Create();
                //pool.Enqueue(item);
            }

            //等待
            Thread.Sleep(config.SocketPool.WaitTime);



            return Dequeue();
        }

        private void Create()
        {
            ThreadPool.QueueUserWorkItem(_create);
        }
        private void _create(object o)
        {
            //创建连接，按服务器列表寻找服务器地址创建连接
            var s = config.Servers.OfType<ServerElement>().OrderBy(p => p.UseCount).FirstOrDefault();
            for (var i = 0; i < config.SocketPool.TryCount; i++)
            {

                try
                {

                    s.Use();
                    TcpClient client = new TcpClient();
                    client.Connect(s.IP, s.Port);
                    clientList.Add(client);
                    Enqueue(new PoolItem() { serverName = s.Name, tcpClient = client });
                    return;
                }
                catch (Exception ex)
                {
                    Logger.WriteLog(LogType.ERROR, "尝试连接服务器" + s.Name + "失败", ex);
                };
            }
            Logger.WriteLog(LogType.WARN, "尝试连接服务器" + s.Name + "失败");
        }


        public void Enqueue(PoolItem item)
        {
            lock (lockp)
            {
                pool.Enqueue(item);
            }
        }

        public void Close()
        {
            foreach (var client in clientList)
            {
                client.Close();
            }
            clientList.Clear();
        }
    }
    public class PoolItem
    {
        public string serverName { get; set; }
        public TcpClient tcpClient { get; set; }
    }

}
