using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.SystemSetting.Domain.Models.Behavior;
using JoveZhao.Framework.DDD;
using PiaoBao.BTicket.EFRepository;

namespace BPiaoBao.SystemSetting.EFRepository
{
    public class BehaviorStatRepository : BaseRepository<BehaviorStat>, IBehaviorStatRepository
    {
        public BehaviorStatRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr)
        {
            
        }

    }
}
