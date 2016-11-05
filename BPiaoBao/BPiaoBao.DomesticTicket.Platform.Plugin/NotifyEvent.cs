using BPiaoBao.DomesticTicket.Platform.Plugin.MessageMap;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    public delegate void PaidEventHandler(object sender, PaidEventArgs args);
    public delegate void IssueEventHandler(object sender, IssueEventArgs args);
    public delegate void PlatformRefundHandler(object sender, PlatformRefundEventArgs args);
    public delegate void CancelIssueHandler(object sender, CancelIssueEventArgs args);
    public delegate void RefundTicketHandler(object sender, RefundTicketArgs args);

    public  class RefundTicketArgs : EventArgs
    {
        /// <summary>
        /// 1废票，2退票
        /// </summary>
        public int NotifyType { get; set; }
        /// <summary>
        /// 售后接口订单号
        /// </summary>
        public string OutOrderId { get; set; }
        /// <summary>
        /// 处理结果
        /// </summary>
        public bool ProcessStatus { get; set; }
        /// <summary>
        /// 乘机人
        /// </summary>
        public List<NotifyRefundPassenger> Passengers { get; set; }
        /// <summary>
        /// 退款总金额
        /// </summary>
        public decimal ReturnTotalMoney { get; set; }
        /// <summary>
        /// 备注信息
        /// </summary>
        public string Remark { get; set; }
    }
    public class NotifyRefundPassenger
    {
        /// <summary>
        /// 票号
        /// </summary>
        public string TicketNo { get; set; }
        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal ReturnMoney { get; set; }
    }
    /// <summary>
    /// 接口平台支付信息
    /// </summary>
    public class PaidEventArgs : EventArgs
    {
        /// <summary>
        /// 代付平台
        /// </summary>
        public string PlatformCode { get; set; }
        /// <summary>
        /// 外部订单号
        /// </summary>
        public string OutOrderId { get; set; }
        /// <summary>
        /// 交易流水号
        /// </summary>
        public string SerialNumber { get; set; }
        /// <summary>
        /// 代付金额 票盟没有此值
        /// </summary>
        public decimal PaidMeony { get; set; }
        /// <summary>
        /// 其他信息
        /// </summary>
        public string OtherInfo { get; set; }
    }
    /// <summary>
    /// 接口平台出票信息
    /// </summary>
    public class IssueEventArgs : EventArgs
    {
        /// <summary>
        /// 出票平台
        /// </summary>
        public string PlatformCode { get; set; }
        /// <summary>
        /// 外部订单号
        /// </summary>
        public string OutOrderId { get; set; }

        /// <summary>
        /// 出票备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 机票信息[name:乘机人;value:票号]
        /// </summary>
        public NameValueCollection TicketInfo { get; set; }

    }
    /// <summary>
    /// 接口平台取消出票信息
    /// </summary>
    public class CancelIssueEventArgs : EventArgs
    {
        /// <summary>
        /// 取消出票平台
        /// </summary>
        public string PlatformCode { get; set; }
        /// <summary>
        /// 外部订单号
        /// </summary>
        public string OutOrderId { get; set; }
        /// <summary>
        /// 取消理由
        /// </summary>
        public string Ramark { get; set; }
        /// <summary>
        /// 通知集合信息
        /// </summary>
        public NameValueCollection NotifyCollection { get; set; }
    }
    /// <summary>
    /// 接口平台退款信息
    /// </summary>
    public class PlatformRefundEventArgs : EventArgs
    {
        /// <summary>
        /// 退款平台
        /// </summary>
        public string PlatformCode { get; set; }

        /// <summary>
        /// 退款类型 如退票,废票,取消出票等
        /// </summary>
        public string RefundType { get; set; }
        /// <summary>
        /// 退款备注
        /// </summary>
        public string RefundRemark { get; set; }
        /// <summary>
        /// 退款订单号
        /// </summary>
        public string OutOrderId { get; set; }

        /// <summary>
        /// 退款交易流水号
        /// </summary>
        public string RefundSerialNumber { get; set; }

        /// <summary>
        /// 退款金额
        /// </summary>
        public decimal RefundMoney { get; set; }

        /// <summary>
        /// 退款手续费总和
        /// </summary>
        public decimal TotalRefundPoundage { get; set; }

        /// <summary>
        /// 通知集合信息
        /// </summary>
        public NameValueCollection NotifyCollection { get; set; }

    }

}
