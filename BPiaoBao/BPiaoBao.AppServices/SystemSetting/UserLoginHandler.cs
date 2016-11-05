using BPiaoBao.SystemSetting.Domain.Services.Auth;
using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PiaoBao.BTicket.AppServices.SystemSetting
{
    public class UserLoginHandler : IDomainEventHandler<UserLoginEvent>
    {
        public void Handle(UserLoginEvent domainEvent)
        {
            //用户登录成功后的事件……
        }
    }
}
