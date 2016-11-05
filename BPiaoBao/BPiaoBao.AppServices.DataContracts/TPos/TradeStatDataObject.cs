using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
    /// <summary>
    /// 
    /// </summary>
    public class TradeStatDataObject
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }

        /// <summary>
        /// Gets the date format string.
        /// </summary>
        /// <value>
        /// The date format string.
        /// </value>
        public string DateFormatString
        {
            get
            {
                return Date.ToString("MM-dd");
            }
        }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TradeMoney { get; set; }
        /// <summary>
        /// 交易笔数
        /// </summary>
        public int TradeTimes { get; set; }
        /// <summary>
        /// 交易收益
        /// </summary>
        public decimal TradeGain { get; set; }

        /// <summary>
        /// 交易笔数 （单位：千）
        /// </summary>
        /// <value>
        public decimal TradeMoneyK { get { return TradeMoney / 1000; } }
    }
}
