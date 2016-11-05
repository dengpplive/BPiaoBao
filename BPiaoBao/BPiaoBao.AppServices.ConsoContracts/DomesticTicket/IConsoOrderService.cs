using BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects;
using BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket
{
    [ServiceContract]
    public interface IConsoOrderService
    {
        /// <summary>
        /// 订单综合查询
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="pnr"></param>
        /// <param name="passengerName"></param>
        /// <param name="ticketNum"></param>
        /// <param name="businessmanCode"></param>
        /// <param name="paySerialNumber"></param>
        /// <param name="carrayCode"></param>
        /// <param name="orderstatus"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseConsoOrder> FindConsoAllOrder(AllOrderSearch allOrderSearch,int Page,int Rows);
        /// <summary>
        /// 售后订单综合查询
        /// </summary>
        /// <param name="orderid"></param>
        /// <param name="pnr"></param>
        /// <param name="businessmanCode"></param>
        /// <param name="processStatus"></param>
        /// <param name="paySerialNumber"></param>
        /// <param name="passengerName"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseConsoSaleOrder> FindConsoAllSaleOrder(AllSaleOrderSearch allSaleOrderSearch);
        /// <summary>
        /// 待处理订单查询
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="pnr">pnr</param>
        /// <param name="passengerName">乘机人</param>
        /// <param name="code">商户号</param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseConsoOrder> FindOrder(string orderid, string pnr, string passengerName, string code, int currentIndex, int pageSize);
        /// <summary>
        /// 待处理售后订单查询
        /// </summary>
        /// <param name="orderid">订单号</param>
        /// <param name="pnr">pnr</param>
        /// <param name="passengerName">乘机人</param>
        /// <param name="code">商户号</param>
        /// <param name="payNum">交易号</param>
        /// <param name="startDate">查询时间-开始</param>
        /// <param name="endDate">查询时间-结束</param>
        /// <param name="PolicyType">政策类型</param>
        /// <param name="AfterSaleType">申请类型</param>
        /// <param name="currentIndex">当前页</param>
        /// <param name="pageSize">分页大小</param>
        /// <param name="lockAccount">锁定当前账户</param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseConsoSaleOrder> FindSaleOrder(string orderid, string pnr, string passengerName, string code, string payNum, EnumTfgProcessStatus? status,DateTime? startDate, DateTime? endDate, string PolicyType, string AfterSaleType,string lockAccount, int currentIndex, int pageSize);
        /// <summary>
        /// 添加订单协调
        /// </summary>
        /// <param name="orderCoordination"></param>
        [OperationContract]
        void AddOrderCoordination(string orderid, bool IsCompleted, string Type, string Content);
        /// <summary>
        /// 获取订单协调
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ConsoOrderCoordination> GetOrderCoordination(string orderid);
        /// <summary>
        /// 添加售后订单协调
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <param name="IsCompleted"></param>
        /// <param name="Type"></param>
        /// <param name="Content"></param>
        [OperationContract]
        void AddSaleOrderCoordination(int saleorderid, bool IsCompleted, string Type, string Content);
        /// <summary>
        /// 获取售后订单协调
        /// </summary>
        /// <param name="saleorderid"></param>
        /// <returns></returns>
        [OperationContract]
        List<ConsoOrderCoordination> GetSaleOrderCoordination(int saleorderid);
        /// <summary>
        /// 机票总表明细
        /// </summary>
        /// <param name="ticketDetailSearch"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseTicket> GetConsoTicketSumDetail(TicketDetailSearch ticketDetailSearch);
        /// <summary>
        /// 采购出票量统计
        /// </summary>
        /// <param name="ticketCountOfBuyer"></param>
        /// <returns></returns>
        [OperationContract]
        PagedList<ResponseTicketCount> GetBuyerTicketCount(TicketCountOfBuyer ticketCountOfBuyer);
    }
}
