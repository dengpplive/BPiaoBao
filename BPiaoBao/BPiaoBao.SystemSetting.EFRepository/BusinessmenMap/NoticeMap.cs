using BPiaoBao.SystemSetting.Domain.Models.Notice;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class NoticeMap : EntityTypeConfiguration<Notice>
    {
        public NoticeMap()
        {
            this.Property(t => t.Title).HasMaxLength(200);
        }
    }
}
