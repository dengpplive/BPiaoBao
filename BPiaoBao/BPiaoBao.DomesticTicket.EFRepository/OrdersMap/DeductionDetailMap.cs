using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class DeductionDetailMap:EntityTypeConfiguration<DeductionDetail>
    {
        public DeductionDetailMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.Name).HasMaxLength(50);
        }
    }
}
