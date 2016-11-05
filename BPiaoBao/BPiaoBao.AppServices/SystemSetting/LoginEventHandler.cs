using BPiaoBao.Common;
using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework;
using JoveZhao.Framework.DDD;
using JoveZhao.Framework.DDD.Events;
using StructureMap;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.SystemSetting
{
    public class LoginEventHandler : IDomainEventHandler<UserLoginEvent>
    {
        private IUnitOfWork unitOfWork = ObjectFactory.GetNamedInstance<IUnitOfWork>(EnumModule.SystemSetting.ToString());
        IUnitOfWorkRepository unitOfWorkRepository = ObjectFactory.GetNamedInstance<IUnitOfWorkRepository>(EnumModule.SystemSetting.ToString());

        public void Handle(UserLoginEvent domainEvent)
        {
            if (domainEvent == null || domainEvent.LoginLog == null)
                throw new CustomException(500, "登录实例化空异常");
            unitOfWorkRepository.PersistCreationOf(domainEvent.LoginLog);
            unitOfWork.Commit();
        }
    }
}
