using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.Cache
{
    public interface ICacheStrategy
    {
        void Set(string key, object value);
        void Set(string key, object value, DateTime expiryTime);
        object Get(string key);
        object[] FindAll();
        void Remove(string key);
        void RemoveAll( );
    }
}
