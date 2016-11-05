using BPiaoBao.Common.Enums;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.PlatformPoint
{
    /// <summary>
    /// 平台扣点组
    /// </summary>
    public class PlatformPointGroup : EntityBase, IAggregationRoot
    {
        /// <summary>
        /// 唯一标识
        /// </summary>
        public Guid ID { get; set; }
        /// <summary>
        /// 组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 默认扣点
        /// </summary>
        public decimal DefaultPoint { get; set; }
        /// <summary>
        /// 是否协调【设置最大点数】
        /// </summary>
        public bool IsMax { get; set; }
        /// <summary>
        /// 协调点数
        /// </summary>
        public decimal? MaxPoint { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 操作员
        /// </summary>
        public string OperatorAccount { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 运营商户
        /// </summary>
        public string Code { get; set; }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }
    /// <summary>
    /// 平台扣点组规则
    /// </summary>
    public class PlatformPointGroupRule : EntityBase, IAggregationRoot
    {
        public int ID { get; set; }
        /// <summary>
        /// 航空公司Code
        /// </summary>
        public string AirCode { get; set; }
        /// <summary>
        /// 有效期【开始】
        /// </summary>
        public DateTime StartDate { get; set; }
        /// <summary>
        /// 有效期【结束】
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 出发城市
        /// </summary>
        public string FromCityCodes { get; set; }
        /// <summary>
        /// 到达城市
        /// </summary>
        public string ToCityCodes { get; set; }
        /// <summary>
        ///政策来源
        /// </summary>
        public string IssueTicketCode { get; set; }
        /// <summary>
        /// 规则类型
        /// </summary>
        public AdjustType AdjustType { get; set; }
        /// <summary>
        /// 扣点组
        /// </summary>
        public Guid? PlatformPointGroupID { get; set; }
        public virtual PlatformPointGroup PlatformPointGroup { get; set; }

        /// <summary>
        /// 明细规则
        /// </summary>
        public virtual ICollection<PlatformPointGroupDetailRule> DetailRules { get; set; }

        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }
    /// <summary>
    /// 平台扣点明细规则
    /// </summary>
    public class PlatformPointGroupDetailRule : EntityBase
    {
        public int ID { get; set; }
        /// <summary>
        /// 扣点范围【最小值】
        /// </summary>
        public decimal StartPoint { get; set; }
        /// <summary>
        /// 扣点范围【最大值】
        /// </summary>
        public decimal EndPoint { get; set; }
        /// <summary>
        /// 扣点
        /// </summary>
        public decimal Point { get; set; }
        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }
}
