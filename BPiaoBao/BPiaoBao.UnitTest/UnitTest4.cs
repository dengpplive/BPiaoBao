using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.AppServices.Hosting;
using BPiaoBao.Client.UIExt.Helper;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Services.Insurance;
using BPiaoBao.DomesticTicket.Platform.Plugin;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using StructureMap;

namespace BPiaoBao.UnitTest
{
    /// <summary>
    /// UnitTest4 的摘要说明
    /// </summary>
    [TestClass]
    public class UnitTest4
    {
         [TestMethod]
        public void TestMethodInsuranceConfig()
        {
             BootStrapper.Boot();
             var service = ObjectFactory.GetInstance<InsuranceService>();
             service.SaveInsuranceConfig(new InsuranceConfigData()
             {
                 IsOpen=true,
                 LeaveCount=1200,
                 SinglePrice=2.00M
             });

        }



         [TestMethod]
        public void TestMethodInsuranceDepoistLog()
        {
            BootStrapper.Boot();
            var service = ObjectFactory.GetInstance<InsuranceService>();
             //service.SaveInsuranceDespositLog(new RequestInsuranceDepositLog()
             //{
             //    AfterLeaveCount=1200,
             //    BeforeLeaveCount = 200,
             //    BuyTime=DateTime.Now,
             //    DepositCount = 1000,
             //    SinglePrice=2.00M,
             //    TotalPrice=2000.00M
             //});
        }



        [TestMethod]
        public void TestMethodInsurance()
        {
            BootStrapper.Boot();
            //var oser = ObjectFactory.GetInstance<OrderService>();
            //var model = oser.GetOrderByOrderId("04630474482726633176");
            //var str = JsonConvert.SerializeObject(model);
            //Console.WriteLine(str);
            var service = ObjectFactory.GetInstance<InsuranceService>();
            var dto = new RequestInsurance();
            dto.OrderId = "05049269408530754042";//1个乘客
            //dto.OrderId = "05283589118296006288";//4个乘客
            //dto.SinglePrice = 20.00M;
            //dto.IsBuyRefundInsurance = true;
            //dto.Passenger = new List<PassengerDto>() { new PassengerDto() { CardNo = "513722198911054516" } }; 
           // dto.Passenger = new List<PassengerDto>() { new PassengerDto() { CardNo = "520201197209083216" }, new PassengerDto() { CardNo = "123654789" }, new PassengerDto() { CardNo = "G1452362" }, new PassengerDto() { CardNo = "513722198911054516" } }; 
            service.SaveInsurance(dto);
            var req=new RequestQueryInsurance(); 
            var m1 = service.QueryInsurance(null, 1, 20);
            var str1 = JsonConvert.SerializeObject(m1);
            Console.WriteLine(str1);

        }


