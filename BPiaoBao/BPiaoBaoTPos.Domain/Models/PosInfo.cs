using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBaoTPos.Domain.Models
{
    public class PosInfo
    {
        /// <summary>
        /// Pos编号
        /// </summary>
        public string PosNo { get; set; }
        /// <summary>
        /// Pos费率
        /// </summary>
        public decimal PosRate { get; set; }
        /// <summary>
        /// Pos商户名
        /// </summary>
        public string BusinessmanName { get; set; }
        /// <summary>
        /// 商户ID
        /// </summary>
        public string CompanyID { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 状态（已分配,未分配）
        /// </summary>
        public string StatusStr { get; set; }
    }
    public class PosAssignLog
    {
        /// <summary>
        /// 操作员
        /// </summary>
        public string Operater { get; set; }
        /// <summary>
        /// 操作时间
        /// </summary>
        public DateTime OperateTime { get; set; }
        /// <summary>
        /// 操作内容
        /// </summary>
        public string Content { get; set; }
    }
}
