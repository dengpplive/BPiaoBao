using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class DefaultPolicyMap : EntityTypeConfiguration<DefaultPolicy>
    {
        public DefaultPolicyMap()
        {
            this.Property(t => t.CarrayCode).HasMaxLength(10);
        }
    }
}
