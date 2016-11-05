using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    /// <summary>
    /// 账户收支明细
    /// </summary>
    public class BalanceDetailDto
    {
        /// <summary>
        /// 支付方式
        /// </summary>
        public string PayType { get; set; }
        /// <summary>
        /// 收支
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 交易时间
        /// </summary>
        public DateTime CreateAmount { get; set; }
        /// <summary>
        /// 剩余金额
        /// </summary>
        public decimal LeaveAmount { get; set; }
        /// <summary>
        /// 交易类型
        /// </summary>
        public string OperationType { get; set; }
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OutOrderNo { get; set; }

        /// <summary>
        /// 交易号
        /// </summary>
        public string OutTradeNo { get; set; }
    }
}
