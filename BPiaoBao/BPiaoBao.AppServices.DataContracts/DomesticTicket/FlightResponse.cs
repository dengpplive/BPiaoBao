using System.Runtime.Serialization;
using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Reflection;
using BPiaoBao.Common;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class FlightResponse
    {
        public List<FlightLineResponse> List { get; set; }
    }
    public class FlightLineResponse
    {
        public FlightSkyWayResponse SkyWay { get; set; }
        public SeatResponse[] SeatList { get; set; }
    }
    /// <summary>
    /// 
    /// </summary>
    public class FlightSkyWayResponse
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
        /// 出发城市名称
        /// </summary>
        public string FromCity { get; set; }
        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCity { get; set; }
        /// <summary>
        /// 出发城市机场名称
        /// </summary>
        public string FromAirPortrName { get; set; }
        /// <summary>
        /// 到达城市机场名称
        /// </summary>
        public string ToAirPortrName { get; set; }
        /// <summary>
        /// 出发航站楼
        /// </summary>
        public string FromTerminal { get; set; }
        /// <summary>
        /// 到达航站楼
        /// </summary>
        public string ToTerminal
        {
            get;
            set;
        }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        public DateTime ToDateTime { get; set; }
        /// <summary>
        /// 航空公司编码
        /// </summary>
        public string CarrayCode { get; set; }
        public string CarrayShortName { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 机型
        /// </summary>
        public string Model
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
        /// 是否经停
        /// </summary>
        public bool IsStop
        {
            get;
            set;
        }
        /// <summary>
        /// 是否共享
        /// </summary>
        public bool IsShareFlight
        {
            get;
            set;
        }
    }

    public class SeatResponse
    {
        /// <summary>
        /// 舱位
        /// </summary>
        public string SeatCode { get; set; }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Discount { get; set; }
        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }
        /// <summary>
        /// IBE原始舱位价不变
        /// </summary>
        public decimal IbeSeatPrice { get; set; }
        /// <summary>
        ///该航空公司的Y舱价格
        /// </summary>
        public decimal YPrice { get; set; }
        /// <summary>
        /// 政策点数
        /// </summary>
        public decimal PolicyPoint { get; set; }
        /// <summary>
        /// 剩余座位数
        /// </summary>
        public string SeatCount { get; set; }
        /// <summary>
        /// 票面价
        /// </summary>
        public decimal TicketPrice { get; set; }
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }
    }

    public class DestineResponse
    {
        public DestineResponse()
        {
            this.AdultPnrData = new PnrData();
            this.ChdPnrData = new PnrData();
        }
        /// <summary>
        /// 成人编码内容包含PAT
        /// </summary>
        public string AdultPnrContent { get; set; }
        /// <summary>
        /// 儿童编码内容 包含PAT
        /// </summary>
        public string ChdPnrContent { get; set; }
        /// <summary>
        /// 成人编码
        /// </summary>
        public string AdultPnr { get; set; }
        /// <summary>
        /// 儿童编码
        /// </summary>
        public string ChdPnr { get; set; }
        /// <summary>
        /// 是否显示换编码政策
        /// </summary>
        public bool IsChangePnr
        {
            get;
            set;
        }
        /// <summary>
        /// 编码中的婴儿人数和预定的婴儿人数是否一致
        /// </summary>
        public bool INFPnrIsSame = true;
        /// <summary>
        /// 成人的编码解析
        /// </summary>
        public PnrData AdultPnrData
        {
            get;
            set;
        }
        /// <summary>
        /// 儿童的编码解析
        /// </summary>
        public PnrData ChdPnrData
        {
            get;
            set;
        }
    }
    public class DestineRequest
    {
        public DestineSkyWayRequest[] SkyWay { get; set; }
        public PassengerRequest[] Passengers { get; set; }
        public string Tel { get; set; }
        /// <summary>
        /// 单独生成儿童编码时备注的成人PNR 这个从关联的订单号中获取成人编码
        /// </summary>
        public string ChdRemarkAdultPnr
        {
            get;
            set;
        }
        /// <summary>
        /// 儿童关联成人订单号
        /// </summary>
        public string OldOrderId
        {
            get;
            set;
        }
        /// <summary>
        /// 是否显示换编码政策
        /// </summary>
        public bool IsChangePnr
        {
            get;
            set;
        }
        /// <summary>
        /// 多个价格(高低价格) true低价格(默认) false高价格
        /// </summary>
        public bool IsLowPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }
        /// <summary>
        /// 特价舱位价
        /// </summary>
        public decimal SpecialPrice { get; set; }
        /// <summary>
        /// 特价机建
        /// </summary>
        public decimal SpecialTax { get; set; }
        /// <summary>
        /// 特价燃油
        /// </summary>
        public decimal SpecialFuelFee { get; set; }
        /// <summary>
        /// 特价Y舱的舱位价
        /// </summary>
        public decimal SpecialYPrice { get; set; }

        /// <summary>
        /// Ibe机建费
        /// </summary>
        public decimal IbeTaxFee
        {
            get;
            set;
        }
        /// <summary>
        /// Ibe燃油费
        /// </summary>
        public decimal IbeRQFee
        {
            get;
            set;
        }
        /// <summary>
        /// Ibe舱位价
        /// </summary>
        public decimal IbeSeatPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 字符串显示 用于记录日志
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            StringBuilder sbBuilder = new StringBuilder();
            sbBuilder.Append("\r\n Tel=" + Tel + "\r\n");
            if (SkyWay != null && SkyWay.Length > 0)
            {
                for (int i = 0; i < SkyWay.Length; i++)
                {
                    sbBuilder.Append("航段" + (i + 1) + ":\r\n");
                    DestineSkyWayRequest item = SkyWay[i];
                    PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                    object obj = null;
                    foreach (PropertyInfo p in properties)
                    {
                        obj = p.GetValue(item, null);
                        sbBuilder.Append(p.Name + "=" + (obj == null ? "null" : obj) + "\r\n");
                    }
                }
            }
            if (Passengers != null && Passengers.Length > 0)
            {
                for (int i = 0; i < Passengers.Length; i++)
                {
                    sbBuilder.Append("乘客" + (i + 1) + ":\r\n");
                    PassengerRequest item = Passengers[i];
                    PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                    object obj = null;
                    foreach (PropertyInfo p in properties)
                    {
                        obj = p.GetValue(item, null);
                        sbBuilder.Append(p.Name + "=" + (obj == null ? "null" : obj) + "\r\n");
                    }
                }
            }
            return sbBuilder.ToString();
        }
    }
    public class DestineSkyWayRequest
    {
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 起飞日期和时间
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 到达时间
        /// </summary> 
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat { get; set; }
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string FromCityCode { get; set; }
        /// <summary>
        ///  到达城市城市三字码
        /// </summary>
        public string ToCityCode { get; set; }
        /// <summary>
        /// 出发航站楼
        /// </summary>
        public string FromTerminal
        {
            get;
            set;
        }
        /// <summary>
        /// 到达航站楼
        /// </summary>
        public string ToTerminal
        {
            get;
            set;
        }
        /// <summary>
        /// 折扣
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// 机型
        /// </summary>
        public string FlightModel { get; set; }

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
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }
        /// <summary>
        /// 特价舱位价
        /// </summary>
        public decimal SpecialPrice { get; set; }
        /// <summary>
        /// 特价机建
        /// </summary>
        public decimal SpecialTax { get; set; }
        /// <summary>
        /// 特价燃油
        /// </summary>
        public decimal SpecialFuelFee { get; set; }
        /// <summary>
        /// 特价Y舱的舱位价
        /// </summary>
        public decimal SpecialYPrice { get; set; }

    }
    public class PassengerRequest
    {
        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司卡号
        /// </summary>
        public string MemberCard { get; set; }
        /// <summary>
        /// 1.成人 2.儿童 3.婴儿
        /// </summary>
        public int PassengerType
        {
            get;
            set;
        }
        /// <summary>
        /// 证件类型
        /// </summary>
        public EnumIDType IdType { get; set; }

        /// <summary>
        /// 性别类型
        /// </summary>
        public EnumSexType SexType { get; set; }
        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo
        {
            get;
            set;
        }
        /// <summary>
        /// 乘客类型为儿童或者婴儿的出生日期
        /// </summary>
        public DateTime ChdBirthday
        {
            get;
            set;
        }
        /// <summary>
        /// 联系人手机号
        /// </summary>
        public string LinkPhone
        {
            get;
            set;
        }
        /// <summary>
        /// 出生日期
        /// </summary>
        public DateTime? Birth { get; set; }
    }
    public enum PnrType
    {
        /// <summary>
        /// 成人编码
        /// </summary>
        [Description("成人编码")]
        Adult = 1,
        /// <summary>
        /// 儿童编码
        /// </summary>
        [Description("儿童编码")]
        Child = 2
    }
}
