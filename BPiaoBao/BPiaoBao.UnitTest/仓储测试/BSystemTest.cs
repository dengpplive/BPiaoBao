using BPiaoBao.AppServices.Hosting;
using BPiaoBao.Common;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Models.SMS;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.Expand;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.UnitTest.仓储测试
{
     [TestClass]
    public class BSystemTest
    {
         [TestMethod]
         public void TestMethod1()
         {
             BootStrapper.ConfigureDependencies();
             IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
             IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());

             Buyer bm = new Buyer();
             bm.CreateTime = DateTime.Now;
             bm.Name = "测试商户1";
             bm.Code = "100001";
             bm.ContactWay = new ContactWay();
             bm.ContactWay.Contact = "测试员A";
             bm.ContactWay.Address = "成都";
             bm.ContactWay.Tel = "0281238901";
             bm.CreateTime = DateTime.Now;
             bm.CashbagCode = "0000000326";
             bm.CashbagKey = "33cd87fd33d2493eb99b71a8a91d2ccf";
             bm.Operators = new List<Operator>();
             bm.Operators.Add(new Operator { Account = "test1", Password = "123321".Md5(), OperatorState = Common.Enums.EnumOperatorState.Normal, Realname = "测试员A", Phone = "13509098743" });
             bm.Attachments = new List<Attachment>();
             bm.Attachments.Add(new Attachment { Name = "附件1", Url = "www.ss.com" });

             bm.SMS = new SMS();
             bm.SMS.RemainCount = 20;
             bm.SMS.SendCount = 0;


             unitOfWorkRepository.PersistCreationOf(bm);

             //var businessman = businessmanRepository.FindAll(b => b.Code == code).SingleOrDefault();
             //businessman.Name = "测试商户222";
             //unitOfWorkRepository.PersistUpdateOf(businessman);
             //unitOfWork.Commit();

             //var businessman = businessmanRepository.FindAll(b => b.Code == "100002").SingleOrDefault();
             //unitOfWorkRepository.PersistDeletionOf(businessman);
             unitOfWork.Commit();
         }
    }
}
