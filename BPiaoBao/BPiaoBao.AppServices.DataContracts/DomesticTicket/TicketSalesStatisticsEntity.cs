using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TicketSalesStatisticsEntity
    {
        public int Id { get; set; }
        /// <summary>
        /// 出票地
        /// </summary>
        public string PolicyCode { get; set; }

        /// <summary>
        /// 出票张数
        /// </summary>
        public int IssueTicketCount { get; set; }
        /// <summary>
        /// 出票票价
        /// </summary>
        public decimal IssueTicketPrice { get; set; }
        /// <summary>
        /// 出票税费
        /// </summary>
        public decimal IssueTicketTaxFee { get; set; }
        /// <summary>
        /// 出票佣金
        /// </summary>
        public decimal IssueTicketCommission { get; set; }


        /// <summary>
        /// 信用支付
        /// </summary>
        public decimal IssueCreditShouldMoney { get; set; }
        /// <summary>
        /// 现金账户
        /// </summary>
        public decimal IssueAccountShouldMoney { get; set; }
        /// <summary>
        /// 第三方支付
        /// </summary>
        public decimal IssuePlatFormShouldMoney { get; set; }


        /// <summary>
        /// 应收
        /// </summary>
        public decimal IssueTicketShouldMoney { get; set; }
        /// <summary>
        /// 已收
        /// </summary>
        public decimal IssueTicketRealMoney { get; set; }

        /// <summary>
        /// 退票张数
        /// </summary>
        public int RefundTicketCount { get; set; }
        /// <summary>
        /// 退票票价
        /// </summary>
        public decimal RefundTicketPrice { get; set; }

        /// <summary>
        /// 退票税费
        /// </summary>
        public decimal RefundTicketTaxFee { get; set; }

        /// <summary>
        /// 退票佣金
        /// </summary>
        public decimal RefundTicketCommission { get; set; }

        /// <summary>
        /// 应退
        /// </summary>
        public decimal RefundTicketShouldMoney { get; set; }

        /// <summary>
        /// 已退
        /// </summary>
        public decimal RefundTicketRealMoney { get; set; }

        /// <summary>
        /// 废票数量
        /// </summary>
        public int InvalidTicketCount { get; set; }

        /// <summary>
        /// 废票价格
        /// </summary>
        public decimal InvalidTicketPrice { get; set; }

        /// <summary>
        /// 废票税费
        /// </summary>
        public decimal InvalidTicketTaxFee { get; set; }

        /// <summary>
        /// 废票佣金
        /// </summary>
        public decimal InvalidTicketCommission { get; set; }

        /// <summary>
        /// 应废
        /// </summary>
        public decimal InvalidTicketShouldMoney { get; set; }

        /// <summary>
        /// 已废
        /// </summary>
        public decimal InvalidTicketRealMoney { get; set; }
        public int? _parentId { get; set; }
        public string state { get; set; }
    }
    public class TicketSalesSumEntity
    {
        public int total{get;set;}
        public List<TicketSalesStatisticsEntity> rows{get;set;}
    }
}
