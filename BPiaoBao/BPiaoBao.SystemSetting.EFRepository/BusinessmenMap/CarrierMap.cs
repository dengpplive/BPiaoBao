using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class CarrierMap : EntityTypeConfiguration<Carrier>
    {
        public CarrierMap()
        {
            this.Property(t => t.Label).HasColumnName("CarrierLable").HasMaxLength(200);
            this.Property(t => t.Rate).HasPrecision(18, 3);
            this.Property(t => t.RemoteRate).HasPrecision(18, 3);
            Property(t => t.BuyerRemotoPolicySwich).IsOptional();
            Property(t => t.ForeignRemotePolicySwich).IsOptional();
            Property(t => t.InterfacePolicySwitch).IsOptional();
            Property(t => t.LocalPolicySwitch).IsOptional();
            Property(t => t.ShowLocalCSCSwich).IsOptional();
        }
    }
}
