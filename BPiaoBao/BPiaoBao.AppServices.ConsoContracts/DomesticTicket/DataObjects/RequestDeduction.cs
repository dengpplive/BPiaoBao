using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    public class RequestDeduction
    {
        public int ID { get; set; }
        /// <summary>
        /// 扣点组名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 扣点规则组
        /// </summary>
        public List<RequestDeductionRule> DeductionRules { get; set; }
    }
    /// <summary>
    /// 扣点规则
    /// </summary>
    public class RequestDeductionRule
    {
        public int ID { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarrCode { get; set; }
        /// <summary>
        /// 类型[枚举值]
        /// </summary>
        public string DeductionType { get; set; }
        /// <summary>
        /// 类型枚举描述
        /// </summary>
        public string DeductionTypeDescription { get; set; }
        /// <summary>
        /// 有效开始日期
        /// </summary>
        public DateTime StartTime { get; set; }
        /// <summary>
        /// 有效截至日期
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 调整明细集合
        /// </summary>
        public List<RequestAdjustDetail> AdjustDetails { get; set; }
    }
    public class RequestAdjustDetail
    {
        public int ID { get; set; }
        /// <summary>
        /// 点数范围：开始点数
        /// </summary>
        public decimal StartPoint { get; set; }
        /// <summary>
        /// 点数范围：截至点数
        /// </summary>
        public decimal EndPoint { get; set; }
        /// <summary>
        /// 调整类型[枚举值]
        /// </summary>
        public string AdjustType { get; set; }
        /// <summary>
        /// 调整类型[枚举描述]
        /// </summary>
        public string AdjustTypeDescription { get; set; }
        /// <summary>
        /// 点数
        /// </summary>
        public decimal Point { get; set; }
    }
}
