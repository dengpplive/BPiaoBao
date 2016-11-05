using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class CarrierSettingMap : EntityTypeConfiguration<CarrierSetting>
    {
        public CarrierSettingMap()
        {
            this.HasKey(t => t.ID);
            this.Property(t => t.PrintNo).HasMaxLength(20);
            this.Property(t => t.CarrayCode).HasMaxLength(10);
            this.Property(t => t.CPOffice).HasMaxLength(10);
            this.Property(t => t.YDOffice).HasMaxLength(10);

        }
    }
}
