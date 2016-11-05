using JoveZhao.Framework.DDD;
using JoveZhao.Framework.EFRepository.ContextStorage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;

namespace JoveZhao.Framework.EFRepository.UnitOfWork
{
    public class EFUnitOfWorkRepositoryAdapter<T> : IUnitOfWorkRepository where T : DbContext, new()
    {

        public void PersistCreationOf(IAggregationRoot entity)
        {
            DbContextFactory<T>.GetDbContext().Entry(entity).State = System.Data.Entity.EntityState.Added;
        }

        public void PersistUpdateOf(IAggregationRoot entity)
        {
            DbContextFactory<T>.GetDbContext().Entry(entity).State = System.Data.Entity.EntityState.Modified;
        }

        public void PersistDeletionOf(IAggregationRoot entity)
        {
            DbContextFactory<T>.GetDbContext().Entry(entity).State = System.Data.Entity.EntityState.Deleted;
        }


        public int ExecuteCommand(string sql, params object[] parames)
        {
            return DbContextFactory<T>.GetDbContext().Database.ExecuteSqlCommand(sql, parames);

        }
        public DbRawSqlQuery SqlQuery(string sql, Type type, params object[] parames)
        {
            return DbContextFactory<T>.GetDbContext().Database.SqlQuery(type, sql, parames);           
        }
    }
}
