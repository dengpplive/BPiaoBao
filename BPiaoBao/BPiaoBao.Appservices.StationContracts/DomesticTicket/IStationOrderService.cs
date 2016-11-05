using BPiaoBao.AppServices.DataContracts;
using BPiaoBao.AppServices.DataContracts.DomesticTicket;
using BPiaoBao.AppServices.StationContracts.StationMap;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.ServiceModel;
using System.Text;
using BPiaoBao.AppServices.StationContracts.DomesticTicket.DomesticMap;
using BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject;
using BPiaoBao.AppServices.DataContracts.TPos;
using BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap;

namespace BPiaoBao.AppServices.StationContracts.DomesticTicket
{
    [ServiceContract]
    public interface IStationOrderService
    {
        [OperationContract]
        void AutoRefund(string orderid, string remark, bool cancelPnr = true); 
        /// <summary>
        /// 根据原订单号获取新的政策列表并且生成默认订单
        /// </summary>
        /// <param name="pnrContext"></param>
        /// <returns></returns>
        [OperationContract]
        PolicyPack GetPolicyList(string orderId);
        /// <summary>
        /// 选择政策确认订单
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="policyId"></param>
        /// <param name="orderId"></param>
        [OperationContract]
        OrderDto BackChoosePolicy(string platformCode, string policyId, string orderId);

        /// <summary>
        /// 后台重选政策
        /// </summary>
        /// <param name="policy"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract(Name = "NewBackChoosePolicy")]
        OrderDto BackChoosePolicy(PolicyDto policy, string orderId);


        /// <summary>
        /// 查询定单信息
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="pnr"></param>
        /// <param name="passengerName"></param>
        /// <param name="orderStatus"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<OrderDto> FindAll(string PaySerialNumber, string orderId, string pnr, string passengerName, string ticketNumber,
            string fromCity, string toCity, DateTime? startDateTime, DateTime? toDateTime, string businessmanCode, DateTime? startCreateTime, DateTime? endDateTime,
            string carrayCode, string platformCode, int[] orderStatus, int[] specialquery, int? specialschk, int startIndex, int count, bool? interfaceOrder , bool? shareOrder ,bool? localOrder , string IsProcess = "");
        /// <summary>
        /// 获取运营商订单信息
        /// </summary>
        [OperationContract]
        DataPack<OrderDto> FindCarrierAll(string PaySerialNumber, string orderId, string pnr, string passengerName, string ticketNumber,
            string fromCity, string toCity, DateTime? startDateTime, DateTime? toDateTime, string businessmanCode, DateTime? startCreateTime, DateTime? endDateTime,
            string carrayCode, string platformCode, int[] orderStatus, int startIndex, int count);
        [OperationContract]
        DataPack<ResponseOrder> GetOrderBySearch(string orderId, string pnr, string passengerName, string ticketNumber,
            string fromCity, string toCity, DateTime? startDateTime, DateTime? toDateTime, string businessmanCode, DateTime? startCreateTime, DateTime? endDateTime,
            string carrayCode, string platformCode, int[] orderStatus, int startIndex, int count, string OutTradeNo);
        /// <summary>
        /// 生成接口订单
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="orderId"></param>
        [OperationContract]
        void CreateInterfaceOrder(string platformCode, string orderId);

        /// <summary>
        ///代付接口订单
        /// </summary>
        /// <param name="platformCode"></param>
        /// <param name="orderId"></param>
        [OperationContract]
        void PaidOrderByPlatform(string platformCode, string orderId);

        /// <summary>
        ///  出票
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="ticketInfo"></param>
        [OperationContract]
        void IssueTicket(string orderId, IList<PassengerTicketDto> ticketInfo, string remark);
        /// <summary>
        /// 手动出票
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="ticketInfo"></param>
        /// <param name="remark"></param>
        [OperationContract]
        void HandIssueTicket(string orderId, IList<PassengerTicketDto> ticketInfo, string remark, string newPnr = "");

        /// <summary>
        /// 查询订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        string QueryOrderStatus(string orderId);

        /// <summary>
        /// 修改订单状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        void UpdateOrderStatus(string orderId, int? orderStatus);

        /// <summary>
        /// 修改支付/代付状态
        /// </summary>
        /// <param name="orderId">订单号</param>
        /// <param name="updateType">0.修改支付状态 1.修改代付状态</param>
        /// <param name="payStatus">状态值 0未支/代付 1已支/代付</param>
        [OperationContract]
        void UpdatePayOrPaidStatus(string orderId, int updateType, int? payStatus);

