using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.ServiceModel;
using System.Web;

namespace BPiaoBao.Web.SupplierManager.Controllers.Helpers
{
    public static class CommunicateManager
    {
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
            catch (FaultException<CustomException> uex)
            {
                if (error != null)
                {
                    error(uex.Detail);
                }
                throw uex;
            }
            catch (FaultException fex)
            {
                if (error != null)
                    error(fex);
                throw fex;
            }
            catch (CommunicationException ex)
            {
                throw new CommunicationException("请求服务器失败，通信错误");
            }
            catch (System.TimeoutException ex)
            {
                throw new TimeoutException("请求服务器超时");
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                throw new Exception("网络异常");
            }
            catch (Exception ex)
            {
                Logger.WriteLog(LogType.ERROR, ex.ToString());
                throw new Exception("程序发生异常，请联系客服");
            }
        }
    }
}