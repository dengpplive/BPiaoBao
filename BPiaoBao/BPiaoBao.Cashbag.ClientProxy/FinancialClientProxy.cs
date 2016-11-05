using BPiaoBao.Cashbag.Domain.Models;
using BPiaoBao.Cashbag.Domain.Services;
using BPiaoBao.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.ClientProxy
{
    public class FinancialClientProxy : IFinancialClientProxy
    {

        private string webURLfunds = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "funds/";
        private string webURLProduct = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "Product/";

        public IEnumerable<FinancialProduct> GetAllProduct(string code,string key)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLProduct + "GetAllProducts", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);

            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            List<FinancialProduct> lists = new List<FinancialProduct>();
            var rows = JArray.FromObject(result.result);
            foreach (var item in rows)
            {
                FinancialProduct fp = new FinancialProduct()
                {
                    ProductId = item.FinancingProductID,
                    Name = item.ProductName,
                    Description = item.Content,
                    IconUrl = item.ImageUrl,
                    Abstract = "",
                    ReturnRate = item.ReturnRate,
                    Day = item.Day,
                    LimitAmount = item.LimitAmount ?? 0,
                    MaxAmount = item.MaxAmount,
                    CurrentAmount = item.CurrentAmount,
                    EndDate = item.EndDate,
                    ValidDate = item.ValidDate,
                    InterestRate = item.InterestRate,
                    CreateDate = item.CreateDate,
                    CanSettleInAdvance = item.CanSettleInAdvance
                };
                lists.Add(fp);
            }
            return lists;
        }


        public void BuyFinancialProductByCashAccount(string code, string key, string productID, decimal money, string pwd)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "BuyProductByCash", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("Pwd", pwd);
            dictionary.Add("productID", productID);
            dictionary.Add("Money", money.ToString());
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }

        public string BuyFinancialProductByBank(string code, string key, string productID, decimal money, string bankName)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "BuyProductByInternetBank", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("productID", productID);
            dictionary.Add("Money", money.ToString());
            dictionary.Add("BankName", bankName);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }

        public string BuyFinancialProductByPlatform(string code, string key, string productID, decimal money, string payPlatform)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "BuyProductByInternetBank", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("productID", productID);
            dictionary.Add("Money", money.ToString());
            dictionary.Add("payType", payPlatform);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }

        public void AbortFinancial(string code, string key, string tradeID, string pwd)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLProduct + "EarlyTermination", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("tradeID", tradeID);
            dictionary.Add("pwd", pwd);

            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }


        public ExpectProfit GetExpectProfit(string code, string key, string tradeID)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLProduct + "GetExpectProfit", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("tradeID", tradeID);

            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            var ep = new ExpectProfit()
            {
                Profit = result.result.profit == null ? "0" : result.result.profit,
                NormalProfit = result.result.normalProfit == null ? "0" : result.result.normalProfit
            };
            return ep;
        }


        public FinancialProduct GetSingleProductInfo(string code, string key, string productID)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLProduct + "GetSingleProductInfo", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("productID", productID);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            result = result.result;
            var fp = new FinancialProduct()
            {
                ProductId = result.FinancingProductID,
                Name = result.ProductName,
                Description = result.Content,
                IconUrl = result.ImageUrl,
                ReturnRate = result.ReturnRate,
                Day = result.Day,
                LimitAmount = result.LimitAmount,
                EndDate = result.EndDate,
                ValidDate = result.ValidDate,
                InterestRate = result.InterestRate,
                CreateDate = result.CreateDate,
                CanSettleInAdvance = result.CanSettleInAdvance
            };
            return fp;
        }

        public IEnumerable<FinancialProduct> GetShelfProducts(string code, string key, string quantity)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLProduct + "GetTheTopFiveOffTheShelfProducts", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("quantity",quantity);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);

            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            List<FinancialProduct> lists = new List<FinancialProduct>();
            var rows = JArray.FromObject(result.result);
            foreach (var item in rows)
            {
                FinancialProduct fp = new FinancialProduct()
                {
                    ProductId = item.FinancingProductID,
                    Name = item.ProductName,
                    Description = item.Content,
                    IconUrl = item.ImageUrl,
                    Abstract = "",
                    ReturnRate = item.ReturnRate,
                    Day = item.Day,
                    LimitAmount = item.LimitAmount ?? 0,
                    MaxAmount = item.MaxAmount,
                    CurrentAmount = item.CurrentAmount,
                    EndDate = item.EndDate,
                    ValidDate = item.ValidDate,
                    InterestRate = item.InterestRate,
                    CreateDate = item.CreateDate,
                    CanSettleInAdvance = item.CanSettleInAdvance
                };
                lists.Add(fp);
            }
            return lists;
        }

        public IEnumerable<FinancialProduct> GetActiveProduct(string code, string key, string quantity)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLProduct + "GetActiveProduct", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("quantity", quantity);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);

            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            List<FinancialProduct> lists = new List<FinancialProduct>();
            var rows = JArray.FromObject(result.result);
            foreach (var item in rows)
            {
                FinancialProduct fp = new FinancialProduct()
                {
                    ProductId = item.FinancingProductID,
                    Name = item.ProductName,
                    Description = item.Content,
                    IconUrl = item.ImageUrl,
                    Abstract = "",
                    ReturnRate = item.ReturnRate,
                    Day = item.Day,
                    LimitAmount = item.LimitAmount ?? 0,
                    MaxAmount = item.MaxAmount,
                    CurrentAmount = item.CurrentAmount,
                    EndDate = item.EndDate,
                    ValidDate = item.ValidDate,
                    InterestRate = item.InterestRate,
                    CreateDate = item.CreateDate,
                    CanSettleInAdvance = item.CanSettleInAdvance
                };
                lists.Add(fp);
            }
            return lists;
        }
    }
}
