using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    public class RepayInfoDto
    {
        /// <summary>
        /// 信用额度
        /// </summary>
        public decimal CreditAmount { get; set; }
        /// <summary>
        /// 可用额度
        /// </summary>
        public decimal AvailableAmount { get; set; }
        /// <summary>
        /// 逾期金额
        /// </summary>
        public decimal LateAmount { get; set; }
        /// <summary>
        /// 本期应还
        /// </summary>
        public decimal ShouldRepayMoney { get; set; }
        /// <summary>
        /// 欠款总额
        /// </summary>
        public decimal OweRentMoney { get; set; }
        
    }
}
