using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    /// <summary>
    /// 退票
    /// </summary>
    public class BounceOrder : AfterSaleOrder
    {
        /// <summary>
        /// 是否自愿 true为自愿，默认非自愿
        /// </summary>
        public bool IsVoluntary { get; set; }
        /// <summary>
        /// 附件地址
        /// </summary>
        public string AttachmentUrl { get; set; }
        /// <summary>
        /// 退款明细
        /// </summary>
        public virtual IList<BounceLine> BounceLines { get; set; }
        /// <summary>
        /// 售后类型1
        /// </summary>
        public override string AfterSaleType
        {
            get { return "退票"; }
        }
        public override void CheckRule(List<AfterSaleOrder> aftersaleOrders)
        {
            base.CheckRule(aftersaleOrders);
            //查询订单退废了的乘机人
            List<int> list = new List<int>();
            foreach (var item in aftersaleOrders.Where(p => (p is BounceOrder || p is AnnulOrder) && p.ProcessStatus == EnumTfgProcessStatus.Processed))
            {
                list.AddRange(item.Passenger.Select(x => x.PassengerId));
            }
            list.AddRange(this.Passenger.Select(x => x.PassengerId));
            //未退乘机人
            var passengerList = this.Order.Passengers.Where(p => !list.Contains(p.Id)).Select(x => x.PassengerType).ToList();
            int babyCount = passengerList.Count(p => p == EnumPassengerType.Baby);
            int adultCount = passengerList.Count(p => p != EnumPassengerType.Baby);
            if (babyCount > adultCount)
                throw new CustomException(500, "一个婴儿必须有一个成人");

        }
    }
    public class BounceLine : EntityBase
    {
        public string ID { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public EnumPayMethod PayMethod { get; set; }
        /// <summary>
        /// 乘机人名称
        /// </summary>
        public string PassgenerName { get; set; }
        /// <summary>
        /// 改签单
        /// </summary>
        public int? ChangeOrderID { get; set; }
        public virtual ChangeOrder ChangeOrder { get; set; }
        /// <summary>
        /// 支付流水号
        /// </summary>
        public string PaySerialNumber { get; set; }
        /// <summary>
        /// 订单
        /// </summary>
        public string OrderID { get; set; }
        public virtual Order Order { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundMoney { get; set; }

        /// <summary>
        /// 退款时间
        /// </summary>
        public DateTime? RefundTime { get; set; }
        /// <summary>
        /// 退款格式
        /// </summary>
        public string BusArgs { get; set; }
        /// <summary>
        /// 退款状态
        /// </summary>
        public EnumBoundRefundStatus Status { get; set; }
        /// <summary>
        /// 退款服务费
        /// </summary>
        public decimal RefundServiceMoney { get; set; }

        protected override string GetIdentity()
        {
            return ID.ToString();
        }
    }
    public enum EnumBoundRefundStatus : int
    {
        [Description("未退款")]
        NoRefund = -1,
        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        Refunding = 0,
        /// <summary>
        /// 退款完成
        /// </summary>
        [Description("退款完成")]
        Refunded = 1
    }

}
