using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class ResponseInsuranceSaleSum
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 现金金额
        /// </summary>
        public decimal AccountMoney { get; set; }
        /// <summary>
        /// 现金手续费
        /// </summary>
        public decimal AccountPoundage { get; set; }
        /// <summary>
        /// 信用金额
        /// </summary>
        public decimal CreditMoney { get; set; }
        /// <summary>
        /// 信用手续费
        /// </summary>
        public decimal CreditPoundage { get; set; }
        /// <summary>
        /// 银行卡金额
        /// </summary>
        public decimal BankMoney { get; set; }
        /// <summary>
        /// 银行卡手续费
        /// </summary>
        public decimal BankPoundage { get; set; }
        /// <summary>
        /// 支付平台金额
        /// </summary>
        public decimal PlatformMoney { get; set; }
        /// <summary>
        /// 支付平台手续费
        /// </summary>
        public decimal PlatPoundage { get; set; }
        /// <summary>
        /// 赠送张数
        /// </summary>
        public int TotalGiveCount { get; set; }
        /// <summary>
        /// 购买张数
        /// </summary>
        public int TotalBuyCount { get; set; }
        /// <summary>
        /// 合计金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 合计手续费
        /// </summary>
        public decimal TotalPoundage { get; set; }
       
    }
}
