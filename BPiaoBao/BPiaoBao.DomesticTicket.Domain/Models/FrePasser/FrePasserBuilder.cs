using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.Domain.Models.FrePasser
{
    public class FrePasserBuilder : IAggregationBuilder
    {
        public FrePasser CreateFrePasser()
        {
            return new FrePasser();
        }
    }
}
