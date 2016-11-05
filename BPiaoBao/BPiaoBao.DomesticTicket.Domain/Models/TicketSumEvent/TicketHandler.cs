using BPiaoBao.Common;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.DDD.Events;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.TicketSumEvent
{
    public class TicketHandler : IDomainEventHandler<TicketEvent>
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());

        public void Handle(TicketEvent domainEvent)
        {
            if (domainEvent == null || domainEvent.TicketSums==null)
            {
                Logger.WriteLog(LogType.ERROR, "domainEvent is null");
                return;
            }
            domainEvent.TicketSums.ForEach(p =>
            {
                unitOfWorkRepository.PersistCreationOf(p);
            });
            unitOfWork.Commit();
        }
    }
}
