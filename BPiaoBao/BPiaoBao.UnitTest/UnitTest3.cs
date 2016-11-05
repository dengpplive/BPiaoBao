using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.AppServices.Hosting;
using BPiaoBao.AppServices.SystemSetting;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using StructureMap;
using Newtonsoft.Json.Linq;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using BPiaoBao.AppServices;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.Contracts.DomesticTicket;
using PiaoBao.BTicket.EFRepository;
using BPiaoBao.Common.Enums;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.DomesticTicket.Domain.Services;
using BPiaoBao.DomesticTicket.Domain.Models;
using BPiaoBao.AppServices.StationContracts.DomesticTicket;
using PnrAnalysis;
using System.Text.RegularExpressions;
using System.Data;
using System.Xml;
using System.IO;
using BPiaoBao.DomesticTicket.Domain.Services.B2BParam;
using BPiaoBao.Common;
using System.Diagnostics;
using PnrAnalysis.Model;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using BPiaoBao.DomesticTicket.Platforms._Today;
using BPiaoBao.DomesticTicket.Domain.Services.TodayObject;

namespace BPiaoBao.UnitTest
{


    [TestClass]
    public class UnitTest3
    {
        [TestMethod]
        public void TestPid1()
        {
            BootStrapper.Boot();
            PidService pidService = ObjectFactory.GetInstance<PidService>();
            RequestSplitPnrInfo request = new RequestSplitPnrInfo();
            List<SplitPassenger> plist = new List<SplitPassenger>();
            request.SplitPasList = plist;
            request.BusinessmanCode = "003";
            request.Office = "ctu186";
            request.Pnr = "HYCE5L";

            plist.Add(new SplitPassenger()
            {
                PassengerName = "王雪梅"
            });
            //plist.Add(new SplitPassenger()
            //{
            //    PassengerName = "王华"
            //});

            ResposeSplitPnrInfo response = pidService.SplitPnr(request);
        }
        [TestMethod]
        public void TestToday()
        {
            //string result = "<?xml version=\"1.0\" encoding=\"gb2312\"?><Result Result=\"T\"></Result>";           
            //result = Regex.Replace(result, @"<\?xml.*?\?>", "");
            //result = string.Format("<?xml version=\"1.0\" encoding=\"utf-8\" ?><Result>{0}</Result>", result);
            //DataSet ds = XmlToDataSet(result);
            BootStrapper.Boot();
            //OrderService orderService = ObjectFactory.GetInstance<OrderService>();
            //orderService.SynOrder("05502552962983841882");

            //DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
            // domesticService.CreatePlatformOrderAndPaid("04862100682642257060", "测试1", "", true);

            //_TodayPlatform today = new _TodayPlatform();
            //TodayTuiFeiOrderRequest request = new TodayTuiFeiOrderRequest();
            //request._Type = "A";
            //request._OrderNo = "W2014112504020990138";
            //request._Repeal = "1";
            //request._PersonName = "安山彤";
            //request._IsCancelSeat = "是";
            //request._Cause = "A";
            //request._Remarks = "04862100682642257060";
            //request._Rnum = 1;
            //request._TicketNo = "021-1234567893";
            //request._Ramount = 0.03m;
            //today.Today_TuiFeiOrder("ctu", "04862100682642257060", request);

            //today.CancelOrder("ctu", "W2014112004018055769", "HW5JZ1", "支付方式错误", "");
            //string orderstats = today.GetOrderStatus("ctu", "", "W2014112004018055769", "HW5JZ1");
        }
        [TestMethod]
        public void TestData()
        {
            DataBill dataBill = new DataBill();
            decimal d = 181.00m;
            decimal ff = 0m;
            ff = dataBill.CeilAddTen(d);
        }
        [TestMethod]
        public void TestQT()
        {


            /*
            string strqt = @"          

                    QT CTU186   

    GQ 0165 0200    RP 0170 0200    KK 3512 3605    RE 0000 0200

    SR 0161 0200    TC 1489 1435    TL 3396 3028    SC 0000 0253
";
            FormatPNR format = new FormatPNR();
            QT_Queue qt = format.FormatQT(strqt);
            string err = string.Empty;
            string pnrcontent = @"     CTU186 SCHEDULE CHG  (  0000  )  (  0000  )

      **ELECTRONIC TICKET PNR** 

     1.赵恩宇 HVVNFY

     2.  G52638 G   WE15OCT  KWECKG UN1   1525 1610          E      S   

     3.  G52638 G   WE15OCT  KWECKG TK1   1700 1745          E      S   

     4.CTU/T CTU/T 028-85512345/CTU HUA LONG AIR SERVICE CO.,LTD/YANGHONG ABCDEFG   

     5.REM 1014 1758 T0565  

     6.TL/2300/14OCT14/CTU295   

     7.SSR FOID G5 HK1 NI511129197011014039/P1  

     8.SSR ADTK 1E BY CTU14OCT14/1859 OR CXL G52638 G15OCT  

     9.SSR TKNE G5 HK1 KWECKG 2638 G15OCT 9872900692422/1/P1

    10.OSI G5 CTCT15184422090  ";
            string strPnr = format.GetPNR(pnrcontent, out err);
            */

            BootStrapper.Boot();
            IPidService pidService = ObjectFactory.GetInstance<IPidService>();
            PIDInfo pid = new PIDInfo();
            pid.CarrierCode = "ctuadmin";
            pid.IP = "210.14.138.30";
            pid.Port = 2237;
            pid.Office = "ctu186";
            QTResponse response = pidService.SendQT(pid);
            //List<string> tkList = new List<string>() { 
            //    "876-2329295351",
            //    "876-2330330542",
            //    "7842119018923",
            //    "876-2330343865",
            //    "876-2330365214",
            //    "876-2330372379"
            //};
            //OpenTicketResponse response = pidService.ScanOpenTicket("210.14.138.30", "2237", "ctu186", tkList);


        }

