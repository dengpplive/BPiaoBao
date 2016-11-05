using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 订单状态
    /// </summary>
    public enum EnumOrderStatus
    {
        /// <summary>
        /// 新订单，等待选择政策(前台:[取消，选择政策])
        /// </summary>
        [Description("新订单,等待选择政策")]
        WaitChoosePolicy = 0,
        /// <summary>
        /// 生成平台订单失败
        /// </summary>
        [Description("生成平台订单失败")]
        CreatePlatformFail = 1,
        /// <summary>
        /// 新订单，等待支付(前台:[取消，支付]，后台[查询支付状态])
        /// </summary>
        [Description("新订单,等待支付")]
        NewOrder = 2,
        /// <summary>
        /// 已支付，等待代付(后:[重新代付，拒绝出票，重选政策，查询代付状态)
        /// </summary>
        [Description("已支付,等待代付")]
        WaitAndPaid = 3,
        /// <summary>
        /// 已支付，等待出票(后：[查询票号])
        /// </summary>
        [Description("已支付,等待出票")]
        WaitIssue = 4,
        /// <summary>
        /// 订单已取消
        /// </summary>
        [Description("订单已取消")]
        OrderCanceled = 5,
        /// <summary>
        /// 已经出票，订单完成(前:[售后申请])
        /// </summary>
        [Description("已经出票,订单完成")]
        IssueAndCompleted = 6,
        /// <summary>
        /// 拒绝出票，等待退款(后:[退款,重选政策])
        /// </summary>
        [Description("暂停出票")]
        WaitReimburseWithRepelIssue = 7,
        /// <summary>
        /// 拒绝出票，等待平台退款(后:[确认平台已退款])
        /// </summary>
        [Description("拒绝出票,等待平台退款")]
        WaitReimburseWithPlatformRepelIssue = 8,
        /// <summary>
        /// 拒绝出票，订单完成
        /// </summary>
        [Description("拒绝出票,订单完成")]
        RepelIssueAndCompleted = 9,
        /// <summary>
        /// 订单已失效
        /// </summary>
        [Description("订单已失效")]
        Invalid = 10,
        /// <summary>
        /// 自动复合票号失败(后:[手工复合票号])
        /// </summary>
        [Description("自动复合票号失败")]
        AutoIssueFail = 11,
        /// <summary>
        /// 拒绝出票,退款中
        /// </summary>
        [Description("拒绝出票,退款中")]
        RepelIssueRefunding = 12,
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
        [Description("已支付,等待生成订单")]
        PayWaitCreatePlatformOrder = 15,
        /// <summary>
        /// 新订单，支付中
        /// </summary>
        [Description("新订单，支付中")]
        PaymentInWaiting = 16
    }
}
