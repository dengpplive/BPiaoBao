using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class TravelPaperLogDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 操作日期
        /// </summary>
        public DateTime OperationDatetime
        {
            get;
            set;
        }
        /// <summary>
        /// 操作账号
        /// </summary>
        public string OperationPerson
        {
            get;
            set;
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string OperationType
        {
            get;
            set;
        }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string OperationContent
        {
            get;
            set;
        }
    }
}
