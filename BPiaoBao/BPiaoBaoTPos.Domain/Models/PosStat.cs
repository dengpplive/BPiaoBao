using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBaoTPos.Domain.Models
{
    public class AccountStat
    {
        /// <summary>
        /// Pos总数
        /// </summary>
        public int PosCount { get; set; }
        /// <summary>
        /// 已分配Pos总数
        /// </summary>
        public int AssignPosCount { get; set; }
        /// <summary>
        /// 未分配Pos总数
        /// </summary>
        public int UnAssignPosCount { get; set; }
        /// <summary>
        /// 商户总数
        /// </summary>
        public int BusinessmanCount { get; set; }
        /// <summary>
        /// 历史收益
        /// </summary>
        public decimal HistoryGain { get; set; }
        /// <summary>
        /// 昨日收益
        /// </summary>
        public decimal YesterdayGain { get; set; }
    }
    public class TradeStat
    {
        /// <summary>
        /// 日期
        /// </summary>
        public DateTime Date { get; set; }
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
    }

    public class TradeDetail
    {
        /// <summary>
        /// Pos终端号
        /// </summary>
        public string PosNo { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TradeTime { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 交易卡号
        /// </summary>
        public string TradeCardNo { get; set; }
        /// <summary>
        /// 交易卡号类别
        /// </summary>
        public string TradeCardType { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TradeMoney { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal ReceivMoney { get; set; }
        /// <summary>
        /// Pos费率
        /// </summary>
        public decimal PosRate { get; set; }
        /// <summary>
        /// Pos收益
        /// </summary>
        public decimal PosGain { get; set; }
    }

    public class BusinessmanReport
    {
        /// <summary>
        /// 总交易次数
        /// </summary>
        public int TotalTradeTimes
        {
            get
            {
                return TradeList.Sum(p => p.TotalTradeTimes);
            }
        }
        /// <summary>
        /// 总交易金额
        /// </summary>
        public decimal TotalTradeMoney
        {
            get
            {
                return TradeList.Sum(p => p.TotalTradeMoney);
            }
        }
        /// <summary>
        /// 总收益
        /// </summary>
        public decimal TotalGain
        {
            get
            {
                return TradeList.Sum(p => p.TotalTradeGain);
            }
        }
        /// <summary>
        /// 交易列表
        /// </summary>
        public IEnumerable<BusinessmanTrade> TradeList { get; set; }

    }
    public class BusinessmanTrade
    {
        /// <summary>
        /// Pos商户名
        /// </summary>
        public string BusinessmanName { get; set; }
        public IEnumerable<TradeStat> BusinessmanTradeStat { get; set; }
        /// <summary>
        /// 总交易额
        /// </summary>
        public decimal TotalTradeMoney
        {
            get
            {
                return BusinessmanTradeStat.Sum(p => p.TradeMoney);
            }
        }
        /// <summary>
        /// 总交易笔数
        /// </summary>
        public int TotalTradeTimes
        {
            get
            {
                return BusinessmanTradeStat.Sum(p => p.TradeTimes);
            }
        }
        /// <summary>
        /// 总交易收益
        /// </summary>
        public decimal TotalTradeGain
        {
            get
            {
                return BusinessmanTradeStat.Sum(p => p.TradeGain);
            }
        }
    }
}
