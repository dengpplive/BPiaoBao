using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    public class DefaultPolicyDataObject
    {
        /// <summary>
        /// 运营商或供应商
        /// </summary>
        public string CarrayCode { get; set; }
        /// <summary>
        /// 默认政策
        /// </summary>
        public decimal DefaultPolicyPoint { get; set; }
        /// <summary>
        /// 儿童政策
        /// </summary>
        public decimal ChildrenPolicyPoint { get; set; }
        /// <summary>
        /// 出票类型
        /// </summary>
        public string IssueTicketType { get; set; }
        /// <summary>
        ///  office号 
        /// </summary>
        public string Office { get; set; }
        /// <summary>
        /// 出票方式
        /// </summary>
        public EnumIssueTicketWay IssueTicketWay { get; set; }
    }

}
