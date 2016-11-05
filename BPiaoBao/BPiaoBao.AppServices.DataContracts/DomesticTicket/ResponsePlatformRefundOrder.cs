using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
   public class ResponsePlatformRefundOrder
   {
       /// <summary>
       /// 编号
       /// </summary>
       public int Id { get; set; }
       /// <summary>
       /// 退款类型
       /// </summary>
       public EnumPlatformRefundType RefundType { get; set; }
       /// <summary>
       /// 退款订单号
       /// </summary>
       public string RefundOrderId { get; set; }
       /// <summary>
       /// 退款状态
       /// </summary>
       public EnumPlatformRefundStatus RefundStatus { get; set; }
       /// <summary>
       /// 退款金额
       /// </summary>
       public decimal? RefundAmount { get; set; }
       /// <summary>
       /// 退款时间
       /// </summary>
       public DateTime? RefundTime { get; set; }
       /// <summary>
       /// 备注
       /// </summary>
       public string Remark { get; set; }
    }
}
