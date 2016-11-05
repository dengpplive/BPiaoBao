using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 数据统计结构数据
    /// </summary>
    public class DataStatisticsDto
    {
        //一月数据列表
        public List<DataStatistics> DataStatisticsList { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string OperatorAccount { get; set; }
        /// <summary>
        /// 查询月的交易总金额
        /// </summary>
        public decimal TotalTradeMoney { get; set; }
        /// <summary>
        /// 查询月的总佣金
        /// </summary>
        public decimal TotalCommission { get; set; }
        /// <summary>
        /// 查询月的总出票量
        /// </summary>
        public int TotalIssueTicket { get; set; }

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
            /// 佣金
            /// </summary>
            public decimal CommissionTotalMoney { get; set; }

            /// <summary>
            /// 出票量
            /// </summary>
            public int IssueTicketCount { get; set; }

            /// <summary>
            /// 几号
            /// </summary>
            public int Day { get; set; }
        }

    }
}
