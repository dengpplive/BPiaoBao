using BPiaoBao.Cashbag.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Services
{
    public interface IFundClientProxy
    {
        /// <summary>
        /// 充值(银行)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="payBank"></param>
        /// <returns></returns>
        string RechargeByBank(string code, string key, decimal money, string payBank);
        /// <summary>
        /// 充值(第三方)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="payPlatform"></param>
        /// <returns></returns>
        string RechargeByPlatform(string code, string key, decimal money, string payPlatform);
        /// <summary>
        /// 还款(现金账户)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="CashAccount"></param>
        void RepayMoneyByCashAccount(string code, string key, string money, string pwd);
        /// <summary>
        /// 还款(银行卡)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="Bank"></param>
        /// <returns></returns>
        string RepayMoneyByBank(string code, string key, string money, string Bank);
        /// <summary>
        /// 还款(第三方)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        string RepayMoneyByPlatform(string code, string key, string money, string Platform);
        /// <summary>
        ///  转账
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="targetcode"></param>
        /// <param name="money"></param>
        /// <param name="pwd"></param>
        /// <param name="notes"></param>
        void InnerTransfer(string code, string key, string targetcode, string money, string pwd, string notes);
        /// <summary>
        /// 结算(提现)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="bankCardId"></param>
        /// <param name="pwd"></param>
        /// <param name="Type">0当天到账 1次日到账</param>
        void CashOut(string code, string key, decimal money, string bankCardId, string pwd, string Type);
        /// <summary>
        /// 获取结算手续费
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="Type">0当天到账 1次日到账</param>
        /// <returns></returns>
        string GetFeeAmount(string code, string key, string money, string Type);
        /// <summary>
        /// 获取提现手续费规则
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        FeeRuleInfo GetFeeRule(string code, string key);
        /// <summary>
        /// 在线收款（银行卡）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="NotifyUrl"></param>
        /// <param name="payBank"></param>
        /// <returns></returns>
        string OnLineRecieveByBank(string code, string key, decimal money, string NotifyUrl, string payBank);
        /// <summary>
        /// 在线收款（第三方）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="NotifyUrl"></param>
        /// <param name="payPlatform"></param>
        /// <returns></returns>
        string OnLineRecieveByPlatform(string code, string key, decimal money, string NotifyUrl, string payPlatform);
        /// <summary>
        /// 获取最高结算金额
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="Type">到账模式0当天到账 1次日到账</param>
        /// <returns></returns>
        string GetApplicationMaxAmount(string code, string key, string Type);
        /// <summary>
        ///  获取可用临时申请额度
        /// </summary>
        /// <returns></returns>
        decimal GetTempCreditAmount(string code, string key);

        /// <summary>
        ///  申请临时额度
        /// </summary>
        /// <param name="key"></param>
        /// <param name="pwd"></param>
        /// <param name="tempAmount"></param>
        /// <param name="code"></param>
        void TempCreditApplication(string code, string key, string pwd, decimal tempAmount);

        /// <summary>
        /// 申请临时额度条件相关
        /// </summary>
        /// <param name="key"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        TempCreditInfo GetTempCreditSetting(string code, string key);




        #region 支付宝快捷充值，还款
        
        /// <summary>
        /// 支付宝签约充值
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="payPwd"></param>
        void AlipaySignRecharge(string code, string key, decimal money, string payPwd);
        /// <summary>
        /// 支付宝签约还款
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="money"></param>
        /// <param name="payPwd"></param>
        void AlipaySignRepay(string code, string key, decimal money, string payPwd);


        #endregion
    }


}