        [TestMethod]
        public void TestOrder()
        {
            string pnrContent = @"RT JN4F74
  CA4558  H FR24OCT  NKGCKG HK1   1930 2130
JN4F74 -   航空公司使用自动出票时限, 请检查PNR
  *** 预订酒店指令HC, 详情   HC:HELP   ***
 


 1.郑静 JN4F74
 2.  CA4558 H   FR24OCT  NKGCKG HK1   1930 2130          E --T2
 3.CTU/T CTU/T 028-85512345/CTU HUA LONG AIR SERVICE CO.,LTD/YANGHONG ABCDEFG
 4.REM 1023 1122 CNC006 18980808222
 5.TL/2200/23OCT/CTU186
 6.SSR FOID CA HK1 NI51021519760618253X/P1
 7.SSR OTHS 1E CA BKG CXLD DUE TO TKT TIME EXPIRED
 8.SSR FQTV CA HK1 NKGCKG 4558 H24OCT CA008371309901/P1
 9.SSR ADTK 1E BY CTU23OCT14/1328 OR CXL CA ALL SEGS
10.OSI YY CTCT18980808222
11.RMK CA/MTJ923
12.RMK TJ AUTH NKG192                                                          +
 


";
            //FormatPNR format = new FormatPNR();
            //string err = "";
            //string pnr = format.GetPNR(pnrContent, out err);


        }

