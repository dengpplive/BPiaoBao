using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    public class RequestPurchaseFromCarrier
    {
        /// <summary>
        /// 数量
        /// </summary>
        public int buyCount { get; set; }
        
        /// <summary>
        /// 支付密码
        /// </summary>
        public string pwd { get; set; }

        /// <summary>
        /// 支付方式
        /// </summary>
        public EnumPayMethod payMethod { get; set; }
    }
}
