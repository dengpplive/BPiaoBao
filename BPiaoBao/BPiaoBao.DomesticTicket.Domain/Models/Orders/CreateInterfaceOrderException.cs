using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    /// <summary>
    /// 生成接口订单异常
    /// </summary>
    public class CreateInterfaceOrderException : CustomException
    {
        public CreateInterfaceOrderException(string message) : base(700, message) { }
    }
}
