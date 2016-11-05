using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.ServerMessages
{
    [ServiceContract(CallbackContract = typeof(IPublisherEvents))]
    public interface IPublisherService
    {
        /// <summary>
        /// 订阅
        /// </summary>
        /// <param name="codeAccount"></param>
        [OperationContract]
        void Subscriber(Guid guid, string code, string account);
        /// <summary>
        /// 取消订阅
        /// </summary>
        /// <param name="codeAccount"></param>
        [OperationContract]
        void UnSubscriber(Guid guid);
    }
}
