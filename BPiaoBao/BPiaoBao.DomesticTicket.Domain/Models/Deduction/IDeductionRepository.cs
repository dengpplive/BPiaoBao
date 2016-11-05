using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Deduction
{
    /// <summary>
    /// 扣点组仓储
    /// </summary>
    public interface IDeductionRepository : IRepository<DeductionGroup>
    {
    }
}
