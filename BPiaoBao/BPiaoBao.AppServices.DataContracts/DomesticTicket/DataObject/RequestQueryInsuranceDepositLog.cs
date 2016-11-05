using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    public class RequestQueryInsuranceDepositLog
    {

        /// <summary>
        /// 是否是控台调用
        /// </summary>
        public bool IsCtrlStationCall { get; set; }

        /// <summary>
        /// 流水号
        /// </summary>
        public string PayNo { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string TradeId { get; set; }

        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// 充值时间（开始）
        /// </summary>
        public DateTime? BuyStartTime { get; set; }

        /// <summary>
        /// 充值时间（结束）
        /// </summary>
        public DateTime? BuyEndTime { get; set; }

        /// <summary>
        /// 记录类型
        /// </summary>
        public EnumInsuranceDepositType? InsuranceDepositType { get; set; }
      
    }
}
