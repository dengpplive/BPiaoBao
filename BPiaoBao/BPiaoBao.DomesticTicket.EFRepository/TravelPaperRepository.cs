using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.TravelPaper;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class TravelPaperRepository : BaseRepository<TravelPaper>, ITravelPaperRepository
    {
        public TravelPaperRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr) { }
    }
}
