using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    public class PolicyDataObject
    {
        /// <summary>
        /// 政策平台来源
        /// </summary>
        public string PlatformCode { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public EnumPolicySourceType? PolicySourceType { get; set; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public string PolicyType { get; set; }
        /// <summary>
        /// 出票方
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 出票方运营商
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 出票方式
        /// </summary>
        public EnumIssueTicketWay? EnumIssueTicketWay { get; set; }
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType? PolicySpecialType { get; set; }
        
    }
    public class ConsoOrderDataObject
    {
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// Pnr编码
        /// </summary>
        public string PnrCode { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string BusinessmanCode { get; set; }
        /// <summary>
        /// 订单运营商
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 订单出票时间
        /// </summary>
        public DateTime? IssueTicketTime { get; set; }
        /// <summary>
        /// 政策信息
        /// </summary>
        public PolicyDataObject Policy { get; set; }
        /// <summary>
        /// 航段
        /// </summary>
        public List<SkyDataObject> SkyWays { get; set; }
        /// <summary>
        /// 乘客
        /// </summary>
        public List<PassengerDataObject> Passengers { get; set; }
        /// <summary>
        /// 订单类型
        /// </summary>
        public int OrderType { get; set; }
    }

    [KnownType(typeof(ResponseConsoSaleBounceOrder))]
    public class ResponseConsoSaleOrder
    {
        public int Id { get; set; }
        /// <summary>
        /// 售后类型
        /// </summary>
        public string AfterSaleType { get; set; }
        /// <summary>
        /// 处理状态
        /// </summary>
        public EnumTfgProcessStatus ProcessStatus { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 申请时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 产生金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 关联订单
        /// </summary>
        public ConsoOrderDataObject Order { get; set; }
        /// <summary>
        /// 锁定帐号
        /// </summary>
        public string LockCurrentAccount { get; set; }
        /// <summary>
        /// 售后乘机人
        /// </summary>
        public List<AfterPassengerDataObject> Passenger { get; set; }
        /// <summary>
        /// 协调是否完成
        /// </summary>
        public bool? IsCoorCompleted { get; set; }
    }
    public class ResponseConsoSaleBounceOrder : ResponseConsoSaleOrder
    {
        /// <summary>
        /// 是否自愿退票
        /// </summary>
        public bool IsVoluntary { get; set; }
    }
}
