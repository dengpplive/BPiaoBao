using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace JoveZhao.Framework.Email
{
    public interface IEmailService
    {
        void SendMail(string from, string to, string subject, string body, Attachment[] attachments);
        void SendMail(string from, string to, string subject, string body);
    }
}
