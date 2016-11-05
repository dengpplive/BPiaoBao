using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class LoginLogMap : EntityTypeConfiguration<LoginLog>
    {
        public LoginLogMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.Code).HasMaxLength(20);
            this.Property(t=>t.Account).HasMaxLength(50);
            this.Property(t => t.LoginIP).HasMaxLength(100);
        }
    }
}
