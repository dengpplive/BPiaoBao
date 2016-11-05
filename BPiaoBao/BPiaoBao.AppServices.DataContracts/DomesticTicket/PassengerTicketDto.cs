using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
   public class PassengerTicketDto
    {
       /// <summary>
       /// 乘机人名称
       /// </summary>
       public string Name { get; set; }
       /// <summary>
       /// 票号
       /// </summary>
       public string TicketNumber {get;set; } 
       /// <summary>
       /// 行程单号
       /// </summary>
       public string TravelNumber{ get; set; } 
       /// <summary>
       /// 状态(机票)
       /// </summary>
       public int TicketStatus {get; set;}
    }
}
