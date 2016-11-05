using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.ModelConfiguration;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class SkyWayMap:EntityTypeConfiguration<SkyWay>
    {
        public SkyWayMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.FromCityCode).HasMaxLength(10);
            this.Property(t => t.ToCityCode).HasMaxLength(10);
            this.Property(t => t.FlightNumber).HasMaxLength(10);
            this.Property(t => t.CarrayCode).HasMaxLength(10);
            this.Property(t => t.Seat).HasMaxLength(10);
            this.Property(t => t.FromTerminal).HasMaxLength(10);
            this.Property(t => t.ToTerminal).HasMaxLength(10);
            this.Property(t => t.FlightModel).HasMaxLength(10);
        }
    }
}
