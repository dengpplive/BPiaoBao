using BPiaoBao.DomesticTicket.Domain.Models.FrePasser;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class FrePasserMap : EntityTypeConfiguration<FrePasser>
    {
        public FrePasserMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).HasMaxLength(50);
            this.Property(t => t.PasserType).HasMaxLength(10);
            this.Property(t => t.CertificateType).HasMaxLength(20);
            this.Property(t => t.CertificateNo).HasMaxLength(50);
            this.Property(t => t.Mobile).HasMaxLength(20);
            this.Property(t => t.AirCardNo).HasMaxLength(50);
            this.Property(t => t.Remark).HasMaxLength(200);
            this.Property(t => t.BusinessmanCode).HasMaxLength(20); 
        }
    }
}
