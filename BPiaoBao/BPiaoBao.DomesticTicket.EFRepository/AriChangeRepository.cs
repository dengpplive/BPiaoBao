using BPiaoBao.DomesticTicket.Domain.Models.AriChang;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class AriChangeRepository : BaseRepository<AirChange>, IAriChangeRepository
    {
        public AriChangeRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {

        }
    }
}
