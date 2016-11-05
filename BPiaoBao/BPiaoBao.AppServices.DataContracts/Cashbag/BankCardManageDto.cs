using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    /// <summary>
    /// 银行卡管理
    /// </summary>
    public class BankCardManageDto
    {
        /// <summary>
        /// 银行名称
        /// </summary>
        public string BankName { set; get; }
        /// <summary>
        /// 开户行名称
        /// </summary>
        public string NewAccountBank { set; get; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string BanKCardNumber { set; get; }
        /// <summary>
        /// 开户人名
        /// </summary>
        public string NewAccountName { set; get; }
        /// <summary>
        /// 银行卡类型
        /// </summary>
        public string CardType { set; get; }
    }
}
