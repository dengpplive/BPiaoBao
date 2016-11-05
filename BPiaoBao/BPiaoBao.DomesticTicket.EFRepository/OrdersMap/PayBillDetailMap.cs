using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class PayBillDetailMap:EntityTypeConfiguration<PayBillDetail>
    {
        public PayBillDetailMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.Name).HasMaxLength(50);
            this.Property(t => t.Code).HasMaxLength(50);
            this.Property(t => t.CashbagCode).HasMaxLength(50);
        }
    }
}
