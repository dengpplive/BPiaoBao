using System;
using System.Collections.Generic;
using System.ServiceModel;
using BPiaoBao.AppServices.DataContracts.Cashbag;
using BPiaoBao.AppServices.DataContracts;
namespace BPiaoBao.AppServices.Contracts.Cashbag
{
    [ServiceContract]
    public interface IAccountService
    {
        #region 记录

        /// <summary>
        /// 提现记录信息获取
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<CashOutLogDto> FindCashOutLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);

        /// <summary>
        /// 理财记录信息获取
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<FinancialLogDto> FindFinancialLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);

        /// <summary>
        /// 充值记录信息获取
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<RechargeLogDto> FindRechargeLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);
        /// <summary>
        /// 还款记录信息获取
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<RepaymentLogDto> FindRepaymentLog(DateTime? startTime, DateTime? endTime, int startIndex, int count);

        /// <summary>
        /// 转账记录信息获取
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<TransferAccountsLogDto> FindTransferAccountsLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);

        /// <summary>
        /// 交易记录
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<BargainLogDto> GetBargainLog(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);
        /// <summary>
        /// 积分兑换记录
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ScoreConvertLogDto> GetScoreConvertLog(DateTime? startTime, DateTime? endTime, int startIndex, int count);
        #endregion
        #region 明细
        // /// <summary>
        // /// 现金账户明细
        // /// </summary>
        // /// <param name="code"></param>
        // /// <param name="key"></param>
        // /// <param name="startTime"></param>
        // /// <param name="endTime"></param>
        // /// <param name="startIndex"></param>
        // /// <param name="count"></param>
        // /// <returns></returns>
        //[OperationContract]
        // DataPack<BalanceDetailDto> GetReadyAccountDetails(DateTime? startTime, DateTime? endTime, int startIndex, int count);

        /// <summary>
        /// 现金账户明细(带交易号/流水号的查询接口)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="outTradeNo"></param>
        /// <param name="orderNo"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<BalanceDetailDto> GetReadyAccountDetails(DateTime? startTime, DateTime? endTime, string outTradeNo, string orderNo, int startIndex, int count);

        /// <summary>
        /// 信用账户明细
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<BalanceDetailDto> GetCreditAccountDetails(DateTime? startTime, DateTime? endTime, int startIndex, int count);

        /// <summary>
        /// 积分账户明细
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<BalanceDetailDto> GetScoreAccountDetails(DateTime? startTime, DateTime? endTime, int startIndex, int count, string outTradeNo = null);
        #endregion
        #region 账单
        /// <summary>
        /// 获取账单列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="status"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<BillListDto> GetBill(DateTime? startTime, DateTime? endTime, string status, int startIndex, int count);

        /// <summary>
        /// 获取消费账单明细列表
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="payNo"></param>
        /// <param name="amountMin"></param>
        /// <param name="amountMax"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<BillDetailListDto> GetBillDetail(DateTime? startTime, DateTime? endTime, string payNo, string amountMin, string amountMax, int startIndex, int count, string outTradeNo);

        /// <summary>
        /// 获取还款账单明细列表
        /// </summary>
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
        [OperationContract]
        DataPack<RePayDetailListDto> GetRePayDetail(DateTime? startTime, DateTime? endTime, DateTime? repayStartDate, DateTime? repayEndDate, string payNo, string status, int startIndex, int count, string outTradeNo);
        #endregion
        #region 信用账户开通（申请，查询）
        /// <summary>
        /// 信用账户开通申请
        /// </summary>
        /// <param name="ca">信用认证(有多张图用逗号分隔,下同)</param>
        /// <param name="ra">收入认证</param>
        /// <param name="wa">工作认证</param>
        /// <param name="ha">房产认证</param>
        /// <param name="ba">购车认证</param>
        /// <param name="ma">结婚认证</param>
        /// <param name="ea">教育认证</param>
        /// <param name="ia">身份证</param>
        /// <returns></returns>
        [OperationContract]
        void GrantApply(string ca, string ra, string wa, string ha, string ba, string ma, string ea, string ia);
        /// <summary>
        /// 获取信用账户开通信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        GrantInfoDto GetGrantInfo();
        #endregion
        #region 银行卡
        /// <summary>
        /// 取银行卡信息
        /// </summary> 
        /// <returns></returns>
        [OperationContract]
        List<BankCardDto> GetBank();
        /// <summary>
        /// 添加银行卡
        /// </summary> 
        /// <param name="bank"></param>
        [OperationContract]
        void AddBank(BankCardDto bank);

        /// <summary>
        /// 修改银行卡
        /// </summary>
        /// <param name="bank"></param>
        [OperationContract]
        void ModifyBank(BankCardDto bank);

        /// <summary>
        /// 设置默认银行卡信息
        /// </summary>
        /// <param name="bankId"></param>
        [OperationContract]
        void SetDefaultBank(string bankId);
        /// <summary>
        /// 删除银行卡
        /// </summary>
        /// <param name="bankId"></param>
        [OperationContract]
        void RemoveBank(string bankId);


        /// <summary>
        /// 获取银行信息列表
        /// </summary>
        [OperationContract]
        List<BankInfoDto> GetBankListInfo();

        #endregion
        #region 支付密码修改,短信验证码
        /// <summary>
        /// 获取短信验证码
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetValidateCode();
        /// <summary>
        /// 修改支付密码
        /// </summary>
        /// <param name="newpwd"></param>
        /// <param name="smsPwd"></param>
        [OperationContract]
        void SetPayPassword(string newpwd, string smsPwd);
        #endregion
        #region 其它信息
        /// <summary>
        /// 获取账户信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        AccountInfoDto GetAccountInfo();
        /// <summary>
        /// 获取钱袋子公司信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CashCompanyInfoDto GetCompanyInfo();
        /// <summary>
        /// 获取还款信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        RepayInfoDto GetRepayInfo();
        #endregion
        /// <summary>
        /// 兑换积分
        /// </summary>
        /// <param name="source"></param>
        [OperationContract]
        void ExchangeSource(decimal source);
        #region 新增修改用户
        /// <summary>
        /// 新增商户(需要参数:CpyName,Contact,Moblie,ClientAccount)
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CashCompanyInfoDto AddCompany(CashCompanyInfoDto cashcpyinfo);
        /// <summary>
        /// 修改商户（需要参数：PayAccount,ContactUser,Moblie）
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        void UpdateCompany(CashCompanyInfoDto cashcpyinfo);
        #endregion


        #region  支付宝代扣获取签约地址,绑定，解绑，查询
        
        /// <summary>
        /// 获取签约地址
        /// </summary>
        /// <param name="alipayAccount"></param>
        /// <returns></returns>
        [OperationContract]
        string GetAlipaySign(string alipayAccount);
        /// <summary>
        /// 绑定
        /// </summary>
        /// <param name="alipayAccount"></param>
        /// <param name="payPwd"></param>
        /// <returns></returns>
        [OperationContract]
        string AlipayBind(string alipayAccount, string payPwd);
        /// <summary>
        /// 解绑
        /// </summary>
        /// <param name="payPwd"></param>
        /// <returns></returns>
        [OperationContract]
        string AlipayUnBind(string payPwd);
        /// <summary>
        /// 得到绑定帐户信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
       QuickPayDto GetBindAccount();
        #endregion
    }
}
