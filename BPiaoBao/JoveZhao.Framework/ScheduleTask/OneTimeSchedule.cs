
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.ScheduleTask
{
    public class OneTimeSchedule : Schedule
    {
        DateTime _startTime;
        public OneTimeSchedule(DateTime startTime)
        {
            this._startTime = startTime;
        }
        
        public override DateTime GetNextExecuteTime()
        {
            DateTime now = DateTime.Now;
            if (_startTime > now)
                return _startTime;
            else

                return DateTime.MaxValue;
        }
    }
}
