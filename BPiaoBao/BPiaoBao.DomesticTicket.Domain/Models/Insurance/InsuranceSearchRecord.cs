using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{
    /// <summary>
    /// 查询用实体，该实体对应一个视图
    /// </summary>
    public class InsuranceSearchRecord
    {

        /// <summary>
        /// 保险订单Id
        /// </summary>
        public int InsuranceOrderId { get; set; }
        /// <summary>
        /// 保险记录主键id
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 机票订单号
        /// </summary>
        public string OrderId { get; set; }

        ///// <summary>
        ///// 支付交易号
        ///// </summary>
        //public string TradeId { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }

        /// <summary>
        /// 保单状态
        /// </summary>
        public EnumInsuranceStatus EnumInsuranceStatus { get; set; }

        /// <summary>
        /// 下单时间
        /// </summary>
        public DateTime? BuyTime { get; set; }

        /// <summary>
        /// 保险单号
        /// </summary>
        public string InsuranceNo { get; set; }

        /// <summary>
        /// 保险单价
        /// </summary>
        public decimal InsurancePrice { get; set; }

        /// <summary>
        /// 保险生效开始时间
        /// </summary>
        public DateTime? InsuranceLimitStartTime { get; set; }

        /// <summary>
        /// 保险生效结束时间
        /// </summary>
        public DateTime? InsuranceLimitEndTime { get; set; }

        /// <summary>
        /// 保险公司
        /// </summary>
        public string InsuranceCompany { get; set; }

        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }

        /// <summary>
        /// 运营商名称
        /// </summary>
        public string CarrierName { get; set; }

        /// <summary>
        /// 分销商Code
        /// </summary>
        public string BussinessmanCode { get; set; }

        /// <summary>
        /// 分销商名称
        /// </summary>
        public string BussinessmanName { get; set; }

        /// <summary>
        /// 乘客姓名
        /// </summary>
        public string PassengerName { get; set; }

        /// <summary>
        /// 手机
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 出发城市名称
        /// </summary>
        public string FromCity { get; set; }

        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCity { get; set; }

        /// <summary>
        /// 出发城市Code
        /// </summary>
        public string FromCityCode { get; set; }

        /// <summary>
        /// 到达城市Code
        /// </summary>
        public string ToCityCode { get; set; }

        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime? StartDateTime{get;set;}
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime? ToDateTime { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public EnumIDType IdType { get; set; }

        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }

        /// <summary>
        /// 性别类型
        /// </summary>
        public EnumSexType SexType { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birth { get; set; }

        /// <summary>
        /// 乘客类型 1成人 2儿童 3婴儿
        /// </summary>
        public EnumPassengerType PassengerType { get; set; }

        /// <summary>
        /// 保额
        /// </summary>
        public decimal PolicyAmount { get; set; }

        /// <summary>
        /// 投保方式
        /// </summary>
        public EnumInsureMethod InsureType { get; set; }


        public string PNR { get; set; }

        public string ConvertCodeToCityName(string cityCode)
        {
            string city = string.Empty;
            PnrAnalysis.PnrResource pnrResource = new PnrAnalysis.PnrResource();
            var cityInfo = pnrResource.GetCityInfo(cityCode);
            if (cityInfo != null)
                city = cityInfo.city.Name;
            return city;
        }

        ///// <summary>
        ///// 航线
        ///// </summary>
        //public virtual SkyWay SkyWay { get; set; }
    }
}
