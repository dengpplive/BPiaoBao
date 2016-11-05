//using BPiaoBao.AppServices;
//using BPiaoBao.AppServices.Contracts.SystemSetting;
//using BPiaoBao.AppServices.DataContracts.DomesticTicket;
//using BPiaoBao.AppServices.DomesticTicket;
//using BPiaoBao.AppServices.SystemSetting;
//using BPiaoBao.DomesticTicket.Domain.Models.Orders;
//using BPiaoBao.DomesticTicket.Domain.Services;
//using BPiaoBao.DomesticTicket.Platform.Plugin;
//using BPiaoBao.SystemSetting.Domain.Services.Auth;
//using JoveZhao.Framework.Expand;
//using Microsoft.VisualStudio.TestTools.UnitTesting;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;

//namespace BPiaoBao.UnitTest
//{
//    [TestClass]
//    public class payTest
//    {
//        [TestMethod]
//        public void testPayByCashAccount()
//        {
//            BPiaoBao.Common.CashbagHelper bcc = new Common.CashbagHelper();
//            List<string> list = new List<string>();
//            list.Add("0000000326");
//            list.Add("33cd87fd33d2493eb99b71a8a91d2ccf");
//            list.Add("0");//orderID
//            list.Add("1000");//money
//            list.Add("1439e30938174d75a2360e4e3d3c6094");//CollaboratorKey
//            list.Add("123");//pwd
//            list.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime
//            string signature = bcc.GetSignaTure(list);
//            string url = @"http://116.254.206.23:889/api/funds/pay";
//            string data = "code=0000000326&orderID=0&money=1000&CollaboratorKey=1439e30938174d75a2360e4e3d3c6094&pwd=123&currentTime=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&signature=" + signature + "";
//            //var ss = bcc.GetBackData(url, data, "GET");

//            //JObject JObject = JObject.Parse(ss);
//        }
//        [TestMethod]
//        public void testPayByCreditAccount()
//        {
//            BPiaoBao.Common.CashbagHelper bcc = new Common.CashbagHelper();
//            List<string> list = new List<string>();
//            list.Add("0000000326");
//            list.Add("33cd87fd33d2493eb99b71a8a91d2ccf");
//            list.Add("0");//orderID
//            list.Add("1000");//money
//            list.Add("1439e30938174d75a2360e4e3d3c6094");//CollaboratorKey
//            list.Add("123");//pwd
//            list.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime
//            string signature = bcc.GetSignaTure(list);
//            bcc.GetSignaTure("0000000326", "1439e30938174d75a2360e4e3d3c6094", "", "", "", "");
//            string url = @"http://116.254.206.23:889/api/funds/Creditpay";
//            string data = "code=0000000326&orderID=0&money=1000&CollaboratorKey=1439e30938174d75a2360e4e3d3c6094&pwd=123&currentTime=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&signature=" + signature + "";
//            //var ss = bcc.GetBackData(url, data, "GET");

//            //JObject JObject = JObject.Parse(ss);
//        }
//        [TestMethod]
//        public void testPayByBank()
//        {
//            BPiaoBao.Common.CashbagHelper bcc = new Common.CashbagHelper();
//            List<string> list = new List<string>();
//            list.Add("0000000326");
//            list.Add("33cd87fd33d2493eb99b71a8a91d2ccf");
//            list.Add("0");//orderID
//            list.Add("1000");//money
//            list.Add("1439e30938174d75a2360e4e3d3c6094");//CollaboratorKey
//            list.Add("ABC");//
//            list.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime
//            string signature = bcc.GetSignaTure(list);
//            bcc.GetSignaTure("0000000326", "1439e30938174d75a2360e4e3d3c6094", "", "", "", "");
//            string url = @"http://116.254.206.23:889/api/funds/Creditpay";
//            string data = "code=0000000326&orderID=0&money=1000&CollaboratorKey=1439e30938174d75a2360e4e3d3c6094&BankName=ABC&currentTime=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&signature=" + signature + "";
//            //var ss = bcc.GetBackData(url, data, "GET");

