using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 编码来源
    /// </summary>
    public enum EnumPnrSource
    {
        [Description("客户端生成")]
        CreatePnr = 0,
        [Description("客户端内容导入")]
        ImportPnrContent = 1
    }
}
