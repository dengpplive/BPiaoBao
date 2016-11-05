using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TicketQueryEntity
    {
        public string orderId { get; set; }
        public string outOrderId { get; set; }
        public string pnr { get; set; }
        public string ticketNumber { get; set; }
        public string platformCode { get; set; }
        public string policyType { get; set; }
        public string carrayCode { get; set; }
        public string fromCityCode { get; set; }
        public string toCityCode { get; set; }
        public int? ticketStatus { get; set; }
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
        public int startIndex { get; set; }
        public int count { get; set; }
        public int? PayWay { get; set; }
    }
}
