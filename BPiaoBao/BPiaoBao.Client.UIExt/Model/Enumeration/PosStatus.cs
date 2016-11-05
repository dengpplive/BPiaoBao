using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Client.UIExt.Model.Enumeration
{
    /// <summary>
    /// Pos机状态
    /// </summary>
    public enum PosStatus
    {
        [Description("所有")]
        All = -1,
        [Description("未分配")]
        Unassigned = 0,
        [Description("已分配")]
        Assigned = 1,
    }
}
