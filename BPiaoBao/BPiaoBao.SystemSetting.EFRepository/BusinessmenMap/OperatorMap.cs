using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class OperatorMap : EntityTypeConfiguration<Operator>
    {
        public OperatorMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Account).HasMaxLength(20).IsRequired();
            this.Property(t => t.Password).HasMaxLength(50).IsRequired();
            this.Property(t => t.Realname).HasMaxLength(20);
            this.Property(t => t.Phone).HasMaxLength(20);
        }
    }
}
