using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumClientOrderStatus
    {
        /// <summary>
        /// 生成订单等待选择政策
        /// </summary>
        [Description("生成订单等待选择政策")]
        WaitChoosePolicy = 1,
        /// <summary>
        /// 新订单，等待支付
        /// </summary>
        [Description("新订单，等待支付")]
        NewOrder = 2,
        /// <summary>
        /// 支付成功 等待出票
        /// </summary>
        [Description("支付成功 等待出票")]
        WaitIssue = 3,
        /// <summary>
        /// 订单取消
        /// </summary>
        [Description("订单取消")]
        OrderCanceled = 4,
        /// <summary>
        /// 已经出票，订单完成
        /// </summary>
        [Description("已经出票，订单完成")]
        IssueAndCompleted = 5,
        /// <summary>
        /// 暂停出票
        /// </summary>
        [Description("暂停出票")]
        WaitReimburseWithRepelIssue = 6,
        /// <summary>
        /// 拒绝出票，订单完成
        /// </summary>
        [Description("拒绝出票，订单完成")]
        RepelIssueAndCompleted = 7,
        /// <summary>
        /// 订单失效
        /// </summary>
        [Description("订单失效")]
        Invalid = 8,
        /// <summary>
        /// 拒绝出票，退款中
        /// </summary>
        [Description("拒绝出票，退款中")]
        RepelIssueRefunding = 9,
        /// <summary>
        /// 线下婴儿申请,等待审核
        /// </summary>
        [Description("线下婴儿申请,等待审核")]
        ApplyBabyFail = 13,
        /// <summary>
        /// 拒绝审核,订单完成
        /// </summary>
        [Description("拒绝审核,订单完成")]
        RepelApplyBaby = 14,
        /// <summary>
        /// 已支付,等待生成订单
        /// </summary>
        [Description("已支付,等待出票")]
        PayWaitCreatePlatformOrder = 15,
        /// <summary>
        /// 新订单，支付中
        /// </summary>
        [Description("新订单，支付中")]
        PaymentInWaiting = 16
    }


}
