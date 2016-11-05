using BPiaoBao.DomesticTicket.Platform.Plugin;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;
using BPiaoBao.DomesticTicket.Domain.Models.Policies;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public class Policy : EntityBase
    {
        public Policy()
        {
            this.DeductionDetails = new List<DeductionDetail>();
        }
        public string OrderId { get; set; }
        /// <summary>
        /// 拥金
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 区域城市
        /// </summary>
        public string AreaCity { get; set; }
        /// <summary>
        /// 政策编号
        /// </summary>
        public string PolicyId { get; set; }
        /// <summary>
        /// 平台Code(517,8000YI,51Book,BaiTuo,PiaoMeng,Today,YeeXing,系统)
        /// </summary>
        public string PlatformCode { get; set; }
        /// <summary>
        /// 政策点数（最终）
        /// </summary>
        public decimal PolicyPoint { get; set; }
        /// <summary>
        /// 平台对供应和运营的扣点数
        /// </summary>
        public decimal DownPoint { get; set; }
        /// <summary>
        /// 原始政策点数
        /// </summary>
        public decimal OriginalPolicyPoint { get; set; }
        /// <summary>
        /// 供应和运营代付点数
        /// </summary>
        public decimal PaidPoint { get; set; }
        /// <summary>
        /// 现返
        /// </summary>
        public decimal ReturnMoney { get; set; }
        /// <summary>
        /// 是否换编码出票 true是 false 否
        /// </summary>
        public bool IsChangePNRCP { get; set; }

        /// <summary>
        /// 出票方式 
        /// </summary>
        public EnumIssueTicketWay EnumIssueTicketWay { get; set; }

        /// <summary>
        /// 是否特殊政策 true是 false 否 
        /// </summary>
        public bool IsSp { get; set; }

        /// <summary>
        /// true低开 false高开  多个价格取低价 默认取高价
        /// </summary>
        public bool IsLow { get; set; }

        /// <summary>
        /// 政策类型1.BSP 2.B2B
        /// </summary>
        public string PolicyType { get; set; }
        /// <summary>
        /// 工作时间
        /// </summary>
        public StartAndEndTime WorkTime { get; set; }
        /// <summary>
        /// 退票时间
        /// </summary>
        public StartAndEndTime ReturnTicketTime { get; set; }
        /// <summary>
        /// 废票时间
        /// </summary>
        public StartAndEndTime AnnulTicketTime { get; set; }
        /// <summary>
        /// 出票Office
        /// </summary>
        public string CPOffice { get; set; }
        /// <summary>
        /// 出票速度 分钟
        /// </summary>
        public string IssueSpeed { get; set; }

        /// <summary>
        /// 政策备注
        /// </summary>
        public string Remark { get; set; }
        public virtual Order Order { get; set; }

        /// <summary>
        /// 扣点明细
        /// </summary>
        public virtual List<DeductionDetail> DeductionDetails { get; set; }
        /// <summary>
        /// 政策来源类型 本地 接口 共享
        /// </summary>
        public EnumPolicySourceType PolicySourceType { get; set; }
        /// <summary>
        /// 承运人
        /// </summary>
        public string CarryCode { get; set; }
        /// <summary>
        /// 政策所属角色 运营 供应
        /// </summary>
        public EnumPolicyOwnUserRole PolicyOwnUserRole { get; set; }
        /// <summary>
        /// 政策所属商户号(谁发的政策)
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 政策所属商户名
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 钱袋子商户Code
        /// </summary>
        public string CashbagCode { get; set; }
        /// <summary>
        /// 系统对运营商或者运营商对供应设置的服务费率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 运营商code 如果是供应的政策则为他的上级运营商的code
        /// </summary>
        public string CarrierCode { get; set; }

        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }

        /// <summary>
        /// 机建费
        /// </summary>
        public decimal ABFee { get; set; }
        /// <summary>
        /// 燃油费
        /// </summary>
        public decimal RQFee { get; set; }
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }
        /// <summary>
        /// 特价价格或者折扣
        /// </summary>
        public decimal SpecialPriceOrDiscount { get; set; }

        /// <summary>
        /// 是否协调【设置最大点数】
        /// </summary>
        public bool IsCoordination { get; set; }
        /// <summary>
        /// 协调点数
        /// </summary>
        public decimal? MaxPoint { get; set; }
        /// <summary>
        /// 今日供应Code
        /// </summary>
        public string TodayGYCode
        {
            get;
            set;
        }
        protected override string GetIdentity()
        {
            return OrderId.ToString();
        }
    }
}
