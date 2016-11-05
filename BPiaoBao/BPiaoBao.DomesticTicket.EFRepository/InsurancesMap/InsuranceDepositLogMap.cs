using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;

namespace BPiaoBao.DomesticTicket.EFRepository.InsurancesMap
{
    public class InsuranceDepositLogMap : EntityTypeConfiguration<InsuranceDepositLog>
    {
        public InsuranceDepositLogMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.PayNo).HasMaxLength(50);
            this.Property(t => t.OutTradeNo).HasMaxLength(50);
            this.Property(t => t.BusinessmanCode).HasMaxLength(20);
            this.Property(t => t.BusinessmanName).HasMaxLength(50);
        }
    }
}
