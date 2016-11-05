using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    public class ResponseSpecialPolicy : ResponseLocalPolicy
    {
        /// <summary>
        /// 乘机日期
        /// </summary>
        public StringDateLimit PassengeDate { get; set; }
        /// <summary>
        /// 出票方式
        /// </summary>
        public EnumIssueTicketWay IssueTicketWay { get; set; }
        /// <summary>
        /// 挂起状态
        /// </summary>
        public bool HangUp { get; set; }
        /// <summary>
        /// 异地返点
        /// </summary>
        public decimal Different { get; set; }
        /// <summary>
        /// 班期限制
        /// </summary>
        public string WeekLimit { get; set; }
        /// <summary>
        /// 是否适用
        /// </summary>
        public EnumApply Apply { get; set; }
        /// <summary>
        /// 政策使用航班
        /// </summary>
        public string ApplyFlights { get; set; }
        /// <summary>
        /// 审核状态
        /// </summary>
        public bool Review { get; set; }
        /// <summary>
        /// 创建人
        /// </summary>
        public string CreateMan { get; set; }
    }

    public class SearchSpecialPolicy
    {
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCodes { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCodes { get; set; }
        /// <summary>
        /// 乘机日期
        /// </summary>
        public DateTime? PassengeDateStart { get; set; }
        /// <summary>
        /// 乘机日期
        /// </summary>
        public DateTime? PassengeDateEnd { get; set; }
        /// <summary>
        /// 出票日期范围
        /// </summary>
        public DateTime? IssueDateStart { get; set; }
        /// <summary>
        /// 出票日期范围
        /// </summary>
        public DateTime? IssueDateEnd { get; set; }
        /// <summary>
        /// 出票方式
        /// </summary>
        public EnumIssueTicketWay? IssueTicketWay { get; set; }
        /// <summary>
        /// 行程类型
        /// </summary>
        public EnumTravelType? TravelType { get; set; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public string LocalPolicyType { get; set; }
        /// <summary>
        /// 挂起状态
        /// </summary>
        public bool? HangUp { get; set; }
    }

    public class RequestSpecialPolicy : RequestPolicy
    {
        /// <summary>
        /// 挂起状态
        /// </summary>
        public bool? HangUp { get; set; }
        /// <summary>
        /// 特价类型
        /// </summary>
        public SpeciaType SpecialType { get; set; }
        /// <summary>
        ///  固定特价类型
        /// </summary>
        public FixedOnSaleType Type { get; set; }
        /// <summary>
        /// 固定舱位价位
        /// </summary>
        public decimal FixedSeatPirce { get; set; }
    }

    /// <summary>
    /// 返回修改,新增
    /// </summary>
    public class ResponseOperPolicy : RequestPolicy
    {
        /// <summary>
        ///  固定特价类型
        /// </summary>
        public FixedOnSaleType Type { get; set; }
        /// <summary>
        /// 固定舱位价位
        /// </summary>
        public decimal FixedSeatPirce { get; set; }
        /// <summary>
        /// 特价类型
        /// </summary>
        public SpeciaType SpecialType { get; set; }

    }

}
