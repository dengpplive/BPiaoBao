using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
     public class NotTakeOffTicketDto
    {
        public string Code { get; set; }
        public int TicketCount { get; set; }
        public decimal TicketMoney { get; set; }
    }
}
