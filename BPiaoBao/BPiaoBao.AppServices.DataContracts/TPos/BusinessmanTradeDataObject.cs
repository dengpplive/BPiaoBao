using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
    public class BusinessmanTradeDataObject
    {
        /// <summary>
        /// Pos商户名
        /// </summary>
        public string BusinessmanName { get; set; }
        public IEnumerable<TradeStatDataObject> BusinessmanTradeStat { get; set; }
        /// <summary>
        /// 总交易额
        /// </summary>
        public decimal TotalTradeMoney { get; set; }
        /// <summary>
        /// 总交易笔数
        /// </summary>
        public int TotalTradeTimes { get; set; }
        /// <summary>
        /// 总交易收益
        /// </summary>
        public decimal TotalTradeGain { get; set; }
    }
}
