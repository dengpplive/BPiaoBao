using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.SystemSetting.SystemMap
{

    public class RequestCarrier
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
        /// 异地费率
        /// </summary>
        public decimal RemoteRate { get; set; }
        /// <summary>
        /// 显示本地客户中心开关
        /// </summary>
        public bool ShowLocalCSCSwich { get; set; }
    }
    public class  ContactWayDataObject
    {
        /// <summary>
        /// 联系人
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
        /// 省份
        /// </summary>
        public string Province { get; set; }
        /// <summary>
        /// 城市
        /// </summary>
        public string City { get; set; }
        /// <summary>
        /// 业务电话
        /// </summary>
        public string BusinessTel { get; set; }
    }
    public class PIDDataObject
    {
        public int ID { get; set; }
        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 端口号
        /// </summary>
        public int Port { get; set; }
        /// <summary>
        /// Office号
        /// </summary>
        public string Office { get; set; }
    }
}
