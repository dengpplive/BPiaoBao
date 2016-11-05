using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    /// <summary>
    /// 充值记录
    /// </summary>
    public class RechargeLogDto
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// 充值时间
        /// </summary>
        public DateTime RechargeTime { get; set; }
        /// <summary>
        /// 充值类型
        /// </summary>
        public string TypeName { get; set; }
        /// <summary>
        /// 充值金额
        /// </summary>
        public decimal RechargeMoney { get; set; }
        /// <summary>
        /// 资金来源
        /// </summary>
        public string CashSource { get; set; }
        /// <summary>
        /// 充值状态
        /// </summary>
        public string RechargeStatus { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string RechargeNotes { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo { get; set; }
    }
    
}
