using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class CoordinationDto : OrderDto
    {
        /// <summary>
        /// 协调信息
        /// </summary>
        public virtual IList<CoordinationLogDto> CoordinationLogs
        {
            get;
            set;
        }
    }
}
