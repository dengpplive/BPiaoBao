using BPiaoBao.DomesticTicket.Domain.Models.TravelPaper;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class TravelPaperLogMap:EntityTypeConfiguration<TravelPaperLog>
    {
        public TravelPaperLogMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.OperationPerson).HasMaxLength(20);
            this.Property(t => t.OperationType).HasMaxLength(20);
        }
    }
}
