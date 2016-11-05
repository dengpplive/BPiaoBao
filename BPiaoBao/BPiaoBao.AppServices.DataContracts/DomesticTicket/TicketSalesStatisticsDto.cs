using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{



    /// <summary>
    /// 机票销售统计
    /// </summary>
    public class TicketSalesStatisticsDto
    {
        /// <summary>
        /// 出票地
        /// </summary>
        public string PolicyCode { get; set; }

        //public Dictionary<string, List<TicketSalesStatistics>> PolicyCodeDic = new Dictionary<string, List<TicketSalesStatistics>>();

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




        /// <summary>
        /// 取消数量
        /// </summary>
        public int CancelTicketCount { get; set; }

        /// <summary>
        /// 取消票价
        /// </summary>
        public decimal CancelTickePrice { get; set; }

        /// <summary>
        /// 取消税费
        /// </summary>
        public decimal CancelTickeTaxFee { get; set; }

        /// <summary>
        /// 取消佣金
        /// </summary>
        public decimal CancelTickeCommission { get; set; }

        /// <summary>
        /// 取消应退
        /// </summary>
        public decimal CancelTickeShouldMoney { get; set; }

        /// <summary>
        /// 取消已退
        /// </summary>
        public decimal CancelTickeRealMoney { get; set; }
        /// <summary>
        /// 出票地
        /// </summary>
        public List<TicketSalesStatistics> PolicyCodeDic { get; set; }
    }


    public class TicketSalesStatistics
    {
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




        /// <summary>
        /// 取消数量
        /// </summary>
        public int CancelTicketCount { get; set; }

        /// <summary>
        /// 取消票价
        /// </summary>
        public decimal CancelTickePrice { get; set; }

        /// <summary>
        /// 取消税费
        /// </summary>
        public decimal CancelTickeTaxFee { get; set; }

        /// <summary>
        /// 取消佣金
        /// </summary>
        public decimal CancelTickeCommission { get; set; }

        /// <summary>
        /// 取消应退
        /// </summary>
        public decimal CancelTickeShouldMoney { get; set; }

        /// <summary>
        /// 取消已退
        /// </summary>
        public decimal CancelTickeRealMoney { get; set; }
        /// <summary>
        /// 航空公司2字码
        /// </summary>
        public List<TicketSalesCarrayCode> CarrayCodeDic { get; set; }
    }

    public class TicketSalesCarrayCode
    {
        /// <summary>
        /// 航空公司2字码
        /// </summary>
        public string CarrayCode { get; set; }

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




        /// <summary>
        /// 取消数量
        /// </summary>
        public int CancelTicketCount { get; set; }

        /// <summary>
        /// 取消票价
        /// </summary>
        public decimal CancelTickePrice { get; set; }

        /// <summary>
        /// 取消税费
        /// </summary>
        public decimal CancelTickeTaxFee { get; set; }

        /// <summary>
        /// 取消佣金
        /// </summary>
        public decimal CancelTickeCommission { get; set; }

        /// <summary>
        /// 取消应退
        /// </summary>
        public decimal CancelTickeShouldMoney { get; set; }

        /// <summary>
        /// 取消已退
        /// </summary>
        public decimal CancelTickeRealMoney { get; set; }
    }
}
