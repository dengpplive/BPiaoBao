using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class AirChangeMap : EntityTypeConfiguration<AirChange>
    {
        public AirChangeMap()
        {
            this.Property(p => p.BusinessmanName).HasMaxLength(100);
            this.Property(p => p.Code).HasMaxLength(20);
            this.Property(p => p.PNR).HasMaxLength(10);
            this.Property(p => p.OrderId).HasMaxLength(20);
            this.Property(p => p.CTCT).HasMaxLength(50);
            this.Property(p => p.OfficeNum).HasMaxLength(20);
            this.Property(p => p.PassengerName).HasMaxLength(200);
        }
    }
}
