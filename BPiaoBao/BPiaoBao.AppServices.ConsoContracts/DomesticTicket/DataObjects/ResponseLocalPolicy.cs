using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    /// <summary>
    /// 日期值对象
    /// </summary>
    public class StringDateLimit
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public string StartTime { get; set; }
        /// <summary>
        /// 截至日期
        /// </summary>
        public string EndTime { get; set; }
    }
    [KnownType(typeof(ResponseLocalNormalPolicy))]
    public class ResponseLocalPolicy
    {
        public Guid ID { get; set; }

        /// <summary>
        /// 承运人
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCodes { get; set; }
        /// <summary>
        /// 是否是自己的政策
        /// </summary>
        public bool  IsExist { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCodes { get; set; }
        /// <summary>
        /// 本地返点
        /// </summary>
        public decimal LocalPoint { get; set; }
        /// <summary>
        /// 行程类型
        /// </summary>
        public EnumTravelType TravelType { get; set; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public string LocalPolicyType { get; set; }
        /// <summary>
        /// 出票日期范围
        /// </summary>
        public StringDateLimit IssueDate { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seats { get; set; }
    }
    public class ResponseLocalNormalPolicy : ResponseLocalPolicy
    { 
    
    }
}
