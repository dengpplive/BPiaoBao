using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.DataContracts.TPos;

namespace BPiaoBao.AppServices.Contracts.DomesticTicket
{
    [ServiceContract]
    public interface IOrderService
    {
        /// <summary>
        ///   根据订单号重新获取政策
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>       
        [OperationContract]
        PolicyPack GetPolicyByOrderId(string OrderId);

        /// <summary>
        /// 取消本地订单
        /// </summary>
        /// <param name="OrderId"></param>
        [OperationContract]
        void CancelOrder(string OrderId, bool IsCancelPnr = false);

        /// <summary>
        /// 预定
        /// </summary>
        /// <param name="OrderParam"></param>
        /// <returns></returns>
        [OperationContract]
        PolicyPack Destine(DestineRequest destine, EnumDestineSource destineSource);

        /// <summary>
        ///  导入Pnr内容 生成默认订单
        /// </summary>
        /// <param name="pnrContext"></param>
        /// <param name="PnrSource"></param> 
        /// <returns></returns>
        [OperationContract]
        PolicyPack ImportPnrContext(PnrImportParam PnrParam);

        /// <summary>
        /// 通过订单号取编码内容导入生成新订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        PolicyPack ImportByOrderId(string orderId);

        /// <summary>
        /// 选择政策确认订单 修改有关订单数据
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="policyId"></param>
        /// <param name="orderId"></param>
        [OperationContract]
        OrderDto ChoosePolicy(string platformCode, string policyId, string orderId);

        /// <summary>
        /// 新的重选政策确认订单
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract(Name = "NewChoosePolicy")]
        OrderDto ChoosePolicy(PolicyDto policy, string orderId);

        /// <summary>
        /// 支付宝快捷支付代扣支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="password"></param>
        [OperationContract]
        string PayOrderByQuikAliPay(string orderId, string password);
        /// <summary>
        /// 使用钱袋子支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        void PayOrderByCashbagAccount(string orderId, string payPassword);
        /// <summary>
        /// 使用信用账户支付
        /// </summary>
        /// <param name="orderId"></param>
        [OperationContract]
        void PayOrderByCreditAccount(string orderId, string payPassword);
        /// <summary>
        /// 使用银行卡支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="bankCode"></param>
        /// <returns></returns>
        [OperationContract]
        string PayOrderByBank(string orderId, string bankCode);

        /// <summary>
        /// 使用支付平台支付
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="platformCode"></param>
        /// <returns></returns>
        [OperationContract]
        string PayOrderByPlatform(string orderId, string platform);


        /// <summary>
        /// 查询定单信息(前台专用，转化过订单状态)
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="pnr"></param>
        /// <param name="passengerName"></param>
        /// <param name="orderStatus"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<OrderDto> FindAll(string orderId, string pnr, string passengerName, DateTime? startCreateTime, DateTime? endCreateTime, int? orderStatus, int startIndex, int count, bool InterfaceOrder = true, bool ShrOrder = true);
        /// <summary>
        /// 机票查询统计(每月)
        /// </summary>
        /// <param name="CpTime"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<DataStatisticsDto> QueryByCpTime(DateTime CpTime);

        /// <summary>
        /// 查询15天交易总量
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        Current15DayDataDto Query15DayStatistics();

        /// <summary>
        /// 查询机票信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="pnr"></param>
        /// <param name="ticketNumber"></param>
        /// <param name="orderStatus"></param>
        /// <param name="startCreateTime"></param>
        /// <param name="startCreateTime"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<TicketDataInfoDto> FindTicketInfo(string orderId, string pnr, string ticketNumber, string passengerName, int? orderStatus, DateTime? startCreateTime, DateTime? endCreateTime, int startIndex, int count);

        /// <summary>
        /// 机票总表
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="currentOrderId">售后订单号</param>
        /// <param name="pnr"></param>
        /// <param name="ticketNumber">票号</param>
        /// <param name="passengerName">乘机人姓名</param>
        /// <param name="startCreateTime"></param>
        /// <param name="endCreateTime"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <param name="transactionNumber"></param>
        /// <param name="ticketStatus"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ReponseTicketSum> FindTicketSum(string orderId, string currentOrderId, string pnr, string ticketNumber, string passengerName, DateTime? startCreateTime, DateTime? endCreateTime, int startIndex, int count, string transactionNumber = null, EnumTicketStatus? ticketStatus = null);

