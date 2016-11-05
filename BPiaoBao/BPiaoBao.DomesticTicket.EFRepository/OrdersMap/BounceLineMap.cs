using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using System;
using System.Collections.Generic;
using System.Data.Entity.ModelConfiguration;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository.OrdersMap
{
    public class BounceLineMap :  EntityTypeConfiguration<BounceLine>
    {
        public BounceLineMap()
        {
            this.HasKey(t => t.ID);
        }
    }
}
