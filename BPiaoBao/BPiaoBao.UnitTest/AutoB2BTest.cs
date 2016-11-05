using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using BPiaoBao.AppServices;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.DomesticTicket.Domain.Services.B2BParam;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.UnitTest.DomesticTicket;
using JoveZhao.Framework.Helper;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using PnrAnalysis.Model;
using System.Text.RegularExpressions;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class AutoB2BTest
    {
        [TestMethod]
        public void TestB2B()
        {
            BootStrapper.Boot();
            AuthManager.SaveUser(new BPiaoBao.SystemSetting.Domain.Services.Auth.CurrentUserInfo()
            {
                Code = "w0001",
                BusinessmanName = "成都高新环球"
            });

            //IStationOrderService orderService = ObjectFactory.GetInstance<IStationOrderService>();
            //orderService.AddCoordinationDto("05402303911461036293", "121", "jkjkjkjj", false);

            //04750648053131210303

            //string strDetr = "";
            //string err = "";
            //PnrAnalysis.FormatPNR format = new PnrAnalysis.FormatPNR();
            ////string Pnr = format.GetPNR(strDetr, out err);
            ////TicketInfo ticketInfo = format.GetDetr(strDetr);
            //TicketInfo ticketInfo = format.GetDetrS(strDetr);


            //return;
            //IPidService PidService = ObjectFactory.GetInstance<PidService>();
            //bool ss = PidService.CancelPnr("5110270803", "CTU186", "HFT4P4");


            //QueryFlightService queryFlightService = new BPiaoBao.DomesticTicket.Domain.Services.QueryFlightService();
            //CabinData cabinData = queryFlightService.GetBaseCabinUsePolicy("MU");
            //List<string> aa = new List<string>();
            //List<CabinRow> list = cabinData.CabinList;
            //foreach (CabinRow item in list)
            //{
            //    if (!aa.Contains(item.Seat))
            //    {
            //        aa.Add(item.Seat);
            //    }
            //}

            //int nnn = aa.Count;
            //int mmm = list.Count;


            // CabinRow[] CabinRow = list.Union(list);

            //WebHttp http = new WebHttp();
            //string url = "";
            //DataResponse response = http.SendRequest(url, MethodType.GET, Encoding.Default, 60);
            //string strdata = response.Data;

            //TravelPaperService travelPaperService = ObjectFactory.GetInstance<TravelPaperService>();
            //travelPaperService.AddTravelPaper("test01", "000000001", "000000010", "CTU186", "123456", "测试公司", "11");


            //IBusinessmanRepository BusinessmanRepository = ObjectFactory.GetInstance<IBusinessmanRepository>();
            //IPidService IPidService = ObjectFactory.GetInstance<IPidService>();
            //FlightDestineService flightDestineService = new FlightDestineService(BusinessmanRepository, IPidService);
            //flightDestineService.GetLegStop("test01", "EU2241", DateTime.Parse("2014-07-30"));


            ////return;

            //DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
            //domesticService.AutoIssue("04979657898018392247", "测试");
            //Console.ReadLine();
            //AutoTicketService b2bTest = new AutoTicketService();
            //AutoEtdzParam autoEtdzParam = new AutoEtdzParam();
            //autoEtdzParam.IsLimitScope = true;
            ////autoEtdzParam.IsMulPrice = false;
            //autoEtdzParam.OldPolicyPoint = 2.8m;
            //autoEtdzParam.Pnr = "HV7SEK";
            //autoEtdzParam.BigPnr = "NZCLCM ";
            //autoEtdzParam.CarryCode = "HU";
            //autoEtdzParam.FlatformOrderId = "";
            //autoEtdzParam.PayInfo.PayAccount = "btsk01@sina.com";
            //autoEtdzParam.B2BAccount = "urc221";
            //autoEtdzParam.B2BPwd = "xdt08049440";
            //autoEtdzParam.PayInfo.PayTotalPrice = 1320;
            //autoEtdzParam.PayInfo.SeatTotalPrice = 98;
            //autoEtdzParam.PayInfo.TaxTotalPrice = 340;
            //autoEtdzParam.UrlInfo.AlipayAutoCPUrl = "http://210.14.138.26:6350/alidz.do";
            //autoEtdzParam.UrlInfo.AlipayTicketNotifyUrl = "http://210.14.138.26:91/Pay/PTReturnPage/AliPayNotifyUrl.aspx";
            //autoEtdzParam.UrlInfo.AlipayPayNotifyUrl = "http://210.14.138.26:91/Pay/PTReturnPage/AliPayNotifyUrl.aspx";
            //autoEtdzParam.UseAutoType = 0;

            //B2BResponse b2bResponse = new B2BResponse();
            //b2bResponse = b2bTest.NewQueryOrder(autoEtdzParam);
            //b2bResponse = b2bTest.NewQueryPriceByPnr(autoEtdzParam, QueryPolicyType.all);

            string strUrl = "http://210.14.138.26:6350/alidz.do";
            //bool IsRun = b2bTest.checkclt(strUrl);

            string xml = "";
            // b2bResponse = b2bTest.SyncNotifyXmlToModel(xml);

            //b2bResponse = b2bTest.SyncPayCall(xml);
            //b2bResponse = b2bTest.TicketOut(autoEtdzParam);


        }
    }
}
