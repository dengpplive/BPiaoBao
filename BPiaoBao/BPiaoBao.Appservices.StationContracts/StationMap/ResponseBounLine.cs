using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.StationMap
{
    public class ResponseBounLine
    {
        /// <summary>
        /// 退款单号
        /// </summary>
        public string ID { get; set; }
        public string RefundType
        {
            get
            {
                return ChangeOrderID.HasValue ? "改签退款" : "订单退款";
            }
        }
        public string Num
        {
            get
            {
                return ChangeOrderID.HasValue ? ChangeOrderID.Value.ToString() : OrderID;
            }
        }
        /// <summary>
        /// 改签单
        /// </summary>
        public int? ChangeOrderID { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public string OrderID { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundMoney { get; set; }
        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime? RefundTime { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public string Status { get; set; }
    }
}
