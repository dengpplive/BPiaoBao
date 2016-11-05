using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Controllers.Helpers
{
    public class ClientInterpector : IClientMessageInspector
    {
        public void AfterReceiveReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        public object BeforeSendRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel)
        {
            var tokenHeader = MessageHeader.CreateHeader("Token", "http://tempuri.org/", HttpContext.Current.Request.Cookies["auth"].Values["token"]);
            request.Headers.Add(tokenHeader);
            return null;
        }
    }
}