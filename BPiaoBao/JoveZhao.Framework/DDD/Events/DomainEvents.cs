using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;

namespace JoveZhao.Framework.DDD.Events
{
    public static class DomainEvents
    {
        public static IDomainEventHandlerFactory DomainEventHandlerFactory
        {
            get
            {
                return new StructureMapDomainEventHandlerFactory();
            }
        }

        public static void Raise<T>(T domainEvent) where T : IDomainEvent
        {
            try
            {
                DomainEventHandlerFactory
                    .GetDomainEventHandlersFor<T>()
                    .ForEach(h =>
                    {
                        h.Handle(domainEvent);
                    });
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "时间错误", e);
            }
        }
    }
}
