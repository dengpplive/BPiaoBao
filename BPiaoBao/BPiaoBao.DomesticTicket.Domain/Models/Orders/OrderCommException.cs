using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class OrderCommException : CustomException
    {
        public OrderCommException(string message) : base(100, message) { }
    }
}
