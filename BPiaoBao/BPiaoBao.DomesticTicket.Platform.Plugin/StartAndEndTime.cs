using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    public class StartAndEndTime
    {
        public StartAndEndTime()
        {
            this.StartTime = "00:00";
            this.EndTime = "00:00";
        }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public override string ToString()
        {
            return StartTime + "-" + EndTime;
        }
    }
}
