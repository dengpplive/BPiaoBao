using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.DomesticTicket.Model
{
    /// <summary>
    /// 图标模型
    /// </summary>
    public class ChartModel : Collection<BPiaoBao.AppServices.DataContracts.DomesticTicket.Current15DayDataDto.DataStatistics>
    {
        public ChartModel()
        {
            //Add(new AppServices.DataContracts.DomesticTicket.Current15DayDataDto.DataStatistics() { Day = DateTime.Today, TradeTotalMoney = 100 });
            //Add(new AppServices.DataContracts.DomesticTicket.Current15DayDataDto.DataStatistics() { Day = DateTime.Today.AddDays(-1), TradeTotalMoney = 2 });
            //Add(new AppServices.DataContracts.DomesticTicket.Current15DayDataDto.DataStatistics() { Day = DateTime.Today.AddDays(-2), TradeTotalMoney = 300 });
            //Add(new AppServices.DataContracts.DomesticTicket.Current15DayDataDto.DataStatistics() { Day = DateTime.Today.AddDays(-3), TradeTotalMoney = 25 });
            //Add(new AppServices.DataContracts.DomesticTicket.Current15DayDataDto.DataStatistics() { Day = DateTime.Today.AddDays(-4), TradeTotalMoney = 500 });
            //Add(new AppServices.DataContracts.DomesticTicket.Current15DayDataDto.DataStatistics() { Day = DateTime.Today.AddDays(-5), TradeTotalMoney = 211 });

        }
    }
}
