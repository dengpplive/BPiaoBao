using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 最近15天交易统计
    /// </summary>
    public class Current15DayDataDto
    {
        //15天数据列表
        public List<DataStatistics> DataStatisticsList { get; set; }
        /// <summary>
        /// 查询月的交易总金额
        /// </summary>
        public decimal TotalTradeMoney { get; set; }
        /// <summary>
        /// 一天的总量
        /// </summary>
        public class DataStatistics
        {
            /// <summary>
            /// 交易金额
            /// </summary>
            public decimal TradeTotalMoney { get; set; }

            /// <summary>
            /// 那天
            /// </summary>
            public DateTime Day { get; set; }

            /// <summary>
            /// 日期字符串
            /// </summary>
            public string DayFormatString
            {
                get
                {
                    return Day.ToString("MM-dd");
                }
            }
        }
    }
}
