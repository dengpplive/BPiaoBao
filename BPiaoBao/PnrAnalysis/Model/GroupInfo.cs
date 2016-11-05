using System;
using System.Collections.Generic;
using System.Text;

namespace PnrAnalysis.Model
{
    /// <summary>
    /// 团编码信息
    /// </summary>
    /// 
    [Serializable]
    public class GroupInfo
    {
        /// <summary>
        /// 是否为团编码
        /// </summary>
        public bool IsTuan = false;
        /// <summary>
        /// 团人数
        /// </summary>
        public int TuanCount = 0;
        /// <summary>
        /// 团名称
        /// </summary>
        public string TuanName = string.Empty;

    }
}
