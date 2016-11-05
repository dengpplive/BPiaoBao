using BPiaoBao.AppServices.ConsoContracts.SystemSetting;
using BPiaoBao.AppServices.Contracts;
using BPiaoBao.AppServices.Contracts.ServerMessages;
using BPiaoBao.AppServices.DataContracts.SystemSetting;
using BPiaoBao.AppServices.SystemSetting;
using BPiaoBao.Common.DESCrypt;
using BPiaoBao.Common.Enums;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Models.Notice;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace BPiaoBao.AppServices
{
    public class LoginService : IConsoLoginService, ILoginService
    {
        public const string c_NeedSignOut = "请注意您的账号已在其它地方登录，您已被迫下线!";

        IBusinessmanRepository businessmanRepository;
        public LoginService(IBusinessmanRepository businessmanRepository)
        {
            this.businessmanRepository = businessmanRepository;
        }

        /// <summary>
        /// 买票宝登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("买票宝登录")]
        public string Login(string code, string account, string password)
        {
            if (string.IsNullOrEmpty(code))
                throw new CustomException(400, "请输入商户号!");
            if (string.IsNullOrEmpty(account))
                throw new CustomException(400, "请输入登录帐号!");
            if (string.IsNullOrEmpty(password))
                throw new CustomException(400, "请输入密码"); 
            RemoteEndpointMessageProperty endpoint = OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
            AuthService auth = new AuthService(this.businessmanRepository);
            var result = auth.Login(new LoginParames() { Code = code.Trim(), Password = password, Account = account.Trim(), LoginIP = endpoint.Address },
                identity =>
                {
                    MessagePushManager.Send(code, account, Common.Enums.EnumPushCommands.RepeatLogin, c_NeedSignOut);
                    //退出
                    Exit(identity);
                });
            BehaviorStatService.SaveLoginBehaviorStat(result.Token, EnumBehaviorOperate.LoginCount);
            return result.Token;
        }
         

        /// <summary>
        /// 卖票宝登录
        /// </summary>
        /// <param name="code"></param>
        /// <param name="account"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        [ExtOperationInterceptor("卖票宝登录")]
        public string ConsoLogin(string code, string account, string password, string loginIp)
        {
            if (string.IsNullOrEmpty(code))
                throw new CustomException(400, "请输入商户号!");
            if (string.IsNullOrEmpty(account))
                throw new CustomException(400, "请输入登录帐号!");
            if (string.IsNullOrEmpty(password))
                throw new CustomException(400, "请输入密码");
            AuthService auth = new AuthService(this.businessmanRepository);
            var result = auth.Login(new LoginParames() { Code = code.Trim(), Password = password, Account = account.Trim(), LoginIP = loginIp, BusinessmanType = 1 },
                identity =>
                {
                    MessagePushManager.Send(code, account, Common.Enums.EnumPushCommands.RepeatLogin, c_NeedSignOut);
                    //退出
                    Exit(identity);
                });

            BehaviorStatService.SaveLoginBehaviorStat(result.Token, EnumBehaviorOperate.LoginCount);
            return result.Token;
        }
        

        [ExtOperationInterceptor("退出")]
        public void Exist(string token)
        {
            AuthService auth = new AuthService(this.businessmanRepository);
            auth.Logoff(token);
        }
        private void Exit(string identity)
        {
            AuthService auth = new AuthService(this.businessmanRepository);
            auth.LogoffByIdentity(identity);
        }



    }
}
