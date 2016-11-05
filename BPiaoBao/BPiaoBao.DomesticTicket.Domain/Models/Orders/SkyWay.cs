using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    /// <summary>
    /// 航段信息
    /// </summary>
    public class SkyWay : EntityBase
    {
        public int Id { get; set; }
        /// <summary>
        /// 出发城市三字码
        /// </summary>
        public string FromCityCode
        {
            get;
            set;
        }
        /// <summary>
        /// 到达城市三字码
        /// </summary>
        public string ToCityCode
        {
            get;
            set;
        }
        /// <summary>
        /// 航班号
        /// </summary>
        public string FlightNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 起飞时间
        /// </summary>
        public DateTime StartDateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime ToDateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 航空公司二字码
        /// </summary>
        public string CarrayCode
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位
        /// </summary>
        public string Seat
        {
            get;
            set;
        }
        /// <summary>
        /// 出发航站楼
        /// </summary>
        public string FromTerminal
        {
            get;
            set;
        }
        /// <summary>
        /// 到达航站楼
        /// </summary>
        public string ToTerminal
        {
            get;
            set;
        }

        /// <summary>
        /// 折扣
        /// </summary>
        public decimal? Discount { get; set; }

        /// <summary>
        /// 机型
        /// </summary>
        public string FlightModel { get; set; }

        protected override string GetIdentity()
        {
            return Id.ToString();
        }
    }
}
