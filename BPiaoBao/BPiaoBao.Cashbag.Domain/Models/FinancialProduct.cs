using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 理财产品
    /// </summary>
    public class FinancialProduct
    {
        /// <summary>
        /// 产品编号
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        /// 产品名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 产品描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 图片
        /// </summary>
        public string IconUrl { get; set; }
        /// <summary>
        /// 摘要
        /// </summary>
        public string Abstract { get; set; }
        /// <summary>
        /// 年息率
        /// </summary>
        public decimal ReturnRate { get; set; }
        /// <summary>
        /// 最低额度
        /// </summary>
        public decimal LimitAmount { get; set; }
        /// <summary>
        /// 最高额度
        /// </summary>
        public decimal MaxAmount { get; set; }
        /// <summary>
        /// 当前购买额
        /// </summary>
        public decimal CurrentAmount { get; set; }
        /// <summary>
        /// 产品周期
        /// </summary>
        public int Day { get; set; }
        /// <summary>
        /// 产品开始时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 产品结束时间
        /// </summary>
        public DateTime EndDate { get; set; }
        /// <summary>
        /// 有效日期
        /// </summary>
        public string ValidDate { get; set; }
        /// <summary>
        /// 利息(百分比)
        /// </summary>
        public string InterestRate { get; set; }

        /// <summary>
        /// 是否提前转出
        /// </summary>
        public bool CanSettleInAdvance { get; set; }
    }

    
}