        [TestMethod]
        public void Test2()
        {
            BootStrapper.Boot();
            string pnrContent = @" 1.陈绪炎 2.陈耀胜 3.李祖新 4.逄增奎 5.王勇 6.易小平    HTFT12     7.  CA4487 Y   SU02NOV  CTUJZH HK6   1405 1500          E T2--   8.CTU/T CTU/T 028-85512345/CTU HUA LONG AIR SERVICE CO.,LTD/YANGHONG ABCDEFG     9.TL/1405/02NOV/CTU186  10.SSR FOID CA HK1 NI420583198402223411/P6   11.SSR FOID CA HK1 NI222426198404075215/P4   12.SSR FOID CA HK1 NI420523196512060013/P3   13.SSR FOID CA HK1 NI422723196608154631/P2   14.SSR FOID CA HK1 NI422723196309291919/P1   15.SSR FOID CA HK1 NI422723196611120037/P5   16.SSR FQTV CA HK1 CTUJZH 4487 Y02NOV CA002810349584/P1                        + 17.SSR ADTK 1E BY CTU30OCT14/1250 OR CXL CA ALL SEGS                           - 18.OSI CA CTCM18215523063/P5 19.OSI CA CTCT13518105618    20.RMK CA/NW2W0K 21.CTU186       >PAT:A   01 Y FARE:CNY1310.00 TAX:CNY50.00 YQ:CNY50.00  TOTAL:1410.00  SFC:01    SFN:01       ";
            PnrData pnrData = PnrHelper.GetPnrData(pnrContent);
            string PolicyId = "13703423";
            PlatformPolicy platformPolicy = PlatformFactory.GetPlatformByCode("517").GetPoliciesByPnrContent(pnrContent, true, pnrData).Find((p) => p.Id == PolicyId);
            return;


            OrderService IOrderService = ObjectFactory.GetInstance<OrderService>();

            AuthManager.SaveUser(new BPiaoBao.SystemSetting.Domain.Services.Auth.CurrentUserInfo()
          {
              Code = "5105241974",
              OperatorAccount = "梦之旅",
              OperatorName = "陈良文"
          });


            IOrderService.CreateInterfaceOrder("517", "04787578460867039561");
        }

        [TestMethod]
        public void TestLength()
        {
            //string strSource = "02 00 00 46 03 01 67 4E 3F 96 57 A1 D0 BB 15 13   F4 67 A4 5C 01 29 B2 72 65 C5 99 80 5B 7B 7A 29   70 42 B8 AE ED 65 20 4B 4B 4F F0 26 28 EE 1D E3    70 03 ED 8A D6 64 9D D3 31 7D AF 1B FA 7E 56 A5   C0 26 B5 64 A0 0B 8A 00 09 00  ";
            //FormatPNR pnrformat = new FormatPNR();
            //string strDesc = pnrformat.SplitInsertChar(strSource, 46, "\r\n");
            string strPath = "D:\\a.txt";
            TextWriterTraceListener trace = new TextWriterTraceListener(strPath);
            Trace.Listeners.Add(trace);
            Trace.WriteLine("aaaaa");
        }

        [TestMethod]
        public void Test()
        {
            //var m1 = 37.56;
            //var m2 = 37.06;
            //var m3 = 37.66;
            //Console.WriteLine(Math.Floor(m1));
            //Console.WriteLine(Math.Floor(m2));
            //Console.WriteLine(Math.Round(m3));

            //DataBill dataBill = new DataBill();
            //decimal f = dataBill.GetCommissionPartNumber(25, 600, 0);
            //string PassengerName = "陈芷墨";
            //string pinyin = "";
            //if (PinYingMange.IsChina(PassengerName))
            //{
            //    pinyin = PinYingMange.GetSpellByChinese(PassengerName.Substring(0, 1)) + "/" + PinYingMange.GetSpellByChinese(PinYingMange.RepacePinyinChar(PassengerName.Substring(1)));
            //}
            //else
            //{
            //    pinyin = PassengerName;
            //}
            //EnumIssueTicketWay a = EnumIssueTicketWay.Manual;
            //string aaaa = a.ToString();

        }


        [TestMethod]
        public void GetList()
        {
            BootStrapper.Boot();
            BehaviorStatService service = ObjectFactory.GetInstance<BehaviorStatService>();
            var result = service.Query(new RequestQueryBehaviorStatQuery()
            {
                // BusinessmanCode = "100100",
                BusinessmanType = "buyer",
                StartDateTime = DateTime.Now.Date,
                EndDateTime = DateTime.Now.Date,
                PageIndex = 1,
                PageSize = 10
            });

            var str = JsonConvert.SerializeObject(result);

            Console.WriteLine(str);

        }


        [TestMethod]
        public void TestMoney()
        {

            decimal testA = 0.0550m;
            decimal A = Math.Round(testA, 2, MidpointRounding.AwayFromZero);
        }

