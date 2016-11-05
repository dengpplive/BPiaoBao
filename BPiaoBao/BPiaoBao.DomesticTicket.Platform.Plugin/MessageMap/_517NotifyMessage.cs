using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platform.Plugin.MessageMap
{
    public class _517NotifyMessage
    {
        public int NotifyType { get; set; }
        public string Sign { get; set; }
        public Message Message { get; set; }
    }
    public class Message
    {
        public string OrderId { get; set; }
        public int OrderState { get; set; }
        public decimal ReturnTotalMoney { get; set; }
        public int OrderSource { get; set; }
        public string RefuseMsg { get; set; }
        public List<Notify_Passenger> Passengers { get; set; }
    }
    public class Notify_Passenger
    {
        public string TicketNo { get; set; }
        public string PassengerName { get; set; }
        public string CardID { get; set; }
        public decimal ReturnMoney { get; set; }
    }
}
