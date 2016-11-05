using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Domain.Models.Policies
{
    /// <summary>
    /// 获取政策需要的参数
    /// </summary>
    public class PolicyParam
    {
        public PolicyParam()
        {
            this.OrderType = 0;
            this.PolicySpecialType = EnumPolicySpecialType.Normal;
        }
        public string code { get; set; }

        public string PnrContent { get; set; }

        public int OrderType { get; set; }

        public EnumOrderSource OrderSource { get; set; }

        public string OrderId { get; set; }

        public bool IsChangePnrTicket { get; set; }
        /// <summary>
        /// 来源 true白屏  false PNR导入
        /// </summary>
        public bool IsDestine { get; set; }
        /// <summary>
        /// 多个价格(高低价格) true低价格(默认) false高价格
        /// </summary>
        public bool IsLowPrice
        {
            get;
            set;
        }
        public string defFare { get; set; }
        public string defTAX { get; set; }
        public string defRQFare { get; set; }
        /// <summary>
        /// 获取特价政策时是否重新计算
        /// </summary>
        public bool IsUseSpecial { get; set; }

        public PnrData pnrData { get; set; }

        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }

    }
}
