using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using System.Text;

namespace BPiaoBao.AppServices
{
   public class CustomErrorHandler:IErrorHandler
    {
        public bool HandleError(Exception error)
        {
            return true;
        }

        public void ProvideFault(Exception error, System.ServiceModel.Channels.MessageVersion version, ref System.ServiceModel.Channels.Message fault)
        {
            var ex = error as CustomException;
            if (ex != null)
            {
               // FaultException<string> fe = new FaultException<string>(ex.Message, new FaultReason(ex.Message));
                FaultException fe=new FaultException(new FaultReason(ex.Message),new FaultCode(ex.ErrorCode.ToString()));

                MessageFault messagefault = fe.CreateMessageFault();
                fault = Message.CreateMessage(version, messagefault, "http://www.microsoft.com/");
                Logger.WriteLog(LogType.ERROR,ex.Message);
            }
            else
            {
                Logger.WriteLog(LogType.ERROR, error.Message, error);
                var cex = new CustomException(300, "发生系统异常，请联系客服");
                FaultException fe = new FaultException(new FaultReason(cex.Message), new FaultCode(cex.ErrorCode.ToString()));
                MessageFault messagefault = fe.CreateMessageFault();
                fault = Message.CreateMessage(version, messagefault, "http://www.microsoft.com/");
            }
        }
    }
}
