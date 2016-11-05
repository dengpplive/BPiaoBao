using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class PayInterfaceOrderException : CustomException
    {
        public PayInterfaceOrderException(string message) : base(800, message) { }
    }
}
