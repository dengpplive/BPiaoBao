using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    /// <summary>
    /// 保单查询条件实体
    /// </summary>
    public class RequestQueryInsurance
    {

        /// <summary>
        /// 是否是控台调用
        /// </summary>
        public bool IsCtrlStationCall { get; set; }

        /// <summary>
        /// 是否是买票宝客户端调用
        /// </summary>
        public bool IsClientCall { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string PayNo { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 保险单号
        /// </summary>
        public string InsuranceNo { get; set; }

        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 乘机人
        /// </summary>
        public string PassengerName { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 保单状态
        /// </summary>
        public EnumInsuranceStatus? EnumInsuranceStatus { get; set; }

        /// <summary>
        /// 航程起点
        /// </summary>
        public string FlightTripFrom { get; set; }

        /// <summary>
        /// 航程终点
        /// </summary>
        public string FlightTripTo { get; set; }

        /// <summary>
        /// 起飞时间 (开始时间)
        /// </summary>
        public DateTime? FlyStartTime { get; set; }

        /// <summary>
        ///  起飞时间 (结束时间)
        /// </summary>
        public DateTime? FlyEndTime { get; set; }


        /// <summary>
        /// 购买时间 (开始时间)
        /// </summary>
        public DateTime? BuyStartTime { get; set; }

        /// <summary>
        /// 购买时间 (结束时间)
        /// </summary>
        public DateTime? BuyEndTime { get; set; }

        /// <summary>
        /// 保单期限(开始时间)
        /// </summary>
        public DateTime? InsuranceLimitStartTime { get; set; }

        /// <summary>
        /// 保单期限(结束时间)
        /// </summary>
        public DateTime? InsuranceLimitEndTime { get; set; }


        /// <summary>
        /// 保险公司
        /// </summary>
        public string InsuranceCompany { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        public string CardNo { get; set; }
    }
}
