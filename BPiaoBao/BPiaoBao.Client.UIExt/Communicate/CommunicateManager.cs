using System;
using System.ServiceModel;
using JoveZhao.Framework;
using BPiaoBao.Client.UIExt.Communicate;
using BPiaoBao.AppServices.Contracts.ServerMessages;

namespace BPiaoBao.Client.UIExt
{
    public static class CommunicateManager
    {
        private static volatile bool _isStoped;
        private static InstanceContext instanceContext = new InstanceContext(new PublishCallback());
        static void PushMessage()
        {
            DuplexChannelFactory<IPublisherService> duplex = new DuplexChannelFactory<IPublisherService>(instanceContext, typeof(IPublisherService).Name);
            try
            {
                IPublisherService client = duplex.CreateChannel();
                client.Subscriber(LoginInfo.Guid, LoginInfo.Code, LoginInfo.Account);
                while (_isStoped)
                {
                    System.Threading.Thread.Sleep(300000);
                    client.Subscriber(LoginInfo.Guid, LoginInfo.Code, LoginInfo.Account);
                }
            }
            catch (Exception e)
            {
                Logger.WriteLog(LogType.ERROR, "连接失败", e);
                System.Threading.Thread.Sleep(60000);
                duplex.Abort();
                PushMessage();
            }


        }
        public static void StartPushMessage()
        {

            System.Threading.Thread t = new System.Threading.Thread(PushMessage);
            _isStoped = true;
            t.IsBackground = true;
            t.Start();
        }
        public static void StopPushMessage()
        {
            _isStoped = false;
        }
        public static void Invoke<T>(Action<T> act, Action<Exception> error = null)
        {
            try
            {
                using (ChannelFactory<T> cf = new ChannelFactory<T>(typeof(T).Name))
                {
                    var client = cf.CreateChannel();
                    act(client);
                }
            }
            catch (CustomException uex)
            {
                if (error != null)
                {
                    error(uex);
                }
            }
            catch (FaultException<CustomException> uex)
            {
                if (error != null)
                {
                    error(uex.Detail);
                }
            }
            catch (FaultException fex)
            {
                if (error != null)
                    error(fex);
            }
            catch (CommunicationException ex)
            {
                Logger.WriteLog(LogType.WARN, ex.Message, ex);
#if DEBUG
                if (error != null)
                    error(ex);
                return;
#endif
                if (error != null)
                    error(new CommunicationException("请求服务器失败，通信错误"));
            }
            catch (TimeoutException ex)
            {
                Logger.WriteLog(LogType.WARN, ex.Message, ex);
#if DEBUG
                if (error != null)
                    error(ex);
                return;
#endif
                if (error != null)
                    error(new TimeoutException("请求服务器超时"));
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                Logger.WriteLog(LogType.WARN, ex.Message, ex);
#if DEBUG
                if (error != null)
                    error(ex);
                return;
#endif
                if (error != null)
                    error(new Exception("网络异常"));
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.Message, ex);
                if (error != null)
                    error(ex);
            }
        }
    }
}
