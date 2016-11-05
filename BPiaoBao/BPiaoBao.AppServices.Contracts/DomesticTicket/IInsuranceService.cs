using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;

namespace BPiaoBao.AppServices.Contracts.DomesticTicket
{
    [ServiceContract]
    public interface IInsuranceService
    {
  
        /// <summary>
        /// 得到控台保险设置
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CtrlInsuranceDto GetCtrlInsurance();


        
        /// <summary>
        /// 得到急速退实体
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        CtrlInsuranceRefundDto GetCtrlInsuranceRefund();

        /// <summary>
        /// 保存控台保险设置
        /// </summary>
        [OperationContract]
        void SaveCtrlConfig(CtrlInsuranceDto dto);

       

        /// <summary>
        /// 保存急速退设置
        /// </summary>
        [OperationContract]
        void SaveCtrlRefundConfig(CtrlInsuranceRefundDto dto);


        /// <summary>
        /// 查询保单
        /// </summary>
        /// <param name="qInsurance">查询条件实体</param>
        /// <param name="pageIndex">开始页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ResponseInsurance> QueryInsurance(RequestQueryInsurance qInsurance, int pageIndex, int pageSize = 20);

      
        /// <summary>
        /// 查询商户购买保险记录
        /// </summary> 
        /// <param name="qInsuranceDepositLog">查询条件实体</param> 
        /// <param name="pageIndex">开始页索引</param>
        /// <param name="pageSize">每页条数</param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ResponseInsuranceDepositLog> QueryInsuranceDepositLog(RequestQueryInsuranceDepositLog qInsuranceDepositLog,int pageIndex, int pageSize = 20);

        /// <summary>
        /// 查询当前商户的保险设置
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        InsuranceConfigData QueryInsuranceConfig();


        /// <summary>
        /// 保存当前商户的保险设置
        /// </summary>
        /// <param name="req"></param>
        [OperationContract]
        void SaveInsuranceConfig(InsuranceConfigData req);


        /// <summary>
        /// 保存保单信息
        /// </summary>
        /// <param name="req"></param>
        [OperationContract]
        void SaveInsurance(RequestInsurance req);

        ///// <summary>
        ///// 保存充值记录
        ///// </summary>
        ///// <param name="req"></param>
        //[OperationContract]
        //void SaveInsuranceDespositLog(RequestInsuranceDepositLog req);

        /// <summary>
        /// 运营商通过现金账号购买保险
        /// </summary>
        /// <param name="buyCount"></param>
        /// <param name="pwd"></param>
        [OperationContract]
        void BuyInsuranceByCashByCarrier(int buyCount, string pwd);

        /// <summary>
        /// 现金账户或信用卡支付
        /// </summary>
        /// <param name="buyCount"></param>
        /// <param name="pwd"></param>
        /// <param name="payType">0：现金账户；1：信用卡账户</param>
        [OperationContract]
        void BuyInsuranceByCashOrCredit(string OrderId, List<PassengerDto> passengers, int payType, string pwd);

        /// <summary>
        /// 得到当前保险配置信息
        /// </summary>
        [OperationContract]
        ResponseCurrentCarrierInsurance GetCurentInsuranceCfgInfo(bool isBuyer = false);

        /// <summary>
        /// 查询分销商购买保险记录
        /// </summary>
        /// <param name="request"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ResponseInsurancePurchaseByBussinessman> QueryInsurancePurchaseByBussinessman(RequestQueryInsurancePurchaseByBussinessman request, int pageIndex, int pageSize = 20);

        /// <summary>
        /// 从运营商处购买记录
        /// </summary>
        /// <param name="request"></param>
        [OperationContract]
        void PurchaseInsuranceFromCarrier(RequestPurchaseFromCarrier request);

        /// <summary>
        /// 运营商赠送保险
        /// </summary>
        /// <param name="request"></param>
        [OperationContract]
        void OfferInsuranceToBuyer(RequestOfferInsuranceToBuyer request);

        /// <summary>
        /// 赠送运营商保险
        /// </summary>
        /// <param name="request"></param>
        [OperationContract]
        void OfferInsuranceToCarrier(RequestOfferInsuranceToCarrier request);

        /// <summary>
        /// 手动投保
        /// </summary>
        /// <param name="request"></param>
        [OperationContract]
        void BuyInsuranceManually(RequestBuyInsuranceManually request);

        /// <summary>
        /// 删除保单
        /// </summary>
        /// <param name="ticketOrderId"></param>
        [OperationContract]
        void DeleteInsurance(string ticketOrderId);

        /// <summary>
        /// 手动获得保单号
        /// </summary>
        /// <param name="ticketOrderId"></param>
        [OperationContract]
        void GetInsuranceNoManual(string insuranceOrderId, string recordId);
        /// <summary>
        /// 保单使用汇总
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseInsuranceUse> GetInsuranceUseSum(DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// 保单销售汇总(运营)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseInsuranceSaleSum> GetCarrierInsuranceSaleSum(DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// 保单销售汇总(控台)
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseInsuranceSaleSum> GetInsuranceSaleSum(DateTime? startTime, DateTime? endTime);
    }
}
