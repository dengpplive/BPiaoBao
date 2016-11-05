using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.AppServices.Hosting;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System.Linq;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using System.Collections.Generic;
using AutoMapper;

namespace BPiaoBao.UnitTest.DomesticTicket
{
    [TestClass]
    public class Order
    {
        [TestMethod]
        public void FindAll()
        {
            //BootStrapper.Boot();
            //IStationOrderService ser = ObjectFactory.GetInstance<OrderService>();
            ////var t = ser.FindAll(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, 10);
            // var t= ser.GetAfterSaleOrderByPager(1, 10, null, null, null, null, null);

            BootStrapper.Boot();
            IStationOrderService ser = ObjectFactory.GetInstance<OrderService>();
            //var t = ser.FindAll(null, null, null, null, null, null, null, null, null, null, null, null, null, null, 0, 10);
            // var t= ser.GetAfterSaleOrderByPager(1, 10, null, null, null, null, null);

            //var t= ObjectFactory.GetInstance<IAfterSaleOrderRepository>().FindAll().ToList();

            //var s = new AfterSaleSkyWay()
            //{
            //    Id = 11,
            //    NewFlightNumber = "2",
            //    NewSeat = "3",
            //    NewStartDateTime = DateTime.Now
            //};
            //AutoMapper.Mapper.CreateMap<AfterSaleSkyWay, ResponseAfterSaleSkyWay>();
            //var n = AutoMapper.Mapper.Map<IList<AfterSaleSkyWay>, IList<ResponseAfterSaleSkyWay>>(new List<AfterSaleSkyWay> { s });

            // ResponseAfterSaleSkyWay

            //Mapper.CreateMap<AfterSaleSkyWay, ResponseAfterSaleSkyWay>();
            //Mapper.CreateMap<ChangeOrder, ResponseChangeOrder>();
            //var co = new ChangeOrder()
            //{
            //    SkyWay = new List<AfterSaleSkyWay>() { 
            //        new AfterSaleSkyWay()
            //        {
            //            Id = 1,
            //            NewFlightNumber = "11",
            //            NewSeat = "T",
            //            NewStartDateTime = DateTime.Now,
            //            SkyWayId = 2
            //        }
            //    }
            //};
            //var sw = new AfterSaleSkyWay()
            //{
            //    Id = 1,
            //    NewFlightNumber = "11",
            //    NewSeat = "T",
            //    NewStartDateTime = DateTime.Now,
            //    SkyWayId = 2
            //};
            //List<string> lists = new List<string>() { "0000004217" };
            //ser.GetNotTakeOffTicket(lists);
            //var o = Mapper.Map<ChangeOrder, ResponseChangeOrder>(co);
          //  ser.GetTicketInformationSummary("", "", "", "", "","", "", "", "",0, "", "", "", null, null, DateTime.Parse("2014-02-20"),DateTime.Now, null,null,null);
            //ser.GetTicketSalesStatistics("", "", "", "", "", "1", "", "", "", null, "", "", "", null, null, null, null, null,null,null);
            //ser.GetTicketInformationDetail("", "", "", "", "", "1", "", "", "", null, "", "", "", null, null, null, null, null, null,0,110,null);

        }
    }
}