//            //JObject JObject = JObject.Parse(ss);
//        }
       
//        [TestMethod]
//        public void RechargeByBank()
//        {
//            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(@"http://116.254.206.23:889/api/funds/Recharge", "GET");
//            Dictionary<string, string> list = new Dictionary<string, string>();
//            list.Add("code", "0000000326");
//            list.Add("key", "33cd87fd33d2493eb99b71a8a91d2ccf");
//            list.Add("money", "1000");
//            list.Add("payType", "BillPay");
//            list.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime

//            string data = ch.ParamsURLEncode(list);
//            var ss = ch.GetBackData(data);
//            JToken jToken;
//            var josns = JObject.Parse(ss);
//            josns.TryGetValue("status", out jToken);
//            if (string.Compare(jToken.ToString(), "true", true) == 0)
//            {

//            }
//        }
//        [TestMethod]
//        public void CashOut()
//        {
//            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(@"http://116.254.206.23:889/api/Withdraw/Application", "GET");
//            Dictionary<string, string> list = new Dictionary<string, string>();
//            list.Add("code", "0000000326");
//            list.Add("key", "33cd87fd33d2493eb99b71a8a91d2ccf");

//            list.Add("AccountID", "313");
//            list.Add("Amount", "1");
//            list.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime



//            string data = ch.ParamsURLEncode(list);
//            var ss = ch.GetBackData(data);
//            JToken jToken;
//            var josns = JObject.Parse(ss);
//            josns.TryGetValue("status", out jToken);
//            if (string.Compare(jToken.ToString(), "true", true) == 0)
//            {

//            }
//        }
//        [TestMethod]
//        public void BuyFinancialProduct()
//        {
//            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(@"http://116.254.206.23:889/api/Product/EarlyTermination", "GET");
//            Dictionary<string, string> list = new Dictionary<string, string>();
//            list.Add("code", "0000000326");
//            list.Add("key", "33cd87fd33d2493eb99b71a8a91d2ccf");

//            list.Add("bfpdid", "2");
//            list.Add("pwd", "123");
//            list.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime

//            string data = ch.ParamsURLEncode(list);
//            var ss = ch.GetBackData(data);
//            JToken jToken;
//            var josns = JObject.Parse(ss);
//            josns.TryGetValue("status", out jToken);
//            if (string.Compare(jToken.ToString(), "true", true) == 0)
//            {

//            }

//        }

//        [TestMethod]
//        public void GetRechargeLogs()
//        {
//            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(@"http://192.168.1.16:8081/api/History/Recharge", "GET");
//            Dictionary<string, string> list = new Dictionary<string, string>();
//            list.Add("code", "0000000326");
//            list.Add("key", "33cd87fd33d2493eb99b71a8a91d2ccf");
//            list.Add("CurrentPage", "1");
//            list.Add("PageSize", "20");
//            list.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime

//            string data = ch.ParamsURLEncode(list);
//            var ss = ch.GetBackData(data);
//            JToken jToken;
//            var josns = JObject.Parse(ss);
//            List<RechargeLogT> lists = new List<RechargeLogT>();
//            int num = 0;
//            josns.TryGetValue("status", out jToken);
//            if (string.Compare(jToken.ToString(), "true", true) == 0)
//            {
//                JToken jTokenResult;
//                josns.TryGetValue("result", out jTokenResult);
//                dynamic d = JsonConvert.DeserializeObject<dynamic>(jTokenResult.ToString());
//                var rows = JArray.FromObject(d.rows);
//                num = d.total;
//                foreach (var item in rows)
//                {
//                    RechargeLogT rl = new RechargeLogT()
//                    {
//                        SerialNum = item.SerialNum,
//                        RechargeTime = item.RechargeTime,
//                        TypeName = item.TypeName,
//                        InComeOrExpenses = item.InComeOrExpenses,
//                        RechargeMoney = item.RechargeMoney,
//                        CashSource = item.CashSource,
//                        RechargeStatus = item.RechargeStatus
//                    };
//                    lists.Add(rl);
//                }