        /// <summary>
        /// 查询支付状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="flag">标识是否为支付窗口支付时查询订单状态</param>
        /// <returns></returns>
        [OperationContract]
        string QueryPayStatus(string orderId,string flag = null);

        /// <summary>
        /// 售后订单查询支付状态
        /// </summary>
        /// <param name="id">订单号</param>
        /// <returns></returns>
        [OperationContract]
        string QueryAfterSaleOrderPayStatus(int id);

        /// <summary>
        /// 查询代付状态
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        string QueryPaidStatus(string orderId);

        /// <summary>
        /// 接口自动复合票号
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        void AutoCompositeTicket(string orderId);

        /// <summary>
        /// 代付成功通知 修改订单
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="platformCode"></param>
        /// <param name="p2"></param>
        /// <param name="p3"></param>
        [OperationContract]
        void UpdateOrderByPayNotify(string outOrderId, string platform, string SerialNumber, decimal PaidMeony);

        /// <summary>
        /// 系统手动 取消出票
        /// </summary>
        /// <param name="outOrderId"></param>
        /// <param name="nameValueCollection"></param>
        [OperationContract]
        void CancelIssueNotify(string orderId, string remark);
        /// <summary>
        /// 接口取消出票(通知)
        /// </summary>
        /// <param name="outOrderId"></param>
        /// <param name="nameValueCollection"></param>
        [OperationContract]
        void PlatformCancelIssueNotify(string outOrderId, string remark);
        /// <summary>
        /// 平台接口退款通知
        /// </summary>
        /// <param name="outOrderId"></param>
        /// <param name="args"></param>
        [OperationContract]
        void PlatformRefundNotify(string outOrderId, decimal refundMoney, string remark);

        /// <summary>
        /// 系统手动退款
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="refundMoney"></param>
        /// <param name="remark"></param>
        [OperationContract]
        void CashbagRefund(string orderId, decimal refundMoney, string remark);

        /// <summary>
        /// 同步订单
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="refundMoney"></param>
        /// <param name="remark"></param>
        [OperationContract]
        string SynOrder(string orderId);


        /// <summary>
        /// 获取订单详情
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        OrderDetailDto GetOrderDetail(string orderId);
        /// <summary>
        /// 获取订单详情（运营）
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        OrderDetailDto GetCarrierOrderDetail(string orderId);
        /// <summary>
        /// 锁定帐户
        /// </summary>
        [OperationContract]
        void LockAccount(int saleorderid);
        /// <summary>
        /// 解锁账户
        /// </summary>
        /// <param name="saleorderid"></param>
        [OperationContract]
        void UnlockAccount(int saleorderid);
        /// <summary>
        /// 售后订单处理
        /// </summary>
        /// <param name="saleorderid">售后订单ID</param>
        /// <param name="dic">售后乘机人列表</param>
        /// <param name="remark">备注</param>
        [OperationContract]
        void Process(int saleorderid, Dictionary<int, decimal> dic, string remark);
        /// <summary>
        /// 拒绝处理
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="afterorderid"></param>
        [OperationContract]
        void UnProcess(int afterorderid, string unReason);
        /// <summary>
        /// 获取售后订单列表
        /// </summary>
        /// <param name="currentPageIndex">当前页</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pnr">PNR</param>
        /// <param name="code">商户CODE</param>
        /// <param name="policyFrom">政策来源</param>
        /// <param name="orderid">订单号</param>
        /// <param name="status">处理状态</param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ResponseAfterSaleOrder> GetAfterSaleOrderByPager(string LockAccount, DateTime? StartCreateTime, DateTime? EndDateTime, string PaySerialNumber, int currentPageIndex, int pageSize, string pnr, string code, string policyFrom, string orderid, EnumAfterSaleOrder? saleOrderType, EnumTfgProcessStatus? status, bool? InterfaceOrder , bool? ShareOrder ,bool? localOrder, string passengerName = "", bool? isInsuranceRefund = null);
        /// <summary>
        /// 获当前运营取售后订单列表
        /// </summary>
        /// <param name="currentPageIndex">当前页</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="pnr">PNR</param>
        /// <param name="code">商户CODE</param>
        /// <param name="policyFrom">政策来源</param>
        /// <param name="orderid">订单号</param>
        /// <param name="status">处理状态</param>
        /// <returns></returns>
        [OperationContract]
        DataPack<ResponseAfterSaleOrder> GetCarrierAfterSaleOrder(string PaySerialNumber, int currentPageIndex, int pageSize, string pnr, string code, string policyFrom, string orderid, EnumAfterSaleOrder? saleOrderType, EnumTfgProcessStatus? status, string passengerName = "");
        /// <summary>
        /// 退款
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="saleorderid"></param>
        /// <param name="money"></param>
        [OperationContract]
        void SaleOrderRefund(int saleorderid);
        /// <summary>
        /// 明细退款
        /// </summary>
        /// <param name="saleorderid">售后订单ID</param>
        /// <param name="refundid">退款单ID</param>
        [OperationContract]
        void SingleRefund(int saleorderid, string refundid);
        /// <summary>
        /// 退款明细
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseBounLine> RefundDetails(int saleorderid);
        /// <summary>
        /// 锁定订单
        /// </summary>
        [OperationContract]
        void LockOrder(string orderId);

