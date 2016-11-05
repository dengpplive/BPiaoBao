using BPiaoBao.DomesticTicket.Domain.Models.Policies;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class LocalPolicyRepository : BaseRepository<LocalPolicy>, ILocalPolicyRepository
    {
        public LocalPolicyRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {

        }
    }
}
