using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using JoveZhao.Framework.DDD;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class InsuranceDepositLogRepository : BaseRepository<InsuranceDepositLog>,IInsuranceDepositLogRepository
    {
        public InsuranceDepositLogRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr)
        {
        }
    }
}
