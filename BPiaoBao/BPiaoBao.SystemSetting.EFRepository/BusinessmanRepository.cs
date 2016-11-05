using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework.DDD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity.Infrastructure;
namespace PiaoBao.BTicket.EFRepository
{
    public class BusinessmanRepository : BaseRepository<Businessman>, IBusinessmanRepository
    {

        public BusinessmanRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr) { }

        public DbRawSqlQuery SqlQuery(string sql, Type type)
        {
            return this.DbContext.Database.SqlQuery(type, sql);
        }
    }
}
