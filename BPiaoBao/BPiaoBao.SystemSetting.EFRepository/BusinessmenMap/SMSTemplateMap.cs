using BPiaoBao.SystemSetting.Domain.Models.SMS;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class SMSTemplateMap : EntityTypeConfiguration<SMSTemplate>
    {
        public SMSTemplateMap()
        {
            this.Property(t => t.TemplateName).HasMaxLength(50);
            this.Property(t => t.TemplateContents).HasMaxLength(1000);
            this.Property(t => t.CreateName).HasMaxLength(50);
        }
    }
}
