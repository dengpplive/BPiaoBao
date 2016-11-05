using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket.DataObject
{
    public class RequestInsurance
    {
        /// <summary>
        /// 购买航意险和急速退保单的乘客
        /// </summary>
        public List<PassengerDto> UnexpectedPassenger { get; set; }
         
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 购买航意险保险份数
        /// </summary>
        public int BuyInsuranceAllCount { get; set; }
         
    }
}
