using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{
   public class InsuranceConfigBuilder : IAggregationBuilder
    {
       public InsuranceConfig CreateInsuranceConfig()
       {
           return new InsuranceConfig();
       }
    }
}
