using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using Norm;
using System.Reflection;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Domain.Models
{
    /// <summary>
    /// 政策缓存
    /// </summary>
    public class PolicyCache : IAggregationRoot
    {
        public PolicyCache()
        {
            _id = Guid.NewGuid().ToString();
            this.CacheDate = System.DateTime.Now;
            this.CacheExpiresDate = DateTime.Parse("1901-01-01");
            this.TravelType = TravelType.Oneway;
            this.CabinSeatCode = new string[] { };
            this.SuitableFlightNo = new string[] { };
            this.ExceptedFlightNo = new string[] { };
            this.SuitableWeek = new DayOfWeek[] { };
            this.Applay = EnumApply.All;
            this.PolicySourceType = EnumPolicySourceType.Interface;
            this.CheckinTime = new TimePeriod()
            {
                FromTime = DateTime.Parse("1901-01-01"),
                EndTime = DateTime.Parse("1901-01-01")
            };
            this.IssueTime = new TimePeriod()
            {
                FromTime = DateTime.Parse("1901-01-01"),
                EndTime = DateTime.Parse("1901-01-01")
            };
            this.ServiceTime = new WorkTime()
            {
                WeekendTime = new TimePeriod()
                {
                    FromTime = DateTime.Parse("1901-01-01"),
                    EndTime = DateTime.Parse("1901-01-01")
                },
                WeekTime = new TimePeriod()
                {
                    FromTime = DateTime.Parse("1901-01-01"),
                    EndTime = DateTime.Parse("1901-01-01")
                }
            };
            this.TFGTime = new WorkTime()
            {
                WeekendTime = new TimePeriod()
                {
                    FromTime = DateTime.Parse("1901-01-01"),
                    EndTime = DateTime.Parse("1901-01-01")
                },
                WeekTime = new TimePeriod()
                {
                    FromTime = DateTime.Parse("1901-01-01"),
                    EndTime = DateTime.Parse("1901-01-01")
                }
            };
        }
        public string _id { get; set; }
        /// <summary>
        /// 政策ID
        /// </summary>
        public string PolicyId { get; set; }
        /// <summary>
        /// 政策来源
        /// </summary>
        public string PlatformCode { get; set; }
        /// <summary>
        /// 承运人二字码
        /// </summary>
        public string CarrierCode { get; set; }
        /// <summary>
        /// 适用舱位
        /// </summary>
        public string[] CabinSeatCode { get; set; }

        /// <summary>
        /// 出发城市 多个用"/"分割
        /// </summary>
        public string FromCityCode { get; set; }
        /// <summary>
        ///  中转城市 多个用"/"分割
        /// </summary>
        public string MidCityCode { get; set; }
        /// <summary>
        ///  到达城市 多个用"/"分割
        /// </summary>
        public string ToCityCode { get; set; }
        /// <summary>
        /// 使用航班类型
        /// </summary>
        public EnumApply Applay { get; set; }
        /// <summary>
        /// 适用航班
        /// </summary>
        public string[] SuitableFlightNo { get; set; }
        /// <summary>
        /// 排除航班
        /// </summary>
        public string[] ExceptedFlightNo { get; set; }
        /// <summary>
        /// 适用星期
        /// </summary>
        public DayOfWeek[] SuitableWeek { get; set; }
        /// <summary>
        /// 乘机有效期
        /// </summary>
        public TimePeriod CheckinTime { get; set; }
        /// <summary>
        /// 出票有效期
        /// </summary>
        public TimePeriod IssueTime { get; set; }
        /// <summary>
        /// 服务时间
        /// </summary>
        public WorkTime ServiceTime { get; set; }
        /// <summary>
        /// 退废改时间
        /// </summary>
        public WorkTime TFGTime { get; set; }
        /// <summary>
        /// 政策备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 行程类型
        /// </summary>
        public TravelType TravelType { get; set; }
        /// <summary>
        /// 政策类型
        /// </summary>
        public PolicyType PolicyType { get; set; }
        /// <summary>
        /// 政策点数
        /// </summary>
        public decimal Point { get; set; }
        /// <summary>
        /// 原始政策点数
        /// </summary>
        public decimal OldPoint { get; set; }
        /// <summary>
        /// 缓存日期
        /// </summary>
        public DateTime CacheDate { get; set; }
        /// <summary>
        /// 缓存有效期
        /// </summary>
        public DateTime CacheExpiresDate { get; set; }
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }
        /// <summary>
        /// 特价价格或者折扣
        /// </summary>
        public decimal SpecialPriceOrDiscount { get; set; }
        /// <summary>
        /// 政策来源类型 本地 接口 共享
        /// </summary>
        public EnumPolicySourceType PolicySourceType { get; set; }
    }

    public enum TravelType
    {
        /// <summary>
        /// 单程
        /// </summary>        
        [Description("单程")]
        Oneway = 1,
        /// <summary>
        /// 往返
        /// </summary>
        [Description("往返")]
        Twoway = 2,
        /// <summary>
        /// 联程
        /// </summary>
        [Description("联程")]
        Connway = 3,
        /// <summary>
        /// 单程/往返
        /// </summary>
        [Description("单程/往返")]
        OneTwoway = 4
    }

    public enum PolicyType
    {
        /// <summary>
        /// B2B
        /// </summary>
        [Description("B2B")]
        B2B = 0,
        /// <summary>
        /// BSP
        /// </summary>
        [Description("BSP")]
        BSP = 1
    }

    public class TimePeriod
    {
        public TimePeriod()
        {
            this.FromTime = DateTime.Parse("1901-01-01");
            this.EndTime = DateTime.Parse("1901-01-01");
        }

        public DateTime FromTime { get; set; }
        public DateTime EndTime { get; set; }
    }

    public class WorkTime
    {
        public TimePeriod WeekTime { get; set; }
        public TimePeriod WeekendTime { get; set; }
    }

    /// <summary>
    /// 查询政策的参数类
    /// </summary>
    public class PolicyQueryParam
    {
        public List<QueryParam> InputParam { get; set; }
        public TravelType TravelType { get; set; }

        public override string ToString()
        {
            StringBuilder sblog = new StringBuilder();
            sblog.Append("TravelType=" + this.TravelType);
            for (int i = 0; i < this.InputParam.Count; i++)
            {
                QueryParam item = this.InputParam[i];
                PropertyInfo[] properties = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.GetProperty);
                object obj = null;
                foreach (PropertyInfo p in properties)
                {
                    obj = p.GetValue(item, null);
                    sblog.Append(p.Name + "=" + (obj == null ? "null" : obj) + "\r\n");
                }
            }
            return sblog.ToString();
        }
    }

}
