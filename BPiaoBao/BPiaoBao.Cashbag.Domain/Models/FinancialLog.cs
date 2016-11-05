using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 理财日志
    /// </summary>
    public class FinancialLog 
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime BuyTime { get; set; }
        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 收益
        /// </summary>
        public decimal PointAmount { get; set; }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal FinancialMoney { get; set; }
        /// <summary>
        /// 理财状态
        /// </summary>
        public string FinancialLogStatus { get; set; }
        /// <summary>
        /// 开始时间
        /// </summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// 中止时间
        /// </summary>
        public DateTime AbortTime { get; set; }
        /// <summary>
        /// 预订到期时间
        /// </summary>
        public DateTime PreEndDateTime { get; set; }
        /// <summary>
        /// 资金渠道
        /// </summary>
        public string CashSource { get; set; }
        /// <summary>
        /// 交易时间（审核处理时间）
        /// </summary>
        public DateTime? AuditDate { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}
