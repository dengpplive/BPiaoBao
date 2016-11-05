using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using BPiaoBao.AppServices.Contracts;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.Contracts.ServerMessages;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.AppServices.Hosting;
using BPiaoBao.Client.UIExt.Communicate;
using JoveZhao.Framework.Expand;
using StructureMap;

namespace ConsoleApplication1
{
    class Program
    {
        private static string user = ConfigurationManager.AppSettings["testUser"];
        static void Main(string[] args)
        {
            //for (var i = 0; i < 100; i++)
            //{
            //    Thread.Sleep(500);
            //    var th=new Thread(new ThreadStart(Run));
            //    th.Start();
            //}
            //Console.ReadKey();
            Login();
            Parallel.For(0, 1000, t => { Thread.Sleep(2000); Search();});
            Console.ReadKey();
        }

        static void Run()
        {
            while (true)
            {
                Login();
                Thread.Sleep(500);
            }
        }

 

        static void Login()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("当前线程调用：ThreadId-->" + threadId);
            using (ChannelFactory<ILoginService> cf = new ChannelFactory<ILoginService>(typeof(ILoginService).Name))
            {
                var client = cf.CreateChannel();
                LoginInfo.Token = client.Login(user, "admin", "123456");
               // Search();
            }

        }

        private static void Search()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            Console.WriteLine("当前线程调用：ThreadId-->" + threadId);

            if (LoginInfo.Token == null)
            {
                Console.WriteLine("Return....");
                return;
            }
            Console.WriteLine("Search....");
            using (ChannelFactory<IOrderService> cf = new ChannelFactory<IOrderService>(typeof(IOrderService).Name))
            {
                var client = cf.CreateChannel();
                //client.FindAll("", "", "", "", "", "", "", DateTime.Now.AddDays(-10), DateTime.Now, "", null, null, "", "", null,null, null, 1, 20, true, true, "");
               var s=  client.FindAll("", "", "", DateTime.Now.AddDays(-20), DateTime.Now, null, 1, 30,false);
                Console.WriteLine("Result--->"+s.ToJson());
            }
           
        }
    }
}
