using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.TravelPaper;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class TravelGrantRecordRepository : BaseRepository<TravelGrantRecord>, ITravelGrantRecordRepository
    {
        public TravelGrantRecordRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr) { }
    }
}
