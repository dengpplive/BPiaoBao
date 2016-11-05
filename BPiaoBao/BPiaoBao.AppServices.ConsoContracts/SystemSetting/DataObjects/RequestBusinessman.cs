using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    /// <summary>
    /// 商户输入
    /// </summary>
    [KnownType(typeof(RequestBuyer))]
    public abstract class RequestBusinessman
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
        /// 运营商号
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 与钱袋子对接用的Key
        /// </summary>
        public string CashbagKey { get; set; }
        /// <summary>
        /// 与钱袋子对接用的商户Code
        /// </summary>
        public string CashbagCode { get; set; }
    }
    public class RequestBuyer : RequestBusinessman
    {

        /// <summary>
        /// 座机号
        /// </summary>
        public string Plane { get; set; }
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 扣点组id
        /// </summary>
        public int DeductionGroupID { get; set; }
    }
    public class AttachmentDataObject
    {
        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 附件地址
        /// </summary>
        public string Url { get; set; }
    }

    public class ContactWayDataObject
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
        /// 手机
        /// </summary>
        public string Phone { get; set; }
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
    }

    /// <summary>
    /// 供应商输入
    /// </summary>
    public class RequestSupplier
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 业务员联系电话
        /// </summary>
        public string Phone { get; set; }
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
        /// 联系方式
        /// </summary>
        public ContactWayDataObject ContactWay { get; set; }
        /// <summary>
        /// PID配置
        /// </summary>
        public List<PIDDataObject> Pids { get; set; }
        /// <summary>
        /// office配置
        /// </summary>
        public List<CarrierSettingDataObject> CarrierSettings { get; set; }


    }
}
