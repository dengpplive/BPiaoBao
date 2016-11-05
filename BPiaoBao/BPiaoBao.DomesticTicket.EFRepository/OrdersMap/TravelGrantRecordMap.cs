using BPiaoBao.DomesticTicket.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.TravelPaper;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class TravelGrantRecordMap : EntityTypeConfiguration<TravelGrantRecord>
    {
        public TravelGrantRecordMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.BusinessmanCode).HasMaxLength(50);
            this.Property(t => t.BusinessmanName).HasMaxLength(100);
            this.Property(t => t.UseBusinessmanCode).HasMaxLength(50);
            this.Property(t => t.UseBusinessmanName).HasMaxLength(100);
        }
    }
}
