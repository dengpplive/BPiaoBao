
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.ScheduleTask

{
    public interface ITask
    {
        string TaskName { get; }
        bool Execute();
        

    }

}