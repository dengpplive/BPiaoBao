using BPiaoBao.AppServices.DataContracts.Cashbag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.Cashbag
{
    [ServiceContract]
    public interface IFinancialService
    {
        /// <summary>
        /// 取理财产品列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<FinancialProductDto> GetAllProduct();
        /// <summary>
        /// 购买理财产品(现金账户)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="FinancialID"></param>
        /// <param name="money"></param>
        [OperationContract]
        void BuyFinancialProductByCashAccount(string productID, decimal money, string pwd);
        /// <summary>
        /// 购买理财产品(银行卡)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="FinancialID"></param>
        /// <param name="money"></param>
        [OperationContract]
        string BuyFinancialProductByBank(string productID, decimal money, string bankName);
        /// <summary>
        /// 购买理财产品(第三方)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="FinancialID"></param>
        /// <param name="money"></param>
        [OperationContract]
        string BuyFinancialProductByPlatform(string productID, decimal money, string payPlatform);

        /// <summary>
        /// 中止某个理财产品
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="tradeID"></param>
        /// <param name="pwd"></param>
        [OperationContract]
        void AbortFinancial(string tradeID, string pwd);
        /// <summary>
        /// 查看收益
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="tradeID"></param>
        /// <returns></returns>
        [OperationContract]
        ExpectProfitDto GetExpectProfit(string tradeID);
        /// <summary>
        /// 获取理财产品详情
        /// </summary>
        /// <param name="productID"></param>
        /// <returns></returns>
        [OperationContract]
        FinancialProductDto GetSingleProductInfo(string productID);
        /// <summary>
        /// 获取下架产品信息
        /// </summary>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<FinancialProductDto> GetShelfProducts(string quantity);
        /// <summary>
        /// 获取可购买产品信息
        /// </summary>
        /// <param name="quantity">数量</param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<FinancialProductDto> GetActiveProduct(string quantity);
    }
}
