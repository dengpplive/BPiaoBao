using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint
{
    public class PlatformGroupRuleBuilder : IAggregationBuilder
    {
        public PlatformPointGroupRule CreatePlatformPointGroupRule(PlatformPointGroupRule platformPointGroupRule = null)
        {
            if (platformPointGroupRule == null)
                platformPointGroupRule = new PlatformPointGroupRule();
            return null;
        }
        public PlatformPointGroup CreatePlatformPointGroup(PlatformPointGroup platformPointGroup = null)
        {
            if (platformPointGroup == null)
                platformPointGroup = new PlatformPointGroup();
            platformPointGroup.CreateDate = DateTime.Now; return null;
        }
    }
}
