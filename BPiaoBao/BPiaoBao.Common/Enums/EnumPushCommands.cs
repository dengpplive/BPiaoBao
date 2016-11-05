using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 
    /// </summary>
    public enum EnumPushCommands
    {
        /// <summary>
        /// 提醒还款
        /// </summary>
        RemindRepayment,
        /// <summary>
        /// 系统设置
        /// </summary>
        SystemSetting,
        /// <summary>
        /// 切换到现金账户
        /// </summary>
        Cash,
        /// <summary>
        /// 切换到信用账户
        /// </summary>
        Credit,
        /// <summary>
        /// 切换到理财账户
        /// </summary>
        Finance,
        /// <summary>
        /// 切换到积分账户
        /// </summary>
        Points,
        /// <summary>
        /// 重复登陆
        /// </summary>
        RepeatLogin,
        /// <summary>
        /// 公告页面
        /// </summary>
        NoticePage,
        /// <summary>
        /// 登录时打开公告窗口
        /// </summary>
        LoginPopNoticeWindow,
        /// <summary>
        /// 强制弹出公告窗口
        /// </summary>
        EnforcePopNoticeWindow,
        /// <summary>
        /// 温馨提示
        /// </summary>
        [Description("温馨提示")]
        NormalMsg,
        /// <summary>
        /// 我的消息提示
        /// </summary>
        [Description("消息提示")]
        MyMessageTip


    }
}
