using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class DeductionRuleMap : EntityTypeConfiguration<DeductionRule>
    {
        public DeductionRuleMap()
        {
        }
    }
}
