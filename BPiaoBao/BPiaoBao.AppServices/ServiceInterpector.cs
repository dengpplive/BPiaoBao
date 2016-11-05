using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework;
using JoveZhao.Framework.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.ServiceModel;
using System.ServiceModel.Dispatcher;
using System.Text;
using BPiaoBao.Common;

namespace BPiaoBao.AppServices
{
    public class ServiceInterpector : IDispatchMessageInspector
    {
        #region IDispatchMessageInspector Members

        public object AfterReceiveRequest(ref System.ServiceModel.Channels.Message request, System.ServiceModel.IClientChannel channel, System.ServiceModel.InstanceContext instanceContext)
        {
            string token = string.Empty;
            if (request.Headers.MessageVersion.Envelope == EnvelopeVersion.None)
                token = System.ServiceModel.Web.WebOperationContext.Current.IncomingRequest.Headers["Token"];
            else
                token = GetHeaderValue("Token");

            AuthManager.SaveToken(token);
            CurrentUserInfo user = null;
            if (string.IsNullOrEmpty(token))
            {//控台的认证方式
                var key = GetHeaderValue("key");
                var suser = GetHeaderValue("user");
                CashbagConfigurationElement setting = SettingSection.GetInstances().Cashbag;
                if (key == "1439e30938174d75a2360e4e3d3c6094" && !string.IsNullOrEmpty(suser))
                {
                    user = new CurrentUserInfo() { CashbagCode = setting.CashbagCode, CashbagKey = setting.CashbagKey, OperatorName = suser, OperatorAccount = suser };
                }
            }
            else
            {
                Logger.WriteLog(LogType.DEBUG, token);
                var result = UserAuthResult<CurrentUserInfo>.Current(token);
                if (result != null)
                    user = result.UserInfo;
            }

            if (user != null)
                AuthManager.SaveUser(user);
            else
                throw new NotAuthException("账户登录超时，请重新登录");
            return null;
        }

        public void BeforeSendReply(ref System.ServiceModel.Channels.Message reply, object correlationState)
        {
        }

        private string GetHeaderValue(string name, string ns = "http://tempuri.org/")
        {
            var headers = OperationContext.Current.IncomingMessageHeaders;
            var index = headers.FindHeader(name, ns);
            if (index > -1)
                return headers.GetHeader<string>(index);
            else
                return null;
        }
        #endregion
    }
}
