using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace BPiaoBao.Common.Enums
{
    public enum EnumPayMethod
    {
        /// <summary>
        /// 现金账户
        /// </summary>
        [Description("现金账户")]
        Account = 0,
        /// <summary>
        /// 信用账户
        /// </summary>
        [Description("信用账户")]
        Credit = 1,
        /// <summary>
        /// 银行卡
        /// </summary>
        [Description("银行卡")]
        Bank = 2,
        /// <summary>
        /// 支付平台
        /// </summary>
        [Description("支付平台")]
        Platform = 3,
        /// <summary>
        /// 财付通
        /// </summary>
        [Description("财付通")]
        TenPay = 4,
        /// <summary>
        /// 支付宝
        /// </summary>
        [Description("支付宝")]
        AliPay = 5
    }
}
