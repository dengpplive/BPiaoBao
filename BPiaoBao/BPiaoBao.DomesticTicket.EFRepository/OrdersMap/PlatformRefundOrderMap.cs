using BPiaoBao.DomesticTicket.Domain.Models.Refunds;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class PlatformRefundOrderMap : EntityTypeConfiguration<PlatformRefundOrder>
    {
        public PlatformRefundOrderMap()
        {
            this.Property(t => t.RefundAmount).IsOptional();
            this.Property(t => t.RefundTime).IsOptional();
            this.Property(t => t.RefundOrderId).HasMaxLength(20);
            this.Property(t => t.Remark).HasMaxLength(500);
        }
    }
}
