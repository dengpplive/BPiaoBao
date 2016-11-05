using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TicketSumDetailDto
    {
        public int ID { get; set; }
        /// <summary>
        /// 原订单号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 本次交易订单号(出票时表示订单号与原订单号相同)/售后订单号(退,废时表示)
        /// </summary>
        public string CurrentOrderID { get; set; }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNum { get; set; }
        /// <summary>
        /// 机票状态
        /// </summary>
        public string TicketState { get; set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNum { get; set; }
        /// <summary>
        /// 航程
        /// </summary>
        public string Voyage { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat { get; set; }
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// PNR
        /// </summary>
        public string PNR { get; set; }
        /// <summary>
        /// 大编码
        /// </summary>
        public string BigCode { get; set; }
        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal ABFee { get; set; }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal RQFee { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarryCode { get; set; }
        /// <summary>
        /// 票面价
        /// </summary>
        public decimal PMFee { get; set; }
        /// <summary>
        /// 退改签手续费
        /// </summary>
        public decimal RetirementPoundage { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public string PolicyFrom { get; set; }
        /// <summary>
        /// 出票Code
        /// </summary>
        public string IssueTicketCode { get; set; }
        /// <summary>
        /// 运行商Code
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 代付金额
        /// </summary>
        public decimal PaidMoney { get; set; }
        /// <summary>
        /// 代付方式
        /// </summary>
        public string PaidMethod { get; set; }
        /// <summary>
        /// 代付点数
        /// </summary>
        public decimal PaidPoint { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string Paymethod { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 支付手续费
        /// </summary>
        public decimal PayFee { get; set; }
        /// <summary>
        /// 交易手续费
        /// </summary>
        public decimal TransactionFee { get; set; }
        /// <summary>
        /// 收益
        /// </summary>
        public decimal InCome { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderMoney { get; set; }
    }
}
