using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
    /// <summary>
    /// 定义登陆接口
    /// </summary>
    public interface IAuthService<T,R> 
        where T : class, IUserLogin
        where R : class, IUserInfo
    {
        UserAuthResult<R> Login(T user, System.Action<string> action);
            

        void Logoff(string token);

        R GetCurrentUserByToken(string token);
    }
}
