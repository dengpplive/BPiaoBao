using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class ReponseTicketSum
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
        /// 采购商
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMethod { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string PayNumber { get; set; }

        /// <summary>
        /// 政策点数
        /// </summary>
        public decimal PolicyPoint { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderMoney { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission
        {
            get
            {
                
                return Math.Floor(this.PolicyPoint / 100 * this.SeatPrice);

            }
        }
        /// <summary>
        /// 售后订单号
        /// </summary>
        public string AfterSaleOrderID { get; set; }
    }
}
