using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;



namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    /// <summary>
    /// 商户信息
    /// </summary>
    public class AccountInfoDto
    {
        
        /// <summary>
        /// 现金账户
        /// </summary>
        public ReadyAccountDto ReadyInfo { get; set; }

        /// <summary>
        /// 信用账户
        /// </summary>
        public CreditAccountDto CreditInfo { get; set; }
        /// <summary>
        /// 积分账户
        /// </summary>
        public ScoreAccountDto ScoreInfo { get; set; }
        /// <summary>
        /// 理财信息
        /// </summary>
        public FinancialAccountDto FinancialInfo { get; set; }
        
       
    }
}
