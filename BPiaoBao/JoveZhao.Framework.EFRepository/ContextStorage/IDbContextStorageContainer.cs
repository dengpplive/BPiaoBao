using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;

namespace JoveZhao.Framework.EFRepository.ContextStorage
{
   public interface IDbContextStorageContainer<T> where T:DbContext,new()
    {
       T GetDbContext();
       void Store(T dbContext);
       void Clear();
    }
}
