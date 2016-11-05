using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.DomesticTicket
{
    public class PolicyPack
    {
        public PolicyPack()
        {
            OrderSource = EnumOrderSource.PnrContentImport;
        }

        /// <summary>
        /// 政策列表
        /// </summary>
        public List<PolicyDto> PolicyList = new List<PolicyDto>();
        /// <summary>
        /// 成人订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 儿童订单号
        /// </summary>
        public string ChdOrderId { get; set; }

        /// <summary>
        /// 编码中的婴儿人数和预定的婴儿人数是否一致
        /// </summary>
        public bool INFPnrIsSame = true;
        /// <summary>
        /// 成人编码
        /// </summary>
        public string AdultPnr { get; set; }
        /// <summary>
        /// 儿童编码
        /// </summary>
        public string ChdPnr { get; set; }
        /// <summary>
        /// 订单来源
        /// </summary>
        public EnumOrderSource OrderSource { get; set; }

    }
    public class PolicyDto
    {
        /// <summary>
        /// 平台Code(517,8000YI,51Book,BaiTuo,PiaoMeng,Today,YeeXing,系统)
        /// </summary>
        public string PlatformCode { get; set; }
        /// <summary>
        /// 平台名称
        /// </summary>
        public string PlatformName { get; set; }
        /// <summary>
        /// 政策编号
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 政策点数(分销的点数)
        /// </summary>
        public decimal Point { get; set; }
        /// <summary>
        /// 扣点数
        /// </summary>
        public decimal DownPoint
        {
            get;
            set;
        }
        /// <summary>
        /// 原始政策点数
        /// </summary>
        public decimal OriginalPolicyPoint
        {
            get;
            set;
        }
        /// <summary>
        /// 代付点数 供应和运营看到的点数
        /// </summary>
        public decimal PaidPoint
        {
            get;
            set;
        }
        /// <summary>
        /// 票面价
        /// </summary>
        public decimal TicketPrice { get; set; }
        /// <summary>
        /// 佣金
        /// </summary>
        public decimal Commission { get; set; }
        /// <summary>
        /// 现返
        /// </summary>
        public decimal ReturnMoney
        {
            get;
            set;
        }

        /// <summary>
        /// 舱位价
        /// </summary>
        public decimal SeatPrice { get; set; }

        /// <summary>
        /// 单人支付金额
        /// </summary>
        public decimal PayMoney { get; set; }

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
        /// <summary>
        /// 政策类型 B2B BSP
        /// </summary>
        public string PolicyType { get; set; }
        /// <summary>
        /// 出票速度
        /// </summary>
        public string IssueSpeed { get; set; }
        /// <summary>
        /// 工作时间
        /// </summary>
        public string WorkTime { get; set; }
        /// <summary>
        /// 退票时间
        /// </summary>
        public string ReturnTicketTime { get; set; }
        /// <summary>
        /// 废票时间
        /// </summary>
        public string AnnulTicketTime { get; set; }

        /// <summary>
        /// 退费改时间
        /// </summary>
        public string TFGTime { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 区域城市
        /// </summary>
        public string AreaCity { get; set; }

        /// <summary>
        /// 政策来源用于对用数据库实体  0本地 1接口 2共享
        /// </summary>
        public string PolicySourceType { get; set; }
        /// <summary>
        /// 出票方式 0.手动  1.自动
        /// </summary>
        public string IssueTicketWay { get; set; }
        /// <summary>
        /// 界面上显示政策来源
        /// </summary>
        public string ShowPolicySource { get; set; }

        /// <summary>
        /// 是否特殊政策 true是 false 否 
        /// </summary>
        public bool IsSp { get; set; }

        /// <summary>
        /// 是否换编码出票 true是 false 否
        /// </summary>
        public bool IsChangePNRCP { get; set; }

        /// <summary>
        /// true低开 false高开  多个价格取低价 默认取高价
        /// </summary>
        public bool IsLow
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
        /// 承运人
        /// </summary>
        public string CarryCode
        {
            get;
            set;
        }
        /// <summary>
        /// 政策所属角色 运营 供应 (谁发的政策)
        /// </summary>
        public string PolicyOwnUserRole
        {
            get;
            set;
        }
        /// <summary>
        /// 运营商code 如果是供应的政策则为他的上级运营商的code
        /// </summary>
        public string CarrierCode { get; set; }

        /// <summary>
        /// 政策所属商户号(谁发的政策)
        /// </summary>
        public string Code { get; set; }
        /// <summary>
        /// 政策所属商户名(谁发的政策)
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 钱袋子商户Code (谁发的政策)
        /// </summary>
        public string CashbagCode { get; set; }
        /// <summary>
        /// 系统对运营商或者运营商对供应设置的服务费率
        /// </summary>
        public decimal Rate { get; set; }

        /// <summary>
        /// 默认政策类型 0成人默认政策 1儿童默认政策
        /// </summary>
        public int DefaultPolicySource { get; set; }
        /// <summary>
        /// 政策特殊类型
        /// </summary>
        public EnumPolicySpecialType PolicySpecialType { get; set; }
        /// <summary>
        /// 特价价格或者折扣
        /// </summary>
        public decimal SpecialPriceOrDiscount { get; set; }
        /// <summary>
        /// 今日供应Code
        /// </summary>
        public string TodayGYCode
        {
            get;
            set;
        }


    }
}
