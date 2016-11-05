using BPiaoBao.AppServices.DataContracts.Cashbag;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.Cashbag
{
    [ServiceContract]
    public interface IFundService
    {
        /// <summary>
        /// 充值(银行)
        /// </summary>
        /// <param name="money"></param>
        /// <param name="payBank"></param>
        /// <returns></returns>
        [OperationContract]
        string RechargeByBank(decimal money, string payBank);
        /// <summary>
        /// 充值(第三方)
        /// </summary>
        /// <param name="money"></param>
        /// <param name="payPlatform"></param>
        /// <returns></returns>
        [OperationContract]
        string RechargeByPlatform(decimal money, string payPlatform);
        /// <summary>
        /// 还款(现金账户)
        /// </summary>
        /// <param name="money"></param>
        /// <param name="CashAccount"></param>
        [OperationContract]
        void RepayMoneyByCashAccount(string money, string pwd);

        /// <summary>
        /// 还款(银行卡)
        /// </summary>
        /// <param name="money"></param>
        /// <param name="Bank"></param>
        /// <returns></returns>
        [OperationContract]
        string RepayMoneyByBank(string money, string Bank);
        /// <summary>
        /// 还款(第三方)
        /// </summary>
        /// <param name="money"></param>
        /// <param name="Platform"></param>
        /// <returns></returns>
        [OperationContract]
        string RepayMoneyByPlatform(string money, string Platform);
        /// <summary>
        /// 转账
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="targetcode"></param>
        /// <param name="money"></param>
        /// <param name="pwd"></param>
        /// <param name="notes"></param>
        [OperationContract]
        void InnerTransfer(string targetcode, string money, string pwd, string notes);
        /// <summary>
        /// 结算(提现)
        /// </summary>
        /// <param name="money"></param>
        /// <param name="Type">0当天到账 1次日到账</param>
        [OperationContract]
        void CashOut(decimal money, string accountID, string pwd, string Type);
        /// <summary>
        /// 结算手续费
        /// </summary>
        /// <param name="money"></param>
        /// <param name="Type">>0当天到账 1次日到账</param>
        /// <returns></returns>
        [OperationContract]
        string GetFeeAmount(string money, string Type);
        /// <summary>
        /// 获取提现手续费规则
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        [OperationContract]
        FeeRuleInfoDto GetFeeRule();
        /// <summary>
        /// 在线收款（银行卡）
        /// </summary>
        /// <param name="money"></param>
        /// <param name="NotifyUrl"></param>
        /// <param name="payBank"></param>
        /// <returns></returns>
        [OperationContract]
        string OnLineRecieveByBank(decimal money, string NotifyUrl, string payBank);
        /// <summary>
        /// 在线收款（第三方）
        /// </summary>
        /// <param name="money"></param>
        /// <param name="NotifyUrl"></param>
        /// <param name="payPlatform"></param>
        /// <returns></returns>
        [OperationContract]
        string OnLineRecieveByPlatform(decimal money, string NotifyUrl, string payPlatform);
        /// <summary>
        /// 获取最高结算金额
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="Type">到账模式0当天到账 1次日到账</param>
        /// <returns></returns>
        [OperationContract]
        string GetApplicationMaxAmount(string Type);
        /// <summary>
        /// 获取商户姓名
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [OperationContract]
        string GetTargetAccountName(string code);
        /// <summary>
        ///  获取可用临时申请额度
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        decimal GetTempCreditAmount();
        /// <summary>
        ///  申请临时额度
        /// </summary>
        /// <param name="pwd"></param>
        /// <param name="tempAmount"></param>
        [OperationContract]
        void TempCreditApplication(string pwd, decimal tempAmount);

        /// <summary>
        /// 申请临时额度条件相关
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        TempCreditInfoDto GetTempCreditSetting();



        #region 支付宝快捷充值，还款 
        /// <summary>
        /// 支付宝签约充值
        /// </summary>
        /// <param name="money"></param>
        /// <param name="payPwd"></param>
        [OperationContract]
        void AlipaySignRecharge(decimal money, string payPwd);
        /// <summary>
        /// 支付宝签约还款
        /// </summary>
        /// <param name="money"></param>
        /// <param name="payPwd"></param>
        [OperationContract]
        void AlipaySignRepay(decimal money, string payPwd);


        #endregion
    }
}
