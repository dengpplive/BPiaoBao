using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TicketDataInfoDto
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber { get; set; }
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TravelNumber { get; set; }
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// 航班起飞时间
        /// </summary>
        public string FlyDateTime { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }
        /// <summary>
        /// 行程 成都-北京
        /// </summary>
        public string Travel { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat { get; set; }
        /// <summary>
        /// 小编码
        /// </summary>
        public string Pnr { get; set; }
        /// <summary>
        /// 机票操作状态
        /// </summary>
        public string TicketStatus { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        //public string OrderStatus { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMethod { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderMoney { get; set; }
        /// <summary>
        /// 单人舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }
        /// <summary>
        /// 单人机建费
        /// </summary>
        public decimal TaxFee { get; set; }
        /// <summary>
        /// 单人燃油费
        /// </summary>
        public decimal RQFee { get; set; }
        /// <summary>
        /// 税费
        /// </summary>
        //public decimal Taxation { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        public decimal ReturnPoint { get; set; }
        /// <summary>
        /// 退非改费用
        /// </summary>
        public decimal RetirementPoundage { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        //public decimal Discount { get; set; }
        /// <summary>
        /// 大编码
        /// </summary>
        public string BigCode { get; set; }

        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission
        {
            get;
            set;
        }

        /// <summary>
        /// 订单出票时间
        /// </summary>
        public DateTime? IssueTicketTime { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayDateTime
        {
            get;
            set;
        }
    }
}
