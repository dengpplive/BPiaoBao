using BPiaoBao.Common;
using BPiaoBao.DomesticTicket.Domain.Models.Refunds;
using JoveZhao.Framework.DDD;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DomesticTicket
{
    public partial class PlatformRefundOrderService
    {
        IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.DomesticTicket.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.DomesticTicket.ToString());
        IPlatformRefundOrderRepository platformRefundOrderRepository;
        public PlatformRefundOrderService(IPlatformRefundOrderRepository platformRefundOrderRepository)
        {
            this.platformRefundOrderRepository = platformRefundOrderRepository;
        }
    }
}
