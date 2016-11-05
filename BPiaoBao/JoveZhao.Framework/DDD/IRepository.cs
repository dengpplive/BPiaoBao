using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.DDD
{
   public interface IRepository<T>:IReadOnlyRepository<T> where T:IAggregationRoot
    {
       void Create(T t);
       void Update(T t);
       void Delete(T t);

       
    }
  
}
