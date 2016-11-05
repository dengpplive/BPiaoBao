using JoveZhao.Framework.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;

namespace BPiaoBao.SystemSetting.Domain.Services.Auth
{
    [Serializable]
    public class CurrentUserInfo : IUserInfo
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 商户类型
        /// </summary>
        public string Type { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 操作员账号
        /// </summary>
        public string OperatorAccount { get; set; }
        /// <summary>
        /// 操作员名称
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// 操作员电话
        /// </summary>
        public string OperatorPhone { get; set; }
        /// <summary>
        /// 钱袋子商户号
        /// </summary>
        public string CashbagCode { get; set; }
        /// <summary>
        /// 钱袋子Key
        /// </summary>
        public string CashbagKey { get; set; }
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 业务员电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 设置环境配置
        /// </summary>
        public SystemSettingInfo SettingInfo { get; set; }
        /// <summary>
        /// 运营商Code
        /// </summary>
        public string CarrierCode { get; set; }
        public string GetIdentity()
        {
            //return Guid.NewGuid().ToString();
            return (Code + "," + OperatorAccount).Md5();
        }

        public bool IsAdmin { get; set; }
    }
    [Serializable]
    public class SystemSettingInfo
    {
        /// <summary>
        /// 短信价格
        /// </summary>
        public decimal SmsPrice { get; set; }
    }
}
