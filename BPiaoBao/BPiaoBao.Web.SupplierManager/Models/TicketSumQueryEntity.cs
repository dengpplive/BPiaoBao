using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class TicketSumQueryEntity
    {
        public string OrderId { get; set; }
        public string OutOrderId { get; set; }
        public string PNR { get; set; }
        public string TicketNumber { get; set; }
        public string PlatformCode { get; set; }
        public string PolicyType { get; set; }
        public string CarrayCode { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public int? TicketStatus { get; set; }
        public string BusinessmanName { get; set; }
        public string BusinessmanCode { get; set; }
        public string OperatorAccount { get; set; }
        public DateTime? StartIssueRefundTime { get; set; }
        public DateTime? EndIssueRefundTime { get; set; }
        public int? PayWay { get; set; }
        public int page { get; set; }
        public int rows { get; set; }
    }
}