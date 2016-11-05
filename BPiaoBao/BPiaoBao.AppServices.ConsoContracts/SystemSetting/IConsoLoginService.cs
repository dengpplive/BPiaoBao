using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting
{
    [ServiceContract]
    public interface IConsoLoginService
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [OperationContract]
        string ConsoLogin(string code, string account, string password, string loginIp); 
        /// <summary>
        /// 注销
        /// </summary>
        [OperationContract]
        void Exist(string token);
    }
}
