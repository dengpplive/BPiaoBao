using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
    public class FinancialOrderDto
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string FinancialId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 购买金额
        /// </summary>
        public decimal Money { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 购买状态
        /// </summary>
        public string Status { get; set; }
    }
}
