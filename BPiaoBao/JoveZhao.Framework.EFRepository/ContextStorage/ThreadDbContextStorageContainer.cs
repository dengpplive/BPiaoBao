using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading;

namespace JoveZhao.Framework.EFRepository.ContextStorage
{
   public class ThreadDbContextStorageContainer<T>:IDbContextStorageContainer<T> where T:DbContext,new()
    {
        private string _key = typeof(T).Name + "ef_dbcontext";
        public T GetDbContext()
        {

            return CallContext.GetData(_key) as T;
        }

        public void Store(T dbContext)
        {
            CallContext.SetData(_key, dbContext);
        }



       //private static readonly Hashtable _dbContexts = new Hashtable();
       //public T GetDbContext()
       // {
       //     T dbContext = default(T);
       //     if (_dbContexts.Contains(getThreadName()))
       //         dbContext = _dbContexts[getThreadName()] as T;
       //     return dbContext;

       // }

       // private string getThreadName()
       // {
            
            
       //     if (string.IsNullOrEmpty(Thread.CurrentThread.Name))
       //         Thread.CurrentThread.Name = Thread.CurrentThread.GetHashCode().ToString();
       //     return Thread.CurrentThread.Name;
       // }

       // public void Store(T dbContext)
       // {
       //     if (_dbContexts.Contains(getThreadName()))
       //         _dbContexts[getThreadName()] = dbContext;
       //     else
       //         _dbContexts.Add(getThreadName(), dbContext);
       // }


        public void Clear()
        {
            CallContext.FreeNamedDataSlot(_key);            
        }
    }
}