        /// <summary>
        /// 解锁订单
        /// </summary>
        [OperationContract]
        void UnLockOrder(string orderId);

        /// <summary>
        /// 是否可以操作订单
        /// </summary>
        [OperationContract]
        bool CanOperationOrder(string orderId);
        /// <summary>
        /// 售后订单完成
        /// </summary>
        /// <param name="saleorderid">售后订单ID</param>
        [OperationContract]
        void Completed(int saleorderid, Dictionary<int, string> dic);
        /// <summary>
        /// 详细售后订单
        /// </summary>
        /// <param name="saleOrderid"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseAfterSaleOrder AfterSaleOrderDetail(int saleOrderid);

        /// <summary>
        /// 获取协调
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        CoordinationDto GetCoordinationDto(string orderId);

        /// <summary>
        /// 添加协调
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        [OperationContract]
        void AddCoordinationDto(string orderId, string type, string CoordinationContent, bool IsCompleted);
        /// <summary>
        /// 售后订单协调
        /// </summary>
        /// <param name="aftersaleid"></param>
        /// <param name="type"></param>
        /// <param name="CoordinationContent"></param>
        /// <param name="isCompleted"></param>
        [OperationContract]
        void AddCoordination(int aftersaleid, string type, string CoordinationContent, bool isCompleted);
        /// <summary>
        /// 获取售后订单协调
        /// </summary>
        /// <param name="aftersaleid"></param>
        /// <returns></returns>
        [OperationContract]
        CoordinationDto GetCoordinationAfterSale(int aftersaleid);
        //#region Old报表
        ///// <summary>
        ///// 机票信息汇总
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //TicketInformationSummaryDto GetTicketInformationSummary(
        //    string orderId, string outOrderId, string pnr, string ticketNumber,
        //    string platformCode, string policyType,
        //    string carrayCode, string fromCityCode, string toCityCode,
        //    int? ticketStatus, string businessmanName, string businessmanCode, string carrierCode, string operatorAccount,
        //    DateTime? startPayTime, DateTime? endPayTime,
        //    DateTime? startIssueRefundTime, DateTime? endIssueRefundTime,
        //    DateTime? startCreateTime, DateTime? endCreateTime,
        //    int? PayWay
        //    );
        ///// <summary>
        ///// 获取机票信息汇总数据列(转换成列)
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //List<TicketInfoSummaryEntity> GetTicketInfoSummaryList(string orderId, string outOrderId, string pnr, string ticketNumber,
        //    string platformCode, string policyType,
        //    string carrayCode, string fromCityCode, string toCityCode,
        //    int? ticketStatus, string businessmanName, string businessmanCode, string carrierCode, string operatorAccount,
        //    DateTime? startPayTime, DateTime? endPayTime,
        //    DateTime? startIssueRefundTime, DateTime? endIssueRefundTime,
        //    DateTime? startCreateTime, DateTime? endCreateTime,
        //    int? PayWay);

