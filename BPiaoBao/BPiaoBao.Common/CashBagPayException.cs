using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
namespace BPiaoBao.Common
{
    public class CashBagPayException : CustomException
    {
        public CashBagPayException(string message)
            : base(541, message)
        {

        }
    }
}
