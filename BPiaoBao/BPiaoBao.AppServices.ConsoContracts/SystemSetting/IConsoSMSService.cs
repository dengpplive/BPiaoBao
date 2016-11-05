using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting
{
    [ServiceContract]
    public interface IConsoSMSService
    {
        /// <summary>
        /// 添加模板
        /// </summary>
        /// <param name="smstemplate"></param>
        [OperationContract]
        void AddSmsTemplate(SmsTemplateDataObject smstemplate);
        /// <summary>
        /// 获取模板列表
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseSmsTemplate> GetSmsTemplateList(int currentPageIndex, int pageSize);
        /// <summary>
        /// 修改短信模板
        /// </summary>
        /// <param name="smstemplate"></param>
        [OperationContract]
        void EditSmsTemplate(SmsTemplateDataObject smstemplate);
        /// <summary>
        /// 删除模板
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        void DeleteSmsTemplate(int Id);
        /// <summary>
        /// 启用或禁用模板
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        void SmsTemplateEnableOrDisable(int Id);
        /// <summary>
        /// 根据id获取短信模板信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseSmsTemplate GetSmsTemplatebyId(int Id);
        /// <summary>
        /// 短信发送列表
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<SendDetailDataObj> GetSendRecordByPage(string name, string tel, int currentPageIndex, int pageSize, DateTime? startTime, DateTime? endTime, string businessName = "");
        /// <summary>
        /// 购买短信记录
        /// 
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="outTradeNo"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<BuyDetailDataObj> GetBuyRecordByPage(string name, int currentPageIndex, int pageSize, DateTime? startTime, DateTime? endTime, string businessName = "");
        /// <summary>
        /// 短信赠送记录
        /// </summary>
        /// <param name="giveName">赠予人</param>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<GiveDetailDataObj> GetGiveDetailByPage(string giveName, int currentPageIndex, int pageSize, DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// 短信赠送
        /// </summary>
        /// <param name="givedetail"></param>
        [OperationContract]
        void AddSmsGive(GiveDetailDataObj givedetail);
        /// <summary>
        /// 获取短信剩余条数
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        string GetSmsRemainCount();
        /// <summary>
        /// 获取短信费用设置
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<SMSChargeSetDataObj> GetSmsChargeSetByPage(int currentPageIndex, int pageSize);
        /// <summary>
        /// 添加费用设置
        /// </summary>
        /// <param name="smschargeset"></param>
        [OperationContract]
        void AddSmsChargeSet(SMSChargeSetDataObj smschargeset);
        /// <summary>
        /// 删除短信费用设置
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        void DeleteSmsChargeSet(int Id);
        /// <summary>
        /// 修改短信费用设置
        /// </summary>
        /// <param name="smschargeset"></param>
        [OperationContract]
        void EditSmsChargeSet(SMSChargeSetDataObj smschargeset);
        /// <summary>
        /// 根据ID获取费用设置信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        SMSChargeSetDataObj GetSmsChargeSetById(int Id);
        /// <summary>
        /// 启用或禁用短信费用设置
        /// </summary>
        /// <param name="Id"></param>
        [OperationContract]
        void SmsChargeSetEnableOrDisable(int Id);
        /// <summary>
        /// 获取购买短信费用设置
        /// </summary>
        /// <param name="currentPageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        List<SMSChargeSetDataObj> GetBuySmsChargeSetByPage();
        /// <summary>
        /// 购买短信（现金账户）
        /// </summary>
        /// <param name="ChargeSetId"></param>
        /// <param name="pwd"></param>
        /// <returns></returns>
        [OperationContract]
        bool BuySmsByCashAccount(int ChargeSetId, string pwd);
        /// <summary>
        /// 短信发送
        /// </summary>
        /// <param name="SendDetailobj"></param>
        [OperationContract]
        string SmsSend(SendDetailDataObj SendDetailobj);
    }
}