        [TestMethod]
        public void TestSaveBehaviorStat()
        {
            BootStrapper.Boot();
            BehaviorStatService.SaveBehaviorStat(EnumBehaviorOperate.LoginCount);
            //BehaviorStatService.SaveBehaviorStat("100100", "Test", "Buyer","GYS01", EnumBehaviorOperate.LoginCount);
            //BehaviorStatService.SaveBehaviorStat("100100", "Test", "Buyer","GYS01", EnumBehaviorOperate.LoginCount);
            //BehaviorStatService.SaveBehaviorStat("100100", "Test", "Buyer","GYS01", EnumBehaviorOperate.LoginCount);
            //BehaviorStatService.SaveBehaviorStat("100100", "Test", "Buyer","GYS01", EnumBehaviorOperate.LoginCount); 
            //BehaviorStatService.SaveBehaviorStat("100100", "Test", "Buyer","GYS01", EnumBehaviorOperate.LoginCount);
            //BehaviorStatService.SaveBehaviorStat("100200", "Test2", "Supplier", "GYS01", EnumBehaviorOperate.LoginCount);
            //BehaviorStatService.SaveBehaviorStat("100300", "Test3", "Supplier", "GYS01", EnumBehaviorOperate.LoginCount);

        }
        private string GetOrderId(string prxFix)
        {
            byte[] buffer = Guid.NewGuid().ToByteArray();
            long l = BitConverter.ToInt16(buffer, 0);
            return prxFix + l.ToString();
        }
        [TestMethod]
        public void TestOrderId()
        {
            string aa = GetOrderId("0");

        }

