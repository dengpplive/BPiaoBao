using BPiaoBao.DomesticTicket.Domain.Models.Refunds;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class PlatformRefundOrderRepository : BaseRepository<PlatformRefundOrder>, IPlatformRefundOrderRepository
    {
        public PlatformRefundOrderRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr) { }
    }
}
