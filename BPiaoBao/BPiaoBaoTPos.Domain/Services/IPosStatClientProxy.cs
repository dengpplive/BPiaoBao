using BPiaoBaoTPos.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBaoTPos.Domain.Services
{
    public interface IPosStatClientProxy
    {
        /// <summary>
        /// 账户统计
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <returns></returns>
         AccountStat GetAccountStat(string code, string key);
        /// <summary>
        /// 收益统计
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
         IEnumerable<TradeStat> GainStat(string code, string key, DateTime startTime, DateTime endTime);
        /// <summary>
        /// 交易明细
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="posNo"></param>
        /// <param name="startIndex"></param>
        /// <param name="count"></param>
        /// <returns></returns>
         Tuple<IEnumerable<TradeDetail>, int> GetTradeDetail(string code, string key, DateTime? startTime, DateTime? endTime, string posNo, int startIndex, int count);
        /// <summary>
        /// Pos商户报表
        /// </summary>
        /// <param name="code"></param>
        /// <param name="key"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <returns></returns>
         BusinessmanReport GetBusinessmanReport(string code, string key, DateTime startTime, DateTime endTime);
    }
}
