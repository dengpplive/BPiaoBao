using System;

namespace BPiaoBao.Client.UIExt.Communicate
{
    public static class LoginInfo
    {
        /// <summary>
        /// 全局记录是否已经登录 
        /// </summary>
        public static bool IsLogined;
        /// <summary>
        /// Token标识
        /// </summary>
        public static string Token { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public static string Code { get; set; }
        /// <summary>
        /// 当前登录账户
        /// </summary>
        public static string Account { get; set; }
        /// <summary>
        /// 唯一标识
        /// </summary>
        public static Guid Guid { get; set; }
        /// <summary>
        /// 是否管理员账户
        /// </summary>
        public static bool IsAdmin
        {
            get
            {
                return Account.ToLower().Equals("admin");
            }
        }
    }
}
