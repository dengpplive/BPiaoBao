using BPiaoBao.SystemSetting.Domain.Models.SMS;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class GiveDetailMap : EntityTypeConfiguration<GiveDetail>
    {
        public GiveDetailMap()
        {
            this.Property(t => t.GiveCode).HasMaxLength(20);
            this.Property(t => t.GiveName).HasMaxLength(100);
            this.Property(t => t.Remark).HasMaxLength(2000);
        }
    }
}
