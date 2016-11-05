using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 售后申请
    /// </summary>
    [KnownType(typeof(ResponseAnnulOrder))]
    [KnownType(typeof(ResponseBounceOrder))]
    [KnownType(typeof(ResponseChangeOrder))]
    [KnownType(typeof(ResponseModifyOrder))]
    public abstract class ResponseAfterSaleOrder
    {
        /// <summary>
        /// 售后订单号
        /// </summary>
        public int Id { get; set; }
        public string AfterSaleType { get; set; }
        /// <summary>
        /// 处理状态
        /// </summary>
        public EnumTfgProcessStatus ProcessStatus { get; set; }
        /// <summary>
        /// 乘机人
        /// </summary>
        public List<ResponseAfterSalePassenger> Passenger { get; set; }
        /// <summary>
        /// 日志
        /// </summary>
        public List<OrderLogDto> Logs { get; set; }
        /// <summary>
        /// 航段信息
        /// </summary>
        public List<SkyWayDto> SkyWays { get; set; }
        public OrderPayDto OrderPay { get; set; }
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
        /// 订单ID
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 锁定帐号
        /// </summary>
        public string LockCurrentAccount { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public string PolicyFrom { get; set; }
        /// <summary>
        /// 政策来源类型
        /// </summary>
        public int PolicySourceType { get; set; }
        /// <summary>
        /// 出票日期
        /// </summary>
        public DateTime? TicketDate { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// PNR编码
        /// </summary>
        public string PNR { get; set; }
        public string PNRContent { get; set; }
        /// <summary>
        /// 是否有售后信息
        /// </summary>
        public bool? HasAfterSale { get; set; }
        /// <summary>
        /// 退废改订单金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 供应出票金额
        /// </summary>
        public decimal XCPMoney { get; set; }
        /// <summary>
        /// 供应产生金额
        /// </summary>
        public decimal SMoney { get; set; }

        public bool? IsCoorCompleted { get; set; }
        public string YdOffice { get; set; }

        public string CurrentCode { get; set; }
        public string Order_CarrierCode { get; set; }
        public string Issue_CarrierCode { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string PayNum { get; set; }
        //private string _payNum;
        //public string PayNum
        //{
        //    get
        //    {
        //        switch (AfterSaleType)
        //        {
        //            case "改签":
        //                var corder = this as ResponseChangeOrder;
        //                return corder != null ? corder.OutPayNo : string.Empty;
        //            case "退票":
        //                var border = this as ResponseBounceOrder;
        //                var line = new ResponseBounceLine();
        //                if (border != null) line = border.BounceLines.FirstOrDefault();
        //                return line != null ? line.PaySerialNumber : string.Empty;
        //            default:
        //                return string.Empty;
        //        }
        //    }
        //    set { _payNum = value; }
        //}

        public bool IsRefund
        {
            get
            {
                bool result = false;
                if (this.Passenger.All(p => p.IsInsuranceRefund))
                {
                    try
                    {
                        //如果是废票，则为true
                        //如果是退票，则自愿的为true
                        if (this is ResponseAnnulOrder
                            || ((this is ResponseBounceOrder) && (this is ResponseBounceOrder) && ((ResponseBounceOrder)this).IsVoluntary)
                            )
                        {
                            result = true;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("IsRefund Exception:{0}", e.Message);
                    }
                }

                return result;
            }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        private string _payMethod = string.Empty;
        public string PayMethod
        {
            get
            {
                switch (AfterSaleType)
                {
                    case "改签":
                        var order = this as ResponseChangeOrder;
                        return order != null ? EnumCommonHelper.GetDescription(order.PayWay) : OrderPay.PayMethod;

                    default:
                        return OrderPay.PayMethod;
                }
            }
            set { _payMethod = value; }
        }

        /// <summary>
        /// 退票说明自愿与非自愿
        /// </summary>
        private string _voluntary = string.Empty;
        public string Voluntary
        {
            get
            {
                switch (AfterSaleType)
                {
                    case "退票":
                        var order = this as ResponseBounceOrder;
                        if (order != null)
                        {
                            return order.IsVoluntary ? "自愿" : "非自愿";
                        }
                        return "非自愿";
                }
                return _voluntary;
            }
            set { _voluntary = value; }
        }
        /// <summary>
        /// 当前时间
        /// </summary>
        public DateTime NowDateTime
        {
            get { return DateTime.Now; }
        }
        ///// <summary>
        ///// 是否显示发送短信按钮
        ///// </summary>
        //public bool CanSendSMS
        //{
        //    get { return !AfterSaleType.Equals("其它修改") && ProcessStatus == EnumTfgProcessStatus.Processed; }
        //}
    }
    /// <summary>
    /// 废票
    /// </summary>
    public class ResponseAnnulOrder : ResponseAfterSaleOrder
    {
        /// <summary>
        /// 附件地址
        /// </summary>
        public string AttachmentUrl { get; set; }
        /// <summary>
        /// 退款明细
        /// </summary>
        public List<ResponseBounceLine> BounceLines { get; set; }
    }
    /// <summary>
    /// 退票
    /// </summary>
    public class ResponseBounceOrder : ResponseAfterSaleOrder
    {
        /// <summary>
        /// 是否自愿 true为自愿，默认非自愿
        /// </summary>
        public bool IsVoluntary { get; set; }
        /// <summary>
        /// 附件地址
        /// </summary>
        public string AttachmentUrl { get; set; }
        /// <summary>
        /// 退款明细
        /// </summary>
        public List<ResponseBounceLine> BounceLines { get; set; }
    }
    /// <summary>
    /// 改签
    /// </summary>
    public class ResponseChangeOrder : ResponseAfterSaleOrder
    {
        /// <summary>
        /// 支付方式
        /// </summary>
        public EnumPayMethod? PayWay { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 外部支付流水号
        /// </summary>
        public string OutPayNo { get; set; }
        /// <summary>
        /// 航班信息
        /// </summary>
        public IList<ResponseAfterSaleSkyWay> SkyWay { get; set; }
    }

    public class ResponseModifyOrder : ResponseAfterSaleOrder
    {

    }
    /// <summary>
    /// 改签航段信息
    /// </summary>
    public class ResponseAfterSaleSkyWay
    {
        public int Id { get; set; }
        /// <summary>
        /// 原航班编号
        /// </summary>
        public int SkyWayId { get; set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime NewStartDateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime NewToDateTime { get; set; }
        /// <summary>
        /// 
        /// 航班号
        /// </summary>
        public string NewFlightNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位
        /// </summary>
        public string NewSeat
        {
            get;
            set;
        }


    }
    /// <summary>
    /// 退改签乘机人信息
    /// </summary>
    public class ResponseAfterSalePassenger
    {
        public int AfterPassengerID { get; set; }
        public int Id { get; set; }

        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName
        {
            get;
            set;
        }
        /// <summary>
        /// 乘客类型 1成人 2儿童 3婴儿
        /// </summary>
        public EnumPassengerType PassengerType
        {
            get;
            set;
        }
        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo
        {
            get;
            set;
        }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal ABFee
        {
            get;
            set;
        }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal RQFee
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 状态(机票)
        /// </summary>
        public EnumTicketStatus TicketStatus
        {
            get;
            set;
        }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile
        {
            get;
            set;
        }
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 行程单号
        /// </summary>
        public string TravelNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 行程单状态
        /// </summary>
        public EnumPassengerTripStatus PassengerTripStatus
        {
            get;
            set;
        }
        /// <summary>
        /// 售后乘机人状态
        /// </summary>
        public EnumTfgPassengerStatus Status { get; set; }
        /// <summary>
        /// 退改签手续费
        /// </summary>
        public decimal RetirementPoundage
        {
            get;
            set;
        }
        /// <summary>
        /// 是否够买了保险急速退
        /// </summary>
        public bool IsInsuranceRefund { get; set; }

        /// <summary>
        /// 改签后的行程单号
        /// </summary>
        public string AfterSaleTravelNum { get; set; }

        /// <summary>
        /// 改签后的票号
        /// </summary>
        public string AfterSaleTravelTicketNum
        {
            get;
            set;
        }

    }
    public class ResponseBounceLine
    {
        public string ID { get; set; }
        /// <summary>
        /// 乘机人名称
        /// </summary>
        public string PassgenerName { get; set; }
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string PaySerialNumber { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundMoney { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime? RefundTime { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public string Status { get; set; }

    }

}
