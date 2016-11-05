using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
   public class ReadyAccount
    {
        /// <summary>
        /// 现金账户余额
        /// </summary>
        public decimal ReadyBalance { get; set; }
        /// <summary>
        /// 冻结金额
        /// </summary>
        public decimal FreezeAmount { get; set; }

    }
}
