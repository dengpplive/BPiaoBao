
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JoveZhao.Framework.ScheduleTask
{
    public class ScheduleTaskServices
    {
        public static void RegisterTask(ITask task, Schedule schedule)
        {
            _list.Add(new TaskObject()
            {
                Key = schedule.GetNextExecuteTime(),
                Value = new Tuple<ITask, Schedule>(task, schedule)
            });
        }

        public static void Start()
        {
            _setNext();
        }

        public static void Stop()
        {
            _timer.Change(TimeSpan.MaxValue, TimeSpan.MaxValue);
        }

        public static void LoadByConfig()
        {
            throw new System.NotImplementedException();
        }


        static void _setNext()
        {
            var lastTime = _list.Min(p => p.Key);
            if (lastTime < DateTime.Now)
                _timer.Change(0, Timeout.Infinite);
            else
                _timer.Change(lastTime - DateTime.Now, TimeSpan.Parse("24:00:00"));

        }
        static void _callBack(object state)
        {
            var ts = _list.Min(p => p.Key);

            _list.Where(p => p.Key == ts).ToList().ForEach(p =>
            {
                ThreadPool.QueueUserWorkItem(q =>
                {

                    try
                    {
                        p.Value.Item1.Execute();
                    }
                    catch (Exception e)
                    {
                        Logger.WriteLog(LogType.ERROR, "定时任务" + p.Value.Item1.TaskName, e);
                    }


                });
                _list.Remove(p);
                RegisterTask(p.Value.Item1, p.Value.Item2);
            });
            _setNext();
        }
        static List<TaskObject> _list = new List<TaskObject>();
        static Timer _timer = new Timer(_callBack, null, Timeout.Infinite, Timeout.Infinite);
    }

    public class TaskObject
    {
        public DateTime Key { get; set; }
        public Tuple<ITask, Schedule> Value { get; set; }
    }

}
