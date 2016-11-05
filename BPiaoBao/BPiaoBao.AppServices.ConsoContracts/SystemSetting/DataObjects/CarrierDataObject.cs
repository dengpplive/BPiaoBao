using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    [KnownType(typeof(CarrierDataObject))]
    [KnownType(typeof(SupplierDataObject))]
    public class BusinessmanDataObject
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
        /// 商户创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public ContactWayDataObject ContactWay { get; set; }

        /// <summary>
        /// 正常工作日 业务处理时间
        /// </summary>
        public WorkBusinessmanDataObject NormalWork { get; set; }
        /// <summary>
        /// 休息日 业务处理时间
        /// </summary>
        public WorkBusinessmanDataObject RestWork { get; set; }
        /// <summary>
        /// PID配置
        /// </summary>
        public List<PIDDataObject> Pids { get; set; }
        /// <summary>
        /// office配置
        /// </summary>
        public List<CarrierSettingDataObject> CarrierSettings { get; set; }
    }
    /// <summary>
    /// 采购商
    /// </summary>
    public class CarrierDataObject : BusinessmanDataObject
    {
        /// <summary>
        /// 标签
        /// </summary>
        public string Label { get; set; }
    }
    /// <summary>
    /// 供应商
    /// </summary>
    public class SupplierDataObject : BusinessmanDataObject
    {
        /// <summary>
        /// 本地费率
        /// </summary>
        public decimal SupRate { get; set; }
        /// <summary>
        /// 异地费率
        /// </summary>
        public decimal SupRemoteRate { get; set; }
    }
    public class WorkBusinessmanDataObject
    {
        /// <summary>
        /// 星期
        /// </summary>
        public string WeekDay { get; set; }
        /// <summary>
        /// 上班时间
        /// </summary>
        public string WorkOnLineTime { get; set; }
        /// <summary>
        /// 下班时间
        /// </summary>
        public string WorkUnLineTime { get; set; }
        /// <summary>
        /// 业务处理上线时间
        /// </summary>
        public string ServiceOnLineTime { get; set; }
        /// <summary>
        /// 业务处理下线世间
        /// </summary>
        public string ServiceUnLineTime { get; set; }
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
