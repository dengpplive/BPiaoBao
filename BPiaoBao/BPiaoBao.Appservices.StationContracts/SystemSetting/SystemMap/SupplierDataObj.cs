using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class SupplierDataObj
    {
        /// <summary>
        /// 运营商号
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 本地费率
        /// </summary>
        public decimal SupRate { get; set; }
        /// <summary>
        /// 异地费率
        /// </summary>
        public decimal SupRemoteRate { get; set; }
        /// <summary>
        /// 本地政策开关
        /// </summary>
        public bool LocalPolicySwitch { get; set; }
        /// <summary>
        /// 异地政策开关
        /// </summary>
        public bool RemotePolicySwitch { get; set; }
        /// <summary>
        /// 
        /// 商户创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public ContactWayDataObject ContactWay { get; set; }
        /// <summary>
        /// 是否启用【默认启用】
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 与钱袋子对接用的Key
        /// </summary>
        public string CashbagKey { get; set; }
        /// <summary>
        /// 与钱袋子对接用的商户Code
        /// </summary>
        public string CashbagCode { get; set; }
        /// <summary>
        /// 配置列表
        /// </summary>
        public List<PIDDataObject> SupPids { get; set; }
    }
}
