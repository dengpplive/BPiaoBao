using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts
{
    public class DataPack<T>
    {
        public List<T> List { get; set; }
        public int TotalCount { get; set; }
    }
}
