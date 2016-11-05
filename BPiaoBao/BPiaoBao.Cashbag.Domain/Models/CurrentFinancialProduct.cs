using BPiaoBao.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 理财记录
    /// </summary>
    public class CurrentFinancialProduct
    {
        /// <summary>
        /// 流水号
        /// </summary>
        public string SerialNum { get; set; }
        /// <summary>
        /// 购买时间
        /// </summary>
        public DateTime BuyTime { get; set; }
        /// <summary>
        /// 产品名
        /// </summary>
        public string ProductName { get; set; }
       
        /// <summary>
        /// 金额
        /// </summary>
        public decimal FinancialMoney { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 产品id
        /// </summary>
        public string ProductID { get; set; }
        /// <summary>
        /// 年息率
        /// </summary>
        public decimal ReturnRate { get; set; }

        /// <summary>
        /// 最小年息率
        /// </summary>
        public decimal MinRate { get; set; }
        /// <summary>
        /// 产品周期
        /// </summary>
        public int Day { get; set; }
        /// <summary>
        /// 生效日
        /// </summary>
        public DateTime StarDate { get; set; }
        /// <summary>
        /// 预期收益
        /// </summary>
        public decimal PreProfit { get; set; }
        /// <summary>
        /// 购买天数
        /// </summary>
        public int BuyDay { get; set; }
        /// <summary>
        /// 当前收益
        /// </summary>
        public decimal CurrentProfit { get; set; }
        /// <summary>
        /// 交易ID
        /// </summary>
        public int TradeID { get; set; }

        /// <summary>
        /// 预期结束时间
        /// </summary>
        public DateTime PreEndDate { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public string Status { get; set; }

        /// <summary>
        /// 是否提前转出
        /// </summary>
        public bool CanSettleInAdvance { get; set; }
    }
}
