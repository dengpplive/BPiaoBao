using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 行程单状态
    /// </summary>
    public enum EnumTripStatus
    {
        [Description("已分配,未使用")]
        NoUsed = 0,
        [Description("已创建,已使用")]
        HasCreatedUsed = 1,
        [Description("已作废,未回收")]
        HasObsoleteUsed = 2,
        [Description("空白回收,未分配")]
        BlankRecoveryNoUsed = 3,
        [Description("空白回收,已分配")]
        BlankRecoveryUsed = 4,
        [Description("已作废,已回收")]
        VoidRecoveryUsed = 5
    }

    /// <summary>
    /// 乘客对应的行程单状态
    /// </summary>
    public enum EnumPassengerTripStatus
    {
        [Description("未创建")]
        NoCreate = 0,
        [Description("已创建")]
        HasCreate = 1,
        [Description("已作废")]
        HasVoid = 2
    }
}
