using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository.BusinessmenMap
{
   public class BusinessmanMap:EntityTypeConfiguration<Businessman>
    {
       public BusinessmanMap()
       {

           this.HasKey(t => t.Code);
           this.Property(t => t.Code).HasMaxLength(20);
           this.Property(t => t.Name).IsRequired().HasMaxLength(50);
           this.Property(t => t.CashbagCode).HasMaxLength(20);
           this.Property(t => t.CashbagKey).HasMaxLength(50);
           this.Property(t => t.ContactWay.Contact).HasMaxLength(200).HasColumnName("Contact");
           this.Property(t => t.ContactWay.Tel).HasMaxLength(20).HasColumnName("Tel");
           this.Property(t => t.ContactWay.Address).HasMaxLength(200).HasColumnName("Address");
           this.Property(t => t.ContactWay.BusinessTel).HasMaxLength(200).HasColumnName("BusinessTel");
            Map<Buyer>(m => m.Requires("Type").HasValue("Buyer"));
            Map<Supplier>(m => m.Requires("Type").HasValue("Supplier"));
            Map<Carrier>(m => m.Requires("Type").HasValue("Carrier"));
           //this.Property(t => t.ContactName).HasMaxLength(50);
           //this.Property(t => t.Phone).HasMaxLength(20);
           
       }
    }
}
