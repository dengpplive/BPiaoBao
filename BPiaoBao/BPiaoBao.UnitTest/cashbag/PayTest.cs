//using System;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using BPiaoBao.Common;

//namespace BPiaoBao.UnitTest.cashbag
//{
//    [TestClass]
//    public class PayTest
//    {
//        string code = "0000000326";
//        string key = "33cd87fd33d2493eb99b71a8a91d2ccf";
//        string CollaboratorKey = "1439e30938174d75a2360e4e3d3c6094";
//        IPaymentClientProxy proxy = new CashbagPaymentClientProxy();
//        [TestMethod]
//        public void PayStateQuery()
//        {
//            //proxy.PayStateQuery(code, key, "000001");
//            //proxy.GetRecieveAndCreditMoney(code, key);
//            proxy.PaymentByCashAccount(code, key,  "0", "机票订单", decimal.Parse("1000"), "123");
//        }
//    }
//}
