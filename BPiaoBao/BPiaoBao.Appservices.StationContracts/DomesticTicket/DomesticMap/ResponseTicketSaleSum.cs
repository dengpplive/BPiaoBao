using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.DomesticTicket.DomesticMap
{
    public class ResponseTicketSaleSum
    {
        /// <summary>
        /// 日期
        /// </summary>
        public string DateTime { get; set; }
        /// <summary>
        /// 出票现金交易金额
        /// </summary>
        public decimal IssueAccountMoney { get; set; }
        public decimal IssueAccountTransactionFee { get; set; }
        public decimal IssueAccountPayFee { get; set; }
        /// <summary>
        /// 出票信用交易金额
        /// </summary>
        public decimal IssueCreditMoney { get; set; }
        public decimal IssueCreditTransactionFee { get; set; }
        public decimal IssueCreditPayFee { get; set; }
        /// <summary>
        /// 出票支付宝交易金额
        /// </summary>
        public decimal IssueAlipayMoney { get; set; }
        public decimal IssueAlipayTransactionFee { get; set; }
        public decimal IssueAlipayPayFee { get; set; }
        /// <summary>
        /// 出票财付通交易金额
        /// </summary>
        public decimal IssueTenpayMoney { get; set; }
        public decimal IssueTenpayTransactionFee { get; set; }
        public decimal IssueTenpayPayFee { get; set; }
        /// <summary>
        /// 出票代付金额
        /// </summary>
        public decimal IssuePaidMoney { get; set; }
        /// <summary>
        /// 出票收益
        /// </summary>
        public decimal IssueInCome { get; set; }

        /// <summary>
        /// 退票现金交易金额
        /// </summary>
        public decimal BounceAccountMoney { get; set; }
        public decimal BounceAccountTransactionFee { get; set; }
        public decimal BounceAccountPayFee { get; set; }
        /// <summary>
        /// 退票信用交易金额
        /// </summary>
        public decimal BounceCreditMoney { get; set; }
        public decimal BounceCreditTransactionFee { get; set; }
        public decimal BounceCreditPayFee { get; set; }
        /// <summary>
        /// 退票支付宝交易金额
        /// </summary>
        public decimal BounceAlipayMoney { get; set; }
        public decimal BounceAlipayTransactionFee { get; set; }
        public decimal BounceAlipayPayFee { get; set; }
        /// <summary>
        /// 退票财付通交易金额
        /// </summary>
        public decimal BounceTenpayMoney { get; set; }
        public decimal BounceTenpayTransactionFee { get; set; }
        public decimal BounceTenpayPayFee { get; set; }
        /// <summary>
        /// 退票收益
        /// </summary>
        public decimal BounceInCome { get; set; }

        /// <summary>
        /// 废票现金交易金额
        /// </summary>
        public decimal AnnulAccountMoney { get; set; }
        public decimal AnnulAccountTransactionFee { get; set; }
        public decimal AnnulAccountPayFee { get; set; }
        /// <summary>
        /// 废票信用交易金额
        /// </summary>
        public decimal AnnulCreditMoney { get; set; }
        public decimal AnnulCreditTransactionFee { get; set; }
        public decimal AnnulCreditPayFee { get; set; }
        /// <summary>
        /// 废票支付宝交易金额
        /// </summary>
        public decimal AnnulAlipayMoney { get; set; }
        public decimal AnnulAlipayTransactionFee { get; set; }
        public decimal AnnulAlipayPayFee { get; set; }
        /// <summary>
        /// 废票财付通交易金额
        /// </summary>
        public decimal AnnulTenpayMoney { get; set; }
        public decimal AnnulTenpayTransactionFee { get; set; }
        public decimal AnnulTenpayPayFee { get; set; }
        /// <summary>
        /// 废票收益
        /// </summary>
        public decimal AnnulInCome { get; set; }

        /// <summary>
        /// 改签现金交易金额
        /// </summary>
        public decimal ChangeAccountMoney { get; set; }
        public decimal ChangeAccountTransactionFee { get; set; }
        public decimal ChangeAccountPayFee { get; set; }
        /// <summary>
        /// 改签信用交易金额
        /// </summary>
        public decimal ChangeCreditMoney { get; set; }
        public decimal ChangeCreditTransactionFee { get; set; }
        public decimal ChangeCreditPayFee { get; set; }
        /// <summary>
        /// 改签支付宝交易金额
        /// </summary>
        public decimal ChangeAlipayMoney { get; set; }
        public decimal ChangeAlipayTransactionFee { get; set; }
        public decimal ChangeAlipayPayFee { get; set; }
        /// <summary>
        /// 改签财付通交易金额
        /// </summary>
        public decimal ChangeTenpayMoney { get; set; }
        public decimal ChangeTenpayTransactionFee { get; set; }
        public decimal ChangeTenpayPayFee { get; set; }
        /// <summary>
        /// 改签代付金额
        /// </summary>
        public decimal ChangePaidMoney { get; set; }
        /// <summary>
        /// 改签收益
        /// </summary>
        public decimal ChangeInCome { get; set; }
        /// <summary>
        /// 合计收益
        /// </summary>
        public decimal TotalInCome { get; set; }
    }
}
