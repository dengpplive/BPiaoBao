using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumNoticeType:int
    {
        /// <summary>
        /// 滚动
        /// </summary>
        [Description("滚动")]
        Roll = 0,
        /// <summary>
        /// 强制弹出（一次弹出）
        /// </summary>
        [Description("强制弹出")]
        Eject = 1,
        /// <summary>
        /// 登录弹出（登录仅显示最近一条）
        /// </summary>
        [Description("登录弹出")]
        LoginEject = 2

    }
    public enum EnumNoticeShowType : int
    {
        /// <summary>
        /// 买票宝
        /// </summary>
        [Description("买票宝")]
        Mpb = 0,
        /// <summary>
        /// 运营商
        /// </summary>
        [Description("运营商")]
        Carrier = 1,
        /// <summary>
        /// 代理人
        /// </summary>
        [Description("代理人")]
        agent = 2
    }
}
