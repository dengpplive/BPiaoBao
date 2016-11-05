using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class CoordinationLogMap : EntityTypeConfiguration<CoordinationLog>
    {
        public CoordinationLogMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Content).HasMaxLength(500);
            this.Property(t => t.OperationPerson).HasMaxLength(50);
        }
    }
}