        [TestMethod]
        public void TestMethodCtrlInsuranceConfig()
        {
            BootStrapper.Boot();
            var service = ObjectFactory.GetInstance<InsuranceService>();
            var m1 = service.GetCtrlInsurance();
            var str1 = JsonConvert.SerializeObject(m1);
            Console.WriteLine(str1);
            ///////////////////////
            //var list = new List<CtrlInsuranceConfig>();
            //list.Add(new CtrlInsuranceConfig(){Value = "新一站",IsCurrent=false,LeaveCount=100000,SinglePrice = 5.00M,Url="http://www.baidu.com"});
            //list.Add(new CtrlInsuranceConfig() { Value = "人寿保险", IsCurrent = true, LeaveCount = 300000, SinglePrice = 5.00M, Url = "http://www.sina.com.cn" });
            //service.SaveCtrlConfig(new CtrlInsuranceDto() { CtrlInsurance=list,IsEnabled = false});
            //Console.WriteLine("///////////////////////////////////////////");
            //var m2 = service.GetCtrlInsurance();
            //var str2 = JsonConvert.SerializeObject(m2);
            //Console.WriteLine(str2);
            //////////////////////////////////////////////////
            //Console.WriteLine("***************************************************");
            //var m3 = service.GetCtrlInsuranceInter();
            //var str3 = JsonConvert.SerializeObject(m3);
            //Console.WriteLine(str3);
            //var list1 = new List<CtrlInsuranceBase>();
            //list1.Add(new CtrlInsuranceBase(){ IsCurrent = false,Value = "新一站",Url="http://www.52324.com"});
            //list1.Add(new CtrlInsuranceBase() { IsCurrent = true, Value = "人寿保险", Url = "http://www.reren.com" });
            //service.SaveCtrlInterConfig(new CtrlInsuranceInterDto(){CtrlInsuranceInter=list1,IsEnabled = false});
            //Console.WriteLine("///////////////////////////////////////////");
            //var m4= service.GetCtrlInsuranceInter();
            //var str4 = JsonConvert.SerializeObject(m4);
            //Console.WriteLine(str4);
            //////////////////////////////////////////////////
            //Console.WriteLine("***************************************************");
            //var m5 = service.GetCtrlInsuranceRefund();
            //var str5 = JsonConvert.SerializeObject(m5);
            //Console.WriteLine(str5);
            //service.SaveCtrlRefundConfig(new CtrlInsuranceRefundDto(){IsEnabled=false,SinglePrice = 4.50M});
            //Console.WriteLine("///////////////////////////////////////////");
            //var m6 = service.GetCtrlInsuranceRefund();
            //var str6 = JsonConvert.SerializeObject(m6);
            //Console.WriteLine(str6);
        }

        [TestMethod]
        public void TestMethod()
        {
            var dt = ExcelHelper.RenderToTableByNOPI(@"D:\97.xls");

            var str = JsonConvert.SerializeObject(dt);

            Console.WriteLine(str);

            dt = ExcelHelper.RenderToTableByNOPI(@"D:\100.xlsx");

            str = JsonConvert.SerializeObject(dt);

            Console.WriteLine(str);

        }


        [TestMethod]
        public void TestMethod1()
        {
            BootStrapper.Boot();
            var service = ObjectFactory.GetInstance<FrePasserService>();
            //service.SaveFrePasser(new FrePasserDto()
            //{
            //     AirCardNo="1000001",
            //     CertificateNo="511027198485858583",
            //     CertificateType="身份证",
            //     Mobile="138892222223",
            //     Name = "老段",
            //     PasserType="成人",
            //     Remark="121", 
            //});

            //service.SaveFrePasser(new FrePasserDto()
            //{
            //    AirCardNo = "1000002",
            //    CertificateNo = "511027198485858583",
            //    CertificateType = "身份证",
            //    Mobile = "138892222223",
            //    Name = "老段1",
            //    PasserType = "成人",
            //    Remark = "2333",
            //});
            //service.UpdateFrePasser(new FrePasserDto()
            //{
            //    Id = 1,
            //    AirCardNo = "1000001",
            //    CertificateNo = "1111111111111111111",
            //    CertificateType = "身份证",
            //    Mobile = "1350000000",
            //    Name = "老段1",
            //    PasserType = "成人",
            //    Remark = "11111111111",
            //});
            var list = new List<FrePasserDto>();
            for (int i = 0; i < 2000; i++)
            {
                var m = new FrePasserDto()
                {
                    AirCardNo = (1000003 + i).ToString(),
                    CertificateNo = "1111111111111111111" + i,
                    CertificateType = "身份证",
                    Mobile = "1350000000" + i,
                    Name = "老段1",
                    PasserType = "成人",
                    Remark = "444---ada---" + i,
                };
                list.Add(m);
            }

            service.Import(list);

            //service.DeleteFrePasser(23);

            //var result=  service.Export();

            //var result = service.QueryFrePassers(null,1, 5);

            //var str = JsonConvert.SerializeObject(result);

            //Console.WriteLine(str);

        }
    }
}
