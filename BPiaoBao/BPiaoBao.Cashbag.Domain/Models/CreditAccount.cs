using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    public class CreditAccount
    {
        /// <summary>
        /// 信用账户可用余额
        /// </summary>
        public decimal CreditBalance { get; set; }
        /// <summary>
        /// 信用账户额度
        /// </summary>
        public decimal CreditQuota { get; set; }
        /// <summary>
        /// 临时额度
        /// </summary>
        public decimal TempQuota { get; set; }
        /// <summary>
        /// 信用账户状态(true开通,false未开通)
        /// </summary>
        public bool Status { get; set; }
        
    }
}
