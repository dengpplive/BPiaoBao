using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.InsurancesMap
{
    public class InsuranceRecordMap : EntityTypeConfiguration<InsuranceRecord>
    {
        public InsuranceRecordMap()
        {
            this.Property(t => t.InsuranceNo).HasMaxLength(50);
            this.Property(t => t.CarrierCode).HasMaxLength(20);
            this.Property(t => t.CarrierName).HasMaxLength(50);
            this.Property(t => t.BussinessmanCode).HasMaxLength(20);
            this.Property(t => t.BussinessmanName).HasMaxLength(50);
            this.Property(t => t.InsuranceCompany).HasMaxLength(100);
        }
    }
}
