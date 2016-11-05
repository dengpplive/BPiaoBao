using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
    public class TradeDetailDataObject
    {
        /// <summary>
        /// Pos终端号
        /// </summary>
        public string PosNo { get; set; }
        /// <summary>
        /// 商户名称
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime TradeTime { get; set; }
        /// <summary>
        /// 批次号
        /// </summary>
        public string BatchNo { get; set; }
        /// <summary>
        /// 交易卡号
        /// </summary>
        public string TradeCardNo { get; set; }
        /// <summary>
        /// 交易卡号类别
        /// </summary>
        public string TradeCardType { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal TradeMoney { get; set; }
        /// <summary>
        /// 收款金额
        /// </summary>
        public decimal ReceivMoney { get; set; }
        /// <summary>
        /// Pos费率
        /// </summary>
        public decimal PosRate { get; set; }
        /// <summary>
        /// Pos收益
        /// </summary>
        public decimal PosGain { get; set; }
    }
}