//            }
//            Tuple<IEnumerable<RechargeLogT>, int> tuple = new Tuple<IEnumerable<RechargeLogT>, int>(lists, num);
//        }

//        [TestMethod]
//        public void GetAccountInfo()
//        {
//            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(@"http://116.254.206.23:889/api/Product/GetBuyingProducts", "GET");
//            Dictionary<string, string> list = new Dictionary<string, string>();
//            list.Add("code", "0000000326");
//            list.Add("key", "33cd87fd33d2493eb99b71a8a91d2ccf");
//            list.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime

//            string data = ch.ParamsURLEncode(list);
//            var ss = ch.GetBackData(data);
//            JToken jToken;
//            var josns = JObject.Parse(ss);

//            josns.TryGetValue("status", out jToken);
//            if (string.Compare(jToken.ToString(), "true", true) == 0)
//            {
//                JToken jTokenResult;
//                josns.TryGetValue("result", out jTokenResult);
//                dynamic d = JsonConvert.DeserializeObject<dynamic>(jTokenResult.ToString());
//                var Banks = JArray.FromObject(d.Banks);
//                var FinancialProducts = JArray.FromObject(d.FinancialProducts);
//                List<Bank> listBank = new List<Bank>();
//                List<FinancialLog> listFinancial = new List<FinancialLog>();

//                foreach (var item in Banks)
//                {
//                    Bank bank = new Bank()
//                    {
//                        BankId = item.BankId,
//                        Name = item.Name,
//                        BankBranch = item.BankBranch,
//                        CardNo = item.CardNo,
//                        Owner = item.Owner,
//                        Status = item.Status,
//                        IsDefault = item.IsDefault
//                    };
//                    listBank.Add(bank);
//                }
//                foreach (var item in FinancialProducts)
//                {
//                    FinancialLog bank = new FinancialLog()
//                    {
//                        SerialNum = item.SerialNum,
//                        BuyTime = item.BuyTime,

//                        ProductName = item.ProductName,
//                        FinancialMoney = item.FinancialMoney,
//                        CashSource = item.CashSource,
//                        FinancialLogStatus = item.FinancialLogStatus
//                    };
//                    listFinancial.Add(bank);
//                }
//                AccountInfo accountInfo = new AccountInfo()
//                {
//                    ReadyBalance = d.ReadyBalance,
//                    CreditBalance = d.CreditBalance,
//                    CreditQuota = d.CreditQuota,
//                    FinancialScore = d.FinancialScore,
//                    Banks = listBank,
//                    FinancialProducts = listFinancial
//                };


//            }
//        }

//        [TestMethod]
//        public void er1()
//        {
//            BPiaoBao.Common.CashbagHelper bcc = new Common.CashbagHelper();
//            List<string> list = new List<string>();
//            list.Add("0000000326");//code
//            list.Add("33cd87fd33d2493eb99b71a8a91d2ccf");//key
//            list.Add("1000");//money
//            list.Add("CBA");//BankName

//            list.Add(DateTime.Now.ToString("yyyyMMddHHmmss"));//currentTime
//            string signature = bcc.GetSignaTure(list);
//            string url = @"http://116.254.206.23:889/api/funds/Recharge";
//            string data = "code=0000000326&money=1000&BankName=CBA&currentTime=" + DateTime.Now.ToString("yyyyMMddHHmmss") + "&signature=" + signature + "";
//            //var ss = bcc.GetBackData(url, data, "GET");

