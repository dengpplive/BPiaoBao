using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    /// <summary>
    /// 机票总表
    /// </summary>
    public abstract class Ticket : EntityBase, IAggregationRoot
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
        /// 交易号
        /// </summary>
        public string PayNumber { get; set; }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }
    /// <summary>
    /// 【机票总表】运营商
    /// </summary>
    public class Ticket_Carrier : Ticket
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 运行商Code
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public string PolicyFrom { get; set; }
        /// <summary>
        /// 出票Code
        /// </summary>
        public string IssueTicketCode { get; set; }
        /// <summary>
        /// 政策点数
        /// </summary>
        public decimal PolicyPoint { get; set; }
        /// <summary>
        /// 扣点
        /// </summary>
        public decimal Point { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal Money { get; set; }

    }
    /// <summary>
    /// 【机票总表】供应商
    /// </summary>
    public class Ticket_Supplier : Ticket
    {
        /// <summary>
        /// 出票CODE
        /// </summary>
        public string IssueTicketCode { get; set; }
        /// <summary>
        /// 政策点数
        /// </summary>
        public decimal PolicyPoint { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal Money { get; set; }
    }
    /// <summary>
    /// 【机票总表】采购商
    /// </summary>
    public class Ticket_Buyer : Ticket
    {
        /// <summary>
        /// 采购商
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMethod { get; set; }
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
        public decimal CommissionMoney { get; set; }
    }
    /// <summary>
    /// 【机票总表】控制台
    /// </summary>
    public class Ticket_Conso : Ticket
    {
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
        /// 订单金额
        /// </summary>
        public decimal OrderMoney { get; set; }
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
    }
    /// <summary>
    /// 机票统计总表
    /// </summary>
    public class TicketSum : EntityBase, IAggregationRoot
    {
        public int ID { get; set; }
        /// <summary>
        /// 出票Code
        /// </summary>
        public string IssueTicketCode { get; set; }
        /// <summary>
        /// 运行商Code
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 原订单号
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string TransactionNumber { get; set; }
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
        /// 返点
        /// </summary>
        public decimal PolicyPoint { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderMoney { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMethod { get; set; }
        /// <summary>
        /// 行程程单号
        /// </summary>
        public string TrvalNum { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public string PolicyFrom { get; set; }
        /// <summary>
        /// 代付订单号
        /// </summary>
        public string OutOrderID { get; set; }
        /// <summary>
        /// 大编码
        /// </summary>
        public string BigCode { get; set; }
        /// <summary>
        /// 代付金额
        /// </summary>
        public decimal PaidMoney { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarryCode { get; set; }
        /// <summary>
        /// 票面价
        /// </summary>
        public decimal PMFee { get; set; }
        /// <summary>
        /// 原政策
        /// </summary>
        public decimal OldPolicy { get; set; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public string PolicyType { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public string PolicySourceType { get; set; }
        /// <summary>
        /// 退废改手续费
        /// </summary>
        public decimal RetirementPoundage { get; set; }
        /// <summary>
        /// 机票收支明细
        /// </summary>
        public virtual ICollection<TicketDetail> TicketDetails { get; set; }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }

    }
    public class TicketDetail
    {
        public int ID { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 实收金额
        /// </summary>
        public decimal Money { get; set; }
    }

}
