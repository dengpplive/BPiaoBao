using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class PointGroupMap : EntityTypeConfiguration<PlatformPointGroup>
    {
        public PointGroupMap()
        {
            this.HasKey(t => t.ID).Property(t => t.ID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            Property(t => t.OperatorAccount).HasMaxLength(20);
            Property(t => t.GroupName).HasMaxLength(50);
            Property(t => t.Remark).HasMaxLength(200);
        }
    }
}
