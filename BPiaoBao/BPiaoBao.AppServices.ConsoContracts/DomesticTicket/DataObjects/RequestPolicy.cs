using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    /// <summary>
    /// 日期值对象
    /// </summary>
    public class ClientDateLimit
    {
        /// <summary>
        /// 开始日期
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 截至日期
        /// </summary>
        public DateTime EndTime { get; set; }
    }
    public class RequestPolicy
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 发布类型
        /// </summary>
        public EnumReleaseType ReleaseType { get; set; }
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCodes { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCodes { get; set; }
        /// <summary>
        /// 本地返点
        /// </summary>
        public decimal LocalPoint { get; set; }
        /// <summary>
        /// 异地返点
        /// </summary>
        public decimal Different { get; set; }
        /// <summary>
        /// 行程类型
        /// </summary>
        public EnumTravelType TravelType { get; set; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public string LocalPolicyType { get; set; }
        /// <summary>
        /// 低开
        /// </summary>
        public bool Low { get; set; }
        /// <summary>
        /// 换编码
        /// </summary>
        public bool ChangeCode { get; set; }
        /// <summary>
        /// OFFICE号
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// 班期限制
        /// </summary>
        public string WeekLimit { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seats { get; set; }
        /// <summary>
        /// 共享航班
        /// </summary>
        public bool Share { get; set; }
        /// <summary>
        /// 乘机日期
        /// </summary>
        public ClientDateLimit PassengeDate { get; set; }
        /// <summary>
        /// 出票日期范围
        /// </summary>
        public ClientDateLimit IssueDate { get; set; }
        /// <summary>
        /// 出票方式
        /// </summary>
        public EnumIssueTicketWay IssueTicketWay { get; set; }
        /// <summary>
        /// 是否适用
        /// </summary>
        public EnumApply Apply { get; set; }
        /// <summary>
        /// 政策使用航班
        /// </summary>
        public string ApplyFlights { get; set; }
        /// <summary>
        /// 政策备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public bool Review { get; set; }
    }
    public class RequestNormalPolicy : RequestPolicy
    { 
    
    }
}
