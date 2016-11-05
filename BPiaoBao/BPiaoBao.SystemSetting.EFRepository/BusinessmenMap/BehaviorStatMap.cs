using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;
using BPiaoBao.SystemSetting.Domain.Models.Behavior;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class BehaviorStatMap : EntityTypeConfiguration<BehaviorStat>
    {
        public BehaviorStatMap()
        {

            this.HasKey(t => t.Id);
            this.Property(t => t.BusinessmanCode).IsRequired().HasMaxLength(20);
            this.Property(t => t.BusinessmanName).IsRequired().HasMaxLength(50);
            this.Property(t => t.BusinessmanType).IsRequired().HasMaxLength(20);
            this.Property(t => t.CarrierCode).HasMaxLength(20);

        }
    }
}
