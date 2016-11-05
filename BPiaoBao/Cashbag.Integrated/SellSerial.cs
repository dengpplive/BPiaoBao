using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cashbag.Integrated
{
   public class SellSerial
    {
       public string OrderId { get; set; }
       public DateTime SellTime { get; set; }
       public decimal Money { get; set; }
       public string Remark { get; set; }
    }
}
