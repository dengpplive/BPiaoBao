using BPiaoBao.SystemSetting.Domain.Models.SMS;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class SMSMap:EntityTypeConfiguration<SMS>
    {
        public SMSMap()
        {
            this.HasKey(t => t.BusinessmanCode);
            this.HasRequired(t => t.Businessman).WithRequiredDependent(t=>t.SMS);
        }
    }
}
