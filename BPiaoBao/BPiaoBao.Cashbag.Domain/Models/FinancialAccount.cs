using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.Cashbag.Domain.Models
{
   public class FinancialAccount
    {
        /// <summary>
        /// 理财金额
        /// </summary>
       public decimal FinancialMoney
       {
           get;
           set;
           //get
           //{
           //    return this.FinancialProducts.Sum(p => p.FinancialMoney);
           //}
       }
       public IList<CurrentFinancialProduct> FinancialProducts { get; set; }
    }
}
