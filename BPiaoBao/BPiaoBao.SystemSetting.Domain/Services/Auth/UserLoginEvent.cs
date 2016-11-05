using BPiaoBao.SystemSetting.Domain.Models.Businessmen;
using JoveZhao.Framework.DDD.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.SystemSetting.Domain.Services.Auth
{
    public class UserLoginEvent : IDomainEvent
    {
        public CurrentUserInfo User { get; set; }
        public LoginLog LoginLog { get; set; }
    }
}
