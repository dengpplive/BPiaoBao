using BPiaoBao.Common.Enums;
using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class AnnulOrder : AfterSaleOrder
    {
        /// <summary>
        /// 退款明细
        /// </summary>
        public virtual IList<BounceLine> BounceLines { get; set; }

        public override string AfterSaleType
        {
            get { return "废票"; }
        }
        /// <summary>
        /// 附件地址
        /// </summary>
        public string AttachmentUrl { get; set; }
        public override void CheckRule(List<AfterSaleOrder> aftersaleOrders)
        {
            if (DateTime.Compare(DateTime.Now.Date, this.Order.IssueTicketTime.Value.Date) != 0)
                throw new CustomException(500, "废票只能在当天进行操作!");
            if (DateTime.Compare(DateTime.Now.AddMinutes(150), this.Order.SkyWays.FirstOrDefault().StartDateTime) > 0)
                throw new CustomException(500, "当天废票只能在起飞前150分钟提交");
            string orderD = this.Order.Policy.AnnulTicketTime.EndTime;
            if (DateTime.Compare(DateTime.Now.AddMinutes(20), DateTime.Parse(orderD)) > 0 
                || DateTime.Compare(DateTime.Now, DateTime.Parse(this.Order.Policy.AnnulTicketTime.StartTime)) < 0)
                throw new CustomException(500, string.Format("申请时间不在废票时间之内，订单的废票时间为{0},请提前20分钟申请!", this.Order.Policy.AnnulTicketTime.ToString()));
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
}
