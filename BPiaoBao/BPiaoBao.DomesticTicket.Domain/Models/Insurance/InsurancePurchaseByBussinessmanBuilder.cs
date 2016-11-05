using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Insurance
{
    public class InsurancePurchaseByBussinessmanBuilder : IAggregationBuilder
    {
        public InsurancePurchaseByBussinessman CreateInsurancePurchaseByBussinessman()
        {
            return new InsurancePurchaseByBussinessman();
        }
    }
}