        ///// <summary>       
        ////机票销售统计
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //TicketSalesStatisticsDto GetTicketSalesStatistics(
        //    string orderId, string outOrderId, string pnr, string ticketNumber,
        //    string platformCode, string policyType,
        //    string carrayCode, string fromCityCode, string toCityCode,
        //    int? ticketStatus, string businessmanName, string businessmanCode, string carrierCode, string operatorAccount,
        //    DateTime? startPayTime, DateTime? endPayTime,
        //    DateTime? startIssueRefundTime, DateTime? endIssueRefundTime,
        //    DateTime? startCreateTime, DateTime? endCreateTime,
        //    int? PayWay
        //    );
        ///// <summary>
        ///// 获取机票销售统计数据列(转换成列)
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //List<TicketSalesStatisticsEntity> GetTicketSalesStatisticsList(
        //    string orderId, string outOrderId, string pnr, string ticketNumber,
        //    string platformCode, string policyType,
        //    string carrayCode, string fromCityCode, string toCityCode,
        //    int? ticketStatus, string businessmanName, string businessmanCode, string carrierCode, string operatorAccount,
        //    DateTime? startPayTime, DateTime? endPayTime,
        //    DateTime? startIssueRefundTime, DateTime? endIssueRefundTime,
        //    DateTime? startCreateTime, DateTime? endCreateTime,
        //    int? PayWay
        //    );
        ///// <summary>
        ///// 机票信息明细
        ///// </summary>
        ///// <returns></returns>
        //[OperationContract]
        //DataPack<TicketInformationDetailDto> GetTicketInformationDetail(
        //    string orderId, string outOrderId, string pnr, string ticketNumber,
        //    string platformCode, string policyType,
        //    string carrayCode, string fromCityCode, string toCityCode,
        //    int? ticketStatus, string businessmanName, string businessmanCode, string carrierCode, string operatorAccount,
        //    DateTime? startPayTime, DateTime? endPayTime,
        //    DateTime? startIssueRefundTime, DateTime? endIssueRefundTime,
        //    DateTime? startCreateTime, DateTime? endCreateTime,
        //    int startIndex, int count, int? PayWay, int? businessType
        //    );
        //#endregion
        /// <summary>
        /// 获取未起飞机票信息
        /// </summary>
        /// <param name="Codelist"></param>
        /// <returns></returns>
        [OperationContract]
        List<NotTakeOffTicketDto> GetNotTakeOffTicket(List<string> Codelist);
        /// <summary>
        /// 运营销售统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="BusinessmanCode">运营code</param>
        /// <returns></returns>
        [OperationContract]
        TicketBusniessStaticsDto GetTicketCarrierStatics(DateTime? startTime, DateTime? endTime, string CarrierCode);
        /// <summary>
        /// 用户销售统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="BusinessmanCode">商户code</param>
        /// <returns></returns>
        [OperationContract]
        TicketBusniessStaticsDto GetTicketBusniessStatics(DateTime? startTime, DateTime? endTime, string BusinessmanCode);
        /// <summary>
        /// 平台接口销售统计
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="PlatformCode">接口</param>
        /// <returns></returns>
        [OperationContract]
        TicketBusniessStaticsDto GetTicketInterfaceStatics(DateTime? startTime, DateTime? endTime, string PlatformCode);
        /// <summary>
        /// 退款查询
        /// </summary>
        /// <param name="aftersaleorder_id">售后订单号</param>
        /// <param name="refundNo">退款单号</param>
        /// <returns></returns>
        [OperationContract]
        string RefundQuery(int aftersaleorder_id, string refundNo);
        /// <summary>
        /// 线下代付
        /// </summary>
        [OperationContract]
        void UpdateOrderPay(OrderDataObject orderdata);
        /// <summary>
        /// 获取已有代付信息
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [OperationContract]
        OrderDataObject GetOrderPayInfo(string orderid);
        /// <summary>
        /// 修改乘机人信息
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="list"></param>
        [OperationContract]
        void UpdatePassenger(string orderid, List<PassengerDataObject> list);
        /// <summary>
        /// 乘机人信息
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [OperationContract]
        List<PassengerDataObject> GetPassengerInfo(string orderid);
        /// <summary>
        /// 获取改签单乘机人信息
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        [OperationContract]
        List<AfterPassengerDataObject> GetAfterPassengerInfo(int saleorderid);
        /// <summary>
        /// 机票起飞日期监控分析
        /// </summary>
        /// <param name="code">商户号</param>
        /// <param name="CreateTime">出票时间-范围:开始</param>
        /// <param name="CreateTimeEnd">出票时间-范围:结束</param>
        /// <returns></returns>
        [OperationContract]
        List<ResponeTempSum> MonitorTicketSum(string code, DateTime? CreateTime, DateTime? CreateTimeEnd);
        /// <summary>
        /// 手动调用自动出票
        /// </summary>
        /// <param name="orderId"></param>      
        [OperationContract]
        void HandCallAutoIssue(string orderId);
        /// <summary>
        /// 线下婴儿申请审核
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="seatPrice"></param>
        [OperationContract]
        void ExamineBabyOrder(string orderid, decimal seatPrice);
        /// <summary>
        /// 备注
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="remark"></param>
        [OperationContract]
        void UnExamine(string orderid, string remark);

