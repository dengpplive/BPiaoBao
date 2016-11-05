using BPiaoBao.SystemSetting.Domain.Models.Behavior;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices
{
  public  class UserBehaviorManage
    {
       /// <summary>
       /// 用户行为信息帮助
       /// </summary>
       public static ConcurrentBag<BehaviorStat> list = new ConcurrentBag<BehaviorStat>();
    }
}
