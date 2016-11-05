using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Models
{
    /// <summary>
    /// 保险查询类
    /// </summary>
    public class QueryInsurance
    { 

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
        public string EnumInsuranceStatus { get; set; }

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
        public string FlyStartTime { get; set; }

        /// <summary>
        ///  起飞时间 (结束时间)
        /// </summary>
        public string FlyEndTime { get; set; }


        /// <summary>
        /// 购买时间 (开始时间)
        /// </summary>
        public string BuyStartTime { get; set; }

        /// <summary>
        /// 购买时间 (结束时间)
        /// </summary>
        public string BuyEndTime { get; set; }

        /// <summary>
        /// 保单期限(开始时间)
        /// </summary>
        public string InsuranceLimitStartTime { get; set; }

        /// <summary>
        /// 保单期限(结束时间)
        /// </summary>
        public string InsuranceLimitEndTime { get; set; }


        /// <summary>
        /// 保险公司
        /// </summary>
        public string InsuranceCompany { get; set; }
    }
}