using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common
{
    /// <summary>
    /// 交易相关接口
    /// </summary>
    public interface IPaymentClientProxy
    {
        /// <summary>
        /// 支付退款查询
        /// </summary>
        /// <param name="code"></param>
        /// <param name="serialNum"></param>
        /// <param name="refundNo"></param>
        /// <returns></returns>
        dynamic RefundCheck(string code, string refundNo);
        /// <summary>
        /// 取余额
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        FundInfo GetRecieveAndCreditMoney(string code, string key);
        /// <summary>
        /// 退款 如果走银行卡或第三方，将异步接收退款信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="serialNum"></param>
        /// <param name="money"></param>
        /// <returns></returns>
        void Reimburse(string code, string key, string serialNum, decimal money, string orderId, string remark, string BusDesc = "",string notes="");

        /// <summary>
        /// 支付状态查询 返回交易流水号
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="orderId"></param>
        /// <param name="flag">查询来源标识</param>
        /// <returns></returns>
        Tuple<string, string,string> PayStateQuery(string code, string key, string orderId, string flag = null);

        /// <summary>
        /// 现金支付分润
        /// </summary>      
        /// <returns></returns>
        Tuple<bool, string, string> PaymentByCashAccount(string code, string key, string orderID, string productName, decimal money, string payPassword, string BusDesc = "", string notes = "");

        /// <summary>
        /// 信用支付分润
        /// </summary>       
        /// <returns></returns>
        Tuple<bool, string, string> PaymentByCreditAccount(string code, string key, string orderID, string productName, decimal money, string payPassword, string BusDesc = "", string notes = "");
        /// <summary>
        /// 银行卡支付分润
        /// </summary>      
        /// <returns></returns>
        string PaymentByBank(string code, string key, string orderID, string productName, decimal money, string Bank, string notifyUrl, string remark, string BusDesc="",string notes="");

        /// <summary>
        /// 第三方支付分润s
        /// </summary>       
        /// <returns></returns>
        string PaymentByPlatform(string code, string key, string orderID, string productName, decimal money, string Platform, string notifyUrl, string remark, string BusDesc = "", string notes = "");

        /// <summary>
        /// 支付宝快捷支付分润
        /// </summary>       
        /// <returns></returns>
        string PaymentByQuikAliPay(string code, string key, string orderID, string productName, decimal money, string Platform, string notifyUrl, string remark, string paypassword, string BusDesc = "", string notes = "");
        
    }
}
