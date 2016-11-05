using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    public class ConsoDetailOrder
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
        /// 支付信息
        /// </summary>
        public OrderPayDataObject OrderPay { get; set; }
    }
    /// <summary>
    /// 售后乘机人
    /// </summary>
    public class ConsoAfterPassenger
    {
        /// <summary>
        /// 售后ID编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 乘机人ID
        /// </summary>
        public int PassengerId { get; set; }
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// 乘客类型
        /// </summary>
        public string PassengerType { get; set; }
        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber { get; set; }
        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }
        /// <summary>
        /// 基建
        /// </summary>
        public decimal ABFee { get; set; }
        /// <summary>
        /// 燃油
        /// </summary>
        public decimal RQFee { get; set; }
        /// <summary>
        /// 退票手续费
        /// </summary>
        public decimal RetirementPoundage { get; set; }
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TravelNumber { get; set; }
    }
    public class ConsoAfterSkyWay
    {
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat { get; set; }
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string FromCityCode { get; set; }
        /// <summary>
        /// 出发城市名称
        /// </summary>
        public string FromCityCodeName { get; set; }
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string ToCityCode { get; set; }
        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCityCodeName { get; set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime ToDateTime { get; set; }
    }
    public class ConsoDetailSaleOrder
    {
        /// <summary>
        /// 售后单号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 售后类型
        /// </summary>
        public string AfterSaleType { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 申请备注
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 申请人
        /// </summary>
        public string CreateMan { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 产生金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 处理人
        /// </summary>
        public string ProcessName { get; set; }
        /// <summary>
        /// 处理时间
        /// </summary>
        public DateTime? ProcessDate { get; set; }
        /// <summary>
        /// 处理备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 关联订单
        /// </summary>
        public ConsoOrderDataObject Order { get; set; }
        /// <summary>
        /// 锁定帐号
        /// </summary>
        public string LockCurrentAccount { get; set; }
    }
}
