using JoveZhao.Framework.Auth;
using JoveZhao.Framework;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using System.Linq;
using JoveZhao.Framework.DDD.Events;
using BPiaoBao.Common;
using System;

namespace BPiaoBao.SystemSetting.Domain.Services.Auth
{
    public class AuthService : IAuthService<LoginParames, CurrentUserInfo>
    {

        IBusinessmanRepository businessmanRepository;
        public AuthService(IBusinessmanRepository businessmanRepository)
        {
            this.businessmanRepository = businessmanRepository;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="action"></param>
        /// <param name="loginIp"></param>
        /// <param name="businessmanType">0：采购，1,后台</param>
        /// <returns></returns>
        public UserAuthResult<CurrentUserInfo> Login(LoginParames user, System.Action<string> action)
        {
            var bussinessMan = businessmanRepository.FindAll(p => p.Code.ToUpper() == user.Code.ToUpper()).FirstOrDefault();
            if (bussinessMan == null)
                throw new AuthException("没有找到商户号为" + user.Code + "的商户信息");
            if (user.BusinessmanType == 0 && (bussinessMan is Supplier || bussinessMan is Carrier))
                throw new AuthException("没有找到商户号为" + user.Code + "的商户信息。");
            if (user.BusinessmanType == 1 && bussinessMan is Buyer)
                throw new AuthException("没有找到商户号为" + user.Code + "的商户信息!");
            if (!bussinessMan.IsEnable)
                throw new AuthException("此商户号已经被冻结，请联系管理员");
            var oper = bussinessMan.GetOperatorByPasswordAndAccount(user.Account.Trim(), user.Password);

            if (oper == null)
                throw new AuthException("用户名或密码错误");
            if (oper.OperatorState == Common.Enums.EnumOperatorState.Frozen)
                throw new AuthException("该用户账号已被冻结");
            if (user.BusinessmanType == 1 && oper.IsAdmin == false && oper.Role == null)
                throw new AuthException("该帐号没有权限登录，请联系管理员！");
            var currentUserInfo = new CurrentUserInfo()
            {
                Type = bussinessMan.GetType().BaseType.Name,
                OperatorAccount = oper.Account,
                Code = bussinessMan.Code,
                BusinessmanName = bussinessMan.Name,
                CashbagCode = bussinessMan.CashbagCode,
                CashbagKey = bussinessMan.CashbagKey,
                OperatorName = oper.Realname,
                OperatorPhone = oper.Phone,
                IsAdmin = oper.IsAdmin,
                SettingInfo = new SystemSettingInfo()
                {
                    SmsPrice = SettingSection.GetInstances().Sms.SmsPrice
                }

            };

            if (bussinessMan is Supplier)
            {
                currentUserInfo.CarrierCode = (bussinessMan as Supplier).CarrierCode;
            }
            if (bussinessMan is Buyer)
            {
                currentUserInfo.ContactName = (bussinessMan as Buyer).ContactName;
                currentUserInfo.Phone = (bussinessMan as Buyer).Phone;
                currentUserInfo.CarrierCode = (bussinessMan as Buyer).CarrierCode;
            }
            if (bussinessMan is Carrier)
            {
                currentUserInfo.ContactName = oper.Realname;
            }
            LoginLog loginLog = new LoginLog
            {
                Code = bussinessMan.Code,
                Account = oper.Account,
                LoginIP = user.LoginIP,
                LoginDate = System.DateTime.Now
            };
            var ur = new UserAuthResult<CurrentUserInfo>(currentUserInfo);
            ur.Save(p =>
            {
                action(currentUserInfo.GetIdentity());
            });
            //引发领域事件
            DomainEvents.Raise(new UserLoginEvent() { User = ur.UserInfo, LoginLog = loginLog });

            return ur;
        }

        public void Logoff(string token)
        {
            if (UserAuthResult<CurrentUserInfo>.Current(token) != null)
                UserAuthResult<CurrentUserInfo>.Current(token).Remove();
        }
        public void LogoffByIdentity(string identity)
        {
            var token = UserAuthResult<CurrentUserInfo>.GetTokenByIdentity(identity);
            Logoff(token);
        }

        public CurrentUserInfo GetCurrentUserByToken(string token)
        {
            return UserAuthResult<CurrentUserInfo>.Current(token).UserInfo;
        }
    }
    public class AuthException : CustomException
    {
        public AuthException(string message) : base(500, message) { }
    }
}
