using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using BPiaoBao.Cashbag.Domain.Services;
using BPiaoBao.Cashbag.ClientProxy;

namespace BPiaoBao.UnitTest.cashbag
{
    [TestClass]
    public class AccountTest
    {
        string code = "0000005390";
        string key = "21c47f788c3641dd943c9397b73c653d";
        AccountClientProxy proxy = new AccountClientProxy();

        [TestMethod]
        public void GetValidateCode()
        {
            proxy.GetValidateCode(code, key);
        }

        [TestMethod]
        public void Bill()
        {
            proxy.GetBill(code, key, null, null, "0", 0, 5);
            proxy.GetBillDetail(code, key, null, null, "", "", "", 0, 5,"");
            proxy.GetRePayDetail(code, key, null, null, null, null, "", "", 0, 5,"");
        }
        [TestMethod]
        public void GrantApply()
        {
            //proxy.GrantApply(code, key, "http://116.254.206.23:901/images/b5507573-0992-4f67-b0a5-6d516dc67200.gif", "", "", "", "", "", "");
           // proxy.GetGrantInfo(code, key);
        }
        [TestMethod]
        public void Log()
        {
            proxy.GetRechargeLogs(code, key, null, null, 0, 10);
            proxy.GetFinancialLog(code, key, null, null, 0, 5);
            proxy.GetTransferAccountsLog(code, key, null, null, 0, 10);
            proxy.GetBargainLog(code, key, null, null, 0, 10);
            proxy.GetScoreConvertLog(code, key, null, null, 0, 10);
        }
        [TestMethod]
        public void AccountDetails()
        {
            proxy.GetReadyAccountDetails(code, key, DateTime.Now, DateTime.Now.AddMonths(-1),null,null, 0, 4);
            proxy.GetCreditAccountDetails(code, key, null, null, 0, 10);
            proxy.GetScoreAccountDetails(code, key, null, null, 0, 5);
        }
        [TestMethod]
        public void DeleteBank()
        {
            proxy.RemoveBank(code, key, "1568");
        }
        [TestMethod]
        public void GetBank()
        {
            proxy.GetBank(code, key);
        }
        [TestMethod]
        public void AddBank()
        {
            proxy.AddBank(code, key, new Cashbag.Domain.Models.BankCard()
            {
                BankBranch = "fd22fd",
                CardNo = "11",
                City = "成都",
                Name = "fsfs",
                Owner = "刘江涛",
                Province = "fdfd",
                IsDefault = false
            });
        }
        [TestMethod]
        public void SetDefaultBank()
        {
            proxy.SetDefaultBank(code, key, "313");
        }

        [TestMethod]
        public void GetAccountInfo()
        {
            proxy.GetCompanyInfo(code, key);
            proxy.GetAccountInfo(code, key);
        }

        [TestMethod]
        public void GetRepayInfo()
        {
            proxy.GetRepayInfo(code, key);
        }
    }
}
