using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
   public class CurrentUserInfo
    {
        public string Type { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string BusinessmanCode { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 操作员账号
        /// </summary>
        public string OperatorAccount { get; set; }
        /// <summary>
        /// 操作员名
        /// </summary>
        public string OperatorName { get; set; }
        /// <summary>
        /// 操作员电话
        /// </summary>
        public string OperatorPhone { get; set; }
        /// <summary>
        /// 联系人
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 业务员电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
