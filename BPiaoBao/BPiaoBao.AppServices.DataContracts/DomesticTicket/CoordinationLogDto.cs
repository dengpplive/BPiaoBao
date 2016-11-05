using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    /// <summary>
    /// 订单协调
    /// </summary>
    public class CoordinationLogDto
    {
        /// <summary>
        /// 协调日期
        /// </summary>
        public DateTime AddDatetime
        {
            get;
            set;
        }

        /// <summary>
        /// 协调内容
        /// </summary>
        public string Content
        {
            get;
            set;
        }

        /// <summary>
        /// 协调人
        /// </summary>
        public string OperationPerson
        {
            get;
            set;
        }
        /// <summary>
        /// 协调类型
        /// </summary>
        public string Type { get; set; }
    }
}
