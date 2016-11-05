using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class TicketConsoMap : EntityTypeConfiguration<Ticket_Conso>
    {
        public TicketConsoMap()
        {
            this.Property(t => t.Code).HasMaxLength(20);
            this.Property(t => t.PolicyFrom).HasMaxLength(20);
            this.Property(t => t.IssueTicketCode).HasMaxLength(20);
            this.Property(t => t.CarrierCode).HasMaxLength(20);
        }
    }
}
