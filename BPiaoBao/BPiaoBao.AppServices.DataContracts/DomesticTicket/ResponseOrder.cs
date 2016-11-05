using BPiaoBao.Common;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 订单列表显示对象
    /// </summary>
    public class ResponseOrder
    {
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// PNR编码
        /// </summary>
        public string PnrCode { get; set; }
        private string _PnrCode = string.Empty; //PNR显示隐藏功能实现处理
        /// <summary>
        /// 显示PNR编码
        /// </summary>
        public string ShowPnrCode 
        {

            get
            {
                //if (OrderSource == EnumOrderSource.ChdPnrImport || OrderSource == EnumOrderSource.PnrImport || OrderSource == EnumOrderSource.PnrContentImport || OrderSource == EnumOrderSource.LineOrder) return _PnrCode;
                //if (OrderStatus == 3 || OrderStatus == 5) return _PnrCode;
                //return "******";
                if (OrderStatus == 5) return _PnrCode;
                return OrderSource == EnumOrderSource.WhiteScreenDestine || OrderSource == EnumOrderSource.MobileDestine ? "******" : _PnrCode;
            }
            set{ _PnrCode = PnrCode; } 
        }
        /// <summary>
        /// 新PNR编码
        /// </summary>
        public string NewPnrCode { get; set; }       
        private string _NewPnrCode = string.Empty;
        /// <summary>
        /// 显示新PNR编码
        /// </summary>
        public string ShowNewPnrCode 
        {
            get
            {
                if (!string.IsNullOrEmpty(_NewPnrCode))
                    return "新编码：" + _NewPnrCode;
                else
                    return _NewPnrCode;
            }
            set { _NewPnrCode = NewPnrCode; }
        }
        /// <summary>
        /// 订单状态
        /// </summary>
        public int? OrderStatus { get; set; }
        public EnumOrderStatus? ClientOrderStatus
        {
            get;
            set;
        }
        public string orderStatusName
        {
            get
            {
                return ClientOrderStatus.HasValue ? ClientOrderStatus.ToEnumDesc() : string.Empty;
            }
        }
        /// <summary>
        /// 政策信息
        /// </summary>
        public ResponsePolicy Policy { get; set; }
        /// <summary>
        /// 支付金额
        /// </summary>
        public decimal OrderMoney { get; set; }
        /// <summary>
        /// 是否有售后信息
        /// </summary>
        public bool HasAfterSale { get; set; }
        /// <summary>
        /// 行程
        /// </summary>
        public List<ResponseSkyWay> SkyWays { get; set; }
        /// <summary>
        /// 乘机人列表
        /// </summary>
        public List<ResponsePassenger> Passengers { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo { get; set; }
        /// <summary>
        /// 保险总金额
        /// </summary>
        public decimal? InsuranceMoney { get; set; }
        private string _ShowPassengerType = string.Empty;
        /// </summary>
        /// 订单类型标识 儿童 婴儿
        /// </summary>
        public string PassengerType
        {
            get 
            {
                if (Passengers.Count(p => p.PassengerType == EnumPassengerType.Child) > 0)
                    return "儿童";
                else if (Passengers.Count(p => p.PassengerType == EnumPassengerType.Baby) > 0)
                    return "婴儿";
                else
                    return string.Empty;
            }
            set { _ShowPassengerType = value; }
        }

        /// <summary>
        /// Pnr来源
        /// </summary>
        public EnumPnrSource PnrSource { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public EnumOrderSource OrderSource { get; set; }

        /// <summary>
        /// 票面总价
        /// </summary>
        public decimal? TicketSumPrice 
        {
            get
            {
                decimal TicketPrice = 0;
                Passengers.ToList().ForEach(p => { TicketPrice += p.SeatPrice + p.TaxFee + p.RQFee; });
                return TicketPrice;
            }
        }
        /// <summary>
        /// 非白屏预定（PNR内容导入）
        /// </summary>
        public bool NotFromWhiteScreenDestine
        {
            get
            {
                return OrderSource == EnumOrderSource.PnrContentImport;
            }
        }

        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayMethodCode { get; set; }

    }
    /// <summary>
    ///  政策
    /// </summary>
    public class ResponsePolicy
    {
        /// <summary>
        /// 返点
        /// </summary>
        public decimal? Point { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal? Commission { get; set; }
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType? PolicySpecialType { get; set; }
    }
    public class ResponseSkyWay
    {
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string CarrayShortName
        {
            get
            {
                return ExtHelper.GetAirNameByCode(CarrayCode);
            }
        }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }
        /// <summary>
        /// 出发城市名称简码
        /// </summary>
        public string FromCityCode { get; set; }
        /// <summary>
        /// 出发城市名称
        /// </summary>
        public string FromCity
        {
            get
            {
                return ExtHelper.GetCityNameByCode(FromCityCode);
            }
        }
        /// <summary>
        /// 到达城市名称简码
        /// </summary>
        public string ToCityCode { get; set; }
        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCity
        {
            get
            {
                return ExtHelper.GetCityNameByCode(ToCityCode);
            }
        }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat { get; set; }

    }
    public class ResponsePassenger
    {
        /// <summary>
        /// 乘机人姓名
        /// </summary>
        public string PassengerName { get; set; }
        /// <summary>
        /// 乘客类型 1成人 2儿童 3婴儿
        /// </summary>
        public EnumPassengerType PassengerType
        {
            get;
            set;
        }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal TaxFee
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
    }
}
