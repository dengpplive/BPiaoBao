using BPiaoBao.Common.Enums;
using JoveZhao.Framework.Expand;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 订单详情
    /// </summary>
    public class ResponseOrderDetail
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public decimal OrderMoney { get; set; }

        /// <summary>
        /// PNR编码
        /// </summary>
        public string PnrCode { get; set; }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int? OrderStatus { get; set; }
        /// <summary>
        /// 政策
        /// </summary>
        public ResponsePolicyDetail Policy { get; set; }
        /// <summary>
        /// 支付信息
        /// </summary>
        public ResponsePayInfoDetail PayInfo { get; set; }
        /// <summary>
        /// 航程信息
        /// </summary>
        public List<ResponseSkyWayDetail> SkyWays { get; set; }
        /// <summary>
        /// 乘机人列表
        /// </summary>
        public List<ResponsePassengerDetail> Passengers { get; set; }
    }
    /// <summary>
    /// 政策信息
    /// </summary>
    public class ResponsePolicyDetail
    {
        /// <summary>
        /// 政策点数
        /// </summary>
        public decimal? Point { get; set; }
        /// <summary>
        /// 退改签处理
        /// </summary>
        public string TFGTime
        {
            get
            {
                return BPiaoBao.Common.ExtHelper.FormatTime(this.ReturnTicketTime, this.AnnumTicketTime);
            }
        }
        /// <summary>
        /// 退票时间
        /// </summary>
        public string ReturnTicketTime { get; set; }
        /// <summary>
        /// 改签时间
        /// </summary>
        public string AnnumTicketTime { get; set; }

        /// <summary>
        /// 政策备注
        /// </summary>
        public string Remark { get; set; }
    }
    /// <summary>
    /// 支付信息
    /// </summary>
    public class ResponsePayInfoDetail
    {
        /// <summary>
        /// 支付流水
        /// </summary>
        public string PaidSerialNumber { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public EnumPayMethod? PayMethod { get; set; }
        public string PayMethodName
        {
            get
            {
                return PayMethod.HasValue ? PayMethod.ToEnumDesc() : string.Empty;
            }
        }
        /// <summary>
        /// 支付状态
        /// </summary>
        public EnumPayStatus? PayStatus { get; set; }
        public string PayStatusName {
            get {
                return PayStatus.HasValue ? PayStatus.ToEnumDesc() : string.Empty;
            }
        }
    }
    /// <summary>
    /// 行程信息
    /// </summary>
    public class ResponseSkyWayDetail
    {
        /// <summary>
        /// 起飞城市
        /// </summary>
        public string FromCity
        {
            get
            {
                return BPiaoBao.Common.ExtHelper.GetCityNameByCode(this.FromCityCode);
            }
        }
        public string FromCityCode { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCity
        {
            get
            {
                return BPiaoBao.Common.ExtHelper.GetCityNameByCode(this.ToCityCode);
            }
        }
        public string ToCityCode { get; set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime ToDateTime { get; set; }
        /// <summary>
        /// 航空公司
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
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 机型
        /// </summary>
        public string Model { get; set; }

    }
    /// <summary>
    /// 乘客信息
    /// </summary>
    public class ResponsePassengerDetail
    {
        /// <summary>
        /// 乘机人
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// 乘机人类型
        /// </summary>
        public EnumPassengerType? PassengerType { get; set; }
        public string PassengerTypeName
        {
            get
            {
                return PassengerType.HasValue ? PassengerType.ToEnumDesc() : string.Empty;
            }
        }
        /// <summary>
        /// 证件类型
        /// </summary>
        public string CardType { get; set; }
        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber { get; set; }
        /// <summary>
        /// 类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }
        /// <summary>
        /// 基建
        /// </summary>
        public decimal TaxFee { get; set; }
        /// <summary>
        /// 燃油
        /// </summary>
        public decimal RQFee { get; set; }
        /// <summary>
        /// 退费手续费
        /// </summary>
        public decimal RetirementPoundage { get; set; }
    }
    /// <summary>
    /// 日志信息
    /// </summary>
    public class ResponseLogDetail
    {
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperationDatetime { get; set; }
        /// <summary>
        /// 操作人
        /// </summary>
        public string OperationPerson { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string OperationContent { get; set; }
    }
}
