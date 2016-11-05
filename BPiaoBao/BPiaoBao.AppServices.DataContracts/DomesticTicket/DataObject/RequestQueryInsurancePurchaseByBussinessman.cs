using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    public class RequestQueryInsurancePurchaseByBussinessman
    {
        
        /// <summary>
        /// 购买开始时间
        /// </summary>
        public DateTime? BuyStartTime { get; set; }
        /// <summary>
        /// 购买结束时间
        /// </summary>
        public DateTime? BuyEndTime { get; set; }
        /// <summary>
        /// 记录类别
        /// </summary>
        public EnumInsurancePurchaseType? RecordType { get; set; }

        /// <summary>
        /// 支付状态
        /// </summary>
        public EnumInsurancePayStatus? PayStatus { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeNo { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string PayNo { get; set; }
        /// <summary>
        /// 分销商号，如无此查询条件可不填
        /// </summary>
        public string BuyerCode { get; set; }
        /// <summary>
        /// 运营商号，如无此查询条件可不填
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 客户端类型
        /// 0：买票宝，1：卖票宝，2：控台
        /// </summary>
        public int RequestFrom { get; set; }
    }
}
