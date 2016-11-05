using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.FrePasser;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class FrePasserRepository : BaseRepository<FrePasser>, IFrePasserRepository
    {
        public FrePasserRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {
        }
    }
}
