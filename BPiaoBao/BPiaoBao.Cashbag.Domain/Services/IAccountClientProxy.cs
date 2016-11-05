using BPiaoBao.Cashbag.Domain.Models;
using System;
using System.Collections.Generic;

namespace BPiaoBao.Cashbag.Domain.Services
{
    public interface IAccountClientProxy
    {
        #region 历史记录

        /// <summary>
        /// 充值记录信息获取
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Tuple<IEnumerable<RechargeLog>, int> GetRechargeLogs(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);

        /// <summary>
        /// 结算(提现)记录信息获取
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Tuple<IEnumerable<CashOutLog>, int> GetCashOutLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);

        /// <summary>
        /// 转账记录信息获取
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Tuple<IEnumerable<TransferLog>, int> GetTransferAccountsLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);

        /// <summary>
        /// 理财记录信息获取
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Tuple<IEnumerable<FinancialLog>, int> GetFinancialLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);
        /// <summary>
        /// 还款记录信息获取
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Tuple<IEnumerable<RepaymentLog>, int> GetRepaymentLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count);

        /// <summary>
        /// 交易记录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Tuple<IEnumerable<BargainLog>, int> GetBargainLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);
        /// <summary>
        /// 积分兑换记录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Tuple<IEnumerable<ScoreConvertLog>, int> GetScoreConvertLog(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count);
        #endregion

        #region 明细
        ///// <summary>
        ///// 现金账户明细
        ///// </summary>
        ///// <param name="code"></param>
        ///// <param name="key"></param>
        ///// <param name="startTime"></param>
        ///// <param name="endTime"></param>
        ///// <param name="startIndex"></param>
        ///// <param name="count"></param>
        ///// <returns></returns>
        //Tuple<IEnumerable<BalanceDetail>, int> GetReadyAccountDetails(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count);

        /// <summary>
        /// 现金账户明细(带交易号/流水号的查询接口)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="outTradeNo"></param>
        /// <param name="orderNo"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Tuple<IEnumerable<BalanceDetail>, int> GetReadyAccountDetails(string code, string key, DateTime? startTime, DateTime? endTime, string outTradeNo, string orderNo, int startIndex, int count);
        /// <summary>
        /// 信用账户明细
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Tuple<IEnumerable<BalanceDetail>, int> GetCreditAccountDetails(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count);

        /// <summary>
        /// 积分账户明细
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Tuple<IEnumerable<BalanceDetail>, int> GetScoreAccountDetails(string code, string key, DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);
        #endregion

        #region 账单
        /// <summary>
        /// 获取账单列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status">-1或不传查询全部 1已清 0未清</param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        Tuple<IEnumerable<BillList>, int> GetBill(string code, string key, DateTime? startTime, DateTime? endTime, string status, int startIndex, int count);

        /// <summary>
        /// 获取消费账单明细列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="payNo"></param>
        /// <param name="amountMin"></param>
        /// <param name="amountMax"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Tuple<IEnumerable<BillDetail>, int> GetBillDetail(string code, string key, DateTime? startTime, DateTime? endTime, string payNo, string amountMin, string amountMax, int startIndex, int count, string outTradeNo);

        /// <summary>
        /// 获取还款账单明细列表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="repayStartDate"></param>
        /// <param name="repayEndDate"></param>
        /// <param name="payNo"></param>
        /// <param name="status"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        Tuple<IEnumerable<BillDetail>, int> GetRePayDetail(string code, string key, DateTime? startTime, DateTime? endTime, DateTime? repayStartDate, DateTime? repayEndDate, string payNo, string status, int startIndex, int count, string outTradeNo);
        #endregion

        #region 信用账户开通（申请，查询）
        /// <summary>
        /// 信用账户开通申请
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="ca">信用认证(有多张图用逗号分隔,下同)</param>
        /// <param name="ra">收入认证</param>
        /// <param name="wa">工作认证</param>
        /// <param name="ha">房产认证</param>
        /// <param name="ba">购车认证</param>
        /// <param name="ma">结婚认证</param>
        /// <param name="ea">教育认证</param>
        /// <param name="ia">身份证</param>
        /// <returns></returns>
        void GrantApply(string code, string key, string ca, string ra, string wa, string ha, string ba, string ma, string ea, string ia);
        /// <summary>
        /// 获取信用账户开通信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        GrantInfo GetGrantInfo(string code, string key);
        #endregion

        #region 银行卡
        /// <summary>
        /// 取银行卡信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        IEnumerable<BankCard> GetBank(string code, string key);
        /// <summary>
        /// 添加银行卡
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="bank"></param>
        void AddBank(string code, string key, BankCard bank);

        /// <summary>
        /// 修改银行卡
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="bank"></param>
        void ModifyBank(string code, string key, BankCard bank);

        /// <summary>
        /// 设置默认银行卡信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="bankId"></param>
        void SetDefaultBank(string code, string key, string bankId);
        /// <summary>
        /// 删除银行卡
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="bankId"></param>
        void RemoveBank(string code, string key, string bankId);

        /// <summary>
        /// 获取银行信息列表
        /// </summary> 
        IEnumerable<BankInfo> GetBankListInfo(string code, string key);
        #endregion

        #region 支付密码修改,短信验证码
        /// <summary>
        /// 获取短信验证码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        string GetValidateCode(string code, string key);

        /// <summary>
        /// 修改支付密码
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="newpwd"></param>
        /// <param name="smsPwd"></param>
        void SetPayPassword(string code, string key, string newpwd, string smsPwd);
        #endregion

        #region 其它信息
        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        AccountInfo GetAccountInfo(string code, string key);
        /// <summary>
        /// 获取钱袋子公司信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        CashCompanyInfo GetCompanyInfo(string code, string key);
        /// <summary>
        /// 获取还款信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        RepayInfo GetRepayInfo(string code, string key);
        #endregion

        #region 手工兑换积分
        void ExchangeSource(string code, string key, decimal source);
        #endregion

        #region 新增修改用户

        /// <summary>
        /// 新增商户(需要参数:CpyName,Contact,Moblie,ClientAccount)
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="cashcpyinfo"></param>
        /// <returns></returns>
        CashCompanyInfo AddCompany(string code, string key, CashCompanyInfo cashcpyinfo);

        /// <summary>
        /// 修改商户（需要参数：PayAccount,ContactUser,Moblie）
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="cashcpyinfo"></param>
        void UpdateCompany(string code, string key, CashCompanyInfo cashcpyinfo);
        /// <summary>
        /// 删除钱袋子商户
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="payAccount"></param>
        void DeleteCashBagBusinessman(string code, string payAccount, string key);
        #endregion

        #region 支付宝代扣获取签约地址

        /// <summary>
        /// 获取签约地址
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="alipayAccount"></param>
        /// <returns></returns>
        string GetAliPaySign(string code, string key, string alipayAccount);
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="alipayAccount"></param>
        /// <param name="payPwd"></param>
        /// <returns></returns>
        string AlipayBind(string code, string key, string alipayAccount, string payPwd);
        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="payPwd"></param>
        /// <returns></returns>
        string AlipayUnBind(string code, string key, string payPwd);
        /// <summary>
        /// 得到绑定帐户信息
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
        Tuple<string, string> GetBindAccount(string code, string key);


        #endregion

    }
}
