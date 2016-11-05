using BPiaoBao.AppServices.Contracts.ServerMessages;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.ServerMessages
{
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class PublisherService : IPublisherService
    {
        public void Subscriber(Guid guid, string code, string account)
        {
            

            IPublisherEvents callback = OperationContext.Current.GetCallbackChannel<IPublisherEvents>();
            OperationContext.Current.Channel.Closing += Channel_Closing;


            MessagePushManager.Subscriber(guid, code, account, callback);
            //是否有离线消息
            if (MessagePushManager.OffLineMessages != null && MessagePushManager.OffLineMessages.Count != 0)
            {
                MessagePushManager.OffLineMessages.Where(p => p.Value.Code == code).ToList().ForEach(p =>
                {
                    callback.Notify(p.Value.Command, p.Value.MessageContent, null);
                    OffLineMessage of = null;
                    MessagePushManager.OffLineMessages.TryRemove(p.Key, out of);
                });
            }
        }

        void Channel_Closing(object sender, EventArgs e)
        {
            //IPublisherEvents callback = OperationContext.Current.GetCallbackChannel<IPublisherEvents>();
            //if (MessagePushManager.PushList.ContainsKey(callback))
            //{ 
            //    PushInfo pushInfo=null;
            //    MessagePushManager.PushList.TryRemove(callback, out pushInfo);
            //}
        }



        public void UnSubscriber(Guid guid)
        {
            MessagePushManager.UnSubscriber(guid);
        }
    }
}
