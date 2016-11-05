using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 转账记录
    /// </summary>
    public class TransferLog
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// 转账时间
        /// </summary>
        public DateTime TransferAccountsTime { get; set; }
        /// <summary>
        /// 转账方式
        /// </summary>
        public string TransferAccountsType { get; set; }
        /// <summary>
        /// 目标账户
        /// </summary>
        public string TargetAccount { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string TransferNotes { get; set; }
        /// <summary>
        /// 转账金额
        /// </summary>
        public decimal TransferAccountsMoney { get; set; }
        /// <summary>
        /// 转账状态
        /// </summary>
        public string TransferAccountsStatus { get; set; }
        /// <summary>
        /// 转账类型(转入，转出)
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo { get; set; }

    }
}
