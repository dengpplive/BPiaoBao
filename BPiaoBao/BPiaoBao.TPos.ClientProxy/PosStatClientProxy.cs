using BPiaoBao.Common;
using BPiaoBaoTPos.Domain.Models;
using BPiaoBaoTPos.Domain.Services;
using JoveZhao.Framework;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.TPos.ClientProxy
{
    public class PosStatClientProxy : IPosStatClientProxy
    {
        private string WebUrlTPos = @"" + SettingSection.GetInstances().Cashbag.BaseUrl + "TPos/";
        public BPiaoBaoTPos.Domain.Models.AccountStat GetAccountStat(string code, string key)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "GetFirstPageInfo", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
            var account = new AccountStat()
            {
                BusinessmanCount = result.result.CompanyCount,
                HistoryGain = result.result.HisAmount,
                PosCount = result.result.POSCount,
                YesterdayGain = result.result.YesterdayAmount,
                AssignPosCount = result.result.AssignPosCount,
                UnAssignPosCount = result.result.UnAssignPosCount
            };
            return account;
        }

        public IEnumerable<BPiaoBaoTPos.Domain.Models.TradeStat> GainStat(string code, string key, DateTime startTime, DateTime endTime)
        {

            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "IncomeStatistics", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("StartDate", startTime.ToString("yyyyMMddHHmmss"));
            dictionary.Add("EndDate", endTime.ToString("yyyyMMddHHmmss"));
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
            List<TradeStat> list = new List<TradeStat>();
            var rows = JArray.FromObject(result.result.rows);
            foreach (var item in rows)
            {
                TradeStat tradestat = new TradeStat()
                {
                    Date = item.CreateDate,
                    TradeGain = item.Amount,
                    TradeMoney = item.OrderAmount,
                    TradeTimes = item.Count
                };
                list.Add(tradestat);
            }
            return list;
        }

        public Tuple<IEnumerable<BPiaoBaoTPos.Domain.Models.TradeDetail>, int> GetTradeDetail(string code, string key, DateTime? startTime, DateTime? endTime, string posNo, int startIndex, int count)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "QueryTrade", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            if (startTime.HasValue)
            {
                dictionary.Add("StartDate", startTime.Value.ToString("yyyyMMddHHmmss"));
            }
            if (endTime.HasValue)
            {
                dictionary.Add("EndDate", endTime.Value.ToString("yyyyMMddHHmmss"));
            }
            dictionary.Add("POSNumber", posNo);
            var page = Math.Ceiling((double)startIndex / count) + 1;
            dictionary.Add("CurrentPage", page.ToString());
            dictionary.Add("PageSize", count.ToString());
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
            List<TradeDetail> list = new List<TradeDetail>();
            var rows = JArray.FromObject(result.result.rows);
            int totalcount = result.result.total;
            foreach (var item in rows)
            {
                TradeDetail tradedetail = new TradeDetail()
                {
                    BatchNo = item.OutOrderNo,
                    BusinessmanName = item.CompanyFullName,
                    ReceivMoney = item.Amount,
                    PosGain = item.POSAmount,
                    PosNo = item.POSNumber,
                    PosRate = item.Rate,
                    TradeCardNo = item.CardNo,
                    TradeCardType = item.CardType,
                    TradeMoney = item.OrderAmount,
                    TradeTime = item.CreateDate
                };
                list.Add(tradedetail);
            }
            Tuple<IEnumerable<TradeDetail>, int> tuple = new Tuple<IEnumerable<TradeDetail>, int>(list,totalcount);
            return tuple;
        }

        public BPiaoBaoTPos.Domain.Models.BusinessmanReport GetBusinessmanReport(string code, string key, DateTime startTime, DateTime endTime)
        {
            CashbagHelper ch = new CashbagHelper(WebUrlTPos + "AgentTradeSummary", "GET");
            Dictionary<string, string> dictionary = new Dictionary<string, string>();
            dictionary.Add("code", code);
            dictionary.Add("key", key);
            dictionary.Add("StartDate", startTime.ToString("yyyyMMddHHmmss"));
            dictionary.Add("EndDate", endTime.ToString("yyyyMMddHHmmss"));
            dictionary.Add("currentTime", DateTime.Now.ToString("yyyyMMddHHmmss"));
            string data = ch.ParamsURLEncode(dictionary);
            var result = ch.GetBackJsonData(data);
            if (result.status == false)
                throw new CustomException(500, result.message.ToString());
            List<BusinessmanTrade> listtrade = new List<BusinessmanTrade>();
            
            var rows = JArray.FromObject(result.result.rows);
            
            foreach (var item in rows)
            {
                 BusinessmanTrade btrade = new BusinessmanTrade() 
                {
                    BusinessmanName = item.CompanyName
                };
                 List<TradeStat> listtradestat = new List<TradeStat>();
                foreach (var TradeSet in item.TradeStat)
                {
                    listtradestat.Add(new TradeStat() {
                        Date = TradeSet.Date,
                        TradeGain = TradeSet.SplitAmount,
                        TradeMoney = TradeSet.TradeMoney,
                        TradeTimes = TradeSet.TradeTimes 
                    });
                }
                btrade.BusinessmanTradeStat = listtradestat;
                listtrade.Add(btrade);
            }
            
            var report = new BusinessmanReport()
            {
                TradeList = listtrade
               
            };
            return report;
        }
    }
}
