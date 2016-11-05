using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
    public class BusinessmanReportDataObject
    {
        /// <summary>
        /// 总交易次数
        /// </summary>
        public int TotalTradeTimes
        {
            get;
            set;
        }
        /// <summary>
        /// 总交易金额
        /// </summary>
        public decimal TotalTradeMoney
        {
            get;
            set;
        }
        /// <summary>
        /// 总收益
        /// </summary>
        public decimal TotalGain
        {
            get;
            set;
        }
        /// <summary>
        /// 交易列表
        /// </summary>
        public IEnumerable<BusinessmanTradeDataObject> TradeList { get; set; }
    }
}
