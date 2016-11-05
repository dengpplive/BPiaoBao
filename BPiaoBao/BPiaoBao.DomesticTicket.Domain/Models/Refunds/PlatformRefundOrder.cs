using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Orders;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Refunds
{
    /// <summary>
    /// 平台退款单
    /// </summary>
    public class PlatformRefundOrder : EntityBase, IAggregationRoot
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
        /// 退款日志
        /// </summary>
        public virtual IList<OrderLog> RefundLogs { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }


        public void ConfirmRefund(decimal amount, DateTime refundTime, string operatorName)
        {
            this.RefundAmount = amount;
            this.RefundTime = refundTime;
            this.RefundLogs.Add(new OrderLog()
            {
                IsShowLog = false,
                OperationContent = "退款",
                OperationDatetime = DateTime.Now,
                OperationPerson = operatorName
            });
            DomainEvents.Raise(new RefundEvent() { RefundOrder = this });
        }


        protected override string GetIdentity()
        {
            return Id.ToString();
        }
    }

}
