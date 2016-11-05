using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
    public interface ITokenBuilder
    {
        string Builder(IUserInfo userInfo);
    }
}
