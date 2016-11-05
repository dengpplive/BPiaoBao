using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common
{
    public class FundInfo
    {
        /// <summary>
        /// 现金账户总余额
        /// </summary>
        public decimal RecieveMoney { get; set; }

        /// <summary>
        /// 信用账户可用余额
        /// </summary>
        public decimal CreditMoney { get; set; }

        /// <summary>
        /// 现金账户可用余额
        /// </summary>
        public decimal RecieveAmount { get; set; }
    }
}
