using BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.PlatformPointGroupMap
{
    public class PlatformPointGroupRuleMap : EntityTypeConfiguration<PlatformPointGroupRule>
    {
        public PlatformPointGroupRuleMap()
        {
            this.HasKey(t => t.ID).Property(t => t.ID).IsRequired().HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);
            this.HasRequired(t => t.PlatformPointGroup).WithMany().HasForeignKey(t => t.PlatformPointGroupID);
        }
    }
}
