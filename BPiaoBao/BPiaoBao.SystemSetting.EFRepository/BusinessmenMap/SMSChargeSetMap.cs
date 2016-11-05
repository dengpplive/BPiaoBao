using BPiaoBao.SystemSetting.Domain.Models.SMS;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class SMSChargeSetMap:EntityTypeConfiguration<SMSChargeSet>
    {
        public SMSChargeSetMap()
        {
            this.Property(t => t.Price).HasPrecision(18, 3);
        }
    }
}
