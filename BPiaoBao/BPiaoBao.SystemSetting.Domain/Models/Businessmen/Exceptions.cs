using JoveZhao.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Models.Businessmen
{
    public class ChangePasswordException : CustomException
    {
        public ChangePasswordException(string message) : base(610, message) { }
    }
    public class AcccountAuthException : CustomException
    {
        public AcccountAuthException(string message) : base(611, message) { }
    }
}
