using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 还款记录
    /// </summary>
    public class RepaymentLog
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// 还款时间
        /// </summary>
        public DateTime RepaymentTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string RepaymentNotes { get; set; }
        /// <summary>
        /// 还款金额
        /// </summary>
        public decimal RepaymentMoney { get; set; }
        /// <summary>
        /// 资金渠道
        /// </summary>
        public string CashSource { get; set; }
        /// <summary>
        /// 还款状态
        /// </summary>
        public string RepaymentStatus { get; set; }
        /// <summary>
        /// 未还款金额
        /// </summary>
        public decimal ShouldAmount { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TotalAmount { get; set; }
         /// <summary>
        /// 账单时间
        /// </summary>
        public DateTime BillTime { get; set; }
        /// <summary>
        /// 账单金额
        /// </summary>
        public decimal BillAmount { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OutOrderNo { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo { get; set; }
    }
}
