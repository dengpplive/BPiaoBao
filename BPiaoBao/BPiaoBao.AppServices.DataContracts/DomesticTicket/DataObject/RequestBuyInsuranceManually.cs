using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    public class RequestBuyInsuranceManually
    {
        /// <summary>
        /// 保险期限，开始时间
        /// </summary>
        public DateTime InsuranceLimitStartTime { get; set; }

        /// <summary>
        /// 保险期限，结束时间
        /// </summary>
        public DateTime InsuranceLimitEndTime { get; set; }

        /// <summary>
        /// 被投保人
        /// </summary>
        public string InsuredName { get; set; }

        /// <summary>
        /// 证件类型
        /// </summary>
        public EnumIDType IdType { get; set; }

        /// <summary>
        /// 被投保人证件号
        /// </summary>
        public string CardNo { get; set; }

        /// <summary>
        /// 性别类型
        /// </summary>
        public EnumSexType SexType { get; set; }

        /// <summary>
        /// 证件号
        /// </summary>
        //public string CardNo { get; set; }

        /// <summary>
        /// 手机号
        /// </summary>
        public string Mobile { get; set; }

        /// <summary>
        /// 保险数量
        /// </summary>
        public int Count { get; set; }

        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }

        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime { get; set; }

        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCityName { get; set; }

        /// <summary>
        /// PNR
        /// </summary>
        public string PNR { get; set; }

        /// <summary>
        /// 生日
        /// </summary>
        public DateTime? Birth { get; set; }

        /// <summary>
        /// 乘客类型 1成人 2儿童 3婴儿
        /// </summary>
        public EnumPassengerType PassengerType { get; set; }
    }
}
