using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{
   public class InsuranceDepositLogBuilder:IAggregationBuilder
    {
       public InsuranceDepositLog CreateInsuranceDepositLog()
       {
           return new InsuranceDepositLog();
       }
    }
}
