using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.DDD
{
    public interface IReadOnlyRepository<T> : IBaseRepository<T> where T : IAggregationRoot
    {
        IQueryable<T> FindAll(Expression<Func<T, bool>> exp);
        /// <summary>
        /// 无状态查询
        /// </summary>
        /// <param name="exp"></param>
        /// <returns></returns>
        IQueryable<T> FindAllNoTracking(Expression<Func<T, bool>> exp);
        IQueryable<T> FindAll();
        /// <summary>
        /// 无状态查询
        /// </summary>
        /// <returns></returns>
        IQueryable<T> FindAllNoTracking();
        
    }
    public interface IBaseRepository<T> where T : IAggregationRoot
    {
    }
}
