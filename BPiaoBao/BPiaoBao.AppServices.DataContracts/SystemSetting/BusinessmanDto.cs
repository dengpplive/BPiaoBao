using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.SystemSetting
{
    public class BusinessmanDto
    {
        /// <summary>
        /// 商户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 商户状态
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 短信剩余条数
        /// </summary>
        public int RemainCount { get; set; }
        /// <summary>
        /// 短信发送条数
        /// </summary>
        public int SendCount { get; set; }
        /// <summary>
        /// 与钱袋子对接用的Key
        /// </summary>
        public string CashbagKey { get; set; }
        /// <summary>
        /// 与钱袋子对接用的商户Code
        /// </summary>
        public string CashbagCode { get; set; }
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
    }
}
