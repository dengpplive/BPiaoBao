using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
    /// <summary>
    /// 存放用户登陆后的相关信息
    /// </summary>
    public interface IUserInfo
    {
        string GetIdentity();
    }
}
