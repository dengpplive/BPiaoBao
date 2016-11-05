using BPiaoBao.AppServices.Contracts.ServerMessages;
using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using System;

namespace BPiaoBao.Client.UIExt.Communicate
{
    public class PublishCallback : IPublisherEvents
    {
        //public PublishCallback()
        //{

        //}

        public void Notify(EnumPushCommands pushCommand, string content, params object[] param)
        {
            try
            {
                UIManager.InvokeCommand(pushCommand, content, param);
            }
            catch(Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "回调处理异常",e);
            }
        }
    }
}
