using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 订单生成来源
    /// </summary>
    public enum EnumOrderSource
    {
        /// <summary>
        /// 白屏预定
        /// </summary>
        [Description("白屏预定")]
        WhiteScreenDestine = 0,
        /// <summary>
        /// 编码导入
        /// </summary>
        [Description("编码导入")]
        PnrImport = 2,
        /// <summary>
        /// 儿童编码导入
        /// </summary>
        [Description("儿童编码导入")]
        ChdPnrImport = 3,
        /// <summary>
        /// 升舱换开,编码导入
        /// </summary>
        [Description("升舱换开,编码导入")]
        UpSeatChangePnrImport = 4,
        /// <summary>
        /// 线下订单
        /// </summary>
        [Description("线下订单")]
        LineOrder = 5,
        /// <summary>
        /// 手机预定
        /// </summary>
        [Description("手机预定")]
        MobileDestine = 6,
        /// <summary>
        /// PNR内容导入
        /// </summary>
        [Description("PNR内容导入")]
        PnrContentImport = 7
    }
}
