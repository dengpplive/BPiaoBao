using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.ConsoContracts.SystemSetting.DataObjects
{
    public class PagedList<T>
    {
        public PagedList()
        {
            this.Total = 0;
            this.Rows = new List<T>();
        }
        public int Total { get; set; }
        public List<T> Rows { get; set; }
    }
}
