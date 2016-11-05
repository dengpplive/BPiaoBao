using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
    public class BuyerMap : EntityTypeConfiguration<Buyer>
    {
        public BuyerMap()
        {
            this.Property(t => t.Label).HasColumnName("BuyerLable").HasMaxLength(200);
            this.Property(t => t.CarrierCode).HasColumnName("BuyerCarrierCode");
            this.Property(t => t.StationBuyGroupID).HasMaxLength(40);
        }
    }
}
