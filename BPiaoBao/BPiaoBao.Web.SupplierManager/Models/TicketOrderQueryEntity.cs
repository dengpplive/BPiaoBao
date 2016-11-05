using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class TicketOrderQueryEntity
    {
        public string OrderId { get; set; }
        public string PNR { get; set; }
        public string PassengerName { get; set; }
        public string TicketNumber { get; set; }
        public string FromCity { get; set; }
        public string ToCity { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? ToDateTime { get; set; }
        public string BusinessmanCode { get; set; }
        public DateTime? StartCreateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public int? ProcessStatus { get; set; }
        public int? OrderType { get; set; }
        //航空公司
        public string CarrayCode { get; set; }
        //航班号
        public string FlightNumber { get; set; }
        //政策来源
        public string PlatformCode { get; set; }
        public int? OrderStatus { get; set; }
        //交易号
        public string PaySerialNumber { get; set; }
        //查询需要的参数 _ 从第几条开始查
        public int StartIndex_Q { get; set; }
        //查询的条数
        public int Count { get; set; }

       
        public int page { get; set; }
        public int rows { get; set; }
    }
}