using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class OrderDetailDto : OrderDto
    {
        /// <summary>
        /// 日志信息
        /// </summary>
        public virtual IList<OrderLogDto> OrderLogs
        {
            get;
            set;
        }
    }
}
