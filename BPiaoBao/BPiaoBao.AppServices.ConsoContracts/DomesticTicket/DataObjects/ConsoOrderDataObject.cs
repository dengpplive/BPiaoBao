using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    /// <summary>
    /// 支付信息
    /// </summary>
    public class OrderPayDataObject
    {
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string PaySerialNumber { get; set; }
        /// <summary>
        /// 支付状态 0未支付 1已支付
        /// </summary>
        public EnumPayStatus PayStatus { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public EnumPayMethod? PayMethod { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayDateTime { get; set; }
    }
    /// <summary>
    /// 航段信息
    /// </summary>
    public class SkyDataObject
    {
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string FromCityCode { get; set; }
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string ToCityCode { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime ToDateTime { get; set; }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode { get; set; }
    }
    public class PassengerDataObject
    {
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName { get; set; }
    }
    /// <summary>
    /// 售后乘机人
    /// </summary>
    public class AfterPassengerDataObject
    {
        public string PassengerName { get; set; }
        public decimal RetirementPoundage { get; set; }
    }

    public class ResponseConsoOrder
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderMoney { get; set; }
        /// <summary>
        /// Pnr编码
        /// </summary>
        public string PnrCode { get; set; }
        /// <summary>
        /// 是否换编码
        /// </summary>
        public bool IsChangePnrTicket { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string BusinessmanCode { get; set; }
        /// <summary>
        /// 政策信息
        /// </summary>
        public PolicyDataObject Policy { get; set; }

        /// <summary>
        /// 锁定账号
        /// </summary>
        public string LockAccount { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public EnumOrderStatus OrderStatus { get; set; }
        /// <summary>
        /// 订单创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 支付信息
        /// </summary>
        public OrderPayDataObject OrderPay { get; set; }
        /// <summary>
        /// 航段
        /// </summary>
        public List<SkyDataObject> SkyWays { get; set; }
        /// <summary>
        /// 乘客
        /// </summary>
        public List<PassengerDataObject> Passengers { get; set; }
        /// <summary>
        /// 协调状态
        /// </summary>
        public bool? CoordinationStatus { get; set; }
        /// <summary>
        /// 是否是自己出票的票
        /// </summary>
        public bool IsSelfIssueTicket { get; set; }
        /// <summary>
        /// 是否有退废改信息
        /// </summary>
        public bool HasAfterSale { get; set; }
        /// <summary>
        ///  订单类型
        /// </summary>
        public int OrderType { get; set; }
        /// <summary>
        /// 订单来源
        /// </summary>
        public EnumOrderSource OrderSource { get; set; }
    }

}
