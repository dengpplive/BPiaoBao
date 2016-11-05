using JoveZhao.Framework.HttpServers;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    /// <summary>
    /// 平台通知
    /// </summary>
    public interface IPlatformNotify
    {

        bool CanProcess(NameValueCollection nameValueCollection);
        /// <summary>
        /// 处理通知信息
        /// </summary>
        string Process(NameValueCollection nameValueCollection);


        string GetPlatformName();
        string Code { get; }


        event PaidEventHandler OnPaid;
        event IssueEventHandler OnIssue;
        event CancelIssueHandler OnCancelIssue;
        event PlatformRefundHandler OnRefund;
        event RefundTicketHandler OnRefundTicket;

    }
}
