using BPiaoBao.SystemSetting.Domain.Models.Notice;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class NoticeAttachmentMap : EntityTypeConfiguration<NoticeAttachment>
    {
        public NoticeAttachmentMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).HasMaxLength(200);
            this.Property(t => t.Url).HasMaxLength(500);
        }
    }
}
