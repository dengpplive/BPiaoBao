using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 代付状态
    /// </summary>
    public enum EnumPaidStatus
    {
        /// <summary>
        /// 未代付
        /// </summary>
        [Description("未代付")]
        NoPaid = 0,
        /// <summary>
        /// 已代付
        /// </summary>
        [Description("已代付")]
        OK = 1
    }
}
