using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{
    public class InsuranceOrderBuilder : IAggregationBuilder
    {
        public InsuranceOrder CreateInsuranceOrder()
        {
            return new InsuranceOrder();
        }
    }
}
