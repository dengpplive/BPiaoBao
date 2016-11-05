
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.ScheduleTask
{
    public class WeeklySchedule : DailySchedule
    {
        public WeeklySchedule(int intervalWeek ,DateTime startTime )
            : base(7 * intervalWeek,startTime)
        {
        
        }
                
    }
}
