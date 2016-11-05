using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class PolicyMap: EntityTypeConfiguration<Policy>
    {
        public PolicyMap()
        {
            this.HasKey(t => t.OrderId);
            this.HasRequired(t => t.Order).WithOptional(t => t.Policy);
            this.Property(t => t.OrderId).HasMaxLength(20);
            this.Property(t => t.PolicyId).HasMaxLength(50);
            this.Property(t => t.PlatformCode).HasMaxLength(20);
            this.Property(t => t.PolicyPoint).IsOptional();
            this.Property(t => t.ReturnMoney).IsOptional();
            this.Property(t => t.Remark).HasMaxLength(500);
            this.Property(t => t.IsChangePNRCP).IsOptional();
            this.Property(t => t.IsSp).IsOptional();
            this.Property(t => t.PolicyType).HasMaxLength(10);
            this.Property(t => t.CPOffice).HasMaxLength(20);
            this.Property(t => t.AreaCity).HasMaxLength(10);
            this.Property(t => t.IssueSpeed).HasMaxLength(20);
            this.Property(t => t.Rate).HasPrecision(18, 3);
        }
    }
}
