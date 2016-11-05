using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.InsurancesMap
{
    public class InsurancePurchaseByBussinessmanMap:EntityTypeConfiguration<InsurancePurchaseByBussinessman>
    {
        public InsurancePurchaseByBussinessmanMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.PayNo).HasMaxLength(50);
            this.Property(t => t.OutTradeNo).HasMaxLength(50);
            this.Property(t => t.BusinessmanCode).HasMaxLength(20);
            this.Property(t => t.BusinessmanName).HasMaxLength(50);
            this.Property(t => t.CarrierCode).HasMaxLength(20);
            this.Property(t => t.CarrierName).HasMaxLength(50);
        }
    }
}
