
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.ScheduleTask
{
    /// <summary>
    /// 按日执行，可设置间隔几日执行
    /// </summary>
    public class DailySchedule : IntervalSchedule
    {
        public DailySchedule(int intervalDays, DateTime startTime)
            : base(new TimeSpan(intervalDays, 0, 0, 0), startTime)
        {
            
        }

    }

}
