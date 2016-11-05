using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class LocalPolicyMap : EntityTypeConfiguration<LocalPolicy>
    {
        public LocalPolicyMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.ID).HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.Property(t => t.Code).HasMaxLength(20);
            Map<SpecialPolicy>(t => t.Requires("PolicyType").HasValue("Special"));
            Map<LocalNormalPolicy>(t => t.Requires("PolicyType").HasValue("Local"));
        }
    }
}
