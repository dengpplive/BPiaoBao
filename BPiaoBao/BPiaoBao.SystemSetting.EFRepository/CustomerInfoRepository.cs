using BPiaoBao.DomesticTicket.Domain.Models.CustomerInfo;
using JoveZhao.Framework.DDD;
using PiaoBao.BTicket.EFRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.EFRepository
{
    public class CustomerInfoRepository :BaseRepository<CustomerInfo> ,ICustomerInfoRepository
    {
        public CustomerInfoRepository(IUnitOfWork uow, IUnitOfWorkRepository uowr) : base(uow, uowr)
        {
            
        }
    }
}
