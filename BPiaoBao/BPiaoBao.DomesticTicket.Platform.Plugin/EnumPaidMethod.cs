using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.DomesticTicket.Platform.Plugin
{
    /// <summary>
    /// 接口平台绑定的代付方式
    /// </summary>
    public enum EnumPaidMethod
    {
        未知 = -1,
        支付宝 = 0,
        快钱 = 1,
        财付通 = 2,
        汇付 = 3,
        预存款 = 4
    }
}
