using JoveZhao.Framework.DDD;
using JoveZhao.Framework.EFRepository.ContextStorage;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JoveZhao.Framework.EFRepository.UnitOfWork
{
    public class EFUnitOfWork<T> : IUnitOfWork where T : DbContext, new()
    {

        public void RegisterAmended(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistUpdateOf(entity);
        }

        public void RegisterNew(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistCreationOf(entity);
        }

        public void RegisterRemoved(IAggregationRoot entity, IUnitOfWorkRepository unitOfWorkRepository)
        {
            unitOfWorkRepository.PersistDeletionOf(entity);
        }

        public void Commit()
        {

            try
            {
                var result = DbContextFactory<T>.GetDbContext().GetValidationErrors();
                foreach (var item in result)
                {
                    foreach (var itemA in item.ValidationErrors)
                    {
                        Logger.WriteLog(LogType.ERROR, string.Format("{0}:{1}", itemA.PropertyName, itemA.ErrorMessage));
                    }
                }
                DbContextFactory<T>.GetDbContext().SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                throw e;
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public void Dispose()
        {

            DbContextFactory<T>.GetDbContext().Dispose();
            DbContextFactory<T>.ClearDbContext();
        }
    }
}
