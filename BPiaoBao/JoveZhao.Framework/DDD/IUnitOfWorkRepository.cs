using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
namespace JoveZhao.Framework.DDD
{
   public interface IUnitOfWorkRepository
    {
       void PersistCreationOf(IAggregationRoot entity);
       void PersistUpdateOf(IAggregationRoot entity);
       void PersistDeletionOf(IAggregationRoot entity);
       int ExecuteCommand(string sql,params object[] parames);
       DbRawSqlQuery SqlQuery(string sql, Type type, params object[] parames);
    }
}
