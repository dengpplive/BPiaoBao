using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Domain.Models
{
    public class QueryParam
    {
        /// <summary>
        /// CTU
        /// </summary>
        public string FromCode
        {
            get;
            set;
        }
        /// <summary>
        /// PEK
        /// </summary>
        public string ToCode
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 2014-03-30
        /// </summary>
        public string FlyDate
        {
            get;
            set;
        }
        /// <summary>
        /// 00:00:00
        /// </summary>
        public string FlyTime
        {
            get;
            set;
        }
        /// <summary>
        /// 是否查看共享航班
        /// </summary>
        public bool IsShare
        {
            get;
            set;
        }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code
        {
            get;
            set;
        }
    }
    [Serializable]
    public class AVHData
    {
        public AVHData()
        {
            this.DicYSeatPrice = new Dictionary<string, decimal>();
            this.IbeData = new List<IBERow>();
        }
        /// <summary>
        /// IBE返回来的AVH原始数据
        /// </summary>
        public string AVHString
        {
            get;
            set;
        }
        public QueryParam QueryParam
        {
            get;
            set;
        }
        /// <summary>
        /// Y舱舱位价
        /// </summary>
        public Dictionary<string, decimal> DicYSeatPrice
        {
            get;
            set;
        }
        public List<IBERow> IbeData
        {
            get;
            set;
        }
    }
    [Serializable]
    public class IBERow
    {
        #region 基本数据

        /// <summary>
        /// CTU
        /// </summary>
        public string FromCode
        {
            get;
            set;
        }
        /// <summary>
        /// PEK
        /// </summary>
        public string ToCode
        {
            get;
            set;
        }
        /// <summary>
        /// 2014-03-30
        /// </summary>
        public string FlyDate
        {
            get;
            set;
        }
        /// <summary>
        /// 00:00
        /// </summary>
        public string StartTime
        {
            get;
            set;
        }
        /// <summary>
        /// 00:00
        /// </summary>
        public string EndTime
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNo { get; set; }
        /// <summary>
        /// IBE舱位信息
        /// </summary>
        public List<IbeSeat> IBESeat { get; set; }
        /// <summary>
        /// 机型
        /// </summary>
        public string AirModel { get; set; }
        /// <summary>
        /// 是否经停
        /// </summary>
        public bool IsStop { get; set; }
        /// <summary>
        /// 是否餐食
        /// </summary>
        public bool IsMeals { get; set; }
        /// <summary>
        /// 是否电子客票标示
        /// </summary>
        public bool IsElectronicTicket { get; set; }
        /// <summary>
        /// 是否共享航班
        /// </summary>
        public bool IsShareFlight { get; set; }
        /// <summary>
        /// 子舱位
        /// </summary>
        public string ChildSeat { get; set; }
        /// <summary>
        /// 出发航站楼
        /// </summary>
        public string FromCityT1 { get; set; }
        /// <summary>
        /// 到达航站楼
        /// </summary>
        public string ToCityT1 { get; set; }

        #endregion

        #region  附加数据
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal TaxFee { get; set; }
        /// <summary>
        /// 成人燃油费
        /// </summary>
        public decimal ADultFuleFee { get; set; }
        /// <summary>
        /// 儿童燃油费
        /// </summary>
        public decimal ChildFuleFee { get; set; }
        #endregion


    }
    [Serializable]
    public class IbeSeat : ICloneable
    {
        public IbeSeat()
        {
            this.PolicySpecialType = EnumPolicySpecialType.Normal;
        }
        #region 基本数据
        /// <summary>
        /// 舱位 Y
        /// </summary>
        public string Seat { get; set; }
        /// <summary>
        /// 如A S
        /// </summary>
        public string SeatSymbol
        {
            get;
            set;
        }
        /// <summary>
        /// 剩余舱位个数 如>9 8
        /// </summary>
        public string SeatCount { get; set; }
        #endregion

        #region 附加数据

        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }

        /// <summary>
        /// IBE原始舱位价不变
        /// </summary>
        public decimal IbeSeatPrice { get; set; }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 政策ID
        /// </summary>
        public string PolicyId { get; set; }
        /// <summary>
        /// 政策
        /// </summary>
        public decimal Policy { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public string PlatformCode { get; set; }
        /// <summary>
        /// 政策备注
        /// </summary>
        public string PolicyRMK { get; set; }
        /// <summary>
        /// 现返
        /// </summary>
        public decimal ReturnMoney { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }

        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }
        /// <summary>
        /// 特价价格或者折扣
        /// </summary>
        public decimal SpecialPriceOrDiscount { get; set; }

        #region ICloneable 成员

        public object Clone()
        {
            return base.MemberwiseClone();
        }

        #endregion
        #endregion
    }
}
