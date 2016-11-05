using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class AfterSaleOrderRepository : BaseRepository<AfterSaleOrder>, IAfterSaleOrderRepository
    {
        public AfterSaleOrderRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr) { }
    }
}
