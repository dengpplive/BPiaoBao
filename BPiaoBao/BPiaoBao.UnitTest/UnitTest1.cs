using System;
using System.Collections.Generic;
using BPiaoBao.AppServices;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using BPiaoBao.UnitTest.DomesticTicket;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using JoveZhao.Framework.Expand;
using BPiaoBao.Cashbag.ClientProxy;
using BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent;
using JoveZhao.Framework;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;

namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestAddCommany()
        {
            try
            {

                //var currentTime= DateTime.Now.AddMinutes(30 * (-1));

                BootStrapper.Boot();

                var service = ObjectFactory.GetInstance<ITicketSumRepository>();

            //var list = this.DbContext.Ticket.OfType<Ticket_Conso>().Where(p => p.TicketState.Equals("出票") && p.PolicyFrom.Equals("BaiTuo") && p.OrderID.Equals(orderid)).ToList();

            //list.GroupBy(x => x.OrderID).ToList();
           
        
                //DateTime d=DateTime.Parse("2014-09-01");
                ////var list = service.FindAllNoTracking(p => p.CreateDate >= d && p.TicketState == "出票").GroupBy(p => p.OrderID).Select(y => y.Key);
                //var list = service.GetOrderIDS();
                //int i = 0;
              //  service.Test(DateTime.Parse("2014/09/25"), DateTime.Parse("2014/09/30"));


                new TicketEventHelper().FindIssueTicket("05593557438705623979");
               
                //service.HandIssueTicket("05288718000060619529", new List<PassengerTicketDto>()`
                //{
                //    new PassengerTicketDto{Name="孙偲轶",TicketNumber="9992345808752"},
                //    new PassengerTicketDto{Name="王书彦",TicketNumber="9992345808753"}
                //}, "");

               // var orderRerpository = ObjectFactory.GetInstance<IOrderRepository>();
               // //var aftersaleorderRepository=ObjectFactory.GetInstance<IAfterSaleOrderRepository>();
               // //var order = orderRerpository.FindAll(p => p.OrderId.Equals("5394309432275326556")).FirstOrDefault();
               // //var list = order.SkyWays.ToList();
               // //var aftersaleORder=aftersaleorderRepository.FindAll(p=>p.Id==4471).FirstOrDefault();
               // //var ticketSums = new TicketEventHelper().GetTicketSums(aftersaleORder, list);

               // List<string> list = new List<string>(){
               //"04725542912480565780",
               // "05445489308327334223"
               // };

               // orderRerpository.FindAllNoTracking(p => list.Contains(p.OrderId)).ForEach(p =>
               // {
               //     JoveZhao.Framework.DDD.Events.DomainEvents.Raise<TicketEvent>(new TicketEvent
               //     {
               //         TicketSums = new TicketEventHelper().GetTicketSums(p)
               //     });
               // });
               // //引发领域事件

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                // Logger.WriteLog(LogType.ERROR, string.Format("{0}:写入总表失败", order.OrderId), e);
            }
            //string code = "0000000001";
            //string key = "581cc62a83894a6b8e14db2b4c3881ec";
            //AccountClientProxy pr = new AccountClientProxy();
            //var s = pr.AddCompany(code, key, new Cashbag.Domain.Models.CashCompanyInfo()
            //  {
            //      ClientAccount = "yys",
            //      Contact = "老赵",
            //      CpyName = "测试公司",
            //      Moblie = "13000000000"

            //  });


        }
        public decimal CeilAngle(decimal del)
        {
            decimal d = del * 10;
            del = Math.Ceiling(d) / 10;
            return del;
        }
        public void TestMethod()
        {
            decimal m = CeilAngle(1.20877m);
        }

        [TestMethod]
        public void TestMethod1()
        {


            BootStrapper.Boot();
            AuthManager.SaveUser(new CurrentUserInfo()
            {
                Code = "yys",
                BusinessmanName = "运营",
                OperatorAccount = "00001"
            });
            TravelPaperService service = ObjectFactory.GetInstance<TravelPaperService>();


            //int count = service.AddTravelPaper("00001", "1111111111", "1111111119", "ctu324", "77777777", "填开单位", "备注");
            //查询发放记录
            //List<TravelGrantRecordDto> list = service.FindTravelRecord("111","","", null, null);            
            //查询详细记录
            // DataPack<TravelPaperDto> detaillist = service.FindTravelPaper("caigou", "", "", "1111111111", "1111111111", null, null, null, null, null, null, null, null, null, null, 0, 1, 10);

            //查询统计
            //TravelPaperStaticsDto travelPaperStaticsDto = service.FindTravelPaper("111","","");

            List<int> ids = new List<int>();
            ids.Add(1);
            ids.Add(2);
            ids.Add(3);
            ids.Add(4);
            ids.Add(5);
            ids.Add(6);
            ids.Add(7);
            ids.Add(8);
            ids.Add(9);
            //回收空白行程单
            //service.RecoveryBlackTravelPaper("111", ids);

            //发放空白行程单
            // service.GrantBlankRecoveryTravelPaper("111", "", "", "", "", "", "", "", ids);
            //批量修改行程单Office
            service.UpdateOffice("CTU303", ids);

        }
    }
}
