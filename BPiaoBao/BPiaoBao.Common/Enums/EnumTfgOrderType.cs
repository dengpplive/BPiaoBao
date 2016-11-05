using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{

    /// <summary>
    /// 退废改类型
    /// </summary>
    public enum EnumTfgProcessStatus
    {
        /// <summary>
        /// 未处理
        /// </summary>
        [Description("未处理")]
        UnProcess = 0,
        /// <summary>
        /// 处理中
        /// </summary>
        [Description("处理中")]
        Processing = 1,
        /// <summary>
        /// 已处理,等待退款
        /// </summary>
        [Description("已处理,等待退款")]
        ProcessingWaitRefund = 2,
        /// <summary>
        /// 已处理,等待支付
        /// </summary>
        [Description("已处理,等待支付")]
        ProcessingWaitPay = 3,
        /// <summary>
        /// 退款中
        /// </summary>
        [Description("退款中")]
        Refunding = 4,
        /// <summary>
        /// 已支付等待出票
        /// </summary>
        [Description("已支付,等待出票")]
        WaitIssue = 5,
        /// <summary>
        /// 处理完成
        /// </summary>
        [Description("处理完成")]
        Processed = 6,
        /// <summary>
        /// 拒绝处理
        /// </summary>
        [Description("拒绝处理")]
        RepelProcess = 7

    }
    /// <summary>
    /// 售后订单【退改签】乘机人状态
    /// </summary>
    public enum EnumTfgPassengerStatus:int
    { 
        /// <summary>
        /// 申请中
        /// </summary>
        [Description("申请中")]
        Apply=0,
        /// <summary>
        /// 处理中
        /// </summary>
        [Description("处理中")]
        Processing=1,
        /// <summary>
        /// 已退款
        /// </summary>
        [Description("已退票")]
        Refunded=2,
        /// <summary>
        /// 已废票
        /// </summary>
        [Description("已废票")]
        AnnulTicketed=3,
        /// <summary>
        /// 改签完成
        /// </summary>
        [Description("改签完成")]
        ChangeTicketed=4,
        /// <summary>
        /// 修改完成
        /// </summary>
        [Description("修改完成")]
        Modified=5,
        /// <summary>
        /// 拒绝处理
        /// </summary>
        [Description("拒绝处理")]
        RepelProcess=6
    }
}
