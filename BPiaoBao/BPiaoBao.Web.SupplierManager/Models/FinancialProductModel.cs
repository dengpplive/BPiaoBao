using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using BPiaoBao.AppServices.DataContracts.Cashbag;

namespace BPiaoBao.Web.SupplierManager.Models
{
    public class FinancialProductModel : FinancialProductDto
    {
        /// <summary>
        /// 用于购买当前理财产品的金额
        /// </summary>
        public decimal Money { get; set; }

        /// <summary>
        /// 用于购买当前理财产品的支付密码
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        /// 是否提前转出显示字段
        /// </summary>
        public bool CanSettleInAdvanceText { get; set; }

    }
}