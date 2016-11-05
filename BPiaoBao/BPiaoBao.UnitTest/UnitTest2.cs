using System.Collections.Generic;
using BPiaoBao.Common.Enums;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using JoveZhao.Framework.DDD;
using BPiaoBao.Common;
using StructureMap;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System.Linq;
using BPiaoBao.AppServices;
using BPiaoBao.AppServices.Hosting;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using BPiaoBao.AppServices.DomesticTicket;
using System;
using BPiaoBao.AppServices.StationContracts.SystemSetting;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using System.Xml.Linq;
using System.Xml;
using System.Text;
using System.Net;
using System.IO;

namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class UnitTest2
    {
        
        public string GetBackData(string m_baseurl, string Data, string m_doMetthod)
        {
            string rs = string.Empty;
            try
            {
                byte[] dt = Encoding.UTF8.GetBytes(Data);
                if (m_doMetthod == "GET")
                {
                    if (!string.IsNullOrEmpty(Data))
                        m_baseurl = m_baseurl + "?" + Data;
                }
                Uri uRI = new Uri(m_baseurl);
                HttpWebRequest req = WebRequest.Create(uRI) as HttpWebRequest;


                req.Method = m_doMetthod;
                if (m_doMetthod != "GET")
                {
                    req.KeepAlive = true;
                    req.ContentType = "application/x-www-form-urlencoded";
                    req.ContentLength = dt.Length;
                    req.AllowAutoRedirect = true;
                    Stream outStream = req.GetRequestStream();
                    outStream.Write(dt, 0, dt.Length);
                    outStream.Close();
                }

                HttpWebResponse res = req.GetResponse() as HttpWebResponse;

                Stream inStream = res.GetResponseStream();
                StreamReader sr = new StreamReader(inStream, Encoding.UTF8);
                rs = sr.ReadToEnd();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return rs;
        }
        [TestMethod]
        public void TestOpenBuyer()
        {
            BootStrapper.Boot();
            //IUnitOfWork unitOfWorkTicket = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
            //IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
            //IBusinessmanRepository repository = ObjectFactory.GetInstance<IBusinessmanRepository>();
            //IPaymentClientProxy proxy = ObjectFactory.GetInstance<IPaymentClientProxy>();
            //IConsoBusinessmanService service = new BusinessmanService(repository, proxy);
            //var buyer = new RequestBuyer
            //{
            //    CarrierCode = "GYS01",
            //    Code = "CGS001",
            //    ContactWay = new AppServices.ConsoContracts.SystemSetting.DataObjects.ContactWayDataObject { Address = "A", Contact = "B", Tel = "TEl" },
            //    DeductionGroupID = 1,
            //    Name = "CC"
            //};
            //AutoMapper.Mapper.Reset();
            //AutoMapper.Mapper.CreateMap<RequestBuyer, Buyer>()
            //    .ForMember(p => p.Attachments, opt => opt.Ignore())
            //    .ForMember(p => p.BuyDetails, opt => opt.Ignore())
            //    .ForMember(p => p.SendDetails, opt => opt.Ignore())
            //    .ForMember(p => p.SMS, opt => opt.Ignore())
            //    .ForMember(p => p.Operators, opt => opt.Ignore())
            //    .ForMember(p => p.DeductionGroupID, opt => opt.Ignore());
            //    //.ForMember(p => p.ContactWay, opt => opt.Ignore());

            //var model = AutoMapper.Mapper.Map<RequestBuyer, Buyer>(buyer);

            //Console.WriteLine(model.ToString());
            // service.OpenBuyer(buyer);
            string to = "15198226260";
            string message = "尊敬的[张超] 您好，您购买的[西安-成都] ，起飞时间[2014-07-31 20:30]，到达时间[2014-07-31 21:50]，航班号[3U8806],已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!【买票宝】尊敬的[张超] 您好，您购买的[西安-成都] ，起飞时间[2014-07-31 20:30]，到达时间[2014-07-31 21:50]，航班号[3U8806],已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!【买票宝】尊敬的[张超] 您好，您购买的[西安-成都] ，起飞时间[2014-07-31 20:30]，到达时间[2014-07-31 21:50]，航班号[3U8806],已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!【买票宝】尊敬的[张超] 您好，您购买的[西安-成都] ，起飞时间[2014-07-31 20:30]，到达时间[2014-07-31 21:50]，航班号[3U8806],已出票，请于航班起飞前90分钟到机场办理值机手续。祝您旅途愉快!【买票宝】";
            int sendCount = 0;
            bool sendState = false;
            string url = "http://122.4.79.43:2852/sms.aspx";
            string data = "action=send&userid=1190&account=mypb&password=mypb2014&mobile=" + to + "&content=" + message + "&sendTime=&checkcontent=1";
            string rs = GetBackData(url, data, "POST");

            XmlDocument xd = new XmlDocument();
            xd.LoadXml(rs);
            string state = xd.SelectSingleNode("returnsms/returnstatus/text()").Value;
            string count = xd.SelectSingleNode("returnsms/successCounts/text()").Value;
            string info = xd.SelectSingleNode("returnsms/message/text()").Value;
            if (state == "Success")
            {
                sendState = true;
                sendCount = int.Parse(count);
            }


            //IPaymentClientProxy proxy = ObjectFactory.GetInstance<IPaymentClientProxy>();
            //proxy.RefundCheck("0000000046", "201405291552034534323");
        }
        [TestMethod]
        public void MineTest()
        {
            BootStrapper.Boot();
            IUnitOfWork unitOfWorkTicket = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
            IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
            //IBusinessmanRepository repository = ObjectFactory.GetInstance<IBusinessmanRepository>();
            //IPaymentClientProxy proxy = ObjectFactory.GetInstance<IPaymentClientProxy>();
            //IStationBusinessmanService service = new BusinessmanService(repository, proxy, null);
            var orderRepository=ObjectFactory.GetInstance<IOrderRepository>();
            var businessmanRepository=ObjectFactory.GetInstance<IBusinessmanRepository>();
            var afterSaleRepository=ObjectFactory.GetInstance<IAfterSaleOrderRepository>();
            var deductionRepository=ObjectFactory.GetInstance<IDeductionRepository>();
            var ticketSumRepository=ObjectFactory.GetInstance<ITicketSumRepository>();
            //IStationOrderService orderService = new OrderService(orderRepository, businessmanRepository
            //, new BPiaoBao.DomesticTicket.Domain.Services.DomesticService(orderRepository, businessmanRepository, deductionRepository), afterSaleRepository, ticketSumRepository);
            //orderService.SynOrder("05750547197214516686");
            //var carrier = new Requ05750547197214516686estCarrier()
            //{
            //    Code = "GYS01",
            //    Name = "成都供应商",
            //    ContactWay = new BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.ContactWayDataObject
            //    {
            //        Address = "成都青羊宫",
            //        Contact = "彭英杰",
            //        Tel = "15198226260"
            //    },
            //    CashbagCode = "aaabb",
            //    CashbagKey = "ccccc",
            //    Pids = new List<BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.PIDDataObject> { 
            //        new BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap.PIDDataObject{
            //            IP="127.0.0.1",
            //            Port=80,
            //            Office="abcd"
            //        }
            //    }
            //};
            //RequestCarrier rc = new RequestCarrier()
            //{
            //    ContactWay = new AppServices.StationContracts.SystemSetting.SystemMap.ContactWayDataObject()
            //};
            //service.AddBussinessmen(rc);
        }
        [TestMethod]
        public void MinTest1()
        {
            //BootStrapper.Boot();
            //IUnitOfWork unitOfWorkTicket = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
            //IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
            //IBusinessmanRepository repository = ObjectFactory.GetInstance<IBusinessmanRepository>();
            //IPaymentClientProxy proxy = ObjectFactory.GetInstance<IPaymentClientProxy>();
            //IStationBusinessmanService service = new BusinessmanService(repository, proxy);
            //var list = service.FindCarrier(null, null, null, null, null, 0, 10);

        }
        [TestMethod]
        public void TestMethod1()
        {
            BootStrapper.Boot();
            IUnitOfWork unitOfWorkTicket = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
            IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
            IOrderRepository orderRepository = ObjectFactory.GetInstance<IOrderRepository>();
            IAfterSaleOrderRepository afterSaleOrderRepository = ObjectFactory.GetInstance<IAfterSaleOrderRepository>();
            ITicketSumRepository ticketSumRepository = ObjectFactory.GetInstance<ITicketSumRepository>();
            List<TicketSum> list = new List<TicketSum>();
            AuthManager.SaveUser(new SystemSetting.Domain.Services.Auth.CurrentUserInfo
            {
                Code = "00",
                BusinessmanName = "航旅业测试",
                CashbagCode = "0000000088",
                CashbagKey = "17481a756353499794bd4fbf39d61339",
                OperatorAccount = "admin",
                OperatorName = "admin",
                Type = "Carrier"
            });

            //orderRepository.FindAll(p => p.OrderStatus == EnumOrderStatus.IssueAndCompleted).ToList().ForEach(p =>
            //{
            //    list.AddRange(new TicketEventHelper().GetTicketSums(p));
            //});
            //afterSaleOrderRepository.FindAll(p => (p is AnnulOrder || p is BounceOrder || p is ChangeOrder) && p.ProcessStatus == EnumTfgProcessStatus.Processed).ToList().ForEach(p =>
            //{
            //    list.AddRange(new TicketEventHelper().GetTicketSums(p, p.Order.SkyWays.ToList()));

            //});
            //int i = 0;
            //list.ForEach(p =>
            //{
            //    i++;
            //    unitOfWorkRepository.PersistCreationOf(p);
            //    if (i % 10 == 0)
            //        unitOfWorkTicket.Commit();
            //});
            //unitOfWorkTicket.Commit();

            IStationOrderService service = ObjectFactory.GetInstance<OrderService>();

            service.SingleRefund(3398, "201407231000297424051");

            //Dictionary<int, decimal> dic = new Dictionary<int, decimal>();
            //dic.Add(61694, 0);
            //service.Process(3026, dic, "");


        }

        [TestMethod]
        public void TestDecimal()
        {
            CashbagPaymentClientProxy service = ObjectFactory.GetInstance<CashbagPaymentClientProxy>();


            //service.PaymentProfitByCashAccount();

        }
        [TestMethod]
        public void TestPid()
        {
            BootStrapper.Boot();
            AuthManager.SaveUser(new SystemSetting.Domain.Services.Auth.CurrentUserInfo
            {
                Code = "5110251984",
                BusinessmanName = "曾波",
                CashbagCode = "0000000088",
                CashbagKey = "17481a756353499794bd4fbf39d61339",
                OperatorAccount = "admin",
                OperatorName = "admin",
                Type = "Carrier",
                CarrierCode = "cd001"
            });
            //PidService pidService = ObjectFactory.GetInstance<PidService>();
            TravelPaperService travelPaperServiceService = ObjectFactory.GetInstance<TravelPaperService>();
            BPiaoBao.AppServices.DataContracts.DomesticTicket.TravelAppRequrst req = new AppServices.DataContracts.DomesticTicket.TravelAppRequrst();
            req.CreateOffice = "CTU186";
            req.OrderId = "04998506676414985905";
            req.TicketNumber = "7844891506403";
            req.TripNumber = "6351430995";
            req.PassengerId = 13528;
            travelPaperServiceService.VoidTrip(req);
            // pidService.GetFlow("", "CTU186");
        }
    }
}
