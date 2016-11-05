using BPiaoBao.SystemSetting.Domain.Models.SMS;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class BuyDetailMap : EntityTypeConfiguration<BuyDetail>
    {
        public BuyDetailMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.Name).HasMaxLength(20);
            this.Property(t => t.PayNo).HasMaxLength(50);
            this.Property(t => t.PayTime).IsOptional();
            this.Property(t => t.Name).HasMaxLength(20);
            this.Property(t => t.OutPayNo).HasMaxLength(50);
        }
    }
}
