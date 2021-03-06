﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    /// <summary>
    /// 消费账单列表
    /// </summary>
    public class BillDetailListDto
    {
        /// <summary>
        /// 消费时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 账单日期
        /// </summary>
        public DateTime BillDate { get; set; }
        /// <summary>
        /// 消费金额
        /// </summary>
        public decimal Amount { get; set; }
        /// <summary>
        /// 交易号
        /// </summary>
        public string PayNo { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Notes { get; set; }
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
