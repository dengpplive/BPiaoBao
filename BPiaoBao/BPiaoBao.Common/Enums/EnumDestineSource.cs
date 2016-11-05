using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumDestineSource
    {
        /// <summary>
        /// 白屏预定
        /// </summary>
        [Description("白屏预定")]
        WhiteScreenDestine = 0,
        /// <summary>
        /// 手机预定
        /// </summary>
        [Description("手机预定")]
        MobileDestine = 1,
    }
}
