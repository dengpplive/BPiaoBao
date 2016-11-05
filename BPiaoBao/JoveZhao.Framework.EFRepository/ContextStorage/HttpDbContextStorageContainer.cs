using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;

namespace JoveZhao.Framework.EFRepository.ContextStorage
{
    public class HttpDbContextStorageContainer<T> : IDbContextStorageContainer<T> where T : DbContext, new()
    {


        private string _key = typeof(T).Name + "ef_dbcontext";
       public T GetDbContext()
        {


            T dbContext = default(T);
            if (HttpContext.Current.Items.Contains(_key))
                dbContext = HttpContext.Current.Items[_key] as T;
            return dbContext;
        }

       public void Store(T dbContext)
        {
            if (HttpContext.Current.Items.Contains(_key))
                HttpContext.Current.Items[_key] = dbContext;
            else
                HttpContext.Current.Items.Add(_key, dbContext);
        }


       public void Clear()
       {
           if (HttpContext.Current.Items.Contains(_key))
               HttpContext.Current.Items.Remove(_key);
            
       }
    }
}
