using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class OrderLogDto
    {
        /// <summary>
        /// 编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 订单操作日期
        /// </summary>
        public DateTime OperationDatetime
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
        /// <summary>
        /// 操作人
        /// </summary>
        public string OperationPerson
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
        
    }
}
