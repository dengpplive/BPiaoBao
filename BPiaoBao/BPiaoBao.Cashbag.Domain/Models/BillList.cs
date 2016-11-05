using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 账单
    /// </summary>
    public class BillList
    {
        /// <summary>
        /// 账单日期
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 手续费
        /// </summary>
        public decimal FeeAmount { get; set; }
        /// <summary>
        /// 滞纳金
        /// </summary>
        public decimal LateAmount { get; set; }
        /// <summary>
        /// 还款金额
        /// </summary>
        public decimal RepayAmount { get; set; }
        /// <summary>
        /// 应还金额
        /// </summary>
        public decimal ShouldRepayAmount { get; set; }
        /// <summary>
        /// 账单状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 账单金额
        /// </summary>
        public decimal BillAmount { get; set; }
        /// <summary>
        /// 最迟还款日期
        /// </summary>
        public DateTime ShouldRepayDate { get; set; }
        /// <summary>
        /// 账期
        /// </summary>
        public string CreditDayStr { get; set; }
    }
}