        /// <summary>
        /// 取订单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        OrderDetailDto GetClientOrderDetail(string orderId);
        /// <summary>
        /// 申请售后订单
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        void ApplySaleOrder(string orderId, RequestAfterSaleOrder afterSaleOrderDto);
        /// <summary>
        /// 获取订单售后列表信息
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [OperationContract]
        IEnumerable<ResponseAfterSaleOrder> GetAfterSaleOrderById(string orderId);
        /// <summary>
        /// 获取售后详细
        /// </summary>
        /// <returns></returns>
        [OperationContract]
        ResponseAfterSaleOrder GetAfterSaleOrderDetail(string orderId, int afterSaleOrderId);

        /// <summary>
        /// 查询订单
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="pnr">PNR</param>
        /// <param name="passengerName">乘机人</param>
        /// <param name="startCreateTime">创建时间[开始]</param>
        /// <param name="endCreateTime">创建时间[结束]</param>
        /// <param name="orderStatus">订单状态</param>
        /// <param name="startIndex">查询开始条数</param>
        /// <param name="count">分页大小</param>
        /// <param name="OutTradeNo"></param>
        /// <param name="startFlightTime">起飞时间</param>
        /// <param name="endFlightTime">起飞结束时间</param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ResponseOrder> GetOrderBySearch(string orderId, string pnr, string passengerName, DateTime? startCreateTime, DateTime? endCreateTime, int? orderStatus, int startIndex, int count, string OutTradeNo, DateTime? startFlightTime = null, DateTime? endFlightTime = null);
        [OperationContract]
        ResponseOrderDetail GetOrderByOrderId(string orderId);
        /// <summary>
        /// 改签支付
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="saleorderid">售后订单ID</param>
        /// <param name="enumPayMethod">支付方式</param>
        /// <param name="attachInfo">附加信息【信用和现金支付attachInfo为密码，银行卡编码，第三方编码】</param>
        /// <returns></returns>
        [OperationContract]
        void SaleOrderPayByCashbagAccount(int saleorderid, string payPassword);
        [OperationContract]
        void SaleOrderPayByCreditAccount(int saleorderid, string payPassword);
        [OperationContract]
        string SaleOrderPayByBank(int saleorderid, string bankCode);
        [OperationContract]
        string SaleOrderPayByPlatform(int saleorderid, string platform);
        [OperationContract]
        string SaleOrderPayByQuikAliPay(int saleorderid, string pwd);
        /// <summary>
        /// 获取退废改订单
        /// </summary>
        /// <param name="currentPageIndex">当前页码</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pnr">PNR</param>
        /// <param name="code">商户号</param>
        /// <param name="orderid">订单号</param>
        /// <param name="saleOrderType">退废改类型</param>
        /// <param name="status">退废改订单处理状态</param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ResponseAfterSaleOrder> GetSaleOrderBySearch(int currentPageIndex, int pageSize, string pnr, string orderid, EnumAfterSaleOrder? saleOrderType, EnumTfgProcessStatus? status, DateTime? startTime, DateTime? endTime, string payno, DateTime? startFlightTime = null, DateTime? endFlightTime = null, int id = 0);
        /// <summary>
        /// 更改订单的支付金额信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="totalmoney"></param>
        [OperationContract]
        void UpdateOrderPayMoney(string orderId, decimal totalmoney);
        /// <summary>
        /// 更改订单的状态信息(超时就失效)
        /// </summary>
        /// <param name="orderId"></param>
        [OperationContract]
        void UpdateOrderStatus(string orderId);

        /// <summary>
        /// 判断该订单中的编码是否已经支付成功过
        /// </summary>
        /// <param name="pnr"></param>
        /// <returns></returns>
        [OperationContract]
        bool PnrIsPay(string orderId);
        /// <summary>
        /// 线下婴儿申请
        /// </summary>
        [OperationContract]
        void ApplyBaby(ApplyBabyDataObject applyBaby);

        /// <summary>
        /// 订单支付查询
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="flag">查询来源标识</param>
        /// <returns></returns>
        [OperationContract]
        string QueryPayStatus(string orderId,string flag = null);

        /// <summary>
        /// 售后订单查询支付状态
        /// </summary>
        /// <param name="id"></param> 
        /// <returns></returns>
        [OperationContract]
        string QueryAfterSaleOrderPayStatus(int id);

    }
}
