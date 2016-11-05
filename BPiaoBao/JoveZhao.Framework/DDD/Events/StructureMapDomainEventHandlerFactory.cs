using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.DDD.Events
{
    public class StructureMapDomainEventHandlerFactory : IDomainEventHandlerFactory
    {
        public IEnumerable<IDomainEventHandler<T>> GetDomainEventHandlersFor<T>() where T : IDomainEvent
        {
            
           return ObjectFactory.GetAllInstances<IDomainEventHandler<T>>();
        }
    }
}
