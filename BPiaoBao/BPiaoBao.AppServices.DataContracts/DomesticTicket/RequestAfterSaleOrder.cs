using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 售后申请
    /// </summary>
    [KnownType(typeof(RequestAnnulOrder))]
    [KnownType(typeof(RequestBounceOrder))]
    [KnownType(typeof(RequestChangeOrder))]
    [KnownType(typeof(RequestModifyOrder))]
    public class RequestAfterSaleOrder
    {
        /// <summary>
        /// 乘机人
        /// </summary>
        public int[] Passengers { get; set; }
        /// <summary>
        /// 申请理由
        /// </summary>
        public string Reason { get; set; }
        /// <summary>
        /// 申请备注
        /// </summary>
        public string Description { get; set; }
    }
    /// <summary>
    /// 废票
    /// </summary>
    public class RequestAnnulOrder : RequestAfterSaleOrder
    {
        /// <summary>
        /// 附件地址
        /// </summary>
        public string AttachmentUrl { get; set; }
    }
    /// <summary>
    /// 退票
    /// </summary>
    public class RequestBounceOrder : RequestAfterSaleOrder
    {
        /// <summary>
        /// 是否自愿 true为自愿，默认非自愿
        /// </summary>
        public bool IsVoluntary { get; set; }
        /// <summary>
        /// 附件地址
        /// </summary>
        public string AttachmentUrl { get; set; }
    }
    /// <summary>
    /// 改签
    /// </summary>
    public class RequestChangeOrder : RequestAfterSaleOrder
    {
        /// <summary>
        /// 航班信息
        /// </summary>
        public IList<RequestAfterSaleSkyWay> SkyWay { get; set; }
    }
    public class RequestModifyOrder : RequestAfterSaleOrder
    {

    }
    /// <summary>
    /// 售后航段信息
    /// </summary>
    public class RequestAfterSaleSkyWay
    {
        /// <summary>
        /// 原航班编号
        /// </summary>
        public int SkyWayId { get; set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime NewStartDateTime
        {
            get;
            set;
        }
        public DateTime NewToDateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// 航班号
        /// </summary>
        public string NewFlightNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位
        /// </summary>
        public string NewSeat
        {
            get;
            set;
        }
    }
}
