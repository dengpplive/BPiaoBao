using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class ResponseSMSSum
    {
        /// <summary>
        /// 商户Code
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string BusinessName { get; set; }
        /// <summary>
        /// 购买条数
        /// </summary>
        public int BuyCount { get; set; }
        /// <summary>
        /// 购买次数
        /// </summary>
        public int BuyTimes { get; set; }
        /// <summary>
        /// 购买金额
        /// </summary>
        public decimal BuyMoney { get; set; }
        /// <summary>
        /// 使用条数
        /// </summary>
        public int UseCount { get; set; }
        /// <summary>
        /// 剩余条数
        /// </summary>
        public int RemainCount { get; set; }
    }
}
