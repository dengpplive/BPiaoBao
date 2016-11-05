using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.TPos
{
    public class AccountStatDataObject
    {
        /// <summary>
        /// Pos总数
        /// </summary>
        public int PosCount { get; set; }
        /// <summary>
        /// 已分配Pos总数
        /// </summary>
        public int AssignPosCount { get; set; }
        /// <summary>
        /// 未分配Pos总数
        /// </summary>
        public int UnAssignPosCount { get; set; }
        /// <summary>
        /// 商户总数
        /// </summary>
        public int BusinessmanCount { get; set; }
        /// <summary>
        /// 历史收益
        /// </summary>
        public decimal HistoryGain { get; set; }
        /// <summary>
        /// 昨日收益
        /// </summary>
        public decimal YesterdayGain { get; set; }
    }
}
