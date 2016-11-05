using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.Auth
{
    public class DefaultTokenBuilder:ITokenBuilder
    {

        public string Builder(IUserInfo userInfo)
        {
            return Guid.NewGuid().ToString();
        }
    }
}
