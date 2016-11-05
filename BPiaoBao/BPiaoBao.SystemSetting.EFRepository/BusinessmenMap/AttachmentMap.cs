using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class AttachmentMap:EntityTypeConfiguration<Attachment>
    {
        public AttachmentMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.Name).HasMaxLength(200);
            this.Property(t => t.Url).HasMaxLength(200);
        }
    }
}
