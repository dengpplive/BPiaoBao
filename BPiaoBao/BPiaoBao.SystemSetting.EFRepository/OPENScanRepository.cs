using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework.DDD;
using PiaoBao.BTicket.EFRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository
{
    public class OPENScanRepository : BaseRepository<OPENScan>,IOPENScanRepository
    {
        public OPENScanRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr)
            : base(uow, uowr)
        {

        }
    }
}
