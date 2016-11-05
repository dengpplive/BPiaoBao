using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
    public class PosInfoDataObject
    {
        /// <summary>
        /// 商户ID
        /// </summary>
        public string CompanyID { get; set; }
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
        /// 状态
        /// </summary>
        public bool Status { get; set; }
        /// <summary>
        /// 状态（已分配,未分配）
        /// </summary>
        public string StatusStr { get; set; }
    }
}
