using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Norm;

namespace BPiaoBao.DomesticTicket.Domain.Models
{
    /// <summary>
    /// 基本舱位折扣
    /// </summary>
    public class BaseCabin
    {
        public BaseCabin()
        {
            // _id = ObjectId.NewObjectId();
        }
        //public ObjectId _id { get; set; }
        /// <summary>
        /// 承运人二字码
        /// </summary>
        public string CarrierCode { get; set; }
        /// 基本舱位代码
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 基本折扣
        /// </summary>
        public decimal Rebate { get; set; }

    }
}
