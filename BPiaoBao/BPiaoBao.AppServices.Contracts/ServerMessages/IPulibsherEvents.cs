using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.Contracts.ServerMessages
{
    public interface IPublisherEvents
    {
        /// <summary>
        /// 消息通知
        /// </summary>
        [OperationContract(IsOneWay = true)]
        void Notify(EnumPushCommands pushCommand, string content, params object[] param);
    }
}
