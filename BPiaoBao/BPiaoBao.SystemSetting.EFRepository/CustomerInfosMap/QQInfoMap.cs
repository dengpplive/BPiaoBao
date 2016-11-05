using BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.CustomerInfosMap
{
    public class QQInfoMap : EntityTypeConfiguration<QQInfo>
    {
        public QQInfoMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.QQ).HasMaxLength(15);
            this.Property(t => t.Description).HasMaxLength(50);
        }
    }
}
