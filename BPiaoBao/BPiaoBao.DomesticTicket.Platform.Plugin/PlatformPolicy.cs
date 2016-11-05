using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    public class PlatformPolicy
    {
        /// <summary>
        /// 政策编号
        /// </summary>
        public string Id
        {
            get;
            set;
        }
        /// <summary>
        /// 政策来源
        /// </summary>
        public string PlatformCode
        {
            get;
            set;
        }
        /// <summary>
        /// 政策点数
        /// </summary>
        public decimal PolicyPoint
        {
            get;
            set;
        }
        /// <summary>
        /// 现返
        /// </summary>
        public decimal ReturnMoney
        {
            get;
            set;
        }
        /// <summary>
        /// 区域城市
        /// </summary>
        public string AreaCity { get; set; }
        /// <summary>
        /// 政策备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
        /// <summary>
        /// 是否换编码出票 true是 false 否
        /// </summary>
        public bool IsChangePNRCP
        {
            get;
            set;
        }
        /// <summary>
        /// 是否特殊政策 true是 false 否 
        /// </summary>
        public bool IsSp
        {
            get;
            set;
        }
        /// <summary>
        /// 票面价
        /// </summary>
        public decimal TicketPrice
        {
            get;
            set;
        }
        /// <summary>
        /// 政策类型1.BSP 2.B2B
        /// </summary>
        public string PolicyType
        {
            get;
            set;
        }
        /// <summary>
        /// 工作时间
        /// </summary>
        public StartAndEndTime WorkTime
        {
            get;
            set;
        }
        /// <summary>
        /// 退票时间
        /// </summary>
        public StartAndEndTime ReturnTicketTime
        {
            get;
            set;
        }
        /// <summary>
        /// 废票时间
        /// </summary>
        public StartAndEndTime AnnulTicketTime
        {
            get;
            set;
        }
        /// <summary>
        /// 出票Office
        /// </summary>
        public string CPOffice
        {
            get;
            set;
        }
        /// <summary>
        /// 出票速度 分钟
        /// </summary>
        public string IssueSpeed
        {
            get;
            set;
        }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarryCode
        {
            get;
            set;
        }
        /// <summary>
        /// 低开 多个价格取低价 默认取高价
        /// </summary>
        public bool IsLow
        {
            get;
            set;
        }
        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }

        /// <summary>
        /// 机建费
        /// </summary>
        public decimal ABFee
        {
            get;
            set;
        }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal RQFee
        {
            get;
            set;
        }
        //今日供应Code
        public string TodayGYCode
        {
            get;
            set;
        }
    }
}
