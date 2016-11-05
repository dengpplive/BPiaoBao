using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 扣点组类型
    /// </summary>
    public enum DeductionType : int
    {
        /// <summary>
        /// 本地
        /// </summary>
        [Description("本地")]
        Local = 0,
        /// <summary>
        /// 接口
        /// </summary>
        [Description("接口")]
        Interface = 1,
        /// <summary>
        /// 共享
        /// </summary>
        [Description("共享")]
        Share = 2
    }
}
