using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TicketInfoSummaryEntity
    {
        /// <summary>
        /// 机票状态
        /// </summary>
        public string TicketStatus { get; set; }
        /// <summary>
        /// 数量
        /// </summary>
        public string TicketCount { get; set; }
        /// <summary>
        /// 票价
        /// </summary>
        public string TicketPrice { get; set; }
        /// <summary>
        /// 税费
        /// </summary>
        public string TaxFee { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public string Commission { get; set; }
        /// <summary>
        /// 应收
        /// </summary>
        public string ShouldMoney { get; set; }
        /// <summary>
        /// 已收
        /// </summary>
        public string RealMoney { get; set; }
        /// <summary>
        /// 代付金额
        /// </summary>
        public string PaidMoney { get; set; }
        /// <summary>
        /// 应退金额
        /// </summary>
        public string ShouldRefundMoney { get; set; }
        /// <summary>
        /// 已退金额
        /// </summary>
        public string RealRefundMoney { get; set; }
    }
}
