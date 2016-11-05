using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumPolicySourceType : int
    {
        /// <summary>
        /// 本地
        /// </summary>
        [Description("本地")]
        Local = 0,
        /// <summary>
        /// 接口
        /// </summary>
        [Description("接口")]
        Interface = 1,
        /// <summary>
        /// 共享
        /// </summary>
        [Description("共享")]
        Share = 2
    }
    public enum EnumPolicySpecialType : int
    { 

        Normal=0,
        /// <summary>
        /// 固定特价
        /// </summary>
        [Description("固定特价")]
        FixedSpecial=1,
        /// <summary>
        /// 动态特价
        /// </summary>
        [Description("动态特价")]
        DynamicSpecial=2,
        /// <summary>
        /// 直降
        /// </summary>
        [Description("直降")]
        DownSpecial=3,
        /// <summary>
        /// 折上折
        /// </summary>
        [Description("折上折")]
        DiscountOnDiscount=4
    }
}
