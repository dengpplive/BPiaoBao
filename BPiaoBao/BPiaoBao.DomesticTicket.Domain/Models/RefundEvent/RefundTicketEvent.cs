using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.RefundEvent
{
    public class RefundTicketEvent : IDomainEvent
    {
        /// <summary>
        /// 售后订单ID
        /// </summary>
        public int SaleOrderId { get; set; }
    }
}
