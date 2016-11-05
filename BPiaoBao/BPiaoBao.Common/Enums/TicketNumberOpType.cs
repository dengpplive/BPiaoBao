using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum TicketNumberOpType : int
    {
        /// <summary>
        /// 票号挂起锁定操作
        /// </summary>
        [Description("解挂")]
        Resume = 0,
        /// <summary>
        /// 票号解挂恢复操作
        /// </summary>
        [Description("挂起")]
        Suppend = 1
    }
}
