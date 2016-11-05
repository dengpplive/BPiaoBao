using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class STRequestSupplier
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
        public decimal SupRate { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public ContactWayDataObject ContactWay { get; set; }
        /// <summary>
        /// 与钱袋子对接用的Key
        /// </summary>
        public string CashbagKey { get; set; }
        /// <summary>
        /// 与钱袋子对接用的商户Code
        /// </summary>
        public string CashbagCode { get; set; }
        /// <summary>
        /// PID配置
        /// </summary>
        public List<PIDDataObject> Pids { get; set; }
        /// <summary>
        /// 是否使用运营商客服信息
        /// true使用运营商客服，false使用控台客服
        /// </summary>
        public bool UseCustomerInfo { get; set; }
        /// <summary>
        /// 本地政策开关
        /// </summary>
        public bool SupLocalPolicySwitch { get; set; }
        /// <summary>
        /// 异地政策开关
        /// </summary>
        public bool SupRemotePolicySwitch { get; set; }
        /// <summary>
        /// 异地费率
        /// </summary>
        public decimal SupRemoteRate { get; set; }

      

    }
}
