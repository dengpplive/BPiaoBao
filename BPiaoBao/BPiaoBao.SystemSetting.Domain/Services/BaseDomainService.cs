using BPiaoBao.Common;
using JoveZhao.Framework.DDD;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Services
{
    public class BaseDomainService
    {
        protected IUnitOfWork _unitOfWork;
        protected IUnitOfWorkRepository _unitOfWorkRepository;

        protected BaseDomainService()
        {
            _unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
            _unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());
        }
    }
}
