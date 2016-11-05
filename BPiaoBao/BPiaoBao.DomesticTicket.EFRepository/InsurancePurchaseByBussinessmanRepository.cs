using BPiaoBao.DomesticTicket.Domain.Models.Insurance;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    public class InsurancePurchaseByBussinessmanRepository : BaseRepository<InsurancePurchaseByBussinessman>, IInsurancePurchaseByBussinessmanRepository
    {
        public InsurancePurchaseByBussinessmanRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {
        }
    }
}
