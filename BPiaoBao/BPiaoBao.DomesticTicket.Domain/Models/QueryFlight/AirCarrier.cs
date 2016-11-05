using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Norm;

namespace BPiaoBao.DomesticTicket.Domain.Models
{
    /// <summary>
    /// 承运人
    /// </summary>
    public class AirCarrier
    {
        public AirCarrier()
        {
            _id = Guid.NewGuid().ToString();
        }
        public string _id { get; set; }
        /// <summary>
        /// 航空公司全称
        /// </summary>
        public string AirName
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司简称
        /// </summary>
        public string ShortName
        {
            get;
            set;
        }
        /// <summary>
        /// 结算代码
        /// </summary>
        public string SettleCode
        {
            get;
            set;
        }
        /// <summary>
        /// 预定编码Office
        /// </summary>
        public string YDOffice
        {
            get;
            set;
        }
        /// <summary>
        /// 出票Office
        /// </summary>
        public string CPOffice
        {
            get;
            set;
        }
        /// <summary>
        /// 打票机号
        /// </summary>
        public string PrintNo
        {
            get;
            set;
        }
    }
}
