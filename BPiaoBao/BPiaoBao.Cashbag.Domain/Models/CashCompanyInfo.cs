using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    public class CashCompanyInfo
    {
        /// <summary>
        /// 公司名
        /// </summary>
        public string CpyName { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Moblie { get; set; }
        /// <summary>
        /// 账号
        /// </summary>
        public string ClientAccount { get; set; }
        /// <summary>
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 钱袋子code
        /// </summary>
        public string PayAccount { get; set; }
        /// <summary>
        /// 钱袋子token
        /// </summary>
        public string Token { get; set; }
    }
}
