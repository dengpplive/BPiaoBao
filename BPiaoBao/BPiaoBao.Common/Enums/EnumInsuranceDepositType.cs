using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    /// <summary>
    /// 仅用于InsuranceDeposit相关实体
    /// </summary>
    public enum EnumInsuranceDepositType : int
    {
        [Description("购买")]
        Normal = 0,
        [Description("赠送")]
        Offer = 1
    }
}
