using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class ChangeOrder : AfterSaleOrder
    {
        /// <summary>
        /// 支付人Code
        /// </summary>
        public string CashBagCode { get; set; }
        /// <summary>
        /// 支付方式
        /// </summary>
        public EnumPayMethod? PayWay { get; set; }
        /// <summary>
        /// 支付时间
        /// </summary>
        public DateTime? PayTime { get; set; }
        /// <summary>
        /// 外部支付流水号
        /// </summary>
        public string OutPayNo { get; set; }
        /// <summary>
        /// 支付状态
        /// </summary>
        public EnumChangePayStatus? PayStatus { get; set; }
        /// <summary>
        /// 航班信息
        /// </summary>
        public virtual IList<AfterSaleSkyWay> SkyWay { get; set; }
        /// <summary>
        /// 已退款金额
        /// </summary>
        public decimal RefundMoney { get; set; }
        public override string AfterSaleType
        {
            get { return "改签"; }
        }
        public override void CheckRule(List<AfterSaleOrder> aftersaleOrders)
        {
            string orderD = this.Order.Policy.AnnulTicketTime.EndTime;
            if (DateTime.Compare(DateTime.Now.AddMinutes(20), DateTime.Parse(orderD)) > 0
                || DateTime.Compare(DateTime.Now, DateTime.Parse(this.Order.Policy.AnnulTicketTime.StartTime)) < 0)
                throw new CustomException(500, string.Format("申请时间不在改签时间之内，订单的改签时间为{0},请提前20分钟申请!", this.Order.Policy.AnnulTicketTime.ToString()));
            base.CheckRule(aftersaleOrders);
            //查询订单退废了的乘机人
            List<int> list = new List<int>();
            foreach (var item in aftersaleOrders.Where(p => (p is BounceOrder || p is AnnulOrder) && p.ProcessStatus == EnumTfgProcessStatus.Processed))
            {
                list.AddRange(item.Passenger.Select(x => x.PassengerId));
            }
            var passengerIds = this.Passenger.Select(x => x.PassengerId);
            list.AddRange(passengerIds);
            var pList = this.Order.Passengers.Where(p => passengerIds.Contains(p.Id)).Select(x => x.PassengerType).ToList();
            int pbabyCount = pList.Count(p => p == EnumPassengerType.Baby);
            int padultCount = pList.Count(p => p != EnumPassengerType.Baby);
            if (pbabyCount > padultCount)
                throw new CustomException(500, "一个成人只能带有一个婴儿改签");

            //未退废乘机人可以改签
            var passengerList = this.Order.Passengers.Where(p => !list.Contains(p.Id)).Select(x => x.PassengerType).ToList();
            int babyCount = passengerList.Count(p => p == EnumPassengerType.Baby);
            int adultCount = passengerList.Count(p => p != EnumPassengerType.Baby);
            if (babyCount > adultCount)
                throw new CustomException(500, "一个成人只能带有一个婴儿改签");
        }
    }
}
