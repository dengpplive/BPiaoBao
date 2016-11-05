using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumPnrImportType
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
        GenericPnrImport = 1,
        /// <summary>
        /// 儿童编码导入
        /// </summary>
        [Description("儿童编码导入")]
        CHDPnrImport = 2,
        /// <summary>
        /// 升舱换开
        /// </summary>
        [Description("升舱换开")]
        UpSeatChangePnrImport = 3,
        /// <summary>
        /// 手机预定
        /// </summary>
        [Description("手机预定")]
        MobileDestine = 4,
        /// <summary>
        /// PNR内容导入
        /// </summary>
        [Description(" PNR内容导入")]
        PnrContentImport = 5
    }
}
