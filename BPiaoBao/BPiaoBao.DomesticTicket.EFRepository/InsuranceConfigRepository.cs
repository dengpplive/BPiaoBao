using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class InsuranceConfigRepository : BaseRepository<InsuranceConfig>, IInsuranceConfigRepository
    {
        public InsuranceConfigRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {
        }
    }
}
