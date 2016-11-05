using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;
namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    /// <summary>
    /// 保单实体
    /// </summary>
    public class ResponseInsurance
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 保险单号
        /// </summary>
        public string InsuranceNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }

        ///// <summary>
        ///// 交易号
        ///// </summary>
        //public string TradeId { get; set; }

        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName { get; set; }

        public string Mobile { get; set; }

        public string CardNo { get; set; }

        public DateTime? StartDateTime { get; set; }

        public DateTime? ToDateTime { get; set; }

        /// <summary>
        /// 购买时间 
        /// </summary>
        public DateTime BuyTime { get; set; }

        /// <summary>
        /// 保单期限(开始时间)
        /// </summary>
        public DateTime InsuranceLimitStartTime { get; set; }

        /// <summary>
        /// 保单期限(结束时间)
        /// </summary>
        public DateTime InsuranceLimitEndTime { get; set; }

        /// <summary>
        /// 保险公司
        /// </summary>
        public string InsuranceCompany { get; set; }


        /// <summary>
        /// 保单金额
        /// </summary>
        public decimal InsurancePrice { get; set; }

        /// <summary>
        /// 保单状态
        /// </summary>
        public EnumInsuranceStatus EnumInsuranceStatus { get; set; }


        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }

        /// <summary>
        /// 运营商名称
        /// </summary>
        public string CarrierName { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string BussinessmanCode { get; set; }

        /// <summary>
        /// 商户名称
        /// </summary>
        public string BussinessmanName { get; set; }

        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCity{get;set;}

        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCode { get; set; }

        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCity { get; set; }

        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCode { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public EnumIDType IdType { get; set; }

        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }

        public string PNR { get; set; }

        /// <summary>
        /// 性别类型
        /// </summary>
        public EnumSexType SexType { get; set; }

        /// <summary>
        /// 投保方式
        /// </summary>
        public EnumInsureMethod InsureType { get; set; }

        /// <summary>
        /// 保额
        /// </summary>
        public decimal PolicyAmount { get; set; }

        /// <summary>
        /// 乘客类型 1成人 2儿童 3婴儿
        /// </summary>
        public EnumPassengerType PassengerType { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birth { get; set; }

        /// <summary>
        /// 保险记录Id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 保险订单Id
        /// </summary>
        public int InsuranceOrderId { get; set; }

        ///// <summary>
        ///// 保险总额
        ///// </summary>
        //public decimal Sum
        //{
        //    get { return Passenger.BuyInsuranceCount * Passenger.BuyInsurancePrice; }
        //    set { this._Sum = value; }
        //}
        //private decimal _Sum;
    }
}
