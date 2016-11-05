using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BPiaoBao.AppServices.DataContracts.Cashbag
{
   public class FinancialAccountDto
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
       public IList<CurrentFinancialProductDto> FinancialProducts { get; set; }
    }
}
