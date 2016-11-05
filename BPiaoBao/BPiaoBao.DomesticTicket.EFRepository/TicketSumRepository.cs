using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class TicketSumRepository : BaseRepository<TicketSum>, ITicketSumRepository
    {
        public TicketSumRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {

        }
        public void Test(DateTime d, DateTime d2)
        {
            var orderList = this.DbContext.Ticket.OfType<Ticket_Conso>().Where(p => p.TicketState.Equals("出票") && p.PolicyFrom.Equals("BaiTuo") && p.CreateDate >= d && p.CreateDate <= d2).GroupBy(x => x.OrderID).Select(p => p.Key).ToList();
            orderList.ForEach(m =>
            {
                var PaidMoney = this.DbContext.OrderPays.Where(p => p.OrderId.Equals(m)).Select(y => y.PaidMoney).FirstOrDefault();
                var list = this.DbContext.Ticket.OfType<Ticket_Conso>().Where(y => y.TicketState.Equals("出票") && y.PolicyFrom.Equals("BaiTuo") && y.OrderID.Equals(m)).ToList();
                int passengerCount = list.Count;
                decimal recordPaidMoney = 0;
                int currentPassengerIndex = 1;
                list.ForEach(n =>
                {
                    decimal tempMoney = GetPaidMoney(PaidMoney, recordPaidMoney, passengerCount, currentPassengerIndex);
                    n.PaidMoney = tempMoney;
                    recordPaidMoney += tempMoney;
                    currentPassengerIndex++;
                });
            });
            this.DbContext.SaveChanges();

        }
        private decimal GetPaidMoney(decimal paidMoney, decimal recordPaidMoney, int passengerCount, int currentPassengerIndex)
        {
            if (passengerCount == currentPassengerIndex)
                return paidMoney - recordPaidMoney;
            return Math.Round((decimal)paidMoney / passengerCount, 2);
        }

    }
    public class TicketRepository : BaseRepository<Ticket>, ITicketRepository
    {
        public TicketRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {

        }
    }
}
