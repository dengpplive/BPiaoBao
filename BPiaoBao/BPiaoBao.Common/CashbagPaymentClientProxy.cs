using System.Runtime.Remoting;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace BPiaoBao.Common
{
    public class CashbagPaymentClientProxy : IPaymentClientProxy
    {

        private string webURLfunds = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "funds/";


        public FundInfo GetRecieveAndCreditMoney(string code, string key)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "GetRecieveAndCreditMoney", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagPayException(result.message.ToString());
            var fundinfo = new FundInfo()
            {
                CreditMoney = result.result.creditMoney,
                RecieveMoney = result.result.recieveMoney,
                RecieveAmount = result.result.RecieveAmount
            };
            return fundinfo;
        }
        public void Reimburse(string code, string key, string serialNum, decimal money, string orderId, string remark, string BusDesc = "", string notes = "")
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "refund", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("money", money.ToString());
            dictionary.Add("serialNum", serialNum);
            dictionary.Add("refundNo", orderId);
            dictionary.Add("remark", remark);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            dictionary.Add("BusDesc", BusDesc);
            dictionary.Add("Notes", notes);
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
            {
                if (result.message == "退款已成功")
                    return;
                throw new CashBagPayException(result.message.ToString());
            }

        }


        public Tuple<string, string,string> PayStateQuery(string code, string key, string orderId, string flag = null)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "GetPayStatus", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("orderID", orderId);
            dictionary.Add("key", key);
            dictionary.Add("code", code);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagPayException(result.message.ToString());
            //if (result.result == "F")
            //    throw new CashBagPayException("未支付");
            //return result.result;
            var info = result.result;
            if (info.TradeNo == "F" && string.IsNullOrEmpty(flag)) throw new CashBagPayException("未支付");
            string t = info.TradeNo;
            string p = info.PayType;
            string b= info.BankCode;
            return Tuple.Create(t, p,b);
        }



        #region 支付分润

        public Tuple<bool, string, string> PaymentByCashAccount(string code, string key, string orderID, string productName, decimal money, string payPassword, string BusDesc, string notes = "")
        {
            //string rs = "";
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "pay", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("orderNo", orderID);
            dictionary.Add("ProductName", productName);
            dictionary.Add("money", money.ToString());
            dictionary.Add("pwd", payPassword);
            dictionary.Add("BusDesc", BusDesc);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            dictionary.Add("Notes", notes);
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagPayException(result.message.ToString());
            //return result.result;
            //是否已经在线支付
            bool isPay = (bool)result.result.IsPay;
            string tradeNo = result.result.TradeNo;
            string payType = result.result.PayType;
            return Tuple.Create(isPay, tradeNo, payType);
        }

        public Tuple<bool, string, string> PaymentByCreditAccount(string code, string key, string orderID, string productName, decimal money, string payPassword, string BusDesc, string notes = "")
        {
            //string rs = "";
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "Creditpay", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("orderNo", orderID);
            dictionary.Add("ProductName", productName);
            dictionary.Add("money", money.ToString());
            dictionary.Add("pwd", payPassword);
            dictionary.Add("BusDesc", BusDesc);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            dictionary.Add("Notes", notes);
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagPayException(result.message.ToString());
            //return result.result;
            //是否已经在线支付
            bool isPay = (bool)result.result.IsPay;
            string tradeNo = result.result.TradeNo;
            string payType = result.result.PayType;
            return Tuple.Create(isPay, tradeNo, payType);
        }

        public string PaymentByBank(string code, string key, string orderID, string productName, decimal money, string Bank, string notifyUrl, string remark, string BusDesc, string notes)
        {
            //string rs = "";
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "onlinepay", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("orderNo", orderID);
            dictionary.Add("ProductName", productName);
            dictionary.Add("money", money.ToString());
            dictionary.Add("BankName", Bank);
            dictionary.Add("Remark", remark);
            dictionary.Add("NotifyUrl", notifyUrl);
            dictionary.Add("BusDesc", BusDesc);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            dictionary.Add("Notes", notes);
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagPayException(result.message.ToString());
            //return result.result;
            var payvalue = result.result;
            var ispay = (bool)payvalue.PayStatus;
            return !ispay ? payvalue.value : payvalue.PayStatus.ToString();
        }

        public string PaymentByPlatform(string code, string key, string orderID, string productName, decimal money, string Platform, string notifyUrl, string remark, string BusDesc, string notes)
        {
            //string rs = "";
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "onlinepay", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("orderNo", orderID);
            dictionary.Add("ProductName", productName);
            dictionary.Add("money", money.ToString());
            dictionary.Add("NotifyUrl", notifyUrl);
            dictionary.Add("remark", remark);
            dictionary.Add("payType", Platform);
            dictionary.Add("BusDesc", BusDesc);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            dictionary.Add("Notes", notes);
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagPayException(result.message.ToString());
            //return result.result;
            var payvalue = result.result;
            var ispay = (bool)payvalue.PayStatus;
            return !ispay ? payvalue.value : payvalue.PayStatus.ToString();
        }

        public string PaymentByQuikAliPay(string code, string key, string orderID, string productName, decimal money, string Platform, string notifyUrl, string remark, string paypassword, string BusDesc, string notes)
        {
            //string rs = "";
            var ch = new CashbagHelper(webURLfunds + "AlipaySignPay", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"orderNo", orderID},
                {"ProductName", productName},
                {"money", money.ToString()},
                {"NotifyUrl", notifyUrl},
                {"remark", remark},
                {"pwd", paypassword},
                {"payType", Platform},
                {"BusDesc", BusDesc},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")},
                {"Notes", notes}
            };
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagPayException(result.message.ToString());
            //return result.result;
            var payvalue = result.result;
            var ispay = (bool)payvalue.PayStatus;
            return !ispay ? payvalue.value : payvalue.PayStatus.ToString();
        }
        #endregion

        /// <summary>
        /// 支付退款查询
        /// </summary>
        /// <param name="code"></param>
        /// <param name="refundNo"></param>
        /// <returns></returns>
        public dynamic RefundCheck(string code, string refundNo)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "RefundCheck", "GET");
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("code", code);
            dic.Add("refundNo", refundNo);
            dic.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dic);
            return ch.GetBackJsonData(data);
        }
    }
}
