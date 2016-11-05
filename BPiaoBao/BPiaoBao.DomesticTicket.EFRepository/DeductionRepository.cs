using BPiaoBao.DomesticTicket.Domain.Models.Deduction;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.EFRepository
{
    /// <summary>
    /// 扣点组仓储实现
    /// </summary>
    public class DeductionRepository : BaseRepository<DeductionGroup>, IDeductionRepository
    {
        public DeductionRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {
            
        }
    }
}
