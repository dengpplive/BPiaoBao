using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace BPiaoBao.AppServices
{
   public class IocInstanceProvider : IInstanceProvider
    {
       Type _serviceType;
       public IocInstanceProvider(Type serviceType)
       {
           this._serviceType = serviceType;
       }
        public object GetInstance(System.ServiceModel.InstanceContext instanceContext, System.ServiceModel.Channels.Message message)
        {
            return ObjectFactory.GetInstance(this._serviceType);
        }

        public object GetInstance(System.ServiceModel.InstanceContext instanceContext)
        {
            return GetInstance(instanceContext, null);
        }

        public void ReleaseInstance(System.ServiceModel.InstanceContext instanceContext, object instance)
        {
            if (instance is IDisposable)
                ((IDisposable)instance).Dispose();
        }
    }
}
