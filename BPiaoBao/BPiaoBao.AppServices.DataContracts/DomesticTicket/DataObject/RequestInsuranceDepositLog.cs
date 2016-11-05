using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
   public  class RequestInsuranceDepositLog
    {  
        /// <summary>
        /// 充值前剩余
        /// </summary>
        public int BeforeLeaveCount { get; set; }


        /// <summary>
        /// 充值后剩余
        /// </summary>
        public int AfterLeaveCount { get; set; }

        /// <summary>
        /// 充值张数
        /// </summary>
        public int DepositCount { get; set; }


        /// <summary>
        /// 充值时间 
        /// </summary>
        public DateTime BuyTime { get; set; }

        /// <summary>
        /// 单价
        /// </summary>
        public decimal SinglePrice { get; set; }

        /// <summary>
        /// 总价
        /// </summary>
        public decimal TotalPrice { get; set; }
 
    }
}
