using BPiaoBao.Cashbag.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Services
{
    public interface IFinancialClientProxy
    {
        /// <summary>
        /// 取理财产品列表
        /// </summary>
        /// <returns></returns>
        IEnumerable<FinancialProduct> GetAllProduct(string code,string key);


        /// <summary>
        /// 购买理财产品(现金账户)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="FinancialID"></param>
        /// <param name="money"></param>
        void BuyFinancialProductByCashAccount(string code, string key, string productID, decimal money, string pwd);
        /// <summary>
        /// 购买理财产品(银行卡)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="FinancialID"></param>
        /// <param name="money"></param>
        string BuyFinancialProductByBank(string code, string key, string productID, decimal money, string bankName);
        /// <summary>
        /// 购买理财产品(第三方)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="FinancialID"></param>
        /// <param name="money"></param>
        string BuyFinancialProductByPlatform(string code, string key, string productID, decimal money, string payPlatform);

        /// <summary>
        /// 中止某个理财产品
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="tradeID"></param>
        /// <param name="pwd"></param>
        void AbortFinancial(string code, string key, string tradeID, string pwd);
        /// <summary>
        /// 查看收益
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="tradeID"></param>
        /// <returns></returns>
        ExpectProfit GetExpectProfit(string code, string key, string tradeID);
        /// <summary>
        /// 获取理财产品详情
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="productID"></param>
        /// <returns></returns>
        FinancialProduct GetSingleProductInfo(string code, string key, string productID);
        /// <summary>
        /// 获取下架产品信息
        /// </summary>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        IEnumerable<FinancialProduct> GetShelfProducts(string code, string key, string quantity);
       /// <summary>
        /// 获取可购买产品信息
       /// </summary>
       /// <param name="code"></param>
       /// <param name="key"></param>
       /// <param name="quantity">数量</param>
       /// <returns></returns>
        IEnumerable<FinancialProduct> GetActiveProduct(string code, string key, string quantity);
    }
}
