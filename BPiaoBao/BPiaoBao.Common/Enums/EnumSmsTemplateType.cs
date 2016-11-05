using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumSmsTemplateType
    {
        /// <summary>
        /// 出票
        /// </summary>
        [Description("出票")]
        IssueTicket = 0,
        /// <summary>
        /// 退票
        /// </summary>
        [Description("退票")]
        RefoundTicket = 1,
        /// <summary>
        /// 废票
        /// </summary>
        [Description("废票")]
        AnnulTicket = 2,
        /// <summary>
        /// 改签
        /// </summary>
        [Description("改签")]
        ChangeTicket = 3
    }
}
