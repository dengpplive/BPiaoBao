using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using BPiaoBao.Cashbag.Domain.Services;
using BPiaoBao.Common;
using JoveZhao.Framework.Expand;
using Newtonsoft.Json.Linq;
using BPiaoBao.Cashbag.Domain.Models;

namespace BPiaoBao.Cashbag.ClientProxy
{
    public class FundClientProxy : IFundClientProxy
    {
        private string webURLfunds = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "funds/";
        private string webURLWithdraw = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "Withdraw/";
        private string webURLAccount = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "Account/";


        public string RechargeByBank(string code, string key, decimal money, string payBank)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "Recharge", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("money", money.ToString());
            dictionary.Add("bankName", payBank);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            return result.result;

        }

        public string RechargeByPlatform(string code, string key, decimal money, string payPlatform)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "Recharge", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("money", money.ToString());
            dictionary.Add("payType", payPlatform);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            return result.result;
        }


        public void RepayMoneyByCashAccount(string code, string key, string money, string pwd)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "Repay", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("money", money.ToString());
            dictionary.Add("pwd", pwd);
            dictionary.Add("RepayType", "1");
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }

        public string RepayMoneyByBank(string code, string key, string money, string Bank)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "Repay", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("money", money.ToString());
            dictionary.Add("bankName", Bank);
            dictionary.Add("RepayType", "3");
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            return result.result;
        }

        public string RepayMoneyByPlatform(string code, string key, string money, string Platform)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "Repay", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("money", money.ToString());
            dictionary.Add("payType", Platform);
            dictionary.Add("RepayType", "2");
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

            return result.result;
        }


        public void InnerTransfer(string code, string key, string targetcode, string money, string pwd, string notes)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLfunds + "InnerTransfer", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("pwd", pwd);
            dictionary.Add("targetCode", targetcode);
            dictionary.Add("money", money.ToString());
            dictionary.Add("notes", notes);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }
        public void CashOut(string code, string key, decimal money, string AccountID, string pwd, string Type)
        {

            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLWithdraw + "ManualCreate", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("Amount", money.ToString());
            dictionary.Add("AccountID", AccountID);
            dictionary.Add("pwd", pwd);
            dictionary.Add("Type", Type);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

        }
        public string GetFeeAmount(string code, string key, string money, string Type)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLWithdraw + "GetFeeAmount", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("Amount", money);
            dictionary.Add("Type", Type);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }


        public FeeRuleInfo GetFeeRule(string code, string key)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLWithdraw + "GetFeeRule", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var FeeRuleInfo = new FeeRuleInfo()
            {
                //todayFee = result.result.todayFee,
                //todayMax = result.result.todayMax,
                //todayMin = result.result.todayMin,
                //tomorrowFee = result.result.tomorrowFee,
                //tomorrowMax = result.result.tomorrowMax,
                //tomorrowMin = result.result.tomorrowMin
                IsHoliday = result.result.IsHoliday,
                Id = result.result.withdraw.Id,
                Name = result.result.withdraw.Name,
                CustomerType = result.result.withdraw.CustomerType,
                IsDefault = result.result.withdraw.IsDefault,
                TodayEnable = result.result.withdraw.TodayEnable,
                TodayLast = result.result.withdraw.TodayLast,
                TodayWithdrawRateType = result.result.withdraw.TodayWithdrawRateType,
                TodayEachFeeAmount = result.result.withdraw.TodayEachFeeAmount,
                TodayEachRate = result.result.withdraw.TodayEachRate,
                TodayEachFeeAmountMin = result.result.withdraw.TodayEachFeeAmountMin,
                TodayEachFeeAmountMax = result.result.withdraw.TodayEachFeeAmountMax,
                TodayDayAmount = result.result.withdraw.TodayDayAmount,
                TodayEachAmount = result.result.withdraw.TodayEachAmount,
                MorrowEnable = result.result.withdraw.MorrowEnable,
                MorrowLast = result.result.withdraw.MorrowLast,
                MorrowWithdrawRateType = result.result.withdraw.MorrowWithdrawRateType,
                MorrowEachFeeAmount = result.result.withdraw.MorrowEachFeeAmount,
                MorrowEachRate = result.result.withdraw.MorrowEachRate,
                MorrowEachFeeAmountMin = result.result.withdraw.MorrowEachFeeAmountMin,
                MorrowEachFeeAmountMax = result.result.withdraw.MorrowEachFeeAmountMax,
                MorrowDayAmount = result.result.withdraw.MorrowDayAmount,
                MorrowEachAmount = result.result.withdraw.MorrowEachAmount
            };
            return FeeRuleInfo;
        }


        public string OnLineRecieveByBank(string code, string key, decimal money, string NotifyUrl, string payBank)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLWithdraw + "OnLineRecieve", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("money", money.ToString());
            dictionary.Add("bankName", payBank);
            dictionary.Add("NotifyUrl", NotifyUrl);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }

        public string OnLineRecieveByPlatform(string code, string key, decimal money, string NotifyUrl, string payPlatform)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLWithdraw + "OnLineRecieve", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("money", money.ToString());
            dictionary.Add("payType", payPlatform);
            dictionary.Add("NotifyUrl", NotifyUrl);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }


        public string GetApplicationMaxAmount(string code, string key, string Type)
        {
            BPiaoBao.Common.CashbagHelper ch = new BPiaoBao.Common.CashbagHelper(webURLWithdraw + "GetMaxAmount", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("Type", Type);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }

        /// <summary>
        /// 获取可用临时额度
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        public decimal GetTempCreditAmount(string code, string key)
        {
            var ch = new CashbagHelper(webURLAccount + "GetTempCreditAmount", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            return result.result;
        }
        /// <summary>
        /// 申请临时额度
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="tempAmount"></param>
        /// <param name="pwd"></param>
        public void TempCreditApplication(string code, string key, string pwd, decimal tempAmount)
        {
            var ch = new CashbagHelper(webURLAccount + "TempCreditApplication", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"pwd", pwd},
                {"tempAmount", tempAmount.ToString(CultureInfo.InvariantCulture)},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
        }
        /// <summary>
        /// 申请临时额度条件相关
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public TempCreditInfo GetTempCreditSetting(string code, string key)
        {
            var ch = new CashbagHelper(webURLAccount + "GetTempCreditSetting", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},
                {"currentTime", DateTime.Now.ToString("yyyyMMddHHmmss")}
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());
            var tempCreditInfo = new TempCreditInfo
            {
                Day = result.result.Day,
                Number = result.result.Number
            };
            return tempCreditInfo;
        }


        #region 支付宝快捷充值，还款

         
        public void AlipaySignRecharge(string code, string key, decimal money, string payPwd)
        {

            var ch = new CashbagHelper(webURLfunds + "AlipaySignRecharge", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},  
                {"money", money.ToString(CultureInfo.InvariantCulture)},
                {"pwd", payPwd} 
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

        }

        public void AlipaySignRepay(string code, string key, decimal money, string payPwd)
        {
            var ch = new CashbagHelper(webURLfunds + "AlipaySignRepay", "GET");
            var dictionary = new Dictionary<string, string>
            {
                {"code", code},
                {"key", key},  
                {"money", money.ToString(CultureInfo.InvariantCulture)},
                {"pwd", payPwd} 
            };
            var data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CashBagException(result.message.ToString());

        }

        #endregion


    }
}
