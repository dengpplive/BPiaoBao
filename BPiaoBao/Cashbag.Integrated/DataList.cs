using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Cashbag.Integrated
{
    public class DataList<T>
    {
        public List<T> List { get; set; }
        public int TotalCount { get; set; }
    }
}
