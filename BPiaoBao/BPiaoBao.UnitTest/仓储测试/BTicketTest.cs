using BPiaoBao.AppServices.Hosting;
using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.DomesticTicket.EFRepository;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework.DDD;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace BPiaoBao.UnitTest.仓储测试
{
    [TestClass]
    public class BTicketTest
    {
        [TestMethod]
        public void TestMethod1()
        {
           // Database.SetInitializer(new MigrateDatabaseToLatestVersion<TicketDbContext,Configuration>());
            BootStrapper.ConfigureDependencies();
            IUnitOfWork unitOfWorkTicket = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
            IUnitOfWorkRepository unitOfWorkRepositoryticket = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());

            Order order = new Order();
            order.OrderId = "01305201230119290277";
            order.PnrCode = "HPK3BQ";
            order.OrderMoney = decimal.Parse("1000");
            order.CreateTime = DateTime.Now;

            OrderPay orderPay = new OrderPay();
            orderPay.PayMoney = decimal.Parse("1000");
            order.OrderPay = orderPay ?? null;


            order.Policy = new Policy();
            order.Policy.PolicyPoint = decimal.Parse("8");
            order.Policy.PlatformCode = "517";
            order.Policy.PolicyType = "1";
            order.Policy.WorkTime = new StartAndEndTime();
            order.Policy.WorkTime.StartTime = "9:00";
            order.Policy.WorkTime.EndTime = "23:00";
            order.Policy.ReturnTicketTime = new StartAndEndTime();
            order.Policy.ReturnTicketTime.StartTime = "9:00";
            order.Policy.ReturnTicketTime.EndTime = "23:00";
            order.Policy.AnnulTicketTime = new StartAndEndTime();
            order.Policy.AnnulTicketTime.StartTime = "9:00";
            order.Policy.AnnulTicketTime.EndTime = "23:00";
            order.Passengers = new List<Passenger>();
            order.Passengers.Add(new Passenger { PassengerName = "测试", ABFee = decimal.Parse("50"), RQFee = decimal.Parse("120") });
            order.OrderLogs = new List<OrderLog>();
            order.OrderLogs.Add(new OrderLog { OperationContent = "新订单", OperationPerson = "test1", OperationDatetime = DateTime.Now });
            order.SkyWays = new List<SkyWay>();
            order.SkyWays.Add(new SkyWay { CarrayCode = "3U", FromCityCode = "CTU", ToCityCode = "PEK", Seat = "F", StartDateTime = DateTime.Parse("2013-12-05 11:15:00.000"), ToDateTime = DateTime.Parse("2013-12-05 11:17:00.000") });
            unitOfWorkRepositoryticket.PersistCreationOf(order);
            unitOfWorkTicket.Commit();
        }
    }
}
