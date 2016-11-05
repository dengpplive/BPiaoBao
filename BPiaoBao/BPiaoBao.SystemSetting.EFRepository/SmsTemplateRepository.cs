using BPiaoBao.SystemSetting.Domain.Models.SMS;
using JoveZhao.Framework.DDD;
using PiaoBao.BTicket.EFRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository
{
    public class SmsTemplateRepository : BaseRepository<SMSTemplate>, ISmsTemplateRepository
    {
        public SmsTemplateRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {

        }
    }
}