        /// <summary>
        /// 出票统计
        /// </summary>
        /// <param name="code">商户号</param>
        /// <param name="CreateTime">出票时间-范围:开始</param>
        /// <param name="CreateTimeEnd">出票时间-范围:结束</param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseAllTicketSum> TicketSum(string code, DateTime? CreateTime, DateTime? CreateTimeEnd);

        /// <summary>
        /// 机票汇总
        /// </summary>
        /// <param name="ticketQueryEntity"></param>
        /// <returns></returns>
        [OperationContract]
        List<TicketInformationSummaryDto> GetTicketSumSummary(TicketQueryEntity ticketQueryEntity);

        /// <summary>
        /// 机票销售统计
        /// </summary>
        /// <param name="ticketQueryEntity"></param>
        /// <returns></returns>
        [OperationContract]
        TicketSalesStatisticsDto GetTicketSumSales(TicketQueryEntity ticketQueryEntity);

        /// <summary>
        /// 机票总表明细
        /// </summary>
        /// <param name="ticketQueryEntity"></param>
        /// <returns></returns>
        [OperationContract]
        DataPack<TicketSumDetailDto> GetTicketSumDetail(TicketQueryEntity ticketQueryEntity);


        /// <summary>
        /// 机票销售汇总
        /// </summary>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
        [OperationContract]
        List<ResponseTicketSaleSum> FindTicketSaleSum(DateTime? startTime, DateTime? endTime);
        /// <summary>
        /// QT信息
        /// </summary>
        /// <param name="pnrCode"></param>
        /// <returns></returns>
        [OperationContract]
        QTInfo QTRecord(string pnrCode);

        ///// <summary>
        ///// 创建航变信息
        ///// </summary>
        ///// <param name="model"></param>
        ///// <returns></returns>
        //[OperationContract]
        //List<AirChange> CreateAirChangeInfo(BPiaoBao.AppServices.DataContracts.DomesticTicket.QTResponse model);
        /// <summary>
        /// 查询航变列表
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="PNR"></param>
        /// <param name="Passenger"></param>
        /// <param name="statue"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponeAirChange> GetAirChangeList(DateTime? startDate, DateTime? endDate, string startTime, string endTime, string PNR, string Passenger, bool? status, int page, int rows, int i = -1, string CarrayNmae = null);

        /// <summary>
        /// 查询商户信息
        /// </summary>
        /// <param name="Code"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseBusinessMan GetBuseInfo(string Code);
        /// <summary>
        /// QT信息
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseAirQtInfo GetQtInfo(int Id);
        /// <summary>
        /// PNR信息
        /// </summary>
        /// <param name="pnr"></param>
        /// <returns></returns>
        [OperationContract]
        ResponeAirPnrInfo GetPnrInfo(string pnr, int Id);
        /// <summary>
        /// 新建航变协调
        /// </summary>
        /// <param name="type"></param>
        /// <param name="status"></param>
        /// <param name="content"></param>
        /// <param name="Id"></param>
        [OperationContract]
        void CreateAirChangeCoordion(EnumAriChangNotifications type, bool status, string content, int Id);
        /// <summary>
        /// 航变协调操作详情
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [OperationContract]
        ResponseOperateDetail GetOperateDetail(int Id);
        [OperationContract]
        List<AirChangeCoordionDto> GetAirChangeCoordion(int Id);

        /// <summary>
        /// 最近月份出票量
        /// </summary>
        /// <param name="code"></param>
        /// <param name="months"></param>
        /// <param name="ticketStatus"></param>
        /// <returns></returns>
        [OperationContract]
        int FindTicketCountByMonth(string code, int months, EnumTicketStatus? ticketStatus = EnumTicketStatus.IssueTicket);
        /// <summary>
        /// 查询订单处理理由
        /// </summary>
        /// <param name="Refuse"></param>
        /// <param name="page"></param>
        /// <param name="rows"></param>
        /// <returns></returns>
        PagedList<ResponseRefund> RefundReasonList(EnumRefuse? Refuse, int page, int rows);
        /// <summary>
        /// 新建订单处理信息
        /// </summary>
        /// <param name="info"></param>
        void CreateRefundReason(RequestRefund info);
        /// <summary>
        /// 查询订单处理信息jin 
        /// </summary>
        /// <param name="Id"></param>
        void DeleteRefundReason(int Id);
        /// <summary>
        /// 修改订单处理信息
        /// </summary>
        /// <param name="info"></param>
        void ModifyRefundReason(RequestRefund info);
     
    }
}
