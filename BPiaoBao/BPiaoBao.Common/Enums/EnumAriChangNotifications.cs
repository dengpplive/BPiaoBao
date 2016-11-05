using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumAriChangNotifications
    {
        /// <summary>
        /// 无
        /// </summary>
        [Description("无")]
        Null =0,
        /// <summary>
        /// 电话通知航变
        /// </summary>
        [Description("电话通知航变")]
        CallNotify= 4,
        /// <summary>
        /// 短信通知航变
        /// </summary>
        [Description("短信通知航变")]
        MessageNotify =1,
        /// <summary>
        /// 自动弹出提醒
        /// </summary>
        [Description("自动弹出提醒")]
        AutoPopMessage =2,
        /// <summary>
        /// 其他
        /// </summary>
        [Description("其他")]
        other = 3

    }
}
