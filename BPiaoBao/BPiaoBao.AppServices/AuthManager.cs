using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;

namespace BPiaoBao.AppServices
{
    public class AuthManager
    {
        private const string key = "fafagjalgjagahgdh";
        private const string tokenKey = "aaaaavvvvvcccc";
        public static CurrentUserInfo GetCurrentUser()
        {
            return CallContext.GetData(key) as CurrentUserInfo;
        }
        public static void SaveUser(CurrentUserInfo user)
        {
            CallContext.SetData(key, user);
            
        }
        public static List<CurrentUserInfo> GetOnLineUserInfo()
        {
            return UserAuthResult<CurrentUserInfo>.FindAll().Select(p => p.UserInfo).ToList();
        }
        public static void SaveToken(string token)
        {
            CallContext.SetData(tokenKey, token);
        }
        public static string GetToken()
        {
            var temp = CallContext.GetData(tokenKey);
            if (temp == null)
                return null;
            return temp.ToString();
        }
    }


}
