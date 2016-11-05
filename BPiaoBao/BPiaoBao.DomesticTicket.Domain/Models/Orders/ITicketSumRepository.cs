using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public interface ITicketSumRepository : IRepository<TicketSum>
    {
        void Test(DateTime d, DateTime d2);
    }
}
