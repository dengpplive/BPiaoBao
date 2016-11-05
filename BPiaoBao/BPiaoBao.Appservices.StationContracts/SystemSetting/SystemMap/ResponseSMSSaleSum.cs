using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class ResponseSMSSaleSum
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 现金金额
        /// </summary>
        public decimal AccountMoney { get; set; }
        /// <summary>
        /// 现金手续费
        /// </summary>
        public decimal AccountPoundage { get; set; }
        /// <summary>
        /// 信用金额
        /// </summary>
        public decimal CreditMoney { get; set; }
        /// <summary>
        /// 信用手续费
        /// </summary>
        public decimal CreditPoundage { get; set; }
        /// <summary>
        /// 银行卡金额
        /// </summary>
        public decimal AliPayMoney { get; set; }
        /// <summary>
        /// 银行卡手续费
        /// </summary>
        public decimal AliPayPoundage { get; set; }
        /// <summary>
        /// 支付平台金额
        /// </summary>
        public decimal TenPayMoney { get; set; }
        /// <summary>
        /// 支付平台手续费
        /// </summary>
        public decimal TenPayPoundage { get; set; }
        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 合计手续费
        /// </summary>
        public decimal TotalPoundage { get; set; }
    }
}
