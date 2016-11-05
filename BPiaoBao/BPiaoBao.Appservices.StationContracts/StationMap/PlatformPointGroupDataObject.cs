using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.StationMap
{
    /// <summary>
    /// 扣点组
    /// </summary>
    public class PlatformPointGroupDataObject
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
        /// 运营商组
        /// </summary>
        public string Code { get; set; }
    }
    /// <summary>
    /// 扣点规则
    /// </summary>
    public class PlatformPointGroupRuleDataObject
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
        /// 本地政策Code【多个已,号分割】
        /// </summary>
        public string IssueTicketCode { get; set; }
        /// <summary>
        /// 规则类型
        /// </summary>
        public AdjustType AdjustType { get; set; }
        /// <summary>
        /// 扣点组ID
        /// </summary>
        public Guid? PlatformPointGroupID { get; set; }
        /// <summary>
        /// 扣点组
        /// </summary>
        public string PointGroupName { get; set; }
        /// <summary>
        /// 明细规则
        /// </summary>
        public List<PointGroupDetailRuleDataObject> DetailRules { get; set; }
    }
    public class PointGroup
    {
        /// <summary>
        /// 扣点组ID
        /// </summary>
        public Guid PointGroupID { get; set; }
        /// <summary>
        /// 扣点组名称
        /// </summary>
        public string PointGroupName { get; set; }
    }
    /// <summary>
    /// 扣点明细
    /// </summary>
    public class PointGroupDetailRuleDataObject
    {
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
    }
    public class CodePoint
    {
        /// <summary>
        /// 运营商商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 扣点组名称
        /// </summary>
        public string GroupName { get; set; }
        /// <summary>
        /// 是否选中
        /// </summary>
        public bool IsSelected { get; set; }
    }
    public class BResponse
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public bool IsSelected { get; set; }
    }
   
}
