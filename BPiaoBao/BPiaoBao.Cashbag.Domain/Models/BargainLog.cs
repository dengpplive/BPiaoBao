using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 交易记录
    /// </summary>
    public class BargainLog
    {
        /// <summary>
        /// 资金渠道
        /// </summary>
        public string CashSource { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime BargainTime { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo { get; set; }
         
        /// <summary>
        /// 类型
        /// </summary>
        public string TradeType { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Notes { get; set; }

    }
}
