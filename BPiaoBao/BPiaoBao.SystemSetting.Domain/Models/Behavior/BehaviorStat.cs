using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.SystemSetting.Domain.Models.Behavior
{
    public class BehaviorStat : EntityBase, IAggregationRoot
    {

        public int Id { get; set; }
        public string BusinessmanCode { get; set; }

        public string BusinessmanName { get; set; }

        public string BusinessmanType { get; set; }

        public string CarrierCode { get; set; }

        public int BehaviorOperate { get; set; }

        public DateTime OpDateTime { get; set; }

        public string ContactName { get; set; }

        public string OperatorName { get; set; }

        protected override string GetIdentity()
        {
            return this.Id.ToString();
        }


    }
}
