using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class TicketMap : EntityTypeConfiguration<Ticket>
    {
        public TicketMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.OrderID).HasMaxLength(20);
            this.Property(t => t.PNR).HasMaxLength(10);
            this.Property(t => t.PassengerName).HasMaxLength(50);
            this.Property(t => t.FlightNum).HasMaxLength(50);
            this.Property(t => t.Voyage).HasMaxLength(50);
            this.Property(t => t.Seat).HasMaxLength(10);
            this.Property(t => t.TicketState).HasMaxLength(10);
            this.Map<Ticket_Carrier>(m => m.ToTable("TicketCarrier"))
                .Map<Ticket_Supplier>(m => m.ToTable("TicketSupplier"))
                .Map<Ticket_Buyer>(m => m.ToTable("TicketBuyer"))
                .Map<Ticket_Conso>(m => m.ToTable("TicketConso"));
        }
    }
}
