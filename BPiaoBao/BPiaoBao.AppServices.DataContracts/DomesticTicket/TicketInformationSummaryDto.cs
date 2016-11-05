using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 机票信息汇总
    /// </summary>
    public class TicketInformationSummaryDto
    {
        //public IssueTicketInformation IssueTicketInformation { get; set; }
        //public RefundTicketInformation RefundTicketInformation { get; set; }
        //public InvalidTicketInformation InvalidTicketInformation { get; set; }
        //public ChangeTicketInformation ChangeTicketInformation { get; set; }
        //public TicketCancelInformation TicketCancelInformation { get; set; }
        /// <summary>
        /// 机票类型
        /// </summary>
        public string TicketType { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public int TicketCount { get; set; }

        /// <summary>
        /// 票价
        /// </summary>
        public decimal TicketPrice { get; set; }

        /// <summary>
        /// 税费
        /// </summary>
        public decimal TaxFee { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 应收
        /// </summary>
        public decimal ShouldMoney { get; set; }

        /// <summary>
        /// 已收
        /// </summary>
        public decimal RealMoney { get; set; }

        /// <summary>
        /// 代付金额
        /// </summary>
        public decimal PaidMoney { get; set; }
        /// <summary>
        /// 支付宝代付金额
        /// </summary>
        public decimal PaidMoneyByAlipay { get; set; }
        /// <summary>
        /// 财付通代付金额
        /// </summary>
        public decimal PaidMoneyByTenPay { get; set; }

        /// <summary>
        /// 应退金额
        /// </summary>
        public decimal ShouldRefundMoney { get; set; }

        /// <summary>
        /// 已退金额
        /// </summary>
        public decimal RealRefundMoney { get; set; }
    }


    ///// <summary>
    ///// 出票信息
    ///// </summary>
    //public class IssueTicketInformation
    //{
    //    /// <summary>
    //    /// 数量
    //    /// </summary>
    //    public int TicketCount { get; set; }

    //    /// <summary>
    //    /// 票价
    //    /// </summary>
    //    public decimal TicketPrice { get; set; }

    //    /// <summary>
    //    /// 税费
    //    /// </summary>
    //    public decimal TaxFee { get; set; }
    //    /// <summary>
    //    /// 佣金
    //    /// </summary>
    //    public decimal Commission { get; set; }
    //    /// <summary>
    //    /// 应收
    //    /// </summary>
    //    public decimal ShouldMoney { get; set; }

    //    /// <summary>
    //    /// 已收
    //    /// </summary>
    //    public decimal RealMoney { get; set; }

    //    /// <summary>
    //    /// 代付金额
    //    /// </summary>
    //    public decimal PaidMoney { get; set; }
    //    /// <summary>
    //    /// 支付宝代付金额
    //    /// </summary>
    //    public decimal PaidMoneyByAlipay { get; set; }
    //    /// <summary>
    //    /// 财付通代付金额
    //    /// </summary>
    //    public decimal PaidMoneyByTenPay { get; set; }

    //    /// <summary>
    //    /// 应退金额
    //    /// </summary>
    //    public decimal ShouldRefundMoney { get; set; }

    //    /// <summary>
    //    /// 已退金额
    //    /// </summary>
    //    public decimal RealRefundMoney { get; set; }

    //}

    ///// <summary>
    ///// 退票信息
    ///// </summary>
    //public class RefundTicketInformation
    //{
    //    /// <summary>
    //    /// 数量
    //    /// </summary>
    //    public int TicketCount { get; set; }

    //    /// <summary>
    //    /// 票价
    //    /// </summary>
    //    public decimal TicketPrice { get; set; }

    //    /// <summary>
    //    /// 税费
    //    /// </summary>
    //    public decimal TaxFee { get; set; }
    //    /// <summary>
    //    /// 佣金
    //    /// </summary>
    //    public decimal Commission { get; set; }
    //    /// <summary>
    //    /// 应收
    //    /// </summary>
    //    public decimal ShouldMoney { get; set; }

    //    /// <summary>
    //    /// 实收
    //    /// </summary>
    //    public decimal RealMoney { get; set; }

    //    /// <summary>
    //    /// 代付金额
    //    /// </summary>
    //    public decimal PaidMoney { get; set; }
    //    /// <summary>
    //    /// 支付宝代付金额
    //    /// </summary>
    //    public decimal PaidMoneyByAlipay { get; set; }
    //    /// <summary>
    //    /// 财付通代付金额
    //    /// </summary>
    //    public decimal PaidMoneyByTenPay { get; set; }
    //    /// <summary>
    //    /// 应退金额
    //    /// </summary>
    //    public decimal ShouldRefundMoney { get; set; }

    //    /// <summary>
    //    /// 已退金额
    //    /// </summary>
    //    public decimal RealRefundMoney { get; set; }

    //}

    ///// <summary>
    ///// 废票信息
    ///// </summary>
    //public class InvalidTicketInformation
    //{

    //    /// <summary>
    //    /// 数量
    //    /// </summary>
    //    public int TicketCount { get; set; }

    //    /// <summary>
    //    /// 票价
    //    /// </summary>
    //    public decimal TicketPrice { get; set; }

    //    /// <summary>
    //    /// 税费
    //    /// </summary>
    //    public decimal TaxFee { get; set; }
    //    /// <summary>
    //    /// 佣金
    //    /// </summary>
    //    public decimal Commission { get; set; }
    //    /// <summary>
    //    /// 应收
    //    /// </summary>
    //    public decimal ShouldMoney { get; set; }

    //    /// <summary>
    //    /// 实收
    //    /// </summary>
    //    public decimal RealMoney { get; set; }

    //    /// <summary>
    //    /// 代付金额
    //    /// </summary>
    //    public decimal PaidMoney { get; set; }

    //    /// <summary>
    //    /// 应退金额
    //    /// </summary>
    //    public decimal ShouldRefundMoney { get; set; }

    //    /// <summary>
    //    /// 已退金额
    //    /// </summary>
    //    public decimal RealRefundMoney { get; set; }
    //}

    ///// <summary>
    ///// 改签信息
    ///// </summary>
    //public class ChangeTicketInformation
    //{
    //    /// <summary>
    //    /// 数量
    //    /// </summary>
    //    public int TicketCount { get; set; }

    //    /// <summary>
    //    /// 票价
    //    /// </summary>
    //    public decimal TicketPrice { get; set; }

    //    /// <summary>
    //    /// 税费
    //    /// </summary>
    //    public decimal TaxFee { get; set; }
    //    /// <summary>
    //    /// 佣金
    //    /// </summary>
    //    public decimal Commission { get; set; }
    //    /// <summary>
    //    /// 应收
    //    /// </summary>
    //    public decimal ShouldMoney { get; set; }

    //    /// <summary>
    //    /// 实收
    //    /// </summary>
    //    public decimal RealMoney { get; set; }

    //    /// <summary>
    //    /// 代付金额
    //    /// </summary>
    //    public decimal PaidMoney { get; set; }

    //    /// <summary>
    //    /// 应退金额
    //    /// </summary>
    //    public decimal ShouldRefundMoney { get; set; }

    //    /// <summary>
    //    /// 已退金额
    //    /// </summary>
    //    public decimal RealRefundMoney { get; set; }
    //}

    ///// <summary>
    ///// 机票取消信息
    ///// </summary>
    //public class TicketCancelInformation
    //{
    //    /// <summary>
    //    /// 数量
    //    /// </summary>
    //    public int TicketCount { get; set; }

    //    /// <summary>
    //    /// 票价
    //    /// </summary>
    //    public decimal TicketPrice { get; set; }

    //    /// <summary>
    //    /// 税费
    //    /// </summary>
    //    public decimal TaxFee { get; set; }
    //    /// <summary>
    //    /// 佣金
    //    /// </summary>
    //    public decimal Commission { get; set; }
    //    /// <summary>
    //    /// 应收
    //    /// </summary>
    //    public decimal ShouldMoney { get; set; }

    //    /// <summary>
    //    /// 实收
    //    /// </summary>
    //    public decimal RealMoney { get; set; }

    //    /// <summary>
    //    /// 代付金额
    //    /// </summary>
    //    public decimal PaidMoney { get; set; }

    //    /// <summary>
    //    /// 应退金额
    //    /// </summary>
    //    public decimal ShouldRefundMoney { get; set; }

    //    /// <summary>
    //    /// 已退金额
    //    /// </summary>
    //    public decimal RealRefundMoney { get; set; }
    //}
}
