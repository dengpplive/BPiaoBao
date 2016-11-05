using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.SystemSetting.Domain.Models.Behavior
{
    public class BehaviorStatBuilder : IAggregationBuilder
    {
        public BehaviorStat CreateBehaviorStat()
        {
            return new BehaviorStat() { OpDateTime = DateTime.Now };
        }
    }
}
