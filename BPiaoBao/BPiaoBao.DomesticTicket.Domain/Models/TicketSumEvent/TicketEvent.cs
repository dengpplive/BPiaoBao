using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent
{
    public class TicketEvent : IDomainEvent
    {
        
        public List<TicketSum> TicketSums { get; set; }
    }
}
