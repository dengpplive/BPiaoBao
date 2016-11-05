using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using JoveZhao.Framework;
namespace BPiaoBao.Cashbag.ClientProxy
{
    public class CashBagException:CustomException
    {
        public CashBagException(string message):base(540,message)
        {

        }
    }
}
