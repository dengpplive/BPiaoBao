using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework.DDD;
using PiaoBao.BTicket.EFRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository
{
    public class LoginLogRepository : BaseRepository<LoginLog>, ILoginLogRepository
    {
        public LoginLogRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr) { }
    }
}
