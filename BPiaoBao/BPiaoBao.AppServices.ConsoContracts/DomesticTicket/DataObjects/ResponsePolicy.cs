using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    public class SearchPolicy
    {
        /// <summary>
        /// 政策类型
        /// </summary>
        public string PolicyType { get; set; }
        /// <summary>
        /// 发布类型
        /// </summary>
        public EnumReleaseType? ReleaseType { get; set; }
        /// <summary>
        /// 返点最小值
        /// </summary>
        public decimal? LocalStartPoint { get; set; }
        /// <summary>
        /// 返点最大值
        /// </summary>
        public decimal? LocalEndPoint { get; set; }
        /// <summary>
        /// 乘机开始日期
        /// </summary>
        public DateTime? PStartTime { get; set; }
        /// <summary>
        /// 乘机截至日期
        /// </summary>
        public DateTime? PEndTime { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public bool? Review { get; set; }
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCode { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCode { get; set; }
        /// <summary>
        /// 行程类型
        /// </summary>
        public EnumTravelType? TravelType { get; set; }
        /// <summary>
        /// 出票方式
        /// </summary>
        public EnumIssueTicketWay? IssueTicketWay { get; set; }
        /// <summary>
        /// 航空公司
        /// </summary>
        public string CarrayCode { get; set; }

    }
    public class ResponsePolicy
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 商户Code【运营，供应】
        /// </summary>
        public string Code { get; set; }
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
        /// 承运人
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seats { get; set; }
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
        /// 审核状态
        /// </summary>
        public bool Review { get; set; }
        /// <summary>
        /// 挂起状态
        /// </summary>
        public bool HangUp { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateMan { get; set; }
        /// <summary>
        /// 特价政策类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }
    }
}
