using JoveZhao.Framework.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Services.Auth
{
    public class LoginParames : IUserLogin
    {
        public string Code { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string LoginIP { get; set; }
        public int BusinessmanType = 0;
    }
}