//            //JObject JObject = JObject.Parse(ss);
//        }
//    }
//    public class RechargeLogT
//    {
//        /// <summary>
//        /// 流水号
//        /// </summary>
//        public string SerialNum { get; set; }
//        /// <summary>
//        /// 充值时间
//        /// </summary>
//        public DateTime RechargeTime { get; set; }
//        /// <summary>
//        /// 充值类型
//        /// </summary>
//        public string TypeName { get; set; }
//        /// <summary>
//        /// 收支
//        /// </summary>
//        public string InComeOrExpenses { get; set; }
//        /// <summary>
//        /// 充值金额
//        /// </summary>
//        public decimal RechargeMoney { get; set; }
//        /// <summary>
//        /// 资金渠道
//        /// </summary>
//        public string CashSource { get; set; }
//        /// <summary>
//        /// 充值状态
//        /// </summary>
//        public string RechargeStatus { get; set; }
//    }
//    public class AccountInfo
//    {
       
//        private string code;
//        private string key;

//        public AccountInfo()
//        {
         
//        }
//        /// <summary>
//        /// 现金账户余额
//        /// </summary>
//        public decimal ReadyBalance { get; set; }
//        /// <summary>
//        /// 信用账户可用余额
//        /// </summary>
//        public decimal CreditBalance { get; set; }
//        /// <summary>
//        /// 信用账户额度
//        /// </summary>
//        public decimal CreditQuota { get; set; }
//        /// <summary>
//        /// 理财账户积分
//        /// </summary>
//        public decimal FinancialScore { get; set; }
//        /// <summary>
//        /// 理财金额
//        /// </summary>
//        public decimal FinancialMoney
//        {
//            get
//            {
//                return this.FinancialProducts.Sum(p => p.FinancialMoney);
//            }
//        }
//        /// <summary>
//        /// 默认银行卡
//        /// </summary>
//        public Bank DefaultBank
//        {
//            get
//            {
//                return this.Banks.FirstOrDefault(p => p.IsDefault);
//            }
//        }
//        /// <summary>
//        /// 我的银行卡
//        /// </summary>
//        public IEnumerable<Bank> Banks { get; set; }
//        public IEnumerable<FinancialLog> FinancialProducts { get; set; }



//    }

//    public class Bank
//    {
//        /// <summary>
//        /// 编号
//        /// </summary>
//        public string BankId { get; set; }
//        /// <summary>
//        /// 银行名称
//        /// </summary>
//        public string Name { get; set; }
//        /// <summary>
//        /// 开户行
//        /// </summary>
//        public string BankBranch { get; set; }
//        /// <summary>
//        /// 银行卡号
//        /// </summary>
//        public string CardNo { get; set; }
//        /// <summary>
//        /// 开户人
//        /// </summary>
//        public string Owner { get; set; }
//        /// <summary>
//        /// 卡号状态
//        /// </summary>
//        public string Status { get; set; }
//        /// <summary>
//        /// 是否默认
//        /// </summary>
//        public bool IsDefault { get; set; }
//    }
//    public class FinancialLog
//    {
//        /// <summary>
//        /// 流水号
//        /// </summary>
//        public string SerialNum { get; set; }
//        /// <summary>
//        /// 购买时间
//        /// </summary>
//        public DateTime BuyTime { get; set; }
//        /// <summary>
//        /// 产品名
//        /// </summary>
//        public string ProductName { get; set; }

//        /// <summary>
//        /// 金额
//        /// </summary>
//        public decimal FinancialMoney { get; set; }
//        /// <summary>
//        /// 资金渠道
//        /// </summary>
//        public string CashSource { get; set; }
//        /// <summary>
//        /// 理财状态
//        /// </summary>
//        public string FinancialLogStatus { get; set; }
//    }


//    public class SQLTest<T> where T:class
//    {
//            public void insert(){
            
//            }
//    }

//    public class SQLChild : SQLTest<Bank>
//    {
//        public void test() {
//            insert();
//        }
//    }
//}
