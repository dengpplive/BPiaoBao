using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    public class ReadyAccountDto
    {
        /// <summary>
        /// 现金账户余额
        /// </summary>
        public decimal ReadyBalance { get; set; }
        /// <summary>
        /// 冻结金额
        /// </summary>
        public decimal FreezeAmount { get; set; }
        /// <summary>
        /// 账户总额
        /// </summary>
        public decimal TotalBalance
        {
            get
            {
                return ReadyBalance + FreezeAmount;
            }
        }
    }
}
