using BPiaoBao.SystemSetting.Domain.Models.SMS;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class SendDetailMap : EntityTypeConfiguration<SendDetail>
    {
        public SendDetailMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.Name).HasMaxLength(20);
            this.Property(t => t.Content).HasMaxLength(900);
            this.Property(t => t.ReceiveNum).HasMaxLength(20);
            this.Property(t => t.ReceiveName).HasMaxLength(20);
        }
    }
}
