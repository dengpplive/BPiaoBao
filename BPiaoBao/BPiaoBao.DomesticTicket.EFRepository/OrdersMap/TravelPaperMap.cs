using BPiaoBao.DomesticTicket.Domain.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.TravelPaper;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class TravelPaperMap : EntityTypeConfiguration<TravelPaper>
    {
        public TravelPaperMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.TicketNumber).HasMaxLength(15);
            this.Property(t => t.TripNumber).HasMaxLength(10);
            this.Property(t => t.UseOffice).HasMaxLength(6);
            this.Property(t => t.IataCode).HasMaxLength(8);
            this.Property(t => t.TicketCompanyName).HasMaxLength(200);

        }
    }
}
