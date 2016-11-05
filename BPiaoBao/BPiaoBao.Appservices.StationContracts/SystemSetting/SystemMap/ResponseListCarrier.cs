using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class ResponseListCarrierOrSupplier
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 费率
        /// </summary>
        public decimal Rate { get; set; }
        /// <summary>
        /// 商户创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public string Contact { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Tel { get; set; }
        /// <summary>
        /// 联系地址
        /// </summary>
        public string Address { get; set; }
        /// <summary>
        /// 是否启用【默认启用】
        /// </summary>
        public bool IsEnable { get; set; }
        /// 与钱袋子对接用的Key
        /// </summary>
        public string CashbagKey { get; set; }
        /// <summary>
        /// 与钱袋子对接用的商户Code
        /// </summary>
        public string CashbagCode { get; set; }
    }

}
