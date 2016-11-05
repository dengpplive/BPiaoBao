using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPiaoBao.AppServices.Hosting;
using StructureMap;
using BPiaoBao.AppServices.DomesticTicket;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using BPiaoBao.AppServices;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System.Linq;
using JoveZhao.Framework.DDD;
using BPiaoBao.Common;

namespace BPiaoBao.UnitTest
{
    [TestClass]
    public class Coordination
    {
        [TestMethod]
        public void TestCoordination()
        {
            BootStrapper.ConfigureDependencies();
            //AuthManager.SaveUser(new CurrentUserInfo()
            //{
            //    Code = "aa",
            //    CashbagCode = "0000000326",
            //    OperatorAccount = "cc",
            //    OperatorName = "dd",
            //    CashbagKey = "33cd87fd33d2493eb99b71a8a91d2ccf"
            //});
            BPiaoBao.AppServices.DomesticTicket.OrderService orderservice = ObjectFactory.GetInstance<OrderService>();
            string OrderId = "01312091002468804592";
            string Content = "测试协调12121";
            //添加
           // orderservice.AddCoordinationDto(OrderId, Content);

            //查询
            CoordinationDto cd = orderservice.GetCoordinationDto(OrderId);

        }

        [TestMethod]
        public void TestSms()
        {
            BootStrapper.Boot();
            string s = "100100";
            var bus = ObjectFactory.GetInstance<IBusinessmanRepository>();
            var buz = bus.FindAll(p => p.Code == s).FirstOrDefault();
            buz.SendMessage("老赵", "你好", "15608039993", "测试短信");

            IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
            IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());

            unitOfWorkRepository.PersistUpdateOf(buz);
            unitOfWork.Commit();
        }
    }
}
