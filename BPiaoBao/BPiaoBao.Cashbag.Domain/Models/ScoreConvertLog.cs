using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
    /// <summary>
    /// 积分兑换记录
    /// </summary>
    public class ScoreConvertLog
    {
        /// <summary>
        /// 兑换时间
        /// </summary>
        public DateTime CreateDate { get; set; }
        /// <summary>
        /// 兑换积分
        /// </summary>
        public decimal PointAmount { get; set; }
        /// <summary>
        /// 剩余积分
        /// </summary>
        public decimal LeaveAmount { get; set; }
    }
}
