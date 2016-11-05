using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.AriChang
{
  public  class AirChangeBuilder : IAggregationBuilder
    {
        public AirChange CreateAirChange()
        {
            return new AirChange();
        }
    }
}
