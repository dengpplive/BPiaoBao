using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class AfterSaleOrderMap : EntityTypeConfiguration<AfterSaleOrder>
    {
        public AfterSaleOrderMap()
        {
            this.HasKey(t => t.Id);
            this.Ignore(t => t.AfterSaleType);
            this.Property(t => t.Description).HasMaxLength(1000);
            this.Property(t => t.Reason).HasMaxLength(1000);
            this.Property(t => t.CreateMan).HasMaxLength(20);
            this.Property(t => t.OrderID).HasMaxLength(20).HasColumnName("Order_OrderId");
            this.Property(t => t.ProcessStatus).IsConcurrencyToken();
            this.Property(t => t.StationBuyGroupID).HasMaxLength(40);
            Map<AnnulOrder>(m => m.Requires("Type").HasValue("Annul"));
            Map<BounceOrder>(m => m.Requires("Type").HasValue("Bounce"));
            Map<ChangeOrder>(m => m.Requires("Type").HasValue("Change"));
            Map<ModifyOrder>(m => m.Requires("Type").HasValue("Modify"));
        }
    }
}
