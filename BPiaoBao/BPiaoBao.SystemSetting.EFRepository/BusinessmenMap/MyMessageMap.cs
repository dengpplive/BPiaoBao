using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class MyMessageMap : EntityTypeConfiguration<MyMessage>
    {
        public MyMessageMap()
        {
            this.Property(t => t.Code).HasMaxLength(20);
            this.Property(t => t.Title).HasMaxLength(200);
        }
    }
}
