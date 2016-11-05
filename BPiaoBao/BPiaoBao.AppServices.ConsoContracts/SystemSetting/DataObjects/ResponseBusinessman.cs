using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    [KnownType(typeof(ResponseBuyer))]
    [KnownType(typeof(ResponseSupplier))]
    public abstract class ResponseBusinessman
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string Name { get; set; }
        ///// <summary>
        ///// 联系人
        ///// </summary>
        //public string Contact { get; set; }
        ///// <summary>
        ///// 联系电话
        ///// </summary>
        //public string Tel { get; set; }
        ///// <summary>
        ///// 联系地址
        ///// </summary>
        //public string Address { get; set; }
        /// <summary>
        /// 联系方式
        /// </summary>
        public ContactWayDataObject ContactWay { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 运营商号
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 商户创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    /// <summary>
    /// 采购商
    /// </summary>
    public class ResponseBuyer : ResponseBusinessman
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
        /// 业务员联系电话
        /// </summary>
        public string Phone { get; set; }
        /// <summary>
        /// 采购商标签
        /// </summary>
        public string Lable { get; set; }

    }
    /// <summary>
    /// 供应商
    /// </summary>
    public class ResponseSupplier : ResponseBusinessman
    {
        /// <summary>
        /// 所属业务员
        /// </summary>
        public string ContactName { get; set; }
        /// <summary>
        /// 业务员联系电话
        /// </summary>
        public string Phone { get; set; }
    }
    
}
