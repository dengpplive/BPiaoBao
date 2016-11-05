using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TicketBusniessStaticsDto
    {
        public string BusnissName { get; set; }
        public decimal IssueTicketCount { get; set; }
        public decimal PayMoney { get; set; }
        public decimal CreditMoney { get; set; }
        public decimal AccountMoney { get; set; }
        public decimal PlatFormMoney { get; set; }
        public decimal PaidMoney { get; set; }
        public List<TicketBusniessListInfo> TicketBusniessListInfo { get; set; }
        public List<TicketDayListInfo> TicketDayListInfo { get; set; }
    }
    /// <summary>
    /// 用户组
    /// </summary>
    public class TicketBusniessListInfo
    {
        public string BusnissName { get; set; }
        public decimal IssueTicketCount { get; set; }
        public decimal PayMoney { get; set; }
        public decimal CreditMoney { get; set; }
        public decimal AccountMoney { get; set; }
        public decimal PlatFormMoney { get; set; }
        public decimal PaidMoney { get; set; }
        public List<TicketDayListInfo> TicketDayListInfo { get; set; }
    }
    /// <summary>
    /// 每天销售信息
    /// </summary>
    public class TicketDayListInfo
    {
        public string Daytime { get; set; }
        /// <summary>
        /// 出票数量
        /// </summary>
        public decimal IssueTicketCount { get; set; }
        /// <summary>
        /// 交易金额
        /// </summary>
        public decimal PayMoney { get; set; }
        /// <summary>
        /// 信用支付
        /// </summary>
        public decimal CreditMoney { get; set; }
        /// <summary>
        /// 现金账户
        /// </summary>
        public decimal AccountMoney { get; set; }
        /// <summary>
        /// 第三方支付
        /// </summary>
        public decimal PlatFormMoney { get; set; }
        /// <summary>
        /// 代付金额
        /// </summary>
        public decimal PaidMoney { get; set; }
    }
    
}
