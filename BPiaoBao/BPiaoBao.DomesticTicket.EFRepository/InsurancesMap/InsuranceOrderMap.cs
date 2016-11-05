using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.InsurancesMap
{
    public class InsuranceOrderMap : EntityTypeConfiguration<InsuranceOrder>
    {
        public InsuranceOrderMap()
        {
            this.Property(t => t.OrderId).HasMaxLength(20);
        }
    }
}
