
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.ScheduleTask

{
    /// <summary>
    /// 按月执行计划
    /// </summary>
    public class MonthlySchedule : Schedule
    {
        public MonthlySchedule(int intervalMonthly, DateTime startTime)
        {
            this._intervalMonthly = intervalMonthly;
            this._startTime = startTime;
        }



        private DateTime _startTime;
        private int _intervalMonthly;

        public override DateTime GetNextExecuteTime()
        {
            DateTime now = DateTime.Now;
            if (_startTime > now)
                return _startTime;
            else
                return now.AddMonths(_intervalMonthly);
        }
    }
}
