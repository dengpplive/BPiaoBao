using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.StationContracts.StationMap
{
    public class PagedList<T>
    {
        public int Total { get; set; }
        public List<T> Rows { get; set; }
    }

}
