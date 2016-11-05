using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class PassengerDto
    {
        public int Id { get; set; }
        /// <summary>
        /// 姓名
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
        /// 单人出票金额
        /// </summary>
        public decimal CPMoney
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
        /// 退改签手续费
        /// </summary>
        public decimal RetirementPoundage
        {
            get;
            set;
        }
        /// <summary>
        /// 单人支付金额
        /// </summary>
        public decimal PayMoney
        {
            get;
            set;
        }

        /// <summary>
        /// 购买航意保险份数
        /// </summary>
        public int BuyInsuranceCount
        {
            get;
            set;
        }

        /// <summary>
        /// 购买航意保险单价
        /// </summary>
        public decimal BuyInsurancePrice
        {
            get;
            set;
        }

        /// <summary>
        /// 是否购买航意险
        /// </summary>
        public bool IsBuyInsurance
        {
            get { return BuyInsuranceCount > 0 ? true : false; }
            set { this._isbuyInsurance = value; }
        }

        /// <summary>
        /// 是否够买了保险急速退
        /// </summary>
        public bool IsInsuranceRefund { get; set; }

        /// <summary>
        /// 急速度退保险金额
        /// </summary>
        public decimal InsuranceRefunrPrice { get; set; }

        private bool _isbuyInsurance = false;

        public List<SkyWayDto> SkyWays { get; set; }

        public DateTime? Birth { get; set; }

    }
}
