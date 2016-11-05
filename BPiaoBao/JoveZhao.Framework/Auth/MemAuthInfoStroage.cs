using JoveZhao.Framework.Cache;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
    
    public class MemAuthInfoStroage : IAuthInfoStroage
    {
        ICacheStrategy strategy = CacheFactory.CreateCacheStrategy("auth");
       public void Save<T>(string token,T userInfo, Action<T> existCallBack) where T:class ,IUserInfo
        {
            
            if (strategy.Get(userInfo.GetIdentity()) != null)
            {  //存在登录信息
                try
                {
                    existCallBack(userInfo);
                }
                catch (Exception)
                {
                }
            }
            strategy.Set(token, userInfo);
            strategy.Set(userInfo.GetIdentity(), token);

            
        }

        public IUserInfo GetUserByToken(string token)
        {
            return strategy.Get(token) as IUserInfo;
        }
        public T GetUserByToken<T>(string token) where T : class, IUserInfo
        {
            return strategy.Get(token) as T;
        }

        public void Remove(string token)
        {
            var user = GetUserByToken(token);
            strategy.Remove(user.GetIdentity());
            strategy.Remove(token);
        }


        public List<T> FindAll<T>() where T : class, IUserInfo
        {
            return strategy.FindAll().Select(p => p as T).ToList();
        }




        public string GetTokenByIdentity(string identity)
        {
            return (string)strategy.Get(identity);
        }
    }
}
