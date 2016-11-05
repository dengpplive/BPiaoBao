using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class SupplierMap:EntityTypeConfiguration<Supplier>
    {
        public SupplierMap()
        {
            this.Property(t => t.CarrierCode).HasColumnName("SupplierCarrierCode");
            this.Property(t => t.SupRate).HasPrecision(18, 3);
            this.Property(t => t.SupRemoteRate).HasPrecision(18, 3);
            Property(t => t.SupLocalPolicySwitch).IsOptional();
            Property(t => t.SupRemotePolicySwitch).IsOptional();
        }
    }
}
