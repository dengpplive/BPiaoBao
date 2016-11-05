using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumRefuse
    {
        /// <summary>
        /// 修改订单申请理由
        /// </summary>
        [Description("修改订单申请理由")]
        ModifyOrder = 0,
        /// <summary>
        /// 退票申请理由(自愿)
        /// </summary>
        [Description("退票申请理由(自愿)")]
        RefundOrder_Voluntarily = 1,
        /// <summary>
        /// 退票申请理由(非自愿)
        /// </summary>
        [Description("退票申请理由(非自愿)")]
        RefuseOrder_UnVoluntarily = 2,
        /// <summary>
        /// 废票申请理由
        /// </summary>
        [Description("废票申请理由")]
        AnnulOrder = 3,
        /// <summary>
        /// 改签申请理由
        /// </summary>
        [Description("改签申请理由")]
        ChangeOrder = 4,
        /// <summary>
        /// 拒绝出票理由
        /// </summary>
        [Description("拒绝出票理由")]
        RefuseTicket = 5,
        /// <summary>
        /// 拒绝废票理由
        /// </summary>
        [Description("拒绝废票理由")]
        RefuseAnnulOrder = 6,
        /// <summary>
        /// 拒绝退票理由
        /// </summary>
        [Description("拒绝退票理由")]
        RefuseBounceOrder = 7,
        /// <summary>
        /// 拒绝改签理由
        /// </summary>
        [Description("拒绝改签理由")]
        RefuseChangeOrder =8,
        /// <summary>
        /// 拒绝修改订单理由
        /// </summary>
        [Description("拒绝修改订单理由")]
        RefuseModifyOrder =9
    }
}
