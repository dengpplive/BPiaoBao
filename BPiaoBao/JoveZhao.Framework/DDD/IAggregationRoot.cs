using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.DDD
{
    public interface IAggregationRoot<T> : IAggregationRoot where T : struct
    {
         T Id { get; set; }
    }
    public interface IAggregationRoot { }
}
