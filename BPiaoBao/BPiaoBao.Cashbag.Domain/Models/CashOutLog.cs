using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 提现记录
    /// </summary>
    public class CashOutLog
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// 提现时间
        /// </summary>
        public DateTime? ClearDateTime { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CashOutTime { get; set; }
        /// <summary>
        /// 提现金额
        /// </summary>
        public decimal CashOutMoney { get; set; }
        /// <summary>
        /// 提现状态
        /// </summary>
        public string CashOutStatus { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BankNo { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal FeeAmount { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo { get; set; }
        /// <summary>
        /// 到账方式
        /// </summary>
        public string ReceivingType { get; set; }
    }
}
