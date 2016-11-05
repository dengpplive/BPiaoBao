using BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.CustomerInfosMap
{
    public class CustomerInfoMap:EntityTypeConfiguration<CustomerInfo>
    {
        public CustomerInfoMap()
        {
            this.HasKey(t => t.Id);
            this.Property(t => t.CustomPhone).HasMaxLength(20);
            this.Property(t => t.CarrierCode).HasMaxLength(20);
        }
    }
}
