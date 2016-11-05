using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class OrderPayMap: EntityTypeConfiguration<OrderPay>
    {
        public OrderPayMap()
        {
            this.HasKey(t => t.OrderId);
            this.Property(t => t.OrderId).HasMaxLength(20);
            this.HasRequired(t => t.Order).WithOptional(t =>t.OrderPay);
            this.Property(t => t.PaySerialNumber).HasMaxLength(50);
            this.Property(t => t.PaidSerialNumber).HasMaxLength(50);
            this.Property(t => t.PaidMoney).IsOptional();
            this.Property(t => t.PayMoney).IsOptional();
            this.Property(t => t.TradePoundage).IsOptional();
            this.Property(t => t.SystemFee).IsOptional();
            this.Property(t => t.PayDateTime).IsOptional();
            this.Property(t => t.PaidDateTime).IsOptional();
        }
    }
}
