using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class AfterSaleSkyWay : EntityBase
    {
        public int Id { get; set; }
        /// <summary>
        /// 原航班编号
        /// </summary>
        public int SkyWayId { get; set; }
        public virtual SkyWay SkyWay { get; private set; }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime? FlyDate { get; set; }
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime? ToDate { get; set; }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber { get; set; }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat { get; set; }



        protected override string GetIdentity()
        {
            return Id.ToString();
        }
    }
}
