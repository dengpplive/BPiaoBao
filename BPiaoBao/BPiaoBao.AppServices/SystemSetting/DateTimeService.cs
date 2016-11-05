using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.AppServices.Contracts.SystemSetting;

namespace BPiaoBao.AppServices.SystemSetting
{
   public class DateTimeService : IDateTimeService
    {
       public DateTime GetCurrentSystemDateTime()
       {
           return DateTime.Now;
       }

       public string GetCurrentSystemDateTimeLongString()
       {
           var currentDateTime = GetCurrentSystemDateTime();
           var dateTimeStr = currentDateTime.ToString("yyyy-MM-dd HH:mm:ss");
           return dateTimeStr;
       }

       public string GetCurrentSystemDateTimeShortString()
       {
           var currentDateTime = GetCurrentSystemDateTime();
           var dateTimeStr = currentDateTime.ToString("yyyy-MM-dd");
           return dateTimeStr;
       }

       public string GetCurrentSystemTimeString()
       {
           var currentDateTime = GetCurrentSystemDateTime();
           var timeStr = currentDateTime.ToString("HH:mm:ss");
           return timeStr;
       }


       /// <summary>
       /// 表示自 0001 年 1 月 1 日午夜 12:00:00（表示 DateTime.MinValue）
       /// 以来经过的以 100 纳秒为间隔的间隔数.
       /// 每个计时周期表示一百纳秒，即一千万分之一秒
       /// </summary>
       /// <returns></returns>
       public long GetCurrentSystemTimeTicks()
       {
           var currentDateTime = GetCurrentSystemDateTime();
           var ticks = currentDateTime.Ticks;
           return ticks;
       }
    }
}
