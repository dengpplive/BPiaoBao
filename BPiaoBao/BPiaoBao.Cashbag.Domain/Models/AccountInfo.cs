using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using JoveZhao.Framework.Expand;
using StructureMap;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 商户信息
    /// </summary>
    public class AccountInfo
    {
        
        /// <summary>
        /// 现金账户
        /// </summary>
        public ReadyAccount ReadyInfo { get; set; }

        /// <summary>
        /// 信用账户
        /// </summary>
        public CreditAccount CreditInfo { get; set; }
        /// <summary>
        /// 积分账户
        /// </summary>
        public ScoreAccount ScoreInfo { get; set; }
        /// <summary>
        /// 理财信息
        /// </summary>
        public FinancialAccount FinancialInfo { get; set; }
        
       
    }
}
