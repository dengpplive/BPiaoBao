using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    /// <summary>
    /// 订单综合查询
    /// </summary>
    public class AllOrderSearch
    { 
        public string OrderID { get; set; }
        public string PNR { get; set; }
        public string PassengerName { get; set; }
        public string TicketNum { get; set; }
        public string BusinessmanCode { get; set; }
        public string PaySerialNumber { get; set; }
        public string CarrayCode { get; set; }
        public EnumOrderStatus? OrderStatus { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public EnumPolicySourceType? PolicysSourceType { get; set; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public string PolicyType { get; set; }
    }
    public class AllSaleOrderSearch
    { 
         public string OrderID { get; set; }
        public string PNR { get; set; }
        public string PassengerName { get; set; }
        public string BusinessmanCode { get; set; }
        public string PaySerialNumber { get; set; }
        public EnumTfgProcessStatus? ProcessStatus { get; set; }
        public int Page { get; set; }
        public int Rows { get; set; }
    }
    public class TicketDetailSearch
    {
        public string orderId { get; set; }
        public string pnr { get; set; }
        public string ticketNumber { get; set; }
        public string platformCode { get; set; }
        public string policyType { get; set; }
        public string carrayCode { get; set; }
        public string fromCity { get; set; }
        public string toCity { get; set; }
        public string ticketStatus { get; set; }
        public string businessmanName { get; set; }
        public string businessmanCode { get; set; }
        public string carrierCode { get; set; }
        public string operatorAccount { get; set; }
        public DateTime? startPayTime { get; set; }
        public DateTime? endPayTime { get; set; }
        public DateTime? startIssueRefundTime { get; set; }
        public DateTime? endIssueRefundTime { get; set; }
        public DateTime? startCreateTime { get; set; }
        public DateTime? endCreateTime { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
        public int? PayWay { get; set; }
    }
    public class TicketCountOfBuyer
    {
        public string BusinessmanCode { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int Page { get; set; }
        public int Rows { get; set; }
    }
}
