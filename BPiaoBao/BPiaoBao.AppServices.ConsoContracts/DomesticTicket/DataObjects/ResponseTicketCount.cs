using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.DomesticTicket.DataObjects
{
    public class ResponseTicketCount
    {
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 商户名
        /// </summary>
        public string BusinessName { get; set; }
        /// <summary>
        /// 出票总量
        /// </summary>
        public int TotalCount { get; set; }
    }
}
