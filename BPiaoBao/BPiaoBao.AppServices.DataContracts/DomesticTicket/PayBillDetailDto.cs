using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class PayBillDetailDto
    {
        public int ID { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 钱袋子合作方
        /// </summary>
        public string CashbagCode
        {
            get;
            set;
        }
        /// <summary>
        /// 商户名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money
        {
            get;
            set;
        }
        /// <summary>
        /// 点数
        /// </summary>
        public decimal Point
        {
            get;
            set;
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public string OpType
        {
            get;
            set;
        }
        /// <summary>
        /// 扣点类型
        /// </summary>
        public AdjustType AdjustType
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
