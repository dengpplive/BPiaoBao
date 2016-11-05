using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class PassengerMap : EntityTypeConfiguration<Passenger>
    {
        public PassengerMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.PassengerName).HasMaxLength(100);
            this.Property(t => t.CardNo).HasMaxLength(50);
            this.Property(t => t.ABFee).IsOptional();
            this.Property(t => t.RQFee).IsOptional();
            this.Property(t => t.SeatPrice).IsOptional();
            this.Property(t => t.Mobile).HasMaxLength(20);
            this.Property(t => t.TicketNumber).HasMaxLength(20);
            this.Property(t => t.TravelNumber).HasMaxLength(20);
        }
    }
}
