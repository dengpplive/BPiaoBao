using BPiaoBao.AppServices.Cashbag;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.AppServices.ServerMessages;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.AppServices.TPos;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework.HttpServers;
using System;
using System.ServiceModel;

namespace BPiaoBao.AppServices.Hosting
{
    static class Program
    {
        private static bool isGoOn = false;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        static void Main()
        {
            #region MyCode

            //Stopwatch watch = new Stopwatch();
            //watch.Start();

            Func<Type[]> funcs = new Func<Type[]>(() =>
            {
                Type[] types = {
                              typeof(DateTimeService),
                              typeof(LoginService),
                              typeof(PublisherService),
                              typeof(BusinessmanService),
                              typeof(OrderService),
                              typeof(PlatformRefundOrderService),
                              typeof(AccountService),
                              typeof(FundService),
                              typeof(FinancialService),
                              typeof(RiskControlService),
                              typeof(TPosService),
                              typeof(FlightDestineService),
                              typeof(TravelPaperService),
                              typeof(PlatformCfgService),
                              typeof(DeductionGroupService),
                              typeof(BehaviorStatService),
                              typeof(PidService),
                              typeof(FrePasserService),
                              typeof(ConsoLocalPolicyService),
                              typeof(InsuranceService),
                              typeof(NoticeService),
                              typeof(MemoryService),
                              typeof(MyMessageService),
                              typeof(PlatformPointGroupService),
                              typeof(OperationLogService)
                          }; // 这里耗时最多。
                return types;
            });
            
            IAsyncResult result = funcs.BeginInvoke(new AsyncCallback(Callback), funcs);
            BootStrapper.Boot(); // 耗时！
       

            while (true)
            {
                if (isGoOn)
                {

                    var ser = SettingSection.GetInstances().HttpServer;
                    HttpServer server = new HttpServer(ser.Host, ser.Port);
                    server.Start();
                    Console.WriteLine("Web服务启动成功");
                    //1106
                    //JoveZhao.Framework.DDD.Events.DomainEvents.Raise(new BPiaoBao.DomesticTicket.Domain.Models.RefundEvent.RefundTicketEvent() { SaleOrderId = 3797 });
                    while (true)
                    {
                        Console.WriteLine("请输入命令数字:");
                        EnumMessageCommand command = (EnumMessageCommand)int.Parse(Console.ReadLine());
                        Console.WriteLine("请输入商户号:");
                        string code = Console.ReadLine();
                        Console.WriteLine("请输入发送内容");
                        string content = Console.ReadLine();
                        WebMessageManager.GetInstance().Send(command, code, content);
                        MessagePushManager.SendMsgByBuyerCodes(new string[] { code }, (Common.Enums.EnumPushCommands)(command), content, true);
                        Console.WriteLine("消息已发送");
                    }
                }
            }

            #endregion



            #region OldCode
            //ServiceBase[] ServicesToRun;
            //ServicesToRun = new ServiceBase[] 
            //{ 
            //    new Service1() 
            //};
            //ServiceBase.Run(ServicesToRun);

            //Stopwatch watch = new Stopwatch();
            //watch.Start();
            //BootStrapper.Boot();
            //Type[] types = {
            //                  typeof(DateTimeService),
            //                  typeof(LoginService),
            //                  typeof(PublisherService),
            //                  typeof(BusinessmanService),
            //                  typeof(OrderService),
            //                  typeof(PlatformRefundOrderService),
            //                  typeof(AccountService),
            //                  typeof(FundService),
            //                  typeof(FinancialService),
            //                  typeof(RiskControlService),
            //                  typeof(TPosService),
            //                  typeof(FlightDestineService),
            //                  typeof(TravelPaperService),
            //                  typeof(PlatformCfgService),
            //                  typeof(DeductionGroupService),
            //                  typeof(BehaviorStatService),
            //                  typeof(PidService),
            //                  typeof(FrePasserService),
            //                  typeof(ConsoLocalPolicyService),
            //                  typeof(InsuranceService),
            //                  typeof(NoticeService)
            //              };
            //watch.Stop();
            //var c = watch.ElapsedMilliseconds;  // 反射耗时：6486(ms)  = 6.486(s)

            //foreach (var t in types)
            //{
            //    ServiceHost host = new ServiceHost(t);
            //    host.Opened += (p, q) =>
            //    {
            //        Console.WriteLine(t.Name + "启动成功");
            //    };
            //    host.Open();
            //} 


            //AutoIssueManage.Start();
            //Console.WriteLine("自动出票服务启动");
            //DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
            //domesticService.AutoIssue("04654475181900151211");



            //while (true)
            //{
            //    Console.WriteLine("清输入命令[数字]:");
            //    string inputCommand = Console.ReadLine();
            //    Console.WriteLine("输入发送内容:");
            //    string inputContent = Console.ReadLine();
            //    Console.WriteLine("请输入发送商户号");
            //    string code = Console.ReadLine();
            //    MessagePushManager.SendMsgByBuyerCodes(new string[] { code }, (Common.Enums.EnumPushCommands)(inputCommand.ToInt
            //        ()), inputContent, true);
            //    Console.WriteLine("消息已发送");
            //} 

            //while (true)
            //{
            //    Console.WriteLine("请输入Token：");
            //    string token = Console.ReadLine();
            //    MemAuthInfoStroage mi = new MemAuthInfoStroage();
            //    var u = mi.GetUserByToken(token);
            //    Console.WriteLine(u.ToJson());
            //}

            #endregion

        }

        /// <summary>
        /// 异步回调
        /// </summary>
        /// <param name="ar"></param>
        private static void Callback(IAsyncResult ar)
        {
            Func<Type[]> funcs = (Func<Type[]>)ar.AsyncState;
            Type[] types = null;
            if (null != funcs)
            {
                types = funcs.EndInvoke(ar);
            }

            foreach (var t in types)
            {
                ServiceHost host = new ServiceHost(t);
                host.Opened += (p, q) =>
                {
                    Console.WriteLine(t.Name + "启动成功");
                };
                host.Open();
            }

            isGoOn = true;
        }

    }
}
