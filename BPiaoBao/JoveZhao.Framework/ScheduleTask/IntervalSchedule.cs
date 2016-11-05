
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.ScheduleTask

{
    /// <summary>
    /// 按时间间隔执行
    /// </summary>
    public class IntervalSchedule : Schedule
    {
        private TimeSpan _timeSpan;
        private DateTime _startTime;

        public IntervalSchedule(TimeSpan timeSpan, DateTime startTime)
        {
            this._timeSpan = timeSpan;
            this._startTime = startTime;

        }

        public override DateTime GetNextExecuteTime()
        {
            var now = DateTime.Now;
            if (_startTime > now)
                return _startTime;
            else
                return DateTime.Now.Add(_timeSpan);
        }
    }

}