using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Norm;
namespace BPiaoBao.DomesticTicket.Domain.Models
{
    /// <summary>
    /// 机型
    /// </summary>
    public class AirplainType
    {
        public AirplainType()
        {
            _id = ObjectId.NewObjectId();
        }
        public ObjectId _id { get; set; }
        /// <summary>
        /// 编号
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 机建费
        /// </summary>
        public decimal TaxFee { get; set; }
    }
}
