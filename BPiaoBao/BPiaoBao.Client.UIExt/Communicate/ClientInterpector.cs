using JoveZhao.Framework;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace BPiaoBao.Client.UIExt.Communicate
{
    public class ClientInterpector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            Logger.WriteLog(LogType.DEBUG, LoginInfo.Token);
            var tokenHeader = MessageHeader.CreateHeader("Token", "http://tempuri.org/", LoginInfo.Token);
            request.Headers.Add(tokenHeader);
            return null;
        }
    }
}
