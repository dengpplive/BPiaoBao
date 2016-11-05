using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 调整类型
    /// </summary>
    public enum AdjustType : int
    {
        /// <summary>
        /// 扣点
        /// </summary>
        [Description("扣点")]
        Lrish = 0,
        /// <summary>
        /// 留点
        /// </summary>
        [Description("留点")]
        Leave = 1,
        /// <summary>
        /// 补点
        /// </summary>
        [Description("补点")]
        Compensation = 2
    }
}
