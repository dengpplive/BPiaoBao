using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
    /// <summary>
    /// 定义登录信息的存放方式
    /// </summary>
    public interface IAuthInfoStroage
    {

        void Remove(string token);
        void Save<T>(string token,T userInfo, Action<T> existCallBack) where T : class ,IUserInfo;
        List<T> FindAll<T>() where T : class, IUserInfo;
        IUserInfo GetUserByToken(string token);
        T GetUserByToken<T>(string token) where T : class, IUserInfo;

        string GetTokenByIdentity(string identity);
    }
}
