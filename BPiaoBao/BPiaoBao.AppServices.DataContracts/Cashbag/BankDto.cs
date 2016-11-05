using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    /// <summary>
    /// 银行卡信息
    /// </summary>
    public class BankDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public string BankId { get; set; }
        /// <summary>
        /// 银行名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 开户行
        /// </summary>
        public string BankBranch { get; set; }
        /// <summary>
        /// 银行卡号
        /// </summary>
        public string CardNo { get; set; }
        /// <summary>
        /// 开户人
        /// </summary>
        public string Owner { get; set; }
        /// <summary>
        /// 卡号状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }
    }
}
