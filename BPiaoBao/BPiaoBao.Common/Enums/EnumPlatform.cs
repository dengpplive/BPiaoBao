using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 接口平台枚举值
    /// </summary>
    public enum EnumPlatform
    {
        [Description("517")]
        _517 = 0,
        [Description("8000翼")]
        _8000YI = 1,
        [Description("票盟")]
        _PiaoMeng = 2,
        [Description("今日")]
        _Today = 3,
        [Description("百拓")]
        _BaiTuo = 4,
        [Description("易行")]
        _YeeXing = 5,
        [Description("51Book")]
        _51Book = 6
    }
}
