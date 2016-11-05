using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using BPiaoBao.Common.Enums;

namespace BPiaoBao.DomesticTicket.Domain.Models.Orders
{
    public enum EnumOperationType
    {
        /// <summary>
        /// 分润
        /// </summary>
        [Description("分润")]
        Profit = 0,
        /// <summary>
        /// 收款
        /// </summary>
        [Description("收款")]
        Receivables = 1,
        /// <summary>
        /// 付款
        /// </summary>
        [Description("付款")]
        PayMoney = 2,
        /// <summary>
        /// 出票服务费
        /// </summary>
        [Description("服务费")]
        IssuePayServer = 3,
        /// <summary>
        /// 运营商收取服务费
        /// </summary>
        [Description("服务费")]
        CarrierRecvServer = 4,
        /// <summary>
        /// 运营商支出服务费
        /// </summary>
        [Description("服务费")]
        CarrierPayServer = 5,
        /// <summary>
        /// 运营商支出分润服务费
        /// </summary>
        [Description("分润服务费")]
        CarrierPayProfitServer = 6,
        /// <summary>
        /// 合作者收取分润服务费
        /// </summary>
        [Description("分润服务费")]
        ParterProfitServer = 7,
        /// <summary>
        /// 合作者收取服务费
        /// </summary>
        [Description("服务费")]
        ParterServer = 8,
        /// <summary>
        /// 保留费用
        /// </summary>
        [Description("保留")]
        ParterRetainServer = 9,
        /// <summary>
        /// 保险金额
        /// </summary>
        [Description("保险")]
        Insurance = 10,
        /// <summary>
        /// 保险服务费
        /// </summary>
        [Description("保险服务费")]
        InsuranceServer = 11,
        /// <summary>
        /// 保险收款
        /// </summary>
        [Description("保险收款")]
        InsuranceReceivables = 12,
        /// <summary>
        /// 系统分润（平台扣点）
        /// </summary>
        [Description("系统分润")]
        ParterProfit = 13
    }
    /// <summary>
    /// 收支明细
    /// </summary>
    public class PayBillDetail
    {

        public int ID { get; set; }
        /// <summary>
        /// 商户号
        /// </summary>
        public string Code
        {
            get;
            set;
        }
        /// <summary>
        /// 钱袋子合作方
        /// </summary>
        public string CashbagCode
        {
            get;
            set;
        }
        /// <summary>
        /// 商户名
        /// </summary>
        public string Name
        {
            get;
            set;
        }
        /// <summary>
        /// 金额
        /// </summary>
        public decimal Money
        {
            get;
            set;
        }
        /// <summary>
        /// 婴儿服务费 金额
        /// </summary>
        public decimal InfMoney
        {
            get;
            set;
        }
        /// <summary>
        /// 点数
        /// </summary>
        public decimal Point
        {
            get;
            set;
        }
        /// <summary>
        /// 操作类型
        /// </summary>
        public EnumOperationType OpType
        {
            get;
            set;
        }
        /// <summary>
        /// 扣点类型
        /// </summary>
        public AdjustType AdjustType
        {
            get;
            set;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get;
            set;
        }
    }
}
