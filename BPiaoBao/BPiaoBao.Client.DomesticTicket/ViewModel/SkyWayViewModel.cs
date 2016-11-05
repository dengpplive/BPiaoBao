using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.DomesticTicket.ViewModel
{
    public class SkyWayViewModel
    {
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
        /// 出发城市名称
        /// </summary>
        public string FromCity
        {
            get;
            set;
        }
        /// <summary>
        /// 到达城市名称
        /// </summary>
        public string ToCity
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
        /// 航空公司简称
        /// </summary>
        public string CarrayShortName
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
        /// 起飞时间
        /// </summary>
        public DateTime NewStartDateTime
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// 航班号
        /// </summary>
        public string NewFlightNumber
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位
        /// </summary>
        public string NewSeat
        {
            get;
            set;
        }
        /// <summary>
        /// 到达时间
        /// </summary>
        public DateTime NewToDateTime { get; set; }
    }
}
