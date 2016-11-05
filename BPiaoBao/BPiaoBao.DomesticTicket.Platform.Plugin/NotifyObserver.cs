using StructureMap;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using JoveZhao.Framework.Expand;
using JoveZhao.Framework;

namespace BPiaoBao.DomesticTicket.Platform.Plugin
{

    public class NotifyObserver
    {
        public string Process(NameValueCollection nameValueCollection)
        {
            var r = string.Empty;
            bool tag = false;
            StringBuilder sbLog = new StringBuilder();
            sbLog.Append("Start=====================================================================\r\n\r\n");
            sbLog.Append("时间:" + System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff") + "\r\n");
            sbLog.Append("内容:\r\n");
            foreach (string key in nameValueCollection.Keys)
            {
                sbLog.Append(key + "=" + nameValueCollection[key] + "\r\n");
            }
            ObjectFactory.GetAllInstances<IPlatformNotify>()
               .ForEach(p =>
               {
                   p.OnIssue += p_OnIssue;
                   p.OnPaid += p_OnPaid;
                   p.OnCancelIssue += p_OnCancelIssue;
                   p.OnRefund += p_OnRefund;
                   p.OnRefundTicket += p_OnRefundTicket;
                   if (p.CanProcess(nameValueCollection))
                   {
                       tag = true;
                       sbLog.AppendFormat("已处理{0}\r\n", p.GetPlatformName());
                       try
                       {
                           //处理
                           r = p.Process(nameValueCollection);
                       }
                       catch (Exception e)
                       {
                           sbLog.Append("异常信息:" + e.Message + e.StackTrace + "\r\n");
                       }
                       finally
                       {
                           sbLog.Append("End=====================================================================\r\n\r\n");
                           Logger.WriteLog(LogType.INFO, sbLog.ToString() + "\r\n");
                       }
                   }
               });
            if (!tag)
            {
                sbLog.Append("未处理\r\n");
                sbLog.Append("End=====================================================================\r\n\r\n");
                Logger.WriteLog(LogType.INFO, sbLog.ToString());
            }
            return r;
        }

        void p_OnRefund(object sender, PlatformRefundEventArgs args)
        {
            Refund(args);
        }

        void p_OnCancelIssue(object sender, CancelIssueEventArgs args)
        {
            CancelIssue(args);
        }

        void p_OnPaid(object sender, PaidEventArgs args)
        {
            Paid(args);
        }

        void p_OnIssue(object sender, IssueEventArgs args)
        {
            Issue(args);
        }


        public event RefundTicketHandler OnRefundTicket;
        void p_OnRefundTicket(object sender, RefundTicketArgs args)
        {
            if (OnRefundTicket != null)
                OnRefundTicket(this, args);
        }


        public event PaidEventHandler OnPaid;
        void Paid(PaidEventArgs args)
        {
            if (OnPaid != null)
                OnPaid(this, args);
        }
        public event IssueEventHandler OnIssue;
        void Issue(IssueEventArgs args)
        {
            if (OnIssue != null)
                OnIssue(this, args);
        }

        public event CancelIssueHandler OnCancelIssue;
        void CancelIssue(CancelIssueEventArgs args)
        {
            if (OnCancelIssue != null)
                OnCancelIssue(this, args);
        }

        public event PlatformRefundHandler OnRefund;
        void Refund(PlatformRefundEventArgs args)
        {
            if (OnRefund != null)
                OnRefund(this, args);
        }
    }
}
