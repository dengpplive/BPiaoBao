using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.IO;

namespace JoveZhao.Framework.Email
{
   
    public class SMTPService : IEmailService
    {
        AccountConfigurationElement accounConfig;
        public SMTPService()
        {
            this.accounConfig = JZFSection.GetInstances().Email.Account;
        }
        public void SendMail(string from, string to, string subject, string body, Attachment[] attachments)
        {
            MailMessage message = new MailMessage();
            message.From = new MailAddress(from);
            message.To.Add(new MailAddress(to));
            message.Subject = subject;
            message.Body = body;
            if (attachments != null && attachments.Length > 0)
            {
                foreach (var att in attachments)
                {
                    message.Attachments.Add(att);
                }
            }
            SmtpClient smtp = new SmtpClient(accounConfig.Host, accounConfig.Port);
            smtp.Credentials = new NetworkCredential(accounConfig.Account, accounConfig.Password);

            smtp.Send(message);
        }

        public void SendMail(string from, string to, string subject, string body)
        {
            SendMail(from, to, subject, body, null);
        }
    }
}
