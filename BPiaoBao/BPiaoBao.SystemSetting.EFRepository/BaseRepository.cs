using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Linq;
using System.Text;
using JoveZhao.Framework.EFRepository.ContextStorage;



namespace PiaoBao.BTicket.EFRepository
{
    public abstract class BaseRepository<T>
        where T : EntityBase, IAggregationRoot
    {
        private IUnitOfWork _uow;
        private IUnitOfWorkRepository _uowr;
        public BaseRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
        {
            this._uow = uow;
            this._uowr = uowr;
        }
        public virtual IQueryable<T> FindAll(Expression<Func<T, bool>> exp)
        {
            return DbContextFactory<SystemDbContext>.GetDbContext().Set<T>().Where(exp);
        }
        /// <summary>
        /// 无状态查询
        /// </summary>
        /// <param name="exp">表达式</param>
        /// <returns></returns>
        public virtual IQueryable<T> FindAllNoTracking(Expression<Func<T, bool>> exp)
        {
            return DbContextFactory<SystemDbContext>.GetDbContext().Set<T>().AsNoTracking().Where(exp);
        }
        public virtual IQueryable<T> FindAll()
        {
            return DbContextFactory<SystemDbContext>.GetDbContext().Set<T>().AsQueryable();
        }
        /// <summary>
        /// 无状态查询
        /// </summary>
        /// <returns></returns>
        public virtual IQueryable<T> FindAllNoTracking()
        {
            return DbContextFactory<SystemDbContext>.GetDbContext().Set<T>().AsNoTracking().AsQueryable();
        }

        public virtual void Create(T t)
        {
            _uow.RegisterNew(t, _uowr);
        }
        public virtual void Update(T t)
        {
            _uow.RegisterAmended(t, _uowr);
        }
        public virtual void Delete(T t)
        {
            _uow.RegisterRemoved(t, _uowr);
        }
        public SystemDbContext DbContext
        {
            get
            {
                return DbContextFactory<SystemDbContext>.GetDbContext();
            }
        }
    }
}
