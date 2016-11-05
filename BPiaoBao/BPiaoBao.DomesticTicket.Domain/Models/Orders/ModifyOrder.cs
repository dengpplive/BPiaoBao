using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class ModifyOrder : AfterSaleOrder
    {
        public override string AfterSaleType
        {
            get { return "其它修改"; }
        }
    }
}