        [TestMethod]
        public void TestPid()
        {
            string ss = "";

            //BootStrapper.Boot();
            //AuthManager.SaveUser(new BPiaoBao.SystemSetting.Domain.Services.Auth.CurrentUserInfo()
            //{
            //    Code = "5131010006",
            //    OperatorAccount = "何静",
            //    OperatorName = "苏苏"
            //});
            ////OrderService orderService = ObjectFactory.GetInstance<OrderService>();
            //FlightService flightService = ObjectFactory.GetInstance<FlightService>();
            //TravelAppRequrst travelAppRequrst = new TravelAppRequrst()
            //{
            //    CreateOffice = "CTU186",
            //    OrderId = "05562703510376942401",
            //    PassengerId = 149425,
            //    TicketNumber = "7842145067778",
            //    TripNumber = "6377705171"
            //};
            //TravelAppResponse travelAppResponse = flightService.VoidTrip(travelAppRequrst);
            //return;

            //OrderDto orderdto = orderService.ChoosePolicy("517", "9789867~", "05747392918379086547");

            //OrderDto orderdto = orderService.ChoosePolicy("05747392918379086547");

            // DomesticService domesticService = ObjectFactory.GetInstance<DomesticService>();
            // domesticService.AutoIssue("5453763231711235650", "婴儿测试");
            // domesticService.CreatePlatformOrderAndPaid("04882623127929209734", "11", "测试");

            //string pnrContent = @"1.黄月群 2.兰崇军 3.罗彬 4.宋小平 5.王荟 6.徐铭 7.杨中 8.张训华 9.朱光甫 JT1NN410.  CZ3447 M   TH18SEP  WUHCTU HK9   0810 0950          E --T211.CTU/T CTU/T 028-85512345/CTU HUA LONG AIR SERVICE CO.,LTD/YANGHONG ABCDEFG12.REM 0904 1646 SHENYI0113.BY OPT 8835 2014/09/04 1646A14.TL/1946/04SEP/CTU18615.SSR FOID CZ HK1 NI513822198608114841/P116.SSR FOID CZ HK1 NI510104198112264113/P917.SSR FOID CZ HK1 NI513622198312050315/P218.SSR FOID CZ HK1 NI511524198908080078/P819.SSR FOID CZ HK1 NI510524198509070779/P320.SSR FOID CZ HK1 NI510522197405120973/P421.SSR FOID CZ HK1 NI510104198012084107/P522.SSR FOID CZ HK1 NI513027197605264610/P623.SSR FOID CZ HK1 NI510802197409130715/P724.SSR ADTK 1E BY CTU05SEP14/1646 OR CXL CZ BOOKING25.OSI CZ CTCT02896567                                                         +26.RMK CA/MBSKT1                                                               -27.CTU186
            //";
            // pnrContent = @" 1.尚福乐  JFBNCZ\r 2.  CA4193 G   WE19NOV  CTUPEK HK1   0700 0935          E T2T3                \r 3.SHE/T SHE/T024-86853111/SHENYANGZHONGYUANGUOJIHANGKONGFUWUYOUXIANGONGSI/    \r    /MENG JING TAN ABCDEFG\r 4.013940582959\r 5.TL/0530/19NOV/SHE185\r 6.SSR FOID CA HK1 NI211322198509260317/P1\r 7.SSR ADTK 1E BY SHE12NOV14/0700 OR CXL CA ALL SEGS\r 8.OSI YY CTCT013940582959\r 9.OSI CA CTCT13541362279\r10.OSI CA CTCM15820648302/P1                                                  +3.4%利润2.80支付总金额861.20\r支付出票请登录: HTTP://WWW.86853111.NET\r 重要通知:所有用户不得加价销售机票,如有航司罚款追讨赔偿 \r11.OSI CA CTCT13940582959                                                     -\r12.RMK CA/MX6F2H\r13.RMK QQ1024593972\r14.RMK TLWBINSD\r15.SHE1853.4%利润2.80支付总金额861.20\r支付出票请登录: HTTP://WWW.86853111.NET\r 重要通知:所有用户不得加价销售机票,如有航司罚款追讨赔偿 \r\r";
            //            pnrContent = @">PAT:A  
            //01 G FARE:CNY440.00 TAX:CNY50.00 YQ:CNY60.00  TOTAL:550.00  
            // SFC:01    SFN:01   
            //02 G/CA4Z142374 FARE:CNY400.00 TAX:CNY50.00 YQ:CNY60.00  TOTAL:510.00   
            // SFC:02    SFN:02   
            //";
            string err = "";
            string pnrContent = @"1.代涛 2.张娟 HVR0VP   3.  MU5856 B   TH23OCT  CTUKMG HK2   1540 1710          E T2--   4.CTU/T CTU/T028-85555666/CHENG DU FENG XU HANG KONG PIAO WU FU WU YOU XIAN       GONG SI/LIUX ABCDEFG   5.REM 1023 0844 JPSG02 13658034125   6.023-62973108   7.TL/1000/23OCT/CTU373   8.SSR FOID MU HK1 NI522323197508170526/P2   9.SSR FOID MU HK1 NI53012819961127151X/P1  10.SSR CKIN MU  11.SSR FQTV MU HK1 CTUKMG 5856 B23OCT MU660283321266/P2  12.SSR ADTK 1E BY CTU23OCT14/1216 OR CXL MU5856 B23OCT  13.OSI MU CTCT13436009296  14.RMK CA/MEV5DC  15.RMK TJ AUTH CTU186  16.CTU373           >PAT:A  01 B FARE:CNY980.00 TAX:CNY50.00 YQ:CNY50.00  TOTAL:1080.00   SFC:01";
            PnrAnalysis.FormatPNR format = new FormatPNR();
            //PatModel pat = format.GetPATInfo(pnrContent, out err);


            //List<string> strList = format.NewSplitPnr(pnrContent);
            //string err = "";
            //PnrAnalysis.Model.SplitPnrCon splitPnrCon = format.GetSplitPnrCon(pnrContent);
            //string RTCon = splitPnrCon.RTCon;
            //string PatCon = splitPnrCon.AdultPATCon != string.Empty ? splitPnrCon.AdultPATCon : splitPnrCon.ChdPATCon;
            //PatModel pat = format.GetPATInfo(PatCon, out err);
            //PnrModel pnrmodel = format.GetPNRInfo("JT1NN4", pnrContent, false, out err);

            //////去重复和备注
            //string pnrRemark = string.Empty;
            //RTCon = format.DelRepeatRTCon(RTCon, ref pnrRemark);
            //if (!string.IsNullOrEmpty(pnrRemark))
            //{
            //    PatCon = PatCon.Replace(pnrRemark, "");
            //}

            // PnrModel pnr = pnrformat.GetPNRInfo("JXNWW5",pnrContent,false,out err);

            return;

            //var  domesticService.PnrIsPay();
            //domesticService.SetOrderStatusInvalid("", "JGKK47", "05304779119416242765");
            // ss = "O FM:1PEK 3U    8886  P 06AUG 1520 OK HCY                      20K OPEN FOR USE      T3T1 RL:NKX89L  /";
            // string strSky = @"(\s*O\s*(?<dentity>FM|TO)\:(?<fnum>\d)(?<from>[A-Za-z]{3})\s*(?<carray>[a-zA-Z0-9]{2})\s*(?<flightno>\d{3,5})\s*(?<seat>[A-Za-z]{1}\d?)\s*(?<date>\d{2}[A-Za-z]{3}(\d{2})?)\s*(?<startTime>[^\d]{0,}\d{4})\s*OK\s*[A-Za-z]{1}\d?\s*((.*\/.*)|[a-zA-Z]{3}\d{1,2}|[a-zA-Z]{2})?\s*(?<packet>\d{2}K)\s*(?<status>.*)\s*(?<eterm>[T|\d|\-]{4})?\s*(.{3})(?<bigpnr>[a-zA-Z0-9]{6})?\s*.((?<pnr>[a-zA-Z0-9]{6})\s*.{2})?)";
            //// string strExchData = @"(?<=\s*EXCH\:\s*(?<tk>\d{3,4}[-]?\d{10})\s*CONJ\s*TKT\:.*?\s*)(?<exchData>[\s|\S]+)\s*TO\:\s*(?<to>[A-Za-z]{3})\s*";
            // //航段
            // Match match = Regex.Match(ss, strSky, RegexOptions.Compiled | RegexOptions.Multiline | RegexOptions.IgnoreCase);

            // if (match.Success)
            // {

            // }
            // return;
            //PnrAnalysis.FormatPNR format = new PnrAnalysis.FormatPNR();
            // format.GetDetr(ss);
            //format.GetDetrS(ss);
            //AuthManager.SaveUser(new BPiaoBao.SystemSetting.Domain.Services.Auth.CurrentUserInfo()
            //{
            //    Code = "5110270005",
            //    OperatorAccount = "徐均",
            //    OperatorName = "四川华电航空票务服务公司翔安分公司"
            //});

            //   OrderService orderService = ObjectFactory.GetInstance<OrderService>();
            //  string aaa = orderService.QueryPayStatus("04809550518362161518");
            //return;
            //orderService.AddCoordinationDto("05402303911461036293", "121", "jkjkjkjj", false);
            //string msg = "";
            //string aaa = format.GetPnrStatus(ss, out msg);



            //04750648053131210303

            // PolicyService policyService = ObjectFactory.GetInstance<PolicyService>();
            // policyService.test();

            //PolicyQueryParam policyQueryParam = new PolicyQueryParam();
            //policyQueryParam.TravelType = TravelType.Oneway;
            //policyQueryParam.InputParam = new List<QueryParam>();
            //policyQueryParam.InputParam.Add(new QueryParam()
            //{
            //    FromCode = "CTU",
            //    ToCode = "PEK",
            //    FlyDate = "2014-06-20"
            //});
            //List<PolicyCache> policyCacheList = policyService.GetFlightPolicy("123", policyQueryParam);



            //return;


            //PidService pidService = ObjectFactory.GetInstance<PidService>();
            //string result = pidService.GetAV("test01", "ctu", "pek", "", "2014-08-20", "0000");
            // string strData = pidService.GetPnrAndTickeNumInfo("c1001", "", "CTU324");
            // OrderService orderService = ObjectFactory.GetInstance<OrderService>();
            //orderService.AddCoordinationDto();

            // currentUser = AuthManager.GetCurrentUser();

            //AuthManager.SaveUser(new BPiaoBao.SystemSetting.Domain.Services.Auth.CurrentUserInfo()
            //{
            //    Code = "",
            //    OperatorAccount = ""
            //});

            // PolicyPack pack = orderService.GetPolicyList("05407898320500536131");

            //RequestSplitPnrInfo request = new RequestSplitPnrInfo();
            //request.BusinessmanCode = "c7516";
            //request.Office = "CTU186";
            //request.Pnr = "HQG9FS";
            //request.SplitPasList = new List<SplitPassenger>() { 
            //    new  SplitPassenger(){
            //         PassengerName="张阳"

            //    }
            //};
            //ResposeSplitPnrInfo response = pidService.SplitPnr(request);

            //            string strPnr = @"  3U8737  L TH17JUL  CTUCAN DK1   0715 0930 
            //HFEMDE -   航空公司使用自动出票时限, 请检查PNR  
            //  *** 预订酒店指令HC, 详情   HC:HELP   ***  
            //";

            //            strPnr = @" 1.李国瑞 HZPGDY
            // 2.  3U8701 E   TH26JUN  CTUSZX HK1   0705 0915          E T1T3 
            // 3.CTU/T CTU/T 028-5566222/CTU QI MING INDUSTRY CO.,LTD/TONG LILI ABCDEFG   
            // 4.TL/0605/26JUN/CTU324 
            // 5.SSR FOID 3U HK1 NI220122198205162232/P1  
            // 6.SSR FQTV 3U HK1 CTUSZX 8701 E26JUN 3U981209305/C/P1  
            // 7.OSI 3U CTCM18608021562/P1
            // 8.OSI 3U CTCT13541362279   
            // 9.RMK CA/MT0CZB
            //10.CTU324   
            // 1.李国瑞 HZPGDY
            // 2.  3U8701 E   TH26JUN  CTUSZX HK1   0705 0915          E T1T3 
            //     -CA-MT0CZB 
            // 3.CTU/T CTU/T 028-5566222/CTU QI MING INDUSTRY CO.,LTD/TONG LILI ABCDEFG   
            // 4.TL/0605/26JUN/CTU324 
            // 5.SSR FOID 3U HK1 NI220122198205162232/P1  
            // 6.SSR FQTV 3U HK1 CTUSZX 8701 E26JUN 3U981209305/C/P1  
            // 7.OSI 3U CTCM18608021562/P1
            // 8.OSI 3U CTCT13541362279   
            // 9.RMK CA/MT0CZB
            //10.CTU324   
            //";
            //            string errMsg = "";
            //            //string Pnr = format.GetBigCode(strPnr, out errMsg);
            //            string strData = format.DelRepeatRTCon(strPnr, ref errMsg);
            //            //decimal DownPoint = 6.9m;
            //            //string d = DownPoint.ToString("F2");
            //            return;


            //            BusinessmanRepository businessmanRepository = ObjectFactory.GetInstance<BusinessmanRepository>();
            //            FlightDestineService flightDestineService = ObjectFactory.GetInstance<FlightDestineService>();
            //            //new FlightDestineService(businessmanRepository);
            //            DestineRequest destine = new DestineRequest();
            //            destine.Passengers = new PassengerRequest[] { 
            //                //成人
            //                new PassengerRequest(){
            //                     CardNo="1236547890",
            //                     ChdBirthday=System.DateTime.Now,
            //                     LinkPhone="13610001236",
            //                     MemberCard="",
            //                     PassengerName="刘飞",
            //                     PassengerType=1                      
            //                }
            //                /*,
            //                //儿童
            //                new PassengerRequest(){
            //                     CardNo="420821198710072011",
            //                     ChdBirthday=System.DateTime.Now,
            //                     LinkPhone="15928636275",
            //                     MemberCard="",
            //                     PassengerName="王冰",
            //                     PassengerType=2
            //                },
            //                 //婴儿
            //                new PassengerRequest(){
            //                     CardNo="652328194203150011",
            //                     ChdBirthday=System.DateTime.Now,
            //                     LinkPhone="15928636271",
            //                     MemberCard="",
            //                     PassengerName="王婴",
            //                     PassengerType=3
            //                }*/
            //            };
            //            destine.SkyWay = new DestineSkyWayRequest[]{
            //                new DestineSkyWayRequest(){
            //                       CarrayCode="CA",
            //                       FlightNumber="1405",
            //                       StartDate=DateTime.Parse("2014-12-01 07:00:00"),
            //                       EndDate=DateTime.Parse("2014-12-01 09:45:00"),
            //                       FromCityCode="PEK",
            //                       ToCityCode="CTU"  ,
            //                       Seat="Y"                
            //                }            
            //            };
            //            destine.Tel = "13610001000";
            //            //flightDestineService.Destine(destine);
            //            PolicyPack pp = orderService.Destine(destine, Common.Enums.EnumDestineSource.MobileDestine);

            //            return;
            //            PnrImportParam pnrImportParam = new PnrImportParam();
            //            pnrImportParam.PnrAndPnrContent = @" 1.王芬 JZ5WGL  
            // 2.  3U8881 Y   MO28JUL  CTUPEK HK1   0730 1005          E T1T3 
            // 3.CTU/T CTU/T 028-85512345/CTU HUA LONG AIR SERVICE CO.,LTD/YANGHONG ABCDEFG   
            // 4.TL/0600/28JUL/CTU186 
            // 5.SSR FOID 3U HK1 NI420821198710072018/P1  
            // 6.SSR ADTK 1E BY CTU16JUN14/1418 OR CXL 3U8881 Y28JUL  
            // 7.OSI 3U CTCM15928636274/P1
            // 8.RMK CA/MF82RZ
            // 9.CTU186   
            //>PAT:A  
            //01 Y FARE:CNY1440.00 TAX:CNY50.00 YQ:CNY120.00  TOTAL:1610.00   
            // SFC:01 
            //";
            //            pnrImportParam.PnrImportType = EnumPnrImportType.GenericPnrImport;
            //            pnrImportParam.OldOrderId = "";
            //            PolicyPack policyPack = orderService.ImportPnrContext(pnrImportParam);


            //            ConsoLocalPolicyService policy = ObjectFactory.GetInstance<ConsoLocalPolicyService>();
            //            CabinData cabinData = policy.GetBaseCabinData("CA");


        }
        /// <summary>
        /// 将本票通返回的XML转换成数据集
        /// </summary>       
        /// <returns></returns>
        private DataSet XmlToDataSet(string xmlContent)
        {
            DataSet ds = new DataSet();
            try
            {
                if (!string.IsNullOrEmpty(xmlContent))
                {
                    //去掉不可见字符                        
                    //xmlContent = format.RemoveHideChar(xmlContent);
                    if (!xmlContent.Trim().ToLower().StartsWith("<?xml"))
                    {
                        xmlContent = "<?xml version=\"1.0\" encoding=\"utf-8\" ?>" + xmlContent;
                    }
                    StringReader rea = new StringReader(xmlContent);
                    XmlTextReader xmlReader = new XmlTextReader(rea);
                    ds.ReadXml(xmlReader);
                }
            }
            catch
            {
            }
            return ds;
        }
        [TestMethod]
        public void TestAuto()
        {

            //PnrAnalysis.PnrResource pnrs = new PnrResource();
            //CityInfo cinfo = pnrs.CityDictionary.CityList.Where(p => p.key == "HCJ").FirstOrDefault();
            //return;
            //PnrAnalysis.SendNewPID sendAuto = new PnrAnalysis.SendNewPID();
            //PnrAnalysis.Model.BSPParam bsp = new PnrAnalysis.Model.BSPParam();
            //bsp.CarrayCode = "3U";
            //bsp.Pnr = "HPJXX9";
            //bsp.CpPrice = 1150m;
            //bsp.PrintNo = "10";
            //bsp.Param.ServerIP = "10.11.5.251";
            //bsp.Param.ServerPort = 399;
            //bsp.Param.Office = "CTU186";
            //bsp.Param.WebUserName = "test";
            //PnrAnalysis.Model.BSPResponse response = sendAuto.BSPAutoIssue(bsp);

            AutoTicketService autoTicketService = new AutoTicketService();
            string strData = "<pnrinfo><code>1</code><pnr>MCLE2D</pnr><air>CA</air><pnrsrcid>05635108590040151070</pnrsrcid><message></message><tickets><ticket><passenger>%c2%de%cb%bc%bb%d5</passenger><tktno>999-2344273598</tktno></ticket></tickets></pnrinfo>";

            B2BResponse b2BResponse = autoTicketService.SyncTicketCall(strData);
            //DataSet ds = XmlToDataSet(strData);
        }
    }
}
