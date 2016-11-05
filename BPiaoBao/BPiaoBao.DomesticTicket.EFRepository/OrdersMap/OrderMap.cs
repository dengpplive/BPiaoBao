using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class OrderMap : EntityTypeConfiguration<Order>
    {
        public OrderMap()
        {
            this.HasKey(t => t.OrderId);
            this.Property(t => t.OrderId).HasMaxLength(20);
            this.Property(t => t.PnrCode).IsRequired().HasMaxLength(10);
            this.Property(t => t.BigCode).IsRequired().HasMaxLength(10);
            this.Property(t => t.BusinessmanCode).HasMaxLength(20);
            this.Property(t => t.OperatorAccount).HasMaxLength(20);
            this.Property(t => t.LockAccount).HasMaxLength(20);
            this.Property(t => t.OutOrderId).HasMaxLength(50);
            this.Property(t => t.Remark).HasMaxLength(200);
            this.Property(t => t.PnrContent).HasMaxLength(2000);
            this.Property(t => t.BusinessmanName).HasMaxLength(20);
            this.Property(t => t.IssueTicketTime).IsOptional();
            this.Property(t => t.OrderStatus).IsConcurrencyToken();
            this.Property(t => t.StationBuyGroupID).HasMaxLength(40);
                
        }
    }
}
