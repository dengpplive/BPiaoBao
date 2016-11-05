using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
    public class UserAuthResult<T> where T : class,IUserInfo
    {
        public static string GetTokenByIdentity(string identity)
        {
            var authInfoStroage = AuthInfoStroageFactory.CreateAuthInfoStroage();
            return  authInfoStroage.GetTokenByIdentity(identity);
        }
        public static UserAuthResult<T> Current(string token)
        {
            var authInfoStroage = AuthInfoStroageFactory.CreateAuthInfoStroage();
            var t = authInfoStroage.GetUserByToken<T>(token);
            if (t == null)
                return null;
            else
                return new UserAuthResult<T>(token, t);
        }
        public static List<UserAuthResult<T>> FindAll()
        {
            var authInfoStroage = AuthInfoStroageFactory.CreateAuthInfoStroage();
            return authInfoStroage.FindAll<T>().Select(p=>new UserAuthResult<T>(p)).ToList();
        }
        private UserAuthResult(string token,T t)
        {
            this.Token = token;
            this.UserInfo = t;
        }
        public UserAuthResult(T t)
        {
            UserInfo = t;
            Token = TokenBuilderFactory.CreateTokenBuilder().Builder(t);
        }
        public string Token { get; private set; }
        public T UserInfo { get; private set; }

        public void Save(Action<T> existCallBack)
        {
            var authInfoStroage= AuthInfoStroageFactory.CreateAuthInfoStroage();
            authInfoStroage.Save(Token, this.UserInfo, existCallBack);
        }
        public void Remove()
        {
            var authInfoStroage = AuthInfoStroageFactory.CreateAuthInfoStroage();
            authInfoStroage.Remove(this.Token);
        }
    }
}
