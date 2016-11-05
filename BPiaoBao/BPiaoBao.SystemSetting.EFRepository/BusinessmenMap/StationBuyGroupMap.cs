using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class StationBuyGroupMap : EntityTypeConfiguration<StationBuyGroup>
    {
        public StationBuyGroupMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.Color).HasMaxLength(200);
            this.Property(t => t.Description).HasMaxLength(400);
            this.Property(t => t.GroupName).HasMaxLength(200);
            this.Property(t => t.LastOperatorUser).HasMaxLength(50);
        }
    }
}
