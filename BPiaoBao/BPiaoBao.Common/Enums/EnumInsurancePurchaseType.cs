using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 仅用于InsurancePurchase相关实体
    /// </summary>
    public enum EnumInsurancePurchaseType : int
    {
        [Description("购买")]
        Normal = 0,
        [Description("赠送")]
        Offer = 1
    }
}
