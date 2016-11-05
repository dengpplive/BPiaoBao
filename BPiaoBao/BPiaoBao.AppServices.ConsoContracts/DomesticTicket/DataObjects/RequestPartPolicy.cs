using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    /// <summary>
    /// 部分修改视图
    /// </summary>
    public class RequestPartPolicy
    {
        public Guid ID { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarrayCode { get; set; }
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
        /// 低开
        /// </summary>
        public bool Low { get; set; }
        /// <summary>
        /// 换编码
        /// </summary>
        public bool ChangeCode { get; set; }
    }
}
