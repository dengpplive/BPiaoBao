using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.SystemSetting
{
    [ServiceContract]
    public interface IBusinessmanService
    {

        /// <summary>
        /// 查询员工
        /// </summary>
        /// <param name="realName"></param>
        /// <param name="account"></param>
        /// <param name="operatorState"></param>
        /// <returns></returns>
        [OperationContract]
        List<OperatorDto> GetAllOperators(string realName, string account, EnumOperatorState? operatorState);
        /// <summary>
        /// 更改员工状态
        /// </summary>
        /// <param name="account"></param>
        [OperationContract]
        void ModifyOperatorState(string account);
        /// <summary>
        /// 账号是否重复
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        [OperationContract]
        bool IsExistAccount(string account);
        /// <summary>
        /// 删除员工
        /// </summary>
        /// <param name="account"></param>
        [OperationContract]
        void DeleteOperator(string account);
        /// <summary>
        /// 新增员工
        /// </summary>
        /// <param name="operatorDto"></param>
        [OperationContract]
        void AddOperator(OperatorDto operatorDto);
        /// <summary>
        /// 修改
        /// </summary>
        /// <param name="operatorDto"></param>
        [OperationContract]
        void UpdateOperator(OperatorDto operatorDto);
        /// <summary>
        /// 修改密码
        /// </summary>
        /// <param name="account"></param>
        /// <param name="newPassword"></param>
        [OperationContract]
        void ChangePassword(string account, string oldPassword, string newPassword);
        /// <summary>
        /// 重置员工密码
        /// </summary>
        /// <param name="account"></param>
        [OperationContract]
        void ResetPassword(string account);
        /// <summary>
        /// 发送短信
        /// </summary>
        /// <param name="receiveName">接收人姓名</param>
        /// <param name="receiveNum">接收号码</param>
        /// <param name="content">消息内容</param>
        [OperationContract]
        void SendSms(string receiveName, string receiveNum, string content);
        /// <summary>
        /// 短信发送（兼容手机端）
        /// </summary>
        /// <param name="OrderId">订单号</param>
        /// <param name="Content">内容（需服务端替换字段）</param>
        /// <param name="PassengerId">乘机人id</param>
        [OperationContract]
        void SendSmsForPhone(string OrderId, string Content, int[] PassengerId);
        /// <summary>
        /// 购买短信【账户支付】
        /// </summary>
        [OperationContract]
        void BuySmsByAccount(int count, decimal smsPrice, int payAccountWay, string payPassword);
        /// <summary>
        /// 短信购买【银行卡支付】
        /// </summary>
        /// <param name="oName"></param>
        /// <param name="count"></param>
        /// <param name="payAmount"></param>
        /// <returns></returns>
        [OperationContract]
        string BuySmsByBank(int count, decimal smsPrice, string bankCode);
        /// <summary>
        /// 短信购买【平台支付】
        /// </summary>
        /// <param name="oName"></param>
        /// <param name="count"></param>
        /// <param name="payAmount"></param>
        /// <param name="platformCode"></param>
        /// <returns></returns>
        [OperationContract]
        string BuySmsByPlatform(int count, decimal smsPrice, string platformCode);
        /// <summary>
        /// 短信发送列表
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<SendDetailDto> SendRecordByPage(int currentPageIndex, int? pageSize, DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// 购买短信记录
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<BuyDetailDto> BuyRecordByPage(int currentPageIndex, int? pageSize, DateTime? startTime, DateTime? endTime, string outTradeNo = null);
        /// <summary>
        /// 获取商户基本信息和系统信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Tuple<int, int, decimal> GetSystemInfo();
        /// <summary>
        /// 获取钱袋子余额信息
        /// </summary>
        /// <param name="code">商户号</param>
        /// <param name="key">合作者KEY</param>
        /// <returns></returns>
        [OperationContract]
        Tuple<decimal, decimal> GetRecieveAndCreditMoney();
        /// <summary>
        /// 获取当前账户信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CurrentUserInfoDto GetCurrentUserInfo();
        /// <summary>
        /// 获取商户名称
        /// </summary>
        /// <param name="code">商户号</param>
        /// <returns></returns>
        [OperationContract]
        string GetBusinessmanName(string code);
        /// <summary>
        /// 获取客服信息
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CustomerDto GetCustomerInfo();
        [OperationContract]
        DateTime GetServerTime();
        /// <summary>
        /// 获取短信模板
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<SMSTemplateDto> GetAllSmsTemplate();
        /// <summary>
        /// 获取短信模板by行程类型
        /// </summary>
        /// <param name="SkyWayType">行程类型</param>
        /// <returns></returns>
        [OperationContract]
        List<SMSTemplateDto> GetAllSmsTemplateForPhone(int SkyWayType);
        /// <summary>
        /// 获取短信费用设置
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        List<SMSChargeSetDto> GetAllChargeSet();
        /// <summary>
        /// 获取短信赠送记录
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<GiveDetailDto> GetSmsGiveDetail(int currentPageIndex, int pageSize, DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// 发送一般信息
        /// </summary>
        [OperationContract]
        void SendNormalMsg(string content, bool isRepeatSend = false);
    }
}
