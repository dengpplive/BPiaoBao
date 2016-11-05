using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    /// <summary>
    /// 钱袋子商户信息
    /// </summary>
    public class CashCompanyInfoDto
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
        /// 钱袋子code
        /// </summary>
        public string PayAccount { get; set; }
        /// <summary>
        /// 钱袋子token
        /// </summary>
        public string Token { get; set; }
    }
}
