using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.Model
{
    /// <summary>
    /// 本地存储数据
    /// </summary>
    public class LocalData
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Num { get; set; }

        /// <summary>
        /// 登录名
        /// </summary>
        public string LoginName { get; set; }

        /// <summary>
        /// 登录密码
        /// </summary>
        public string LoginPwd { get; set; }

        /// <summary>
        /// 是否自动登录
        /// </summary>
        public bool AutoLogin { get; set; }

        /// <summary>
        /// 是否记住密码
        /// </summary>
        public bool RememberPassword { get; set; }

        /// <summary>
        /// 转化数据
        /// </summary>
        /// <param name="data">原始结构</param>
        /// <returns></returns>
        internal static LocalData Transfer(ViewModel.LoginViewModel data)
        {
            if (data == null)
                return null;

            LocalData result = new LocalData();
            result.AutoLogin = data.AutoLogin;
            result.LoginName = data.LoginName;
            result.LoginPwd = data.LoginPwd;
            result.Num = data.Num;
            result.RememberPassword = data.RememberPassword;
            return result;
        }
    }
}
