using BPiaoBao.AppServices.DataContracts.SystemSetting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{
    public class ResponseDetailCarrier
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
        /// 异地费率
        /// </summary>
        public decimal RemoteRate { get; set; }
        /// <summary>
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
        /// 本地政策开关
        /// </summary>
        public bool LocalPolicySwitch { get; set; }
        /// <summary>
        /// 接口政策开关
        /// </summary>
        public bool InterfacePolicySwitch { get; set; }
        /// <summary>
        /// 对外异地政策开关
        /// </summary>
        public bool ForeignRemotePolicySwich { get; set; }
        /// <summary>
        /// 采购异地政策开关
        /// </summary>
        public bool BuyerRemotoPolicySwich { get; set; }
        /// <summary>
        /// 显示本地客户中心开关
        /// </summary>
        public bool ShowLocalCSCSwich { get; set; }
        /// <summary>
        /// 配置列表
        /// </summary>
        public List<PIDDataObject> Pids { get; set; }
        ///// <summary>
        ///// 运营商客服信息
        ///// </summary>
        //public CustomerDto CustomerInfo { get; set; }

        public string AdvisoryQQ { get; set; }
        public string HotlinePhone { get; set; }
        public string CustomPhone { get; set; }
    }

    public class ResponseDetailSupplier
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
        /// 异地费率
        /// </summary>
        public decimal SupRemoteRate { get; set; }
        /// <summary>
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
        /// 本地政策开关 true开 false关
        /// </summary>
        public bool SupLocalPolicySwitch { get; set; }
        /// <summary>
        /// 异地政策开关 true开 false关
        /// </summary>
        public bool SupRemotePolicySwitch { get; set; }
        /// <summary>
        /// 配置列表
        /// </summary>
        public List<PIDDataObject> SupPids { get; set; }

    }
}
