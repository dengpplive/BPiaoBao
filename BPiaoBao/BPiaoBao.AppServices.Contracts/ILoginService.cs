using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using BPiaoBao.AppServices.Contracts.ServerMessages;

namespace BPiaoBao.AppServices.Contracts
{
    [ServiceContract]
    public interface ILoginService 
    {
        /// <summary>
        /// 登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [OperationContract]
        string Login(string code, string account, string password);
        /// <summary>
        /// 注销
        /// </summary>
        [OperationContract]
        void Exist(string token);

       

    }
  
}
