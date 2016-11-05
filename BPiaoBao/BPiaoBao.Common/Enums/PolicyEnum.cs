using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 政策发布类型
    /// </summary>
    public enum EnumReleaseType : int
    {
        /// <summary>
        /// 出港
        /// </summary>
        [Description("出港")]
        LeavePort = 0,
        /// <summary>
        /// 入港
        /// </summary>
        [Description("入港")]
        Port = 1,
        /// <summary>
        /// 全国
        /// </summary>
        [Description("全国")]
        WholeCountry = 2
    }
    public enum EnumTravelType : int
    {
        /// <summary>
        /// 全部
        /// </summary>
        [Description("全部")]
        All = 0,
        /// <summary>
        /// 单程
        /// </summary>
        [Description("单程")]
        OneWay = 1,
        /// <summary>
        /// 往返
        /// </summary>
        [Description("往返")]
        Return = 2,
        /// <summary>
        /// 单程/往返
        /// </summary>
        [Description("单程/往返")]
        OneWayReturn = 3,
        /// <summary>
        /// 联程
        /// </summary>
        [Description("联程")]
        Connecting = 4
    }
    public enum EnumIssueTicketWay : int
    {
        /// <summary>
        /// 手动
        /// </summary>
        [Description("手动")]
        Manual = 0,
        /// <summary>
        /// 自动
        /// </summary>
        [Description("自动")]
        Automatic = 1
    }
    public enum EnumApply : int
    {
        /// <summary>
        /// 适用所有航班
        /// </summary>
        [Description("适用所有航班")]
        All = 0,
        /// <summary>
        /// 仅适用以下航班
        /// </summary>
        [Description("仅适用以下航班")]
        Apply = 1,
        /// <summary>
        /// 不适用以下航班
        /// </summary>
        [Description("不适用以下航班")]
        NotApply = 2
    }

    /// <summary>
    /// 固定特价类型
    /// </summary>
    public enum FixedOnSaleType : int
    {
        /// <summary>
        /// 固定价格
        /// </summary>
        [Description("固定价格")]
        Fixed = 0,
        /// <summary>
        /// 直降
        /// </summary>
        [Description("直降")]
        Plummeted  = 1,
        /// <summary>
        /// 折上折
        /// </summary>
        [Description("折上折")]
        Credit =2
    }

    public enum SpeciaType : int
    {
        /// <summary>
        /// 动态特价
        /// </summary>
        [Description("动态特价")]
        Dynamic = 0,
        /// <summary>
        /// 固定特价
        /// </summary>
        [Description("固定特价")]
        Fixed = 1
    }
}
