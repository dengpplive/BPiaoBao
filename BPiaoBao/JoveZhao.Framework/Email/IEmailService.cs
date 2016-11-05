using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;

namespace JoveZhao.Framework.SMS
{
    public interface ISMSService
    {
        Tuple<int,bool> SMS(string to, string message);
    }

}
