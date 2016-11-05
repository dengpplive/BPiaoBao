using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    public class BankCard
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
        /// 是否默认
        /// </summary>
        public bool IsDefault { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市 
        /// </summary>
        public string City { get; set; }
    }
}
